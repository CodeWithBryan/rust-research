using System;

// Token: 0x020001A0 RID: 416
public class NPCPlayerCorpse : PlayerCorpse
{
	// Token: 0x060017B2 RID: 6066 RVA: 0x000B0734 File Offset: 0x000AE934
	public override float GetRemovalTime()
	{
		return 600f;
	}

	// Token: 0x060017B3 RID: 6067 RVA: 0x000B073B File Offset: 0x000AE93B
	public override bool CanLoot()
	{
		return this.lootEnabled;
	}

	// Token: 0x060017B4 RID: 6068 RVA: 0x000B0743 File Offset: 0x000AE943
	public void SetLootableIn(float when)
	{
		base.Invoke(new Action(this.EnableLooting), when);
	}

	// Token: 0x060017B5 RID: 6069 RVA: 0x000B0758 File Offset: 0x000AE958
	public void EnableLooting()
	{
		this.lootEnabled = true;
	}

	// Token: 0x060017B6 RID: 6070 RVA: 0x000B0761 File Offset: 0x000AE961
	protected override bool CanLootContainer(ItemContainer c, int index)
	{
		return index != 1 && index != 2 && base.CanLootContainer(c, index);
	}

	// Token: 0x060017B7 RID: 6071 RVA: 0x000B0775 File Offset: 0x000AE975
	protected override void PreDropItems()
	{
		base.PreDropItems();
		if (this.containers != null && this.containers.Length >= 2)
		{
			this.containers[1].Clear();
			ItemManager.DoRemoves();
		}
	}

	// Token: 0x040010B5 RID: 4277
	private bool lootEnabled;
}
