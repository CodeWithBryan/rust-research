using System;
using UnityEngine;

namespace Rust
{
	// Token: 0x02000AC0 RID: 2752
	public class GC : MonoBehaviour, IClientComponent
	{
		// Token: 0x170005C0 RID: 1472
		// (get) Token: 0x060042A5 RID: 17061 RVA: 0x00003A54 File Offset: 0x00001C54
		public static bool Enabled
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060042A6 RID: 17062 RVA: 0x00185066 File Offset: 0x00183266
		public static void Collect()
		{
			GC.Collect();
		}

		// Token: 0x060042A7 RID: 17063 RVA: 0x0018506D File Offset: 0x0018326D
		public static long GetTotalMemory()
		{
			return GC.GetTotalMemory(false) / 1048576L;
		}

		// Token: 0x060042A8 RID: 17064 RVA: 0x0018507C File Offset: 0x0018327C
		public static int CollectionCount()
		{
			return GC.CollectionCount(0);
		}
	}
}
