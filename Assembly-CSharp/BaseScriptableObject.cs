using System;
using UnityEngine;

// Token: 0x020008BC RID: 2236
public class BaseScriptableObject : ScriptableObject
{
	// Token: 0x06003609 RID: 13833 RVA: 0x001431A8 File Offset: 0x001413A8
	public string LookupFileName()
	{
		return StringPool.Get(this.FilenameStringId);
	}

	// Token: 0x0600360A RID: 13834 RVA: 0x001431B5 File Offset: 0x001413B5
	public static bool operator ==(BaseScriptableObject a, BaseScriptableObject b)
	{
		return a == b || (a != null && b != null && a.FilenameStringId == b.FilenameStringId);
	}

	// Token: 0x0600360B RID: 13835 RVA: 0x001431D3 File Offset: 0x001413D3
	public static bool operator !=(BaseScriptableObject a, BaseScriptableObject b)
	{
		return !(a == b);
	}

	// Token: 0x0600360C RID: 13836 RVA: 0x001431DF File Offset: 0x001413DF
	public override int GetHashCode()
	{
		return (int)this.FilenameStringId;
	}

	// Token: 0x0600360D RID: 13837 RVA: 0x001431E7 File Offset: 0x001413E7
	public override bool Equals(object o)
	{
		return o != null && o is BaseScriptableObject && o as BaseScriptableObject == this;
	}

	// Token: 0x04003107 RID: 12551
	[HideInInspector]
	public uint FilenameStringId;
}
