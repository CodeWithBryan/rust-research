using System;
using UnityEngine;

// Token: 0x02000933 RID: 2355
[Serializable]
public class ResourceRef<T> where T : UnityEngine.Object
{
	// Token: 0x1700043D RID: 1085
	// (get) Token: 0x06003812 RID: 14354 RVA: 0x0014BDFF File Offset: 0x00149FFF
	public bool isValid
	{
		get
		{
			return !string.IsNullOrEmpty(this.guid);
		}
	}

	// Token: 0x06003813 RID: 14355 RVA: 0x0014BE0F File Offset: 0x0014A00F
	public T Get()
	{
		if (this._cachedObject == null)
		{
			this._cachedObject = (GameManifest.GUIDToObject(this.guid) as T);
		}
		return this._cachedObject;
	}

	// Token: 0x1700043E RID: 1086
	// (get) Token: 0x06003814 RID: 14356 RVA: 0x0014BE45 File Offset: 0x0014A045
	public string resourcePath
	{
		get
		{
			return GameManifest.GUIDToPath(this.guid);
		}
	}

	// Token: 0x1700043F RID: 1087
	// (get) Token: 0x06003815 RID: 14357 RVA: 0x0014BE52 File Offset: 0x0014A052
	public uint resourceID
	{
		get
		{
			return StringPool.Get(this.resourcePath);
		}
	}

	// Token: 0x0400321E RID: 12830
	public string guid;

	// Token: 0x0400321F RID: 12831
	private T _cachedObject;
}
