using System;

// Token: 0x020006ED RID: 1773
[Serializable]
public struct ExtendGBufferParams
{
	// Token: 0x0400281E RID: 10270
	public bool enabled;

	// Token: 0x0400281F RID: 10271
	public static ExtendGBufferParams Default = new ExtendGBufferParams
	{
		enabled = false
	};
}
