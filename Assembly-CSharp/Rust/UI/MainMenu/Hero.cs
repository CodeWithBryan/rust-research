using System;
using UnityEngine;

namespace Rust.UI.MainMenu
{
	// Token: 0x02000ADD RID: 2781
	public class Hero : SingletonComponent<Hero>
	{
		// Token: 0x04003B71 RID: 15217
		public CanvasGroup CanvasGroup;

		// Token: 0x04003B72 RID: 15218
		public Video VideoPlayer;

		// Token: 0x04003B73 RID: 15219
		public RustText TitleText;

		// Token: 0x04003B74 RID: 15220
		public RustText ButtonText;

		// Token: 0x04003B75 RID: 15221
		public HttpImage TitleImage;

		// Token: 0x04003B76 RID: 15222
		[Header("Item Store Links")]
		public RustButton ItemStoreButton;

		// Token: 0x04003B77 RID: 15223
		public RustButton LimitedTabButton;

		// Token: 0x04003B78 RID: 15224
		public RustButton GeneralTabButton;
	}
}
