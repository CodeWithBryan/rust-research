using System;
using Network;

// Token: 0x020005F6 RID: 1526
public class NetworkCryptographyServer : NetworkCryptography
{
	// Token: 0x06002C9B RID: 11419 RVA: 0x0010B0B5 File Offset: 0x001092B5
	protected override void EncryptionHandler(Connection connection, ArraySegment<byte> src, ref ArraySegment<byte> dst)
	{
		if (connection.encryptionLevel > 1U)
		{
			EACServer.Encrypt(connection, src, ref dst);
			return;
		}
		Craptography.XOR(2370U, src, ref dst);
	}

	// Token: 0x06002C9C RID: 11420 RVA: 0x0010B0D5 File Offset: 0x001092D5
	protected override void DecryptionHandler(Connection connection, ArraySegment<byte> src, ref ArraySegment<byte> dst)
	{
		if (connection.encryptionLevel > 1U)
		{
			EACServer.Decrypt(connection, src, ref dst);
			return;
		}
		Craptography.XOR(2370U, src, ref dst);
	}
}
