using System;
using Rust;
using UnityEngine;

// Token: 0x02000476 RID: 1142
public class VehicleLiftOccupantTrigger : TriggerBase
{
	// Token: 0x170002DF RID: 735
	// (get) Token: 0x06002548 RID: 9544 RVA: 0x000E98E0 File Offset: 0x000E7AE0
	// (set) Token: 0x06002549 RID: 9545 RVA: 0x000E98E8 File Offset: 0x000E7AE8
	public ModularCar carOccupant { get; private set; }

	// Token: 0x0600254A RID: 9546 RVA: 0x000E98F1 File Offset: 0x000E7AF1
	protected override void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		base.OnDisable();
		if (this.carOccupant != null)
		{
			this.carOccupant = null;
		}
	}

	// Token: 0x0600254B RID: 9547 RVA: 0x000E9918 File Offset: 0x000E7B18
	internal override GameObject InterestedInObject(GameObject obj)
	{
		if (base.InterestedInObject(obj) == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null || baseEntity.isClient)
		{
			return null;
		}
		if (!(baseEntity is ModularCar))
		{
			return null;
		}
		return obj;
	}

	// Token: 0x0600254C RID: 9548 RVA: 0x000E995B File Offset: 0x000E7B5B
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (this.carOccupant == null && ent.isServer)
		{
			this.carOccupant = (ModularCar)ent;
		}
	}

	// Token: 0x0600254D RID: 9549 RVA: 0x000E9988 File Offset: 0x000E7B88
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		if (this.carOccupant == ent)
		{
			this.carOccupant = null;
			if (this.entityContents != null && this.entityContents.Count > 0)
			{
				foreach (BaseEntity baseEntity in this.entityContents)
				{
					if (baseEntity != null)
					{
						this.carOccupant = (ModularCar)baseEntity;
						break;
					}
				}
			}
		}
	}
}
