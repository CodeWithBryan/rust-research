using System;
using UnityEngine;

// Token: 0x0200019E RID: 414
public class TreadAnimator : MonoBehaviour, IClientComponent
{
	// Token: 0x040010A5 RID: 4261
	public Animator mainBodyAnimator;

	// Token: 0x040010A6 RID: 4262
	public Transform[] wheelBones;

	// Token: 0x040010A7 RID: 4263
	public Vector3[] vecShocksOffsetPosition;

	// Token: 0x040010A8 RID: 4264
	public Vector3[] wheelBoneOrigin;

	// Token: 0x040010A9 RID: 4265
	public float wheelBoneDistMax = 0.26f;

	// Token: 0x040010AA RID: 4266
	public Material leftTread;

	// Token: 0x040010AB RID: 4267
	public Material rightTread;

	// Token: 0x040010AC RID: 4268
	public TreadEffects treadEffects;

	// Token: 0x040010AD RID: 4269
	public float traceThickness = 0.25f;

	// Token: 0x040010AE RID: 4270
	public float heightFudge = 0.13f;

	// Token: 0x040010AF RID: 4271
	public bool useWheelYOrigin;

	// Token: 0x040010B0 RID: 4272
	public Vector2 treadTextureDirection = new Vector2(1f, 0f);

	// Token: 0x040010B1 RID: 4273
	public bool isMetallic;
}
