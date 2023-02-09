using System;
using ConVar;
using UnityEngine;

// Token: 0x02000547 RID: 1351
public class JunkpileNPCSpawner : NPCSpawner
{
	// Token: 0x06002924 RID: 10532 RVA: 0x000FA1C8 File Offset: 0x000F83C8
	protected override void Spawn(int numToSpawn)
	{
		if (this.UseSpawnChance && UnityEngine.Random.value > AI.npc_junkpilespawn_chance)
		{
			return;
		}
		base.Spawn(numToSpawn);
	}

	// Token: 0x04002161 RID: 8545
	[Header("Junkpile NPC Spawner")]
	public bool UseSpawnChance;
}
