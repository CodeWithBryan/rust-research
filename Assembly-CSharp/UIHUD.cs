using System;
using UnityEngine;

// Token: 0x020007DB RID: 2011
public class UIHUD : SingletonComponent<UIHUD>, IUIScreen
{
	// Token: 0x04002C93 RID: 11411
	public UIChat chatPanel;

	// Token: 0x04002C94 RID: 11412
	public HudElement Hunger;

	// Token: 0x04002C95 RID: 11413
	public HudElement Thirst;

	// Token: 0x04002C96 RID: 11414
	public HudElement Health;

	// Token: 0x04002C97 RID: 11415
	public HudElement PendingHealth;

	// Token: 0x04002C98 RID: 11416
	public HudElement VehicleHealth;

	// Token: 0x04002C99 RID: 11417
	public HudElement AnimalStamina;

	// Token: 0x04002C9A RID: 11418
	public HudElement AnimalStaminaMax;

	// Token: 0x04002C9B RID: 11419
	public RectTransform vitalsRect;

	// Token: 0x04002C9C RID: 11420
	public Canvas healthCanvas;

	// Token: 0x04002C9D RID: 11421
	public UICompass CompassWidget;

	// Token: 0x04002C9E RID: 11422
	public GameObject KeyboardCaptureMode;
}
