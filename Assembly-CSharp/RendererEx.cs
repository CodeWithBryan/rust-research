using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020008F4 RID: 2292
public static class RendererEx
{
	// Token: 0x060036AA RID: 13994 RVA: 0x00145378 File Offset: 0x00143578
	public static void SetSharedMaterials(this Renderer renderer, List<Material> materials)
	{
		if (materials.Count == 0)
		{
			return;
		}
		if (materials.Count > 10)
		{
			throw new ArgumentOutOfRangeException("materials");
		}
		Material[] array = RendererEx.ArrayCache.Get(materials.Count);
		for (int i = 0; i < materials.Count; i++)
		{
			array[i] = materials[i];
		}
		renderer.sharedMaterials = array;
	}

	// Token: 0x0400318F RID: 12687
	private static readonly Memoized<Material[], int> ArrayCache = new Memoized<Material[], int>((int n) => new Material[n]);
}
