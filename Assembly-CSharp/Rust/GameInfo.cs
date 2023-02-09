using System;
using ConVar;
using UnityEngine;

namespace Rust
{
	// Token: 0x02000AC5 RID: 2757
	internal static class GameInfo
	{
		// Token: 0x170005C1 RID: 1473
		// (get) Token: 0x060042BC RID: 17084 RVA: 0x0018528F File Offset: 0x0018348F
		internal static bool IsOfficialServer
		{
			get
			{
				return Application.isEditor || Server.official;
			}
		}

		// Token: 0x170005C2 RID: 1474
		// (get) Token: 0x060042BD RID: 17085 RVA: 0x0018529F File Offset: 0x0018349F
		internal static bool HasAchievements
		{
			get
			{
				return GameInfo.IsOfficialServer;
			}
		}
	}
}
