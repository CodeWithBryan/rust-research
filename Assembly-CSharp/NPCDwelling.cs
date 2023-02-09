using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000176 RID: 374
public class NPCDwelling : BaseEntity
{
	// Token: 0x060016BE RID: 5822 RVA: 0x000ABD80 File Offset: 0x000A9F80
	public override void ServerInit()
	{
		base.ServerInit();
		this.UpdateInformationZone(false);
		if (this.npcSpawner != null && UnityEngine.Random.Range(0f, 1f) <= this.NPCSpawnChance)
		{
			this.npcSpawner.SpawnInitial();
		}
		SpawnGroup[] array = this.spawnGroups;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SpawnInitial();
		}
	}

	// Token: 0x060016BF RID: 5823 RVA: 0x000ABDE7 File Offset: 0x000A9FE7
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			this.CleanupSpawned();
		}
		base.DestroyShared();
		if (base.isServer)
		{
			this.UpdateInformationZone(true);
		}
	}

	// Token: 0x060016C0 RID: 5824 RVA: 0x000ABE0C File Offset: 0x000AA00C
	public bool ValidateAIPoint(Vector3 pos)
	{
		base.gameObject.SetActive(false);
		bool result = !GamePhysics.CheckSphere(pos + Vector3.up * 0.6f, 0.5f, 65537, QueryTriggerInteraction.UseGlobal);
		base.gameObject.SetActive(true);
		return result;
	}

	// Token: 0x060016C1 RID: 5825 RVA: 0x000ABE5C File Offset: 0x000AA05C
	public void UpdateInformationZone(bool remove)
	{
		AIInformationZone forPoint = AIInformationZone.GetForPoint(base.transform.position, true);
		if (forPoint == null)
		{
			return;
		}
		if (remove)
		{
			forPoint.RemoveDynamicAIPoints(this.movePoints, this.coverPoints);
			return;
		}
		forPoint.AddDynamicAIPoints(this.movePoints, this.coverPoints, new Func<Vector3, bool>(this.ValidateAIPoint));
	}

	// Token: 0x060016C2 RID: 5826 RVA: 0x000ABEB9 File Offset: 0x000AA0B9
	public void CheckDespawn()
	{
		if (this.PlayersNearby())
		{
			return;
		}
		if (this.npcSpawner && this.npcSpawner.currentPopulation > 0)
		{
			return;
		}
		this.CleanupSpawned();
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060016C3 RID: 5827 RVA: 0x000ABEF0 File Offset: 0x000AA0F0
	public void CleanupSpawned()
	{
		if (this.spawnGroups != null)
		{
			SpawnGroup[] array = this.spawnGroups;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Clear();
			}
		}
		if (this.npcSpawner)
		{
			this.npcSpawner.Clear();
		}
	}

	// Token: 0x060016C4 RID: 5828 RVA: 0x000ABF3C File Offset: 0x000AA13C
	public bool PlayersNearby()
	{
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Vis.Entities<BasePlayer>(base.transform.position, this.TimeoutPlayerCheckRadius(), list, 131072, QueryTriggerInteraction.Collide);
		bool result = false;
		foreach (BasePlayer basePlayer in list)
		{
			if (!basePlayer.IsSleeping() && basePlayer.IsAlive())
			{
				result = true;
				break;
			}
		}
		Pool.FreeList<BasePlayer>(ref list);
		return result;
	}

	// Token: 0x060016C5 RID: 5829 RVA: 0x0004A2DA File Offset: 0x000484DA
	public virtual float TimeoutPlayerCheckRadius()
	{
		return 10f;
	}

	// Token: 0x04000FCE RID: 4046
	public NPCSpawner npcSpawner;

	// Token: 0x04000FCF RID: 4047
	public float NPCSpawnChance = 1f;

	// Token: 0x04000FD0 RID: 4048
	public SpawnGroup[] spawnGroups;

	// Token: 0x04000FD1 RID: 4049
	public AIMovePoint[] movePoints;

	// Token: 0x04000FD2 RID: 4050
	public AICoverPoint[] coverPoints;
}
