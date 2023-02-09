using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200004E RID: 78
public class Candle : BaseCombatEntity, ISplashable, IIgniteable
{
	// Token: 0x060008C6 RID: 2246 RVA: 0x0005389C File Offset: 0x00051A9C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Candle.OnRpcMessage", 0))
		{
			if (rpc == 2523893445U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetWantsOn ");
				}
				using (TimeWarning.New("SetWantsOn", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(2523893445U, "SetWantsOn", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage wantsOn = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetWantsOn(wantsOn);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SetWantsOn");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060008C7 RID: 2247 RVA: 0x00053A04 File Offset: 0x00051C04
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetWantsOn(BaseEntity.RPCMessage msg)
	{
		bool b = msg.read.Bit();
		base.SetFlag(BaseEntity.Flags.On, b, false, true);
		this.UpdateInvokes();
	}

	// Token: 0x060008C8 RID: 2248 RVA: 0x00053A2D File Offset: 0x00051C2D
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.UpdateInvokes();
	}

	// Token: 0x060008C9 RID: 2249 RVA: 0x00053A3B File Offset: 0x00051C3B
	public void UpdateInvokes()
	{
		if (base.IsOn())
		{
			base.InvokeRandomized(new Action(this.Burn), this.burnRate, this.burnRate, 1f);
			return;
		}
		base.CancelInvoke(new Action(this.Burn));
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x00053A7C File Offset: 0x00051C7C
	public void Burn()
	{
		float num = this.burnRate / this.lifeTimeSeconds;
		base.Hurt(num * this.MaxHealth(), DamageType.Decay, this, false);
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x00053AA9 File Offset: 0x00051CA9
	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer && info.damageTypes.Get(DamageType.Heat) > 0f && !base.IsOn())
		{
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			this.UpdateInvokes();
		}
		base.OnAttacked(info);
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x00053AE7 File Offset: 0x00051CE7
	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return !base.IsDestroyed && amount > 1 && base.IsOn();
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x00053AFD File Offset: 0x00051CFD
	public int DoSplash(ItemDefinition splashType, int amount)
	{
		if (amount > 1)
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
			this.UpdateInvokes();
			amount--;
		}
		return amount;
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x00053B19 File Offset: 0x00051D19
	public void Ignite(Vector3 fromPos)
	{
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.UpdateInvokes();
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x0004D07C File Offset: 0x0004B27C
	public bool CanIgnite()
	{
		return !base.IsOn();
	}

	// Token: 0x040005DA RID: 1498
	private float lifeTimeSeconds = 7200f;

	// Token: 0x040005DB RID: 1499
	private float burnRate = 10f;
}
