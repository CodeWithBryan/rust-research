using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000062 RID: 98
public class Detonator : global::HeldEntity, IRFObject
{
	// Token: 0x060009F0 RID: 2544 RVA: 0x0005B0D8 File Offset: 0x000592D8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Detonator.OnRpcMessage", 0))
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
			if (rpc == 1106698135U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetPressed ");
				}
				using (TimeWarning.New("SetPressed", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage pressed = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetPressed(pressed);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in SetPressed");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x0005B338 File Offset: 0x00059538
	[global::BaseEntity.RPC_Server]
	public void SetPressed(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || msg.player != base.GetOwnerPlayer())
		{
			return;
		}
		bool flag = base.HasFlag(global::BaseEntity.Flags.On);
		bool flag2 = msg.read.Bit();
		this.InternalSetPressed(flag2);
		if (flag != flag2)
		{
			Effect.server.Run(flag2 ? this.attackEffect.resourcePath : this.unAttackEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		}
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x0005B3B2 File Offset: 0x000595B2
	internal void InternalSetPressed(bool pressed)
	{
		base.SetFlag(global::BaseEntity.Flags.On, pressed, false, true);
		if (pressed)
		{
			RFManager.AddBroadcaster(this.frequency, this);
			return;
		}
		RFManager.RemoveBroadcaster(this.frequency, this);
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x000299AB File Offset: 0x00027BAB
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x0005B3DA File Offset: 0x000595DA
	public float GetMaxRange()
	{
		return 100000f;
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x000059DD File Offset: 0x00003BDD
	public void RFSignalUpdate(bool on)
	{
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x0005B3E1 File Offset: 0x000595E1
	public override void SetHeld(bool bHeld)
	{
		if (!bHeld)
		{
			this.InternalSetPressed(false);
		}
		base.SetHeld(bHeld);
	}

	// Token: 0x060009F7 RID: 2551 RVA: 0x0005B3F4 File Offset: 0x000595F4
	[global::BaseEntity.RPC_Server]
	public void ServerSetFrequency(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || !msg.player.CanBuild())
		{
			return;
		}
		if (base.GetOwnerPlayer() != msg.player)
		{
			return;
		}
		if (UnityEngine.Time.time < this.nextChangeTime)
		{
			return;
		}
		this.nextChangeTime = UnityEngine.Time.time + 2f;
		int newFrequency = msg.read.Int32();
		if (RFManager.IsReserved(newFrequency))
		{
			RFManager.ReserveErrorPrint(msg.player);
			return;
		}
		RFManager.ChangeFrequency(this.frequency, newFrequency, this, false, base.IsOn());
		this.frequency = newFrequency;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		global::Item item = this.GetItem();
		if (item != null)
		{
			if (item.instanceData == null)
			{
				item.instanceData = new ProtoBuf.Item.InstanceData();
				item.instanceData.ShouldPool = false;
			}
			item.instanceData.dataInt = this.frequency;
			item.MarkDirty();
		}
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x0005B4D2 File Offset: 0x000596D2
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ioEntity == null)
		{
			info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		}
		info.msg.ioEntity.genericInt1 = this.frequency;
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x0005B50E File Offset: 0x0005970E
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.frequency = info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x0005B53A File Offset: 0x0005973A
	public int GetFrequency()
	{
		return this.frequency;
	}

	// Token: 0x04000672 RID: 1650
	public int frequency = 55;

	// Token: 0x04000673 RID: 1651
	private float timeSinceDeploy;

	// Token: 0x04000674 RID: 1652
	public GameObjectRef frequencyPanelPrefab;

	// Token: 0x04000675 RID: 1653
	public GameObjectRef attackEffect;

	// Token: 0x04000676 RID: 1654
	public GameObjectRef unAttackEffect;

	// Token: 0x04000677 RID: 1655
	private float nextChangeTime;
}
