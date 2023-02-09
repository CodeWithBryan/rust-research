using System;
using System.Collections.Generic;

// Token: 0x02000737 RID: 1847
public static class StringFormatCache
{
	// Token: 0x06003300 RID: 13056 RVA: 0x0013AE00 File Offset: 0x00139000
	public static string Get(string format, string value1)
	{
		StringFormatCache.Key1 key = new StringFormatCache.Key1(format, value1);
		string text;
		if (!StringFormatCache.dict1.TryGetValue(key, out text))
		{
			text = string.Format(format, value1);
			StringFormatCache.dict1.Add(key, text);
		}
		return text;
	}

	// Token: 0x06003301 RID: 13057 RVA: 0x0013AE3C File Offset: 0x0013903C
	public static string Get(string format, string value1, string value2)
	{
		StringFormatCache.Key2 key = new StringFormatCache.Key2(format, value1, value2);
		string text;
		if (!StringFormatCache.dict2.TryGetValue(key, out text))
		{
			text = string.Format(format, value1, value2);
			StringFormatCache.dict2.Add(key, text);
		}
		return text;
	}

	// Token: 0x06003302 RID: 13058 RVA: 0x0013AE78 File Offset: 0x00139078
	public static string Get(string format, string value1, string value2, string value3)
	{
		StringFormatCache.Key3 key = new StringFormatCache.Key3(format, value1, value2, value3);
		string text;
		if (!StringFormatCache.dict3.TryGetValue(key, out text))
		{
			text = string.Format(format, value1, value2, value3);
			StringFormatCache.dict3.Add(key, text);
		}
		return text;
	}

	// Token: 0x06003303 RID: 13059 RVA: 0x0013AEB8 File Offset: 0x001390B8
	public static string Get(string format, string value1, string value2, string value3, string value4)
	{
		StringFormatCache.Key4 key = new StringFormatCache.Key4(format, value1, value2, value3, value4);
		string text;
		if (!StringFormatCache.dict4.TryGetValue(key, out text))
		{
			text = string.Format(format, new object[]
			{
				value1,
				value2,
				value3,
				value4
			});
			StringFormatCache.dict4.Add(key, text);
		}
		return text;
	}

	// Token: 0x04002981 RID: 10625
	private static Dictionary<StringFormatCache.Key1, string> dict1 = new Dictionary<StringFormatCache.Key1, string>();

	// Token: 0x04002982 RID: 10626
	private static Dictionary<StringFormatCache.Key2, string> dict2 = new Dictionary<StringFormatCache.Key2, string>();

	// Token: 0x04002983 RID: 10627
	private static Dictionary<StringFormatCache.Key3, string> dict3 = new Dictionary<StringFormatCache.Key3, string>();

	// Token: 0x04002984 RID: 10628
	private static Dictionary<StringFormatCache.Key4, string> dict4 = new Dictionary<StringFormatCache.Key4, string>();

	// Token: 0x02000E16 RID: 3606
	private struct Key1 : IEquatable<StringFormatCache.Key1>
	{
		// Token: 0x06004FD8 RID: 20440 RVA: 0x001A074F File Offset: 0x0019E94F
		public Key1(string format, string value1)
		{
			this.format = format;
			this.value1 = value1;
		}

		// Token: 0x06004FD9 RID: 20441 RVA: 0x001A075F File Offset: 0x0019E95F
		public override int GetHashCode()
		{
			return this.format.GetHashCode() ^ this.value1.GetHashCode();
		}

		// Token: 0x06004FDA RID: 20442 RVA: 0x001A0778 File Offset: 0x0019E978
		public override bool Equals(object other)
		{
			return other is StringFormatCache.Key1 && this.Equals((StringFormatCache.Key1)other);
		}

		// Token: 0x06004FDB RID: 20443 RVA: 0x001A0790 File Offset: 0x0019E990
		public bool Equals(StringFormatCache.Key1 other)
		{
			return this.format == other.format && this.value1 == other.value1;
		}

		// Token: 0x04004931 RID: 18737
		public string format;

		// Token: 0x04004932 RID: 18738
		public string value1;
	}

	// Token: 0x02000E17 RID: 3607
	private struct Key2 : IEquatable<StringFormatCache.Key2>
	{
		// Token: 0x06004FDC RID: 20444 RVA: 0x001A07B8 File Offset: 0x0019E9B8
		public Key2(string format, string value1, string value2)
		{
			this.format = format;
			this.value1 = value1;
			this.value2 = value2;
		}

