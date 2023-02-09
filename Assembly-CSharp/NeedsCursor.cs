using System;
using UnityEngine;

// Token: 0x02000776 RID: 1910
public class NeedsCursor : MonoBehaviour, IClientComponent
{
	// Token: 0x0600336A RID: 13162 RVA: 0x0013B805 File Offset: 0x00139A05
	private void Update()
	{
		CursorManager.HoldOpen(false);
	}
}
