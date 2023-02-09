using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000080 RID: 128
public class IndustrialCrafter : IndustrialEntity, IItemContainerEntity, IIdealSlotEntity, LootPanel.IHasLootPanel, IContainerSounds, IIndustrialStorage
{
	// Token: 0x06000C0C RID: 3084 RVA: 0x000682D0 File Offset: 0x000664D0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("IndustrialCrafter.OnRpcMessage", 0))
		{
			if (rpc == 331989034U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenLoot ");
				}
				using (TimeWarning.New("RPC_OpenLoot", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(331989034U, "RPC_OpenLoot", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenLoot(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_OpenLoot");
					}
				}
				return true;
			}
			if (rpc == 4167839872U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SvSwitch ");
				}
				using (TimeWarning.New("SvSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(4167839872U, "SvSwitch", this, player, 2UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(4167839872U, "SvSwitch", this, player, 3f))
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
							this.SvSwitch(msg2);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in SvSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000105 RID: 261
	// (get) Token: 0x06000C0D RID: 3085 RVA: 0x000685EC File Offset: 0x000667EC
	// (set) Token: 0x06000C0E RID: 3086 RVA: 0x000685F4 File Offset: 0x000667F4
	public TimeUntilWithDuration jobFinishes { get; private set; }

	// Token: 0x06000C0F RID: 3087 RVA: 0x00068600 File Offset: 0x00066800
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		bool flag = next.HasFlag(global::BaseEntity.Flags.On);
		if (old.HasFlag(global::BaseEntity.Flags.On) != flag && base.isServer)
		{
			float industrialCrafterFrequency = ConVar.Server.industrialCrafterFrequency;
			if (flag && industrialCrafterFrequency > 0f)
			{
				base.InvokeRandomized(new Action(this.CheckCraft), industrialCrafterFrequency, industrialCrafterFrequency, industrialCrafterFrequency * 0.5f);
				return;
			}
			base.CancelInvoke(new Action(this.CheckCraft));
		}
	}

	// Token: 0x17000106 RID: 262
	// (get) Token: 0x06000C10 RID: 3088 RVA: 0x00068682 File Offset: 0x00066882
	// (set) Token: 0x06000C11 RID: 3089 RVA: 0x0006868A File Offset: 0x0006688A
	public global::ItemContainer inventory { get; set; }

	// Token: 0x17000107 RID: 263
	// (get) Token: 0x06000C12 RID: 3090 RVA: 0x00059891 File Offset: 0x00057A91
	public Transform Transform
	{
		get
		{
			return base.transform;
		}
	}

	// Token: 0x17000108 RID: 264
	// (get) Token: 0x06000C13 RID: 3091 RVA: 0x00003A54 File Offset: 0x00001C54
	public bool DropsLoot
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000109 RID: 265
	// (get) Token: 0x06000C14 RID: 3092 RVA: 0x00068693 File Offset: 0x00066893
	public bool DropFloats { get; }

	// Token: 0x06000C15 RID: 3093 RVA: 0x000059DD File Offset: 0x00003BDD
	public void DropItems(global::BaseEntity initiator = null)
	{
	}

	// Token: 0x06000C16 RID: 3094 RVA: 0x00007074 File Offset: 0x00005274
	public bool ShouldDropItemsIndividually()
	{
		return false;
	}

	// Token: 0x06000C17 RID: 3095 RVA: 0x000059DD File Offset: 0x00003BDD
	public void DropBonusItems(global::BaseEntity initiator, global::ItemContainer container)
	{
	}

	// Token: 0x06000C18 RID: 3096 RVA: 0x0006869C File Offset: 0x0006689C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(global::BaseEntity.RPCMessage rpc)
	{
		if (this.inventory == null)
		{
			return;
		}
		global::BasePlayer player = rpc.player;
		if (!player || !player.CanInteract())
		{
			return;
		}
		this.PlayerOpenLoot(player, "", true);
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x000686D8 File Offset: 0x000668D8
	public virtual bool PlayerOpenLoot(global::BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		if (this.NeedsBuildingPrivilegeToUse && !player.CanBuild())
		{
			return false;
		}
		if (this.OnlyOneUser && base.IsOpen())
		{
			player.ChatMessage("Already in use");
			return false;
		}
		if (player.inventory.loot.StartLootingEntity(this, doPositionChecks))
		{
			base.SetFlag(global::BaseEntity.Flags.Open, true, false, true);
			player.inventory.loot.AddContainer(this.inventory);
			player.inventory.loot.SendImmediate();
			player.ClientRPCPlayer<string>(null, player, "RPC_OpenLootPanel", this.LootPanelName);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			return true;
		}
		return false;
	}

	// Token: 0x06000C1A RID: 3098 RVA: 0x00059B66 File Offset: 0x00057D66
	public virtual void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000C1B RID: 3099 RVA: 0x00068775 File Offset: 0x00066975
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.inventory == null)
		{
			this.CreateInventory(true);
		}
	}

	// Token: 0x06000C1C RID: 3100 RVA: 0x0006878C File Offset: 0x0006698C
	public void CreateInventory(bool giveUID)
	{
		this.inventory = new global::ItemContainer();
		this.inventory.entityOwner = this;
		this.inventory.canAcceptItem = new Func<global::Item, int, bool>(this.CanAcceptItem);
		this.inventory.ServerInitialize(null, 11);
		if (giveUID)
		{
			this.inventory.GiveUID();
		}
	}

	// Token: 0x06000C1D RID: 3101 RVA: 0x000687E3 File Offset: 0x000669E3
	private bool CanAcceptItem(global::Item item, int index)
	{
		return index != 0 || item.IsBlueprint();
	}

	// Token: 0x06000C1E RID: 3102 RVA: 0x00067376 File Offset: 0x00065576
	private void CheckCraft()
	{
		global::IndustrialEntity.Queue.Add(this);
	}

	// Token: 0x06000C1F RID: 3103 RVA: 0x000687F4 File Offset: 0x000669F4
	private global::Item GetTargetBlueprint()
	{
		if (this.inventory == null)
		{
			return null;
		}
		global::Item slot = this.inventory.GetSlot(0);
		if (slot == null || !slot.IsBlueprint())
		{
			return null;
		}
		return slot;
	}

	// Token: 0x06000C20 RID: 3104 RVA: 0x00068828 File Offset: 0x00066A28
	protected override void RunJob()
	{
		base.RunJob();
		if (ConVar.Server.industrialCrafterFrequency <= 0f)
		{
			return;
		}
		if (base.HasFlag(global::BaseEntity.Flags.Reserved1) || this.currentlyCrafting != null)
		{
			return;
		}
		global::Item targetBlueprint = this.GetTargetBlueprint();
		if (targetBlueprint == null)
		{
			return;
		}
		if (this.GetWorkbench() == null || this.GetWorkbench().Workbenchlevel < targetBlueprint.blueprintTargetDef.Blueprint.workbenchLevelRequired)
		{
			return;
		}
		ItemBlueprint blueprint = targetBlueprint.blueprintTargetDef.Blueprint;
		bool flag = true;
		foreach (ItemAmount itemAmount in blueprint.ingredients)
		{
			if ((float)this.GetInputAmount(itemAmount.itemDef) < itemAmount.amount)
			{
				flag = false;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		flag = false;
		for (int i = 5; i <= 8; i++)
		{
			global::Item slot = this.inventory.GetSlot(i);
			if (slot == null || (slot.info == targetBlueprint.blueprintTargetDef && slot.amount + blueprint.amountToCreate <= slot.MaxStackable()))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		foreach (ItemAmount am in blueprint.ingredients)
		{
			this.ConsumeInputIngredient(am);
		}
		this.currentlyCrafting = targetBlueprint.blueprintTargetDef;
		this.currentlyCraftingAmount = blueprint.amountToCreate;
		float time = blueprint.time;
		base.Invoke(new Action(this.CompleteCraft), time);
		this.jobFinishes = time;
		base.SetFlag(global::BaseEntity.Flags.Reserved1, true, false, true);
		base.ClientRPC<float, float>(null, "ClientUpdateCraftTimeRemaining", this.jobFinishes, this.jobFinishes.Duration);
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x00068A18 File Offset: 0x00066C18
	private void CompleteCraft()
	{
		bool flag = false;
		for (int i = 5; i <= 8; i++)
		{
			global::Item slot = this.inventory.GetSlot(i);
			if (slot == null)
			{
				global::Item item = ItemManager.Create(this.currentlyCrafting, this.currentlyCraftingAmount, 0UL);
				item.position = i;
				this.inventory.Insert(item);
				flag = true;
				break;
			}
			if (slot.info == this.currentlyCrafting && slot.amount + this.currentlyCraftingAmount <= slot.MaxStackable())
			{
				slot.amount += this.currentlyCraftingAmount;
				slot.MarkDirty();
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			ItemManager.Create(this.currentlyCrafting, this.currentlyCraftingAmount, 0UL).Drop(base.transform.position + base.transform.forward * 0.5f, Vector3.zero, default(Quaternion));
		}
		this.currentlyCrafting = null;
		this.currentlyCraftingAmount = 0;
		base.SetFlag(global::BaseEntity.Flags.Reserved1, false, false, true);
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x00068B28 File Offset: 0x00066D28
	private int GetInputAmount(ItemDefinition def)
	{
		if (def == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 1; i <= 4; i++)
		{
			global::Item slot = this.inventory.GetSlot(i);
			if (slot != null && def == slot.info)
			{
				num += slot.amount;
			}
		}
		return num;
	}

	// Token: 0x06000C23 RID: 3107 RVA: 0x00068B78 File Offset: 0x00066D78
	private bool ConsumeInputIngredient(ItemAmount am)
	{
		if (am.itemDef == null)
		{
			return false;
		}
		float num = am.amount;
		for (int i = 1; i <= 4; i++)
		{
			global::Item slot = this.inventory.GetSlot(i);
			if (slot != null && am.itemDef == slot.info)
			{
				float num2 = Mathf.Min(num, (float)slot.amount);
				slot.UseItem((int)num2);
				num -= num2;
				if (num2 <= 0f)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000C24 RID: 3108 RVA: 0x00068BF0 File Offset: 0x00066DF0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			if (this.currentlyCrafting != null)
			{
				info.msg.industrialCrafter = Facepunch.Pool.Get<ProtoBuf.IndustrialCrafter>();
				info.msg.industrialCrafter.currentlyCrafting = this.currentlyCrafting.itemid;
				info.msg.industrialCrafter.currentlyCraftingAmount = this.currentlyCraftingAmount;
			}
			if (this.inventory != null)
			{
				info.msg.storageBox = Facepunch.Pool.Get<StorageBox>();
				info.msg.storageBox.contents = this.inventory.Save();
			}
		}
	}

	// Token: 0x06000C25 RID: 3109 RVA: 0x00068C94 File Offset: 0x00066E94
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.storageBox != null && this.inventory != null)
		{
			this.inventory.Load(info.msg.storageBox.contents);
			this.inventory.capacity = 11;
		}
		if (base.isServer && info.fromDisk && info.msg.industrialCrafter != null)
		{
			this.currentlyCrafting = ItemManager.FindItemDefinition(info.msg.industrialCrafter.currentlyCrafting);
			this.currentlyCraftingAmount = info.msg.industrialCrafter.currentlyCraftingAmount;
			this.CompleteCraft();
		}
	}

	// Token: 0x1700010A RID: 266
	// (get) Token: 0x06000C26 RID: 3110 RVA: 0x00068D39 File Offset: 0x00066F39
	public global::ItemContainer Container
	{
		get
		{
			return this.inventory;
		}
	}

	// Token: 0x06000C27 RID: 3111 RVA: 0x00068D41 File Offset: 0x00066F41
	public Vector2i InputSlotRange(int slotIndex)
	{
		if (slotIndex == 3)
		{
			return new Vector2i(0, 0);
		}
		return new Vector2i(1, 4);
	}

	// Token: 0x06000C28 RID: 3112 RVA: 0x00068D56 File Offset: 0x00066F56
	public Vector2i OutputSlotRange(int slotIndex)
	{
		if (slotIndex == 1)
		{
			return Vector2i.zero;
		}
		return new Vector2i(5, 8);
	}

	// Token: 0x06000C29 RID: 3113 RVA: 0x000059DD File Offset: 0x00003BDD
	public void OnStorageItemTransferBegin()
	{
	}

	// Token: 0x06000C2A RID: 3114 RVA: 0x000059DD File Offset: 0x00003BDD
	public void OnStorageItemTransferEnd()
	{
	}

	// Token: 0x1700010B RID: 267
	// (get) Token: 0x06000C2B RID: 3115 RVA: 0x00002E37 File Offset: 0x00001037
	public global::BaseEntity IndustrialEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000C2C RID: 3116 RVA: 0x000059DD File Offset: 0x00003BDD
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x00068D6C File Offset: 0x00066F6C
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		if (inputSlot == 1)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved8, inputAmount >= this.ConsumptionAmount() && inputAmount > 0, false, true);
			this.currentEnergy = inputAmount;
			this.ensureOutputsUpdated = true;
			this.MarkDirty();
		}
		if (inputSlot == 1 && inputAmount <= 0 && base.IsOn())
		{
			this.SetSwitch(false);
		}
		if (inputSlot == 2)
		{
			if (base.IsOn() && inputAmount == 0)
			{
				this.SetSwitch(false);
			}
			else if (!base.IsOn() && inputAmount > 0 && base.HasFlag(global::BaseEntity.Flags.Reserved8))
			{
				this.SetSwitch(true);
			}
		}
		if (inputSlot == 4 && inputAmount > 0 && base.HasFlag(global::BaseEntity.Flags.Reserved8))
		{
			this.SetSwitch(true);
		}
		if (inputSlot == 5 && inputAmount > 0 && base.HasFlag(global::BaseEntity.Flags.Reserved8))
		{
			this.SetSwitch(false);
		}
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x00068E40 File Offset: 0x00067040
	public virtual void SetSwitch(bool wantsOn)
	{
		if (wantsOn == base.IsOn())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, wantsOn, false, true);
		base.SetFlag(global::BaseEntity.Flags.Busy, true, false, true);
		base.Invoke(new Action(this.Unbusy), 0.5f);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x0005E510 File Offset: 0x0005C710
	public void Unbusy()
	{
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x06000C30 RID: 3120 RVA: 0x00068E93 File Offset: 0x00067093
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(2UL)]
	private void SvSwitch(global::BaseEntity.RPCMessage msg)
	{
		this.SetSwitch(!base.IsOn());
	}

	// Token: 0x06000C31 RID: 3121 RVA: 0x00068EA4 File Offset: 0x000670A4
	public override bool CanPickup(global::BasePlayer player)
	{
		if (base.isServer)
		{
			return this.inventory != null && this.inventory.IsEmpty() && base.CanPickup(player);
		}
		return base.CanPickup(player);
	}

	// Token: 0x06000C32 RID: 3122 RVA: 0x00040DA9 File Offset: 0x0003EFA9
	public int GetIdealSlot(global::BasePlayer player, global::Item item)
	{
		return -1;
	}

	// Token: 0x06000C33 RID: 3123 RVA: 0x00007074 File Offset: 0x00005274
	public uint GetIdealContainer(global::BasePlayer player, global::Item item)
	{
		return 0U;
	}

	// Token: 0x1700010C RID: 268
	// (get) Token: 0x06000C34 RID: 3124 RVA: 0x00068ED4 File Offset: 0x000670D4
	public Translate.Phrase LootPanelTitle
	{
		get
		{
			return new Translate.Phrase("industrial.crafter.loot", "Industrial Crafter");
		}
	}

	// Token: 0x1700010D RID: 269
	// (get) Token: 0x06000C35 RID: 3125 RVA: 0x00068EE5 File Offset: 0x000670E5
	public SoundDefinition OpenSound
	{
		get
		{
			return this.ContainerOpenSound;
		}
	}

	// Token: 0x1700010E RID: 270
	// (get) Token: 0x06000C36 RID: 3126 RVA: 0x00068EED File Offset: 0x000670ED
	public SoundDefinition CloseSound
	{
		get
		{
			return this.ContainerCloseSound;
		}
	}

	// Token: 0x06000C37 RID: 3127 RVA: 0x00068EF5 File Offset: 0x000670F5
	public Workbench GetWorkbench()
	{
		return base.GetParentEntity() as Workbench;
	}

	// Token: 0x040007B8 RID: 1976
	public string LootPanelName = "generic";

	// Token: 0x040007B9 RID: 1977
	public bool NeedsBuildingPrivilegeToUse;

	// Token: 0x040007BA RID: 1978
	public bool OnlyOneUser;

	// Token: 0x040007BB RID: 1979
	public SoundDefinition ContainerOpenSound;

	// Token: 0x040007BC RID: 1980
	public SoundDefinition ContainerCloseSound;

	// Token: 0x040007BD RID: 1981
	public AnimationCurve MaterialOffsetCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040007BE RID: 1982
	public const global::BaseEntity.Flags Crafting = global::BaseEntity.Flags.Reserved1;

	// Token: 0x040007BF RID: 1983
	public Renderer[] MeshRenderers;

	// Token: 0x040007C0 RID: 1984
	public ParticleSystemContainer JobCompleteFx;

	// Token: 0x040007C1 RID: 1985
	public SoundDefinition JobCompleteSoundDef;

	// Token: 0x040007C5 RID: 1989
	private ItemDefinition currentlyCrafting;

	// Token: 0x040007C6 RID: 1990
	private int currentlyCraftingAmount;

	// Token: 0x040007C7 RID: 1991
	private const int StorageSize = 11;

	// Token: 0x040007C8 RID: 1992
	private const int BlueprintSlot = 0;

	// Token: 0x040007C9 RID: 1993
	private const int InputSlotStart = 1;

	// Token: 0x040007CA RID: 1994
	private const int InputSlotEnd = 4;

	// Token: 0x040007CB RID: 1995
	private const int OutputSlotStart = 5;

	// Token: 0x040007CC RID: 1996
	private const int OutputSlotEnd = 8;
}
