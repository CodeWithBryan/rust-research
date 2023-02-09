using System;

namespace ConVar
{
	// Token: 0x02000A77 RID: 2679
	[ConsoleSystem.Factory("file")]
	public class FileConVar : ConsoleSystem
	{
		// Token: 0x1700053E RID: 1342
		// (get) Token: 0x06003FBF RID: 16319 RVA: 0x001783A7 File Offset: 0x001765A7
		// (set) Token: 0x06003FC0 RID: 16320 RVA: 0x001783AE File Offset: 0x001765AE
		[ClientVar]
		public static bool debug
		{
			get
			{
				return FileSystem.LogDebug;
			}
			set
			{
				FileSystem.LogDebug = value;
			}
		}

		// Token: 0x1700053F RID: 1343
		// (get) Token: 0x06003FC1 RID: 16321 RVA: 0x001783B6 File Offset: 0x001765B6
		// (set) Token: 0x06003FC2 RID: 16322 RVA: 0x001783BD File Offset: 0x001765BD
		[ClientVar]
		public static bool time
		{
			get
			{
				return FileSystem.LogTime;
			}
			set
			{
				FileSystem.LogTime = value;
			}
		}
	}
}
