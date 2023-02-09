using System;
using ConVar;
using UnityEngine.AI;

// Token: 0x020004C1 RID: 1217
public abstract class BuildingManager
{
	// Token: 0x06002720 RID: 10016 RVA: 0x000F1424 File Offset: 0x000EF624
	public BuildingManager.Building GetBuilding(uint buildingID)
	{
		BuildingManager.Building result = null;
		this.buildingDictionary.TryGetValue(buildingID, out result);
		return result;
	}

	// Token: 0x06002721 RID: 10017 RVA: 0x000F1444 File Offset: 0x000EF644
	public void Add(DecayEntity ent)
	{
		if (ent.buildingID == 0U)
		{
			if (!this.decayEntities.Contains(ent))
			{
				this.decayEntities.Add(ent);
			}
			return;
		}
		BuildingManager.Building building = this.GetBuilding(ent.buildingID);
		if (building == null)
		{
			building = this.CreateBuilding(ent.buildingID);
			this.buildingDictionary.Add(ent.buildingID, building);
		}
		building.Add(ent);
		building.Dirty();
	}

	// Token: 0x06002722 RID: 10018 RVA: 0x000F14B0 File Offset: 0x000EF6B0
	public void Remove(DecayEntity ent)
	{
		if (ent.buildingID == 0U)
		{
			this.decayEntities.Remove(ent);
			return;
		}
		BuildingManager.Building building = this.GetBuilding(ent.buildingID);
		if (building == null)
		{
			return;
		}
		building.Remove(ent);
		if (building.IsEmpty())
		{
			this.buildingDictionary.Remove(ent.buildingID);
			this.DisposeBuilding(ref building);
			return;
		}
		building.Dirty();
	}

	// Token: 0x06002723 RID: 10019 RVA: 0x000F1514 File Offset: 0x000EF714
	public void Clear()
	{
		this.buildingDictionary.Clear();
	}

	// Token: 0x06002724 RID: 10020
	protected abstract BuildingManager.Building CreateBuilding(uint id);

	// Token: 0x06002725 RID: 10021
	protected abstract void DisposeBuilding(ref BuildingManager.Building building);

	// Token: 0x04001F82 RID: 8066
	public static ServerBuildingManager server = new ServerBuildingManager();

	// Token: 0x04001F83 RID: 8067
	protected ListHashSet<DecayEntity> decayEntities = new ListHashSet<DecayEntity>(8);

	// Token: 0x04001F84 RID: 8068
	protected ListDictionary<uint, BuildingManager.Building> buildingDictionary = new ListDictionary<uint, BuildingManager.Building>();

	// Token: 0x02000CCD RID: 3277
	public class Building
	{
		// Token: 0x06004D88 RID: 19848 RVA: 0x00198B84 File Offset: 0x00196D84
		public bool IsEmpty()
		{
			return !this.HasBuildingPrivileges() && !this.HasBuildingBlocks() && !this.HasDecayEntities();
		}

		// Token: 0x06004D89 RID: 19849 RVA: 0x00198BA8 File Offset: 0x00196DA8
		public BuildingPrivlidge GetDominatingBuildingPrivilege()
		{
			BuildingPrivlidge buildingPrivlidge = null;
			if (this.HasBuildingPrivileges())
			{
				for (int i = 0; i < this.buildingPrivileges.Count; i++)
				{
					BuildingPrivlidge buildingPrivlidge2 = this.buildingPrivileges[i];
					if (!(buildingPrivlidge2 == null) && buildingPrivlidge2.IsOlderThan(buildingPrivlidge))
					{
						buildingPrivlidge = buildingPrivlidge2;
					}
				}
			}
			return buildingPrivlidge;
		}

		// Token: 0x06004D8A RID: 19850 RVA: 0x00198BF7 File Offset: 0x00196DF7
		public bool HasBuildingPrivileges()
		{
			return this.buildingPrivileges != null && this.buildingPrivileges.Count > 0;
		}

		// Token: 0x06004D8B RID: 19851 RVA: 0x00198C11 File Offset: 0x00196E11
		public bool HasBuildingBlocks()
		{
			return this.buildingBlocks != null && this.buildingBlocks.Count > 0;
		}

		// Token: 0x06004D8C RID: 19852 RVA: 0x00198C2B File Offset: 0x00196E2B
		public bool HasDecayEntities()
		{
			return this.decayEntities != null && this.decayEntities.Count > 0;
		}

		// Token: 0x06004D8D RID: 19853 RVA: 0x00198C45 File Offset: 0x00196E45
		public void AddBuildingPrivilege(BuildingPrivlidge ent)
		{
			if (ent == null)
			{
				return;
			}
			if (!this.buildingPrivileges.Contains(ent))
			{
				this.buildingPrivileges.Add(ent);
			}
		}

