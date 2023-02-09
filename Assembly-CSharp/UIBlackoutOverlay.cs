using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000895 RID: 2197
public class UIBlackoutOverlay : MonoBehaviour
{
	// Token: 0x040030A7 RID: 12455
	public CanvasGroup group;

	// Token: 0x040030A8 RID: 12456
	public static Dictionary<UIBlackoutOverlay.blackoutType, UIBlackoutOverlay> instances;

	// Token: 0x040030A9 RID: 12457
	public UIBlackoutOverlay.blackoutType overlayType = UIBlackoutOverlay.blackoutType.NONE;

	// Token: 0x02000E4B RID: 3659
	public enum blackoutType
	{
		// Token: 0x040049E3 RID: 18915
		FULLBLACK,
		// Token: 0x040049E4 RID: 18916
		BINOCULAR,
		// Token: 0x040049E5 RID: 18917
		SCOPE,
		// Token: 0x040049E6 RID: 18918
		HELMETSLIT,
		// Token: 0x040049E7 RID: 18919
		SNORKELGOGGLE,
		// Token: 0x040049E8 RID: 18920
		NVG,
		// Token: 0x040049E9 RID: 18921
		FULLWHITE,
		// Token: 0x040049EA RID: 18922
		SUNGLASSES,
		// Token: 0x040049EB RID: 18923
		NONE = 64
	}
}
