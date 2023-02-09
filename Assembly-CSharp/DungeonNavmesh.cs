using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ConVar;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020001F2 RID: 498
public class DungeonNavmesh : FacepunchBehaviour, IServerComponent
{
	// Token: 0x06001A0B RID: 6667 RVA: 0x000B978C File Offset: 0x000B798C
	public static bool NavReady()
	{
		if (DungeonNavmesh.Instances == null || DungeonNavmesh.Instances.Count == 0)
		{
			return true;
		}
		using (List<DungeonNavmesh>.Enumerator enumerator = DungeonNavmesh.Instances.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsBuilding)
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x170001FE RID: 510
	// (get) Token: 0x06001A0C RID: 6668 RVA: 0x000B97FC File Offset: 0x000B79FC
	public bool IsBuilding
	{
		get
		{
			return !this.HasBuildOperationStarted || this.BuildingOperation != null;
		}
	}

	// Token: 0x06001A0D RID: 6669 RVA: 0x000B9814 File Offset: 0x000B7A14
	private void OnEnable()
	{
		this.agentTypeId = NavMesh.GetSettingsByIndex(this.NavMeshAgentTypeIndex).agentTypeID;
		this.NavMeshData = new NavMeshData(this.agentTypeId);
		this.sources = new List<NavMeshBuildSource>();
		this.defaultArea = NavMesh.GetAreaFromName(this.DefaultAreaName);
		base.InvokeRepeating(new Action(this.FinishBuildingNavmesh), 0f, 1f);
		DungeonNavmesh.Instances.Add(this);
	}

	// Token: 0x06001A0E RID: 6670 RVA: 0x000B988E File Offset: 0x000B7A8E
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		base.CancelInvoke(new Action(this.FinishBuildingNavmesh));
		this.NavMeshDataInstance.Remove();
		DungeonNavmesh.Instances.Remove(this);
	}

	// Token: 0x06001A0F RID: 6671 RVA: 0x000B98C4 File Offset: 0x000B7AC4
	[ContextMenu("Update Monument Nav Mesh")]
	public void UpdateNavMeshAsync()
	{
		if (this.HasBuildOperationStarted)
		{
			return;
		}
		if (AiManager.nav_disable || !AI.npc_enable)
		{
			return;
		}
		float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
		UnityEngine.Debug.Log("Starting Dungeon Navmesh Build with " + this.sources.Count + " sources");
		NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(this.NavMeshAgentTypeIndex);
		settingsByIndex.overrideVoxelSize = true;
		settingsByIndex.voxelSize *= this.NavmeshResolutionModifier;
		this.BuildingOperation = NavMeshBuilder.UpdateNavMeshDataAsync(this.NavMeshData, settingsByIndex, this.sources, this.Bounds);
		this.BuildTimer.Reset();
		this.BuildTimer.Start();
		this.HasBuildOperationStarted = true;
		float num = UnityEngine.Time.realtimeSinceStartup - realtimeSinceStartup;
		if (num > 0.1f)
		{
			UnityEngine.Debug.LogWarning("Calling UpdateNavMesh took " + num);
		}
		this.NotifyInformationZonesOfCompletion();
	}

	// Token: 0x06001A10 RID: 6672 RVA: 0x000B99A4 File Offset: 0x000B7BA4
	public void NotifyInformationZonesOfCompletion()
	{
		foreach (AIInformationZone aiinformationZone in AIInformationZone.zones)
		{
			aiinformationZone.NavmeshBuildingComplete();
		}
	}

	// Token: 0x06001A11 RID: 6673 RVA: 0x000B99F4 File Offset: 0x000B7BF4
	public void SourcesCollected()
	{
		int count = this.sources.Count;
		UnityEngine.Debug.Log("Source count Pre cull : " + this.sources.Count);
		for (int i = this.sources.Count - 1; i >= 0; i--)
		{
			NavMeshBuildSource item = this.sources[i];
			Matrix4x4 transform = item.transform;
			Vector3 vector = new Vector3(transform[0, 3], transform[1, 3], transform[2, 3]);
			bool flag = false;
			using (List<AIInformationZone>.Enumerator enumerator = AIInformationZone.zones.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (Vector3Ex.Distance2D(enumerator.Current.ClosestPointTo(vector), vector) <= 50f)
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				this.sources.Remove(item);
			}
		}
		UnityEngine.Debug.Log(string.Concat(new object[]
		{
			"Source count post cull : ",
			this.sources.Count,
			" total removed : ",
			count - this.sources.Count
		}));
	}

