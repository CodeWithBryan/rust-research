using System;

// Token: 0x020005A9 RID: 1449
public class ItemModAnimalEquipment : ItemMod
{
	// Token: 0x04002340 RID: 9024
	public BaseEntity.Flags WearableFlag;

	// Token: 0x04002341 RID: 9025
	public bool hideHair;

	// Token: 0x04002342 RID: 9026
	public ProtectionProperties animalProtection;

	// Token: 0x04002343 RID: 9027
	public ProtectionProperties riderProtection;

	// Token: 0x04002344 RID: 9028
	public int additionalInventorySlots;

	// Token: 0x04002345 RID: 9029
	public float speedModifier;

	// Token: 0x04002346 RID: 9030
	public float staminaUseModifier;

	// Token: 0x04002347 RID: 9031
	public ItemModAnimalEquipment.SlotType slot;

	// Token: 0x02000D22 RID: 3362
	public enum SlotType
	{
		// Token: 0x0400452C RID: 17708
		Basic,
		// Token: 0x0400452D RID: 17709
		Armor,
		// Token: 0x0400452E RID: 17710
		Saddle,
		// Token: 0x0400452F RID: 17711
		Bit,
		// Token: 0x04004530 RID: 17712
		Feet
	}
}
