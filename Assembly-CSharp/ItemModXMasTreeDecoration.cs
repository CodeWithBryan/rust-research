using System;

// Token: 0x02000181 RID: 385
public class ItemModXMasTreeDecoration : ItemMod
{
	// Token: 0x04000FE6 RID: 4070
	public ItemModXMasTreeDecoration.xmasFlags flagsToChange;

	// Token: 0x02000BE1 RID: 3041
	public enum xmasFlags
	{
		// Token: 0x0400400B RID: 16395
		pineCones = 128,
		// Token: 0x0400400C RID: 16396
		candyCanes = 256,
		// Token: 0x0400400D RID: 16397
		gingerbreadMen = 512,
		// Token: 0x0400400E RID: 16398
		Tinsel = 1024,
		// Token: 0x0400400F RID: 16399
		Balls = 2048,
		// Token: 0x04004010 RID: 16400
		Star = 16384,
		// Token: 0x04004011 RID: 16401
		Lights = 32768
	}
}
