using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000089 RID: 137
public class Landmine : BaseTrap
{
	// Token: 0x06000CC9 RID: 3273 RVA: 0x0006CAD8 File Offset: 0x0006ACD8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Landmine.OnRpcMessage", 0))
		{
			if (rpc == 1552281787U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Disarm ");
				}
				using (TimeWarning.New("RPC_Disarm", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1552281787U, "RPC_Disarm", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Disarm(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Disarm");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000CCA RID: 3274 RVA: 0x00027835 File Offset: 0x00025A35
	public bool Triggered()
	{
		return base.HasFlag(global::BaseEntity.Flags.Open);
	}

	// Token: 0x06000CCB RID: 3275 RVA: 0x0002782C File Offset: 0x00025A2C
	public bool Armed()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06000CCC RID: 3276 RVA: 0x0006CC40 File Offset: 0x0006AE40
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.landmine = Facepunch.Pool.Get<ProtoBuf.Landmine>();
			info.msg.landmine.triggeredID = this.triggerPlayerID;
		}
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x0006CC77 File Offset: 0x0006AE77
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (!info.fromDisk && info.msg.landmine != null)
		{
			this.triggerPlayerID = info.msg.landmine.triggeredID;
		}
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x0006CCAB File Offset: 0x0006AEAB
	public override void ServerInit()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		base.Invoke(new Action(this.Arm), 1.5f);
		base.ServerInit();
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x0006CCD8 File Offset: 0x0006AED8
	public override void ObjectEntered(GameObject obj)
	{
		if (base.isClient)
		{
			return;
		}
		if (!this.Armed())
		{
			base.CancelInvoke(new Action(this.Arm));
			this.blocked = true;
			return;
		}
		global::BasePlayer ply = obj.ToBaseEntity() as global::BasePlayer;
		this.Trigger(ply);
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x0006CD24 File Offset: 0x0006AF24
	public void Trigger(global::BasePlayer ply = null)
	{
		if (ply)
		{
			this.triggerPlayerID = ply.userID;
		}
		base.SetFlag(global::BaseEntity.Flags.Open, true, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000CD1 RID: 3281 RVA: 0x0006CD4B File Offset: 0x0006AF4B
	public override void OnEmpty()
	{
		if (this.blocked)
		{
			this.Arm();
			this.blocked = false;
			return;
		}
		if (this.Triggered())
		{
			base.Invoke(new Action(this.TryExplode), 0.05f);
		}
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x0006CD84 File Offset: 0x0006AF84
	public virtual void Explode()
	{
		base.health = float.PositiveInfinity;
		Effect.server.Run(this.explosionEffect.resourcePath, base.PivotPoint(), base.transform.up, null, true);
		DamageUtil.RadiusDamage(this, base.LookupPrefab(), base.CenterPoint(), this.minExplosionRadius, this.explosionRadius, this.damageTypes, 2263296, true);
		if (base.IsDestroyed)
		{
			return;
		}
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000CD3 RID: 3283 RVA: 0x0006CDF9 File Offset: 0x0006AFF9
	public override void OnKilled(HitInfo info)
	{
		base.Invoke(new Action(this.Explode), UnityEngine.Random.Range(0.1f, 0.3f));
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x0006CE1D File Offset: 0x0006B01D
	private void OnGroundMissing()
	{
		this.Explode();
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x0006CE25 File Offset: 0x0006B025
	private void TryExplode()
	{
		if (this.Armed())
		{
			this.Explode();
		}
	}

	// Token: 0x06000CD6 RID: 3286 RVA: 0x0006CE35 File Offset: 0x0006B035
	public override void Arm()
	{
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000CD7 RID: 3287 RVA: 0x0006CE48 File Offset: 0x0006B048
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Disarm(global::BaseEntity.RPCMessage rpc)
	{
		if ((ulong)rpc.player.net.ID == this.triggerPlayerID)
		{
			return;
		}
		if (!this.Armed())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		if (UnityEngine.Random.Range(0, 100) < 15)
		{
			base.Invoke(new Action(this.TryExplode), 0.05f);
			return;
		}
		rpc.player.GiveItem(ItemManager.CreateByName("trap.landmine", 1, 0UL), global::BaseEntity.GiveItemReason.PickedUp);
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0400081F RID: 2079
	public GameObjectRef explosionEffect;

	// Token: 0x04000820 RID: 2080
	public GameObjectRef triggeredEffect;

	// Token: 0x04000821 RID: 2081
	public float minExplosionRadius;

	// Token: 0x04000822 RID: 2082
	public float explosionRadius;

	// Token: 0x04000823 RID: 2083
	public bool blocked;

	// Token: 0x04000824 RID: 2084
	private ulong triggerPlayerID;

	// Token: 0x04000825 RID: 2085
	public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();
}
