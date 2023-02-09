using System;
using System.Collections.Generic;
using Facepunch;
using Rust.UI;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000842 RID: 2114
public class ItemStore : SingletonComponent<ItemStore>, VirtualScroll.IDataSource
{
	// Token: 0x060034A8 RID: 13480 RVA: 0x0013E5E3 File Offset: 0x0013C7E3
	public int GetItemCount()
	{
		return this.Cart.Count;
	}

	// Token: 0x060034A9 RID: 13481 RVA: 0x0013E5F0 File Offset: 0x0013C7F0
	public void SetItemData(int i, GameObject obj)
	{
		obj.GetComponent<ItemStoreCartItem>().Init(i, this.Cart[i]);
	}

	// Token: 0x04002F05 RID: 12037
	public static readonly Translate.Phrase CartEmptyPhrase = new Translate.Phrase("store.cart.empty", "Cart");

	// Token: 0x04002F06 RID: 12038
	public static readonly Translate.Phrase CartSingularPhrase = new Translate.Phrase("store.cart.singular", "1 item");

	// Token: 0x04002F07 RID: 12039
	public static readonly Translate.Phrase CartPluralPhrase = new Translate.Phrase("store.cart.plural", "{amount} items");

	// Token: 0x04002F08 RID: 12040
	public GameObject ItemPrefab;

	// Token: 0x04002F09 RID: 12041
	[FormerlySerializedAs("ItemParent")]
	public RectTransform LimitedItemParent;

	// Token: 0x04002F0A RID: 12042
	public RectTransform GeneralItemParent;

	// Token: 0x04002F0B RID: 12043
	public List<IPlayerItemDefinition> Cart = new List<IPlayerItemDefinition>();

	// Token: 0x04002F0C RID: 12044
	public ItemStoreItemInfoModal ItemStoreInfoModal;

	// Token: 0x04002F0D RID: 12045
	public GameObject BuyingModal;

	// Token: 0x04002F0E RID: 12046
	public ItemStoreBuyFailedModal ItemStoreBuyFailedModal;

	// Token: 0x04002F0F RID: 12047
	public ItemStoreBuySuccessModal ItemStoreBuySuccessModal;

	// Token: 0x04002F10 RID: 12048
	public SoundDefinition AddToCartSound;

	// Token: 0x04002F11 RID: 12049
	public RustText CartButtonLabel;

	// Token: 0x04002F12 RID: 12050
	public RustText QuantityValue;

	// Token: 0x04002F13 RID: 12051
	public RustText TotalValue;
}
