using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200004F RID: 79
public class CardReader : IOEntity
{
	// Token: 0x060008D1 RID: 2257 RVA: 0x00053B4C File Offset: 0x00051D4C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CardReader.OnRpcMessage", 0))
		{
			if (rpc == 979061374U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerCardSwiped ");
				}
				using (TimeWarning.New("ServerCardSwiped", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(979061374U, "ServerCardSwiped", this, player, 3f))
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
							this.ServerCardSwiped(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ServerCardSwiped");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060008D2 RID: 2258 RVA: 0x00053CB4 File Offset: 0x00051EB4
	public override void ResetIOState()
	{
		base.ResetIOState();
		base.CancelInvoke(new Action(this.GrantCard));
		base.CancelInvoke(new Action(this.CancelAccess));
		this.CancelAccess();
	}

	// Token: 0x060008D3 RID: 2259 RVA: 0x00053CE6 File Offset: 0x00051EE6
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return base.GetPassthroughAmount(outputSlot);
	}

	// Token: 0x060008D4 RID: 2260 RVA: 0x00053CF9 File Offset: 0x00051EF9
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x060008D5 RID: 2261 RVA: 0x00053D03 File Offset: 0x00051F03
	public void CancelAccess()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.MarkDirty();
	}

	// Token: 0x060008D6 RID: 2262 RVA: 0x00053D15 File Offset: 0x00051F15
	public void FailCard()
	{
		Effect.server.Run(this.accessDeniedEffect.resourcePath, this.audioPosition.position, Vector3.up, null, false);
	}

	// Token: 0x060008D7 RID: 2263 RVA: 0x00053D3C File Offset: 0x00051F3C
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetFlag(this.AccessLevel1, this.accessLevel == 1, false, true);
		base.SetFlag(this.AccessLevel2, this.accessLevel == 2, false, true);
		base.SetFlag(this.AccessLevel3, this.accessLevel == 3, false, true);
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x00053D94 File Offset: 0x00051F94
	public void GrantCard()
	{
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.MarkDirty();
		Effect.server.Run(this.accessGrantedEffect.resourcePath, this.audioPosition.position, Vector3.up, null, false);
	}

	// Token: 0x060008D9 RID: 2265 RVA: 0x00053DC8 File Offset: 0x00051FC8
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerCardSwiped(BaseEntity.RPCMessage msg)
	{
		if (!this.IsPowered())
		{
			return;
		}
		if (Vector3Ex.Distance2D(msg.player.transform.position, base.transform.position) > 1f)
		{
			return;
		}
		if (base.IsInvoking(new Action(this.GrantCard)) || base.IsInvoking(new Action(this.FailCard)))
		{
			return;
		}
		uint uid = msg.read.UInt32();
		Keycard keycard = BaseNetworkable.serverEntities.Find(uid) as Keycard;
		Effect.server.Run(this.swipeEffect.resourcePath, this.audioPosition.position, Vector3.up, msg.player.net.connection, false);
		if (keycard != null)
		{
			Item item = keycard.GetItem();
			if (item != null && keycard.accessLevel == this.accessLevel && item.conditionNormalized > 0f)
			{
				base.Invoke(new Action(this.GrantCard), 0.5f);
				item.LoseCondition(1f);
				return;
			}
			base.Invoke(new Action(this.FailCard), 0.5f);
		}
	}

	// Token: 0x060008DA RID: 2266 RVA: 0x00053EE5 File Offset: 0x000520E5
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericInt1 = this.accessLevel;
		info.msg.ioEntity.genericFloat1 = this.accessDuration;
	}

	// Token: 0x060008DB RID: 2267 RVA: 0x00053F1C File Offset: 0x0005211C
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.accessLevel = info.msg.ioEntity.genericInt1;
			this.accessDuration = info.msg.ioEntity.genericFloat1;
		}
	}

	// Token: 0x040005DC RID: 1500
	public float accessDuration = 10f;

	// Token: 0x040005DD RID: 1501
	public int accessLevel;

	// Token: 0x040005DE RID: 1502
	public GameObjectRef accessGrantedEffect;

	// Token: 0x040005DF RID: 1503
	public GameObjectRef accessDeniedEffect;

	// Token: 0x040005E0 RID: 1504
	public GameObjectRef swipeEffect;

	// Token: 0x040005E1 RID: 1505
	public Transform audioPosition;

	// Token: 0x040005E2 RID: 1506
	public BaseEntity.Flags AccessLevel1 = BaseEntity.Flags.Reserved1;

	// Token: 0x040005E3 RID: 1507
	public BaseEntity.Flags AccessLevel2 = BaseEntity.Flags.Reserved2;

	// Token: 0x040005E4 RID: 1508
	public BaseEntity.Flags AccessLevel3 = BaseEntity.Flags.Reserved3;
}
