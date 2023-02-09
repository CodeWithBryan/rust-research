using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200008B RID: 139
public class LiquidContainer : ContainerIOEntity
{
	// Token: 0x06000CE0 RID: 3296 RVA: 0x0006D134 File Offset: 0x0006B334
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("LiquidContainer.OnRpcMessage", 0))
		{
			if (rpc == 2002733690U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SVDrink ");
				}
				using (TimeWarning.New("SVDrink", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2002733690U, "SVDrink", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpc2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SVDrink(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SVDrink");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000116 RID: 278
	// (get) Token: 0x06000CE1 RID: 3297 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsGravitySource
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000117 RID: 279
	// (get) Token: 0x06000CE2 RID: 3298 RVA: 0x0006D29C File Offset: 0x0006B49C
	protected override bool DisregardGravityRestrictionsOnLiquid
	{
		get
		{
			return base.HasFlag(BaseEntity.Flags.Reserved8) || base.DisregardGravityRestrictionsOnLiquid;
		}
	}

	// Token: 0x06000CE3 RID: 3299 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x0006D2B4 File Offset: 0x0006B4B4
	private bool CanAcceptItem(Item item, int count)
	{
		if (this.ValidItems == null || this.ValidItems.Length == 0)
		{
			return true;
		}
		ItemDefinition[] validItems = this.ValidItems;
		for (int i = 0; i < validItems.Length; i++)
		{
			if (validItems[i] == item.info)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000CE5 RID: 3301 RVA: 0x0006D2FC File Offset: 0x0006B4FC
	public override void ServerInit()
	{
		this.updateDrainAmountAction = new Action(this.UpdateDrainAmount);
		this.pushLiquidAction = new Action(this.PushLiquidThroughOutputs);
		this.deductFuelAction = new Action(this.DeductFuel);
		this.updatePushLiquidTargetsAction = new Action(this.UpdatePushLiquidTargets);
		base.ServerInit();
		if (this.startingAmount > 0)
		{
			base.inventory.AddItem(this.defaultLiquid, this.startingAmount, 0UL, ItemContainer.LimitStack.Existing);
		}
		if (this.autofillOutputs && this.HasLiquidItem())
		{
			this.UpdatePushLiquidTargets();
		}
		ItemContainer inventory = base.inventory;
		inventory.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<Item, int, bool>(this.CanAcceptItem));
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x0006D3B8 File Offset: 0x0006B5B8
	public override void OnCircuitChanged(bool forceUpdate)
	{
		base.OnCircuitChanged(forceUpdate);
		this.ClearDrains();
		base.Invoke(this.updateDrainAmountAction, 0.1f);
		if (this.autofillOutputs && this.HasLiquidItem())
		{
			base.Invoke(this.updatePushLiquidTargetsAction, 0.1f);
		}
	}

	// Token: 0x06000CE7 RID: 3303 RVA: 0x0006D404 File Offset: 0x0006B604
	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		this.UpdateOnFlag();
		base.MarkDirtyForceUpdateOutputs();
		base.Invoke(this.updateDrainAmountAction, 0.1f);
		if (this.connectedList.Count > 0)
		{
			List<IOEntity> list = Facepunch.Pool.GetList<IOEntity>();
			foreach (IOEntity ioentity in this.connectedList)
			{
				if (ioentity != null)
				{
					list.Add(ioentity);
				}
			}
			foreach (IOEntity ioentity2 in list)
			{
				ioentity2.SendChangedToRoot(true);
			}
			Facepunch.Pool.FreeList<IOEntity>(ref list);
		}
		if (this.HasLiquidItem() && this.autofillOutputs)
		{
			base.Invoke(this.updatePushLiquidTargetsAction, 0.1f);
		}
		if (added)
		{
			this.waterTransferStartTime = 10f;
		}
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x0006D510 File Offset: 0x0006B710
	private void ClearDrains()
	{
		foreach (IOEntity ioentity in this.connectedList)
		{
			if (ioentity != null)
			{
				ioentity.SetFuelType(null, null);
			}
		}
		this.connectedList.Clear();
	}

	// Token: 0x06000CE9 RID: 3305 RVA: 0x0006D578 File Offset: 0x0006B778
	public override int GetCurrentEnergy()
	{
		return Mathf.Clamp(this.GetLiquidCount(), 0, this.maxOutputFlow);
	}

	// Token: 0x06000CEA RID: 3306 RVA: 0x0006D58C File Offset: 0x0006B78C
	public override int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		if (!this.HasLiquidItem())
		{
			return base.CalculateCurrentEnergy(inputAmount, inputSlot);
		}
		return this.GetCurrentEnergy();
	}

	// Token: 0x06000CEB RID: 3307 RVA: 0x0006D5A8 File Offset: 0x0006B7A8
	private void UpdateDrainAmount()
	{
		int value = 0;
		Item liquidItem = this.GetLiquidItem();
		if (liquidItem != null)
		{
			foreach (IOEntity.IOSlot ioslot in this.outputs)
			{
				if (ioslot.connectedTo.Get(true) != null)
				{
					this.CalculateDrain(ioslot.connectedTo.Get(true), base.transform.TransformPoint(ioslot.handlePosition), IOEntity.backtracking * 2, ref value, this, (liquidItem != null) ? liquidItem.info : null);
				}
			}
		}
		this.currentDrainAmount = Mathf.Clamp(value, 0, this.maxOutputFlow);
		if (this.currentDrainAmount <= 0 && base.IsInvoking(this.deductFuelAction))
		{
			base.CancelInvoke(this.deductFuelAction);
			return;
		}
		if (this.currentDrainAmount > 0 && !base.IsInvoking(this.deductFuelAction))
		{
			base.InvokeRepeating(this.deductFuelAction, 0f, 1f);
		}
	}

	// Token: 0x17000118 RID: 280
	// (get) Token: 0x06000CEC RID: 3308 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool BlockFluidDraining
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000CED RID: 3309 RVA: 0x0006D690 File Offset: 0x0006B890
	private void CalculateDrain(IOEntity ent, Vector3 fromSlotWorld, int depth, ref int amount, IOEntity lastEntity, ItemDefinition waterType)
	{
		if (ent == this || depth <= 0 || ent == null || lastEntity == null)
		{
			return;
		}
		if (ent is LiquidContainer)
		{
			return;
		}
		if (!ent.BlockFluidDraining && ent.HasFlag(BaseEntity.Flags.On))
		{
			int num = ent.DesiredPower();
			amount += num;
			ent.SetFuelType(waterType, this);
			this.connectedList.Add(ent);
		}
		if (ent.AllowLiquidPassthrough(lastEntity, fromSlotWorld, false))
		{
			foreach (IOEntity.IOSlot ioslot in ent.outputs)
			{
				if (ioslot.connectedTo.Get(true) != null && ioslot.connectedTo.Get(true) != ent)
				{
					this.CalculateDrain(ioslot.connectedTo.Get(true), ent.transform.TransformPoint(ioslot.handlePosition), depth - 1, ref amount, ent, waterType);
				}
			}
		}
	}

	// Token: 0x06000CEE RID: 3310 RVA: 0x0006D775 File Offset: 0x0006B975
	public override void UpdateOutputs()
	{
		base.UpdateOutputs();
		if (UnityEngine.Time.realtimeSinceStartup - this.lastOutputDrainUpdate < 0.2f)
		{
			return;
		}
		this.lastOutputDrainUpdate = UnityEngine.Time.realtimeSinceStartup;
		this.ClearDrains();
		base.Invoke(this.updateDrainAmountAction, 0.1f);
	}

	// Token: 0x06000CEF RID: 3311 RVA: 0x0006D7B4 File Offset: 0x0006B9B4
	private void DeductFuel()
	{
		if (this.HasLiquidItem())
		{
			Item liquidItem = this.GetLiquidItem();
			liquidItem.amount -= this.currentDrainAmount;
			liquidItem.MarkDirty();
			if (liquidItem.amount <= 0)
			{
				liquidItem.Remove(0f);
			}
		}
	}

	// Token: 0x06000CF0 RID: 3312 RVA: 0x0006D7FD File Offset: 0x0006B9FD
	protected void UpdateOnFlag()
	{
		base.SetFlag(BaseEntity.Flags.On, base.inventory.itemList.Count > 0 && base.inventory.itemList[0].amount > 0, false, true);
	}

	// Token: 0x06000CF1 RID: 3313 RVA: 0x0006D837 File Offset: 0x0006BA37
	public virtual void OpenTap(float duration)
	{
		if (base.HasFlag(BaseEntity.Flags.Reserved5))
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved5, true, false, true);
		base.Invoke(new Action(this.ShutTap), duration);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000CF2 RID: 3314 RVA: 0x0006D870 File Offset: 0x0006BA70
	public virtual void ShutTap()
	{
		base.SetFlag(BaseEntity.Flags.Reserved5, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000CF3 RID: 3315 RVA: 0x0006D887 File Offset: 0x0006BA87
	public bool HasLiquidItem()
	{
		return this.GetLiquidItem() != null;
	}

	// Token: 0x06000CF4 RID: 3316 RVA: 0x0006D892 File Offset: 0x0006BA92
	public Item GetLiquidItem()
	{
		if (base.inventory.itemList.Count == 0)
		{
			return null;
		}
		return base.inventory.itemList[0];
	}

	// Token: 0x06000CF5 RID: 3317 RVA: 0x0006D8B9 File Offset: 0x0006BAB9
	public int GetLiquidCount()
	{
		if (!this.HasLiquidItem())
		{
			return 0;
		}
		return this.GetLiquidItem().amount;
	}

	// Token: 0x06000CF6 RID: 3318 RVA: 0x0006D8D0 File Offset: 0x0006BAD0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void SVDrink(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.metabolism.CanConsume())
		{
			return;
		}
		foreach (Item item in base.inventory.itemList)
		{
			ItemModConsume component = item.info.GetComponent<ItemModConsume>();
			if (!(component == null) && component.CanDoAction(item, rpc.player))
			{
				component.DoAction(item, rpc.player);
				break;
			}
		}
	}

	// Token: 0x06000CF7 RID: 3319 RVA: 0x0006D968 File Offset: 0x0006BB68
	private void UpdatePushLiquidTargets()
	{
		this.pushTargets.Clear();
		if (!this.HasLiquidItem())
		{
			return;
		}
		if (base.IsConnectedTo(this, IOEntity.backtracking * 2, false))
		{
			return;
		}
		Item liquidItem = this.GetLiquidItem();
		using (TimeWarning.New("UpdatePushTargets", 0))
		{
			foreach (IOEntity.IOSlot ioslot in this.outputs)
			{
				if (ioslot.type == IOEntity.IOType.Fluidic)
				{
					IOEntity ioentity = ioslot.connectedTo.Get(true);
					if (ioentity != null)
					{
						this.CheckPushLiquid(ioentity, liquidItem, this, IOEntity.backtracking * 4);
					}
				}
			}
		}
		if (this.pushTargets.Count > 0)
		{
			base.InvokeRandomized(this.pushLiquidAction, 0f, this.autofillTickRate, this.autofillTickRate * 0.2f);
		}
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x0006DA4C File Offset: 0x0006BC4C
	private void PushLiquidThroughOutputs()
	{
		if (this.waterTransferStartTime > 0f)
		{
			return;
		}
		if (!this.HasLiquidItem())
		{
			base.CancelInvoke(this.pushLiquidAction);
			return;
		}
		Item liquidItem = this.GetLiquidItem();
		if (this.pushTargets.Count > 0)
		{
			int num = Mathf.Clamp(this.autofillTickAmount, 0, liquidItem.amount) / this.pushTargets.Count;
			if (num == 0 && liquidItem.amount > 0)
			{
				num = liquidItem.amount;
			}
			if (ConVar.Server.waterContainersLeaveWaterBehind && num == liquidItem.amount)
			{
				num--;
			}
			if (num == 0)
			{
				return;
			}
			foreach (ContainerIOEntity containerIOEntity in this.pushTargets)
			{
				if (containerIOEntity.inventory.CanAcceptItem(liquidItem, 0) == ItemContainer.CanAcceptResult.CanAccept && (containerIOEntity.inventory.CanAccept(liquidItem) || containerIOEntity.inventory.FindItemByItemID(liquidItem.info.itemid) != null))
				{
					int num2 = Mathf.Clamp(num, 0, containerIOEntity.inventory.GetMaxTransferAmount(liquidItem.info));
					containerIOEntity.inventory.AddItem(liquidItem.info, num2, 0UL, ItemContainer.LimitStack.Existing);
					liquidItem.amount -= num2;
					liquidItem.MarkDirty();
					if (liquidItem.amount <= 0)
					{
						break;
					}
				}
			}
		}
		if (liquidItem.amount <= 0 || this.pushTargets.Count == 0)
		{
			if (liquidItem.amount <= 0)
			{
				liquidItem.Remove(0f);
			}
			base.CancelInvoke(this.pushLiquidAction);
		}
	}

	// Token: 0x06000CF9 RID: 3321 RVA: 0x0006DBE4 File Offset: 0x0006BDE4
	private void CheckPushLiquid(IOEntity connected, Item ourFuel, IOEntity fromSource, int depth)
	{
		if (depth <= 0 || ourFuel.amount <= 0)
		{
			return;
		}
		Vector3 zero = Vector3.zero;
		IOEntity ioentity = connected.FindGravitySource(ref zero, IOEntity.backtracking * 2, true);
		if (ioentity != null && !connected.AllowLiquidPassthrough(ioentity, zero, false))
		{
			return;
		}
		if (connected == this || this.ConsiderConnectedTo(connected))
		{
			return;
		}
		ContainerIOEntity containerIOEntity;
		if ((containerIOEntity = (connected as ContainerIOEntity)) != null && !this.pushTargets.Contains(containerIOEntity) && containerIOEntity.inventory.CanAcceptItem(ourFuel, 0) == ItemContainer.CanAcceptResult.CanAccept)
		{
			this.pushTargets.Add(containerIOEntity);
			return;
		}
		foreach (IOEntity.IOSlot ioslot in connected.outputs)
		{
			IOEntity ioentity2 = ioslot.connectedTo.Get(true);
			Vector3 sourceWorldPosition = connected.transform.TransformPoint(ioslot.handlePosition);
			if (ioentity2 != null && ioentity2 != fromSource && ioentity2.AllowLiquidPassthrough(connected, sourceWorldPosition, false))
			{
				this.CheckPushLiquid(ioentity2, ourFuel, fromSource, depth - 1);
				if (this.pushTargets.Count >= 3)
				{
					return;
				}
			}
		}
	}

	// Token: 0x06000CFA RID: 3322 RVA: 0x0006DCF4 File Offset: 0x0006BEF4
	public void SetConnectedTo(IOEntity entity)
	{
		this.considerConnectedTo = entity;
	}

	// Token: 0x06000CFB RID: 3323 RVA: 0x0006DCFD File Offset: 0x0006BEFD
	protected override bool ConsiderConnectedTo(IOEntity entity)
	{
		return entity == this.considerConnectedTo;
	}

	// Token: 0x04000829 RID: 2089
	public ItemDefinition defaultLiquid;

	// Token: 0x0400082A RID: 2090
	public int startingAmount;

	// Token: 0x0400082B RID: 2091
	public bool autofillOutputs;

	// Token: 0x0400082C RID: 2092
	public float autofillTickRate = 2f;

	// Token: 0x0400082D RID: 2093
	public int autofillTickAmount = 2;

	// Token: 0x0400082E RID: 2094
	public int maxOutputFlow = 6;

	// Token: 0x0400082F RID: 2095
	public ItemDefinition[] ValidItems;

	// Token: 0x04000830 RID: 2096
	private int currentDrainAmount;

	// Token: 0x04000831 RID: 2097
	private HashSet<IOEntity> connectedList = new HashSet<IOEntity>();

	// Token: 0x04000832 RID: 2098
	private HashSet<ContainerIOEntity> pushTargets = new HashSet<ContainerIOEntity>();

	// Token: 0x04000833 RID: 2099
	private const int maxPushTargets = 3;

	// Token: 0x04000834 RID: 2100
	private IOEntity considerConnectedTo;

	// Token: 0x04000835 RID: 2101
	private Action updateDrainAmountAction;

	// Token: 0x04000836 RID: 2102
	private Action updatePushLiquidTargetsAction;

	// Token: 0x04000837 RID: 2103
	private Action pushLiquidAction;

	// Token: 0x04000838 RID: 2104
	private Action deductFuelAction;

	// Token: 0x04000839 RID: 2105
	private TimeUntil waterTransferStartTime;

	// Token: 0x0400083A RID: 2106
	private float lastOutputDrainUpdate;
}
