using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200001A RID: 26
public class StrobeLight : IOEntity
{
	// Token: 0x06000067 RID: 103 RVA: 0x00003EE0 File Offset: 0x000020E0
	public float GetFrequency()
	{
		if (base.HasFlag(BaseEntity.Flags.Reserved6))
		{
			return this.speedSlow;
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved7))
		{
			return this.speedMed;
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved8))
		{
			return this.speedFast;
		}
		return this.speedSlow;
	}

	// Token: 0x06000068 RID: 104 RVA: 0x00003F30 File Offset: 0x00002130
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetStrobe(BaseEntity.RPCMessage msg)
	{
		bool strobe = msg.read.Bit();
		this.SetStrobe(strobe);
	}

	// Token: 0x06000069 RID: 105 RVA: 0x00003F50 File Offset: 0x00002150
	private void SetStrobe(bool wantsOn)
	{
		this.ServerEnableStrobing(wantsOn);
		if (wantsOn)
		{
			this.UpdateSpeedFlags();
		}
	}

	// Token: 0x0600006A RID: 106 RVA: 0x00003F64 File Offset: 0x00002164
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetStrobeSpeed(BaseEntity.RPCMessage msg)
	{
		int num = msg.read.Int32();
		this.currentSpeed = num;
		this.UpdateSpeedFlags();
	}

	// Token: 0x0600006B RID: 107 RVA: 0x00003F8C File Offset: 0x0000218C
	public void UpdateSpeedFlags()
	{
		base.SetFlag(BaseEntity.Flags.Reserved6, this.currentSpeed == 1, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved7, this.currentSpeed == 2, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved8, this.currentSpeed == 3, false, true);
	}

	// Token: 0x0600006C RID: 108 RVA: 0x00003FDC File Offset: 0x000021DC
	public void ServerEnableStrobing(bool wantsOn)
	{
		base.SetFlag(BaseEntity.Flags.Reserved6, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved7, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved8, false, false, true);
		base.SetFlag(BaseEntity.Flags.On, wantsOn, false, true);
		base.SendNetworkUpdateImmediate(false);
		this.UpdateSpeedFlags();
		if (wantsOn)
		{
			base.InvokeRandomized(new Action(this.SelfDamage), 0f, 10f, 0.1f);
			return;
		}
		base.CancelInvoke(new Action(this.SelfDamage));
	}

	// Token: 0x0600006D RID: 109 RVA: 0x00004064 File Offset: 0x00002264
	public void SelfDamage()
	{
		float num = this.burnRate / this.lifeTimeSeconds;
		base.Hurt(num * this.MaxHealth(), DamageType.Decay, this, false);
	}

	// Token: 0x0600006E RID: 110 RVA: 0x00004094 File Offset: 0x00002294
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		bool strobe = false;
		if (inputSlot == 0)
		{
			strobe = (inputAmount > 0);
		}
		else if (inputSlot == 1)
		{
			if (inputAmount == 0)
			{
				return;
			}
			strobe = true;
		}
		else if (inputSlot == 2)
		{
			if (inputAmount == 0)
			{
				return;
			}
			strobe = false;
		}
		this.SetStrobe(strobe);
	}

	// Token: 0x0600006F RID: 111 RVA: 0x000040D4 File Offset: 0x000022D4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("StrobeLight.OnRpcMessage", 0))
		{
			if (rpc == 1433326740U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetStrobe ");
				}
				using (TimeWarning.New("SetStrobe", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1433326740U, "SetStrobe", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage strobe = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetStrobe(strobe);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SetStrobe");
					}
				}
				return true;
			}
			if (rpc == 1814332702U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetStrobeSpeed ");
				}
				using (TimeWarning.New("SetStrobeSpeed", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1814332702U, "SetStrobeSpeed", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage strobeSpeed = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetStrobeSpeed(strobeSpeed);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in SetStrobeSpeed");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x04000064 RID: 100
	public float frequency;

	// Token: 0x04000065 RID: 101
	public MeshRenderer lightMesh;

	// Token: 0x04000066 RID: 102
	public Light strobeLight;

	// Token: 0x04000067 RID: 103
	private float speedSlow = 10f;

	// Token: 0x04000068 RID: 104
	private float speedMed = 20f;

	// Token: 0x04000069 RID: 105
	private float speedFast = 40f;

	// Token: 0x0400006A RID: 106
	public float burnRate = 10f;

	// Token: 0x0400006B RID: 107
	public float lifeTimeSeconds = 21600f;

	// Token: 0x0400006C RID: 108
	public const BaseEntity.Flags Flag_Slow = BaseEntity.Flags.Reserved6;

	// Token: 0x0400006D RID: 109
	public const BaseEntity.Flags Flag_Med = BaseEntity.Flags.Reserved7;

	// Token: 0x0400006E RID: 110
	public const BaseEntity.Flags Flag_Fast = BaseEntity.Flags.Reserved8;

	// Token: 0x0400006F RID: 111
	private int currentSpeed = 1;
}
