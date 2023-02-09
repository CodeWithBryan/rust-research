using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x0200072C RID: 1836
[CreateAssetMenu(menuName = "Rust/Protection Properties")]
public class ProtectionProperties : ScriptableObject
{
	// Token: 0x060032DB RID: 13019 RVA: 0x0013A5DC File Offset: 0x001387DC
	public void OnValidate()
	{
		if (this.amounts.Length < 25)
		{
			float[] array = new float[25];
			for (int i = 0; i < array.Length; i++)
			{
				if (i >= this.amounts.Length)
				{
					if (i == 21)
					{
						array[i] = this.amounts[9];
					}
				}
				else
				{
					array[i] = this.amounts[i];
				}
			}
			this.amounts = array;
		}
	}

	// Token: 0x060032DC RID: 13020 RVA: 0x0013A63C File Offset: 0x0013883C
	public void Clear()
	{
		for (int i = 0; i < this.amounts.Length; i++)
		{
			this.amounts[i] = 0f;
		}
	}

	// Token: 0x060032DD RID: 13021 RVA: 0x0013A66C File Offset: 0x0013886C
	public void Add(float amount)
	{
		for (int i = 0; i < this.amounts.Length; i++)
		{
			this.amounts[i] += amount;
		}
	}

	// Token: 0x060032DE RID: 13022 RVA: 0x0013A69D File Offset: 0x0013889D
	public void Add(DamageType index, float amount)
	{
		this.amounts[(int)index] += amount;
	}

	// Token: 0x060032DF RID: 13023 RVA: 0x0013A6B0 File Offset: 0x001388B0
	public void Add(ProtectionProperties other, float scale)
	{
		for (int i = 0; i < Mathf.Min(other.amounts.Length, this.amounts.Length); i++)
		{
			this.amounts[i] += other.amounts[i] * scale;
		}
	}

	// Token: 0x060032E0 RID: 13024 RVA: 0x0013A6F8 File Offset: 0x001388F8
	public void Add(List<Item> items, HitArea area = (HitArea)(-1))
	{
		for (int i = 0; i < items.Count; i++)
		{
			Item item = items[i];
			ItemModWearable component = item.info.GetComponent<ItemModWearable>();
			if (!(component == null) && component.ProtectsArea(area))
			{
				component.CollectProtection(item, this);
			}
		}
	}

	// Token: 0x060032E1 RID: 13025 RVA: 0x0013A744 File Offset: 0x00138944
	public void Multiply(float multiplier)
	{
		for (int i = 0; i < this.amounts.Length; i++)
		{
			this.amounts[i] *= multiplier;
		}
	}

	// Token: 0x060032E2 RID: 13026 RVA: 0x0013A775 File Offset: 0x00138975
	public void Multiply(DamageType index, float multiplier)
	{
		this.amounts[(int)index] *= multiplier;
	}

	// Token: 0x060032E3 RID: 13027 RVA: 0x0013A788 File Offset: 0x00138988
	public void Scale(DamageTypeList damageList, float ProtectionAmount = 1f)
	{
		for (int i = 0; i < this.amounts.Length; i++)
		{
			if (this.amounts[i] != 0f)
			{
				damageList.Scale((DamageType)i, 1f - Mathf.Clamp(this.amounts[i] * ProtectionAmount, -1f, 1f));
			}
		}
	}

	// Token: 0x060032E4 RID: 13028 RVA: 0x0013A7DD File Offset: 0x001389DD
	public float Get(DamageType damageType)
	{
		return this.amounts[(int)damageType];
	}

	// Token: 0x04002937 RID: 10551
	[TextArea]
	public string comments;

	// Token: 0x04002938 RID: 10552
	[Range(0f, 100f)]
	public float density = 1f;

	// Token: 0x04002939 RID: 10553
	[ArrayIndexIsEnumRanged(enumType = typeof(DamageType), min = -4f, max = 3f)]
	public float[] amounts = new float[25];
}
