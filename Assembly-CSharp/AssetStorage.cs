using System;
using UnityEngine;

// Token: 0x020008FE RID: 2302
public static class AssetStorage
{
	// Token: 0x060036D3 RID: 14035 RVA: 0x0014643F File Offset: 0x0014463F
	public static void Save<T>(ref T asset, string path) where T : UnityEngine.Object
	{
		asset;
	}

	// Token: 0x060036D4 RID: 14036 RVA: 0x000059DD File Offset: 0x00003BDD
	public static void Save(ref Texture2D asset)
	{
	}

	// Token: 0x060036D5 RID: 14037 RVA: 0x00146452 File Offset: 0x00144652
	public static void Save(ref Texture2D asset, string path, bool linear, bool compress)
	{
		asset;
	}

	// Token: 0x060036D6 RID: 14038 RVA: 0x000059DD File Offset: 0x00003BDD
	public static void Load<T>(ref T asset, string path) where T : UnityEngine.Object
	{
	}

	// Token: 0x060036D7 RID: 14039 RVA: 0x0014645C File Offset: 0x0014465C
	public static void Delete<T>(ref T asset) where T : UnityEngine.Object
	{
		if (!asset)
		{
			return;
		}
		UnityEngine.Object.Destroy(asset);
		asset = default(T);
	}
}
