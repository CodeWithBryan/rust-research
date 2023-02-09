using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200079D RID: 1949
public class KeyCodeEntry : UIDialog
{
	// Token: 0x04002B18 RID: 11032
	public Text textDisplay;

	// Token: 0x04002B19 RID: 11033
	public Action<string> onCodeEntered;

	// Token: 0x04002B1A RID: 11034
	public Action onClosed;

	// Token: 0x04002B1B RID: 11035
	public Text typeDisplay;

	// Token: 0x04002B1C RID: 11036
	public Translate.Phrase masterCodePhrase;

	// Token: 0x04002B1D RID: 11037
	public Translate.Phrase guestCodePhrase;

	// Token: 0x04002B1E RID: 11038
	public GameObject memoryKeycodeButton;
}
