using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008B8 RID: 2232
public class VitalNoteOxygen : MonoBehaviour, IClientComponent, IVitalNotice
{
	// Token: 0x040030FA RID: 12538
	[SerializeField]
	private float refreshTime = 1f;

	// Token: 0x040030FB RID: 12539
	[SerializeField]
	private TextMeshProUGUI valueText;

	// Token: 0x040030FC RID: 12540
	[SerializeField]
	private Animator animator;

	// Token: 0x040030FD RID: 12541
	[SerializeField]
	private Image airIcon;

	// Token: 0x040030FE RID: 12542
	[SerializeField]
	private RectTransform airIconTr;

	// Token: 0x040030FF RID: 12543
	[SerializeField]
	private Image backgroundImage;

	// Token: 0x04003100 RID: 12544
	[SerializeField]
	private Color baseColour;

	// Token: 0x04003101 RID: 12545
	[SerializeField]
	private Color badColour;

	// Token: 0x04003102 RID: 12546
	[SerializeField]
	private Image iconImage;

	// Token: 0x04003103 RID: 12547
	[SerializeField]
	private Color iconBaseColour;

	// Token: 0x04003104 RID: 12548
	[SerializeField]
	private Color iconBadColour;

	// Token: 0x04003105 RID: 12549
	protected bool show = true;
}
