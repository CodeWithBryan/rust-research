using System;
using System.Globalization;

namespace TinyJSON
{
	// Token: 0x020009A6 RID: 2470
	public abstract class Variant : IConvertible
	{
		// Token: 0x06003A73 RID: 14963 RVA: 0x00157FFC File Offset: 0x001561FC
		public void Make<T>(out T item)
		{
			JSON.MakeInto<T>(this, out item);
		}

		// Token: 0x06003A74 RID: 14964 RVA: 0x00158008 File Offset: 0x00156208
		public T Make<T>()
		{
			T result;
			JSON.MakeInto<T>(this, out result);
			return result;
		}

		// Token: 0x06003A75 RID: 14965 RVA: 0x0015801E File Offset: 0x0015621E
		public string ToJSON()
		{
			return JSON.Dump(this);
		}

		// Token: 0x06003A76 RID: 14966 RVA: 0x00003A54 File Offset: 0x00001C54
		public virtual TypeCode GetTypeCode()
		{
			return TypeCode.Object;
		}

		// Token: 0x06003A77 RID: 14967 RVA: 0x00158026 File Offset: 0x00156226
		public virtual object ToType(Type conversionType, IFormatProvider provider)
		{
			throw new InvalidCastException(string.Concat(new object[]
			{
				"Cannot convert ",
				base.GetType(),
				" to ",
				conversionType.Name
			}));
		}

		// Token: 0x06003A78 RID: 14968 RVA: 0x0015805A File Offset: 0x0015625A
		public virtual DateTime ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to DateTime");
		}

		// Token: 0x06003A79 RID: 14969 RVA: 0x00158076 File Offset: 0x00156276
		public virtual bool ToBoolean(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Boolean");
		}

		// Token: 0x06003A7A RID: 14970 RVA: 0x00158092 File Offset: 0x00156292
		public virtual byte ToByte(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Byte");
		}

		// Token: 0x06003A7B RID: 14971 RVA: 0x001580AE File Offset: 0x001562AE
		public virtual char ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Char");
		}

		// Token: 0x06003A7C RID: 14972 RVA: 0x001580CA File Offset: 0x001562CA
		public virtual decimal ToDecimal(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Decimal");
		}

		// Token: 0x06003A7D RID: 14973 RVA: 0x001580E6 File Offset: 0x001562E6
		public virtual double ToDouble(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Double");
		}

		// Token: 0x06003A7E RID: 14974 RVA: 0x00158102 File Offset: 0x00156302
		public virtual short ToInt16(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Int16");
		}

		// Token: 0x06003A7F RID: 14975 RVA: 0x0015811E File Offset: 0x0015631E
		public virtual int ToInt32(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Int32");
		}

		// Token: 0x06003A80 RID: 14976 RVA: 0x0015813A File Offset: 0x0015633A
		public virtual long ToInt64(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Int64");
		}

		// Token: 0x06003A81 RID: 14977 RVA: 0x00158156 File Offset: 0x00156356
		public virtual sbyte ToSByte(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to SByte");
		}

		// Token: 0x06003A82 RID: 14978 RVA: 0x00158172 File Offset: 0x00156372
		public virtual float ToSingle(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Single");
		}

		// Token: 0x06003A83 RID: 14979 RVA: 0x0015818E File Offset: 0x0015638E
		public virtual string ToString(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to String");
		}

		// Token: 0x06003A84 RID: 14980 RVA: 0x001581AA File Offset: 0x001563AA
		public virtual ushort ToUInt16(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to UInt16");
		}

		// Token: 0x06003A85 RID: 14981 RVA: 0x001581C6 File Offset: 0x001563C6
		public virtual uint ToUInt32(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to UInt32");
		}

		// Token: 0x06003A86 RID: 14982 RVA: 0x001581E2 File Offset: 0x001563E2
		public virtual ulong ToUInt64(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to UInt64");
		}

		// Token: 0x06003A87 RID: 14983 RVA: 0x001581FE File Offset: 0x001563FE
		public override string ToString()
		{
			return this.ToString(Variant.FormatProvider);
		}

		// Token: 0x17000487 RID: 1159
		public virtual Variant this[string key]
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x17000488 RID: 1160
		public virtual Variant this[int index]
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x06003A8C RID: 14988 RVA: 0x00158212 File Offset: 0x00156412
		public static implicit operator bool(Variant variant)
		{
			return variant.ToBoolean(Variant.FormatProvider);
		}

		// Token: 0x06003A8D RID: 14989 RVA: 0x0015821F File Offset: 0x0015641F
		public static implicit operator float(Variant variant)
		{
			return variant.ToSingle(Variant.FormatProvider);
		}

		// Token: 0x06003A8E RID: 14990 RVA: 0x0015822C File Offset: 0x0015642C
		public static implicit operator double(Variant variant)
		{
			return variant.ToDouble(Variant.FormatProvider);
		}

		// Token: 0x06003A8F RID: 14991 RVA: 0x00158239 File Offset: 0x00156439
		public static implicit operator ushort(Variant variant)
		{
			return variant.ToUInt16(Variant.FormatProvider);
		}

		// Token: 0x06003A90 RID: 14992 RVA: 0x00158246 File Offset: 0x00156446
		public static implicit operator short(Variant variant)
		{
			return variant.ToInt16(Variant.FormatProvider);
		}

		// Token: 0x06003A91 RID: 14993 RVA: 0x00158253 File Offset: 0x00156453
		public static implicit operator uint(Variant variant)
		{
			return variant.ToUInt32(Variant.FormatProvider);
		}

		// Token: 0x06003A92 RID: 14994 RVA: 0x00158260 File Offset: 0x00156460
		public static implicit operator int(Variant variant)
		{
			return variant.ToInt32(Variant.FormatProvider);
		}

		// Token: 0x06003A93 RID: 14995 RVA: 0x0015826D File Offset: 0x0015646D
		public static implicit operator ulong(Variant variant)
		{
			return variant.ToUInt64(Variant.FormatProvider);
		}

		// Token: 0x06003A94 RID: 14996 RVA: 0x0015827A File Offset: 0x0015647A
		public static implicit operator long(Variant variant)
		{
			return variant.ToInt64(Variant.FormatProvider);
		}

		// Token: 0x06003A95 RID: 14997 RVA: 0x00158287 File Offset: 0x00156487
		public static implicit operator decimal(Variant variant)
		{
			return variant.ToDecimal(Variant.FormatProvider);
		}

		// Token: 0x06003A96 RID: 14998 RVA: 0x001581FE File Offset: 0x001563FE
		public static implicit operator string(Variant variant)
		{
			return variant.ToString(Variant.FormatProvider);
		}

		// Token: 0x06003A97 RID: 14999 RVA: 0x00158294 File Offset: 0x00156494
		public static implicit operator Guid(Variant variant)
		{
			return new Guid(variant.ToString(Variant.FormatProvider));
		}

		// Token: 0x040034D3 RID: 13523
		protected static readonly IFormatProvider FormatProvider = new NumberFormatInfo();
	}
}
