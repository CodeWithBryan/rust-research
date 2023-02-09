using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000047 RID: 71
public class BearTrap : BaseTrap
{
	// Token: 0x0600080F RID: 2063 RVA: 0x0004DE50 File Offset: 0x0004C050
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BearTrap.OnRpcMessage", 0))
		{
			if (rpc == 547827602U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Arm ");
				}
				using (TimeWarning.New("RPC_Arm", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(547827602U, "RPC_Arm", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpc2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Arm(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Arm");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x0002782C File Offset: 0x00025A2C
	public bool Armed()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x06000811 RID: 2065 RVA: 0x0004DFB8 File Offset: 0x0004C1B8
	public override void InitShared()
	{
		this.animator = base.GetComponent<Animator>();
		base.InitShared();
	}

	// Token: 0x06000812 RID: 2066 RVA: 0x0004DFCC File Offset: 0x0004C1CC
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !this.Armed() && player.CanBuild();
	}

	// Token: 0x06000813 RID: 2067 RVA: 0x0004DFE7 File Offset: 0x0004C1E7
	public override void ServerInit()
	{
		base.ServerInit();
		this.Arm();
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x0004DFF5 File Offset: 0x0004C1F5
	public override void Arm()
	{
		base.Arm();
		this.RadialResetCorpses(120f);
	}

	// Token: 0x06000815 RID: 2069 RVA: 0x0004E008 File Offset: 0x0004C208
	public void Fire()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x0004E01B File Offset: 0x0004C21B
	public override void ObjectEntered(GameObject obj)
	{
		if (!this.Armed())
		{
			return;
		}
		this.hurtTarget = obj;
		base.Invoke(new Action(this.DelayedFire), 0.05f);
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x0004E044 File Offset: 0x0004C244
	public void DelayedFire()
	{
		if (this.hurtTarget)
		{
			BaseEntity baseEntity = this.hurtTarget.ToBaseEntity();
			if (baseEntity != null)
			{
				HitInfo hitInfo = new HitInfo(this, baseEntity, DamageType.Bite, 50f, base.transform.position);
				hitInfo.damageTypes.Add(DamageType.Stab, 30f);
				baseEntity.OnAttacked(hitInfo);
			}
			this.hurtTarget = null;
		}
		this.RadialResetCorpses(1800f);
		this.Fire();
		base.Hurt(25f);
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x0004E0CC File Offset: 0x0004C2CC
	public void RadialResetCorpses(float duration)
	{
		List<BaseCorpse> list = Facepunch.Pool.GetList<BaseCorpse>();
		global::Vis.Entities<BaseCorpse>(base.transform.position, 5f, list, 512, QueryTriggerInteraction.Collide);
		foreach (BaseCorpse baseCorpse in list)
		{
			baseCorpse.ResetRemovalTime(duration);
		}
		Facepunch.Pool.FreeList<BaseCorpse>(ref list);
	}

	// Token: 0x06000819 RID: 2073 RVA: 0x0004E144 File Offset: 0x0004C344
	public override void OnAttacked(HitInfo info)
	{
		float num = info.damageTypes.Total();
		if ((info.damageTypes.IsMeleeType() && num > 20f) || num > 30f)
		{
			this.Fire();
		}
		base.OnAttacked(info);
	}

	// Token: 0x0600081A RID: 2074 RVA: 0x0004E187 File Offset: 0x0004C387
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Arm(BaseEntity.RPCMessage rpc)
	{
		if (this.Armed())
		{
			return;
		}
		this.Arm();
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x0004E198 File Offset: 0x0004C398
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (!base.isServer && this.animator.isInitialized)
		{
			this.animator.SetBool("armed", this.Armed());
		}
	}

	// Token: 0x04000558 RID: 1368
	protected Animator animator;

	// Token: 0x04000559 RID: 1369
	private GameObject hurtTarget;
}