	// Token: 0x06001A12 RID: 6674 RVA: 0x000B9B34 File Offset: 0x000B7D34
	public IEnumerator UpdateNavMeshAndWait()
	{
		if (this.HasBuildOperationStarted)
		{
			yield break;
		}
		if (AiManager.nav_disable || !AI.npc_enable)
		{
			yield break;
		}
		this.HasBuildOperationStarted = false;
		this.Bounds.center = base.transform.position;
		this.Bounds.size = new Vector3(1000000f, 100000f, 100000f);
		IEnumerator enumerator = NavMeshTools.CollectSourcesAsync(base.transform, this.LayerMask.value, this.NavMeshCollectGeometry, this.defaultArea, this.sources, new Action<List<NavMeshBuildSource>>(this.AppendModifierVolumes), new Action(this.UpdateNavMeshAsync));
		if (AiManager.nav_wait)
		{
			yield return enumerator;
		}
		else
		{
			base.StartCoroutine(enumerator);
		}
		if (!AiManager.nav_wait)
		{
			UnityEngine.Debug.Log("nav_wait is false, so we're not waiting for the navmesh to finish generating. This might cause your server to sputter while it's generating.");
			yield break;
		}
		int lastPct = 0;
		while (!this.HasBuildOperationStarted)
		{
			yield return CoroutineEx.waitForSecondsRealtime(0.25f);
		}
		while (this.BuildingOperation != null)
		{
			int num = (int)(this.BuildingOperation.progress * 100f);
			if (lastPct != num)
			{
				UnityEngine.Debug.LogFormat("{0}%", new object[]
				{
					num
				});
				lastPct = num;
			}
			yield return CoroutineEx.waitForSecondsRealtime(0.25f);
			this.FinishBuildingNavmesh();
		}
		yield break;
	}

	// Token: 0x06001A13 RID: 6675 RVA: 0x000B9B44 File Offset: 0x000B7D44
	private void AppendModifierVolumes(List<NavMeshBuildSource> sources)
	{
		foreach (NavMeshModifierVolume navMeshModifierVolume in NavMeshModifierVolume.activeModifiers)
		{
			if ((this.LayerMask & 1 << navMeshModifierVolume.gameObject.layer) != 0 && navMeshModifierVolume.AffectsAgentType(this.agentTypeId))
			{
				Vector3 vector = navMeshModifierVolume.transform.TransformPoint(navMeshModifierVolume.center);
				if (this.Bounds.Contains(vector))
				{
					Vector3 lossyScale = navMeshModifierVolume.transform.lossyScale;
					Vector3 size = new Vector3(navMeshModifierVolume.size.x * Mathf.Abs(lossyScale.x), navMeshModifierVolume.size.y * Mathf.Abs(lossyScale.y), navMeshModifierVolume.size.z * Mathf.Abs(lossyScale.z));
					sources.Add(new NavMeshBuildSource
					{
						shape = NavMeshBuildSourceShape.ModifierBox,
						transform = Matrix4x4.TRS(vector, navMeshModifierVolume.transform.rotation, Vector3.one),
						size = size,
						area = navMeshModifierVolume.area
					});
				}
			}
		}
	}

	// Token: 0x06001A14 RID: 6676 RVA: 0x000B9C9C File Offset: 0x000B7E9C
	public void FinishBuildingNavmesh()
	{
		if (this.BuildingOperation == null)
		{
			return;
		}
		if (!this.BuildingOperation.isDone)
		{
			return;
		}
		if (!this.NavMeshDataInstance.valid)
		{
			this.NavMeshDataInstance = NavMesh.AddNavMeshData(this.NavMeshData);
		}
		UnityEngine.Debug.Log(string.Format("Monument Navmesh Build took {0:0.00} seconds", this.BuildTimer.Elapsed.TotalSeconds));
		this.BuildingOperation = null;
	}

	// Token: 0x06001A15 RID: 6677 RVA: 0x000B9D0C File Offset: 0x000B7F0C
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta * new Color(1f, 1f, 1f, 0.5f);
		Gizmos.DrawCube(base.transform.position, this.Bounds.size);
	}

	// Token: 0x04001256 RID: 4694
	public int NavMeshAgentTypeIndex;

	// Token: 0x04001257 RID: 4695
	[Tooltip("The default area associated with the NavMeshAgent index.")]
	public string DefaultAreaName = "HumanNPC";

	// Token: 0x04001258 RID: 4696
	public float NavmeshResolutionModifier = 1.25f;

	// Token: 0x04001259 RID: 4697
	[Tooltip("Bounds which are auto calculated from CellSize * CellCount")]
	public Bounds Bounds;

	// Token: 0x0400125A RID: 4698
	public NavMeshData NavMeshData;

	// Token: 0x0400125B RID: 4699
	public NavMeshDataInstance NavMeshDataInstance;

	// Token: 0x0400125C RID: 4700
	public LayerMask LayerMask;

	// Token: 0x0400125D RID: 4701
	public NavMeshCollectGeometry NavMeshCollectGeometry;

	// Token: 0x0400125E RID: 4702
	public static List<DungeonNavmesh> Instances = new List<DungeonNavmesh>();

	// Token: 0x0400125F RID: 4703
	[ServerVar]
	public static bool use_baked_terrain_mesh = true;

	// Token: 0x04001260 RID: 4704
	private List<NavMeshBuildSource> sources;

	// Token: 0x04001261 RID: 4705
	private AsyncOperation BuildingOperation;

	// Token: 0x04001262 RID: 4706
	private bool HasBuildOperationStarted;

	// Token: 0x04001263 RID: 4707
	private Stopwatch BuildTimer = new Stopwatch();

	// Token: 0x04001264 RID: 4708
	private int defaultArea;

	// Token: 0x04001265 RID: 4709
	private int agentTypeId;
}
