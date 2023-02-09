using System;
using UnityEngine;

// Token: 0x0200026B RID: 619
public class CanvasOrderHack : MonoBehaviour
{
	// Token: 0x06001BC2 RID: 7106 RVA: 0x000C0F14 File Offset: 0x000BF114
	private void OnEnable()
	{
		foreach (Canvas canvas in base.GetComponentsInChildren<Canvas>(true))
		{
			if (canvas.overrideSorting)
			{
				Canvas canvas2 = canvas;
				int sortingOrder = canvas2.sortingOrder;
				canvas2.sortingOrder = sortingOrder + 1;
			}
		}
		foreach (Canvas canvas3 in base.GetComponentsInChildren<Canvas>(true))
		{
			if (canvas3.overrideSorting)
			{
				Canvas canvas4 = canvas3;
				int sortingOrder = canvas4.sortingOrder;
				canvas4.sortingOrder = sortingOrder - 1;
			}
		}
	}
}
