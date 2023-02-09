using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200005F RID: 95
public class DeployableBoomBox : ContainerIOEntity, ICassettePlayer, IAudioConnectionSource
{
	// Token: 0x060009C3 RID: 2499 RVA: 0x0005A200 File Offset: 0x00058400
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DeployableBoomBox.OnRpcMessage", 0))
		{
			if (rpc == 1918716764U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_UpdateRadioIP ");
				}
				using (TimeWarning.New("Server_UpdateRadioIP", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(1918716764U, "Server_UpdateRadioIP", this, player, 2UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsVisible.Test(1918716764U, "Server_UpdateRadioIP", this, player, 3f))
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
							this.Server_UpdateRadioIP(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_UpdateRadioIP");
					}
				}
				return true;
			}
			if (rpc == 1785864031U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerTogglePlay ");
				}
				using (TimeWarning.New("ServerTogglePlay", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1785864031U, "ServerTogglePlay", this, player, 3f))
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
							this.ServerTogglePlay(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in ServerTogglePlay");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170000E4 RID: 228
	// (get) Token: 0x060009C4 RID: 2500 RVA: 0x00002E37 File Offset: 0x00001037
	public BaseEntity ToBaseEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x00002E37 File Offset: 0x00001037
	public IOEntity ToEntity()
	{
		return this;
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x0005A51C File Offset: 0x0005871C
	public override int ConsumptionAmount()
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.PowerUsageWhilePlaying;
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x0005A51C File Offset: 0x0005871C
	public override int DesiredPower()
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.PowerUsageWhilePlaying;
	}

	// Token: 0x060009C8 RID: 2504 RVA: 0x0005A530 File Offset: 0x00058730
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateHasPower(inputAmount, inputSlot);
			if (!this.IsPowered() && base.IsOn())
			{
				this.BoxController.ServerTogglePlay(false);
			}
			return;
		}
		if (this.IsPowered() && !base.IsConnectedToAnySlot(this, inputSlot, IOEntity.backtracking, false))
		{
			this.BoxController.ServerTogglePlay(inputAmount > 0);
		}
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x0005A58C File Offset: 0x0005878C
	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.canAcceptItem = new Func<Item, int, bool>(this.ItemFilter);
		this.BoxController.HurtCallback = new Action<float>(this.HurtCallback);
		if (this.IsStatic)
		{
			base.SetFlag(BaseEntity.Flags.Reserved8, true, false, true);
		}
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x0005A5E4 File Offset: 0x000587E4
	private bool ItemFilter(Item item, int count)
	{
		ItemDefinition[] validCassettes = this.BoxController.ValidCassettes;
		for (int i = 0; i < validCassettes.Length; i++)
		{
			if (validCassettes[i] == item.info)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x00053CE6 File Offset: 0x00051EE6
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return base.GetPassthroughAmount(outputSlot);
	}

	// Token: 0x060009CC RID: 2508 RVA: 0x0005A61E File Offset: 0x0005881E
	public override int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		if (inputSlot != 0)
		{
			return this.currentEnergy;
		}
		return base.CalculateCurrentEnergy(inputAmount, inputSlot);
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x0005A632 File Offset: 0x00058832
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerTogglePlay(BaseEntity.RPCMessage msg)
	{
		this.BoxController.ServerTogglePlay(msg);
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x0005A640 File Offset: 0x00058840
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.CallsPerSecond(2UL)]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	private void Server_UpdateRadioIP(BaseEntity.RPCMessage msg)
	{
		this.BoxController.Server_UpdateRadioIP(msg);
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x0005A64E File Offset: 0x0005884E
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		this.BoxController.Save(info);
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x0005A663 File Offset: 0x00058863
	public bool ClearRadioByUserId(ulong id)
	{
		return this.BoxController.ClearRadioByUserId(id);
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x0005A671 File Offset: 0x00058871
	public void OnCassetteInserted(Cassette c)
	{
		this.BoxController.OnCassetteInserted(c);
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x0005A67F File Offset: 0x0005887F
	public void OnCassetteRemoved(Cassette c)
	{
		this.BoxController.OnCassetteRemoved(c);
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x0005A68D File Offset: 0x0005888D
	public void HurtCallback(float amount)
	{
		base.Hurt(amount, DamageType.Decay, null, true);
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x0005A69A File Offset: 0x0005889A
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		this.BoxController.Load(info);
		base.Load(info);
		if (base.isServer && this.IsStatic)
		{
			base.SetFlag(BaseEntity.Flags.Reserved8, true, false, true);
		}
	}

	// Token: 0x04000668 RID: 1640
	public BoomBox BoxController;

	// Token: 0x04000669 RID: 1641
	public int PowerUsageWhilePlaying = 10;

	// Token: 0x0400066A RID: 1642
	public const int MaxBacktrackHopsClient = 30;

	// Token: 0x0400066B RID: 1643
	public bool IsStatic;
}
