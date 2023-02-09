using System;
using Rust;

// Token: 0x0200049C RID: 1180
public class ItemPickup : DroppedItem
{
	// Token: 0x06002646 RID: 9798 RVA: 0x0007CBEF File Offset: 0x0007ADEF
	public override float GetDespawnDuration()
	{
		return float.PositiveInfinity;
	}

	// Token: 0x06002647 RID: 9799 RVA: 0x000EE9A4 File Offset: 0x000ECBA4
	public override void Spawn()
	{
		base.Spawn();
		if (Application.isLoadingSave)
		{
			return;
		}
		Item item = ItemManager.Create(this.itemDef, this.amount, this.skinOverride);
		base.InitializeItem(item);
		item.SetWorldEntity(this);
	}

	// Token: 0x06002648 RID: 9800 RVA: 0x000EE9E5 File Offset: 0x000ECBE5
	internal override void DoServerDestroy()
	{
		if (this.item != null)
		{
			this.item.Remove(0f);
			this.item = null;
		}
		base.DoServerDestroy();
	}

	// Token: 0x06002649 RID: 9801 RVA: 0x000EEA0C File Offset: 0x000ECC0C
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.IdleDestroy();
	}

	// Token: 0x04001F11 RID: 7953
	public ItemDefinition itemDef;

	// Token: 0x04001F12 RID: 7954
	public int amount = 1;

	// Token: 0x04001F13 RID: 7955
	public ulong skinOverride;
}
