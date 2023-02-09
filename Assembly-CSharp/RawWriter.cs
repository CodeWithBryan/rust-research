using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x0200091A RID: 2330
public static class RawWriter
{
	// Token: 0x06003782 RID: 14210 RVA: 0x0014988C File Offset: 0x00147A8C
	public static void Write(IEnumerable<byte> data, string path)
	{
		using (FileStream fileStream = File.Open(path, FileMode.Create))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				foreach (byte value in data)
				{
					binaryWriter.Write(value);
				}
			}
		}
	}

	// Token: 0x06003783 RID: 14211 RVA: 0x00149910 File Offset: 0x00147B10
	public static void Write(IEnumerable<int> data, string path)
	{
		using (FileStream fileStream = File.Open(path, FileMode.Create))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				foreach (int value in data)
				{
					binaryWriter.Write(value);
				}
			}
		}
	}

	// Token: 0x06003784 RID: 14212 RVA: 0x00149994 File Offset: 0x00147B94
	public static void Write(IEnumerable<short> data, string path)
	{
		using (FileStream fileStream = File.Open(path, FileMode.Create))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				foreach (short value in data)
				{
					binaryWriter.Write(value);
				}
			}
		}
	}

	// Token: 0x06003785 RID: 14213 RVA: 0x00149A18 File Offset: 0x00147C18
	public static void Write(IEnumerable<float> data, string path)
	{
		using (FileStream fileStream = File.Open(path, FileMode.Create))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				foreach (float value in data)
				{
					binaryWriter.Write(value);
				}
			}
		}
	}
}
