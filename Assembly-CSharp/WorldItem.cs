using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000EB RID: 235
public class WorldItem : global::BaseEntity
{
	// Token: 0x06001460 RID: 5216 RVA: 0x000A13C0 File Offset: 0x0009F5C0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("WorldItem.OnRpcMessage", 0))
		{
			if (rpc == 2778075470U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Pickup ");
				}
				using (TimeWarning.New("Pickup", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2778075470U, "Pickup", this, player, 3f))
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
							this.Pickup(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Pickup");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001461 RID: 5217 RVA: 0x000A1528 File Offset: 0x0009F728
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.item != null)
		{
			base.BroadcastMessage("OnItemChanged", this.item, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x06001462 RID: 5218 RVA: 0x000A154A File Offset: 0x0009F74A
	private void DoItemNetworking()
	{
		if (this._isInvokingSendItemUpdate)
		{
			return;
		}
		this._isInvokingSendItemUpdate = true;
		base.Invoke(new Action(this.SendItemUpdate), 0.1f);
	}

	// Token: 0x06001463 RID: 5219 RVA: 0x000A1574 File Offset: 0x0009F774
	private void SendItemUpdate()
	{
		this._isInvokingSendItemUpdate = false;
		if (this.item == null)
		{
			return;
		}
		using (UpdateItem updateItem = Facepunch.Pool.Get<UpdateItem>())
		{
			updateItem.item = this.item.Save(false, false);
			base.ClientRPC<UpdateItem>(null, "UpdateItem", updateItem);
		}
	}

	// Token: 0x06001464 RID: 5220 RVA: 0x000A15D4 File Offset: 0x0009F7D4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void Pickup(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (this.item == null)
		{
			return;
		}
		if (!this.allowPickup)
		{
			return;
		}
		base.ClientRPC(null, "PickupSound");
		global::Item item = this.item;
		this.RemoveItem();
		msg.player.GiveItem(item, global::BaseEntity.GiveItemReason.PickedUp);
		msg.player.SignalBroadcast(global::BaseEntity.Signal.Gesture, "pickup_item", null);
	}

	// Token: 0x06001465 RID: 5221 RVA: 0x000A163C File Offset: 0x0009F83C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (this.item == null)
		{
			return;
		}
		bool forDisk = info.forDisk;
		info.msg.worldItem = Facepunch.Pool.Get<ProtoBuf.WorldItem>();
		info.msg.worldItem.item = this.item.Save(forDisk, false);
	}

	// Token: 0x06001466 RID: 5222 RVA: 0x000A168D File Offset: 0x0009F88D
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.DestroyItem();
	}

	// Token: 0x06001467 RID: 5223 RVA: 0x000A169B File Offset: 0x0009F89B
	public override void SwitchParent(global::BaseEntity ent)
	{
		base.SetParent(ent, this.parentBone, false, false);
	}

	// Token: 0x06001468 RID: 5224 RVA: 0x000A16AC File Offset: 0x0009F8AC
	public override global::Item GetItem()
	{
		return this.item;
	}

	// Token: 0x06001469 RID: 5225 RVA: 0x000A16B4 File Offset: 0x0009F8B4
	public void InitializeItem(global::Item in_item)
	{
		if (this.item != null)
		{
			this.RemoveItem();
		}
		this.item = in_item;
		if (this.item == null)
		{
			return;
		}
		this.item.OnDirty += this.OnItemDirty;
		base.name = this.item.info.shortname + " (world)";
		this.item.SetWorldEntity(this);
		this.OnItemDirty(this.item);
	}

	// Token: 0x0600146A RID: 5226 RVA: 0x000A172F File Offset: 0x0009F92F
	public void RemoveItem()
	{
		if (this.item == null)
		{
			return;
		}
		this.item.OnDirty -= this.OnItemDirty;
		this.item = null;
	}

	// Token: 0x0600146B RID: 5227 RVA: 0x000A1759 File Offset: 0x0009F959
	public void DestroyItem()
	{
		if (this.item == null)
		{
			return;
		}
		this.item.OnDirty -= this.OnItemDirty;
		this.item.Remove(0f);
		this.item = null;
	}

	// Token: 0x0600146C RID: 5228 RVA: 0x000A1793 File Offset: 0x0009F993
	protected virtual void OnItemDirty(global::Item in_item)
	{
		Assert.IsTrue(this.item == in_item, "WorldItem:OnItemDirty - dirty item isn't ours!");
		if (this.item != null)
		{
			base.BroadcastMessage("OnItemChanged", this.item, SendMessageOptions.DontRequireReceiver);
		}
		this.DoItemNetworking();
	}

	// Token: 0x0600146D RID: 5229 RVA: 0x000A17C8 File Offset: 0x0009F9C8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.worldItem == null)
		{
			return;
		}
		if (info.msg.worldItem.item == null)
		{
			return;
		}
		global::Item item = ItemManager.Load(info.msg.worldItem.item, this.item, base.isServer);
		if (item != null)
		{
			this.InitializeItem(item);
		}
	}

	// Token: 0x170001AA RID: 426
	// (get) Token: 0x0600146E RID: 5230 RVA: 0x000A1829 File Offset: 0x0009FA29
	public override global::BaseEntity.TraitFlag Traits
	{
		get
		{
			if (this.item != null)
			{
				return this.item.Traits;
			}
			return base.Traits;
		}
	}

	// Token: 0x0600146F RID: 5231 RVA: 0x000A1848 File Offset: 0x0009FA48
	public override void Eat(BaseNpc baseNpc, float timeSpent)
	{
		if (this.eatSeconds <= 0f)
		{
			return;
		}
		this.eatSeconds -= timeSpent;
		baseNpc.AddCalories(this.caloriesPerSecond * timeSpent);
		if (this.eatSeconds < 0f)
		{
			this.DestroyItem();
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06001470 RID: 5232 RVA: 0x000A189C File Offset: 0x0009FA9C
	public override string ToString()
	{
		if (this._name == null)
		{
			if (base.isServer)
			{
				this._name = string.Format("{1}[{0}] {2}", (this.net != null) ? this.net.ID : 0U, base.ShortPrefabName, this.IsUnityNull<global::WorldItem>() ? "NULL" : base.name);
			}
			else
			{
				this._name = base.ShortPrefabName;
			}
		}
		return this._name;
	}

	// Token: 0x04000CF7 RID: 3319
	private bool _isInvokingSendItemUpdate;

	// Token: 0x04000CF8 RID: 3320
	[Header("WorldItem")]
	public bool allowPickup = true;

	// Token: 0x04000CF9 RID: 3321
	[NonSerialized]
	public global::Item item;

	// Token: 0x04000CFA RID: 3322
	protected float eatSeconds = 10f;

	// Token: 0x04000CFB RID: 3323
	protected float caloriesPerSecond = 1f;
}
