using System;
using UnityEngine;

// Token: 0x020005C8 RID: 1480
public class ItemModPetStats : ItemMod
{
	// Token: 0x06002BD5 RID: 11221 RVA: 0x001074EC File Offset: 0x001056EC
	public void Apply(BasePet pet)
	{
		if (pet == null)
		{
			return;
		}
		pet.SetMaxHealth(pet.MaxHealth() + this.MaxHealthModifier);
		if (pet.Brain != null && pet.Brain.Navigator != null)
		{
			pet.Brain.Navigator.Speed += this.SpeedModifier;
		}
		pet.BaseAttackRate += this.AttackRateModifier;
		pet.BaseAttackDamge += this.AttackDamageModifier;
	}

	// Token: 0x040023A0 RID: 9120
	[Tooltip("Speed modifier. Value, not percentage.")]
	public float SpeedModifier;

	// Token: 0x040023A1 RID: 9121
	[Tooltip("HP amount to modify max health by. Value, not percentage.")]
	public float MaxHealthModifier;

	// Token: 0x040023A2 RID: 9122
	[Tooltip("Damage amount to modify base attack damage by. Value, not percentage.")]
	public float AttackDamageModifier;

	// Token: 0x040023A3 RID: 9123
	[Tooltip("Attack rate (seconds) to modify base attack rate by. Value, not percentage.")]
	public float AttackRateModifier;
}
