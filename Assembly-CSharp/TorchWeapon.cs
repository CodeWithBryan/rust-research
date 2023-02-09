using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000DB RID: 219
public class TorchWeapon : BaseMelee
{
	// Token: 0x060012D6 RID: 4822 RVA: 0x00096DE4 File Offset: 0x00094FE4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TorchWeapon.OnRpcMessage", 0))
		{
			if (rpc == 2235491565U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Extinguish ");
				}
				using (TimeWarning.New("Extinguish", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(2235491565U, "Extinguish", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Extinguish(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Extinguish");
					}
				}
				return true;
			}
			if (rpc == 3010584743U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Ignite ");
				}
				using (TimeWarning.New("Ignite", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(3010584743U, "Ignite", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Ignite(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in Ignite");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060012D7 RID: 4823 RVA: 0x000970DC File Offset: 0x000952DC
	public override void GetAttackStats(HitInfo info)
	{
		base.GetAttackStats(info);
		if (base.HasFlag(BaseEntity.Flags.On))
		{
			info.damageTypes.Add(DamageType.Heat, 1f);
		}
	}

	// Token: 0x060012D8 RID: 4824 RVA: 0x000970FF File Offset: 0x000952FF
	public override float GetConditionLoss()
	{
		return base.GetConditionLoss() + (base.HasFlag(BaseEntity.Flags.On) ? 6f : 0f);
	}

	// Token: 0x060012D9 RID: 4825 RVA: 0x00097120 File Offset: 0x00095320
	public void SetIsOn(bool isOn)
	{
		if (isOn)
		{
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			base.InvokeRepeating(new Action(this.UseFuel), 1f, 1f);
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		base.CancelInvoke(new Action(this.UseFuel));
	}

	// Token: 0x060012DA RID: 4826 RVA: 0x00097173 File Offset: 0x00095373
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void Ignite(BaseEntity.RPCMessage msg)
	{
		if (msg.player.CanInteract())
		{
			this.SetIsOn(true);
		}
	}

	// Token: 0x060012DB RID: 4827 RVA: 0x00097189 File Offset: 0x00095389
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void Extinguish(BaseEntity.RPCMessage msg)
	{
		if (msg.player.CanInteract())
		{
			this.SetIsOn(false);
		}
	}

	// Token: 0x060012DC RID: 4828 RVA: 0x000971A0 File Offset: 0x000953A0
	public void UseFuel()
	{
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		ownerItem.LoseCondition(this.fuelTickAmount);
	}

	// Token: 0x060012DD RID: 4829 RVA: 0x000971C4 File Offset: 0x000953C4
	public override void OnHeldChanged()
	{
		if (base.IsDisabled())
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
			base.CancelInvoke(new Action(this.UseFuel));
		}
	}

	// Token: 0x060012DE RID: 4830 RVA: 0x000971EC File Offset: 0x000953EC
	public override string GetStrikeEffectPath(string materialName)
	{
		for (int i = 0; i < this.materialStrikeFX.Count; i++)
		{
			if (this.materialStrikeFX[i].materialName == materialName && this.materialStrikeFX[i].fx.isValid)
			{
				return this.materialStrikeFX[i].fx.resourcePath;
			}
		}
		if (base.HasFlag(BaseEntity.Flags.On) && this.litStrikeFX.isValid)
		{
			return this.litStrikeFX.resourcePath;
		}
		return this.strikeFX.resourcePath;
	}

	// Token: 0x04000BBC RID: 3004
	[NonSerialized]
	public float fuelTickAmount = 0.083333336f;

	// Token: 0x04000BBD RID: 3005
	[Header("TorchWeapon")]
	public AnimatorOverrideController LitHoldAnimationOverride;

	// Token: 0x04000BBE RID: 3006
	public GameObjectRef litStrikeFX;
}
