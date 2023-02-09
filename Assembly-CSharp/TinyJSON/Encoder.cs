using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace TinyJSON
{
	// Token: 0x02000995 RID: 2453
	public sealed class Encoder
	{
		// Token: 0x06003A14 RID: 14868 RVA: 0x001565C6 File Offset: 0x001547C6
		private Encoder(EncodeOptions options)
		{
			this.options = options;
			this.builder = new StringBuilder();
			this.indent = 0;
		}

		// Token: 0x06003A15 RID: 14869 RVA: 0x001565E7 File Offset: 0x001547E7
		public static string Encode(object obj)
		{
			return Encoder.Encode(obj, EncodeOptions.None);
		}

		// Token: 0x06003A16 RID: 14870 RVA: 0x001565F0 File Offset: 0x001547F0
		public static string Encode(object obj, EncodeOptions options)
		{
			Encoder encoder = new Encoder(options);
			encoder.EncodeValue(obj, false);
			return encoder.builder.ToString();
		}

		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x06003A17 RID: 14871 RVA: 0x0015660A File Offset: 0x0015480A
		private bool PrettyPrintEnabled
		{
			get
			{
				return (this.options & EncodeOptions.PrettyPrint) == EncodeOptions.PrettyPrint;
			}
		}

		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x06003A18 RID: 14872 RVA: 0x00156617 File Offset: 0x00154817
		private bool TypeHintsEnabled
		{
			get
			{
				return (this.options & EncodeOptions.NoTypeHints) != EncodeOptions.NoTypeHints;
			}
		}

		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x06003A19 RID: 14873 RVA: 0x00156627 File Offset: 0x00154827
		private bool IncludePublicPropertiesEnabled
		{
			get
			{
				return (this.options & EncodeOptions.IncludePublicProperties) == EncodeOptions.IncludePublicProperties;
			}
		}

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x06003A1A RID: 14874 RVA: 0x00156634 File Offset: 0x00154834
		private bool EnforceHierarchyOrderEnabled
		{
			get
			{
				return (this.options & EncodeOptions.EnforceHierarchyOrder) == EncodeOptions.EnforceHierarchyOrder;
			}
		}

		// Token: 0x06003A1B RID: 14875 RVA: 0x00156644 File Offset: 0x00154844
		private void EncodeValue(object value, bool forceTypeHint)
		{
			if (value == null)
			{
				this.builder.Append("null");
				return;
			}
			if (value is string)
			{
				this.EncodeString((string)value);
				return;
			}
			if (value is ProxyString)
			{
				this.EncodeString(((ProxyString)value).ToString(CultureInfo.InvariantCulture));
				return;
			}
			if (value is char)
			{
				this.EncodeString(value.ToString());
				return;
			}
			if (value is bool)
			{
				this.builder.Append(((bool)value) ? "true" : "false");
				return;
			}
			if (value is Enum)
			{
				this.EncodeString(value.ToString());
				return;
			}
			if (value is Array)
			{
				this.EncodeArray((Array)value, forceTypeHint);
				return;
			}
			if (value is IList)
			{
				this.EncodeList((IList)value, forceTypeHint);
				return;
			}
			if (value is IDictionary)
			{
				this.EncodeDictionary((IDictionary)value, forceTypeHint);
				return;
			}
			if (value is Guid)
			{
				this.EncodeString(value.ToString());
				return;
			}
			if (value is ProxyArray)
			{
				this.EncodeProxyArray((ProxyArray)value);
				return;
			}
			if (value is ProxyObject)
			{
				this.EncodeProxyObject((ProxyObject)value);
				return;
			}
			if (value is float || value is double || value is int || value is uint || value is long || value is sbyte || value is byte || value is short || value is ushort || value is ulong || value is decimal || value is ProxyBoolean || value is ProxyNumber)
			{
				this.builder.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
				return;
			}
			this.EncodeObject(value, forceTypeHint);
		}

		// Token: 0x06003A1C RID: 14876 RVA: 0x001567F8 File Offset: 0x001549F8
		private IEnumerable<FieldInfo> GetFieldsForType(Type type)
		{
			if (this.EnforceHierarchyOrderEnabled)
			{
				Stack<Type> stack = new Stack<Type>();
				while (type != null)
				{
					stack.Push(type);
					type = type.BaseType;
				}
				List<FieldInfo> list = new List<FieldInfo>();
				while (stack.Count > 0)
				{
					list.AddRange(stack.Pop().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
				}
				return list;
			}
			return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		// Token: 0x06003A1D RID: 14877 RVA: 0x0015685C File Offset: 0x00154A5C
		private IEnumerable<PropertyInfo> GetPropertiesForType(Type type)
		{
			if (this.EnforceHierarchyOrderEnabled)
			{
				Stack<Type> stack = new Stack<Type>();
				while (type != null)
				{
					stack.Push(type);
					type = type.BaseType;
				}
				List<PropertyInfo> list = new List<PropertyInfo>();
				while (stack.Count > 0)
				{
					list.AddRange(stack.Pop().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
				}
				return list;
			}
			return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		// Token: 0x06003A1E RID: 14878 RVA: 0x001568C0 File Offset: 0x00154AC0
		private void EncodeObject(object value, bool forceTypeHint)
		{
			Type type = value.GetType();
			this.AppendOpenBrace();
			forceTypeHint = (forceTypeHint || this.TypeHintsEnabled);
			bool includePublicPropertiesEnabled = this.IncludePublicPropertiesEnabled;
			bool firstItem = !forceTypeHint;
			if (forceTypeHint)
			{
				if (this.PrettyPrintEnabled)
				{
					this.AppendIndent();
				}
				this.EncodeString("@type");
				this.AppendColon();
				this.EncodeString(type.FullName);
				firstItem = false;
			}
			foreach (FieldInfo fieldInfo in this.GetFieldsForType(type))
			{
				bool forceTypeHint2 = false;
				bool flag = fieldInfo.IsPublic;
				foreach (object o in fieldInfo.GetCustomAttributes(true))
				{
					if (Encoder.excludeAttrType.IsInstanceOfType(o))
					{
						flag = false;
					}
					if (Encoder.includeAttrType.IsInstanceOfType(o))
					{
						flag = true;
					}
					if (Encoder.typeHintAttrType.IsInstanceOfType(o))
					{
						forceTypeHint2 = true;
					}
				}
				if (flag)
				{
					this.AppendComma(firstItem);
					this.EncodeString(fieldInfo.Name);
					this.AppendColon();
					this.EncodeValue(fieldInfo.GetValue(value), forceTypeHint2);
					firstItem = false;
				}
			}
			foreach (PropertyInfo propertyInfo in this.GetPropertiesForType(type))
			{
				if (propertyInfo.CanRead)
				{
					bool forceTypeHint3 = false;
					bool flag2 = includePublicPropertiesEnabled;
					foreach (object o2 in propertyInfo.GetCustomAttributes(true))
					{
						if (Encoder.excludeAttrType.IsInstanceOfType(o2))
						{
							flag2 = false;
						}
						if (Encoder.includeAttrType.IsInstanceOfType(o2))
						{
							flag2 = true;
						}
						if (Encoder.typeHintAttrType.IsInstanceOfType(o2))
						{
							forceTypeHint3 = true;
						}
					}
					if (flag2)
					{
						this.AppendComma(firstItem);
						this.EncodeString(propertyInfo.Name);
						this.AppendColon();
						this.EncodeValue(propertyInfo.GetValue(value, null), forceTypeHint3);
						firstItem = false;
					}
				}
			}
			this.AppendCloseBrace();
		}

		// Token: 0x06003A1F RID: 14879 RVA: 0x00156ADC File Offset: 0x00154CDC
		private void EncodeProxyArray(ProxyArray value)
		{
			if (value.Count == 0)
			{
				this.builder.Append("[]");
				return;
			}
			this.AppendOpenBracket();
			bool firstItem = true;
			foreach (Variant value2 in ((IEnumerable<Variant>)value))
			{
				this.AppendComma(firstItem);
				this.EncodeValue(value2, false);
				firstItem = false;
			}
			this.AppendCloseBracket();
		}

		// Token: 0x06003A20 RID: 14880 RVA: 0x00156B58 File Offset: 0x00154D58
		private void EncodeProxyObject(ProxyObject value)
		{
			if (value.Count == 0)
			{
				this.builder.Append("{}");
				return;
			}
			this.AppendOpenBrace();
			bool firstItem = true;
			foreach (string text in value.Keys)
			{
				this.AppendComma(firstItem);
				this.EncodeString(text);
				this.AppendColon();
				this.EncodeValue(value[text], false);
				firstItem = false;
			}
			this.AppendCloseBrace();
		}

		// Token: 0x06003A21 RID: 14881 RVA: 0x00156BF0 File Offset: 0x00154DF0
		private void EncodeDictionary(IDictionary value, bool forceTypeHint)
		{
			if (value.Count == 0)
			{
				this.builder.Append("{}");
				return;
			}
			this.AppendOpenBrace();
			bool firstItem = true;
			foreach (object obj in value.Keys)
			{
				this.AppendComma(firstItem);
				this.EncodeString(obj.ToString());
				this.AppendColon();
				this.EncodeValue(value[obj], forceTypeHint);
				firstItem = false;
			}
			this.AppendCloseBrace();
		}

		// Token: 0x06003A22 RID: 14882 RVA: 0x00156C90 File Offset: 0x00154E90
		private void EncodeList(IList value, bool forceTypeHint)
		{
			if (value.Count == 0)
			{
				this.builder.Append("[]");
				return;
			}
			this.AppendOpenBracket();
			bool firstItem = true;
			foreach (object value2 in value)
			{
				this.AppendComma(firstItem);
				this.EncodeValue(value2, forceTypeHint);
				firstItem = false;
			}
			this.AppendCloseBracket();
		}

		// Token: 0x06003A23 RID: 14883 RVA: 0x00156D14 File Offset: 0x00154F14
		private void EncodeArray(Array value, bool forceTypeHint)
		{
			if (value.Rank == 1)
			{
				this.EncodeList(value, forceTypeHint);
				return;
			}
			int[] indices = new int[value.Rank];
			this.EncodeArrayRank(value, 0, indices, forceTypeHint);
		}

		// Token: 0x06003A24 RID: 14884 RVA: 0x00156D4C File Offset: 0x00154F4C
		private void EncodeArrayRank(Array value, int rank, int[] indices, bool forceTypeHint)
		{
			this.AppendOpenBracket();
			int lowerBound = value.GetLowerBound(rank);
			int upperBound = value.GetUpperBound(rank);
			if (rank == value.Rank - 1)
			{
				for (int i = lowerBound; i <= upperBound; i++)
				{
					indices[rank] = i;
					this.AppendComma(i == lowerBound);
					this.EncodeValue(value.GetValue(indices), forceTypeHint);
				}
			}
			else
			{
				for (int j = lowerBound; j <= upperBound; j++)
				{
					indices[rank] = j;
					this.AppendComma(j == lowerBound);
					this.EncodeArrayRank(value, rank + 1, indices, forceTypeHint);
				}
			}
			this.AppendCloseBracket();
		}

		// Token: 0x06003A25 RID: 14885 RVA: 0x00156DD4 File Offset: 0x00154FD4
		private void EncodeString(string value)
		{
			this.builder.Append('"');
			char[] array = value.ToCharArray();
			int i = 0;
			while (i < array.Length)
			{
				char c = array[i];
				switch (c)
				{
				case '\b':
					this.builder.Append("\\b");
					break;
				case '\t':
					this.builder.Append("\\t");
					break;
				case '\n':
					this.builder.Append("\\n");
					break;
				case '\v':
					goto IL_DD;
				case '\f':
					this.builder.Append("\\f");
					break;
				case '\r':
					this.builder.Append("\\r");
					break;
				default:
					if (c != '"')
					{
						if (c != '\\')
						{
							goto IL_DD;
						}
						this.builder.Append("\\\\");
					}
					else
					{
						this.builder.Append("\\\"");
					}
					break;
				}
				IL_123:
				i++;
				continue;
				IL_DD:
				int num = Convert.ToInt32(c);
				if (num >= 32 && num <= 126)
				{
					this.builder.Append(c);
					goto IL_123;
				}
				this.builder.Append("\\u" + Convert.ToString(num, 16).PadLeft(4, '0'));
				goto IL_123;
			}
			this.builder.Append('"');
		}

		// Token: 0x06003A26 RID: 14886 RVA: 0x00156F20 File Offset: 0x00155120
		private void AppendIndent()
		{
			for (int i = 0; i < this.indent; i++)
			{
				this.builder.Append('\t');
			}
		}

		// Token: 0x06003A27 RID: 14887 RVA: 0x00156F4C File Offset: 0x0015514C
		private void AppendOpenBrace()
		{
			this.builder.Append('{');
			if (this.PrettyPrintEnabled)
			{
				this.builder.Append('\n');
				this.indent++;
			}
		}

		// Token: 0x06003A28 RID: 14888 RVA: 0x00156F80 File Offset: 0x00155180
		private void AppendCloseBrace()
		{
			if (this.PrettyPrintEnabled)
			{
				this.builder.Append('\n');
				this.indent--;
				this.AppendIndent();
			}
			this.builder.Append('}');
		}

		// Token: 0x06003A29 RID: 14889 RVA: 0x00156FBA File Offset: 0x001551BA
		private void AppendOpenBracket()
		{
			this.builder.Append('[');
			if (this.PrettyPrintEnabled)
			{
				this.builder.Append('\n');
				this.indent++;
			}
		}

		// Token: 0x06003A2A RID: 14890 RVA: 0x00156FEE File Offset: 0x001551EE
		private void AppendCloseBracket()
		{
			if (this.PrettyPrintEnabled)
			{
				this.builder.Append('\n');
				this.indent--;
				this.AppendIndent();
			}
			this.builder.Append(']');
		}

		// Token: 0x06003A2B RID: 14891 RVA: 0x00157028 File Offset: 0x00155228
		private void AppendComma(bool firstItem)
		{
			if (!firstItem)
			{
				this.builder.Append(',');
				if (this.PrettyPrintEnabled)
				{
					this.builder.Append('\n');
				}
			}
			if (this.PrettyPrintEnabled)
			{
				this.AppendIndent();
			}
		}

		// Token: 0x06003A2C RID: 14892 RVA: 0x0015705F File Offset: 0x0015525F
		private void AppendColon()
		{
			this.builder.Append(':');
			if (this.PrettyPrintEnabled)
			{
				this.builder.Append(' ');
			}
		}

		// Token: 0x040034BA RID: 13498
		private static readonly Type includeAttrType = typeof(Include);

		// Token: 0x040034BB RID: 13499
		private static readonly Type excludeAttrType = typeof(Exclude);

		// Token: 0x040034BC RID: 13500
		private static readonly Type typeHintAttrType = typeof(TypeHint);

		// Token: 0x040034BD RID: 13501
		private readonly StringBuilder builder;

		// Token: 0x040034BE RID: 13502
		private readonly EncodeOptions options;

		// Token: 0x040034BF RID: 13503
		private int indent;
	}
}
