using System;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x0200055B RID: 1371
public class TriggerHurt : TriggerBase, IServerComponent, IHurtTrigger
{
	// Token: 0x060029B3 RID: 10675 RVA: 0x000FCFC8 File Offset: 0x000FB1C8
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x060029B4 RID: 10676 RVA: 0x000FD00B File Offset: 0x000FB20B
	internal override void OnObjects()
	{
		base.InvokeRepeating(new Action(this.OnTick), 0f, 1f / this.DamageTickRate);
	}

	// Token: 0x060029B5 RID: 10677 RVA: 0x000FD030 File Offset: 0x000FB230
	internal override void OnEmpty()
	{
		base.CancelInvoke(new Action(this.OnTick));
	}

	// Token: 0x060029B6 RID: 10678 RVA: 0x000FD044 File Offset: 0x000FB244
	private void OnTick()
	{
		BaseEntity attacker = base.gameObject.ToBaseEntity();
		if (this.entityContents == null)
		{
			return;
		}
		foreach (BaseEntity baseEntity in this.entityContents.ToArray<BaseEntity>())
		{
			if (baseEntity.IsValid())
			{
				BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
				if (!(baseCombatEntity == null) && this.CanHurt(baseCombatEntity))
				{
					baseCombatEntity.Hurt(this.DamagePerSecond * (1f / this.DamageTickRate), this.damageType, attacker, true);
				}
			}
		}
	}

	// Token: 0x060029B7 RID: 10679 RVA: 0x00003A54 File Offset: 0x00001C54
	protected virtual bool CanHurt(BaseCombatEntity ent)
	{
		return true;
	}

	// Token: 0x040021BC RID: 8636
	public float DamagePerSecond = 1f;

	// Token: 0x040021BD RID: 8637
	public float DamageTickRate = 4f;

	// Token: 0x040021BE RID: 8638
	public DamageType damageType;
}
