using System;
using UnityEngine;

namespace Windows
{
	// Token: 0x020009D8 RID: 2520
	public class ConsoleInput
	{
		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06003B6B RID: 15211 RVA: 0x0015B7F0 File Offset: 0x001599F0
		// (remove) Token: 0x06003B6C RID: 15212 RVA: 0x0015B828 File Offset: 0x00159A28
		public event Action<string> OnInputText;

		// Token: 0x170004A1 RID: 1185
		// (get) Token: 0x06003B6D RID: 15213 RVA: 0x0015B85D File Offset: 0x00159A5D
		public bool valid
		{
			get
			{
				return Console.BufferWidth > 0;
			}
		}

		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x06003B6E RID: 15214 RVA: 0x0015B867 File Offset: 0x00159A67
		public int lineWidth
		{
			get
			{
				return Console.BufferWidth;
			}
		}

		// Token: 0x06003B6F RID: 15215 RVA: 0x0015B86E File Offset: 0x00159A6E
		public void ClearLine(int numLines)
		{
			Console.CursorLeft = 0;
			Console.Write(new string(' ', this.lineWidth * numLines));
			Console.CursorTop -= numLines;
			Console.CursorLeft = 0;
		}

		// Token: 0x06003B70 RID: 15216 RVA: 0x0015B89C File Offset: 0x00159A9C
		public void RedrawInputLine()
		{
			ConsoleColor backgroundColor = Console.BackgroundColor;
			ConsoleColor foregroundColor = Console.ForegroundColor;
			try
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.CursorTop++;
				for (int i = 0; i < this.statusText.Length; i++)
				{
					Console.CursorLeft = 0;
					Console.Write(this.statusText[i].PadRight(this.lineWidth));
				}
				Console.CursorTop -= this.statusText.Length + 1;
				Console.CursorLeft = 0;
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.Green;
				this.ClearLine(1);
				if (this.inputString.Length == 0)
				{
					Console.BackgroundColor = backgroundColor;
					Console.ForegroundColor = foregroundColor;
					return;
				}
				if (this.inputString.Length < this.lineWidth - 2)
				{
					Console.Write(this.inputString);
				}
				else
				{
					Console.Write(this.inputString.Substring(this.inputString.Length - (this.lineWidth - 2)));
				}
			}
			catch (Exception)
			{
			}
			Console.BackgroundColor = backgroundColor;
			Console.ForegroundColor = foregroundColor;
		}

		// Token: 0x06003B71 RID: 15217 RVA: 0x0015B9AC File Offset: 0x00159BAC
		internal void OnBackspace()
		{
			if (this.inputString.Length < 1)
			{
				return;
			}
			this.inputString = this.inputString.Substring(0, this.inputString.Length - 1);
			this.RedrawInputLine();
		}

		// Token: 0x06003B72 RID: 15218 RVA: 0x0015B9E2 File Offset: 0x00159BE2
		internal void OnEscape()
		{
			this.inputString = "";
			this.RedrawInputLine();
		}

		// Token: 0x06003B73 RID: 15219 RVA: 0x0015B9F8 File Offset: 0x00159BF8
		internal void OnEnter()
		{
			this.ClearLine(this.statusText.Length);
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("> " + this.inputString);
			string obj = this.inputString;
			this.inputString = "";
			if (this.OnInputText != null)
			{
				this.OnInputText(obj);
			}
			this.RedrawInputLine();
		}

		// Token: 0x06003B74 RID: 15220 RVA: 0x0015BA5C File Offset: 0x00159C5C
		public void Update()
		{
			if (!this.valid)
			{
				return;
			}
			if (this.nextUpdate < Time.realtimeSinceStartup)
			{
				this.RedrawInputLine();
				this.nextUpdate = Time.realtimeSinceStartup + 0.5f;
			}
			try
			{
				if (!Console.KeyAvailable)
				{
					return;
				}
			}
			catch (Exception)
			{
				return;
			}
			ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
			if (consoleKeyInfo.Key == ConsoleKey.Enter)
			{
				this.OnEnter();
				return;
			}
			if (consoleKeyInfo.Key == ConsoleKey.Backspace)
			{
				this.OnBackspace();
				return;
			}
			if (consoleKeyInfo.Key == ConsoleKey.Escape)
			{
				this.OnEscape();
				return;
			}
			if (consoleKeyInfo.KeyChar != '\0')
			{
				this.inputString += consoleKeyInfo.KeyChar.ToString();
				this.RedrawInputLine();
				return;
			}
		}

		// Token: 0x0400354D RID: 13645
		public string inputString = "";

		// Token: 0x0400354E RID: 13646
		public string[] statusText = new string[]
		{
			"",
			"",
			""
		};

		// Token: 0x0400354F RID: 13647
		internal float nextUpdate;
	}
}
