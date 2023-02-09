using System;
using System.IO;
using System.Text;

namespace TinyJSON
{
	// Token: 0x02000993 RID: 2451
	public sealed class Decoder : IDisposable
	{
		// Token: 0x06003A06 RID: 14854 RVA: 0x001560DC File Offset: 0x001542DC
		private Decoder(string jsonString)
		{
			this.json = new StringReader(jsonString);
		}

		// Token: 0x06003A07 RID: 14855 RVA: 0x001560F0 File Offset: 0x001542F0
		public static Variant Decode(string jsonString)
		{
			Variant result;
			using (Decoder decoder = new Decoder(jsonString))
			{
				result = decoder.DecodeValue();
			}
			return result;
		}

		// Token: 0x06003A08 RID: 14856 RVA: 0x00156128 File Offset: 0x00154328
		public void Dispose()
		{
			this.json.Dispose();
			this.json = null;
		}

		// Token: 0x06003A09 RID: 14857 RVA: 0x0015613C File Offset: 0x0015433C
		private ProxyObject DecodeObject()
		{
			ProxyObject proxyObject = new ProxyObject();
			this.json.Read();
			for (;;)
			{
				Decoder.Token nextToken = this.NextToken;
				if (nextToken == Decoder.Token.None)
				{
					break;
				}
				if (nextToken == Decoder.Token.CloseBrace)
				{
					return proxyObject;
				}
				if (nextToken != Decoder.Token.Comma)
				{
					string text = this.DecodeString();
					if (text == null)
					{
						goto Block_4;
					}
					if (this.NextToken != Decoder.Token.Colon)
					{
						goto Block_5;
					}
					this.json.Read();
					proxyObject.Add(text, this.DecodeValue());
				}
			}
			return null;
			Block_4:
			return null;
			Block_5:
			return null;
		}

		// Token: 0x06003A0A RID: 14858 RVA: 0x001561AC File Offset: 0x001543AC
		private ProxyArray DecodeArray()
		{
			ProxyArray proxyArray = new ProxyArray();
			this.json.Read();
			bool flag = true;
			while (flag)
			{
				Decoder.Token nextToken = this.NextToken;
				if (nextToken == Decoder.Token.None)
				{
					return null;
				}
				if (nextToken != Decoder.Token.CloseBracket)
				{
					if (nextToken != Decoder.Token.Comma)
					{
						proxyArray.Add(this.DecodeByToken(nextToken));
					}
				}
				else
				{
					flag = false;
				}
			}
			return proxyArray;
		}

		// Token: 0x06003A0B RID: 14859 RVA: 0x001561FC File Offset: 0x001543FC
		private Variant DecodeValue()
		{
			Decoder.Token nextToken = this.NextToken;
			return this.DecodeByToken(nextToken);
		}

		// Token: 0x06003A0C RID: 14860 RVA: 0x00156218 File Offset: 0x00154418
		private Variant DecodeByToken(Decoder.Token token)
		{
			switch (token)
			{
			case Decoder.Token.OpenBrace:
				return this.DecodeObject();
			case Decoder.Token.OpenBracket:
				return this.DecodeArray();
			case Decoder.Token.String:
				return this.DecodeString();
			case Decoder.Token.Number:
				return this.DecodeNumber();
			case Decoder.Token.True:
				return new ProxyBoolean(true);
			case Decoder.Token.False:
				return new ProxyBoolean(false);
			case Decoder.Token.Null:
				return null;
			}
			return null;
		}

		// Token: 0x06003A0D RID: 14861 RVA: 0x00156288 File Offset: 0x00154488
		private Variant DecodeString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.json.Read();
			bool flag = true;
			while (flag)
			{
				if (this.json.Peek() == -1)
				{
					break;
				}
				char nextChar = this.NextChar;
				if (nextChar != '"')
				{
					if (nextChar != '\\')
					{
						stringBuilder.Append(nextChar);
					}
					else if (this.json.Peek() == -1)
					{
						flag = false;
					}
					else
					{
						nextChar = this.NextChar;
						if (nextChar <= '\\')
						{
							if (nextChar == '"' || nextChar == '/' || nextChar == '\\')
							{
								stringBuilder.Append(nextChar);
							}
						}
						else if (nextChar <= 'f')
						{
							if (nextChar != 'b')
							{
								if (nextChar == 'f')
								{
									stringBuilder.Append('\f');
								}
							}
							else
							{
								stringBuilder.Append('\b');
							}
						}
						else if (nextChar != 'n')
						{
							switch (nextChar)
							{
							case 'r':
								stringBuilder.Append('\r');
								break;
							case 't':
								stringBuilder.Append('\t');
								break;
							case 'u':
							{
								StringBuilder stringBuilder2 = new StringBuilder();
								for (int i = 0; i < 4; i++)
								{
									stringBuilder2.Append(this.NextChar);
								}
								stringBuilder.Append((char)Convert.ToInt32(stringBuilder2.ToString(), 16));
								break;
							}
							}
						}
						else
						{
							stringBuilder.Append('\n');
						}
					}
				}
				else
				{
					flag = false;
				}
			}
			return new ProxyString(stringBuilder.ToString());
		}

