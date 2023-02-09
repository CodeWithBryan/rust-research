using System;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x020009F6 RID: 2550
	internal static class SetPropertyUtility
	{
		// Token: 0x06003C8E RID: 15502 RVA: 0x001623E4 File Offset: 0x001605E4
		public static bool SetColor(ref Color currentValue, Color newValue)
		{
			if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		// Token: 0x06003C8F RID: 15503 RVA: 0x00162433 File Offset: 0x00160633
		public static bool SetEquatableStruct<T>(ref T currentValue, T newValue) where T : IEquatable<T>
		{
			if (currentValue.Equals(newValue))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		// Token: 0x06003C90 RID: 15504 RVA: 0x0016244E File Offset: 0x0016064E
		public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
		{
			if (currentValue.Equals(newValue))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		// Token: 0x06003C91 RID: 15505 RVA: 0x00162470 File Offset: 0x00160670
		public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
		{
			if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}
	}
}
