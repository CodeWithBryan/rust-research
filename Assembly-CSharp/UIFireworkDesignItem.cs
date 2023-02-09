using System;
using Rust.UI;
using UnityEngine;

// Token: 0x0200079A RID: 1946
public class UIFireworkDesignItem : MonoBehaviour
{
	// Token: 0x04002B09 RID: 11017
	public static readonly Translate.Phrase EmptyPhrase = new Translate.Phrase("firework.pattern.design.empty", "Empty");

	// Token: 0x04002B0A RID: 11018
	public static readonly Translate.Phrase UntitledPhrase = new Translate.Phrase("firework.pattern.design.untitled", "Untitled");

	// Token: 0x04002B0B RID: 11019
	public RustText Title;

	// Token: 0x04002B0C RID: 11020
	public RustButton LoadButton;

	// Token: 0x04002B0D RID: 11021
	public RustButton SaveButton;

	// Token: 0x04002B0E RID: 11022
	public RustButton EraseButton;

	// Token: 0x04002B0F RID: 11023
	public UIFireworkDesigner Designer;

	// Token: 0x04002B10 RID: 11024
	public int Index;
}
