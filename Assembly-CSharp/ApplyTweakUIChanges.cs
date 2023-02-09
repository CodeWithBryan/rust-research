using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000850 RID: 2128
public class ApplyTweakUIChanges : MonoBehaviour
{
	// Token: 0x060034DE RID: 13534 RVA: 0x0013F3F6 File Offset: 0x0013D5F6
	private void OnEnable()
	{
		this.SetClean();
	}

	// Token: 0x060034DF RID: 13535 RVA: 0x0013F400 File Offset: 0x0013D600
	public void Apply()
	{
		if (this.Options == null)
		{
			return;
		}
		foreach (TweakUIBase tweakUIBase in this.Options)
		{
			if (!(tweakUIBase == null))
			{
				tweakUIBase.OnApplyClicked();
			}
		}
		this.SetClean();
	}

	// Token: 0x060034E0 RID: 13536 RVA: 0x0013F444 File Offset: 0x0013D644
	public void SetDirty()
	{
		if (this.ApplyButton != null)
		{
			this.ApplyButton.interactable = true;
		}
	}

	// Token: 0x060034E1 RID: 13537 RVA: 0x0013F460 File Offset: 0x0013D660
	public void SetClean()
	{
		if (this.ApplyButton != null)
		{
			this.ApplyButton.interactable = false;
		}
	}

	// Token: 0x04002F5F RID: 12127
	public Button ApplyButton;

	// Token: 0x04002F60 RID: 12128
	public TweakUIBase[] Options;
}
