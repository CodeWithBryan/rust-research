using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007D5 RID: 2005
public class SelectedContact : SingletonComponent<SelectedContact>
{
	// Token: 0x04002C74 RID: 11380
	public RustText nameText;

	// Token: 0x04002C75 RID: 11381
	public RustText seenText;

	// Token: 0x04002C76 RID: 11382
	public RawImage mugshotImage;

	// Token: 0x04002C77 RID: 11383
	public Texture2D unknownMugshot;

	// Token: 0x04002C78 RID: 11384
	public InputField noteInput;

	// Token: 0x04002C79 RID: 11385
	public GameObject[] relationshipTypeTags;

	// Token: 0x04002C7A RID: 11386
	public Translate.Phrase lastSeenPrefix;

	// Token: 0x04002C7B RID: 11387
	public Translate.Phrase nowPhrase;

	// Token: 0x04002C7C RID: 11388
	public Translate.Phrase agoSuffix;

	// Token: 0x04002C7D RID: 11389
	public RustButton FriendlyButton;

	// Token: 0x04002C7E RID: 11390
	public RustButton SeenButton;

	// Token: 0x04002C7F RID: 11391
	public RustButton EnemyButton;

	// Token: 0x04002C80 RID: 11392
	public RustButton chatMute;
}
