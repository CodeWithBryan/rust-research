using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200086F RID: 2159
public class UIPaintableImage : MonoBehaviour
{
	// Token: 0x17000401 RID: 1025
	// (get) Token: 0x06003541 RID: 13633 RVA: 0x000B11AC File Offset: 0x000AF3AC
	public RectTransform rectTransform
	{
		get
		{
			return base.transform as RectTransform;
		}
	}

	// Token: 0x04002FD5 RID: 12245
	public RawImage image;

	// Token: 0x04002FD6 RID: 12246
	public int texSize = 64;

	// Token: 0x04002FD7 RID: 12247
	public Color clearColor = Color.clear;

	// Token: 0x04002FD8 RID: 12248
	public FilterMode filterMode = FilterMode.Bilinear;

	// Token: 0x04002FD9 RID: 12249
	public bool mipmaps;

	// Token: 0x02000E42 RID: 3650
	public enum DrawMode
	{
		// Token: 0x040049C3 RID: 18883
		AlphaBlended,
		// Token: 0x040049C4 RID: 18884
		Additive,
		// Token: 0x040049C5 RID: 18885
		Lighten,
		// Token: 0x040049C6 RID: 18886
		Erase
	}
}
