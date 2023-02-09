using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020002F5 RID: 757
public static class SystemInfoEx
{
	// Token: 0x06001D80 RID: 7552
	[DllImport("RustNative")]
	private static extern ulong System_GetMemoryUsage();

	// Token: 0x17000233 RID: 563
	// (get) Token: 0x06001D81 RID: 7553 RVA: 0x000C9A33 File Offset: 0x000C7C33
	public static int systemMemoryUsed
	{
		get
		{
			return (int)(SystemInfoEx.System_GetMemoryUsage() / 1024UL / 1024UL);
		}
	}

	// Token: 0x06001D82 RID: 7554 RVA: 0x000C9A4C File Offset: 0x000C7C4C
	public static bool SupportsRenderTextureFormat(RenderTextureFormat format)
	{
		if (SystemInfoEx.supportedRenderTextureFormats == null)
		{
			Array values = Enum.GetValues(typeof(RenderTextureFormat));
			int num = (int)values.GetValue(values.Length - 1);
			SystemInfoEx.supportedRenderTextureFormats = new bool[num + 1];
			for (int i = 0; i <= num; i++)
			{
				bool flag = Enum.IsDefined(typeof(RenderTextureFormat), i);
				SystemInfoEx.supportedRenderTextureFormats[i] = (flag && SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat)i));
			}
		}
		return SystemInfoEx.supportedRenderTextureFormats[(int)format];
	}

	// Token: 0x040016DE RID: 5854
	private static bool[] supportedRenderTextureFormats;
}
