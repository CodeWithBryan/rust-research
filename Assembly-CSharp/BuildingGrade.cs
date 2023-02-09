using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200071E RID: 1822
[CreateAssetMenu(menuName = "Rust/Building Grade")]
public class BuildingGrade : ScriptableObject
{
	// Token: 0x040028EC RID: 10476
	public BuildingGrade.Enum type;

	// Token: 0x040028ED RID: 10477
	public float baseHealth;

	// Token: 0x040028EE RID: 10478
	public List<ItemAmount> baseCost;

	// Token: 0x040028EF RID: 10479
	public PhysicMaterial physicMaterial;

	// Token: 0x040028F0 RID: 10480
	public ProtectionProperties damageProtecton;

	// Token: 0x040028F1 RID: 10481
	public BaseEntity.Menu.Option upgradeMenu;

	// Token: 0x02000DFA RID: 3578
	public enum Enum
	{
		// Token: 0x04004893 RID: 18579
		None = -1,
		// Token: 0x04004894 RID: 18580
		Twigs,
		// Token: 0x04004895 RID: 18581
		Wood,
		// Token: 0x04004896 RID: 18582
		Stone,
		// Token: 0x04004897 RID: 18583
		Metal,
		// Token: 0x04004898 RID: 18584
		TopTier,
		// Token: 0x04004899 RID: 18585
		Count
	}
}
