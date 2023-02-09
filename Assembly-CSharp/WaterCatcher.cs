using System;
using UnityEngine;

// Token: 0x020003BD RID: 957
public class WaterCatcher : LiquidContainer
{
	// Token: 0x060020B5 RID: 8373 RVA: 0x000D48B2 File Offset: 0x000D2AB2
	public override void ServerInit()
	{
		base.ServerInit();
		this.AddResource(1);
		base.InvokeRandomized(new Action(this.CollectWater), 60f, 60f, 6f);
	}

	// Token: 0x060020B6 RID: 8374 RVA: 0x000D48E4 File Offset: 0x000D2AE4
	private void CollectWater()
	{
		if (this.IsFull())
		{
			return;
		}
		float num = 0.25f;
		num += Climate.GetFog(base.transform.position) * 2f;
		if (this.TestIsOutside())
		{
			num += Climate.GetRain(base.transform.position);
			num += Climate.GetSnow(base.transform.position) * 0.5f;
		}
		this.AddResource(Mathf.CeilToInt(this.maxItemToCreate * num));
	}

	// Token: 0x060020B7 RID: 8375 RVA: 0x000D4960 File Offset: 0x000D2B60
	private bool IsFull()
	{
		return base.inventory.itemList.Count != 0 && base.inventory.itemList[0].amount >= base.inventory.maxStackSize;
	}

	// Token: 0x060020B8 RID: 8376 RVA: 0x000D499C File Offset: 0x000D2B9C
	private bool TestIsOutside()
	{
		return !Physics.SphereCast(new Ray(base.transform.localToWorldMatrix.MultiplyPoint3x4(this.rainTestPosition), Vector3.up), this.rainTestSize, 256f, 161546513);
	}

	// Token: 0x060020B9 RID: 8377 RVA: 0x000D49E4 File Offset: 0x000D2BE4
	private void AddResource(int iAmount)
	{
		if (this.outputs.Length != 0)
		{
			IOEntity ioentity = this.CheckPushLiquid(this.outputs[0].connectedTo.Get(true), iAmount, this, IOEntity.backtracking * 2);
			LiquidContainer liquidContainer;
			if (ioentity != null && (liquidContainer = (ioentity as LiquidContainer)) != null)
			{
				liquidContainer.inventory.AddItem(this.itemToCreate, iAmount, 0UL, ItemContainer.LimitStack.Existing);
				return;
			}
		}
		base.inventory.AddItem(this.itemToCreate, iAmount, 0UL, ItemContainer.LimitStack.Existing);
		base.UpdateOnFlag();
	}

	// Token: 0x060020BA RID: 8378 RVA: 0x000D4A64 File Offset: 0x000D2C64
	private IOEntity CheckPushLiquid(IOEntity connected, int amount, IOEntity fromSource, int depth)
	{
		if (depth <= 0 || this.itemToCreate == null)
		{
			return null;
		}
		if (connected == null)
		{
			return null;
		}
		Vector3 zero = Vector3.zero;
		IOEntity ioentity = connected.FindGravitySource(ref zero, IOEntity.backtracking, true);
		if (ioentity != null && !connected.AllowLiquidPassthrough(ioentity, zero, false))
		{
			return null;
		}
		if (connected == this || this.ConsiderConnectedTo(connected))
		{
			return null;
		}
		if (connected.prefabID == 2150367216U)
		{
			return null;
		}
		foreach (IOEntity.IOSlot ioslot in connected.outputs)
		{
			IOEntity ioentity2 = ioslot.connectedTo.Get(true);
			Vector3 sourceWorldPosition = connected.transform.TransformPoint(ioslot.handlePosition);
			if (ioentity2 != null && ioentity2 != fromSource && ioentity2.AllowLiquidPassthrough(connected, sourceWorldPosition, false))
			{
				IOEntity ioentity3 = this.CheckPushLiquid(ioentity2, amount, fromSource, depth - 1);
				if (ioentity3 != null)
				{
					return ioentity3;
				}
			}
		}
		LiquidContainer liquidContainer;
		if ((liquidContainer = (connected as LiquidContainer)) != null && liquidContainer.inventory.GetAmount(this.itemToCreate.itemid, false) + amount > liquidContainer.maxStackSize)
		{
			return connected;
		}
		return null;
	}

	// Token: 0x04001960 RID: 6496
	[Header("Water Catcher")]
	public ItemDefinition itemToCreate;

	// Token: 0x04001961 RID: 6497
	public float maxItemToCreate = 10f;

	// Token: 0x04001962 RID: 6498
	[Header("Outside Test")]
	public Vector3 rainTestPosition = new Vector3(0f, 1f, 0f);

	// Token: 0x04001963 RID: 6499
	public float rainTestSize = 1f;

	// Token: 0x04001964 RID: 6500
	private const float collectInterval = 60f;
}
