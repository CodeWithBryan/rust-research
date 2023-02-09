using System;
using UnityEngine;

// Token: 0x0200013B RID: 315
public class ArcadeEntity : BaseMonoBehaviour
{
	// Token: 0x04000EA9 RID: 3753
	public uint id;

	// Token: 0x04000EAA RID: 3754
	public uint spriteID;

	// Token: 0x04000EAB RID: 3755
	public uint soundID;

	// Token: 0x04000EAC RID: 3756
	public bool visible;

	// Token: 0x04000EAD RID: 3757
	public Vector3 heading = new Vector3(0f, 1f, 0f);

	// Token: 0x04000EAE RID: 3758
	public bool isEnabled;

	// Token: 0x04000EAF RID: 3759
	public bool dirty;

	// Token: 0x04000EB0 RID: 3760
	public float alpha = 1f;

	// Token: 0x04000EB1 RID: 3761
	public BoxCollider boxCollider;

	// Token: 0x04000EB2 RID: 3762
	public bool host;

	// Token: 0x04000EB3 RID: 3763
	public bool localAuthorativeOverride;

	// Token: 0x04000EB4 RID: 3764
	public ArcadeEntity arcadeEntityParent;

	// Token: 0x04000EB5 RID: 3765
	public uint prefabID;

	// Token: 0x04000EB6 RID: 3766
	[Header("Health")]
	public bool takesDamage;

	// Token: 0x04000EB7 RID: 3767
	public float health = 1f;

	// Token: 0x04000EB8 RID: 3768
	public float maxHealth = 1f;

	// Token: 0x04000EB9 RID: 3769
	[NonSerialized]
	public bool mapLoadedEntiy;
}
