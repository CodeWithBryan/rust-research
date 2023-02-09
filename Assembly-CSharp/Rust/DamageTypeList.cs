using System;
using System.Collections.Generic;

namespace Rust
{
	// Token: 0x02000AC2 RID: 2754
	public class DamageTypeList
	{
		// Token: 0x060042AA RID: 17066 RVA: 0x00185084 File Offset: 0x00183284
		public void Set(DamageType index, float amount)
		{
			this.types[(int)index] = amount;
		}

		// Token: 0x060042AB RID: 17067 RVA: 0x0018508F File Offset: 0x0018328F
		public float Get(DamageType index)
		{
			return this.types[(int)index];
		}

		// Token: 0x060042AC RID: 17068 RVA: 0x00185099 File Offset: 0x00183299
		public void Add(DamageType index, float amount)
		{
			this.Set(index, this.Get(index) + amount);
		}

		// Token: 0x060042AD RID: 17069 RVA: 0x001850AB File Offset: 0x001832AB
		public void Scale(DamageType index, float amount)
		{
			this.Set(index, this.Get(index) * amount);
		}

		// Token: 0x060042AE RID: 17070 RVA: 0x001850BD File Offset: 0x001832BD
		public bool Has(DamageType index)
		{
			return this.Get(index) > 0f;
		}

		// Token: 0x060042AF RID: 17071 RVA: 0x001850D0 File Offset: 0x001832D0
		public float Total()
		{
			float num = 0f;
			for (int i = 0; i < this.types.Length; i++)
			{
				float num2 = this.types[i];
				if (!float.IsNaN(num2) && !float.IsInfinity(num2))
				{
					num += num2;
				}
			}
			return num;
		}

		// Token: 0x060042B0 RID: 17072 RVA: 0x00185114 File Offset: 0x00183314
		public void Clear()
		{
			for (int i = 0; i < this.types.Length; i++)
			{
				this.types[i] = 0f;
			}
		}

		// Token: 0x060042B1 RID: 17073 RVA: 0x00185144 File Offset: 0x00183344
		public void Add(List<DamageTypeEntry> entries)
		{
			foreach (DamageTypeEntry damageTypeEntry in entries)
			{
				this.Add(damageTypeEntry.type, damageTypeEntry.amount);
			}
		}

		// Token: 0x060042B2 RID: 17074 RVA: 0x001851A0 File Offset: 0x001833A0
		public void ScaleAll(float amount)
		{
			for (int i = 0; i < this.types.Length; i++)
			{
				this.Scale((DamageType)i, amount);
			}
		}

		// Token: 0x060042B3 RID: 17075 RVA: 0x001851C8 File Offset: 0x001833C8
		public DamageType GetMajorityDamageType()
		{
			int result = 0;
			float num = 0f;
			for (int i = 0; i < this.types.Length; i++)
			{
				float num2 = this.types[i];
				if (!float.IsNaN(num2) && !float.IsInfinity(num2) && num2 >= num)
				{
					result = i;
					num = num2;
				}
			}
			return (DamageType)result;
		}

		// Token: 0x060042B4 RID: 17076 RVA: 0x00185212 File Offset: 0x00183412
		public bool IsMeleeType()
		{
			return this.GetMajorityDamageType().IsMeleeType();
		}

		// Token: 0x060042B5 RID: 17077 RVA: 0x0018521F File Offset: 0x0018341F
		public bool IsBleedCausing()
		{
			return this.GetMajorityDamageType().IsBleedCausing();
		}

		// Token: 0x060042B6 RID: 17078 RVA: 0x0018522C File Offset: 0x0018342C
		public bool IsConsideredAnAttack()
		{
			return this.GetMajorityDamageType().IsConsideredAnAttack();
		}

		// Token: 0x04003AF5 RID: 15093
		public float[] types = new float[25];
	}
}
