using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000188 RID: 392
public class DynamicDungeon : BaseEntity, IMissionEntityListener
{
	// Token: 0x06001703 RID: 5891 RVA: 0x000AD050 File Offset: 0x000AB250
	public static void AddDungeon(DynamicDungeon newDungeon)
	{
		DynamicDungeon._dungeons.Add(newDungeon);
		Vector3 position = newDungeon.transform.position;
		if (position.y >= DynamicDungeon.nextDungeonPos.y)
		{
			DynamicDungeon.nextDungeonPos = position + Vector3.up * DynamicDungeon.dungeonSpacing;
		}
	}

	// Token: 0x06001704 RID: 5892 RVA: 0x000AD0A0 File Offset: 0x000AB2A0
	public static void RemoveDungeon(DynamicDungeon dungeon)
	{
		Vector3 position = dungeon.transform.position;
		if (DynamicDungeon._dungeons.Contains(dungeon))
		{
			DynamicDungeon._dungeons.Remove(dungeon);
		}
		DynamicDungeon.nextDungeonPos = position;
	}

	// Token: 0x06001705 RID: 5893 RVA: 0x000AD0CB File Offset: 0x000AB2CB
	public static Vector3 GetNextDungeonPoint()
	{
		if (DynamicDungeon.nextDungeonPos == Vector3.zero)
		{
			DynamicDungeon.nextDungeonPos = Vector3.one * 700f;
		}
		return DynamicDungeon.nextDungeonPos;
	}

	// Token: 0x06001706 RID: 5894 RVA: 0x000AD0F7 File Offset: 0x000AB2F7
	public IEnumerator UpdateNavMesh()
	{
		Debug.Log("Dungeon Building navmesh");
		yield return base.StartCoroutine(this.monumentNavMesh.UpdateNavMeshAndWait());
		Debug.Log("Dunngeon done!");
		yield break;
	}

	// Token: 0x06001707 RID: 5895 RVA: 0x000AD108 File Offset: 0x000AB308
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			SpawnGroup[] array = this.spawnGroups;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Clear();
			}
			if (this.exitPortal != null)
			{
				this.exitPortal.Kill(BaseNetworkable.DestroyMode.None);
			}
			DynamicDungeon.RemoveDungeon(this);
		}
		base.DestroyShared();
	}

	// Token: 0x06001708 RID: 5896 RVA: 0x000AD160 File Offset: 0x000AB360
	public override void ServerInit()
	{
		base.ServerInit();
		DynamicDungeon.AddDungeon(this);
		if (this.portalPrefab.isValid)
		{
			this.exitPortal = GameManager.server.CreateEntity(this.portalPrefab.resourcePath, this.portalSpawnPoint.position, this.portalSpawnPoint.rotation, true).GetComponent<BasePortal>();
			this.exitPortal.SetParent(this, true, false);
			this.exitPortal.Spawn();
		}
		if (this.doorPrefab.isValid)
		{
			this.doorInstance = GameManager.server.CreateEntity(this.doorPrefab.resourcePath, this.doorSpawnPoint.position, this.doorSpawnPoint.rotation, true).GetComponent<Door>();
			this.doorInstance.SetParent(this, true, false);
			this.doorInstance.Spawn();
		}
		this.MergeAIZones();
		base.StartCoroutine(this.UpdateNavMesh());
	}

	// Token: 0x06001709 RID: 5897 RVA: 0x000AD248 File Offset: 0x000AB448
	private void MergeAIZones()
	{
		if (!this.AutoMergeAIZones)
		{
			return;
		}
		List<AIInformationZone> list = base.GetComponentsInChildren<AIInformationZone>().ToList<AIInformationZone>();
		foreach (AIInformationZone aiinformationZone in list)
		{
			aiinformationZone.AddInitialPoints();
		}
		GameObject gameObject = new GameObject("AIZ");
		gameObject.transform.position = base.transform.position;
		AIInformationZone.Merge(list, gameObject).ShouldSleepAI = false;
		gameObject.transform.SetParent(base.transform);
	}

	// Token: 0x0600170A RID: 5898 RVA: 0x000AD2E8 File Offset: 0x000AB4E8
	public void MissionStarted(BasePlayer assignee, BaseMission.MissionInstance instance)
	{
		foreach (MissionEntity missionEntity in instance.createdEntities)
		{
			BunkerEntrance component = missionEntity.GetComponent<BunkerEntrance>();
			if (component != null)
			{
				BasePortal portalInstance = component.portalInstance;
				if (portalInstance)
				{
					portalInstance.targetPortal = this.exitPortal;
					this.exitPortal.targetPortal = portalInstance;
					Debug.Log("Dungeon portal linked...");
				}
			}
		}
	}

	// Token: 0x0600170B RID: 5899 RVA: 0x000059DD File Offset: 0x00003BDD
	public void MissionEnded(BasePlayer assignee, BaseMission.MissionInstance instance)
	{
	}

	// Token: 0x0400101A RID: 4122
	public Transform exitEntitySpawn;

	// Token: 0x0400101B RID: 4123
	public GameObjectRef exitEntity;

	// Token: 0x0400101C RID: 4124
	public string exitString;

	// Token: 0x0400101D RID: 4125
	public MonumentNavMesh monumentNavMesh;

	// Token: 0x0400101E RID: 4126
	private static List<DynamicDungeon> _dungeons = new List<DynamicDungeon>();

	// Token: 0x0400101F RID: 4127
	public GameObjectRef portalPrefab;

	// Token: 0x04001020 RID: 4128
	public Transform portalSpawnPoint;

	// Token: 0x04001021 RID: 4129
	public BasePortal exitPortal;

	// Token: 0x04001022 RID: 4130
	public GameObjectRef doorPrefab;

	// Token: 0x04001023 RID: 4131
	public Transform doorSpawnPoint;

	// Token: 0x04001024 RID: 4132
	public Door doorInstance;

	// Token: 0x04001025 RID: 4133
	public static Vector3 nextDungeonPos = Vector3.zero;

	// Token: 0x04001026 RID: 4134
	public static Vector3 dungeonStartPoint = Vector3.zero;

	// Token: 0x04001027 RID: 4135
	public static float dungeonSpacing = 50f;

	// Token: 0x04001028 RID: 4136
	public SpawnGroup[] spawnGroups;

	// Token: 0x04001029 RID: 4137
	public bool AutoMergeAIZones = true;
}
