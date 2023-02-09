using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003B8 RID: 952
public class PoweredWaterPurifier : WaterPurifier
{
	// Token: 0x06002092 RID: 8338 RVA: 0x000228A0 File Offset: 0x00020AA0
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x06002093 RID: 8339 RVA: 0x000D4334 File Offset: 0x000D2534
	public override bool CanPickup(BasePlayer player)
	{
		if (base.isClient)
		{
			return base.CanPickup(player);
		}
		return base.CanPickup(player) && !base.HasDirtyWater() && this.waterStorage != null && (this.waterStorage.inventory == null || this.waterStorage.inventory.itemList.Count == 0);
	}

	// Token: 0x06002094 RID: 8340 RVA: 0x000D439C File Offset: 0x000D259C
	protected override void SpawnStorageEnt(bool load)
	{
		if (load)
		{
			using (List<BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					LiquidContainer waterStorage;
					if ((waterStorage = (enumerator.Current as LiquidContainer)) != null)
					{
						this.waterStorage = waterStorage;
					}
				}
			}
		}
		if (this.waterStorage != null)
		{
			this.waterStorage.SetConnectedTo(this);
			return;
		}
		this.waterStorage = (GameManager.server.CreateEntity(this.storagePrefab.resourcePath, this.storagePrefabAnchor.position, this.storagePrefabAnchor.rotation, true) as LiquidContainer);
		this.waterStorage.SetParent(this, true, false);
		this.waterStorage.Spawn();
		this.waterStorage.SetConnectedTo(this);
	}

	// Token: 0x06002095 RID: 8341 RVA: 0x000D4474 File Offset: 0x000D2674
	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		if (base.HasLiquidItem())
		{
			if (base.HasFlag(BaseEntity.Flags.Reserved8) && !base.IsInvoking(new Action(this.ConvertWater)))
			{
				base.InvokeRandomized(new Action(this.ConvertWater), this.ConvertInterval, this.ConvertInterval, this.ConvertInterval * 0.1f);
				return;
			}
		}
		else if (base.IsInvoking(new Action(this.ConvertWater)))
		{
			base.CancelInvoke(new Action(this.ConvertWater));
		}
	}

	// Token: 0x06002096 RID: 8342 RVA: 0x000D4503 File Offset: 0x000D2703
	private void ConvertWater()
	{
		if (!base.HasDirtyWater())
		{
			return;
		}
		base.ConvertWater(this.ConvertInterval);
	}

	// Token: 0x06002097 RID: 8343 RVA: 0x000D451A File Offset: 0x000D271A
	public override int ConsumptionAmount()
	{
		return this.PowerDrain;
	}

	// Token: 0x06002098 RID: 8344 RVA: 0x000D4524 File Offset: 0x000D2724
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			if (old.HasFlag(BaseEntity.Flags.Reserved8) != next.HasFlag(BaseEntity.Flags.Reserved8))
			{
				if (next.HasFlag(BaseEntity.Flags.Reserved8))
				{
					if (!base.IsInvoking(new Action(this.ConvertWater)))
					{
						base.InvokeRandomized(new Action(this.ConvertWater), this.ConvertInterval, this.ConvertInterval, this.ConvertInterval * 0.1f);
					}
				}
				else if (base.IsInvoking(new Action(this.ConvertWater)))
				{
					base.CancelInvoke(new Action(this.ConvertWater));
				}
			}
			if (this.waterStorage != null)
			{
				this.waterStorage.SetFlag(BaseEntity.Flags.Reserved8, base.HasFlag(BaseEntity.Flags.Reserved8), false, true);
			}
		}
	}

	// Token: 0x04001954 RID: 6484
	public float ConvertInterval = 5f;

	// Token: 0x04001955 RID: 6485
	public int PowerDrain = 5;

	// Token: 0x04001956 RID: 6486
	public Material PoweredMaterial;

	// Token: 0x04001957 RID: 6487
	public Material UnpoweredMaterial;

	// Token: 0x04001958 RID: 6488
	public MeshRenderer TargetRenderer;
}
