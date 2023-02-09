using System;
using UnityEngine;

// Token: 0x020005A5 RID: 1445
public class ItemMod : MonoBehaviour
{
	// Token: 0x06002B55 RID: 11093 RVA: 0x00105CC0 File Offset: 0x00103EC0
	public virtual void ModInit()
	{
		this.siblingMods = base.GetComponents<ItemMod>();
	}

	// Token: 0x06002B56 RID: 11094 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnItemCreated(Item item)
	{
	}

	// Token: 0x06002B57 RID: 11095 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnVirginItem(Item item)
	{
	}

	// Token: 0x06002B58 RID: 11096 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void ServerCommand(Item item, string command, BasePlayer player)
	{
	}

	// Token: 0x06002B59 RID: 11097 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void DoAction(Item item, BasePlayer player)
	{
	}

	// Token: 0x06002B5A RID: 11098 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnRemove(Item item)
	{
	}

	// Token: 0x06002B5B RID: 11099 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnParentChanged(Item item)
	{
	}

	// Token: 0x06002B5C RID: 11100 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void CollectedForCrafting(Item item, BasePlayer crafter)
	{
	}

	// Token: 0x06002B5D RID: 11101 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void ReturnedFromCancelledCraft(Item item, BasePlayer crafter)
	{
	}

	// Token: 0x06002B5E RID: 11102 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnAttacked(Item item, HitInfo info)
	{
	}

	// Token: 0x06002B5F RID: 11103 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnChanged(Item item)
	{
	}

	// Token: 0x06002B60 RID: 11104 RVA: 0x00105CD0 File Offset: 0x00103ED0
	public virtual bool CanDoAction(Item item, BasePlayer player)
	{
		ItemMod[] array = this.siblingMods;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].Passes(item))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002B61 RID: 11105 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool Passes(Item item)
	{
		return true;
	}

	// Token: 0x06002B62 RID: 11106 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnRemovedFromWorld(Item item)
	{
	}

	// Token: 0x06002B63 RID: 11107 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnMovedToWorld(Item item)
	{
	}

	// Token: 0x0400233C RID: 9020
	protected ItemMod[] siblingMods;
}
