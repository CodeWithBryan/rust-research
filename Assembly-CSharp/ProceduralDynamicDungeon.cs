using System;
using System.Collections;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x0200018C RID: 396
public class ProceduralDynamicDungeon : global::BaseEntity
{
	// Token: 0x06001728 RID: 5928 RVA: 0x000ADBEA File Offset: 0x000ABDEA
	public override void InitShared()
	{
		base.InitShared();
		ProceduralDynamicDungeon.dungeons.Add(this);
	}

	// Token: 0x06001729 RID: 5929 RVA: 0x000ADC00 File Offset: 0x000ABE00
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		foreach (ProceduralDungeonCell proceduralDungeonCell in this.spawnedCells)
		{
			EntityFlag_Toggle[] componentsInChildren = proceduralDungeonCell.GetComponentsInChildren<EntityFlag_Toggle>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].DoUpdate(this);
			}
		}
	}

	// Token: 0x0600172A RID: 5930 RVA: 0x000ADC70 File Offset: 0x000ABE70
	public global::BaseEntity GetExitPortal(bool serverSide)
	{
		return this.exitPortal.Get(serverSide);
	}

	// Token: 0x0600172B RID: 5931 RVA: 0x000ADC7E File Offset: 0x000ABE7E
	public override void DestroyShared()
	{
		ProceduralDynamicDungeon.dungeons.Remove(this);
		this.RetireAllCells();
		base.DestroyShared();
	}

	// Token: 0x0600172C RID: 5932 RVA: 0x000ADC98 File Offset: 0x000ABE98
	public bool ContainsAnyPlayers()
	{
		Bounds bounds = new Bounds(base.transform.position, new Vector3((float)this.gridResolution * this.gridSpacing, 20f, (float)this.gridResolution * this.gridSpacing));
		for (int i = 0; i < global::BasePlayer.activePlayerList.Count; i++)
		{
			global::BasePlayer basePlayer = global::BasePlayer.activePlayerList[i];
			if (bounds.Contains(basePlayer.transform.position))
			{
				return true;
			}
		}
		for (int j = 0; j < global::BasePlayer.sleepingPlayerList.Count; j++)
		{
			global::BasePlayer basePlayer2 = global::BasePlayer.sleepingPlayerList[j];
			if (bounds.Contains(basePlayer2.transform.position))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600172D RID: 5933 RVA: 0x000ADD50 File Offset: 0x000ABF50
	public void KillPlayers()
	{
		Bounds bounds = new Bounds(base.transform.position, new Vector3((float)this.gridResolution * this.gridSpacing, 20f, (float)this.gridResolution * this.gridSpacing));
		for (int i = 0; i < global::BasePlayer.activePlayerList.Count; i++)
		{
			global::BasePlayer basePlayer = global::BasePlayer.activePlayerList[i];
			if (bounds.Contains(basePlayer.transform.position))
			{
				basePlayer.Hurt(10000f, DamageType.Suicide, null, false);
			}
		}
		for (int j = 0; j < global::BasePlayer.sleepingPlayerList.Count; j++)
		{
			global::BasePlayer basePlayer2 = global::BasePlayer.sleepingPlayerList[j];
			if (bounds.Contains(basePlayer2.transform.position))
			{
				basePlayer2.Hurt(10000f, DamageType.Suicide, null, false);
			}
		}
	}

	// Token: 0x0600172E RID: 5934 RVA: 0x000ADE1E File Offset: 0x000AC01E
	internal override void DoServerDestroy()
	{
		this.KillPlayers();
		if (this.exitPortal.IsValid(true))
		{
			this.exitPortal.Get(true).Kill(global::BaseNetworkable.DestroyMode.None);
		}
		base.DoServerDestroy();
	}

	// Token: 0x0600172F RID: 5935 RVA: 0x000ADE4C File Offset: 0x000AC04C
	public override void ServerInit()
	{
		if (!Rust.Application.isLoadingSave)
		{
			this.baseseed = (this.seed = (uint)UnityEngine.Random.Range(0, 12345567));
			Debug.Log("Spawning dungeon with seed :" + (int)this.seed);
		}
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.DoGeneration();
			BasePortal component = GameManager.server.CreateEntity(this.exitPortalPrefab.resourcePath, this.entranceHack.exitPointHack.position, this.entranceHack.exitPointHack.rotation, true).GetComponent<BasePortal>();
			component.Spawn();
			this.exitPortal.Set(component);
		}
	}

	// Token: 0x06001730 RID: 5936 RVA: 0x000ADEF8 File Offset: 0x000AC0F8
	public void DoGeneration()
	{
		this.GenerateGrid();
		this.CreateAIZ();
		if (base.isServer)
		{
			Debug.Log("Server DoGeneration,calling routine update nav mesh");
			base.StartCoroutine(this.UpdateNavMesh());
		}
		base.Invoke(new Action(this.InitSpawnGroups), 1f);
	}

	// Token: 0x06001731 RID: 5937 RVA: 0x000ADF48 File Offset: 0x000AC148
	private void CreateAIZ()
	{
		AIInformationZone aiinformationZone = base.gameObject.AddComponent<AIInformationZone>();
		aiinformationZone.UseCalculatedCoverDistances = false;
		aiinformationZone.bounds.extents = new Vector3((float)this.gridResolution * this.gridSpacing * 0.75f, 10f, (float)this.gridResolution * this.gridSpacing * 0.75f);
		aiinformationZone.Init();
	}

	// Token: 0x06001732 RID: 5938 RVA: 0x000ADFA9 File Offset: 0x000AC1A9
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.DoGeneration();
	}

	// Token: 0x06001733 RID: 5939 RVA: 0x000ADFB7 File Offset: 0x000AC1B7
	public IEnumerator UpdateNavMesh()
	{
		Debug.Log("Dungeon Building navmesh");
		yield return base.StartCoroutine(this.monumentNavMesh.UpdateNavMeshAndWait());
		Debug.Log("Dungeon done!");
		yield break;
	}

	// Token: 0x06001734 RID: 5940 RVA: 0x000ADFC8 File Offset: 0x000AC1C8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.proceduralDungeon == null)
		{
			info.msg.proceduralDungeon = Pool.Get<ProceduralDungeon>();
		}
		info.msg.proceduralDungeon.seed = this.baseseed;
		info.msg.proceduralDungeon.exitPortalID = this.exitPortal.uid;
		info.msg.proceduralDungeon.mapOffset = this.mapOffset;
	}

	// Token: 0x06001735 RID: 5941 RVA: 0x000AE040 File Offset: 0x000AC240
	public BasePortal GetExitPortal()
	{
		return this.exitPortal.Get(true);
	}

	// Token: 0x06001736 RID: 5942 RVA: 0x000AE050 File Offset: 0x000AC250
	public void InitSpawnGroups()
	{
		foreach (ProceduralDungeonCell proceduralDungeonCell in this.spawnedCells)
		{
			if (!(this.entranceHack != null) || Vector3.Distance(this.entranceHack.transform.position, proceduralDungeonCell.transform.position) >= 20f)
			{
				SpawnGroup[] spawnGroups = proceduralDungeonCell.spawnGroups;
				for (int i = 0; i < spawnGroups.Length; i++)
				{
					spawnGroups[i].Spawn();
				}
			}
		}
	}

	// Token: 0x06001737 RID: 5943 RVA: 0x000AE0F0 File Offset: 0x000AC2F0
	public void CleanupSpawnGroups()
	{
		foreach (ProceduralDungeonCell proceduralDungeonCell in this.spawnedCells)
		{
			SpawnGroup[] spawnGroups = proceduralDungeonCell.spawnGroups;
			for (int i = 0; i < spawnGroups.Length; i++)
			{
				spawnGroups[i].Clear();
			}
		}
	}

	// Token: 0x06001738 RID: 5944 RVA: 0x000AE158 File Offset: 0x000AC358
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.proceduralDungeon != null)
		{
			this.baseseed = (this.seed = info.msg.proceduralDungeon.seed);
			this.exitPortal.uid = info.msg.proceduralDungeon.exitPortalID;
			this.mapOffset = info.msg.proceduralDungeon.mapOffset;
		}
	}

	// Token: 0x06001739 RID: 5945 RVA: 0x000AE1CC File Offset: 0x000AC3CC
	[ContextMenu("Test Grid")]
	[ExecuteInEditMode]
	public void GenerateGrid()
	{
		Vector3 a = base.transform.position - new Vector3((float)this.gridResolution * this.gridSpacing * 0.5f, 0f, (float)this.gridResolution * this.gridSpacing * 0.5f);
		this.RetireAllCells();
		this.grid = new bool[this.gridResolution * this.gridResolution];
		for (int i = 0; i < this.grid.Length; i++)
		{
			this.grid[i] = ((SeedRandom.Range(ref this.seed, 0, 2) == 0) ? true : false);
		}
		this.SetEntrance(3, 0);
		for (int j = 0; j < this.gridResolution; j++)
		{
			for (int k = 0; k < this.gridResolution; k++)
			{
				if (this.GetGridState(j, k) && !this.HasPathToEntrance(j, k))
				{
					this.SetGridState(j, k, false);
				}
			}
		}
		for (int l = 0; l < this.gridResolution; l++)
		{
			for (int m = 0; m < this.gridResolution; m++)
			{
				if (this.GetGridState(l, m))
				{
					bool gridState = this.GetGridState(l, m + 1);
					bool gridState2 = this.GetGridState(l, m - 1);
					bool gridState3 = this.GetGridState(l - 1, m);
					bool gridState4 = this.GetGridState(l + 1, m);
					bool flag = this.IsEntrance(l, m);
					GameObjectRef gameObjectRef = null;
					ProceduralDungeonCell x = null;
					if (x == null)
					{
						foreach (GameObjectRef gameObjectRef2 in this.cellPrefabReferences)
						{
							ProceduralDungeonCell component = gameObjectRef2.Get().GetComponent<ProceduralDungeonCell>();
							if (component.north == gridState && component.south == gridState2 && component.west == gridState3 && component.east == gridState4 && component.entrance == flag)
							{
								x = component;
								gameObjectRef = gameObjectRef2;
								break;
							}
						}
					}
					if (x != null)
					{
						ProceduralDungeonCell proceduralDungeonCell = this.CellInstantiate(gameObjectRef.resourcePath);
						proceduralDungeonCell.transform.position = a + new Vector3((float)l * this.gridSpacing, 0f, (float)m * this.gridSpacing);
						this.spawnedCells.Add(proceduralDungeonCell);
						proceduralDungeonCell.transform.SetParent(base.transform);
						if (proceduralDungeonCell.entrance && this.entranceHack == null)
						{
							this.entranceHack = proceduralDungeonCell;
						}
					}
				}
			}
		}
	}

	// Token: 0x0600173A RID: 5946 RVA: 0x000AE464 File Offset: 0x000AC664
	public ProceduralDungeonCell CellInstantiate(string path)
	{
		if (base.isServer)
		{
			return GameManager.server.CreatePrefab(path, true).GetComponent<ProceduralDungeonCell>();
		}
		return null;
	}

	// Token: 0x0600173B RID: 5947 RVA: 0x000AE481 File Offset: 0x000AC681
	public void RetireCell(GameObject cell)
	{
		if (cell == null)
		{
			return;
		}
		if (base.isServer)
		{
			GameManager.server.Retire(cell);
		}
	}

	// Token: 0x0600173C RID: 5948 RVA: 0x000AE4A0 File Offset: 0x000AC6A0
	public void RetireAllCells()
	{
		if (base.isServer)
		{
			this.CleanupSpawnGroups();
		}
		for (int i = this.spawnedCells.Count - 1; i >= 0; i--)
		{
			ProceduralDungeonCell proceduralDungeonCell = this.spawnedCells[i];
			if (proceduralDungeonCell)
			{
				this.RetireCell(proceduralDungeonCell.gameObject);
			}
		}
		this.spawnedCells.Clear();
	}

	// Token: 0x0600173D RID: 5949 RVA: 0x000AE500 File Offset: 0x000AC700
	public bool CanSeeEntrance(int x, int y, ref List<int> checkedCells)
	{
		int gridIndex = this.GetGridIndex(x, y);
		if (checkedCells.Contains(gridIndex))
		{
			return false;
		}
		checkedCells.Add(gridIndex);
		if (!this.GetGridState(x, y))
		{
			return false;
		}
		if (this.IsEntrance(x, y))
		{
			return true;
		}
		bool flag = this.CanSeeEntrance(x, y + 1, ref checkedCells);
		bool flag2 = this.CanSeeEntrance(x, y - 1, ref checkedCells);
		bool flag3 = this.CanSeeEntrance(x - 1, y, ref checkedCells);
		bool flag4 = this.CanSeeEntrance(x + 1, y, ref checkedCells);
		return flag || flag4 || flag3 || flag2;
	}

	// Token: 0x0600173E RID: 5950 RVA: 0x000AE578 File Offset: 0x000AC778
	public bool HasPathToEntrance(int x, int y)
	{
		List<int> list = new List<int>();
		bool result = this.CanSeeEntrance(x, y, ref list);
		list.Clear();
		return result;
	}

	// Token: 0x0600173F RID: 5951 RVA: 0x000AE59B File Offset: 0x000AC79B
	public bool CanFindEntrance(int x, int y)
	{
		new List<int>();
		this.GetGridState(x, y + 1);
		this.GetGridState(x, y - 1);
		this.GetGridState(x - 1, y);
		this.GetGridState(x + 1, y);
		return true;
	}

	// Token: 0x06001740 RID: 5952 RVA: 0x000AE5D0 File Offset: 0x000AC7D0
	public bool IsEntrance(int x, int y)
	{
		return this.GetGridIndex(x, y) == this.GetEntranceIndex();
	}

	// Token: 0x06001741 RID: 5953 RVA: 0x000AE5E2 File Offset: 0x000AC7E2
	public int GetEntranceIndex()
	{
		return this.GetGridIndex(3, 0);
	}

	// Token: 0x06001742 RID: 5954 RVA: 0x000AE5EC File Offset: 0x000AC7EC
	public void SetEntrance(int x, int y)
	{
		this.grid[this.GetGridIndex(x, y)] = true;
		this.grid[this.GetGridIndex(x, y + 1)] = true;
		this.grid[this.GetGridIndex(x - 1, y)] = false;
		this.grid[this.GetGridIndex(x + 1, y)] = false;
		this.grid[this.GetGridIndex(x, y + 2)] = true;
		this.grid[this.GetGridIndex(x + 1, y + 2)] = ((SeedRandom.Range(ref this.seed, 0, 1) == 1) ? true : false);
		this.grid[this.GetGridIndex(x + 2, y + 2)] = ((SeedRandom.Range(ref this.seed, 0, 1) == 1) ? true : false);
		this.grid[this.GetGridIndex(x, y + 3)] = true;
		this.grid[this.GetGridIndex(x, y + 4)] = true;
		this.grid[this.GetGridIndex(x - 1, y + 4)] = ((SeedRandom.Range(ref this.seed, 0, 1) == 1) ? true : false);
		this.grid[this.GetGridIndex(x - 2, y + 4)] = ((SeedRandom.Range(ref this.seed, 0, 1) == 1) ? true : false);
	}

	// Token: 0x06001743 RID: 5955 RVA: 0x000AE714 File Offset: 0x000AC914
	public void SetGridState(int x, int y, bool state)
	{
		int gridIndex = this.GetGridIndex(x, y);
		this.grid[gridIndex] = state;
	}

	// Token: 0x06001744 RID: 5956 RVA: 0x000AE734 File Offset: 0x000AC934
	public bool GetGridState(int x, int y)
	{
		return this.GetGridIndex(x, y) < this.grid.Length && x >= 0 && x < this.gridResolution && y >= 0 && y < this.gridResolution && this.grid[this.GetGridIndex(x, y)];
	}

	// Token: 0x06001745 RID: 5957 RVA: 0x000AE782 File Offset: 0x000AC982
	public int GetGridX(int index)
	{
		return index % this.gridResolution;
	}

	// Token: 0x06001746 RID: 5958 RVA: 0x000AE78C File Offset: 0x000AC98C
	public int GetGridY(int index)
	{
		return Mathf.FloorToInt((float)index / (float)this.gridResolution);
	}

	// Token: 0x06001747 RID: 5959 RVA: 0x000AE79D File Offset: 0x000AC99D
	public int GetGridIndex(int x, int y)
	{
		return y * this.gridResolution + x;
	}

	// Token: 0x04001043 RID: 4163
	public int gridResolution = 6;

	// Token: 0x04001044 RID: 4164
	public float gridSpacing = 12f;

	// Token: 0x04001045 RID: 4165
	public bool[] grid;

	// Token: 0x04001046 RID: 4166
	public List<GameObjectRef> cellPrefabReferences = new List<GameObjectRef>();

	// Token: 0x04001047 RID: 4167
	public List<ProceduralDungeonCell> spawnedCells = new List<ProceduralDungeonCell>();

	// Token: 0x04001048 RID: 4168
	public EnvironmentVolume envVolume;

	// Token: 0x04001049 RID: 4169
	public MonumentNavMesh monumentNavMesh;

	// Token: 0x0400104A RID: 4170
	public GameObjectRef exitPortalPrefab;

	// Token: 0x0400104B RID: 4171
	private EntityRef<BasePortal> exitPortal;

	// Token: 0x0400104C RID: 4172
	public TriggerRadiation exitRadiation;

	// Token: 0x0400104D RID: 4173
	public uint seed;

	// Token: 0x0400104E RID: 4174
	public uint baseseed;

	// Token: 0x0400104F RID: 4175
	public Vector3 mapOffset = Vector3.zero;

	// Token: 0x04001050 RID: 4176
	public static readonly List<ProceduralDynamicDungeon> dungeons = new List<ProceduralDynamicDungeon>();

	// Token: 0x04001051 RID: 4177
	public ProceduralDungeonCell entranceHack;
}
