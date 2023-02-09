using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200088A RID: 2186
public class TeamMemberElement : MonoBehaviour
{
	// Token: 0x04003083 RID: 12419
	public RustText nameText;

	// Token: 0x04003084 RID: 12420
	public RawImage icon;

	// Token: 0x04003085 RID: 12421
	public Color onlineColor;

	// Token: 0x04003086 RID: 12422
	public Color offlineColor;

	// Token: 0x04003087 RID: 12423
	public Color deadColor;

	// Token: 0x04003088 RID: 12424
	public Color woundedColor;

	// Token: 0x04003089 RID: 12425
	public GameObject hoverOverlay;

	// Token: 0x0400308A RID: 12426
	public RawImage memberIcon;

	// Token: 0x0400308B RID: 12427
	public RawImage leaderIcon;

	// Token: 0x0400308C RID: 12428
	public RawImage deadIcon;

	// Token: 0x0400308D RID: 12429
	public RawImage woundedIcon;

	// Token: 0x0400308E RID: 12430
	public int teamIndex;
}
