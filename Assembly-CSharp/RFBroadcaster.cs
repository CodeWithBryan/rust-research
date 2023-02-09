using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000BB RID: 187
public class RFBroadcaster : IOEntity, IRFObject
{
	// Token: 0x060010AA RID: 4266 RVA: 0x00088670 File Offset: 0x00086870
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RFBroadcaster.OnRpcMessage", 0))
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

	// Token: 0x060010AB RID: 4267 RVA: 0x000887D8 File Offset: 0x000869D8
	public int GetFrequency()
	{
		return this.frequency;
	}

	// Token: 0x060010AC RID: 4268 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool WantsPower()
	{
		return true;
	}

	// Token: 0x060010AD RID: 4269 RVA: 0x000299AB File Offset: 0x00027BAB
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x060010AE RID: 4270 RVA: 0x0005B3DA File Offset: 0x000595DA
	public float GetMaxRange()
	{
		return 100000f;
	}

	// Token: 0x060010AF RID: 4271 RVA: 0x000059DD File Offset: 0x00003BDD
	public void RFSignalUpdate(bool on)
	{
	}

	// Token: 0x060010B0 RID: 4272 RVA: 0x000887E0 File Offset: 0x000869E0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerSetFrequency(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || !msg.player.CanBuild())
		{
			return;
		}
		if (!this.playerUsable)
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
		RFManager.ChangeFrequency(this.frequency, newFrequency, this, false, this.IsPowered());
		this.frequency = newFrequency;
		this.MarkDirty();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060010B1 RID: 4273 RVA: 0x00088879 File Offset: 0x00086A79
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericInt1 = this.frequency;
	}

	// Token: 0x060010B2 RID: 4274 RVA: 0x00088898 File Offset: 0x00086A98
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		if (inputAmount > 0)
		{
			base.CancelInvoke(new Action(this.StopBroadcasting));
			RFManager.AddBroadcaster(this.frequency, this);
			base.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
			this.nextStopTime = UnityEngine.Time.time + 1f;
			return;
		}
		base.Invoke(new Action(this.StopBroadcasting), Mathf.Clamp01(this.nextStopTime - UnityEngine.Time.time));
	}

	// Token: 0x060010B3 RID: 4275 RVA: 0x0008890C File Offset: 0x00086B0C
	public void StopBroadcasting()
	{
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		RFManager.RemoveBroadcaster(this.frequency, this);
	}

	// Token: 0x060010B4 RID: 4276 RVA: 0x00088928 File Offset: 0x00086B28
	internal override void DoServerDestroy()
	{
		RFManager.RemoveBroadcaster(this.frequency, this);
		base.DoServerDestroy();
	}

	// Token: 0x060010B5 RID: 4277 RVA: 0x0008893C File Offset: 0x00086B3C
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.frequency = info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x04000A67 RID: 2663
	public int frequency;

	// Token: 0x04000A68 RID: 2664
	public GameObjectRef frequencyPanelPrefab;

	// Token: 0x04000A69 RID: 2665
	public const BaseEntity.Flags Flag_Broadcasting = BaseEntity.Flags.Reserved3;

	// Token: 0x04000A6A RID: 2666
	public bool playerUsable = true;

	// Token: 0x04000A6B RID: 2667
	private float nextChangeTime;

	// Token: 0x04000A6C RID: 2668
	private float nextStopTime;
}
