using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000882 RID: 2178
public class RepairCostIndicatorRow : MonoBehaviour
{
	// Token: 0x04003043 RID: 12355
	public RustText ItemName;

	// Token: 0x04003044 RID: 12356
	public Image ItemSprite;

	// Token: 0x04003045 RID: 12357
	public RustText Amount;

	// Token: 0x04003046 RID: 12358
	public RectTransform FillRect;

	// Token: 0x04003047 RID: 12359
	public Image BackgroundImage;

	// Token: 0x04003048 RID: 12360
	public Color OkColour;

	// Token: 0x04003049 RID: 12361
	public Color MissingColour;
}
