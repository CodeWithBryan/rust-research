using System;
using UnityEngine;

// Token: 0x020002A7 RID: 679
public class Gib : ListComponent<Gib>
{
	// Token: 0x06001C3A RID: 7226 RVA: 0x000C32D8 File Offset: 0x000C14D8
	public static string GetEffect(PhysicMaterial physicMaterial)
	{
		string nameLower = physicMaterial.GetNameLower();
		if (nameLower == "wood")
		{
			return "assets/bundled/prefabs/fx/building/wood_gib.prefab";
		}
		if (nameLower == "concrete")
		{
			return "assets/bundled/prefabs/fx/building/stone_gib.prefab";
		}
		if (nameLower == "metal")
		{
			return "assets/bundled/prefabs/fx/building/metal_sheet_gib.prefab";
		}
		if (nameLower == "rock")
		{
			return "assets/bundled/prefabs/fx/building/stone_gib.prefab";
		}
		if (!(nameLower == "flesh"))
		{
			return "assets/bundled/prefabs/fx/building/wood_gib.prefab";
		}
		return "assets/bundled/prefabs/fx/building/wood_gib.prefab";
	}

	// Token: 0x04001576 RID: 5494
	public static int gibCount;

	// Token: 0x04001577 RID: 5495
	public MeshFilter _meshFilter;

	// Token: 0x04001578 RID: 5496
	public MeshRenderer _meshRenderer;

	// Token: 0x04001579 RID: 5497
	public MeshCollider _meshCollider;

	// Token: 0x0400157A RID: 5498
	public BoxCollider _boxCollider;

	// Token: 0x0400157B RID: 5499
	public SphereCollider _sphereCollider;

	// Token: 0x0400157C RID: 5500
	public CapsuleCollider _capsuleCollider;

	// Token: 0x0400157D RID: 5501
	public Rigidbody _rigidbody;
}
