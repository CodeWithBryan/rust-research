using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000868 RID: 2152
public class UnreadMessages : SingletonComponent<UnreadMessages>
{
	// Token: 0x04002FB6 RID: 12214
	public StyleAsset AllRead;

	// Token: 0x04002FB7 RID: 12215
	public StyleAsset Unread;

	// Token: 0x04002FB8 RID: 12216
	public RustButton Button;

	// Token: 0x04002FB9 RID: 12217
	public GameObject UnreadTextObject;

	// Token: 0x04002FBA RID: 12218
	public RustText UnreadText;

	// Token: 0x04002FBB RID: 12219
	public GameObject MessageList;

	// Token: 0x04002FBC RID: 12220
	public GameObject MessageListContainer;

	// Token: 0x04002FBD RID: 12221
	public GameObject MessageListEmpty;
}
