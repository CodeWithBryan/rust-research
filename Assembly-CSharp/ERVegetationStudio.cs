using System;
using EasyRoads3Dv3;
using UnityEngine;

// Token: 0x02000956 RID: 2390
public class ERVegetationStudio : ScriptableObject
{
	// Token: 0x06003862 RID: 14434 RVA: 0x00007074 File Offset: 0x00005274
	public static bool VegetationStudio()
	{
		return false;
	}

	// Token: 0x06003863 RID: 14435 RVA: 0x00007074 File Offset: 0x00005274
	public static bool VegetationStudioPro()
	{
		return false;
	}

	// Token: 0x06003864 RID: 14436 RVA: 0x000059DD File Offset: 0x00003BDD
	public static void CreateVegetationMaskLine(GameObject go, float grassPerimeter, float plantPerimeter, float treePerimeter, float objectPerimeter, float largeObjectPerimeter)
	{
	}

	// Token: 0x06003865 RID: 14437 RVA: 0x000059DD File Offset: 0x00003BDD
	public static void UpdateVegetationMaskLine(GameObject go, ERVSData[] vsData, float grassPerimeter, float plantPerimeter, float treePerimeter, float objectPerimeter, float largeObjectPerimeter)
	{
	}

	// Token: 0x06003866 RID: 14438 RVA: 0x000059DD File Offset: 0x00003BDD
	public static void UpdateHeightmap(Bounds bounds)
	{
	}

	// Token: 0x06003867 RID: 14439 RVA: 0x000059DD File Offset: 0x00003BDD
	public static void RemoveVegetationMaskLine(GameObject go)
	{
	}

	// Token: 0x06003868 RID: 14440 RVA: 0x000059DD File Offset: 0x00003BDD
	public static void CreateBiomeArea(GameObject go, float distance, float blendDistance, float noise)
	{
	}

	// Token: 0x06003869 RID: 14441 RVA: 0x000059DD File Offset: 0x00003BDD
	public static void UpdateBiomeArea(GameObject go, ERVSData[] vsData, float distance, float blendDistance, float noise)
	{
	}

	// Token: 0x0600386A RID: 14442 RVA: 0x000059DD File Offset: 0x00003BDD
	public static void RemoveBiomeArea(GameObject go)
	{
	}
}
