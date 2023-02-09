using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000BC RID: 188
public class RFReceiver : IOEntity, IRFObject
{
	// Token: 0x060010B7 RID: 4279 RVA: 0x00088978 File Offset: 0x00086B78
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RFReceiver.OnRpcMessage", 0))
		{
			if (rpc == 2778616053U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerSetFrequency ");
				}
				using (TimeWarning.New("ServerSetFrequency", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(2778616053U, "ServerSetFrequency", this, player, 3f))
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
							this.ServerSetFrequency(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ServerSetFrequency");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060010B8 RID: 4280 RVA: 0x00088AE0 File Offset: 0x00086CE0
	public int GetFrequency()
	{
		return this.frequency;
	}

	// Token: 0x060010B9 RID: 4281 RVA: 0x00006C79 File Offset: 0x00004E79
	public override bool WantsPower()
	{
		return base.IsOn();
	}

	// Token: 0x060010BA RID: 4282 RVA: 0x0005E44D File Offset: 0x0005C64D
	public override void ResetIOState()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x060010BB RID: 4283 RVA: 0x0005E459 File Offset: 0x0005C659
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.GetCurrentEnergy();
	}

	// Token: 0x060010BC RID: 4284 RVA: 0x000299AB File Offset: 0x00027BAB
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x060010BD RID: 4285 RVA: 0x0005B3DA File Offset: 0x000595DA
	public float GetMaxRange()
	{
		return 100000f;
	}

	// Token: 0x060010BE RID: 4286 RVA: 0x00088AE8 File Offset: 0x00086CE8
	public override void Init()
	{
		base.Init();
		RFManager.AddListener(this.frequency, this);
	}

	// Token: 0x060010BF RID: 4287 RVA: 0x00088AFC File Offset: 0x00086CFC
	internal override void DoServerDestroy()
	{
		RFManager.RemoveListener(this.frequency, this);
		base.DoServerDestroy();
	}

	// Token: 0x060010C0 RID: 4288 RVA: 0x00088B10 File Offset: 0x00086D10
	public void RFSignalUpdate(bool on)
	{
		if (base.IsDestroyed)
		{
			return;
		}
		if (base.IsOn() == on)
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, on, false, true);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
	}

	// Token: 0x060010C1 RID: 4289 RVA: 0x00088B3C File Offset: 0x00086D3C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerSetFrequency(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || !msg.player.CanBuild())
		{
			return;
		}
		int newFrequency = msg.read.Int32();
		RFManager.ChangeFrequency(this.frequency, newFrequency, this, true, true);
		this.frequency = newFrequency;
		this.MarkDirty();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060010C2 RID: 4290 RVA: 0x00088B94 File Offset: 0x00086D94
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericInt1 = this.frequency;
	}

	// Token: 0x060010C3 RID: 4291 RVA: 0x00088BB3 File Offset: 0x00086DB3
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.frequency = info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x04000A6D RID: 2669
	public int frequency;

	// Token: 0x04000A6E RID: 2670
	public GameObjectRef frequencyPanelPrefab;
}
