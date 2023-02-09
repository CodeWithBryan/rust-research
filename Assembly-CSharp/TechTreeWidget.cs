using System;
using UnityEngine;

// Token: 0x020007AE RID: 1966
public class TechTreeWidget : BaseMonoBehaviour
{
	// Token: 0x170003F9 RID: 1017
	// (get) Token: 0x060033B0 RID: 13232 RVA: 0x0013B9B7 File Offset: 0x00139BB7
	public RectTransform rectTransform
	{
		get
		{
			return base.GetComponent<RectTransform>();
		}
	}

	// Token: 0x04002B9E RID: 11166
	public int id;
}
