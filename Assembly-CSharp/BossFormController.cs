using System;
using UnityEngine;

// Token: 0x0200013F RID: 319
public class BossFormController : ArcadeEntityController
{
	// Token: 0x04000ECE RID: 3790
	public float animationSpeed = 0.5f;

	// Token: 0x04000ECF RID: 3791
	public Sprite[] animationFrames;

	// Token: 0x04000ED0 RID: 3792
	public Vector2 roamDistance;

	// Token: 0x04000ED1 RID: 3793
	public Transform colliderParent;

	// Token: 0x04000ED2 RID: 3794
	public BossFormController.BossDamagePoint[] damagePoints;

	// Token: 0x04000ED3 RID: 3795
	public ArcadeEntityController flashController;

	// Token: 0x04000ED4 RID: 3796
	public float health = 50f;

	// Token: 0x02000BDA RID: 3034
	[Serializable]
	public class BossDamagePoint
	{
		// Token: 0x04003FEF RID: 16367
		public BoxCollider hitBox;

		// Token: 0x04003FF0 RID: 16368
		public float health;

		// Token: 0x04003FF1 RID: 16369
		public ArcadeEntityController damagePrefab;

		// Token: 0x04003FF2 RID: 16370
		public ArcadeEntityController damageInstance;

		// Token: 0x04003FF3 RID: 16371
		public bool destroyed;
	}
}
