using System;
using System.Globalization;

namespace TinyJSON
{
	// Token: 0x020009A3 RID: 2467
	public sealed class ProxyNumber : Variant
	{
		// Token: 0x06003A55 RID: 14933 RVA: 0x00157D58 File Offset: 0x00155F58
		public ProxyNumber(IConvertible value)
		{
			string text = value as string;
			this.value = ((text != null) ? ProxyNumber.Parse(text) : value);
		}

		// Token: 0x06003A56 RID: 14934 RVA: 0x00157D84 File Offset: 0x00155F84
		private static IConvertible Parse(string value)
		{
			if (value.IndexOfAny(ProxyNumber.floatingPointCharacters) == -1)
			{
				ulong num2;
				if (value[0] == '-')
				{
					long num;
					if (long.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out num))
					{
						return num;
					}
				}
				else if (ulong.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out num2))
				{
					return num2;
				}
			}
			decimal num3;
			if (decimal.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out num3))
			{
				double num4;
				if (num3 == 0m && double.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out num4) && Math.Abs(num4) > 5E-324)
				{
					return num4;
				}
				return num3;
			}
			else
			{
				double num5;
				if (double.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out num5))
				{
					return num5;
				}
				return 0;
			}
		}

		// Token: 0x06003A57 RID: 14935 RVA: 0x00157E57 File Offset: 0x00156057
		public override bool ToBoolean(IFormatProvider provider)
		{
			return this.value.ToBoolean(provider);
		}

		// Token: 0x06003A58 RID: 14936 RVA: 0x00157E65 File Offset: 0x00156065
		public override byte ToByte(IFormatProvider provider)
		{
			return this.value.ToByte(provider);
		}

		// Token: 0x06003A59 RID: 14937 RVA: 0x00157E73 File Offset: 0x00156073
		public override char ToChar(IFormatProvider provider)
		{
			return this.value.ToChar(provider);
		}

		// Token: 0x06003A5A RID: 14938 RVA: 0x00157E81 File Offset: 0x00156081
		public override decimal ToDecimal(IFormatProvider provider)
		{
			return this.value.ToDecimal(provider);
		}

		// Token: 0x06003A5B RID: 14939 RVA: 0x00157E8F File Offset: 0x0015608F
		public override double ToDouble(IFormatProvider provider)
		{
			return this.value.ToDouble(provider);
		}

		// Token: 0x06003A5C RID: 14940 RVA: 0x00157E9D File Offset: 0x0015609D
		public override short ToInt16(IFormatProvider provider)
		{
			return this.value.ToInt16(provider);
		}

		// Token: 0x06003A5D RID: 14941 RVA: 0x00157EAB File Offset: 0x001560AB
		public override int ToInt32(IFormatProvider provider)
		{
			return this.value.ToInt32(provider);
		}

		// Token: 0x06003A5E RID: 14942 RVA: 0x00157EB9 File Offset: 0x001560B9
		public override long ToInt64(IFormatProvider provider)
		{
			return this.value.ToInt64(provider);
		}

		// Token: 0x06003A5F RID: 14943 RVA: 0x00157EC7 File Offset: 0x001560C7
		public override sbyte ToSByte(IFormatProvider provider)
		{
			return this.value.ToSByte(provider);
		}

		// Token: 0x06003A60 RID: 14944 RVA: 0x00157ED5 File Offset: 0x001560D5
		public override float ToSingle(IFormatProvider provider)
		{
			return this.value.ToSingle(provider);
		}

		// Token: 0x06003A61 RID: 14945 RVA: 0x00157EE3 File Offset: 0x001560E3
		public override string ToString(IFormatProvider provider)
		{
			return this.value.ToString(provider);
		}

		// Token: 0x06003A62 RID: 14946 RVA: 0x00157EF1 File Offset: 0x001560F1
		public override ushort ToUInt16(IFormatProvider provider)
		{
			return this.value.ToUInt16(provider);
		}

		// Token: 0x06003A63 RID: 14947 RVA: 0x00157EFF File Offset: 0x001560FF
		public override uint ToUInt32(IFormatProvider provider)
		{
			return this.value.ToUInt32(provider);
		}

		// Token: 0x06003A64 RID: 14948 RVA: 0x00157F0D File Offset: 0x0015610D
		public override ulong ToUInt64(IFormatProvider provider)
		{
			return this.value.ToUInt64(provider);
		}

		// Token: 0x040034CE RID: 13518
		private static readonly char[] floatingPointCharacters = new char[]
		{
			'.',
			'e'
		};

		// Token: 0x040034CF RID: 13519
		private readonly IConvertible value;
	}
}