		// Token: 0x06003A0E RID: 14862 RVA: 0x001563DF File Offset: 0x001545DF
		private Variant DecodeNumber()
		{
			return new ProxyNumber(this.NextWord);
		}

		// Token: 0x06003A0F RID: 14863 RVA: 0x001563EC File Offset: 0x001545EC
		private void ConsumeWhiteSpace()
		{
			while (" \t\n\r".IndexOf(this.PeekChar) != -1)
			{
				this.json.Read();
				if (this.json.Peek() == -1)
				{
					break;
				}
			}
		}

		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x06003A10 RID: 14864 RVA: 0x00156420 File Offset: 0x00154620
		private char PeekChar
		{
			get
			{
				int num = this.json.Peek();
				if (num != -1)
				{
					return Convert.ToChar(num);
				}
				return '\0';
			}
		}

		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x06003A11 RID: 14865 RVA: 0x00156445 File Offset: 0x00154645
		private char NextChar
		{
			get
			{
				return Convert.ToChar(this.json.Read());
			}
		}

		// Token: 0x17000479 RID: 1145
		// (get) Token: 0x06003A12 RID: 14866 RVA: 0x00156458 File Offset: 0x00154658
		private string NextWord
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				while (" \t\n\r{}[],:\"".IndexOf(this.PeekChar) == -1)
				{
					stringBuilder.Append(this.NextChar);
					if (this.json.Peek() == -1)
					{
						break;
					}
				}
				return stringBuilder.ToString();
			}
		}

		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x06003A13 RID: 14867 RVA: 0x001564A4 File Offset: 0x001546A4
		private Decoder.Token NextToken
		{
			get
			{
				this.ConsumeWhiteSpace();
				if (this.json.Peek() == -1)
				{
					return Decoder.Token.None;
				}
				char peekChar = this.PeekChar;
				if (peekChar <= '[')
				{
					switch (peekChar)
					{
					case '"':
						return Decoder.Token.String;
					case '#':
					case '$':
					case '%':
					case '&':
					case '\'':
					case '(':
					case ')':
					case '*':
					case '+':
					case '.':
					case '/':
						break;
					case ',':
						this.json.Read();
						return Decoder.Token.Comma;
					case '-':
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						return Decoder.Token.Number;
					case ':':
						return Decoder.Token.Colon;
					default:
						if (peekChar == '[')
						{
							return Decoder.Token.OpenBracket;
						}
						break;
					}
				}
				else
				{
					if (peekChar == ']')
					{
						this.json.Read();
						return Decoder.Token.CloseBracket;
					}
					if (peekChar == '{')
					{
						return Decoder.Token.OpenBrace;
					}
					if (peekChar == '}')
					{
						this.json.Read();
						return Decoder.Token.CloseBrace;
					}
				}
				string nextWord = this.NextWord;
				if (nextWord == "false")
				{
					return Decoder.Token.False;
				}
				if (nextWord == "true")
				{
					return Decoder.Token.True;
				}
				if (!(nextWord == "null"))
				{
					return Decoder.Token.None;
				}
				return Decoder.Token.Null;
			}
		}

		// Token: 0x040034B0 RID: 13488
		private const string whiteSpace = " \t\n\r";

		// Token: 0x040034B1 RID: 13489
		private const string wordBreak = " \t\n\r{}[],:\"";

		// Token: 0x040034B2 RID: 13490
		private StringReader json;

		// Token: 0x02000E87 RID: 3719
		private enum Token
		{
			// Token: 0x04004AEB RID: 19179
			None,
			// Token: 0x04004AEC RID: 19180
			OpenBrace,
			// Token: 0x04004AED RID: 19181
			CloseBrace,
			// Token: 0x04004AEE RID: 19182
			OpenBracket,
			// Token: 0x04004AEF RID: 19183
			CloseBracket,
			// Token: 0x04004AF0 RID: 19184
			Colon,
			// Token: 0x04004AF1 RID: 19185
			Comma,
			// Token: 0x04004AF2 RID: 19186
			String,
			// Token: 0x04004AF3 RID: 19187
			Number,
			// Token: 0x04004AF4 RID: 19188
			True,
			// Token: 0x04004AF5 RID: 19189
			False,
			// Token: 0x04004AF6 RID: 19190
			Null
		}
	}
}