		// Token: 0x06004D8E RID: 19854 RVA: 0x00198C6B File Offset: 0x00196E6B
		public void RemoveBuildingPrivilege(BuildingPrivlidge ent)
		{
			if (ent == null)
			{
				return;
			}
			this.buildingPrivileges.Remove(ent);
		}

		// Token: 0x06004D8F RID: 19855 RVA: 0x00198C84 File Offset: 0x00196E84
		public void AddBuildingBlock(BuildingBlock ent)
		{
			if (ent == null)
			{
				return;
			}
			if (!this.buildingBlocks.Contains(ent))
			{
				this.buildingBlocks.Add(ent);
				if (AI.nav_carve_use_building_optimization)
				{
					NavMeshObstacle component = ent.GetComponent<NavMeshObstacle>();
					if (component != null)
					{
						this.isNavMeshCarvingDirty = true;
						if (this.navmeshCarvers == null)
						{
							this.navmeshCarvers = new ListHashSet<NavMeshObstacle>(8);
						}
						this.navmeshCarvers.Add(component);
					}
				}
			}
		}

		// Token: 0x06004D90 RID: 19856 RVA: 0x00198CF4 File Offset: 0x00196EF4
		public void RemoveBuildingBlock(BuildingBlock ent)
		{
			if (ent == null)
			{
				return;
			}
			this.buildingBlocks.Remove(ent);
			if (AI.nav_carve_use_building_optimization && this.navmeshCarvers != null)
			{
				NavMeshObstacle component = ent.GetComponent<NavMeshObstacle>();
				if (component != null)
				{
					this.navmeshCarvers.Remove(component);
					if (this.navmeshCarvers.Count == 0)
					{
						this.navmeshCarvers = null;
					}
					this.isNavMeshCarvingDirty = true;
					if (this.navmeshCarvers == null)
					{
						BuildingManager.Building building = ent.GetBuilding();
						if (building != null)
						{
							int num = 2;
							BuildingManager.server.UpdateNavMeshCarver(building, ref num, 0);
						}
					}
				}
			}
		}

		// Token: 0x06004D91 RID: 19857 RVA: 0x00198D81 File Offset: 0x00196F81
		public void AddDecayEntity(DecayEntity ent)
		{
			if (ent == null)
			{
				return;
			}
			if (!this.decayEntities.Contains(ent))
			{
				this.decayEntities.Add(ent);
			}
		}

		// Token: 0x06004D92 RID: 19858 RVA: 0x00198DA7 File Offset: 0x00196FA7
		public void RemoveDecayEntity(DecayEntity ent)
		{
			if (ent == null)
			{
				return;
			}
			this.decayEntities.Remove(ent);
		}

		// Token: 0x06004D93 RID: 19859 RVA: 0x00198DC0 File Offset: 0x00196FC0
		public void Add(DecayEntity ent)
		{
			this.AddDecayEntity(ent);
			this.AddBuildingBlock(ent as BuildingBlock);
			this.AddBuildingPrivilege(ent as BuildingPrivlidge);
		}

		// Token: 0x06004D94 RID: 19860 RVA: 0x00198DE1 File Offset: 0x00196FE1
		public void Remove(DecayEntity ent)
		{
			this.RemoveDecayEntity(ent);
			this.RemoveBuildingBlock(ent as BuildingBlock);
			this.RemoveBuildingPrivilege(ent as BuildingPrivlidge);
		}

		// Token: 0x06004D95 RID: 19861 RVA: 0x00198E04 File Offset: 0x00197004
		public void Dirty()
		{
			BuildingPrivlidge dominatingBuildingPrivilege = this.GetDominatingBuildingPrivilege();
			if (dominatingBuildingPrivilege != null)
			{
				dominatingBuildingPrivilege.BuildingDirty();
			}
		}

		// Token: 0x040043DD RID: 17373
		public uint ID;

		// Token: 0x040043DE RID: 17374
		public ListHashSet<BuildingPrivlidge> buildingPrivileges = new ListHashSet<BuildingPrivlidge>(8);

		// Token: 0x040043DF RID: 17375
		public ListHashSet<BuildingBlock> buildingBlocks = new ListHashSet<BuildingBlock>(8);

		// Token: 0x040043E0 RID: 17376
		public ListHashSet<DecayEntity> decayEntities = new ListHashSet<DecayEntity>(8);

		// Token: 0x040043E1 RID: 17377
		public NavMeshObstacle buildingNavMeshObstacle;

		// Token: 0x040043E2 RID: 17378
		public ListHashSet<NavMeshObstacle> navmeshCarvers;

		// Token: 0x040043E3 RID: 17379
		public bool isNavMeshCarvingDirty;

		// Token: 0x040043E4 RID: 17380
		public bool isNavMeshCarveOptimized;
	}
}
