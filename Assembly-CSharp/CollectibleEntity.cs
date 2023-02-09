using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000057 RID: 87
public class CollectibleEntity : BaseEntity, IPrefabPreProcess
{
	// Token: 0x0600095F RID: 2399 RVA: 0x00057B28 File Offset: 0x00055D28
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CollectibleEntity.OnRpcMessage", 0))
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
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2778075470U, "Pickup", this, player, 3f))
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
			if (rpc == 3528769075U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - PickupEat ");
				}
				using (TimeWarning.New("PickupEat", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(3528769075U, "PickupEat", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.PickupEat(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in PickupEat");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000960 RID: 2400 RVA: 0x00057E28 File Offset: 0x00056028
	public bool IsFood(bool checkConsumeMod = false)
	{
		for (int i = 0; i < this.itemList.Length; i++)
		{
			if (this.itemList[i].itemDef.category == ItemCategory.Food && (!checkConsumeMod || this.itemList[i].itemDef.GetComponent<ItemModConsume>() != null))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000961 RID: 2401 RVA: 0x00057E80 File Offset: 0x00056080
	public void DoPickup(BasePlayer reciever, bool eat = false)
	{
		if (this.itemList == null)
		{
			return;
		}
		foreach (ItemAmount itemAmount in this.itemList)
		{
			Item item = ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, 0UL);
			if (item != null)
			{
				if (eat && item.info.category == ItemCategory.Food && reciever != null)
				{
					ItemModConsume component = item.info.GetComponent<ItemModConsume>();
					if (component != null)
					{
						component.DoAction(item, reciever);
						goto IL_C0;
					}
				}
				if (reciever)
				{
					reciever.GiveItem(item, BaseEntity.GiveItemReason.ResourceHarvested);
				}
				else
				{
					item.Drop(base.transform.position + Vector3.up * 0.5f, Vector3.up, default(Quaternion));
				}
			}
			IL_C0:;
		}
		this.itemList = null;
		if (this.pickupEffect.isValid)
		{
			Effect.server.Run(this.pickupEffect.resourcePath, base.transform.position, base.transform.up, null, false);
		}
		RandomItemDispenser randomItemDispenser = PrefabAttribute.server.Find<RandomItemDispenser>(this.prefabID);
		if (randomItemDispenser != null)
		{
			randomItemDispenser.DistributeItems(reciever, base.transform.position);
		}
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000962 RID: 2402 RVA: 0x00057FC9 File Offset: 0x000561C9
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void Pickup(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		this.DoPickup(msg.player, false);
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x00057FE6 File Offset: 0x000561E6
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void PickupEat(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		this.DoPickup(msg.player, true);
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x00058003 File Offset: 0x00056203
	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		if (serverside)
		{
			preProcess.RemoveComponent(base.GetComponent<Collider>());
		}
	}

	// Token: 0x04000634 RID: 1588
	public Translate.Phrase itemName;

	// Token: 0x04000635 RID: 1589
	public ItemAmount[] itemList;

	// Token: 0x04000636 RID: 1590
	public GameObjectRef pickupEffect;

	// Token: 0x04000637 RID: 1591
	public float xpScale = 1f;
}
