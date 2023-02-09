using System;
using System.Collections;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000074 RID: 116
public class FrankensteinTable : StorageContainer
{
	// Token: 0x06000AF0 RID: 2800 RVA: 0x000611D4 File Offset: 0x0005F3D4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FrankensteinTable.OnRpcMessage", 0))
		{
			if (rpc == 629197370U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - CreateFrankenstein ");
				}
				using (TimeWarning.New("CreateFrankenstein", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(629197370U, "CreateFrankenstein", this, player, 3f))
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
							this.CreateFrankenstein(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in CreateFrankenstein");
					}
				}
				return true;
			}
			if (rpc == 4797457U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RequestSleepFrankenstein ");
				}
				using (TimeWarning.New("RequestSleepFrankenstein", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4797457U, "RequestSleepFrankenstein", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RequestSleepFrankenstein(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RequestSleepFrankenstein");
					}
				}
				return true;
			}
			if (rpc == 3804893505U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RequestWakeFrankenstein ");
				}
				using (TimeWarning.New("RequestWakeFrankenstein", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3804893505U, "RequestWakeFrankenstein", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RequestWakeFrankenstein(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RequestWakeFrankenstein");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000AF1 RID: 2801 RVA: 0x00061630 File Offset: 0x0005F830
	public bool IsHeadItem(ItemDefinition itemDef)
	{
		return this.HeadItems.Contains(itemDef);
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x0006163E File Offset: 0x0005F83E
	public bool IsTorsoItem(ItemDefinition itemDef)
	{
		return this.TorsoItems.Contains(itemDef);
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x0006164C File Offset: 0x0005F84C
	public bool IsLegsItem(ItemDefinition itemDef)
	{
		return this.LegItems.Contains(itemDef);
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x0006165A File Offset: 0x0005F85A
	public bool HasValidItems(global::ItemContainer container)
	{
		return this.GetValidItems(container) != null;
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x00061668 File Offset: 0x0005F868
	public List<ItemDefinition> GetValidItems(global::ItemContainer container)
	{
		if (container == null)
		{
			return null;
		}
		if (container.itemList == null)
		{
			return null;
		}
		if (container.itemList.Count == 0)
		{
			return null;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		List<ItemDefinition> list = new List<ItemDefinition>();
		for (int i = 0; i < container.capacity; i++)
		{
			global::Item slot = container.GetSlot(i);
			if (slot != null)
			{
				this.CheckItem(slot.info, list, this.HeadItems, ref flag);
				this.CheckItem(slot.info, list, this.TorsoItems, ref flag2);
				this.CheckItem(slot.info, list, this.LegItems, ref flag3);
				if (flag && flag2 && flag3)
				{
					return list;
				}
			}
		}
		return null;
	}

	// Token: 0x06000AF6 RID: 2806 RVA: 0x00061710 File Offset: 0x0005F910
	public bool HasAllValidItems(List<ItemDefinition> items)
	{
		if (items == null)
		{
			return false;
		}
		if (items.Count < 3)
		{
			return false;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		foreach (ItemDefinition itemDefinition in items)
		{
			if (itemDefinition == null)
			{
				return false;
			}
			this.CheckItem(itemDefinition, null, this.HeadItems, ref flag);
			this.CheckItem(itemDefinition, null, this.TorsoItems, ref flag2);
			this.CheckItem(itemDefinition, null, this.LegItems, ref flag3);
		}
		return flag && flag2 && flag3;
	}

	// Token: 0x06000AF7 RID: 2807 RVA: 0x000617B8 File Offset: 0x0005F9B8
	private void CheckItem(ItemDefinition item, List<ItemDefinition> itemList, List<ItemDefinition> validItems, ref bool set)
	{
		if (set)
		{
			return;
		}
		if (validItems.Contains(item))
		{
			set = true;
			if (itemList != null)
			{
				itemList.Add(item);
			}
		}
	}

	// Token: 0x06000AF8 RID: 2808 RVA: 0x000617D8 File Offset: 0x0005F9D8
	public override void ServerInit()
	{
		base.ServerInit();
		global::ItemContainer inventory = base.inventory;
		inventory.canAcceptItem = (Func<global::Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<global::Item, int, bool>(this.CanAcceptItem));
		base.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
	}

	// Token: 0x06000AF9 RID: 2809 RVA: 0x0006182A File Offset: 0x0005FA2A
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x0006183C File Offset: 0x0005FA3C
	private bool CanAcceptItem(global::Item item, int targetSlot)
	{
		return item != null && ((this.HeadItems != null && this.IsHeadItem(item.info)) || (this.TorsoItems != null && this.IsTorsoItem(item.info)) || (this.LegItems != null && this.IsLegsItem(item.info)));
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x000059DD File Offset: 0x00003BDD
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void CreateFrankenstein(global::BaseEntity.RPCMessage msg)
	{
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x00061897 File Offset: 0x0005FA97
	private bool CanStartCreating(global::BasePlayer player)
	{
		return !this.waking && !(player == null) && !(player.PetEntity != null) && this.HasValidItems(base.inventory);
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x000618D0 File Offset: 0x0005FAD0
	private bool IsInventoryEmpty()
	{
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			if (base.inventory.GetSlot(i) != null)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x00061904 File Offset: 0x0005FB04
	private void ConsumeInventory()
	{
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot != null)
			{
				slot.UseItem(slot.amount);
			}
		}
		ItemManager.DoRemoves();
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x00061948 File Offset: 0x0005FB48
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RequestWakeFrankenstein(global::BaseEntity.RPCMessage msg)
	{
		this.WakeFrankenstein(msg.player);
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x00061958 File Offset: 0x0005FB58
	private void WakeFrankenstein(global::BasePlayer owner)
	{
		if (owner == null)
		{
			return;
		}
		if (!this.CanStartCreating(owner))
		{
			return;
		}
		this.waking = true;
		base.inventory.SetLocked(true);
		base.SendNetworkUpdateImmediate(false);
		base.StartCoroutine(this.DelayWakeFrankenstein(owner));
		base.ClientRPC(null, "CL_WakeFrankenstein");
	}

	// Token: 0x06000B01 RID: 2817 RVA: 0x000619AD File Offset: 0x0005FBAD
	private IEnumerator DelayWakeFrankenstein(global::BasePlayer owner)
	{
		yield return new WaitForSeconds(1.5f);
		yield return new WaitForSeconds(this.TableDownDuration);
		if (owner != null && owner.PetEntity != null)
		{
			base.inventory.SetLocked(false);
			base.SendNetworkUpdateImmediate(false);
			this.waking = false;
			yield break;
		}
		this.ItemsToUse = this.GetValidItems(base.inventory);
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.FrankensteinPrefab.resourcePath, this.SpawnLocation.position, this.SpawnLocation.rotation, false);
		baseEntity.enableSaving = false;
		baseEntity.gameObject.AwakeFromInstantiate();
		baseEntity.Spawn();
		this.EquipFrankenstein(baseEntity as FrankensteinPet);
		this.ConsumeInventory();
		base.inventory.SetLocked(false);
		base.SendNetworkUpdateImmediate(false);
		base.StartCoroutine(this.WaitForFrankensteinBrainInit(baseEntity as BasePet, owner));
		this.waking = false;
		yield return null;
		yield break;
	}

	// Token: 0x06000B02 RID: 2818 RVA: 0x000619C4 File Offset: 0x0005FBC4
	private void EquipFrankenstein(FrankensteinPet frank)
	{
		if (this.ItemsToUse == null)
		{
			return;
		}
		if (frank == null)
		{
			return;
		}
		if (frank.inventory == null)
		{
			return;
		}
		foreach (ItemDefinition template in this.ItemsToUse)
		{
			frank.inventory.GiveItem(ItemManager.Create(template, 1, 0UL), frank.inventory.containerWear);
		}
		if (this.WeaponItem != null)
		{
			base.StartCoroutine(frank.DelayEquipWeapon(this.WeaponItem, 1.5f));
		}
		this.ItemsToUse.Clear();
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x00061A84 File Offset: 0x0005FC84
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RequestSleepFrankenstein(global::BaseEntity.RPCMessage msg)
	{
		this.SleepFrankenstein(msg.player);
	}

	// Token: 0x06000B04 RID: 2820 RVA: 0x00061A94 File Offset: 0x0005FC94
	private void SleepFrankenstein(global::BasePlayer owner)
	{
		if (!this.IsInventoryEmpty())
		{
			return;
		}
		if (owner == null)
		{
			return;
		}
		if (owner.PetEntity == null)
		{
			return;
		}
		FrankensteinPet frankensteinPet = owner.PetEntity as FrankensteinPet;
		if (frankensteinPet == null)
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, frankensteinPet.transform.position) >= 5f)
		{
			return;
		}
		this.ReturnFrankensteinItems(frankensteinPet);
		ItemManager.DoRemoves();
		base.SendNetworkUpdateImmediate(false);
		frankensteinPet.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x00061B18 File Offset: 0x0005FD18
	private void ReturnFrankensteinItems(FrankensteinPet frank)
	{
		if (frank == null)
		{
			return;
		}
		if (frank.inventory == null)
		{
			return;
		}
		if (frank.inventory.containerWear == null)
		{
			return;
		}
		for (int i = 0; i < frank.inventory.containerWear.capacity; i++)
		{
			global::Item slot = frank.inventory.containerWear.GetSlot(i);
			if (slot != null)
			{
				slot.MoveToContainer(base.inventory, -1, true, false, null, true);
			}
		}
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x00061B8E File Offset: 0x0005FD8E
	private IEnumerator WaitForFrankensteinBrainInit(BasePet frankenstein, global::BasePlayer player)
	{
		yield return new WaitForEndOfFrame();
		frankenstein.ApplyPetStatModifiers();
		frankenstein.Brain.SetOwningPlayer(player);
		frankenstein.CreateMapMarker();
		player.SendClientPetLink();
		yield break;
	}

	// Token: 0x06000B07 RID: 2823 RVA: 0x00061BA4 File Offset: 0x0005FDA4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			return;
		}
		info.msg.FrankensteinTable = Facepunch.Pool.Get<ProtoBuf.FrankensteinTable>();
		info.msg.FrankensteinTable.itemIds = new List<int>();
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot != null)
			{
				info.msg.FrankensteinTable.itemIds.Add(slot.info.itemid);
			}
		}
	}

	// Token: 0x06000B08 RID: 2824 RVA: 0x00061C2C File Offset: 0x0005FE2C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	// Token: 0x0400071A RID: 1818
	public GameObjectRef FrankensteinPrefab;

	// Token: 0x0400071B RID: 1819
	public Transform SpawnLocation;

	// Token: 0x0400071C RID: 1820
	public ItemDefinition WeaponItem;

	// Token: 0x0400071D RID: 1821
	public List<ItemDefinition> HeadItems;

	// Token: 0x0400071E RID: 1822
	public List<ItemDefinition> TorsoItems;

	// Token: 0x0400071F RID: 1823
	public List<ItemDefinition> LegItems;

	// Token: 0x04000720 RID: 1824
	[HideInInspector]
	public List<ItemDefinition> ItemsToUse;

	// Token: 0x04000721 RID: 1825
	public FrankensteinTableVisuals TableVisuals;

	// Token: 0x04000722 RID: 1826
	[Header("Timings")]
	public float TableDownDuration = 0.9f;

	// Token: 0x04000723 RID: 1827
	private bool waking;
}
