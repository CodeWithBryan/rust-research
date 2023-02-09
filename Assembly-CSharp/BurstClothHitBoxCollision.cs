using System;
using System.Collections.Generic;
using Facepunch.BurstCloth;
using UnityEngine;

// Token: 0x020000F4 RID: 244
public class BurstClothHitBoxCollision : BurstCloth, IClientComponent, IPrefabPreProcess
{
	// Token: 0x060014A9 RID: 5289 RVA: 0x000059DD File Offset: 0x00003BDD
	protected override void GatherColliders(List<CapsuleParams> colliders)
	{
	}

	// Token: 0x060014AA RID: 5290 RVA: 0x000059DD File Offset: 0x00003BDD
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
	}

	// Token: 0x04000D3F RID: 3391
	[Header("Rust Wearable BurstCloth")]
	public bool UseLocalGravity = true;

	// Token: 0x04000D40 RID: 3392
	public float GravityStrength = 0.8f;

	// Token: 0x04000D41 RID: 3393
	public float DefaultLength = 1f;

	// Token: 0x04000D42 RID: 3394
	public float MountedLengthMultiplier;

	// Token: 0x04000D43 RID: 3395
	public float DuckedLengthMultiplier = 0.5f;

	// Token: 0x04000D44 RID: 3396
	public float CorpseLengthMultiplier = 0.2f;

	// Token: 0x04000D45 RID: 3397
	public Transform UpAxis;

	// Token: 0x04000D46 RID: 3398
	[Header("Collision")]
	public Transform ColliderRoot;

	// Token: 0x04000D47 RID: 3399
	[Tooltip("Keywords in bone names which should be ignored for collision")]
	public string[] IgnoreKeywords;
}
