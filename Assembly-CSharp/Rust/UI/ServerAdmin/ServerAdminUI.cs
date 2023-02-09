using System;
using Facepunch;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI.ServerAdmin
{
	// Token: 0x02000ADC RID: 2780
	public class ServerAdminUI : SingletonComponent<ServerAdminUI>
	{
		// Token: 0x04003B5F RID: 15199
		public GameObjectRef PlayerEntry;

		// Token: 0x04003B60 RID: 15200
		public RectTransform PlayerInfoParent;

		// Token: 0x04003B61 RID: 15201
		public RustText PlayerCount;

		// Token: 0x04003B62 RID: 15202
		public RustInput PlayerNameFilter;

		// Token: 0x04003B63 RID: 15203
		public GameObjectRef ServerInfoEntry;

		// Token: 0x04003B64 RID: 15204
		public RectTransform ServerInfoParent;

		// Token: 0x04003B65 RID: 15205
		public GameObjectRef ConvarInfoEntry;

		// Token: 0x04003B66 RID: 15206
		public GameObjectRef ConvarInfoLongEntry;

		// Token: 0x04003B67 RID: 15207
		public RectTransform ConvarInfoParent;

		// Token: 0x04003B68 RID: 15208
		public ServerAdminPlayerInfo PlayerInfo;

		// Token: 0x04003B69 RID: 15209
		public RustInput UgcNameFilter;

		// Token: 0x04003B6A RID: 15210
		public GameObjectRef ImageEntry;

		// Token: 0x04003B6B RID: 15211
		public GameObjectRef PatternEntry;

		// Token: 0x04003B6C RID: 15212
		public GameObjectRef SoundEntry;

		// Token: 0x04003B6D RID: 15213
		public VirtualScroll UgcVirtualScroll;

		// Token: 0x04003B6E RID: 15214
		public GameObject ExpandedUgcRoot;

		// Token: 0x04003B6F RID: 15215
		public RawImage ExpandedImage;

		// Token: 0x04003B70 RID: 15216
		public RectTransform ExpandedImageBacking;
	}
}
