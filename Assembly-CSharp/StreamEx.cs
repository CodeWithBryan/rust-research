using System;
using System.IO;

// Token: 0x020008F5 RID: 2293
public static class StreamEx
{
	// Token: 0x060036AC RID: 13996 RVA: 0x001453F4 File Offset: 0x001435F4
	public static void WriteToOtherStream(this Stream self, Stream target)
	{
		int count;
		while ((count = self.Read(StreamEx.StaticBuffer, 0, StreamEx.StaticBuffer.Length)) > 0)
		{
			target.Write(StreamEx.StaticBuffer, 0, count);
		}
	}

	// Token: 0x04003190 RID: 12688
	private static readonly byte[] StaticBuffer = new byte[16384];
}
