using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ConVar;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020001F4 RID: 500
public class MonumentNavMesh : FacepunchBehaviour, IServerComponent
{
	// Token: 0x17000200 RID: 512
	// (get) Token: 0x06001A21 RID: 6689 RVA: 0x000BA106 File Offset: 0x000B8306
	public bool IsBuilding
	{
		get
		{
			return !this.HasBuildOperationStarted || this.BuildingOperation != null;
		}
	}

	// Token: 0x06001A22 RID: 6690 RVA: 0x000BA11C File Offset: 0x000B831C
	private void OnEnable()
	{
		this.agentTypeId = NavMesh.GetSettingsByIndex(this.NavMeshAgentTypeIndex).agentTypeID;
		this.NavMeshData = new NavMeshData(this.agentTypeId);
		this.sources = new List<NavMeshBuildSource>();
		this.defaultArea = NavMesh.GetAreaFromName(this.DefaultAreaName);
		base.InvokeRepeating(new Action(this.FinishBuildingNavmesh), 0f, 1f);
	}

	// Token: 0x06001A23 RID: 6691 RVA: 0x000BA18B File Offset: 0x000B838B
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		base.CancelInvoke(new Action(this.FinishBuildingNavmesh));
		this.NavMeshDataInstance.Remove();
	}

	// Token: 0x06001A24 RID: 6692 RVA: 0x000BA1B4 File Offset: 0x000B83B4
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
		UnityEngine.Debug.Log("Starting Monument Navmesh Build with " + this.sources.Count + " sources");
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
		if (this.shouldNotifyAIZones)
		{
			this.NotifyInformationZonesOfCompletion();
		}
	}

	// Token: 0x06001A25 RID: 6693 RVA: 0x000BA29A File Offset: 0x000B849A
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
		if (!this.overrideAutoBounds)
		{
			this.Bounds.size = new Vector3((float)(this.CellSize * this.CellCount), (float)this.Height, (float)(this.CellSize * this.CellCount));
		}
		IEnumerator enumerator = NavMeshTools.CollectSourcesAsync(this.Bounds, this.LayerMask, this.NavMeshCollectGeometry, this.defaultArea, MonumentNavMesh.use_baked_terrain_mesh && !this.forceCollectTerrain, this.CellSize, this.sources, new Action<List<NavMeshBuildSource>>(this.AppendModifierVolumes), new Action(this.UpdateNavMeshAsync), this.CustomNavMeshRoot);
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

	// Token: 0x06001A26 RID: 6694 RVA: 0x000BA2AC File Offset: 0x000B84AC
	public void NotifyInformationZonesOfCompletion()
	{
		foreach (AIInformationZone aiinformationZone in AIInformationZone.zones)
		{
			aiinformationZone.NavmeshBuildingComplete();
		}
	}

	// Token: 0x06001A27 RID: 6695 RVA: 0x000BA2FC File Offset: 0x000B84FC
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

	// Token: 0x06001A28 RID: 6696 RVA: 0x000BA454 File Offset: 0x000B8654
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

	// Token: 0x06001A29 RID: 6697 RVA: 0x000BA4C4 File Offset: 0x000B86C4
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta * new Color(1f, 1f, 1f, 0.5f);
		Gizmos.DrawCube(base.transform.position + this.Bounds.center, this.Bounds.size);
	}

	// Token: 0x04001276 RID: 4726
	public int NavMeshAgentTypeIndex;

	// Token: 0x04001277 RID: 4727
	[Tooltip("The default area associated with the NavMeshAgent index.")]
	public string DefaultAreaName = "HumanNPC";

	// Token: 0x04001278 RID: 4728
	[Tooltip("How many cells to use squared")]
	public int CellCount = 1;

	// Token: 0x04001279 RID: 4729
	[Tooltip("The size of each cell for async object gathering")]
	public int CellSize = 80;

	// Token: 0x0400127A RID: 4730
	public int Height = 100;

	// Token: 0x0400127B RID: 4731
	public float NavmeshResolutionModifier = 0.5f;

	// Token: 0x0400127C RID: 4732
	[Tooltip("Use the bounds specified in editor instead of generating it from cellsize * cellcount")]
	public bool overrideAutoBounds;

	// Token: 0x0400127D RID: 4733
	[Tooltip("Bounds which are auto calculated from CellSize * CellCount")]
	public Bounds Bounds;

	// Token: 0x0400127E RID: 4734
	public NavMeshData NavMeshData;

	// Token: 0x0400127F RID: 4735
	public NavMeshDataInstance NavMeshDataInstance;

	// Token: 0x04001280 RID: 4736
	public LayerMask LayerMask;

	// Token: 0x04001281 RID: 4737
	public NavMeshCollectGeometry NavMeshCollectGeometry;

	// Token: 0x04001282 RID: 4738
	public bool forceCollectTerrain;

	// Token: 0x04001283 RID: 4739
	public bool shouldNotifyAIZones = true;

	// Token: 0x04001284 RID: 4740
	public Transform CustomNavMeshRoot;

	// Token: 0x04001285 RID: 4741
	[ServerVar]
	public static bool use_baked_terrain_mesh = true;

	// Token: 0x04001286 RID: 4742
	private List<NavMeshBuildSource> sources;

	// Token: 0x04001287 RID: 4743
	private AsyncOperation BuildingOperation;

	// Token: 0x04001288 RID: 4744
	private bool HasBuildOperationStarted;

	// Token: 0x04001289 RID: 4745
	private Stopwatch BuildTimer = new Stopwatch();

	// Token: 0x0400128A RID: 4746
	private int defaultArea;

	// Token: 0x0400128B RID: 4747
	private int agentTypeId;
}
