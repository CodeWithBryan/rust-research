using System;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
	// Token: 0x02000AF8 RID: 2808
	[DefaultExecutionOrder(-103)]
	public class AiManager : SingletonComponent<AiManager>, IServerComponent
	{
		// Token: 0x0600437D RID: 17277 RVA: 0x000059DD File Offset: 0x00003BDD
		internal void OnEnableAgency()
		{
		}

		// Token: 0x0600437E RID: 17278 RVA: 0x000059DD File Offset: 0x00003BDD
		internal void OnDisableAgency()
		{
		}

		// Token: 0x0600437F RID: 17279 RVA: 0x000059DD File Offset: 0x00003BDD
		internal void UpdateAgency()
		{
		}

		// Token: 0x06004380 RID: 17280 RVA: 0x00187828 File Offset: 0x00185A28
		internal void OnEnableCover()
		{
			if (this.coverPointVolumeGrid == null)
			{
				Vector3 size = TerrainMeta.Size;
				this.coverPointVolumeGrid = new WorldSpaceGrid<CoverPointVolume>(size.x, this.CoverPointVolumeCellSize);
			}
		}

		// Token: 0x06004381 RID: 17281 RVA: 0x0018785C File Offset: 0x00185A5C
		internal void OnDisableCover()
		{
			if (this.coverPointVolumeGrid == null || this.coverPointVolumeGrid.Cells == null)
			{
				return;
			}
			for (int i = 0; i < this.coverPointVolumeGrid.Cells.Length; i++)
			{
				UnityEngine.Object.Destroy(this.coverPointVolumeGrid.Cells[i]);
			}
		}

		// Token: 0x06004382 RID: 17282 RVA: 0x001878AC File Offset: 0x00185AAC
		public static CoverPointVolume CreateNewCoverVolume(Vector3 point, Transform coverPointGroup)
		{
			if (SingletonComponent<AiManager>.Instance != null && SingletonComponent<AiManager>.Instance.enabled && SingletonComponent<AiManager>.Instance.UseCover)
			{
				CoverPointVolume coverPointVolume = SingletonComponent<AiManager>.Instance.GetCoverVolumeContaining(point);
				if (coverPointVolume == null)
				{
					Vector2i vector2i = SingletonComponent<AiManager>.Instance.coverPointVolumeGrid.WorldToGridCoords(point);
					if (SingletonComponent<AiManager>.Instance.cpvPrefab != null)
					{
						coverPointVolume = UnityEngine.Object.Instantiate<CoverPointVolume>(SingletonComponent<AiManager>.Instance.cpvPrefab);
					}
					else
					{
						coverPointVolume = new GameObject("CoverPointVolume").AddComponent<CoverPointVolume>();
					}
					coverPointVolume.transform.localPosition = default(Vector3);
					coverPointVolume.transform.position = SingletonComponent<AiManager>.Instance.coverPointVolumeGrid.GridToWorldCoords(vector2i) + Vector3.up * point.y;
					coverPointVolume.transform.localScale = new Vector3(SingletonComponent<AiManager>.Instance.CoverPointVolumeCellSize, SingletonComponent<AiManager>.Instance.CoverPointVolumeCellHeight, SingletonComponent<AiManager>.Instance.CoverPointVolumeCellSize);
					coverPointVolume.CoverLayerMask = SingletonComponent<AiManager>.Instance.DynamicCoverPointVolumeLayerMask;
					coverPointVolume.CoverPointRayLength = SingletonComponent<AiManager>.Instance.CoverPointRayLength;
					SingletonComponent<AiManager>.Instance.coverPointVolumeGrid[vector2i] = coverPointVolume;
					coverPointVolume.GenerateCoverPoints(coverPointGroup);
				}
				return coverPointVolume;
			}
			return null;
		}

		// Token: 0x06004383 RID: 17283 RVA: 0x001879F4 File Offset: 0x00185BF4
		public CoverPointVolume GetCoverVolumeContaining(Vector3 point)
		{
			if (this.coverPointVolumeGrid == null)
			{
				return null;
			}
			Vector2i cellCoords = this.coverPointVolumeGrid.WorldToGridCoords(point);
			return this.coverPointVolumeGrid[cellCoords];
		}

		// Token: 0x170005E6 RID: 1510
		// (get) Token: 0x06004385 RID: 17285 RVA: 0x00187A2C File Offset: 0x00185C2C
		// (set) Token: 0x06004384 RID: 17284 RVA: 0x00187A24 File Offset: 0x00185C24
		[ServerVar(Help = "The maximum amount of nodes processed each frame in the asynchronous pathfinding process. Increasing this value will cause the paths to be processed faster, but can cause some hiccups in frame rate. Default value is 100, a good range for tuning is between 50 and 500.")]
		public static int pathfindingIterationsPerFrame
		{
			get
			{
				return NavMesh.pathfindingIterationsPerFrame;
			}
			set
			{
				NavMesh.pathfindingIterationsPerFrame = value;
			}
		}

		// Token: 0x170005E7 RID: 1511
		// (get) Token: 0x06004386 RID: 17286 RVA: 0x00003A54 File Offset: 0x00001C54
		public bool repeat
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004387 RID: 17287 RVA: 0x00187A33 File Offset: 0x00185C33
		public void Initialize()
		{
			this.OnEnableAgency();
			if (this.UseCover)
			{
				this.OnEnableCover();
			}
		}

		// Token: 0x06004388 RID: 17288 RVA: 0x00187A49 File Offset: 0x00185C49
		private void OnDisable()
		{
			if (Application.isQuitting)
			{
				return;
			}
			this.OnDisableAgency();
			if (this.UseCover)
			{
				this.OnDisableCover();
			}
		}

		// Token: 0x06004389 RID: 17289 RVA: 0x00187A67 File Offset: 0x00185C67
		public float? ExecuteUpdate(float deltaTime, float nextInterval)
		{
			if (AiManager.nav_disable)
			{
				return new float?(nextInterval);
			}
			this.UpdateAgency();
			return new float?(UnityEngine.Random.value + 1f);
		}

		// Token: 0x0600438A RID: 17290 RVA: 0x00187A90 File Offset: 0x00185C90
		private static bool InterestedInPlayersOnly(BaseEntity entity)
		{
			BasePlayer basePlayer = entity as BasePlayer;
			return !(basePlayer == null) && !basePlayer.IsSleeping() && basePlayer.IsConnected;
		}

		// Token: 0x04003BFB RID: 15355
		[Header("Cover System")]
		[SerializeField]
		public bool UseCover = true;

		// Token: 0x04003BFC RID: 15356
		public float CoverPointVolumeCellSize = 20f;

		// Token: 0x04003BFD RID: 15357
		public float CoverPointVolumeCellHeight = 8f;

		// Token: 0x04003BFE RID: 15358
		public float CoverPointRayLength = 1f;

		// Token: 0x04003BFF RID: 15359
		public CoverPointVolume cpvPrefab;

		// Token: 0x04003C00 RID: 15360
		[SerializeField]
		public LayerMask DynamicCoverPointVolumeLayerMask;

		// Token: 0x04003C01 RID: 15361
		private WorldSpaceGrid<CoverPointVolume> coverPointVolumeGrid;

		// Token: 0x04003C02 RID: 15362
		[ServerVar(Help = "If true we'll wait for the navmesh to generate before completely starting the server. This might cause your server to hitch and lag as it generates in the background.")]
		public static bool nav_wait = true;

		// Token: 0x04003C03 RID: 15363
		[ServerVar(Help = "If set to true the navmesh won't generate.. which means Ai that uses the navmesh won't be able to move")]
		public static bool nav_disable = false;

		// Token: 0x04003C04 RID: 15364
		[ServerVar(Help = "If set to true, npcs will attempt to place themselves on the navmesh if not on a navmesh when set destination is called.")]
		public static bool setdestination_navmesh_failsafe = false;

		// Token: 0x04003C05 RID: 15365
		[ServerVar(Help = "If ai_dormant is true, any npc outside the range of players will render itself dormant and take up less resources, but wildlife won't simulate as well.")]
		public static bool ai_dormant = true;

		// Token: 0x04003C06 RID: 15366
		[ServerVar(Help = "If an agent is beyond this distance to a player, it's flagged for becoming dormant.")]
		public static float ai_to_player_distance_wakeup_range = 160f;

		// Token: 0x04003C07 RID: 15367
		[ServerVar(Help = "nav_obstacles_carve_state defines which obstacles can carve the terrain. 0 - No carving, 1 - Only player construction carves, 2 - All obstacles carve.")]
		public static int nav_obstacles_carve_state = 2;

		// Token: 0x04003C08 RID: 15368
		[ServerVar(Help = "ai_dormant_max_wakeup_per_tick defines the maximum number of dormant agents we will wake up in a single tick. (default: 30)")]
		public static int ai_dormant_max_wakeup_per_tick = 30;

		// Token: 0x04003C09 RID: 15369
		[ServerVar(Help = "ai_htn_player_tick_budget defines the maximum amount of milliseconds ticking htn player agents are allowed to consume. (default: 4 ms)")]
		public static float ai_htn_player_tick_budget = 4f;

		// Token: 0x04003C0A RID: 15370
		[ServerVar(Help = "ai_htn_player_junkpile_tick_budget defines the maximum amount of milliseconds ticking htn player junkpile agents are allowed to consume. (default: 4 ms)")]
		public static float ai_htn_player_junkpile_tick_budget = 4f;

		// Token: 0x04003C0B RID: 15371
		[ServerVar(Help = "ai_htn_animal_tick_budget defines the maximum amount of milliseconds ticking htn animal agents are allowed to consume. (default: 4 ms)")]
		public static float ai_htn_animal_tick_budget = 4f;

		// Token: 0x04003C0C RID: 15372
		[ServerVar(Help = "If ai_htn_use_agency_tick is true, the ai manager's agency system will tick htn agents at the ms budgets defined in ai_htn_player_tick_budget and ai_htn_animal_tick_budget. If it's false, each agent registers with the invoke system individually, with no frame-budget restrictions. (default: true)")]
		public static bool ai_htn_use_agency_tick = true;

		// Token: 0x04003C0D RID: 15373
		private readonly BasePlayer[] playerVicinityQuery = new BasePlayer[1];

		// Token: 0x04003C0E RID: 15374
		private readonly Func<BasePlayer, bool> filter = new Func<BasePlayer, bool>(AiManager.InterestedInPlayersOnly);
	}
}
