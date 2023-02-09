using System;
using TMPro;
using UnityEngine;

// Token: 0x020008A7 RID: 2215
public class UISleepingScreen : SingletonComponent<UISleepingScreen>, IUIScreen
{
	// Token: 0x060035D9 RID: 13785 RVA: 0x00142C7E File Offset: 0x00140E7E
	protected override void Awake()
	{
		base.Awake();
		this.canvasGroup = base.GetComponent<CanvasGroup>();
		this.visible = true;
	}

	// Token: 0x060035DA RID: 13786 RVA: 0x00142C9C File Offset: 0x00140E9C
	public void SetVisible(bool b)
	{
		if (this.visible == b)
		{
			return;
		}
		this.visible = b;
		this.canvasGroup.alpha = (this.visible ? 1f : 0f);
		SingletonComponent<UISleepingScreen>.Instance.gameObject.SetChildComponentsEnabled(this.visible);
	}

	// Token: 0x040030DF RID: 12511
	protected CanvasGroup canvasGroup;

	// Token: 0x040030E0 RID: 12512
	private bool visible;
}
