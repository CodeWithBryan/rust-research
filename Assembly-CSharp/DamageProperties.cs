using System;
using UnityEngine;

// Token: 0x02000530 RID: 1328
[CreateAssetMenu(menuName = "Rust/Damage Properties")]
public class DamageProperties : ScriptableObject
{
	// Token: 0x060028B2 RID: 10418 RVA: 0x000F7B00 File Offset: 0x000F5D00
	public float GetMultiplier(HitArea area)
	{
		for (int i = 0; i < this.bones.Length; i++)
		{
			DamageProperties.HitAreaProperty hitAreaProperty = this.bones[i];
			if (hitAreaProperty.area == area)
			{
				return hitAreaProperty.damage;
			}
		}
		if (!this.fallback)
		{
			return 1f;
		}
		return this.fallback.GetMultiplier(area);
	}

	// Token: 0x060028B3 RID: 10419 RVA: 0x000F7B58 File Offset: 0x000F5D58
	public void ScaleDamage(HitInfo info)
	{
		HitArea boneArea = info.boneArea;
		if (boneArea == (HitArea)(-1) || boneArea == (HitArea)0)
		{
			return;
		}
		info.damageTypes.ScaleAll(this.GetMultiplier(boneArea));
	}

	// Token: 0x04002110 RID: 8464
	public DamageProperties fallback;

	// Token: 0x04002111 RID: 8465
	[Horizontal(1, 0)]
	public DamageProperties.HitAreaProperty[] bones;

	// Token: 0x02000CEE RID: 3310
	[Serializable]
	public class HitAreaProperty
	{
		// Token: 0x04004454 RID: 17492
		public HitArea area = HitArea.Head;

		// Token: 0x04004455 RID: 17493
		public float damage = 1f;
	}
}
