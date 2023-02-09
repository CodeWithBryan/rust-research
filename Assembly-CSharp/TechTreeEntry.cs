using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007AA RID: 1962
public class TechTreeEntry : TechTreeWidget
{
	// Token: 0x04002B7A RID: 11130
	public RawImage icon;

	// Token: 0x04002B7B RID: 11131
	public GameObject ableToUnlockBackground;

	// Token: 0x04002B7C RID: 11132
	public GameObject unlockedBackground;

	// Token: 0x04002B7D RID: 11133
	public GameObject lockedBackground;

	// Token: 0x04002B7E RID: 11134
	public GameObject lockOverlay;

	// Token: 0x04002B7F RID: 11135
	public GameObject selectedBackground;

	// Token: 0x04002B80 RID: 11136
	public Image radialUnlock;

	// Token: 0x04002B81 RID: 11137
	public float holdTime = 1f;
}
