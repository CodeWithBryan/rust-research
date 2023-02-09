using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004ED RID: 1261
public class Gibbable : PrefabAttribute, IClientComponent
{
	// Token: 0x06002809 RID: 10249 RVA: 0x000F5199 File Offset: 0x000F3399
	protected override Type GetIndexedType()
	{
		return typeof(Gibbable);
	}

	// Token: 0x04002036 RID: 8246
	public GameObject gibSource;

	// Token: 0x04002037 RID: 8247
	public Material[] customMaterials;

	// Token: 0x04002038 RID: 8248
	public GameObject materialSource;

	// Token: 0x04002039 RID: 8249
	public bool copyMaterialBlock = true;

	// Token: 0x0400203A RID: 8250
	public bool applyDamageTexture;

	// Token: 0x0400203B RID: 8251
	public PhysicMaterial physicsMaterial;

	// Token: 0x0400203C RID: 8252
	public GameObjectRef fxPrefab;

	// Token: 0x0400203D RID: 8253
	public bool spawnFxPrefab = true;

	// Token: 0x0400203E RID: 8254
	[Tooltip("If enabled, gibs will spawn even though we've hit a gib limit")]
	public bool important;

	// Token: 0x0400203F RID: 8255
	public bool useContinuousCollision;

	// Token: 0x04002040 RID: 8256
	public float explodeScale;

	// Token: 0x04002041 RID: 8257
	public float scaleOverride = 1f;

	// Token: 0x04002042 RID: 8258
	[ReadOnly]
	public int uniqueId;

	// Token: 0x04002043 RID: 8259
	public Gibbable.BoundsEffectType boundsEffectType;

	// Token: 0x04002044 RID: 8260
	public bool isConditional;

	// Token: 0x04002045 RID: 8261
	[ReadOnly]
	public Bounds effectBounds;

	// Token: 0x04002046 RID: 8262
	public List<Gibbable.OverrideMesh> MeshOverrides = new List<Gibbable.OverrideMesh>();

	// Token: 0x02000CE0 RID: 3296
	[Serializable]
	public struct OverrideMesh
	{
		// Token: 0x04004418 RID: 17432
		public bool enabled;

		// Token: 0x04004419 RID: 17433
		public Gibbable.ColliderType ColliderType;

		// Token: 0x0400441A RID: 17434
		public Vector3 BoxSize;

		// Token: 0x0400441B RID: 17435
		public Vector3 ColliderCentre;

		// Token: 0x0400441C RID: 17436
		public float ColliderRadius;

		// Token: 0x0400441D RID: 17437
		public float CapsuleHeight;

		// Token: 0x0400441E RID: 17438
		public int CapsuleDirection;

		// Token: 0x0400441F RID: 17439
		public bool BlockMaterialCopy;
	}

	// Token: 0x02000CE1 RID: 3297
	public enum ColliderType
	{
		// Token: 0x04004421 RID: 17441
		Box,
		// Token: 0x04004422 RID: 17442
		Sphere,
		// Token: 0x04004423 RID: 17443
		Capsule
	}

	// Token: 0x02000CE2 RID: 3298
	public enum ParentingType
	{
		// Token: 0x04004425 RID: 17445
		None,
		// Token: 0x04004426 RID: 17446
		GibsOnly,
		// Token: 0x04004427 RID: 17447
		FXOnly,
		// Token: 0x04004428 RID: 17448
		All
	}

	// Token: 0x02000CE3 RID: 3299
	public enum BoundsEffectType
	{
		// Token: 0x0400442A RID: 17450
		None,
		// Token: 0x0400442B RID: 17451
		Electrical,
		// Token: 0x0400442C RID: 17452
		Glass,
		// Token: 0x0400442D RID: 17453
		Scrap,
		// Token: 0x0400442E RID: 17454
		Stone,
		// Token: 0x0400442F RID: 17455
		Wood
	}
}
