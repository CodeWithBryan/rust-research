using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200078F RID: 1935
public class BlackjackScreenCardUI : FacepunchBehaviour
{
	// Token: 0x04002A62 RID: 10850
	[SerializeField]
	private Canvas baseCanvas;

	// Token: 0x04002A63 RID: 10851
	[SerializeField]
	private Canvas cardFront;

	// Token: 0x04002A64 RID: 10852
	[SerializeField]
	private Canvas cardBack;

	// Token: 0x04002A65 RID: 10853
	[SerializeField]
	private Image image;

	// Token: 0x04002A66 RID: 10854
	[SerializeField]
	private RustText text;

	// Token: 0x04002A67 RID: 10855
	[SerializeField]
	private Sprite heartSprite;

	// Token: 0x04002A68 RID: 10856
	[SerializeField]
	private Sprite diamondSprite;

	// Token: 0x04002A69 RID: 10857
	[SerializeField]
	private Sprite spadeSprite;

	// Token: 0x04002A6A RID: 10858
	[SerializeField]
	private Sprite clubSprite;
}
