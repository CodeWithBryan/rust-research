using System;
using System.Runtime.InteropServices;

namespace Rust
{
	// Token: 0x02000ABF RID: 2751
	public static class Numlock
	{
		// Token: 0x060042A1 RID: 17057
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		private static extern short GetKeyState(int keyCode);

		// Token: 0x060042A2 RID: 17058
		[DllImport("user32.dll")]
		private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

		// Token: 0x170005BF RID: 1471
		// (get) Token: 0x060042A3 RID: 17059 RVA: 0x0018502B File Offset: 0x0018322B
		public static bool IsOn
		{
			get
			{
				return ((ushort)Numlock.GetKeyState(144) & ushort.MaxValue) > 0;
			}
		}

		// Token: 0x060042A4 RID: 17060 RVA: 0x00185041 File Offset: 0x00183241
		public static void TurnOn()
		{
			if (!Numlock.IsOn)
			{
				Numlock.keybd_event(144, 69, 1U, 0);
				Numlock.keybd_event(144, 69, 3U, 0);
			}
		}

		// Token: 0x04003AD6 RID: 15062
		private const byte VK_NUMLOCK = 144;

		// Token: 0x04003AD7 RID: 15063
		private const uint KEYEVENTF_EXTENDEDKEY = 1U;

		// Token: 0x04003AD8 RID: 15064
		private const int KEYEVENTF_KEYUP = 2;

		// Token: 0x04003AD9 RID: 15065
		private const int KEYEVENTF_KEYDOWN = 0;
	}
}
