using System;
using UnityEngine;

// Token: 0x020005BD RID: 1469
public class ItemModCookable : ItemMod
{
	// Token: 0x06002BAE RID: 11182 RVA: 0x00106C28 File Offset: 0x00104E28
	public void OnValidate()
	{
		if (this.amountOfBecome < 1)
		{
			this.amountOfBecome = 1;
		}
		if (this.becomeOnCooked == null)
		{
			Debug.LogWarning("[ItemModCookable] becomeOnCooked is unset! [" + base.name + "]", base.gameObject);
		}
	}

	// Token: 0x06002BAF RID: 11183 RVA: 0x00106C68 File Offset: 0x00104E68
	public bool CanBeCookedByAtTemperature(float temperature)
	{
		return temperature > (float)this.lowTemp && temperature < (float)this.highTemp;
	}

	// Token: 0x06002BB0 RID: 11184 RVA: 0x00106C80 File Offset: 0x00104E80
	private void CycleCooking(Item item, float delta)
	{
		if (!this.CanBeCookedByAtTemperature(item.temperature) || item.cookTimeLeft < 0f)
		{
			if (this.setCookingFlag && item.HasFlag(Item.Flag.Cooking))
			{
				item.SetFlag(Item.Flag.Cooking, false);
				item.MarkDirty();
			}
			return;
		}
		if (this.setCookingFlag && !item.HasFlag(Item.Flag.Cooking))
		{
			item.SetFlag(Item.Flag.Cooking, true);
			item.MarkDirty();
		}
		item.cookTimeLeft -= delta;
		if (item.cookTimeLeft > 0f)
		{
			item.MarkDirty();
			return;
		}
		float num = item.cookTimeLeft * -1f;
		int num2 = 1 + Mathf.FloorToInt(num / this.cookTime);
		item.cookTimeLeft = this.cookTime - num % this.cookTime;
		BaseOven baseOven = item.GetEntityOwner() as BaseOven;
		num2 = Mathf.Min(num2, item.amount);
		if (item.amount > num2)
		{
			item.amount -= num2;
			item.MarkDirty();
		}
		else
		{
			item.Remove(0f);
		}
		if (this.becomeOnCooked != null)
		{
			Item item2 = ItemManager.Create(this.becomeOnCooked, this.amountOfBecome * num2, 0UL);
			if (item2 != null && !item2.MoveToContainer(item.parent, -1, true, false, null, true) && !item2.MoveToContainer(item.parent, -1, true, false, null, true))
			{
				item2.Drop(item.parent.dropPosition, item.parent.dropVelocity, default(Quaternion));
				if (item.parent.entityOwner && baseOven != null)
				{
					baseOven.OvenFull();
				}
			}
		}
	}

	// Token: 0x06002BB1 RID: 11185 RVA: 0x00106E1A File Offset: 0x0010501A
	public override void OnItemCreated(Item itemcreated)
	{
		itemcreated.cookTimeLeft = this.cookTime;
		itemcreated.onCycle += this.CycleCooking;
	}

	// Token: 0x04002375 RID: 9077
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition becomeOnCooked;

	// Token: 0x04002376 RID: 9078
	public float cookTime = 30f;

	// Token: 0x04002377 RID: 9079
	public int amountOfBecome = 1;

	// Token: 0x04002378 RID: 9080
	public int lowTemp;

	// Token: 0x04002379 RID: 9081
	public int highTemp;

	// Token: 0x0400237A RID: 9082
	public bool setCookingFlag;
}
