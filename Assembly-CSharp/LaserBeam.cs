using System;
using UnityEngine;

// Token: 0x020001B7 RID: 439
public class LaserBeam : MonoBehaviour
{
	// Token: 0x0400110B RID: 4363
	public float scrollSpeed = 0.5f;

	// Token: 0x0400110C RID: 4364
	public LineRenderer beamRenderer;

	// Token: 0x0400110D RID: 4365
	public GameObject dotObject;

	// Token: 0x0400110E RID: 4366
	public Renderer dotRenderer;

	// Token: 0x0400110F RID: 4367
	public GameObject dotSpotlight;

	// Token: 0x04001110 RID: 4368
	public Vector2 scrollDir;

	// Token: 0x04001111 RID: 4369
	public float maxDistance = 100f;

	// Token: 0x04001112 RID: 4370
	public float stillBlendFactor = 0.1f;

	// Token: 0x04001113 RID: 4371
	public float movementBlendFactor = 0.5f;

	// Token: 0x04001114 RID: 4372
	public float movementThreshhold = 0.15f;

	// Token: 0x04001115 RID: 4373
	public bool isFirstPerson;

	// Token: 0x04001116 RID: 4374
	public Transform emissionOverride;

	// Token: 0x04001117 RID: 4375
	private MaterialPropertyBlock block;
}
