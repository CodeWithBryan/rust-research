using System;
using UnityEngine;

// Token: 0x02000677 RID: 1655
public class TerrainMargin
{
	// Token: 0x06002F41 RID: 12097 RVA: 0x0011AD1C File Offset: 0x00118F1C
	public static void Create()
	{
		Material marginMaterial = TerrainMeta.Config.MarginMaterial;
		Vector3 center = TerrainMeta.Center;
		Vector3 size = TerrainMeta.Size;
		Vector3 b = new Vector3(size.x, 0f, 0f);
		Vector3 b2 = new Vector3(0f, 0f, size.z);
		center.y = TerrainMeta.HeightMap.GetHeight(0, 0);
		TerrainMargin.Create(center - b2, size, marginMaterial);
		TerrainMargin.Create(center - b2 - b, size, marginMaterial);
		TerrainMargin.Create(center - b2 + b, size, marginMaterial);
		TerrainMargin.Create(center - b, size, marginMaterial);
		TerrainMargin.Create(center + b, size, marginMaterial);
		TerrainMargin.Create(center + b2, size, marginMaterial);
		TerrainMargin.Create(center + b2 - b, size, marginMaterial);
		TerrainMargin.Create(center + b2 + b, size, marginMaterial);
	}

	// Token: 0x06002F42 RID: 12098 RVA: 0x0011AE10 File Offset: 0x00119010
	private static void Create(Vector3 position, Vector3 size, Material material)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
		gameObject.name = "TerrainMargin";
		gameObject.layer = 16;
		gameObject.transform.position = position;
		gameObject.transform.localScale = size * 0.1f;
		UnityEngine.Object.Destroy(gameObject.GetComponent<MeshRenderer>());
		UnityEngine.Object.Destroy(gameObject.GetComponent<MeshFilter>());
	}

	// Token: 0x04002635 RID: 9781
	private static MaterialPropertyBlock materialPropertyBlock;
}
