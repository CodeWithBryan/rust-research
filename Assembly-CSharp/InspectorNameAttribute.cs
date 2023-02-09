using System;
using UnityEngine;

// Token: 0x020008BE RID: 2238
public class InspectorNameAttribute : PropertyAttribute
{
	// Token: 0x06003611 RID: 13841 RVA: 0x0014322F File Offset: 0x0014142F
	public InspectorNameAttribute(string name)
	{
		this.name = name;
	}

	// Token: 0x04003108 RID: 12552
	public string name;
}
