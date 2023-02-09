using System;
using UnityEngine;

// Token: 0x02000143 RID: 323
public class ChippyMainCharacter : SpriteArcadeEntity
{
	// Token: 0x04000EF2 RID: 3826
	public float speed;

	// Token: 0x04000EF3 RID: 3827
	public float maxSpeed = 0.25f;

	// Token: 0x04000EF4 RID: 3828
	public ChippyBulletEntity bulletPrefab;

	// Token: 0x04000EF5 RID: 3829
	public float fireRate = 0.1f;

	// Token: 0x04000EF6 RID: 3830
	public Vector3 aimDir = Vector3.up;
}
