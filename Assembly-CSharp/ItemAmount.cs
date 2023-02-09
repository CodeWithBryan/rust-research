using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020005DA RID: 1498
[Serializable]
public class ItemAmount : ISerializationCallbackReceiver
{
	// Token: 0x06002C0B RID: 11275 RVA: 0x0010867B File Offset: 0x0010687B
	public ItemAmount(ItemDefinition item = null, float amt = 0f)
	{
		this.itemDef = item;
		this.amount = amt;
		this.startAmount = this.amount;
	}

	// Token: 0x1700036D RID: 877
	// (get) Token: 0x06002C0C RID: 11276 RVA: 0x0010869D File Offset: 0x0010689D
	public int itemid
	{
		get
		{
			if (this.itemDef == null)
			{
				return 0;
			}
			return this.itemDef.itemid;
		}
	}

	// Token: 0x06002C0D RID: 11277 RVA: 0x001086BA File Offset: 0x001068BA
	public virtual float GetAmount()
	{
		return this.amount;
	}

	// Token: 0x06002C0E RID: 11278 RVA: 0x001086C2 File Offset: 0x001068C2
	public virtual void OnAfterDeserialize()
	{
		this.startAmount = this.amount;
	}

	// Token: 0x06002C0F RID: 11279 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnBeforeSerialize()
	{
	}

	// Token: 0x06002C10 RID: 11280 RVA: 0x001086D0 File Offset: 0x001068D0
	public static ItemAmountList SerialiseList(List<ItemAmount> list)
	{
		ItemAmountList itemAmountList = Pool.Get<ItemAmountList>();
		itemAmountList.amount = Pool.GetList<float>();
		itemAmountList.itemID = Pool.GetList<int>();
		foreach (ItemAmount itemAmount in list)
		{
			itemAmountList.amount.Add(itemAmount.amount);
			itemAmountList.itemID.Add(itemAmount.itemid);
		}
		return itemAmountList;
	}

	// Token: 0x06002C11 RID: 11281 RVA: 0x00108758 File Offset: 0x00106958
	public static void DeserialiseList(List<ItemAmount> target, ItemAmountList source)
	{
		target.Clear();
		if (source.amount.Count != source.itemID.Count)
		{
			return;
		}
		for (int i = 0; i < source.amount.Count; i++)
		{
			target.Add(new ItemAmount(ItemManager.FindItemDefinition(source.itemID[i]), source.amount[i]));
		}
	}

	// Token: 0x040023EF RID: 9199
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemDef;

	// Token: 0x040023F0 RID: 9200
	public float amount;

	// Token: 0x040023F1 RID: 9201
	[NonSerialized]
	public float startAmount;
}
