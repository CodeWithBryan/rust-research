using System;
using Network;

// Token: 0x020005F5 RID: 1525
public abstract class NetworkCryptography : INetworkCryptography
{
	// Token: 0x06002C94 RID: 11412 RVA: 0x0010AE60 File Offset: 0x00109060
	public unsafe ArraySegment<byte> EncryptCopy(Connection connection, ArraySegment<byte> data)
	{
		ArraySegment<byte> src = new ArraySegment<byte>(data.Array, data.Offset, data.Count);
		ArraySegment<byte> result = new ArraySegment<byte>(this.buffer, data.Offset, this.buffer.Length - data.Offset);
		if (data.Offset > 0)
		{
			byte[] array;
			byte* destination;
			if ((array = result.Array) == null || array.Length == 0)
			{
				destination = null;
			}
			else
			{
				destination = &array[0];
			}
			byte[] array2;
			byte* source;
			if ((array2 = data.Array) == null || array2.Length == 0)
			{
				source = null;
			}
			else
			{
				source = &array2[0];
			}
			Buffer.MemoryCopy((void*)source, (void*)destination, (long)result.Array.Length, (long)data.Offset);
			array2 = null;
			array = null;
		}
		this.EncryptionHandler(connection, src, ref result);
		return result;
	}

	// Token: 0x06002C95 RID: 11413 RVA: 0x0010AF24 File Offset: 0x00109124
	public unsafe ArraySegment<byte> DecryptCopy(Connection connection, ArraySegment<byte> data)
	{
		ArraySegment<byte> src = new ArraySegment<byte>(data.Array, data.Offset, data.Count);
		ArraySegment<byte> result = new ArraySegment<byte>(this.buffer, data.Offset, this.buffer.Length - data.Offset);
		if (data.Offset > 0)
		{
			byte[] array;
			byte* destination;
			if ((array = result.Array) == null || array.Length == 0)
			{
				destination = null;
			}
			else
			{
				destination = &array[0];
			}
			byte[] array2;
			byte* source;
			if ((array2 = data.Array) == null || array2.Length == 0)
			{
				source = null;
			}
			else
			{
				source = &array2[0];
			}
			Buffer.MemoryCopy((void*)source, (void*)destination, (long)result.Array.Length, (long)data.Offset);
			array2 = null;
			array = null;
		}
		this.DecryptionHandler(connection, src, ref result);
		return result;
	}

	// Token: 0x06002C96 RID: 11414 RVA: 0x0010AFE8 File Offset: 0x001091E8
	public void Encrypt(Connection connection, ref ArraySegment<byte> data)
	{
		ArraySegment<byte> src = new ArraySegment<byte>(data.Array, data.Offset, data.Count);
		ArraySegment<byte> arraySegment = new ArraySegment<byte>(data.Array, data.Offset, data.Array.Length - data.Offset);
		this.EncryptionHandler(connection, src, ref arraySegment);
		data = arraySegment;
	}

	// Token: 0x06002C97 RID: 11415 RVA: 0x0010B044 File Offset: 0x00109244
	public void Decrypt(Connection connection, ref ArraySegment<byte> data)
	{
		ArraySegment<byte> src = new ArraySegment<byte>(data.Array, data.Offset, data.Count);
		ArraySegment<byte> arraySegment = new ArraySegment<byte>(data.Array, data.Offset, data.Array.Length - data.Offset);
		this.DecryptionHandler(connection, src, ref arraySegment);
		data = arraySegment;
	}

	// Token: 0x06002C98 RID: 11416
	protected abstract void EncryptionHandler(Connection connection, ArraySegment<byte> src, ref ArraySegment<byte> dst);

	// Token: 0x06002C99 RID: 11417
	protected abstract void DecryptionHandler(Connection connection, ArraySegment<byte> src, ref ArraySegment<byte> dst);

	// Token: 0x04002469 RID: 9321
	private byte[] buffer = new byte[8388608];
}
