using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000888 RID: 2184
public class SteamUserButton : MonoBehaviour
{
	// Token: 0x17000408 RID: 1032
	// (get) Token: 0x06003582 RID: 13698 RVA: 0x00141CCD File Offset: 0x0013FECD
	// (set) Token: 0x06003583 RID: 13699 RVA: 0x00141CD5 File Offset: 0x0013FED5
	public ulong SteamId { get; private set; }

	// Token: 0x17000409 RID: 1033
	// (get) Token: 0x06003584 RID: 13700 RVA: 0x00141CDE File Offset: 0x0013FEDE
	// (set) Token: 0x06003585 RID: 13701 RVA: 0x00141CE6 File Offset: 0x0013FEE6
	public string Username { get; private set; }

	// Token: 0x04003077 RID: 12407
	public RustText steamName;

	// Token: 0x04003078 RID: 12408
	public RustText steamInfo;

	// Token: 0x04003079 RID: 12409
	public RawImage avatar;

	// Token: 0x0400307A RID: 12410
	public Color textColorInGame;

	// Token: 0x0400307B RID: 12411
	public Color textColorOnline;

	// Token: 0x0400307C RID: 12412
	public Color textColorNormal;
}
