using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000727 RID: 1831
public class ItemSkinDirectory : ScriptableObject
{
	// Token: 0x170003F2 RID: 1010
	// (get) Token: 0x060032C9 RID: 13001 RVA: 0x00139D60 File Offset: 0x00137F60
	public static ItemSkinDirectory Instance
	{
		get
		{
			if (ItemSkinDirectory._Instance == null)
			{
				ItemSkinDirectory._Instance = FileSystem.Load<ItemSkinDirectory>("assets/skins.asset", true);
				if (ItemSkinDirectory._Instance == null)
				{
					throw new Exception("Couldn't load assets/skins.asset");
				}
				if (ItemSkinDirectory._Instance.skins == null || ItemSkinDirectory._Instance.skins.Length == 0)
				{
					throw new Exception("Loaded assets/skins.asset but something is wrong");
				}
			}
			return ItemSkinDirectory._Instance;
		}
	}

	// Token: 0x060032CA RID: 13002 RVA: 0x00139DCC File Offset: 0x00137FCC
	public static ItemSkinDirectory.Skin[] ForItem(ItemDefinition item)
	{
		return (from x in ItemSkinDirectory.Instance.skins
		where x.isSkin && x.itemid == item.itemid
		select x).ToArray<ItemSkinDirectory.Skin>();
	}

	// Token: 0x060032CB RID: 13003 RVA: 0x00139E08 File Offset: 0x00138008
	public static ItemSkinDirectory.Skin FindByInventoryDefinitionId(int id)
	{
		return (from x in ItemSkinDirectory.Instance.skins
		where x.id == id
		select x).FirstOrDefault<ItemSkinDirectory.Skin>();
	}

	// Token: 0x0400291B RID: 10523
	private static ItemSkinDirectory _Instance;

	// Token: 0x0400291C RID: 10524
	public ItemSkinDirectory.Skin[] skins;

	// Token: 0x02000E07 RID: 3591
	[Serializable]
	public struct Skin
	{
		// Token: 0x17000676 RID: 1654
		// (get) Token: 0x06004FC8 RID: 20424 RVA: 0x001A038B File Offset: 0x0019E58B
		public SteamInventoryItem invItem
		{
			get
			{
				if (this._invItem == null && !string.IsNullOrEmpty(this.name))
				{
					this._invItem = FileSystem.Load<SteamInventoryItem>(this.name, true);
				}
				return this._invItem;
			}
		}

		// Token: 0x040048C9 RID: 18633
		public int id;

		// Token: 0x040048CA RID: 18634
		public int itemid;

		// Token: 0x040048CB RID: 18635
		public string name;

		// Token: 0x040048CC RID: 18636
		public bool isSkin;

		// Token: 0x040048CD RID: 18637
		private SteamInventoryItem _invItem;
	}
}
