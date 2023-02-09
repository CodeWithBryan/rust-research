using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Win32.SafeHandles;
using UnityEngine;

namespace Windows
{
	// Token: 0x020009D9 RID: 2521
	[SuppressUnmanagedCodeSecurity]
	public class ConsoleWindow
	{
		// Token: 0x06003B76 RID: 15222 RVA: 0x0015BB58 File Offset: 0x00159D58
		public void Initialize()
		{
			ConsoleWindow.FreeConsole();
			if (!ConsoleWindow.AttachConsole(4294967295U))
			{
				ConsoleWindow.AllocConsole();
			}
			this.oldOutput = Console.Out;
			try
			{
				Console.OutputEncoding = Encoding.UTF8;
				Console.SetOut(new StreamWriter(new FileStream(new SafeFileHandle(ConsoleWindow.GetStdHandle(-11), true), FileAccess.Write), Encoding.UTF8)
				{
					AutoFlush = true
				});
			}
			catch (Exception ex)
			{
				Debug.Log("Couldn't redirect output: " + ex.Message);
			}
		}

		// Token: 0x06003B77 RID: 15223 RVA: 0x0015BBE4 File Offset: 0x00159DE4
		public void Shutdown()
		{
			Console.SetOut(this.oldOutput);
			ConsoleWindow.FreeConsole();
		}

		// Token: 0x06003B78 RID: 15224 RVA: 0x0015BBF7 File Offset: 0x00159DF7
		public void SetTitle(string strName)
		{
			ConsoleWindow.SetConsoleTitleA(strName);
		}

		// Token: 0x06003B79 RID: 15225
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool AttachConsole(uint dwProcessId);

		// Token: 0x06003B7A RID: 15226
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool AllocConsole();

		// Token: 0x06003B7B RID: 15227
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool FreeConsole();

		// Token: 0x06003B7C RID: 15228
		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetStdHandle(int nStdHandle);

		// Token: 0x06003B7D RID: 15229
		[DllImport("kernel32.dll")]
		private static extern bool SetConsoleTitleA(string lpConsoleTitle);

		// Token: 0x04003550 RID: 13648
		private TextWriter oldOutput;

		// Token: 0x04003551 RID: 13649
		private const int STD_INPUT_HANDLE = -10;

		// Token: 0x04003552 RID: 13650
		private const int STD_OUTPUT_HANDLE = -11;
	}
}
