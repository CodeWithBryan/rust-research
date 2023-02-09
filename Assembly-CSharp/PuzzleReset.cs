using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x020004B6 RID: 1206
public class PuzzleReset : FacepunchBehaviour
{
	// Token: 0x060026D8 RID: 9944 RVA: 0x000F003E File Offset: 0x000EE23E
	public float GetResetSpacing()
	{
		return this.timeBetweenResets * (this.scaleWithServerPopulation ? (1f - SpawnHandler.PlayerLerp(Spawn.min_rate, Spawn.max_rate)) : 1f);
	}

	// Token: 0x060026D9 RID: 9945 RVA: 0x000F006B File Offset: 0x000EE26B
	public void Start()
	{
		if (this.timeBetweenResets != float.PositiveInfinity)
		{
			this.ResetTimer();
		}
	}

	// Token: 0x060026DA RID: 9946 RVA: 0x000F0080 File Offset: 0x000EE280
	public void ResetTimer()
	{
		this.resetTimeElapsed = 0f;
		base.CancelInvoke(new Action(this.ResetTick));
		base.InvokeRandomized(new Action(this.ResetTick), UnityEngine.Random.Range(0f, 1f), this.resetTickTime, 0.5f);
	}

	// Token: 0x060026DB RID: 9947 RVA: 0x000F00D6 File Offset: 0x000EE2D6
	public bool PassesResetCheck()
	{
		if (!this.playersBlockReset)
		{
			return true;
		}
		if (this.CheckSleepingAIZForPlayers)
		{
			return this.AIZSleeping();
		}
		return !this.PlayersWithinDistance();
	}

	// Token: 0x060026DC RID: 9948 RVA: 0x000F00FC File Offset: 0x000EE2FC
	private bool AIZSleeping()
	{
		if (this.zone != null)
		{
			if (!this.zone.PointInside(base.transform.position))
			{
				this.zone = AIInformationZone.GetForPoint(base.transform.position, true);
			}
		}
		else
		{
			this.zone = AIInformationZone.GetForPoint(base.transform.position, true);
		}
		return !(this.zone == null) && this.zone.Sleeping;
	}

	// Token: 0x060026DD RID: 9949 RVA: 0x000F017A File Offset: 0x000EE37A
	private bool PlayersWithinDistance()
	{
		return PuzzleReset.AnyPlayersWithinDistance(this.playerDetectionOrigin, this.playerDetectionRadius);
	}

