using System;

namespace TinyJSON
{
	// Token: 0x0200099F RID: 2463
	public sealed class DecodeException : Exception
	{
		// Token: 0x06003A3A RID: 14906 RVA: 0x0015716D File Offset: 0x0015536D
		public DecodeException(string message) : base(message)
		{
		}

		// Token: 0x06003A3B RID: 14907 RVA: 0x00157176 File Offset: 0x00155376
		public DecodeException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
