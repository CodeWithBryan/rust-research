using System;
using UnityEngine.SceneManagement;

namespace Rust
{
	// Token: 0x02000AC8 RID: 2760
	public static class Server
	{
		// Token: 0x170005C4 RID: 1476
		// (get) Token: 0x060042C0 RID: 17088 RVA: 0x001852D4 File Offset: 0x001834D4
		public static Scene EntityScene
		{
			get
			{
				if (!Server._entityScene.IsValid())
				{
					Server._entityScene = SceneManager.CreateScene("Server Entities");
				}
				return Server._entityScene;
			}
		}

		// Token: 0x04003AFB RID: 15099
		public const float UseDistance = 3f;

		// Token: 0x04003AFC RID: 15100
		private static Scene _entityScene;
	}
}
