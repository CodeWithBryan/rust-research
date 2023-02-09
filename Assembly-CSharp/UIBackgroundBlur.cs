using System;
using UnityEngine;

// Token: 0x02000894 RID: 2196
public class UIBackgroundBlur : ListComponent<UIBackgroundBlur>, IClientComponent
{
	// Token: 0x1700040D RID: 1037
	// (get) Token: 0x060035AA RID: 13738 RVA: 0x00142418 File Offset: 0x00140618
	public static float currentMax
	{
		get
		{
			if (ListComponent<UIBackgroundBlur>.InstanceList.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < ListComponent<UIBackgroundBlur>.InstanceList.Count; i++)
			{
				num = Mathf.Max(ListComponent<UIBackgroundBlur>.InstanceList[i].amount, num);
			}
			return num;
		}
	}

	// Token: 0x040030A6 RID: 12454
	public float amount = 1f;
}
