using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000A9F RID: 2719
	[ConsoleSystem.Factory("time")]
	public class Time : ConsoleSystem
	{
		// Token: 0x1700056B RID: 1387
		// (get) Token: 0x060040F1 RID: 16625 RVA: 0x0017E5AC File Offset: 0x0017C7AC
		// (set) Token: 0x060040F2 RID: 16626 RVA: 0x0017E5B3 File Offset: 0x0017C7B3
		[ServerVar]
		[Help("Fixed delta time in seconds")]
		public static float fixeddelta
		{
			get
			{
				return Time.fixedDeltaTime;
			}
			set
			{
				Time.fixedDeltaTime = value;
			}
		}

		// Token: 0x1700056C RID: 1388
		// (get) Token: 0x060040F3 RID: 16627 RVA: 0x0017E5BB File Offset: 0x0017C7BB
		// (set) Token: 0x060040F4 RID: 16628 RVA: 0x0017E5C2 File Offset: 0x0017C7C2
		[ServerVar]
		[Help("The minimum amount of times to tick per frame")]
		public static float maxdelta
		{
			get
			{
				return Time.maximumDeltaTime;
			}
			set
			{
				Time.maximumDeltaTime = value;
			}
		}

		// Token: 0x1700056D RID: 1389
		// (get) Token: 0x060040F5 RID: 16629 RVA: 0x0017E5CA File Offset: 0x0017C7CA
		// (set) Token: 0x060040F6 RID: 16630 RVA: 0x0017E5D1 File Offset: 0x0017C7D1
		[ServerVar]
		[Help("The time scale")]
		public static float timescale
		{
			get
			{
				return Time.timeScale;
			}
			set
			{
				Time.timeScale = value;
			}
		}

		// Token: 0x04003A1A RID: 14874
		[ServerVar]
		[Help("Pause time while loading")]
		public static bool pausewhileloading = true;
	}
}
