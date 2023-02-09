using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200071D RID: 1821
public static class AssetNameCache
{
	// Token: 0x06003296 RID: 12950 RVA: 0x00138D90 File Offset: 0x00136F90
	private static string LookupName(UnityEngine.Object obj)
	{
		if (obj == null)
		{
			return string.Empty;
		}
		string name;
		if (!AssetNameCache.mixed.TryGetValue(obj, out name))
		{
			name = obj.name;
			AssetNameCache.mixed.Add(obj, name);
		}
		return name;
	}

	// Token: 0x06003297 RID: 12951 RVA: 0x00138DD0 File Offset: 0x00136FD0
	private static string LookupNameLower(UnityEngine.Object obj)
	{
		if (obj == null)
		{
			return string.Empty;
		}
		string text;
		if (!AssetNameCache.lower.TryGetValue(obj, out text))
		{
			text = obj.name.ToLower();
			AssetNameCache.lower.Add(obj, text);
		}
		return text;
	}

	// Token: 0x06003298 RID: 12952 RVA: 0x00138E14 File Offset: 0x00137014
	private static string LookupNameUpper(UnityEngine.Object obj)
	{
		if (obj == null)
		{
			return string.Empty;
		}
		string text;
		if (!AssetNameCache.upper.TryGetValue(obj, out text))
		{
			text = obj.name.ToUpper();
			AssetNameCache.upper.Add(obj, text);
		}
		return text;
	}

	// Token: 0x06003299 RID: 12953 RVA: 0x00138E58 File Offset: 0x00137058
	public static string GetName(this PhysicMaterial mat)
	{
		return AssetNameCache.LookupName(mat);
	}

	// Token: 0x0600329A RID: 12954 RVA: 0x00138E60 File Offset: 0x00137060
	public static string GetNameLower(this PhysicMaterial mat)
	{
		return AssetNameCache.LookupNameLower(mat);
	}

	// Token: 0x0600329B RID: 12955 RVA: 0x00138E68 File Offset: 0x00137068
	public static string GetNameUpper(this PhysicMaterial mat)
	{
		return AssetNameCache.LookupNameUpper(mat);
	}

	// Token: 0x0600329C RID: 12956 RVA: 0x00138E58 File Offset: 0x00137058
	public static string GetName(this Material mat)
	{
		return AssetNameCache.LookupName(mat);
	}

	// Token: 0x0600329D RID: 12957 RVA: 0x00138E60 File Offset: 0x00137060
	public static string GetNameLower(this Material mat)
	{
		return AssetNameCache.LookupNameLower(mat);
	}

	// Token: 0x0600329E RID: 12958 RVA: 0x00138E68 File Offset: 0x00137068
	public static string GetNameUpper(this Material mat)
	{
		return AssetNameCache.LookupNameUpper(mat);
	}

	// Token: 0x040028E9 RID: 10473
	private static Dictionary<UnityEngine.Object, string> mixed = new Dictionary<UnityEngine.Object, string>();

	// Token: 0x040028EA RID: 10474
	private static Dictionary<UnityEngine.Object, string> lower = new Dictionary<UnityEngine.Object, string>();

	// Token: 0x040028EB RID: 10475
	private static Dictionary<UnityEngine.Object, string> upper = new Dictionary<UnityEngine.Object, string>();
}
