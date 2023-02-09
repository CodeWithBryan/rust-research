using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B5 RID: 181
public class Recycler : StorageContainer
{
	// Token: 0x06001025 RID: 4133 RVA: 0x00084514 File Offset: 0x00082714
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Recycler.OnRpcMessage", 0))
		{
			if (rpc == 4167839872U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SVSwitch ");
				}
				using (TimeWarning.New("SVSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(4167839872U, "SVSwitch", this, player, 3f))
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
							this.SVSwitch(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SVSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001026 RID: 4134 RVA: 0x0008467C File Offset: 0x0008287C
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x06001027 RID: 4135 RVA: 0x00084684 File Offset: 0x00082884
	private bool CanBeRecycled(Item item)
	{
		return item != null && item.info.Blueprint != null;
	}

	// Token: 0x06001028 RID: 4136 RVA: 0x0008469C File Offset: 0x0008289C
	public override void ServerInit()
	{
		base.ServerInit();
		ItemContainer inventory = base.inventory;
		inventory.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<Item, int, bool>(this.RecyclerItemFilter));
	}

	// Token: 0x06001029 RID: 4137 RVA: 0x000846CC File Offset: 0x000828CC
	public bool RecyclerItemFilter(Item item, int targetSlot)
	{
		int num = Mathf.CeilToInt((float)base.inventory.capacity * 0.5f);
		if (targetSlot == -1)
		{
			bool flag = false;
			for (int i = 0; i < num; i++)
			{
				if (!base.inventory.SlotTaken(item, i))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return targetSlot >= num || this.CanBeRecycled(item);
	}

	// Token: 0x0600102A RID: 4138 RVA: 0x00084728 File Offset: 0x00082928
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	private void SVSwitch(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (flag == base.IsOn())
		{
			return;
		}
		if (msg.player == null)
		{
			return;
		}
		if (flag && !this.HasRecyclable())
		{
			return;
		}
		if (flag)
		{
			foreach (Item item in base.inventory.itemList)
			{
				item.CollectedForCrafting(msg.player);
			}
			this.StartRecycling();
			return;
		}
		this.StopRecycling();
	}

	// Token: 0x0600102B RID: 4139 RVA: 0x000847C4 File Offset: 0x000829C4
	public bool MoveItemToOutput(Item newItem)
	{
		int num = -1;
		for (int i = 6; i < 12; i++)
		{
			Item slot = base.inventory.GetSlot(i);
			if (slot == null)
			{
				num = i;
				break;
			}
			if (slot.CanStack(newItem))
			{
				if (slot.amount + newItem.amount <= slot.MaxStackable())
				{
					num = i;
					break;
				}
				int num2 = Mathf.Min(slot.MaxStackable() - slot.amount, newItem.amount);
				newItem.UseItem(num2);
				slot.amount += num2;
				slot.MarkDirty();
				newItem.MarkDirty();
			}
			if (newItem.amount <= 0)
			{
				return true;
			}
		}
		if (num != -1 && newItem.MoveToContainer(base.inventory, num, true, false, null, true))
		{
			return true;
		}
		newItem.Drop(base.transform.position + new Vector3(0f, 2f, 0f), this.GetInheritedDropVelocity() + base.transform.forward * 2f, default(Quaternion));
		return false;
	}

	// Token: 0x0600102C RID: 4140 RVA: 0x000848D0 File Offset: 0x00082AD0
	public bool HasRecyclable()
	{
		for (int i = 0; i < 6; i++)
		{
			Item slot = base.inventory.GetSlot(i);
			if (slot != null && slot.info.Blueprint != null)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600102D RID: 4141 RVA: 0x00084910 File Offset: 0x00082B10
	public void RecycleThink()
	{
		bool flag = false;
		float num = this.recycleEfficiency;
		for (int i = 0; i < 6; i++)
		{
			Item slot = base.inventory.GetSlot(i);
			if (this.CanBeRecycled(slot))
			{
				if (slot.hasCondition)
				{
					num = Mathf.Clamp01(num * Mathf.Clamp(slot.conditionNormalized * slot.maxConditionNormalized, 0.1f, 1f));
				}
				int num2 = 1;
				if (slot.amount > 1)
				{
					num2 = Mathf.CeilToInt(Mathf.Min((float)slot.amount, (float)slot.MaxStackable() * 0.1f));
				}
				if (slot.info.Blueprint.scrapFromRecycle > 0)
				{
					int num3 = slot.info.Blueprint.scrapFromRecycle * num2;
					if (slot.MaxStackable() == 1 && slot.hasCondition)
					{
						num3 = Mathf.CeilToInt((float)num3 * slot.conditionNormalized);
					}
					if (num3 >= 1)
					{
						Item newItem = ItemManager.CreateByName("scrap", num3, 0UL);
						this.MoveItemToOutput(newItem);
					}
				}
				if (!string.IsNullOrEmpty(slot.info.Blueprint.RecycleStat))
				{
					List<BasePlayer> list = Facepunch.Pool.GetList<BasePlayer>();
					global::Vis.Entities<BasePlayer>(base.transform.position, 3f, list, 131072, QueryTriggerInteraction.Collide);
					foreach (BasePlayer basePlayer in list)
					{
						if (basePlayer.IsAlive() && !basePlayer.IsSleeping() && basePlayer.inventory.loot.entitySource == this)
						{
							basePlayer.stats.Add(slot.info.Blueprint.RecycleStat, num2, (global::Stats)5);
							basePlayer.stats.Save(false);
						}
					}
					Facepunch.Pool.FreeList<BasePlayer>(ref list);
				}
				slot.UseItem(num2);
				using (List<ItemAmount>.Enumerator enumerator2 = slot.info.Blueprint.ingredients.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ItemAmount itemAmount = enumerator2.Current;
						if (!(itemAmount.itemDef.shortname == "scrap"))
						{
							float num4 = itemAmount.amount / (float)slot.info.Blueprint.amountToCreate;
							int num5 = 0;
							if (num4 <= 1f)
							{
								for (int j = 0; j < num2; j++)
								{
									if (UnityEngine.Random.Range(0f, 1f) <= num4 * num)
									{
										num5++;
									}
								}
							}
							else
							{
								num5 = Mathf.CeilToInt(Mathf.Clamp(num4 * num * UnityEngine.Random.Range(1f, 1f), 0f, itemAmount.amount)) * num2;
							}
							if (num5 > 0)
							{
								int num6 = Mathf.CeilToInt((float)num5 / (float)itemAmount.itemDef.stackable);
								for (int k = 0; k < num6; k++)
								{
									int num7 = (num5 > itemAmount.itemDef.stackable) ? itemAmount.itemDef.stackable : num5;
									Item newItem2 = ItemManager.Create(itemAmount.itemDef, num7, 0UL);
									if (!this.MoveItemToOutput(newItem2))
									{
										flag = true;
									}
									num5 -= num7;
									if (num5 <= 0)
									{
										break;
									}
								}
							}
						}
					}
					break;
				}
			}
		}
		if (flag || !this.HasRecyclable())
		{
			this.StopRecycling();
		}
	}

	// Token: 0x0600102E RID: 4142 RVA: 0x00084C84 File Offset: 0x00082E84
	public void StartRecycling()
	{
		if (base.IsOn())
		{
			return;
		}
		base.InvokeRepeating(new Action(this.RecycleThink), 5f, 5f);
		Effect.server.Run(this.startSound.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x0600102F RID: 4143 RVA: 0x00084CE8 File Offset: 0x00082EE8
	public void StopRecycling()
	{
		base.CancelInvoke(new Action(this.RecycleThink));
		if (!base.IsOn())
		{
			return;
		}
		Effect.server.Run(this.stopSound.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x04000A3B RID: 2619
	public float recycleEfficiency = 0.5f;

	// Token: 0x04000A3C RID: 2620
	public SoundDefinition grindingLoopDef;

	// Token: 0x04000A3D RID: 2621
	public GameObjectRef startSound;

	// Token: 0x04000A3E RID: 2622
	public GameObjectRef stopSound;
}
