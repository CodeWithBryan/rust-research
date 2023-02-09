using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200014F RID: 335
public class EggHuntEvent : BaseHuntEvent
{
	// Token: 0x06001632 RID: 5682 RVA: 0x000A9067 File Offset: 0x000A7267
	public bool IsEventActive()
	{
		return this.timeAlive > this.warmupTime && this.timeAlive - this.warmupTime < EggHuntEvent.durationSeconds;
	}

	// Token: 0x06001633 RID: 5683 RVA: 0x000A9090 File Offset: 0x000A7290
	public override void ServerInit()
	{
		base.ServerInit();
		if (EggHuntEvent.serverEvent && base.isServer)
		{
			EggHuntEvent.serverEvent.Kill(global::BaseNetworkable.DestroyMode.None);
			EggHuntEvent.serverEvent = null;
		}
		EggHuntEvent.serverEvent = this;
		base.Invoke(new Action(this.StartEvent), this.warmupTime);
	}

	// Token: 0x06001634 RID: 5684 RVA: 0x000A90E6 File Offset: 0x000A72E6
	public void StartEvent()
	{
		this.SpawnEggs();
	}

	// Token: 0x06001635 RID: 5685 RVA: 0x000A90F0 File Offset: 0x000A72F0
	public void SpawnEggsAtPoint(int numEggs, Vector3 pos, Vector3 aimDir, float minDist = 1f, float maxDist = 2f)
	{
		for (int i = 0; i < numEggs; i++)
		{
			Vector3 vector = pos;
			if (aimDir == Vector3.zero)
			{
				aimDir = UnityEngine.Random.onUnitSphere;
			}
			else
			{
				aimDir = AimConeUtil.GetModifiedAimConeDirection(90f, aimDir, true);
			}
			vector = pos + Vector3Ex.Direction2D(pos + aimDir * 10f, pos) * UnityEngine.Random.Range(minDist, maxDist);
			vector.y = TerrainMeta.HeightMap.GetHeight(vector);
			CollectableEasterEgg collectableEasterEgg = GameManager.server.CreateEntity(this.HuntablePrefab[UnityEngine.Random.Range(0, this.HuntablePrefab.Length)].resourcePath, vector, default(Quaternion), true) as CollectableEasterEgg;
			collectableEasterEgg.Spawn();
			this._spawnedEggs.Add(collectableEasterEgg);
		}
	}

	// Token: 0x06001636 RID: 5686 RVA: 0x000A91BC File Offset: 0x000A73BC
	[ContextMenu("SpawnDebug")]
	public void SpawnEggs()
	{
		foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
		{
			this.SpawnEggsAtPoint(UnityEngine.Random.Range(4, 6) + Mathf.RoundToInt(basePlayer.eggVision), basePlayer.transform.position, basePlayer.eyes.BodyForward(), 15f, 25f);
		}
	}

	// Token: 0x06001637 RID: 5687 RVA: 0x000A9240 File Offset: 0x000A7440
	public void RandPickup()
	{
		foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
		{
		}
	}

	// Token: 0x06001638 RID: 5688 RVA: 0x000A928C File Offset: 0x000A748C
	public void EggCollected(global::BasePlayer player)
	{
		EggHuntEvent.EggHunter eggHunter;
		if (this._eggHunters.ContainsKey(player.userID))
		{
			eggHunter = this._eggHunters[player.userID];
		}
		else
		{
			eggHunter = new EggHuntEvent.EggHunter();
			eggHunter.displayName = player.displayName;
			eggHunter.userid = player.userID;
			this._eggHunters.Add(player.userID, eggHunter);
		}
		if (eggHunter == null)
		{
			Debug.LogWarning("Easter error");
			return;
		}
		eggHunter.numEggs++;
		this.QueueUpdate();
		int num = ((float)Mathf.RoundToInt(player.eggVision) * 0.5f < 1f) ? UnityEngine.Random.Range(0, 2) : 1;
		this.SpawnEggsAtPoint(UnityEngine.Random.Range(1 + num, 2 + num), player.transform.position, player.eyes.BodyForward(), 15f, 25f);
	}

