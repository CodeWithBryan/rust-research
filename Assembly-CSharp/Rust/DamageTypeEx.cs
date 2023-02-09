using System;

namespace Rust
{
	// Token: 0x02000AC4 RID: 2756
	public static class DamageTypeEx
	{
		// Token: 0x060042B9 RID: 17081 RVA: 0x0018524E File Offset: 0x0018344E
		public static bool IsMeleeType(this DamageType damageType)
		{
			return damageType == DamageType.Blunt || damageType == DamageType.Slash || damageType == DamageType.Stab;
		}

		// Token: 0x060042BA RID: 17082 RVA: 0x00185261 File Offset: 0x00183461
		public static bool IsBleedCausing(this DamageType damageType)
		{
			return damageType == DamageType.Bite || damageType == DamageType.Slash || damageType == DamageType.Stab || damageType == DamageType.Bullet || damageType == DamageType.Arrow;
		}

		// Token: 0x060042BB RID: 17083 RVA: 0x0018527E File Offset: 0x0018347E
		public static bool IsConsideredAnAttack(this DamageType damageType)
		{
			return damageType != DamageType.Decay && damageType != DamageType.Collision;
		}
	}
}
