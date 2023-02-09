using System;
using UnityEngine;

// Token: 0x020005BE RID: 1470
public class ItemModCycle : ItemMod
{
	// Token: 0x06002BB3 RID: 11187 RVA: 0x00106E54 File Offset: 0x00105054
	public override void OnItemCreated(Item itemcreated)
	{
		float timeTaken = this.timerStart;
		itemcreated.onCycle += delegate(Item item, float delta)
		{
			if (this.onlyAdvanceTimerWhenPass && !this.CanCycle(item))
			{
				return;
			}
			timeTaken += delta;
			if (timeTaken < this.timeBetweenCycles)
			{
				return;
			}
			timeTaken = 0f;
			if (!this.onlyAdvanceTimerWhenPass && !this.CanCycle(item))
			{
				return;
			}
			this.CustomCycle(item, delta);
		};
	}

	// Token: 0x06002BB4 RID: 11188 RVA: 0x00106E8C File Offset: 0x0010508C
	private bool CanCycle(Item item)
	{
		ItemMod[] array = this.actions;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].CanDoAction(item, item.GetOwnerPlayer()))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002BB5 RID: 11189 RVA: 0x00106EC4 File Offset: 0x001050C4
	public void CustomCycle(Item item, float delta)
	{
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		ItemMod[] array = this.actions;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].DoAction(item, ownerPlayer);
		}
	}

	// Token: 0x06002BB6 RID: 11190 RVA: 0x00106EF7 File Offset: 0x001050F7
	private void OnValidate()
	{
		if (this.actions == null)
		{
			Debug.LogWarning("ItemModMenuOption: actions is null", base.gameObject);
		}
	}

	// Token: 0x0400237B RID: 9083
	public ItemMod[] actions;

	// Token: 0x0400237C RID: 9084
	public float timeBetweenCycles = 1f;

	// Token: 0x0400237D RID: 9085
	public float timerStart;

	// Token: 0x0400237E RID: 9086
	public bool onlyAdvanceTimerWhenPass;
}