	// Token: 0x06001639 RID: 5689 RVA: 0x000A9369 File Offset: 0x000A7569
	public void QueueUpdate()
	{
		if (base.IsInvoking(new Action(this.DoNetworkUpdate)))
		{
			return;
		}
		base.Invoke(new Action(this.DoNetworkUpdate), 2f);
	}

	// Token: 0x0600163A RID: 5690 RVA: 0x00007338 File Offset: 0x00005538
	public void DoNetworkUpdate()
	{
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600163B RID: 5691 RVA: 0x000A9397 File Offset: 0x000A7597
	public static void Sort(List<EggHuntEvent.EggHunter> hunterList)
	{
		hunterList.Sort((EggHuntEvent.EggHunter a, EggHuntEvent.EggHunter b) => b.numEggs.CompareTo(a.numEggs));
	}

	// Token: 0x0600163C RID: 5692 RVA: 0x000A93C0 File Offset: 0x000A75C0
	public List<EggHuntEvent.EggHunter> GetTopHunters()
	{
		List<EggHuntEvent.EggHunter> list = Facepunch.Pool.GetList<EggHuntEvent.EggHunter>();
		foreach (KeyValuePair<ulong, EggHuntEvent.EggHunter> keyValuePair in this._eggHunters)
		{
			list.Add(keyValuePair.Value);
		}
		EggHuntEvent.Sort(list);
		return list;
	}

	// Token: 0x0600163D RID: 5693 RVA: 0x000A9428 File Offset: 0x000A7628
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.eggHunt = Facepunch.Pool.Get<EggHunt>();
		List<EggHuntEvent.EggHunter> topHunters = this.GetTopHunters();
		info.msg.eggHunt.hunters = Facepunch.Pool.GetList<EggHunt.EggHunter>();
		for (int i = 0; i < Mathf.Min(10, topHunters.Count); i++)
		{
			EggHunt.EggHunter eggHunter = Facepunch.Pool.Get<EggHunt.EggHunter>();
			eggHunter.displayName = topHunters[i].displayName;
			eggHunter.numEggs = topHunters[i].numEggs;
			eggHunter.playerID = topHunters[i].userid;
			info.msg.eggHunt.hunters.Add(eggHunter);
		}
	}

	// Token: 0x0600163E RID: 5694 RVA: 0x000A94D4 File Offset: 0x000A76D4
	public void CleanupEggs()
	{
		foreach (CollectableEasterEgg collectableEasterEgg in this._spawnedEggs)
		{
			if (collectableEasterEgg != null)
			{
				collectableEasterEgg.Kill(global::BaseNetworkable.DestroyMode.None);
			}
		}
	}

