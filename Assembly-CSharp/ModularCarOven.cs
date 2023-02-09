using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200009D RID: 157
public class ModularCarOven : BaseOven
{
	// Token: 0x06000E8F RID: 3727 RVA: 0x00079C98 File Offset: 0x00077E98
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ModularCarOven.OnRpcMessage", 0))
		{
			if (rpc == 4167839872U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SVSwitch ");
				}
				using (TimeWarning.New("SVSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(4167839872U, "SVSwitch", this, player, 3f))
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
							this.SVSwitch(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SVSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000148 RID: 328
	// (get) Token: 0x06000E90 RID: 3728 RVA: 0x00079E00 File Offset: 0x00078000
	private BaseVehicleModule ModuleParent
	{
		get
		{
			if (this.moduleParent != null)
			{
				return this.moduleParent;
			}
			this.moduleParent = (base.GetParentEntity() as BaseVehicleModule);
			return this.moduleParent;
		}
	}

	// Token: 0x06000E91 RID: 3729 RVA: 0x00079E2E File Offset: 0x0007802E
	public override void ResetState()
	{
		base.ResetState();
		this.moduleParent = null;
	}

	// Token: 0x06000E92 RID: 3730 RVA: 0x00079E3D File Offset: 0x0007803D
	protected override void SVSwitch(BaseEntity.RPCMessage msg)
	{
		if (this.ModuleParent == null || !this.ModuleParent.CanBeLooted(msg.player) || WaterLevel.Test(base.transform.position, true, null))
		{
			return;
		}
		base.SVSwitch(msg);
	}

	// Token: 0x06000E93 RID: 3731 RVA: 0x00079E7C File Offset: 0x0007807C
	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		return !(this.ModuleParent == null) && this.ModuleParent.CanBeLooted(player) && base.PlayerOpenLoot(player, panelToOpen, doPositionChecks);
	}

	// Token: 0x06000E94 RID: 3732 RVA: 0x00079EA5 File Offset: 0x000780A5
	protected override void OnCooked()
	{
		base.OnCooked();
		if (WaterLevel.Test(base.transform.position, true, null))
		{
			this.StopCooking();
		}
	}

	// Token: 0x04000968 RID: 2408
	private BaseVehicleModule moduleParent;
}
