using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using UnityEngine;

// Token: 0x02000086 RID: 134
public class JunkPile : BaseEntity
{
	// Token: 0x06000C9E RID: 3230 RVA: 0x0006B940 File Offset: 0x00069B40
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("JunkPile.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000C9F RID: 3231 RVA: 0x0006B980 File Offset: 0x00069B80
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.TimeOut), 1800f);
		base.InvokeRepeating(new Action(this.CheckEmpty), 10f, 30f);
		base.Invoke(new Action(this.SpawnInitial), 1f);
		this.isSinking = false;
	}

	// Token: 0x06000CA0 RID: 3232 RVA: 0x0006B9E4 File Offset: 0x00069BE4
	private void SpawnInitial()
	{
		SpawnGroup[] array = this.spawngroups;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SpawnInitial();
		}
	}

	// Token: 0x06000CA1 RID: 3233 RVA: 0x0006BA10 File Offset: 0x00069C10
	public bool SpawnGroupsEmpty()
	{
		SpawnGroup[] array = this.spawngroups;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].currentPopulation > 0)
			{
				return false;
			}
		}
		return !(this.NPCSpawn != null) || this.NPCSpawn.currentPopulation <= 0;
	}

	// Token: 0x06000CA2 RID: 3234 RVA: 0x0006BA5E File Offset: 0x00069C5E
	public void CheckEmpty()
	{
		if (this.SpawnGroupsEmpty() && !this.PlayersNearby())
		{
			base.CancelInvoke(new Action(this.CheckEmpty));
			this.SinkAndDestroy();
		}
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x0006BA88 File Offset: 0x00069C88
	public bool PlayersNearby()
	{
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Vis.Entities<BasePlayer>(base.transform.position, this.TimeoutPlayerCheckRadius(), list, 131072, QueryTriggerInteraction.Collide);
		bool result = false;
		foreach (BasePlayer basePlayer in list)
		{
			if (!basePlayer.IsSleeping() && basePlayer.IsAlive() && !(basePlayer is HumanNPC))
			{
				result = true;
				break;
			}
		}
		Pool.FreeList<BasePlayer>(ref list);
		return result;
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x0006BB18 File Offset: 0x00069D18
	public virtual float TimeoutPlayerCheckRadius()
	{
		return 15f;
	}

	// Token: 0x06000CA5 RID: 3237 RVA: 0x0006BB1F File Offset: 0x00069D1F
	public void TimeOut()
	{
		if (this.PlayersNearby())
		{
			base.Invoke(new Action(this.TimeOut), 30f);
			return;
		}
		this.SpawnGroupsEmpty();
		this.SinkAndDestroy();
	}

	// Token: 0x06000CA6 RID: 3238 RVA: 0x0006BB50 File Offset: 0x00069D50
	public void SinkAndDestroy()
	{
		base.CancelInvoke(new Action(this.SinkAndDestroy));
		SpawnGroup[] array = this.spawngroups;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Clear();
		}
		base.SetFlag(BaseEntity.Flags.Reserved8, true, true, true);
		if (this.NPCSpawn != null)
		{
			this.NPCSpawn.Clear();
		}
		base.ClientRPC(null, "CLIENT_StartSink");
		base.transform.position -= new Vector3(0f, 5f, 0f);
		this.isSinking = true;
		base.Invoke(new Action(this.KillMe), 22f);
	}

	// Token: 0x06000CA7 RID: 3239 RVA: 0x000029D4 File Offset: 0x00000BD4
	public void KillMe()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x04000803 RID: 2051
	public GameObjectRef sinkEffect;

	// Token: 0x04000804 RID: 2052
	public SpawnGroup[] spawngroups;

	// Token: 0x04000805 RID: 2053
	public NPCSpawner NPCSpawn;

	// Token: 0x04000806 RID: 2054
	private const float lifetimeMinutes = 30f;

	// Token: 0x04000807 RID: 2055
	protected bool isSinking;
}
