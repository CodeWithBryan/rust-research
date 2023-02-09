using System;
using UnityEngine.SceneManagement;

namespace Rust
{
	// Token: 0x02000AC7 RID: 2759
	public static class Generic
	{
		// Token: 0x170005C3 RID: 1475
		// (get) Token: 0x060042BF RID: 17087 RVA: 0x001852B2 File Offset: 0x001834B2
		public static Scene BatchingScene
		{
			get
			{
				if (!Generic._batchingScene.IsValid())
				{
					Generic._batchingScene = SceneManager.CreateScene("Batching");
				}
				return Generic._batchingScene;
			}
		}

		// Token: 0x04003AFA RID: 15098
		private static Scene _batchingScene;
	}
}
