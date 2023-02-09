using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200006A RID: 106
public class ElectricalBranch : IOEntity
{
	// Token: 0x06000A61 RID: 2657 RVA: 0x0005E044 File Offset: 0x0005C244
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ElectricalBranch.OnRpcMessage", 0))
		{
			if (rpc == 643124146U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetBranchOffPower ");
				}
				using (TimeWarning.New("SetBranchOffPower", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(643124146U, "SetBranchOffPower", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage branchOffPower = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetBranchOffPower(branchOffPower);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SetBranchOffPower");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A62 RID: 2658 RVA: 0x0005E1AC File Offset: 0x0005C3AC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetBranchOffPower(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (player == null || !player.CanBuild())
		{
			return;
		}
		if (UnityEngine.Time.time < this.nextChangeTime)
		{
			return;
		}
		this.nextChangeTime = UnityEngine.Time.time + 1f;
		int value = msg.read.Int32();
		value = Mathf.Clamp(value, 2, 10000000);
		this.branchAmount = value;
		base.MarkDirtyForceUpdateOutputs();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000A63 RID: 2659 RVA: 0x0005E21E File Offset: 0x0005C41E
	public override bool AllowDrainFrom(int outputSlot)
	{
		return outputSlot != 1;
	}

	// Token: 0x06000A64 RID: 2660 RVA: 0x0005E227 File Offset: 0x0005C427
	public override int DesiredPower()
	{
		return this.branchAmount;
	}

	// Token: 0x06000A65 RID: 2661 RVA: 0x0005E22F File Offset: 0x0005C42F
	public void SetBranchAmount(int newAmount)
	{
		newAmount = Mathf.Clamp(newAmount, 2, 100000000);
		this.branchAmount = newAmount;
	}

	// Token: 0x06000A66 RID: 2662 RVA: 0x0005E246 File Offset: 0x0005C446
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot == 0)
		{
			return Mathf.Clamp(this.GetCurrentEnergy() - this.branchAmount, 0, this.GetCurrentEnergy());
		}
		if (outputSlot == 1)
		{
			return Mathf.Min(this.GetCurrentEnergy(), this.branchAmount);
		}
		return 0;
	}

	// Token: 0x06000A67 RID: 2663 RVA: 0x0005E27C File Offset: 0x0005C47C
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericInt1 = this.branchAmount;
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x0005E29B File Offset: 0x0005C49B
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.branchAmount = info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x040006AE RID: 1710
	public int branchAmount = 2;

	// Token: 0x040006AF RID: 1711
	public GameObjectRef branchPanelPrefab;

	// Token: 0x040006B0 RID: 1712
	private float nextChangeTime;
}