	// Token: 0x060026DE RID: 9950 RVA: 0x000F0190 File Offset: 0x000EE390
	public static bool AnyPlayersWithinDistance(Transform origin, float radius)
	{
		foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
		{
			if (!basePlayer.IsSleeping() && basePlayer.IsAlive() && Vector3.Distance(basePlayer.transform.position, origin.position) < radius)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060026DF RID: 9951 RVA: 0x000F020C File Offset: 0x000EE40C
	public void ResetTick()
	{
		if (this.PassesResetCheck())
		{
			this.resetTimeElapsed += this.resetTickTime;
		}
		if (this.resetTimeElapsed > this.GetResetSpacing())
		{
			this.resetTimeElapsed = 0f;
			this.DoReset();
		}
	}

	// Token: 0x060026E0 RID: 9952 RVA: 0x000F0248 File Offset: 0x000EE448
	public void CleanupSleepers()
	{
		if (this.playerDetectionOrigin == null || BasePlayer.sleepingPlayerList == null)
		{
			return;
		}
		for (int i = BasePlayer.sleepingPlayerList.Count - 1; i >= 0; i--)
		{
			BasePlayer basePlayer = BasePlayer.sleepingPlayerList[i];
			if (!(basePlayer == null) && basePlayer.IsSleeping() && Vector3.Distance(basePlayer.transform.position, this.playerDetectionOrigin.position) <= this.playerDetectionRadius)
			{
				basePlayer.Hurt(1000f, DamageType.Suicide, basePlayer, false);
			}
		}
	}

	// Token: 0x060026E1 RID: 9953 RVA: 0x000F02D0 File Offset: 0x000EE4D0
	public void DoReset()
	{
		this.CleanupSleepers();
		IOEntity component = base.GetComponent<IOEntity>();
		if (component != null)
		{
			PuzzleReset.ResetIOEntRecursive(component, UnityEngine.Time.frameCount);
			component.MarkDirty();
		}
		else if (this.resetPositions != null)
		{
			foreach (Vector3 position in this.resetPositions)
			{
				Vector3 position2 = base.transform.TransformPoint(position);
				List<IOEntity> list = Facepunch.Pool.GetList<IOEntity>();
				global::Vis.Entities<IOEntity>(position2, 0.5f, list, 1235288065, QueryTriggerInteraction.Ignore);
				foreach (IOEntity ioentity in list)
				{
					if (ioentity.IsRootEntity() && ioentity.isServer)
					{
						PuzzleReset.ResetIOEntRecursive(ioentity, UnityEngine.Time.frameCount);
						ioentity.MarkDirty();
					}
				}
				Facepunch.Pool.FreeList<IOEntity>(ref list);
			}
		}
		List<SpawnGroup> list2 = Facepunch.Pool.GetList<SpawnGroup>();
		global::Vis.Components<SpawnGroup>(base.transform.position, 1f, list2, 262144, QueryTriggerInteraction.Collide);
		foreach (SpawnGroup spawnGroup in list2)
		{
			if (!(spawnGroup == null))
			{
				spawnGroup.Clear();
				spawnGroup.DelayedSpawn();
			}
		}
		Facepunch.Pool.FreeList<SpawnGroup>(ref list2);
		foreach (GameObject gameObject in this.resetObjects)
		{
			if (gameObject != null)
			{
				gameObject.SendMessage("OnPuzzleReset", SendMessageOptions.DontRequireReceiver);
			}
		}
		if (this.broadcastResetMessage)
		{
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				if (!basePlayer.IsNpc && basePlayer.IsConnected)
				{
					basePlayer.ShowToast(GameTip.Styles.Server_Event, this.resetPhrase, Array.Empty<string>());
				}
			}
		}
	}

	// Token: 0x060026E2 RID: 9954 RVA: 0x000F04E0 File Offset: 0x000EE6E0
	public static void ResetIOEntRecursive(IOEntity target, int resetIndex)
	{
		if (target.lastResetIndex == resetIndex)
		{
			return;
		}
		target.lastResetIndex = resetIndex;
		target.ResetIOState();
		foreach (IOEntity.IOSlot ioslot in target.outputs)
		{
			if (ioslot.connectedTo.Get(true) != null && ioslot.connectedTo.Get(true) != target)
			{
				PuzzleReset.ResetIOEntRecursive(ioslot.connectedTo.Get(true), resetIndex);
			}
		}
	}

	// Token: 0x04001F4F RID: 8015
	public SpawnGroup[] respawnGroups;

	// Token: 0x04001F50 RID: 8016
	public IOEntity[] resetEnts;

	// Token: 0x04001F51 RID: 8017
	public GameObject[] resetObjects;

	// Token: 0x04001F52 RID: 8018
	public bool playersBlockReset;

	// Token: 0x04001F53 RID: 8019
	public bool CheckSleepingAIZForPlayers;

	// Token: 0x04001F54 RID: 8020
	public float playerDetectionRadius;

	// Token: 0x04001F55 RID: 8021
	public float playerHeightDetectionMinMax = -1f;

	// Token: 0x04001F56 RID: 8022
	public Transform playerDetectionOrigin;

	// Token: 0x04001F57 RID: 8023
	public float timeBetweenResets = 30f;

	// Token: 0x04001F58 RID: 8024
	public bool scaleWithServerPopulation;

	// Token: 0x04001F59 RID: 8025
	[HideInInspector]
	public Vector3[] resetPositions;

	// Token: 0x04001F5A RID: 8026
	public bool broadcastResetMessage;

	// Token: 0x04001F5B RID: 8027
	public Translate.Phrase resetPhrase;

	// Token: 0x04001F5C RID: 8028
	private AIInformationZone zone;

	// Token: 0x04001F5D RID: 8029
	private float resetTimeElapsed;

	// Token: 0x04001F5E RID: 8030
	private float resetTickTime = 10f;
}
