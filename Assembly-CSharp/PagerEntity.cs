using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A3 RID: 163
public class PagerEntity : global::BaseEntity, IRFObject
{
	// Token: 0x06000EF0 RID: 3824 RVA: 0x0007CA4C File Offset: 0x0007AC4C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PagerEntity.OnRpcMessage", 0))
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
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2778616053U, "ServerSetFrequency", this, player, 3f))
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

	// Token: 0x06000EF1 RID: 3825 RVA: 0x0007CBB4 File Offset: 0x0007ADB4
	public int GetFrequency()
	{
		return this.frequency;
	}

	// Token: 0x06000EF2 RID: 3826 RVA: 0x0007CBBC File Offset: 0x0007ADBC
	public override void SwitchParent(global::BaseEntity ent)
	{
		base.SetParent(ent, false, true);
	}

	// Token: 0x06000EF3 RID: 3827 RVA: 0x0007CBC7 File Offset: 0x0007ADC7
	public override void ServerInit()
	{
		base.ServerInit();
		RFManager.AddListener(this.frequency, this);
	}

	// Token: 0x06000EF4 RID: 3828 RVA: 0x0007CBDB File Offset: 0x0007ADDB
	internal override void DoServerDestroy()
	{
		RFManager.RemoveListener(this.frequency, this);
		base.DoServerDestroy();
	}

	// Token: 0x06000EF5 RID: 3829 RVA: 0x000299AB File Offset: 0x00027BAB
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06000EF6 RID: 3830 RVA: 0x0007CBEF File Offset: 0x0007ADEF
	public float GetMaxRange()
	{
		return float.PositiveInfinity;
	}

	// Token: 0x06000EF7 RID: 3831 RVA: 0x0007CBF8 File Offset: 0x0007ADF8
	public void RFSignalUpdate(bool on)
	{
		if (base.IsDestroyed)
		{
			return;
		}
		bool flag = base.IsOn();
		if (on != flag)
		{
			base.SetFlag(global::BaseEntity.Flags.On, on, false, true);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000EF8 RID: 3832 RVA: 0x0007CC2A File Offset: 0x0007AE2A
	public void SetSilentMode(bool wantsSilent)
	{
		base.SetFlag(PagerEntity.Flag_Silent, wantsSilent, false, true);
	}

	// Token: 0x06000EF9 RID: 3833 RVA: 0x0005E44D File Offset: 0x0005C64D
	public void SetOff()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x06000EFA RID: 3834 RVA: 0x0007CC3A File Offset: 0x0007AE3A
	public void ChangeFrequency(int newFreq)
	{
		RFManager.ChangeFrequency(this.frequency, newFreq, this, true, true);
		this.frequency = newFreq;
	}

	// Token: 0x06000EFB RID: 3835 RVA: 0x0007CC54 File Offset: 0x0007AE54
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerSetFrequency(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || !msg.player.CanBuild())
		{
			return;
		}
		if (UnityEngine.Time.time < this.nextChangeTime)
		{
			return;
		}
		this.nextChangeTime = UnityEngine.Time.time + 2f;
		int newFrequency = msg.read.Int32();
		RFManager.ChangeFrequency(this.frequency, newFrequency, this, true, true);
		this.frequency = newFrequency;
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000EFC RID: 3836 RVA: 0x0007CCC5 File Offset: 0x0007AEC5
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericInt1 = this.frequency;
	}

	// Token: 0x06000EFD RID: 3837 RVA: 0x0007CCF4 File Offset: 0x0007AEF4
	internal override void OnParentRemoved()
	{
		base.SetParent(null, false, true);
	}

	// Token: 0x06000EFE RID: 3838 RVA: 0x0007CCFF File Offset: 0x0007AEFF
	public void OnParentDestroying()
	{
		if (base.isServer)
		{
			base.transform.parent = null;
		}
	}

	// Token: 0x06000EFF RID: 3839 RVA: 0x0007CD18 File Offset: 0x0007AF18
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.frequency = info.msg.ioEntity.genericInt1;
		}
		if (base.isServer && info.fromDisk)
		{
			this.ChangeFrequency(this.frequency);
		}
	}

	// Token: 0x040009C6 RID: 2502
	public static global::BaseEntity.Flags Flag_Silent = global::BaseEntity.Flags.Reserved1;

	// Token: 0x040009C7 RID: 2503
	private int frequency = 55;

	// Token: 0x040009C8 RID: 2504
	public float beepRepeat = 2f;

	// Token: 0x040009C9 RID: 2505
	public GameObjectRef pagerEffect;

	// Token: 0x040009CA RID: 2506
	public GameObjectRef silentEffect;

	// Token: 0x040009CB RID: 2507
	private float nextChangeTime;
}
