using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace TinyJSON
{
	// Token: 0x020009A4 RID: 2468
	public sealed class ProxyObject : Variant, IEnumerable<KeyValuePair<string, Variant>>, IEnumerable
	{
		// Token: 0x06003A66 RID: 14950 RVA: 0x00157F32 File Offset: 0x00156132
		public ProxyObject()
		{
			this.dict = new Dictionary<string, Variant>();
		}

		// Token: 0x06003A67 RID: 14951 RVA: 0x00157F45 File Offset: 0x00156145
		IEnumerator<KeyValuePair<string, Variant>> IEnumerable<KeyValuePair<string, Variant>>.GetEnumerator()
		{
			return this.dict.GetEnumerator();
		}

		// Token: 0x06003A68 RID: 14952 RVA: 0x00157F45 File Offset: 0x00156145
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.dict.GetEnumerator();
		}

		// Token: 0x06003A69 RID: 14953 RVA: 0x00157F57 File Offset: 0x00156157
		public void Add(string key, Variant item)
		{
			this.dict.Add(key, item);
		}

		// Token: 0x06003A6A RID: 14954 RVA: 0x00157F66 File Offset: 0x00156166
		public bool TryGetValue(string key, out Variant item)
		{
			return this.dict.TryGetValue(key, out item);
		}

		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x06003A6B RID: 14955 RVA: 0x00157F78 File Offset: 0x00156178
		public string TypeHint
		{
			get
			{
				Variant variant;
				if (this.TryGetValue("@type", out variant))
				{
					return variant.ToString(CultureInfo.InvariantCulture);
				}
				return null;
			}
		}

		// Token: 0x17000483 RID: 1155
		public override Variant this[string key]
		{
			get
			{
				return this.dict[key];
			}
			set
			{
				this.dict[key] = value;
			}
		}

		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x06003A6E RID: 14958 RVA: 0x00157FBE File Offset: 0x001561BE
		public int Count
		{
			get
			{
				return this.dict.Count;
			}
		}

		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x06003A6F RID: 14959 RVA: 0x00157FCB File Offset: 0x001561CB
		public Dictionary<string, Variant>.KeyCollection Keys
		{
			get
			{
				return this.dict.Keys;
			}
		}

		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x06003A70 RID: 14960 RVA: 0x00157FD8 File Offset: 0x001561D8
		public Dictionary<string, Variant>.ValueCollection Values
		{
			get
			{
				return this.dict.Values;
			}
		}

		// Token: 0x040034D0 RID: 13520
		public const string TypeHintKey = "@type";

		// Token: 0x040034D1 RID: 13521
		private readonly Dictionary<string, Variant> dict;
	}
}
