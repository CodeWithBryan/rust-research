using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000896 RID: 2198
public class UICameraOverlay : SingletonComponent<UICameraOverlay>
{
	// Token: 0x060035AD RID: 13741 RVA: 0x0014248D File Offset: 0x0014068D
	public void Show()
	{
		this.CanvasGroup.alpha = 1f;
	}

	// Token: 0x060035AE RID: 13742 RVA: 0x0014249F File Offset: 0x0014069F
	public void Hide()
	{
		this.CanvasGroup.alpha = 0f;
	}

	// Token: 0x060035AF RID: 13743 RVA: 0x001424B1 File Offset: 0x001406B1
	public void SetFocusMode(CameraFocusMode mode)
	{
		if (mode == CameraFocusMode.Auto)
		{
			this.FocusModeLabel.SetPhrase(UICameraOverlay.FocusAutoText);
			return;
		}
		if (mode != CameraFocusMode.Manual)
		{
			this.FocusModeLabel.SetPhrase(UICameraOverlay.FocusOffText);
			return;
		}
		this.FocusModeLabel.SetPhrase(UICameraOverlay.FocusManualText);
	}

	// Token: 0x040030AA RID: 12458
	public static readonly Translate.Phrase FocusOffText = new Translate.Phrase("camera.infinite_focus", "Infinite Focus");

	// Token: 0x040030AB RID: 12459
	public static readonly Translate.Phrase FocusAutoText = new Translate.Phrase("camera.auto_focus", "Auto Focus");

	// Token: 0x040030AC RID: 12460
	public static readonly Translate.Phrase FocusManualText = new Translate.Phrase("camera.manual_focus", "Manual Focus");

	// Token: 0x040030AD RID: 12461
	public CanvasGroup CanvasGroup;

	// Token: 0x040030AE RID: 12462
	public RustText FocusModeLabel;
}
