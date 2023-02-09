using System;
using Ionic.Zlib;

namespace Facepunch.Utility
{
	// Token: 0x02000AB8 RID: 2744
	public class Compression
	{
		// Token: 0x06004271 RID: 17009 RVA: 0x001841B0 File Offset: 0x001823B0
		public static byte[] Compress(byte[] data)
		{
			byte[] result;
			try
			{
				result = GZipStream.CompressBuffer(data);
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06004272 RID: 17010 RVA: 0x001841DC File Offset: 0x001823DC
		public static byte[] Uncompress(byte[] data)
		{
			return GZipStream.UncompressBuffer(data);
		}
	}
}
