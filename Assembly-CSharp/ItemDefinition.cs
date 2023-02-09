using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x020005A0 RID: 1440
public class ItemDefinition : MonoBehaviour
{
	// Token: 0x17000358 RID: 856
	// (get) Token: 0x06002B41 RID: 11073 RVA: 0x00105A08 File Offset: 0x00103C08
	public IPlayerItemDefinition[] skins2
	{
		get
		{
			if (this._skins2 != null)
			{
				return this._skins2;
			}
			if (PlatformService.Instance.IsValid && PlatformService.Instance.ItemDefinitions != null)
			{
				string prefabname = base.name;
				this._skins2 = (from x in PlatformService.Instance.ItemDefinitions
				where (x.ItemShortName == this.shortname || x.ItemShortName == prefabname) && x.WorkshopId > 0UL
				select x).ToArray<IPlayerItemDefinition>();
			}
			return this._skins2;
		}
	}

	// Token: 0x06002B42 RID: 11074 RVA: 0x00105A81 File Offset: 0x00103C81
	public void InvalidateWorkshopSkinCache()
	{
		this._skins2 = null;
	}

	// Token: 0x06002B43 RID: 11075 RVA: 0x00105A8C File Offset: 0x00103C8C
	public static ulong FindSkin(int itemID, int skinID)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemID);
		if (itemDefinition == null)
		{
			return 0UL;
		}
		IPlayerItemDefinition itemDefinition2 = PlatformService.Instance.GetItemDefinition(skinID);
		if (itemDefinition2 != null)
		{
			ulong workshopDownload = itemDefinition2.WorkshopDownload;
			if (workshopDownload != 0UL)
			{
				string itemShortName = itemDefinition2.ItemShortName;
				if (itemShortName == itemDefinition.shortname || itemShortName == itemDefinition.name)
				{
					return workshopDownload;
				}
			}
		}
		for (int i = 0; i < itemDefinition.skins.Length; i++)
		{
			if (itemDefinition.skins[i].id == skinID)
			{
				return (ulong)((long)skinID);
			}
		}
		return 0UL;
	}

	// Token: 0x17000359 RID: 857
	// (get) Token: 0x06002B44 RID: 11076 RVA: 0x00105B1B File Offset: 0x00103D1B
	public ItemBlueprint Blueprint
	{
		get
		{
			return base.GetComponent<ItemBlueprint>();
		}
	}

	// Token: 0x1700035A RID: 858
	// (get) Token: 0x06002B45 RID: 11077 RVA: 0x00105B23 File Offset: 0x00103D23
	public int craftingStackable
	{
		get
		{
			return Mathf.Max(10, this.stackable);
		}
	}

	// Token: 0x06002B46 RID: 11078 RVA: 0x00105B32 File Offset: 0x00103D32
	public bool HasFlag(ItemDefinition.Flag f)
	{
		return (this.flags & f) == f;
	}

	// Token: 0x06002B47 RID: 11079 RVA: 0x00105B40 File Offset: 0x00103D40
	public void Initialize(List<ItemDefinition> itemList)
	{
		if (this.itemMods != null)
		{
			Debug.LogError("Item Definition Initializing twice: " + base.name);
		}
		this.skins = ItemSkinDirectory.ForItem(this);
		this.itemMods = base.GetComponentsInChildren<ItemMod>(true);
		ItemMod[] array = this.itemMods;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ModInit();
		}
		this.Children = (from x in itemList
		where x.Parent == this
		select x).ToArray<ItemDefinition>();
		this.ItemModWearable = base.GetComponent<ItemModWearable>();
		this.isHoldable = (base.GetComponent<ItemModEntity>() != null);
		this.isUsable = (base.GetComponent<ItemModEntity>() != null || base.GetComponent<ItemModConsume>() != null);
	}

	// Token: 0x1700035B RID: 859
	// (get) Token: 0x06002B48 RID: 11080 RVA: 0x00105BFE File Offset: 0x00103DFE
	public bool isWearable
	{
		get
		{
			return this.ItemModWearable != null;
		}
	}

	// Token: 0x1700035C RID: 860
	// (get) Token: 0x06002B49 RID: 11081 RVA: 0x00105C0C File Offset: 0x00103E0C
	// (set) Token: 0x06002B4A RID: 11082 RVA: 0x00105C14 File Offset: 0x00103E14
	public ItemModWearable ItemModWearable { get; private set; }

	// Token: 0x1700035D RID: 861
	// (get) Token: 0x06002B4B RID: 11083 RVA: 0x00105C1D File Offset: 0x00103E1D
	// (set) Token: 0x06002B4C RID: 11084 RVA: 0x00105C25 File Offset: 0x00103E25
	public bool isHoldable { get; private set; }

	// Token: 0x1700035E RID: 862
	// (get) Token: 0x06002B4D RID: 11085 RVA: 0x00105C2E File Offset: 0x00103E2E
	// (set) Token: 0x06002B4E RID: 11086 RVA: 0x00105C36 File Offset: 0x00103E36
	public bool isUsable { get; private set; }

	// Token: 0x1700035F RID: 863
	// (get) Token: 0x06002B4F RID: 11087 RVA: 0x00105C3F File Offset: 0x00103E3F
	public bool HasSkins
	{
		get
		{
			return (this.skins2 != null && this.skins2.Length != 0) || (this.skins != null && this.skins.Length != 0);
		}
	}

	// Token: 0x17000360 RID: 864
	// (get) Token: 0x06002B50 RID: 11088 RVA: 0x00105C68 File Offset: 0x00103E68
	// (set) Token: 0x06002B51 RID: 11089 RVA: 0x00105C70 File Offset: 0x00103E70
	public bool CraftableWithSkin { get; private set; }

	// Token: 0x040022F5 RID: 8949
	[Header("Item")]
	[ReadOnly]
	public int itemid;

	// Token: 0x040022F6 RID: 8950
	[Tooltip("The shortname should be unique. A hash will be generated from it to identify the item type. If this name changes at any point it will make all saves incompatible")]
	public string shortname;

	// Token: 0x040022F7 RID: 8951
	[Header("Appearance")]
	public Translate.Phrase displayName;

	// Token: 0x040022F8 RID: 8952
	public Translate.Phrase displayDescription;

	// Token: 0x040022F9 RID: 8953
	public Sprite iconSprite;

	// Token: 0x040022FA RID: 8954
	public ItemCategory category;

	// Token: 0x040022FB RID: 8955
	public ItemSelectionPanel selectionPanel;

	// Token: 0x040022FC RID: 8956
	[Header("Containment")]
	public int maxDraggable;

	// Token: 0x040022FD RID: 8957
	public ItemContainer.ContentsType itemType = ItemContainer.ContentsType.Generic;

	// Token: 0x040022FE RID: 8958
	public ItemDefinition.AmountType amountType;

	// Token: 0x040022FF RID: 8959
	[InspectorFlags]
	public ItemSlot occupySlots = ItemSlot.None;

	// Token: 0x04002300 RID: 8960
	public int stackable;

	// Token: 0x04002301 RID: 8961
	public bool quickDespawn;

	// Token: 0x04002302 RID: 8962
	[Header("Spawn Tables")]
	[Tooltip("How rare this item is and how much it costs to research")]
	public Rarity rarity;

	// Token: 0x04002303 RID: 8963
	public Rarity despawnRarity;

	// Token: 0x04002304 RID: 8964
	public bool spawnAsBlueprint;

	// Token: 0x04002305 RID: 8965
	[Header("Sounds")]
	public SoundDefinition inventoryGrabSound;

	// Token: 0x04002306 RID: 8966
	public SoundDefinition inventoryDropSound;

	// Token: 0x04002307 RID: 8967
	public SoundDefinition physImpactSoundDef;

	// Token: 0x04002308 RID: 8968
	public ItemDefinition.Condition condition;

	// Token: 0x04002309 RID: 8969
	[Header("Misc")]
	public bool hidden;

	// Token: 0x0400230A RID: 8970
	[InspectorFlags]
	public ItemDefinition.Flag flags;

	// Token: 0x0400230B RID: 8971
	[Tooltip("User can craft this item on any server if they have this steam item")]
	public SteamInventoryItem steamItem;

	// Token: 0x0400230C RID: 8972
	[Tooltip("User can craft this item if they have this DLC purchased")]
	public SteamDLCItem steamDlc;

	// Token: 0x0400230D RID: 8973
	[Tooltip("Can only craft this item if the parent is craftable (tech tree)")]
	public ItemDefinition Parent;

	// Token: 0x0400230E RID: 8974
	public GameObjectRef worldModelPrefab;

	// Token: 0x0400230F RID: 8975
	public ItemDefinition isRedirectOf;

	// Token: 0x04002310 RID: 8976
	public ItemDefinition.RedirectVendingBehaviour redirectVendingBehaviour;

	// Token: 0x04002311 RID: 8977
	[NonSerialized]
	public ItemMod[] itemMods;

	// Token: 0x04002312 RID: 8978
	public BaseEntity.TraitFlag Traits;

	// Token: 0x04002313 RID: 8979
	[NonSerialized]
	public ItemSkinDirectory.Skin[] skins;

	// Token: 0x04002314 RID: 8980
	[NonSerialized]
	private IPlayerItemDefinition[] _skins2;

	// Token: 0x04002315 RID: 8981
	[Tooltip("Panel to show in the inventory menu when selected")]
	public GameObject panel;

	// Token: 0x0400231A RID: 8986
	[NonSerialized]
	public ItemDefinition[] Children = new ItemDefinition[0];

	// Token: 0x02000D1D RID: 3357
	[Serializable]
	public struct Condition
	{
		// Token: 0x04004515 RID: 17685
		public bool enabled;

		// Token: 0x04004516 RID: 17686
		[Tooltip("The maximum condition this item type can have, new items will start with this value")]
		public float max;

		// Token: 0x04004517 RID: 17687
		[Tooltip("If false then item will destroy when condition reaches 0")]
		public bool repairable;

		// Token: 0x04004518 RID: 17688
		[Tooltip("If true, never lose max condition when repaired")]
		public bool maintainMaxCondition;

		// Token: 0x04004519 RID: 17689
		public bool ovenCondition;

		// Token: 0x0400451A RID: 17690
		public ItemDefinition.Condition.WorldSpawnCondition foundCondition;

		// Token: 0x02000F65 RID: 3941
		[Serializable]
		public class WorldSpawnCondition
		{
			// Token: 0x04004E24 RID: 20004
			public float fractionMin = 1f;

			// Token: 0x04004E25 RID: 20005
			public float fractionMax = 1f;
		}
	}

	// Token: 0x02000D1E RID: 3358
	public enum RedirectVendingBehaviour
	{
		// Token: 0x0400451C RID: 17692
		NoListing,
		// Token: 0x0400451D RID: 17693
		ListAsUniqueItem
	}

	// Token: 0x02000D1F RID: 3359
	[Flags]
	public enum Flag
	{
		// Token: 0x0400451F RID: 17695
		NoDropping = 1,
		// Token: 0x04004520 RID: 17696
		NotStraightToBelt = 2
	}

	// Token: 0x02000D20 RID: 3360
	public enum AmountType
	{
		// Token: 0x04004522 RID: 17698
		Count,
		// Token: 0x04004523 RID: 17699
		Millilitre,
		// Token: 0x04004524 RID: 17700
		Feet,
		// Token: 0x04004525 RID: 17701
		Genetics,
		// Token: 0x04004526 RID: 17702
		OxygenSeconds,
		// Token: 0x04004527 RID: 17703
		Frequency,
		// Token: 0x04004528 RID: 17704
		Generic
	}
}
