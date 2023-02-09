using System;
using UnityEngine;

// Token: 0x0200067C RID: 1660
public class TerrainPhysics : TerrainExtension
{
	// Token: 0x06002F9A RID: 12186 RVA: 0x0011C571 File Offset: 0x0011A771
	public override void Setup()
	{
		this.splat = this.terrain.GetComponent<TerrainSplatMap>();
		this.materials = this.config.GetPhysicMaterials();
	}

	// Token: 0x06002F9B RID: 12187 RVA: 0x0011C595 File Offset: 0x0011A795
	public PhysicMaterial GetMaterial(Vector3 worldPos)
	{
		return this.materials[this.splat.GetSplatMaxIndex(worldPos, -1)];
	}

	// Token: 0x0400266D RID: 9837
	private TerrainSplatMap splat;

	// Token: 0x0400266E RID: 9838
	private PhysicMaterial[] materials;
}
