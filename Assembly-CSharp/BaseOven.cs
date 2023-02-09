using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200003F RID: 63
public class BaseOven : StorageContainer, ISplashable, IIndustrialStorage
{
	// Token: 0x060004C9 RID: 1225 RVA: 0x00034BAC File Offset: 0x00032DAC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseOven.OnRpcMessage", 0))
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
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4167839872U, "SVSwitch", this, player, 3f))
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

	// Token: 0x060004CA RID: 1226 RVA: 0x00034D14 File Offset: 0x00032F14
	public override void PreInitShared()
	{
		base.PreInitShared();
		this._inputSlotIndex = this.fuelSlots;
		this._outputSlotIndex = this._inputSlotIndex + this.inputSlots;
		this._activeCookingSlot = this._inputSlotIndex;
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x00034D47 File Offset: 0x00032F47
	public override void ServerInit()
	{
		this.inventorySlots = this.fuelSlots + this.inputSlots + this.outputSlots;
		base.ServerInit();
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x00034D69 File Offset: 0x00032F69
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (base.IsOn())
		{
			this.StartCooking();
		}
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x00034D7F File Offset: 0x00032F7F
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.baseOven = Facepunch.Pool.Get<ProtoBuf.BaseOven>();
			info.msg.baseOven.cookSpeed = this.GetSmeltingSpeed();
		}
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x00034DB8 File Offset: 0x00032FB8
	public override void OnInventoryFirstCreated(global::ItemContainer container)
	{
		base.OnInventoryFirstCreated(container);
		if (this.startupContents == null)
		{
			return;
		}
		foreach (ItemAmount itemAmount in this.startupContents)
		{
			ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, 0UL).MoveToContainer(container, -1, true, false, null, true);
		}
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x00034E10 File Offset: 0x00033010
	public override void OnItemAddedOrRemoved(global::Item item, bool bAdded)
	{
		base.OnItemAddedOrRemoved(item, bAdded);
		if (item != null)
		{
			ItemModCookable component = item.info.GetComponent<ItemModCookable>();
			if (component != null)
			{
				item.cookTimeLeft = component.cookTime;
			}
			if (item.HasFlag(global::Item.Flag.OnFire))
			{
				item.SetFlag(global::Item.Flag.OnFire, false);
				item.MarkDirty();
			}
			if (item.HasFlag(global::Item.Flag.Cooking))
			{
				item.SetFlag(global::Item.Flag.Cooking, false);
				item.MarkDirty();
			}
		}
	}

	// Token: 0x060004D0 RID: 1232 RVA: 0x00034E7C File Offset: 0x0003307C
	public override bool ItemFilter(global::Item item, int targetSlot)
	{
		if (!base.ItemFilter(item, targetSlot))
		{
			return false;
		}
		if (targetSlot == -1)
		{
			return false;
		}
		if (this.IsOutputItem(item) && item.GetEntityOwner() != this)
		{
			global::BaseEntity entityOwner = item.GetEntityOwner();
			if (entityOwner != this && entityOwner != null)
			{
				return false;
			}
		}
		global::BaseOven.MinMax? allowedSlots = this.GetAllowedSlots(item);
		return allowedSlots != null && targetSlot >= allowedSlots.Value.Min && targetSlot <= allowedSlots.Value.Max;
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x00034F04 File Offset: 0x00033104
	private global::BaseOven.MinMax? GetAllowedSlots(global::Item item)
	{
		int num = 0;
		int num2;
		if (this.IsBurnableItem(item))
		{
			num2 = this.fuelSlots;
		}
		else if (this.IsOutputItem(item))
		{
			num = this._outputSlotIndex;
			num2 = num + this.outputSlots;
		}
		else
		{
			if (!this.IsMaterialInput(item))
			{
				return null;
			}
			num = this._inputSlotIndex;
			num2 = num + this.inputSlots;
		}
		return new global::BaseOven.MinMax?(new global::BaseOven.MinMax(num, num2 - 1));
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x00034F75 File Offset: 0x00033175
	public global::BaseOven.MinMax GetOutputSlotRange()
	{
		return new global::BaseOven.MinMax(this._outputSlotIndex, this._outputSlotIndex + this.outputSlots - 1);
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x00034F94 File Offset: 0x00033194
	public override int GetIdealSlot(global::BasePlayer player, global::Item item)
	{
		global::BaseOven.MinMax? allowedSlots = this.GetAllowedSlots(item);
		if (allowedSlots == null)
		{
			return -1;
		}
		for (int i = allowedSlots.Value.Min; i <= allowedSlots.Value.Max; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot == null || (slot.CanStack(item) && slot.amount < slot.MaxStackable()))
			{
				return i;
			}
		}
		return base.GetIdealSlot(player, item);
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x00035007 File Offset: 0x00033207
	public void OvenFull()
	{
		this.StopCooking();
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x00003A54 File Offset: 0x00001C54
	private int GetFuelRate()
	{
		return 1;
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x00003A54 File Offset: 0x00001C54
	private int GetCharcoalRate()
	{
		return 1;
	}

	// Token: 0x17000076 RID: 118
	// (get) Token: 0x060004D7 RID: 1239 RVA: 0x00007074 File Offset: 0x00005274
	protected virtual bool CanRunWithNoFuel
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x00035010 File Offset: 0x00033210
	public void Cook()
	{
		global::Item item = this.FindBurnable();
		if (item == null && !this.CanRunWithNoFuel)
		{
			this.StopCooking();
			return;
		}
		foreach (global::Item item2 in base.inventory.itemList)
		{
			if (item2.position >= this._inputSlotIndex && item2.position < this._inputSlotIndex + this.inputSlots && !item2.HasFlag(global::Item.Flag.Cooking))
			{
				item2.SetFlag(global::Item.Flag.Cooking, true);
				item2.MarkDirty();
			}
		}
		this.IncreaseCookTime(0.5f * this.GetSmeltingSpeed());
		global::BaseEntity slot = base.GetSlot(global::BaseEntity.Slot.FireMod);
		if (slot)
		{
			slot.SendMessage("Cook", 0.5f, SendMessageOptions.DontRequireReceiver);
		}
		if (item != null)
		{
			ItemModBurnable component = item.info.GetComponent<ItemModBurnable>();
			item.fuel -= 0.5f * (this.cookingTemperature / 200f);
			if (!item.HasFlag(global::Item.Flag.OnFire))
			{
				item.SetFlag(global::Item.Flag.OnFire, true);
				item.MarkDirty();
			}
			if (item.fuel <= 0f)
			{
				this.ConsumeFuel(item, component);
			}
		}
		this.OnCooked();
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void OnCooked()
	{
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x00035150 File Offset: 0x00033350
	private void ConsumeFuel(global::Item fuel, ItemModBurnable burnable)
	{
		if (this.allowByproductCreation && burnable.byproductItem != null && UnityEngine.Random.Range(0f, 1f) > burnable.byproductChance)
		{
			global::Item item = ItemManager.Create(burnable.byproductItem, burnable.byproductAmount * this.GetCharcoalRate(), 0UL);
			if (!item.MoveToContainer(base.inventory, -1, true, false, null, true))
			{
				this.OvenFull();
				item.Drop(base.inventory.dropPosition, base.inventory.dropVelocity, default(Quaternion));
			}
		}
		if (fuel.amount <= this.GetFuelRate())
		{
			fuel.Remove(0f);
			return;
		}
		fuel.UseItem(this.GetFuelRate());
		fuel.fuel = burnable.fuelAmount;
		fuel.MarkDirty();
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x0003521C File Offset: 0x0003341C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	protected virtual void SVSwitch(global::BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (flag == base.IsOn())
		{
			return;
		}
		if (this.needsBuildingPrivilegeToUse && !msg.player.CanBuild())
		{
			return;
		}
		if (flag)
		{
			this.StartCooking();
			return;
		}
		this.StopCooking();
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x00035265 File Offset: 0x00033465
	public float GetTemperature(int slot)
	{
		if (!base.HasFlag(global::BaseEntity.Flags.On))
		{
			return 15f;
		}
		return this.cookingTemperature;
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x0003527C File Offset: 0x0003347C
	public void UpdateAttachmentTemperature()
	{
		global::BaseEntity slot = base.GetSlot(global::BaseEntity.Slot.FireMod);
		if (slot)
		{
			slot.SendMessage("ParentTemperatureUpdate", base.inventory.temperature, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x000352B8 File Offset: 0x000334B8
	public virtual void StartCooking()
	{
		if (this.FindBurnable() == null && !this.CanRunWithNoFuel)
		{
			return;
		}
		base.inventory.temperature = this.cookingTemperature;
		this.UpdateAttachmentTemperature();
		base.InvokeRepeating(new Action(this.Cook), 0.5f, 0.5f);
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x00035314 File Offset: 0x00033514
	public virtual void StopCooking()
	{
		this.UpdateAttachmentTemperature();
		if (base.inventory != null)
		{
			base.inventory.temperature = 15f;
			foreach (global::Item item in base.inventory.itemList)
			{
				if (item.HasFlag(global::Item.Flag.OnFire))
				{
					item.SetFlag(global::Item.Flag.OnFire, false);
					item.MarkDirty();
				}
				else if (item.HasFlag(global::Item.Flag.Cooking))
				{
					item.SetFlag(global::Item.Flag.Cooking, false);
					item.MarkDirty();
				}
			}
		}
		base.CancelInvoke(new Action(this.Cook));
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x000353D4 File Offset: 0x000335D4
	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return !base.IsDestroyed && base.IsOn() && this.disabledBySplash;
	}

	// Token: 0x060004E1 RID: 1249 RVA: 0x000353EE File Offset: 0x000335EE
	public int DoSplash(ItemDefinition splashType, int amount)
	{
		this.StopCooking();
		return Mathf.Min(200, amount);
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x00035404 File Offset: 0x00033604
	public global::Item FindBurnable()
	{
		if (base.inventory == null)
		{
			return null;
		}
		foreach (global::Item item in base.inventory.itemList)
		{
			if (this.IsBurnableItem(item))
			{
				return item;
			}
		}
		return null;
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x00035470 File Offset: 0x00033670
	private void IncreaseCookTime(float amount)
	{
		List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
		foreach (global::Item item in base.inventory.itemList)
		{
			if (item.HasFlag(global::Item.Flag.Cooking))
			{
				list.Add(item);
			}
		}
		float delta = amount / (float)list.Count;
		foreach (global::Item item2 in list)
		{
			item2.OnCycle(delta);
		}
		Facepunch.Pool.FreeList<global::Item>(ref list);
	}

	// Token: 0x17000077 RID: 119
	// (get) Token: 0x060004E4 RID: 1252 RVA: 0x00035524 File Offset: 0x00033724
	public global::ItemContainer Container
	{
		get
		{
			return base.inventory;
		}
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x0003552C File Offset: 0x0003372C
	public Vector2i InputSlotRange(int slotIndex)
	{
		if (this.IndustrialMode == global::BaseOven.IndustrialSlotMode.LargeFurnace)
		{
			return new Vector2i(0, 7);
		}
		if (this.IndustrialMode == global::BaseOven.IndustrialSlotMode.OilRefinery)
		{
			return new Vector2i(0, 1);
		}
		if (this.IndustrialMode == global::BaseOven.IndustrialSlotMode.ElectricFurnace)
		{
			return new Vector2i(0, 1);
		}
		return new Vector2i(0, 2);
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x00035568 File Offset: 0x00033768
	public Vector2i OutputSlotRange(int slotIndex)
	{
		if (this.IndustrialMode == global::BaseOven.IndustrialSlotMode.LargeFurnace)
		{
			return new Vector2i(7, 16);
		}
		if (this.IndustrialMode == global::BaseOven.IndustrialSlotMode.OilRefinery)
		{
			return new Vector2i(2, 4);
		}
		if (this.IndustrialMode == global::BaseOven.IndustrialSlotMode.ElectricFurnace)
		{
			return new Vector2i(2, 4);
		}
		return new Vector2i(3, 5);
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x000059DD File Offset: 0x00003BDD
	public void OnStorageItemTransferBegin()
	{
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x000059DD File Offset: 0x00003BDD
	public void OnStorageItemTransferEnd()
	{
	}

	// Token: 0x17000078 RID: 120
	// (get) Token: 0x060004E9 RID: 1257 RVA: 0x00002E37 File Offset: 0x00001037
	public global::BaseEntity IndustrialEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x000355A5 File Offset: 0x000337A5
	public float GetSmeltingSpeed()
	{
		if (base.isServer)
		{
			return (float)this.smeltSpeed;
		}
		throw new Exception("No way it should be able to get here?");
	}

	// Token: 0x17000079 RID: 121
	// (get) Token: 0x060004EB RID: 1259 RVA: 0x000355C4 File Offset: 0x000337C4
	private float cookingTemperature
	{
		get
		{
			switch (this.temperature)
			{
			case global::BaseOven.TemperatureType.Warming:
				return 50f;
			case global::BaseOven.TemperatureType.Cooking:
				return 200f;
			case global::BaseOven.TemperatureType.Smelting:
				return 1000f;
			case global::BaseOven.TemperatureType.Fractioning:
				return 1500f;
			default:
				return 15f;
			}
		}
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x0003560F File Offset: 0x0003380F
	private bool IsBurnableItem(global::Item item)
	{
		return item.info.GetComponent<ItemModBurnable>() && (this.fuelType == null || item.info == this.fuelType);
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x00035648 File Offset: 0x00033848
	private bool IsBurnableByproduct(global::Item item)
	{
		ItemDefinition itemDefinition = this.fuelType;
		ItemModBurnable itemModBurnable = (itemDefinition != null) ? itemDefinition.GetComponent<ItemModBurnable>() : null;
		return !(itemModBurnable == null) && item.info == itemModBurnable.byproductItem;
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x00035684 File Offset: 0x00033884
	private bool IsMaterialInput(global::Item item)
	{
		ItemModCookable component = item.info.GetComponent<ItemModCookable>();
		return !(component == null) && (float)component.lowTemp <= this.cookingTemperature && (float)component.highTemp >= this.cookingTemperature;
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x000356C8 File Offset: 0x000338C8
	private bool IsMaterialOutput(global::Item item)
	{
		if (global::BaseOven._materialOutputCache == null)
		{
			this.BuildMaterialOutputCache();
		}
		HashSet<ItemDefinition> hashSet;
		if (!global::BaseOven._materialOutputCache.TryGetValue(this.cookingTemperature, out hashSet))
		{
			Debug.LogError("Can't find smeltable item list for oven");
			return true;
		}
		return hashSet.Contains(item.info);
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x0003570E File Offset: 0x0003390E
	private bool IsOutputItem(global::Item item)
	{
		return this.IsMaterialOutput(item) || this.IsBurnableByproduct(item);
	}

	// Token: 0x060004F1 RID: 1265 RVA: 0x00035724 File Offset: 0x00033924
	private void BuildMaterialOutputCache()
	{
		global::BaseOven._materialOutputCache = new Dictionary<float, HashSet<ItemDefinition>>();
		foreach (float key in (from x in GameManager.server.preProcessed.prefabList.Values
		select x.GetComponent<global::BaseOven>() into x
		where x != null
		select x.cookingTemperature).Distinct<float>().ToArray<float>())
		{
			HashSet<ItemDefinition> hashSet = new HashSet<ItemDefinition>();
			global::BaseOven._materialOutputCache[key] = hashSet;
			foreach (ItemDefinition itemDefinition in ItemManager.itemList)
			{
				ItemModCookable component = itemDefinition.GetComponent<ItemModCookable>();
				if (!(component == null) && component.CanBeCookedByAtTemperature(key))
				{
					hashSet.Add(component.becomeOnCooked);
				}
			}
		}
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x00035854 File Offset: 0x00033A54
	public override bool HasSlot(global::BaseEntity.Slot slot)
	{
		return (this.canModFire && slot == global::BaseEntity.Slot.FireMod) || base.HasSlot(slot);
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x0003586B File Offset: 0x00033A6B
	public override bool CanPickup(global::BasePlayer player)
	{
		return base.CanPickup(player) && this.CanPickupOven();
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x0003587E File Offset: 0x00033A7E
	protected virtual bool CanPickupOven()
	{
		return this.children.Count == 0;
	}

	// Token: 0x0400037F RID: 895
	private static Dictionary<float, HashSet<ItemDefinition>> _materialOutputCache;

	// Token: 0x04000380 RID: 896
	public global::BaseOven.TemperatureType temperature;

	// Token: 0x04000381 RID: 897
	public global::BaseEntity.Menu.Option switchOnMenu;

	// Token: 0x04000382 RID: 898
	public global::BaseEntity.Menu.Option switchOffMenu;

	// Token: 0x04000383 RID: 899
	public ItemAmount[] startupContents;

	// Token: 0x04000384 RID: 900
	public bool allowByproductCreation = true;

	// Token: 0x04000385 RID: 901
	public ItemDefinition fuelType;

	// Token: 0x04000386 RID: 902
	public bool canModFire;

	// Token: 0x04000387 RID: 903
	public bool disabledBySplash = true;

	// Token: 0x04000388 RID: 904
	public int smeltSpeed = 1;

	// Token: 0x04000389 RID: 905
	public int fuelSlots = 1;

	// Token: 0x0400038A RID: 906
	public int inputSlots = 1;

	// Token: 0x0400038B RID: 907
	public int outputSlots = 1;

	// Token: 0x0400038C RID: 908
	public global::BaseOven.IndustrialSlotMode IndustrialMode;

	// Token: 0x0400038D RID: 909
	private int _activeCookingSlot = -1;

	// Token: 0x0400038E RID: 910
	private int _inputSlotIndex;

	// Token: 0x0400038F RID: 911
	private int _outputSlotIndex;

	// Token: 0x04000390 RID: 912
	private const float UpdateRate = 0.5f;

	// Token: 0x02000B57 RID: 2903
	public enum TemperatureType
	{
		// Token: 0x04003DD1 RID: 15825
		Normal,
		// Token: 0x04003DD2 RID: 15826
		Warming,
		// Token: 0x04003DD3 RID: 15827
		Cooking,
		// Token: 0x04003DD4 RID: 15828
		Smelting,
		// Token: 0x04003DD5 RID: 15829
		Fractioning
	}

	// Token: 0x02000B58 RID: 2904
	public enum IndustrialSlotMode
	{
		// Token: 0x04003DD7 RID: 15831
		Furnace,
		// Token: 0x04003DD8 RID: 15832
		LargeFurnace,
		// Token: 0x04003DD9 RID: 15833
		OilRefinery,
		// Token: 0x04003DDA RID: 15834
		ElectricFurnace
	}

	// Token: 0x02000B59 RID: 2905
	public struct MinMax
	{
		// Token: 0x06004A60 RID: 19040 RVA: 0x0018FFE5 File Offset: 0x0018E1E5
		public MinMax(int min, int max)
		{
			this.Min = min;
			this.Max = max;
		}

		// Token: 0x04003DDB RID: 15835
		public int Min;

		// Token: 0x04003DDC RID: 15836
		public int Max;
	}

	// Token: 0x02000B5A RID: 2906
	private enum OvenItemType
	{
		// Token: 0x04003DDE RID: 15838
		Burnable,
		// Token: 0x04003DDF RID: 15839
		Byproduct,
		// Token: 0x04003DE0 RID: 15840
		MaterialInput,
		// Token: 0x04003DE1 RID: 15841
		MaterialOutput
	}
}
