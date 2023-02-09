using System;

namespace ConVar
{
	// Token: 0x02000A76 RID: 2678
	[ConsoleSystem.Factory("env")]
	public class Env : ConsoleSystem
	{
		// Token: 0x17000538 RID: 1336
		// (get) Token: 0x06003FB2 RID: 16306 RVA: 0x00178221 File Offset: 0x00176421
		// (set) Token: 0x06003FB1 RID: 16305 RVA: 0x001781FC File Offset: 0x001763FC
		[ServerVar]
		public static bool progresstime
		{
			get
			{
				return !(TOD_Sky.Instance == null) && TOD_Sky.Instance.Components.Time.ProgressTime;
			}
			set
			{
				if (TOD_Sky.Instance == null)
				{
					return;
				}
				TOD_Sky.Instance.Components.Time.ProgressTime = value;
			}
		}

		// Token: 0x17000539 RID: 1337
		// (get) Token: 0x06003FB4 RID: 16308 RVA: 0x00178266 File Offset: 0x00176466
		// (set) Token: 0x06003FB3 RID: 16307 RVA: 0x00178246 File Offset: 0x00176446
		[ServerVar(ShowInAdminUI = true)]
		public static float time
		{
			get
			{
				if (TOD_Sky.Instance == null)
				{
					return 0f;
				}
				return TOD_Sky.Instance.Cycle.Hour;
			}
			set
			{
				if (TOD_Sky.Instance == null)
				{
					return;
				}
				TOD_Sky.Instance.Cycle.Hour = value;
			}
		}

		// Token: 0x1700053A RID: 1338
		// (get) Token: 0x06003FB6 RID: 16310 RVA: 0x001782AA File Offset: 0x001764AA
		// (set) Token: 0x06003FB5 RID: 16309 RVA: 0x0017828A File Offset: 0x0017648A
		[ServerVar]
		public static int day
		{
			get
			{
				if (TOD_Sky.Instance == null)
				{
					return 0;
				}
				return TOD_Sky.Instance.Cycle.Day;
			}
			set
			{
				if (TOD_Sky.Instance == null)
				{
					return;
				}
				TOD_Sky.Instance.Cycle.Day = value;
			}
		}

		// Token: 0x1700053B RID: 1339
		// (get) Token: 0x06003FB8 RID: 16312 RVA: 0x001782EA File Offset: 0x001764EA
		// (set) Token: 0x06003FB7 RID: 16311 RVA: 0x001782CA File Offset: 0x001764CA
		[ServerVar]
		public static int month
		{
			get
			{
				if (TOD_Sky.Instance == null)
				{
					return 0;
				}
				return TOD_Sky.Instance.Cycle.Month;
			}
			set
			{
				if (TOD_Sky.Instance == null)
				{
					return;
				}
				TOD_Sky.Instance.Cycle.Month = value;
			}
		}

		// Token: 0x1700053C RID: 1340
		// (get) Token: 0x06003FBA RID: 16314 RVA: 0x0017832A File Offset: 0x0017652A
		// (set) Token: 0x06003FB9 RID: 16313 RVA: 0x0017830A File Offset: 0x0017650A
		[ServerVar]
		public static int year
		{
			get
			{
				if (TOD_Sky.Instance == null)
				{
					return 0;
				}
				return TOD_Sky.Instance.Cycle.Year;
			}
			set
			{
				if (TOD_Sky.Instance == null)
				{
					return;
				}
				TOD_Sky.Instance.Cycle.Year = value;
			}
		}

		// Token: 0x06003FBB RID: 16315 RVA: 0x0017834C File Offset: 0x0017654C
		[ServerVar]
		public static void addtime(ConsoleSystem.Arg arg)
		{
			if (TOD_Sky.Instance == null)
			{
				return;
			}
			DateTime dateTime = TOD_Sky.Instance.Cycle.DateTime.AddTicks(arg.GetTicks(0, 0L));
			TOD_Sky.Instance.Cycle.DateTime = dateTime;
		}

		// Token: 0x1700053D RID: 1341
		// (get) Token: 0x06003FBC RID: 16316 RVA: 0x00178398 File Offset: 0x00176598
		// (set) Token: 0x06003FBD RID: 16317 RVA: 0x0017839F File Offset: 0x0017659F
		[ReplicatedVar(Default = "0")]
		public static float oceanlevel
		{
			get
			{
				return WaterSystem.OceanLevel;
			}
			set
			{
				WaterSystem.OceanLevel = value;
			}
		}
	}
}
