using System;
using UnityEngine;

// Token: 0x0200056A RID: 1386
public class TriggerRadiation : TriggerBase
{
	// Token: 0x060029FE RID: 10750 RVA: 0x000FE36A File Offset: 0x000FC56A
	private float GetRadiationSize()
	{
		if (!this.sphereCollider)
		{
			this.sphereCollider = base.GetComponent<SphereCollider>();
		}
		return this.sphereCollider.radius * base.transform.localScale.Max();
	}

	// Token: 0x060029FF RID: 10751 RVA: 0x000FE3A4 File Offset: 0x000FC5A4
	public float GetRadiationAmount()
	{
		if (this.RadiationAmountOverride > 0f)
		{
			return this.RadiationAmountOverride;
		}
		if (this.radiationTier == TriggerRadiation.RadiationTier.MINIMAL)
		{
			return 2f;
		}
		if (this.radiationTier == TriggerRadiation.RadiationTier.LOW)
		{
			return 10f;
		}
		if (this.radiationTier == TriggerRadiation.RadiationTier.MEDIUM)
		{
			return 25f;
		}
		if (this.radiationTier == TriggerRadiation.RadiationTier.HIGH)
		{
			return 51f;
		}
		return 1f;
	}

	// Token: 0x06002A00 RID: 10752 RVA: 0x000FE408 File Offset: 0x000FC608
	public float GetRadiation(Vector3 position, float radProtection)
	{
		float radiationSize = this.GetRadiationSize();
		float radiationAmount = this.GetRadiationAmount();
		float value = Vector3.Distance(base.gameObject.transform.position, position);
		float num = Mathf.InverseLerp(radiationSize, radiationSize * (1f - this.falloff), value);
		return Mathf.Clamp(radiationAmount - radProtection, 0f, radiationAmount) * num;
	}

	// Token: 0x06002A01 RID: 10753 RVA: 0x000FE460 File Offset: 0x000FC660
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
		if (!(baseEntity is BaseCombatEntity))
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002A02 RID: 10754 RVA: 0x000FE4B0 File Offset: 0x000FC6B0
	public void OnDrawGizmosSelected()
	{
		float radiationSize = this.GetRadiationSize();
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, radiationSize);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, radiationSize * (1f - this.falloff));
	}

	// Token: 0x040021F1 RID: 8689
	public TriggerRadiation.RadiationTier radiationTier = TriggerRadiation.RadiationTier.LOW;

	// Token: 0x040021F2 RID: 8690
	public float RadiationAmountOverride;

	// Token: 0x040021F3 RID: 8691
	public float falloff = 0.1f;

	// Token: 0x040021F4 RID: 8692
	private SphereCollider sphereCollider;

	// Token: 0x02000D06 RID: 3334
	public enum RadiationTier
	{
		// Token: 0x040044A9 RID: 17577
		MINIMAL,
		// Token: 0x040044AA RID: 17578
		LOW,
		// Token: 0x040044AB RID: 17579
		MEDIUM,
		// Token: 0x040044AC RID: 17580
		HIGH
	}
}
