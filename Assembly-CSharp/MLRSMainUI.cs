using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

// Token: 0x02000462 RID: 1122
public class MLRSMainUI : MonoBehaviour
{
	// Token: 0x04001D5B RID: 7515
	[SerializeField]
	private bool isFullscreen;

	// Token: 0x04001D5C RID: 7516
	[SerializeField]
	private GameObject noAimingModuleModeGO;

	// Token: 0x04001D5D RID: 7517
	[SerializeField]
	private GameObject activeModeGO;

	// Token: 0x04001D5E RID: 7518
	[SerializeField]
	private MLRSAmmoUI noAimingModuleAmmoUI;

	// Token: 0x04001D5F RID: 7519
	[SerializeField]
	private MLRSAmmoUI activeAmmoUI;

	// Token: 0x04001D60 RID: 7520
	[SerializeField]
	private MLRSVelocityUI velocityUI;

	// Token: 0x04001D61 RID: 7521
	[SerializeField]
	private RustText titleText;

	// Token: 0x04001D62 RID: 7522
	[SerializeField]
	private RustText usernameText;

	// Token: 0x04001D63 RID: 7523
	[SerializeField]
	private TokenisedPhrase readyStatus;

	// Token: 0x04001D64 RID: 7524
	[SerializeField]
	private TokenisedPhrase realigningStatus;

	// Token: 0x04001D65 RID: 7525
	[SerializeField]
	private TokenisedPhrase firingStatus;

	// Token: 0x04001D66 RID: 7526
	[SerializeField]
	private RustText statusText;

	// Token: 0x04001D67 RID: 7527
	[SerializeField]
	private MapView mapView;

	// Token: 0x04001D68 RID: 7528
	[SerializeField]
	private ScrollRectEx mapScrollRect;

	// Token: 0x04001D69 RID: 7529
	[SerializeField]
	private ScrollRectZoom mapScrollRectZoom;

	// Token: 0x04001D6A RID: 7530
	[SerializeField]
	private RectTransform mapBaseRect;

	// Token: 0x04001D6B RID: 7531
	[SerializeField]
	private RectTransform minRangeCircle;

	// Token: 0x04001D6C RID: 7532
	[SerializeField]
	private RectTransform targetAimRect;

	// Token: 0x04001D6D RID: 7533
	[SerializeField]
	private RectTransform trueAimRect;

	// Token: 0x04001D6E RID: 7534
	[SerializeField]
	private UILineRenderer connectingLine;

	// Token: 0x04001D6F RID: 7535
	[SerializeField]
	private GameObject noTargetCirclePrefab;

	// Token: 0x04001D70 RID: 7536
	[SerializeField]
	private Transform noTargetCircleParent;

	// Token: 0x04001D71 RID: 7537
	[SerializeField]
	private SoundDefinition changeTargetSoundDef;

	// Token: 0x04001D72 RID: 7538
	[SerializeField]
	private SoundDefinition readyToFireSoundDef;
}
