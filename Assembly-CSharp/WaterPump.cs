using System;
using UnityEngine;

// Token: 0x020003BE RID: 958
public class WaterPump : LiquidContainer
{
	// Token: 0x060020BC RID: 8380 RVA: 0x000D4BC5 File Offset: 0x000D2DC5
	public override int ConsumptionAmount()
	{
		return this.PowerConsumption;
	}

	// Token: 0x060020BD RID: 8381 RVA: 0x000D4BD0 File Offset: 0x000D2DD0
	private void CreateWater()
	{
		if (this.IsFull())
		{
			return;
		}
		ItemDefinition atPoint = WaterResource.GetAtPoint(this.WaterResourceLocation.position);
		if (atPoint != null)
		{
			base.inventory.AddItem(atPoint, this.AmountPerPump, 0UL, ItemContainer.LimitStack.Existing);
			base.UpdateOnFlag();
		}
	}

	// Token: 0x060020BE RID: 8382 RVA: 0x000D4C1C File Offset: 0x000D2E1C
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		bool flag = next.HasFlag(BaseEntity.Flags.Reserved8);
		if (base.isServer && old.HasFlag(BaseEntity.Flags.Reserved8) != flag)
		{
			if (flag)
			{
				if (!base.IsInvoking(new Action(this.CreateWater)))
				{
					base.InvokeRandomized(new Action(this.CreateWater), this.PumpInterval, this.PumpInterval, this.PumpInterval * 0.1f);
					return;
				}
			}
			else if (base.IsInvoking(new Action(this.CreateWater)))
			{
				base.CancelInvoke(new Action(this.CreateWater));
			}
		}
	}

	// Token: 0x060020BF RID: 8383 RVA: 0x0006D578 File Offset: 0x0006B778
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return Mathf.Clamp(base.GetLiquidCount(), 0, this.maxOutputFlow);
	}

	// Token: 0x060020C0 RID: 8384 RVA: 0x000D4960 File Offset: 0x000D2B60
	private bool IsFull()
	{
		return base.inventory.itemList.Count != 0 && base.inventory.itemList[0].amount >= base.inventory.maxStackSize;
	}

	// Token: 0x1700028D RID: 653
	// (get) Token: 0x060020C1 RID: 8385 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsGravitySource
	{
		get
		{
			return true;
		}
	}

	// Token: 0x04001965 RID: 6501
	public Transform WaterResourceLocation;

	// Token: 0x04001966 RID: 6502
	public float PumpInterval = 20f;

	// Token: 0x04001967 RID: 6503
	public int AmountPerPump = 30;

	// Token: 0x04001968 RID: 6504
	public int PowerConsumption = 5;
}
