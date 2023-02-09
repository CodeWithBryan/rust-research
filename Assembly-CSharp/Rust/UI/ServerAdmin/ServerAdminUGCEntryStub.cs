using System;
using UnityEngine;

namespace Rust.UI.ServerAdmin
{
	// Token: 0x02000ADB RID: 2779
	public class ServerAdminUGCEntryStub : MonoBehaviour
	{
		// Token: 0x04003B59 RID: 15193
		public ServerAdminUGCEntryAudio AudioWidget;

		// Token: 0x04003B5A RID: 15194
		public ServerAdminUGCEntryImage ImageWidget;

		// Token: 0x04003B5B RID: 15195
		public ServerAdminUGCEntryPattern PatternWidget;

		// Token: 0x04003B5C RID: 15196
		public RustText PrefabName;

		// Token: 0x04003B5D RID: 15197
		public RustButton HistoryButton;

		// Token: 0x04003B5E RID: 15198
		public ServerAdminPlayerId[] HistoryIds = new ServerAdminPlayerId[0];
	}
}
