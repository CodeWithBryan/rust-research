using System;
using UnityEngine;

// Token: 0x02000141 RID: 321
public class ChippyBoss : SpriteArcadeEntity
{
	// Token: 0x04000EE7 RID: 3815
	public Vector2 roamDistance;

	// Token: 0x04000EE8 RID: 3816
	public float animationSpeed = 0.5f;

	// Token: 0x04000EE9 RID: 3817
	public Sprite[] animationFrames;

	// Token: 0x04000EEA RID: 3818
	public ArcadeEntity bulletTest;

	// Token: 0x04000EEB RID: 3819
	public SpriteRenderer flashRenderer;

	// Token: 0x04000EEC RID: 3820
	public ChippyBoss.BossDamagePoint[] damagePoints;

	// Token: 0x02000BDB RID: 3035
	[Serializable]
	public class BossDamagePoint
	{
		// Token: 0x04003FF4 RID: 16372
		public BoxCollider hitBox;

		// Token: 0x04003FF5 RID: 16373
		public float health;

		// Token: 0x04003FF6 RID: 16374
		public ArcadeEntityController damagePrefab;

		// Token: 0x04003FF7 RID: 16375
		public ArcadeEntityController damageInstance;

		// Token: 0x04003FF8 RID: 16376
		public bool destroyed;
	}
}
