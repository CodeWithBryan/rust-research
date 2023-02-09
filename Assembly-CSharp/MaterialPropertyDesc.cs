using System;
using UnityEngine;

// Token: 0x02000702 RID: 1794
public struct MaterialPropertyDesc
{
	// Token: 0x060031BD RID: 12733 RVA: 0x00130F81 File Offset: 0x0012F181
	public MaterialPropertyDesc(string name, Type type)
	{
		this.nameID = Shader.PropertyToID(name);
		this.type = type;
	}

	// Token: 0x0400285D RID: 10333
	public int nameID;

	// Token: 0x0400285E RID: 10334
	public Type type;
}
