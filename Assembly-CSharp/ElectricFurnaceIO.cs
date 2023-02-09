using System;

// Token: 0x020004A2 RID: 1186
public class ElectricFurnaceIO : IOEntity, IIndustrialStorage
{
	// Token: 0x06002670 RID: 9840 RVA: 0x000EF2A2 File Offset: 0x000ED4A2
	public override int ConsumptionAmount()
	{
		return this.PowerConsumption;
	}

	// Token: 0x06002671 RID: 9841 RVA: 0x000EF2AA File Offset: 0x000ED4AA
	public override int DesiredPower()
	{
		if (base.GetParentEntity() == null)
		{
			return 0;
		}
		if (!base.GetParentEntity().IsOn())
		{
			return 0;
		}
		return this.PowerConsumption;
	}

	// Token: 0x06002672 RID: 9842 RVA: 0x000EF2D4 File Offset: 0x000ED4D4
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			ElectricOven parentOven = this.GetParentOven();
			if (parentOven != null)
			{
				parentOven.OnIOEntityFlagsChanged(old, next);
			}
		}
	}

	// Token: 0x06002673 RID: 9843 RVA: 0x000EF30C File Offset: 0x000ED50C
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateHasPower(inputAmount, inputSlot);
		}
		if (inputSlot == 1 && inputAmount > 0)
		{
			ElectricOven parentOven = this.GetParentOven();
			if (parentOven != null)
			{
				parentOven.StartCooking();
			}
		}
		if (inputSlot == 2 && inputAmount > 0)
		{
			ElectricOven parentOven2 = this.GetParentOven();
			if (parentOven2 != null)
			{
				parentOven2.StopCooking();
			}
		}
	}

	// Token: 0x06002674 RID: 9844 RVA: 0x000EF360 File Offset: 0x000ED560
	private ElectricOven GetParentOven()
	{
		return base.GetParentEntity() as ElectricOven;
	}

	// Token: 0x1700030F RID: 783
	// (get) Token: 0x06002675 RID: 9845 RVA: 0x000EF36D File Offset: 0x000ED56D
	public ItemContainer Container
	{
		get
		{
			return this.GetParentOven().inventory;
		}
	}

	// Token: 0x06002676 RID: 9846 RVA: 0x000EF37A File Offset: 0x000ED57A
	public Vector2i InputSlotRange(int slotIndex)
	{
		return new Vector2i(1, 2);
	}

	// Token: 0x06002677 RID: 9847 RVA: 0x000EF383 File Offset: 0x000ED583
	public Vector2i OutputSlotRange(int slotIndex)
	{
		return new Vector2i(3, 5);
	}

	// Token: 0x06002678 RID: 9848 RVA: 0x000059DD File Offset: 0x00003BDD
	public void OnStorageItemTransferBegin()
	{
	}

	// Token: 0x06002679 RID: 9849 RVA: 0x000059DD File Offset: 0x00003BDD
	public void OnStorageItemTransferEnd()
	{
	}

	// Token: 0x17000310 RID: 784
	// (get) Token: 0x0600267A RID: 9850 RVA: 0x00002E37 File Offset: 0x00001037
	public BaseEntity IndustrialEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x04001F20 RID: 7968
	public int PowerConsumption = 3;
}
