using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007DA RID: 2010
public class UIFishing : SingletonComponent<UIFishing>
{
	// Token: 0x0600341A RID: 13338 RVA: 0x0013B4E2 File Offset: 0x001396E2
	private void Start()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04002C90 RID: 11408
	public Slider TensionLine;

	// Token: 0x04002C91 RID: 11409
	public Image FillImage;

	// Token: 0x04002C92 RID: 11410
	public Gradient FillGradient;
}
