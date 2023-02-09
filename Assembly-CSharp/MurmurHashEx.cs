using System;
using System.IO;
using System.Text;

// Token: 0x02000914 RID: 2324
public static class MurmurHashEx
{
	// Token: 0x06003746 RID: 14150 RVA: 0x001480A7 File Offset: 0x001462A7
	public static int MurmurHashSigned(this string str)
	{
		return MurmurHash.Signed(MurmurHashEx.StringToStream(str));
	}

	// Token: 0x06003747 RID: 14151 RVA: 0x001480B4 File Offset: 0x001462B4
	public static uint MurmurHashUnsigned(this string str)
	{
		return MurmurHash.Unsigned(MurmurHashEx.StringToStream(str));
	}

	// Token: 0x06003748 RID: 14152 RVA: 0x001480C1 File Offset: 0x001462C1
	private static MemoryStream StringToStream(string str)
	{
		return new MemoryStream(Encoding.UTF8.GetBytes(str ?? string.Empty));
	}
}
