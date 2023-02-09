using System;

namespace Network
{
	// Token: 0x02000005 RID: 5
	public interface INetworkCryptography
	{
		// Token: 0x06000004 RID: 4
		ArraySegment<byte> EncryptCopy(Connection connection, ArraySegment<byte> data);

		// Token: 0x06000005 RID: 5
		ArraySegment<byte> DecryptCopy(Connection connection, ArraySegment<byte> data);

		// Token: 0x06000006 RID: 6
		void Encrypt(Connection connection, ref ArraySegment<byte> data);

		// Token: 0x06000007 RID: 7
		void Decrypt(Connection connection, ref ArraySegment<byte> data);
	}
}
