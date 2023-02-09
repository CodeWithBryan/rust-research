using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020000FE RID: 254
public class RCBookmarkEntry : MonoBehaviour
{
	// Token: 0x170001AD RID: 429
	// (get) Token: 0x060014D4 RID: 5332 RVA: 0x000A4378 File Offset: 0x000A2578
	// (set) Token: 0x060014D5 RID: 5333 RVA: 0x000A4380 File Offset: 0x000A2580
	public string identifier { get; private set; }

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x060014D6 RID: 5334 RVA: 0x000A4389 File Offset: 0x000A2589
	// (set) Token: 0x060014D7 RID: 5335 RVA: 0x000A4391 File Offset: 0x000A2591
	public uint netid { get; private set; }

	// Token: 0x170001AF RID: 431
	// (get) Token: 0x060014D8 RID: 5336 RVA: 0x000A439A File Offset: 0x000A259A
	// (set) Token: 0x060014D9 RID: 5337 RVA: 0x000A43A2 File Offset: 0x000A25A2
	public bool isControlling { get; private set; }

	// Token: 0x04000D62 RID: 3426
	private ComputerMenu owner;

	// Token: 0x04000D63 RID: 3427
	public RectTransform connectButton;

	// Token: 0x04000D64 RID: 3428
	public RectTransform disconnectButton;

	// Token: 0x04000D65 RID: 3429
	public RawImage onlineIndicator;

	// Token: 0x04000D66 RID: 3430
	public RawImage offlineIndicator;

	// Token: 0x04000D67 RID: 3431
	public GameObject selectedindicator;

	// Token: 0x04000D68 RID: 3432
	public Image backgroundImage;

	// Token: 0x04000D69 RID: 3433
	public Color activeColor;

	// Token: 0x04000D6A RID: 3434
	public Color inactiveColor;

	// Token: 0x04000D6B RID: 3435
	public Text nameLabel;

	// Token: 0x04000D6D RID: 3437
	public EventTrigger eventTrigger;
}
