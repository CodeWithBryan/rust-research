using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000A78 RID: 2680
	[ConsoleSystem.Factory("fps")]
	public class FPS : ConsoleSystem
	{
		// Token: 0x17000540 RID: 1344
		// (get) Token: 0x06003FC4 RID: 16324 RVA: 0x001783C5 File Offset: 0x001765C5
		// (set) Token: 0x06003FC5 RID: 16325 RVA: 0x001783DE File Offset: 0x001765DE
		[ClientVar(Saved = true)]
		[ServerVar(Saved = true)]
		public static int limit
		{
			get
			{
				if (FPS._limit == -1)
				{
					FPS._limit = Application.targetFrameRate;
				}
				return FPS._limit;
			}
			set
			{
				FPS._limit = value;
				Application.targetFrameRate = FPS._limit;
			}
		}

		// Token: 0x17000541 RID: 1345
		// (get) Token: 0x06003FC6 RID: 16326 RVA: 0x001783F0 File Offset: 0x001765F0
		// (set) Token: 0x06003FC7 RID: 16327 RVA: 0x001783F8 File Offset: 0x001765F8
		[ClientVar]
		public static int graph
		{
			get
			{
				return FPS.m_graph;
			}
			set
			{
				FPS.m_graph = value;
				if (!MainCamera.mainCamera)
				{
					return;
				}
				FPSGraph component = MainCamera.mainCamera.GetComponent<FPSGraph>();
				if (!component)
				{
					return;
				}
				component.Refresh();
			}
		}

		// Token: 0x0400392B RID: 14635
		private static int _limit = 240;

		// Token: 0x0400392C RID: 14636
		private static int m_graph;
	}
}
