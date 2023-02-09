using System;
using UnityEngine;

// Token: 0x0200014C RID: 332
public class DiveSite : JunkPile
{
	// Token: 0x0600162C RID: 5676 RVA: 0x000A9015 File Offset: 0x000A7215
	public override float TimeoutPlayerCheckRadius()
	{
		return 40f;
	}

	// Token: 0x04000F15 RID: 3861
	public Transform bobber;
}
