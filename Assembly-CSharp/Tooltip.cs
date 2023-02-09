using System;
using UnityEngine;

// Token: 0x02000890 RID: 2192
public class Tooltip : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x1700040C RID: 1036
	// (get) Token: 0x060035A3 RID: 13731 RVA: 0x00142368 File Offset: 0x00140568
	public string english
	{
		get
		{
			return this.Text;
		}
	}

	// Token: 0x0400309F RID: 12447
	public static GameObject Current;

	// Token: 0x040030A0 RID: 12448
	[TextArea]
	public string Text;

	// Token: 0x040030A1 RID: 12449
	public GameObject TooltipObject;

	// Token: 0x040030A2 RID: 12450
	public string token = "";
}