		// Token: 0x06004FDD RID: 20445 RVA: 0x001A07CF File Offset: 0x0019E9CF
		public override int GetHashCode()
		{
			return this.format.GetHashCode() ^ this.value1.GetHashCode() ^ this.value2.GetHashCode();
		}

		// Token: 0x06004FDE RID: 20446 RVA: 0x001A07F4 File Offset: 0x0019E9F4
		public override bool Equals(object other)
		{
			return other is StringFormatCache.Key2 && this.Equals((StringFormatCache.Key2)other);
		}

		// Token: 0x06004FDF RID: 20447 RVA: 0x001A080C File Offset: 0x0019EA0C
		public bool Equals(StringFormatCache.Key2 other)
		{
			return this.format == other.format && this.value1 == other.value1 && this.value2 == other.value2;
		}

		// Token: 0x04004933 RID: 18739
		public string format;

		// Token: 0x04004934 RID: 18740
		public string value1;

		// Token: 0x04004935 RID: 18741
		public string value2;
	}

	// Token: 0x02000E18 RID: 3608
	private struct Key3 : IEquatable<StringFormatCache.Key3>
	{
		// Token: 0x06004FE0 RID: 20448 RVA: 0x001A0847 File Offset: 0x0019EA47
		public Key3(string format, string value1, string value2, string value3)
		{
			this.format = format;
			this.value1 = value1;
			this.value2 = value2;
			this.value3 = value3;
		}

		// Token: 0x06004FE1 RID: 20449 RVA: 0x001A0866 File Offset: 0x0019EA66
		public override int GetHashCode()
		{
			return this.format.GetHashCode() ^ this.value1.GetHashCode() ^ this.value2.GetHashCode() ^ this.value3.GetHashCode();
		}

		// Token: 0x06004FE2 RID: 20450 RVA: 0x001A0897 File Offset: 0x0019EA97
		public override bool Equals(object other)
		{
			return other is StringFormatCache.Key3 && this.Equals((StringFormatCache.Key3)other);
		}

		// Token: 0x06004FE3 RID: 20451 RVA: 0x001A08B0 File Offset: 0x0019EAB0
		public bool Equals(StringFormatCache.Key3 other)
		{
			return this.format == other.format && this.value1 == other.value1 && this.value2 == other.value2 && this.value3 == other.value3;
		}

		// Token: 0x04004936 RID: 18742
		public string format;

		// Token: 0x04004937 RID: 18743
		public string value1;

		// Token: 0x04004938 RID: 18744
		public string value2;

		// Token: 0x04004939 RID: 18745
		public string value3;
	}

	// Token: 0x02000E19 RID: 3609
	private struct Key4 : IEquatable<StringFormatCache.Key4>
	{
		// Token: 0x06004FE4 RID: 20452 RVA: 0x001A0909 File Offset: 0x0019EB09
		public Key4(string format, string value1, string value2, string value3, string value4)
		{
			this.format = format;
			this.value1 = value1;
			this.value2 = value2;
			this.value3 = value3;
			this.value4 = value4;
		}

		// Token: 0x06004FE5 RID: 20453 RVA: 0x001A0930 File Offset: 0x0019EB30
		public override int GetHashCode()
		{
			return this.format.GetHashCode() ^ this.value1.GetHashCode() ^ this.value2.GetHashCode() ^ this.value3.GetHashCode() ^ this.value4.GetHashCode();
		}

		// Token: 0x06004FE6 RID: 20454 RVA: 0x001A096D File Offset: 0x0019EB6D
		public override bool Equals(object other)
		{
			return other is StringFormatCache.Key4 && this.Equals((StringFormatCache.Key4)other);
		}

		// Token: 0x06004FE7 RID: 20455 RVA: 0x001A0988 File Offset: 0x0019EB88
		public bool Equals(StringFormatCache.Key4 other)
		{
			return this.format == other.format && this.value1 == other.value1 && this.value2 == other.value2 && this.value3 == other.value3 && this.value4 == other.value4;
		}

		// Token: 0x0400493A RID: 18746
		public string format;

		// Token: 0x0400493B RID: 18747
		public string value1;

		// Token: 0x0400493C RID: 18748
		public string value2;

		// Token: 0x0400493D RID: 18749
		public string value3;

		// Token: 0x0400493E RID: 18750
		public string value4;
	}
}
