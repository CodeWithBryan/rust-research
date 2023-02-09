using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000068 RID: 104
public class DudTimedExplosive : TimedExplosive, IIgniteable, ISplashable
{
	// Token: 0x06000A49 RID: 2633 RVA: 0x0005D74C File Offset: 0x0005B94C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DudTimedExplosive.OnRpcMessage", 0))
		{
			if (rpc == 2436818324U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Pickup ");
				}
				using (TimeWarning.New("RPC_Pickup", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2436818324U, "RPC_Pickup", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Pickup(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Pickup");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x0002782C File Offset: 0x00025A2C
	private bool IsWickBurning()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x170000E9 RID: 233
	// (get) Token: 0x06000A4B RID: 2635 RVA: 0x0005D8B4 File Offset: 0x0005BAB4
	protected override bool AlwaysRunWaterCheck
	{
		get
		{
			return this.becomeDudInWater;
		}
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x0005D8BC File Offset: 0x0005BABC
	public override void WaterCheck()
	{
		if (!this.becomeDudInWater || this.WaterFactor() < 0.5f)
		{
			base.WaterCheck();
			return;
		}
		if (this.creatorEntity != null && this.creatorEntity.IsNpc)
		{
			base.Explode();
			return;
		}
		this.BecomeDud();
		if (base.IsInvoking(new Action(this.WaterCheck)))
		{
			base.CancelInvoke(new Action(this.WaterCheck));
		}
		if (base.IsInvoking(new Action(this.Explode)))
		{
			base.CancelInvoke(new Action(this.Explode));
		}
	}

	// Token: 0x06000A4D RID: 2637 RVA: 0x0005D960 File Offset: 0x0005BB60
	public override float GetRandomTimerTime()
	{
		float randomTimerTime = base.GetRandomTimerTime();
		float num = 1f;
		if (UnityEngine.Random.Range(0f, 1f) <= 0.15f)
		{
			num = 0.334f;
		}
		else if (UnityEngine.Random.Range(0f, 1f) <= 0.15f)
		{
			num = 3f;
		}
		return randomTimerTime * num;
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x0005D9B8 File Offset: 0x0005BBB8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Pickup(global::BaseEntity.RPCMessage msg)
	{
		if (this.IsWickBurning())
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (UnityEngine.Random.Range(0f, 1f) >= 0.5f && base.HasParent())
		{
			this.SetFuse(UnityEngine.Random.Range(2.5f, 3f));
			return;
		}
		player.GiveItem(ItemManager.Create(this.itemToGive, 1, this.skinID), global::BaseEntity.GiveItemReason.Generic);
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x0005DA29 File Offset: 0x0005BC29
	public override void SetFuse(float fuseLength)
	{
		base.SetFuse(fuseLength);
		this.explodeTime = UnityEngine.Time.realtimeSinceStartup + fuseLength;
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		base.CancelInvoke(new Action(base.KillMessage));
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x0005DA64 File Offset: 0x0005BC64
	public override void Explode()
	{
		if (this.creatorEntity != null && this.creatorEntity.IsNpc)
		{
			base.Explode();
			return;
		}
		if (UnityEngine.Random.Range(0f, 1f) < this.dudChance)
		{
			this.BecomeDud();
			return;
		}
		base.Explode();
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x0005DAB7 File Offset: 0x0005BCB7
	public override bool CanStickTo(global::BaseEntity entity)
	{
		return base.CanStickTo(entity) && this.IsWickBurning();
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x0005DACC File Offset: 0x0005BCCC
	public virtual void BecomeDud()
	{
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		bool flag = false;
		EntityRef parentEntity = this.parentEntity;
		while (parentEntity.IsValid(base.isServer) && !flag)
		{
			global::BaseEntity baseEntity = parentEntity.Get(base.isServer);
			if (baseEntity.syncPosition)
			{
				flag = true;
			}
			parentEntity = baseEntity.parentEntity;
		}
		if (flag)
		{
			base.SetParent(null, false, false);
		}
		base.transform.SetPositionAndRotation(position, rotation);
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		this.SetCollisionEnabled(true);
		if (flag)
		{
			this.SetMotionEnabled(true);
		}
		Effect.server.Run("assets/bundled/prefabs/fx/impacts/blunt/concrete/concrete1.prefab", this, 0U, Vector3.zero, Vector3.zero, null, false);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		base.CancelInvoke(new Action(base.KillMessage));
		base.Invoke(new Action(base.KillMessage), 1200f);
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x0005DBA7 File Offset: 0x0005BDA7
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.dudExplosive = Facepunch.Pool.Get<DudExplosive>();
		info.msg.dudExplosive.fuseTimeLeft = this.explodeTime - UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x0005DBDC File Offset: 0x0005BDDC
	public void Ignite(Vector3 fromPos)
	{
		this.SetFuse(this.GetRandomTimerTime());
		base.ReceiveCollisionMessages(true);
		if (this.waterCausesExplosion)
		{
			base.InvokeRepeating(new Action(this.WaterCheck), 0f, 0.5f);
		}
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x0005DC16 File Offset: 0x0005BE16
	public bool CanIgnite()
	{
		return !base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x0005DC22 File Offset: 0x0005BE22
	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return !base.IsDestroyed && base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06000A57 RID: 2647 RVA: 0x0005DC35 File Offset: 0x0005BE35
	public int DoSplash(ItemDefinition splashType, int amount)
	{
		this.BecomeDud();
		if (base.IsInvoking(new Action(this.Explode)))
		{
			base.CancelInvoke(new Action(this.Explode));
		}
		return 1;
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x0005DC66 File Offset: 0x0005BE66
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.dudExplosive != null)
		{
			this.explodeTime = UnityEngine.Time.realtimeSinceStartup + info.msg.dudExplosive.fuseTimeLeft;
		}
	}

	// Token: 0x040006A5 RID: 1701
	public GameObjectRef fizzleEffect;

	// Token: 0x040006A6 RID: 1702
	public GameObject wickSpark;

	// Token: 0x040006A7 RID: 1703
	public AudioSource wickSound;

	// Token: 0x040006A8 RID: 1704
	public float dudChance = 0.3f;

	// Token: 0x040006A9 RID: 1705
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemToGive;

	// Token: 0x040006AA RID: 1706
	[NonSerialized]
	private float explodeTime;

	// Token: 0x040006AB RID: 1707
	public bool becomeDudInWater;
}
