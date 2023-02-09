using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x020002E7 RID: 743
public class DirectionalDamageTrigger : TriggerBase
{
	// Token: 0x06001D4B RID: 7499 RVA: 0x000C8A68 File Offset: 0x000C6C68
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
		if (!(baseEntity is BaseCombatEntity))
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06001D4C RID: 7500 RVA: 0x000C8AB5 File Offset: 0x000C6CB5
	internal override void OnObjects()
	{
		base.InvokeRepeating(new Action(this.OnTick), this.repeatRate, this.repeatRate);
	}

	// Token: 0x06001D4D RID: 7501 RVA: 0x000C8AD5 File Offset: 0x000C6CD5
	internal override void OnEmpty()
	{
		base.CancelInvoke(new Action(this.OnTick));
	}

	// Token: 0x06001D4E RID: 7502 RVA: 0x000C8AEC File Offset: 0x000C6CEC
	private void OnTick()
	{
		if (this.attackEffect.isValid)
		{
			Effect.server.Run(this.attackEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
		if (this.entityContents == null)
		{
			return;
		}
		foreach (BaseEntity baseEntity in this.entityContents.ToArray<BaseEntity>())
		{
			if (baseEntity.IsValid())
			{
				BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
				if (!(baseCombatEntity == null))
				{
					HitInfo hitInfo = new HitInfo();
					hitInfo.damageTypes.Add(this.damageType);
					hitInfo.DoHitEffects = true;
					hitInfo.DidHit = true;
					hitInfo.PointStart = base.transform.position;
					hitInfo.PointEnd = baseCombatEntity.transform.position;
					baseCombatEntity.Hurt(hitInfo);
				}
			}
		}
	}

	// Token: 0x040016B0 RID: 5808
	public float repeatRate = 1f;

	// Token: 0x040016B1 RID: 5809
	public List<DamageTypeEntry> damageType;

	// Token: 0x040016B2 RID: 5810
	public GameObjectRef attackEffect;
}
