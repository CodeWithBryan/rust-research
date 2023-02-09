using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200083A RID: 2106
public class LookatTooltip : MonoBehaviour
{
	// Token: 0x04002EE9 RID: 12009
	public static bool Enabled = true;

	// Token: 0x04002EEA RID: 12010
	[NonSerialized]
	public BaseEntity currentlyLookingAt;

	// Token: 0x04002EEB RID: 12011
	public RustText textLabel;

	// Token: 0x04002EEC RID: 12012
	public Image icon;

	// Token: 0x04002EED RID: 12013
	public CanvasGroup canvasGroup;

	// Token: 0x04002EEE RID: 12014
	public CanvasGroup infoGroup;

	// Token: 0x04002EEF RID: 12015
	public CanvasGroup minimiseGroup;
}