	// Token: 0x0600163F RID: 5695 RVA: 0x000A9530 File Offset: 0x000A7730
	public void Cooldown()
	{
		base.CancelInvoke(new Action(this.Cooldown));
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06001640 RID: 5696 RVA: 0x000A954C File Offset: 0x000A774C
	public virtual void PrintWinnersAndAward()
	{
		List<EggHuntEvent.EggHunter> topHunters = this.GetTopHunters();
		if (topHunters.Count > 0)
		{
			EggHuntEvent.EggHunter eggHunter = topHunters[0];
			Chat.Broadcast(string.Concat(new object[]
			{
				eggHunter.displayName,
				" is the top bunny with ",
				eggHunter.numEggs,
				" eggs collected."
			}), "", "#eee", 0UL);
			for (int i = 0; i < topHunters.Count; i++)
			{
				EggHuntEvent.EggHunter eggHunter2 = topHunters[i];
				global::BasePlayer basePlayer = global::BasePlayer.FindByID(eggHunter2.userid);
				if (basePlayer)
				{
					basePlayer.ChatMessage(string.Concat(new object[]
					{
						"You placed ",
						i + 1,
						" of ",
						topHunters.Count,
						" with ",
						topHunters[i].numEggs,
						" eggs collected."
					}));
				}
				else
				{
					Debug.LogWarning("EggHuntEvent Printwinners could not find player with id :" + eggHunter2.userid);
				}
			}
			int num = 0;
			while (num < this.placementAwards.Length && num < topHunters.Count)
			{
				global::BasePlayer basePlayer2 = global::BasePlayer.FindByID(topHunters[num].userid);
				if (basePlayer2)
				{
					basePlayer2.inventory.GiveItem(ItemManager.Create(this.placementAwards[num].itemDef, (int)this.placementAwards[num].amount, 0UL), basePlayer2.inventory.containerMain);
					basePlayer2.ChatMessage(string.Concat(new object[]
					{
						"You received ",
						(int)this.placementAwards[num].amount,
						"x ",
						this.placementAwards[num].itemDef.displayName.english,
						" as an award!"
					}));
				}
				num++;
			}
			return;
		}
		Chat.Broadcast("Wow, no one played so no one won.", "", "#eee", 0UL);
	}

	// Token: 0x06001641 RID: 5697 RVA: 0x000A9760 File Offset: 0x000A7960
	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer)
		{
			EggHuntEvent.serverEvent = null;
			return;
		}
		EggHuntEvent.clientEvent = null;
	}

	// Token: 0x06001642 RID: 5698 RVA: 0x000A9780 File Offset: 0x000A7980
	public void Update()
	{
		this.timeAlive += UnityEngine.Time.deltaTime;
		if (base.isServer && !base.IsDestroyed)
		{
			if (this.timeAlive - this.warmupTime > EggHuntEvent.durationSeconds - this.warnTime)
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved1, true, false, true);
			}
			if (this.timeAlive - this.warmupTime > EggHuntEvent.durationSeconds && !base.IsInvoking(new Action(this.Cooldown)))
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
				this.CleanupEggs();
				this.PrintWinnersAndAward();
				base.Invoke(new Action(this.Cooldown), 10f);
			}
		}
	}

	// Token: 0x06001643 RID: 5699 RVA: 0x000A9838 File Offset: 0x000A7A38
	public float GetTimeRemaining()
	{
		float num = EggHuntEvent.durationSeconds - this.timeAlive;
		if (num < 0f)
		{
			num = 0f;
		}
		return num;
	}

	// Token: 0x04000F18 RID: 3864
	public float warmupTime = 10f;

	// Token: 0x04000F19 RID: 3865
	public float cooldownTime = 10f;

	// Token: 0x04000F1A RID: 3866
	public float warnTime = 20f;

	// Token: 0x04000F1B RID: 3867
	public float timeAlive;

	// Token: 0x04000F1C RID: 3868
	public static EggHuntEvent serverEvent = null;

	// Token: 0x04000F1D RID: 3869
	public static EggHuntEvent clientEvent = null;

	// Token: 0x04000F1E RID: 3870
	[NonSerialized]
	public static float durationSeconds = 180f;

	// Token: 0x04000F1F RID: 3871
	private Dictionary<ulong, EggHuntEvent.EggHunter> _eggHunters = new Dictionary<ulong, EggHuntEvent.EggHunter>();

	// Token: 0x04000F20 RID: 3872
	public List<CollectableEasterEgg> _spawnedEggs = new List<CollectableEasterEgg>();

	// Token: 0x04000F21 RID: 3873
	public ItemAmount[] placementAwards;

	// Token: 0x02000BDC RID: 3036
	public class EggHunter
	{
		// Token: 0x04003FF9 RID: 16377
		public ulong userid;

		// Token: 0x04003FFA RID: 16378
		public string displayName;

		// Token: 0x04003FFB RID: 16379
		public int numEggs;
	}
}
