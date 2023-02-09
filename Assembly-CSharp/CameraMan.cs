using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000287 RID: 647
public class CameraMan : SingletonComponent<CameraMan>
{
	// Token: 0x0400152C RID: 5420
	public static string DefaultSaveName = string.Empty;

	// Token: 0x0400152D RID: 5421
	public const string SavePositionExtension = ".cam";

	// Token: 0x0400152E RID: 5422
	public const string SavePositionDirectory = "camsaves";

	// Token: 0x0400152F RID: 5423
	public bool OnlyControlWhenCursorHidden = true;

	// Token: 0x04001530 RID: 5424
	public bool NeedBothMouseButtonsToZoom;

	// Token: 0x04001531 RID: 5425
	public float LookSensitivity = 1f;

	// Token: 0x04001532 RID: 5426
	public float MoveSpeed = 1f;

	// Token: 0x04001533 RID: 5427
	public static float GuideAspect = 4f;

	// Token: 0x04001534 RID: 5428
	public static float GuideRatio = 3f;

	// Token: 0x04001535 RID: 5429
	public Canvas canvas;

	// Token: 0x04001536 RID: 5430
	public Graphic[] guides;
}
