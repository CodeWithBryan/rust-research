using System;
using UnityEngine;

// Token: 0x020001AF RID: 431
public class SprayCanSpray_Decal : SprayCanSpray, ICustomMaterialReplacer, IPropRenderNotify, INotifyLOD
{
	// Token: 0x040010F3 RID: 4339
	public DeferredDecal DecalComponent;

	// Token: 0x040010F4 RID: 4340
	public GameObject IconPreviewRoot;

	// Token: 0x040010F5 RID: 4341
	public Material DefaultMaterial;
}
