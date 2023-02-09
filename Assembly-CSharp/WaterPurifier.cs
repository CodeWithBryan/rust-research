using System;
using Rust;
using UnityEngine;

// Token: 0x020003BF RID: 959
public class WaterPurifier : LiquidContainer
{
	// Token: 0x060020C3 RID: 8387 RVA: 0x00020A80 File Offset: 0x0001EC80
	public bool IsBoiling()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x060020C4 RID: 8388 RVA: 0x000D4CF4 File Offset: 0x000D2EF4
	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.SpawnStorageEnt(false);
		}
	}

	// Token: 0x060020C5 RID: 8389 RVA: 0x000D4D0A File Offset: 0x000D2F0A
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.SpawnStorageEnt(true);
	}

	// Token: 0x060020C6 RID: 8390 RVA: 0x000D4D1C File Offset: 0x000D2F1C
	protected virtual void SpawnStorageEnt(bool load)
	{
		if (load)
		{
			BaseEntity parentEntity = base.GetParentEntity();
			if (parentEntity)
			{
				foreach (BaseEntity baseEntity in parentEntity.children)
				{
					LiquidContainer liquidContainer;
					if (baseEntity != this && (liquidContainer = (baseEntity as LiquidContainer)) != null)
					{
						this.waterStorage = liquidContainer;
						break;
					}
				}
			}
		}
		if (this.waterStorage != null)
		{
			this.waterStorage.SetConnectedTo(this);
			return;
		}
		this.waterStorage = (GameManager.server.CreateEntity(this.storagePrefab.resourcePath, this.storagePrefabAnchor.localPosition, this.storagePrefabAnchor.localRotation, true) as LiquidContainer);
		this.waterStorage.SetParent(base.GetParentEntity(), false, false);
		this.waterStorage.Spawn();
		this.waterStorage.SetConnectedTo(this);
	}

	// Token: 0x060020C7 RID: 8391 RVA: 0x00026D90 File Offset: 0x00024F90
	internal override void OnParentRemoved()
	{
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x060020C8 RID: 8392 RVA: 0x000D4E14 File Offset: 0x000D3014
	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		if (!this.waterStorage.IsDestroyed)
		{
			this.waterStorage.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x060020C9 RID: 8393 RVA: 0x000059DD File Offset: 0x00003BDD
	public void ParentTemperatureUpdate(float temp)
	{
	}

	// Token: 0x060020CA RID: 8394 RVA: 0x000D4E38 File Offset: 0x000D3038
	public void CheckCoolDown()
	{
		if (!base.GetParentEntity() || !base.GetParentEntity().IsOn() || !this.HasDirtyWater())
		{
			base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
			base.CancelInvoke(new Action(this.CheckCoolDown));
		}
	}

	// Token: 0x060020CB RID: 8395 RVA: 0x000D4E88 File Offset: 0x000D3088
	public bool HasDirtyWater()
	{
		Item slot = base.inventory.GetSlot(0);
		return slot != null && slot.info.itemType == ItemContainer.ContentsType.Liquid && slot.amount > 0;
	}

	// Token: 0x060020CC RID: 8396 RVA: 0x000D4EC0 File Offset: 0x000D30C0
	public void Cook(float timeCooked)
	{
		if (this.waterStorage == null)
		{
			return;
		}
		bool flag = this.HasDirtyWater();
		if (!this.IsBoiling() && flag)
		{
			base.InvokeRepeating(new Action(this.CheckCoolDown), 2f, 2f);
			base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		}
		if (!this.IsBoiling())
		{
			return;
		}
		if (flag)
		{
			this.ConvertWater(timeCooked);
		}
	}

	// Token: 0x060020CD RID: 8397 RVA: 0x000D4F30 File Offset: 0x000D3130
	protected void ConvertWater(float timeCooked)
	{
		if (this.stopWhenOutputFull)
		{
			Item slot = this.waterStorage.inventory.GetSlot(0);
			if (slot != null && slot.amount >= slot.MaxStackable())
			{
				return;
			}
		}
		float num = timeCooked * ((float)this.waterToProcessPerMinute / 60f);
		this.dirtyWaterProcssed += num;
		if (this.dirtyWaterProcssed >= 1f)
		{
			Item slot2 = base.inventory.GetSlot(0);
			int num2 = Mathf.Min(Mathf.FloorToInt(this.dirtyWaterProcssed), slot2.amount);
			num = (float)num2;
			slot2.UseItem(num2);
			this.dirtyWaterProcssed -= (float)num2;
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
		this.pendingFreshWater += num / (float)this.freshWaterRatio;
		if (this.pendingFreshWater >= 1f)
		{
			int num3 = Mathf.FloorToInt(this.pendingFreshWater);
			this.pendingFreshWater -= (float)num3;
			Item slot3 = this.waterStorage.inventory.GetSlot(0);
			if (slot3 != null && slot3.info != this.freshWater)
			{
				slot3.RemoveFromContainer();
				slot3.Remove(0f);
			}
			if (slot3 == null)
			{
				Item item = ItemManager.Create(this.freshWater, num3, 0UL);
				if (!item.MoveToContainer(this.waterStorage.inventory, -1, true, false, null, true))
				{
					item.Remove(0f);
				}
			}
			else
			{
				slot3.amount += num3;
				slot3.amount = Mathf.Clamp(slot3.amount, 0, this.waterStorage.maxStackSize);
				this.waterStorage.inventory.MarkDirty();
			}
			this.waterStorage.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060020CE RID: 8398 RVA: 0x000D50E0 File Offset: 0x000D32E0
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk)
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
		}
	}

	// Token: 0x04001969 RID: 6505
	public GameObjectRef storagePrefab;

	// Token: 0x0400196A RID: 6506
	public Transform storagePrefabAnchor;

	// Token: 0x0400196B RID: 6507
	public ItemDefinition freshWater;

	// Token: 0x0400196C RID: 6508
	public int waterToProcessPerMinute = 120;

	// Token: 0x0400196D RID: 6509
	public int freshWaterRatio = 4;

	// Token: 0x0400196E RID: 6510
	public bool stopWhenOutputFull;

	// Token: 0x0400196F RID: 6511
	protected LiquidContainer waterStorage;

	// Token: 0x04001970 RID: 6512
	private float dirtyWaterProcssed;

	// Token: 0x04001971 RID: 6513
	private float pendingFreshWater;

	// Token: 0x02000C71 RID: 3185
	public static class WaterPurifierFlags
	{
		// Token: 0x0400425A RID: 16986
		public const BaseEntity.Flags Boiling = BaseEntity.Flags.Reserved1;
	}
}
