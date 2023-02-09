using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000774 RID: 1908
public class Crosshair : BaseMonoBehaviour
{
	// Token: 0x040029ED RID: 10733
	public static bool Enabled = true;

	// Token: 0x040029EE RID: 10734
	public Image Image;

	// Token: 0x040029EF RID: 10735
	public RectTransform reticleTransform;

	// Token: 0x040029F0 RID: 10736
	public CanvasGroup reticleAlpha;

	// Token: 0x040029F1 RID: 10737
	public RectTransform hitNotifyMarker;

	// Token: 0x040029F2 RID: 10738
	public CanvasGroup hitNotifyAlpha;

	// Token: 0x040029F3 RID: 10739
	public static Crosshair instance;

	// Token: 0x040029F4 RID: 10740
	public static float lastHitTime = 0f;

	// Token: 0x040029F5 RID: 10741
	public float crosshairAlpha = 0.75f;
}
