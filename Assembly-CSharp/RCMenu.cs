using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000FF RID: 255
public class RCMenu : ComputerMenu
{
	// Token: 0x04000D6E RID: 3438
	public Image backgroundOpaque;

	// Token: 0x04000D6F RID: 3439
	public InputField newBookmarkEntryField;

	// Token: 0x04000D70 RID: 3440
	public NeedsCursor needsCursor;

	// Token: 0x04000D71 RID: 3441
	public float hiddenOffset = -256f;

	// Token: 0x04000D72 RID: 3442
	public RectTransform devicesPanel;

	// Token: 0x04000D73 RID: 3443
	private Vector3 initialDevicesPosition;

	// Token: 0x04000D74 RID: 3444
	public static bool isControllingCamera;

	// Token: 0x04000D75 RID: 3445
	public CanvasGroup overExposure;

	// Token: 0x04000D76 RID: 3446
	public CanvasGroup interference;

	// Token: 0x04000D77 RID: 3447
	public float interferenceFadeDuration = 0.2f;

	// Token: 0x04000D78 RID: 3448
	public Text timeText;

	// Token: 0x04000D79 RID: 3449
	public Text watchedDurationText;

	// Token: 0x04000D7A RID: 3450
	public Text deviceNameText;

	// Token: 0x04000D7B RID: 3451
	public Text noSignalText;

	// Token: 0x04000D7C RID: 3452
	public SoundDefinition bookmarkPressedSoundDef;

	// Token: 0x04000D7D RID: 3453
	public GameObject[] hideIfStatic;
}
