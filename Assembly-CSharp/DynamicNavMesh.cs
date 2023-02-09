using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020001F3 RID: 499
public class DynamicNavMesh : SingletonComponent<DynamicNavMesh>, IServerComponent
{
	// Token: 0x170001FF RID: 511
	// (get) Token: 0x06001A18 RID: 6680 RVA: 0x000B9D97 File Offset: 0x000B7F97
	public bool IsBuilding
	{
		get
		{
			return !this.HasBuildOperationStarted || this.BuildingOperation != null;
		}
	}

	// Token: 0x06001A19 RID: 6681 RVA: 0x000B9DAC File Offset: 0x000B7FAC
	private void OnEnable()
	{
		this.agentTypeId = NavMesh.GetSettingsByIndex(this.NavMeshAgentTypeIndex).agentTypeID;
		this.NavMeshData = new NavMeshData(this.agentTypeId);
		this.sources = new List<NavMeshBuildSource>();
		this.defaultArea = NavMesh.GetAreaFromName(this.DefaultAreaName);
		base.InvokeRepeating(new Action(this.FinishBuildingNavmesh), 0f, 1f);
	}

	// Token: 0x06001A1A RID: 6682 RVA: 0x000B9E1B File Offset: 0x000B801B
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		base.CancelInvoke(new Action(this.FinishBuildingNavmesh));
		this.NavMeshDataInstance.Remove();
	}

	// Token: 0x06001A1B RID: 6683 RVA: 0x000B9E44 File Offset: 0x000B8044
	[ContextMenu("Update Nav Mesh")]
	public void UpdateNavMeshAsync()
	{
		if (this.HasBuildOperationStarted)
		{
			return;
		}
		if (AiManager.nav_disable)
		{
			return;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		UnityEngine.Debug.Log("Starting Navmesh Build with " + this.sources.Count + " sources");
		NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(this.NavMeshAgentTypeIndex);
		settingsByIndex.overrideVoxelSize = true;
		settingsByIndex.voxelSize *= 2f;
		this.BuildingOperation = NavMeshBuilder.UpdateNavMeshDataAsync(this.NavMeshData, settingsByIndex, this.sources, this.Bounds);
		this.BuildTimer.Reset();
		this.BuildTimer.Start();
		this.HasBuildOperationStarted = true;
		float num = Time.realtimeSinceStartup - realtimeSinceStartup;
		if (num > 0.1f)
		{
			UnityEngine.Debug.LogWarning("Calling UpdateNavMesh took " + num);
		}
	}

	// Token: 0x06001A1C RID: 6684 RVA: 0x000B9F14 File Offset: 0x000B8114
	public IEnumerator UpdateNavMeshAndWait()
	{
		if (this.HasBuildOperationStarted)
		{
			yield break;
		}
		if (AiManager.nav_disable)
		{
			yield break;
		}
		this.HasBuildOperationStarted = false;
		this.Bounds.size = TerrainMeta.Size;
		NavMesh.pathfindingIterationsPerFrame = AiManager.pathfindingIterationsPerFrame;
		IEnumerator enumerator = NavMeshTools.CollectSourcesAsync(this.Bounds, this.LayerMask, this.NavMeshCollectGeometry, this.defaultArea, DynamicNavMesh.use_baked_terrain_mesh, this.AsyncTerrainNavMeshBakeCellSize, this.sources, new Action<List<NavMeshBuildSource>>(this.AppendModifierVolumes), new Action(this.UpdateNavMeshAsync), null);
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

	// Token: 0x06001A1D RID: 6685 RVA: 0x000B9F24 File Offset: 0x000B8124
	private void AppendModifierVolumes(List<NavMeshBuildSource> sources)
	{
		foreach (NavMeshModifierVolume navMeshModifierVolume in NavMeshModifierVolume.activeModifiers)
		{
			if ((this.LayerMask & 1 << navMeshModifierVolume.gameObject.layer) != 0 && navMeshModifierVolume.AffectsAgentType(this.agentTypeId))
			{
				Vector3 pos = navMeshModifierVolume.transform.TransformPoint(navMeshModifierVolume.center);
				Vector3 lossyScale = navMeshModifierVolume.transform.lossyScale;
				Vector3 size = new Vector3(navMeshModifierVolume.size.x * Mathf.Abs(lossyScale.x), navMeshModifierVolume.size.y * Mathf.Abs(lossyScale.y), navMeshModifierVolume.size.z * Mathf.Abs(lossyScale.z));
				sources.Add(new NavMeshBuildSource
				{
					shape = NavMeshBuildSourceShape.ModifierBox,
					transform = Matrix4x4.TRS(pos, navMeshModifierVolume.transform.rotation, Vector3.one),
					size = size,
					area = navMeshModifierVolume.area
				});
			}
		}
	}

	// Token: 0x06001A1E RID: 6686 RVA: 0x000BA068 File Offset: 0x000B8268
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
		UnityEngine.Debug.Log(string.Format("Navmesh Build took {0:0.00} seconds", this.BuildTimer.Elapsed.TotalSeconds));
		this.BuildingOperation = null;
	}

	// Token: 0x04001266 RID: 4710
	public int NavMeshAgentTypeIndex;

	// Token: 0x04001267 RID: 4711
	[Tooltip("The default area associated with the NavMeshAgent index.")]
	public string DefaultAreaName = "Walkable";

	// Token: 0x04001268 RID: 4712
	public int AsyncTerrainNavMeshBakeCellSize = 80;

	// Token: 0x04001269 RID: 4713
	public int AsyncTerrainNavMeshBakeCellHeight = 100;

	// Token: 0x0400126A RID: 4714
	public Bounds Bounds;

	// Token: 0x0400126B RID: 4715
	public NavMeshData NavMeshData;

	// Token: 0x0400126C RID: 4716
	public NavMeshDataInstance NavMeshDataInstance;

	// Token: 0x0400126D RID: 4717
	public LayerMask LayerMask;

	// Token: 0x0400126E RID: 4718
	public NavMeshCollectGeometry NavMeshCollectGeometry;

	// Token: 0x0400126F RID: 4719
	[ServerVar]
	public static bool use_baked_terrain_mesh;

	// Token: 0x04001270 RID: 4720
	private List<NavMeshBuildSource> sources;

	// Token: 0x04001271 RID: 4721
	private AsyncOperation BuildingOperation;

	// Token: 0x04001272 RID: 4722
	private bool HasBuildOperationStarted;

	// Token: 0x04001273 RID: 4723
	private Stopwatch BuildTimer = new Stopwatch();

	// Token: 0x04001274 RID: 4724
	private int defaultArea;

	// Token: 0x04001275 RID: 4725
	private int agentTypeId;
}
