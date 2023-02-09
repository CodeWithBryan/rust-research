using System;
using ConVar;
using UnityEngine;

// Token: 0x02000548 RID: 1352
public class NPCSpawner : SpawnGroup
{
	// Token: 0x06002926 RID: 10534 RVA: 0x000FA1EE File Offset: 0x000F83EE
	public override void SpawnInitial()
	{
		this.fillOnSpawn = this.shouldFillOnSpawn;
		if (this.WaitingForNavMesh())
		{
			base.Invoke(new Action(this.LateSpawn), 10f);
			return;
		}
		base.SpawnInitial();
	}

	// Token: 0x06002927 RID: 10535 RVA: 0x000FA222 File Offset: 0x000F8422
	public bool WaitingForNavMesh()
	{
		if (this.monumentNavMesh != null)
		{
			return this.monumentNavMesh.IsBuilding;
		}
		return !DungeonNavmesh.NavReady() || !AI.move;
	}

	// Token: 0x06002928 RID: 10536 RVA: 0x000FA24F File Offset: 0x000F844F
	public void LateSpawn()
	{
		if (!this.WaitingForNavMesh())
		{
			this.SpawnInitial();
			Debug.Log("Navmesh complete, spawning");
			return;
		}
		base.Invoke(new Action(this.LateSpawn), 5f);
	}

	// Token: 0x06002929 RID: 10537 RVA: 0x000FA284 File Offset: 0x000F8484
	protected override void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
	{
		base.PostSpawnProcess(entity, spawnPoint);
		BaseNavigator component = entity.GetComponent<BaseNavigator>();
		HumanNPC humanNPC;
		if (this.AdditionalLOSBlockingLayer != 0 && entity != null && (humanNPC = (entity as HumanNPC)) != null)
		{
			humanNPC.AdditionalLosBlockingLayer = this.AdditionalLOSBlockingLayer;
		}
		HumanNPC humanNPC2 = entity as HumanNPC;
		if (humanNPC2 != null)
		{
			if (this.Loadouts != null && this.Loadouts.Length != 0)
			{
				humanNPC2.EquipLoadout(this.Loadouts);
			}
			this.ModifyHumanBrainStats(humanNPC2.Brain);
		}
		if (this.VirtualInfoZone != null)
		{
			if (this.VirtualInfoZone.Virtual)
			{
				NPCPlayer npcplayer = entity as NPCPlayer;
				if (npcplayer != null)
				{
					npcplayer.VirtualInfoZone = this.VirtualInfoZone;
					if (humanNPC2 != null)
					{
						humanNPC2.VirtualInfoZone.RegisterSleepableEntity(humanNPC2.Brain);
					}
				}
			}
			else
			{
				Debug.LogError("NPCSpawner trying to set a virtual info zone without the Virtual property!");
			}
		}
		if (component != null)
		{
			component.Path = this.Path;
			component.AStarGraph = this.AStarGraph;
		}
	}

	// Token: 0x0600292A RID: 10538 RVA: 0x000FA380 File Offset: 0x000F8580
	private void ModifyHumanBrainStats(BaseAIBrain brain)
	{
		if (!this.UseStatModifiers)
		{
			return;
		}
		if (brain == null)
		{
			return;
		}
		brain.SenseRange = this.SenseRange;
		brain.TargetLostRange *= this.TargetLostRange;
		brain.AttackRangeMultiplier = this.AttackRangeMultiplier;
		brain.ListenRange = this.ListenRange;
		brain.CheckLOS = this.CheckLOS;
		if (this.CanUseHealingItemsChance > 0f)
		{
			brain.CanUseHealingItems = (UnityEngine.Random.Range(0f, 1f) <= this.CanUseHealingItemsChance);
		}
	}

	// Token: 0x04002162 RID: 8546
	public int AdditionalLOSBlockingLayer;

	// Token: 0x04002163 RID: 8547
	public MonumentNavMesh monumentNavMesh;

	// Token: 0x04002164 RID: 8548
	public bool shouldFillOnSpawn;

	// Token: 0x04002165 RID: 8549
	[Header("InfoZone Config")]
	public AIInformationZone VirtualInfoZone;

	// Token: 0x04002166 RID: 8550
	[Header("Navigator Config")]
	public AIMovePointPath Path;

	// Token: 0x04002167 RID: 8551
	public BasePath AStarGraph;

	// Token: 0x04002168 RID: 8552
	[Header("Human Stat Replacements")]
	public bool UseStatModifiers;

	// Token: 0x04002169 RID: 8553
	public float SenseRange = 30f;

	// Token: 0x0400216A RID: 8554
	public bool CheckLOS = true;

	// Token: 0x0400216B RID: 8555
	public float TargetLostRange = 50f;

	// Token: 0x0400216C RID: 8556
	public float AttackRangeMultiplier = 1f;

	// Token: 0x0400216D RID: 8557
	public float ListenRange = 10f;

	// Token: 0x0400216E RID: 8558
	public float CanUseHealingItemsChance;

	// Token: 0x0400216F RID: 8559
	[Header("Loadout Replacements")]
	public PlayerInventoryProperties[] Loadouts;
}
