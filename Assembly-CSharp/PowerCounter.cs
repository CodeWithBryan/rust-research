using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Token: 0x020000AD RID: 173
public class PowerCounter : global::IOEntity
{
	// Token: 0x06000FB4 RID: 4020 RVA: 0x000822EC File Offset: 0x000804EC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PowerCounter.OnRpcMessage", 0))
		{
			if (rpc == 3554226761U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SERVER_SetTarget ");
				}
				using (TimeWarning.New("SERVER_SetTarget", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3554226761U, "SERVER_SetTarget", this, player, 3f))
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
							this.SERVER_SetTarget(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SERVER_SetTarget");
					}
				}
				return true;
			}
			if (rpc == 3222475159U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ToggleDisplayMode ");
				}
				using (TimeWarning.New("ToggleDisplayMode", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3222475159U, "ToggleDisplayMode", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ToggleDisplayMode(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in ToggleDisplayMode");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000FB5 RID: 4021 RVA: 0x00004C84 File Offset: 0x00002E84
	public bool DisplayPassthrough()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved2);
	}

	// Token: 0x06000FB6 RID: 4022 RVA: 0x000825EC File Offset: 0x000807EC
	public bool DisplayCounter()
	{
		return !this.DisplayPassthrough();
	}

	// Token: 0x06000FB7 RID: 4023 RVA: 0x000825F7 File Offset: 0x000807F7
	public bool CanPlayerAdmin(global::BasePlayer player)
	{
		return player != null && player.CanBuild();
	}

	// Token: 0x06000FB8 RID: 4024 RVA: 0x0008260A File Offset: 0x0008080A
	public int GetTarget()
	{
		return this.targetCounterNumber;
	}

	// Token: 0x06000FB9 RID: 4025 RVA: 0x000228A0 File Offset: 0x00020AA0
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x06000FBA RID: 4026 RVA: 0x00082612 File Offset: 0x00080812
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void SERVER_SetTarget(global::BaseEntity.RPCMessage msg)
	{
		if (!this.CanPlayerAdmin(msg.player))
		{
			return;
		}
		this.targetCounterNumber = msg.read.Int32();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000FBB RID: 4027 RVA: 0x0008263B File Offset: 0x0008083B
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void ToggleDisplayMode(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanBuild())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved2, msg.read.Bit(), false, false);
		this.MarkDirty();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000FBC RID: 4028 RVA: 0x00082670 File Offset: 0x00080870
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (this.DisplayPassthrough() || this.counterNumber >= this.targetCounterNumber)
		{
			return base.GetPassthroughAmount(outputSlot);
		}
		return 0;
	}

	// Token: 0x06000FBD RID: 4029 RVA: 0x00082694 File Offset: 0x00080894
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ioEntity == null)
		{
			info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		}
		info.msg.ioEntity.genericInt1 = this.counterNumber;
		info.msg.ioEntity.genericInt2 = this.GetPassthroughAmount(0);
		info.msg.ioEntity.genericInt3 = this.GetTarget();
	}

	// Token: 0x06000FBE RID: 4030 RVA: 0x00082708 File Offset: 0x00080908
	public void SetCounterNumber(int newNumber)
	{
		this.counterNumber = newNumber;
	}

	// Token: 0x06000FBF RID: 4031 RVA: 0x00007338 File Offset: 0x00005538
	public override void SendIONetworkUpdate()
	{
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000FC0 RID: 4032 RVA: 0x00082711 File Offset: 0x00080911
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateHasPower(inputAmount, inputSlot);
		}
	}

	// Token: 0x06000FC1 RID: 4033 RVA: 0x00082720 File Offset: 0x00080920
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (this.DisplayCounter() && inputAmount > 0 && inputSlot != 0)
		{
			int num = this.counterNumber;
			if (inputSlot == 1)
			{
				this.counterNumber++;
			}
			else if (inputSlot == 2)
			{
				this.counterNumber--;
				if (this.counterNumber < 0)
				{
					this.counterNumber = 0;
				}
			}
			else if (inputSlot == 3)
			{
				this.counterNumber = 0;
			}
			this.counterNumber = Mathf.Clamp(this.counterNumber, 0, 100);
			if (num != this.counterNumber)
			{
				this.MarkDirty();
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
		if (inputSlot == 0)
		{
			base.UpdateFromInput(inputAmount, inputSlot);
		}
	}

	// Token: 0x06000FC2 RID: 4034 RVA: 0x000827BC File Offset: 0x000809BC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			if (base.isServer)
			{
				this.counterNumber = info.msg.ioEntity.genericInt1;
			}
			this.targetCounterNumber = info.msg.ioEntity.genericInt3;
		}
	}

	// Token: 0x04000A05 RID: 2565
	private int counterNumber;

	// Token: 0x04000A06 RID: 2566
	private int targetCounterNumber = 10;

	// Token: 0x04000A07 RID: 2567
	public Canvas canvas;

	// Token: 0x04000A08 RID: 2568
	public CanvasGroup screenAlpha;

	// Token: 0x04000A09 RID: 2569
	public Text screenText;

	// Token: 0x04000A0A RID: 2570
	public const global::BaseEntity.Flags Flag_ShowPassthrough = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000A0B RID: 2571
	public GameObjectRef counterConfigPanel;

	// Token: 0x04000A0C RID: 2572
	public Color passthroughColor;

	// Token: 0x04000A0D RID: 2573
	public Color counterColor;
}
