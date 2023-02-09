using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000065 RID: 101
public class DoorCloser : BaseEntity
{
	// Token: 0x06000A2D RID: 2605 RVA: 0x0005CDC0 File Offset: 0x0005AFC0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DoorCloser.OnRpcMessage", 0))
		{
			if (rpc == 342802563U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Take ");
				}
				using (TimeWarning.New("RPC_Take", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(342802563U, "RPC_Take", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpc2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Take(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Take");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x000062DD File Offset: 0x000044DD
	public override float BoundsPadding()
	{
		return 1f;
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x0005CF28 File Offset: 0x0005B128
	public void Think()
	{
		base.Invoke(new Action(this.SendClose), this.delay);
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x0005CF44 File Offset: 0x0005B144
	public void SendClose()
	{
		BaseEntity parentEntity = base.GetParentEntity();
		if (this.children != null)
		{
			using (List<BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current != null)
					{
						base.Invoke(new Action(this.SendClose), this.delay);
						return;
					}
				}
			}
		}
		if (parentEntity)
		{
			parentEntity.SendMessage("CloseRequest");
		}
	}

	// Token: 0x06000A31 RID: 2609 RVA: 0x0005CFD4 File Offset: 0x0005B1D4
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Take(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!rpc.player.CanBuild())
		{
			return;
		}
		Door door = this.GetDoor();
		if (door == null)
		{
			return;
		}
		if (!door.GetPlayerLockPermission(rpc.player))
		{
			return;
		}
		Item item = ItemManager.Create(this.itemType, 1, this.skinID);
		if (item != null)
		{
			rpc.player.GiveItem(item, BaseEntity.GiveItemReason.Generic);
		}
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x0005D047 File Offset: 0x0005B247
	public Door GetDoor()
	{
		return base.GetParentEntity() as Door;
	}

	// Token: 0x04000698 RID: 1688
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemType;

	// Token: 0x04000699 RID: 1689
	public float delay = 3f;
}
