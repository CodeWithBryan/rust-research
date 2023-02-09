using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000FD RID: 253
public class ComputerMenu : UIDialog
{
	// Token: 0x04000D5D RID: 3421
	public RectTransform bookmarkContainer;

	// Token: 0x04000D5E RID: 3422
	public GameObject bookmarkPrefab;

	// Token: 0x04000D5F RID: 3423
	public List<RCBookmarkEntry> activeEntries = new List<RCBookmarkEntry>();
}
