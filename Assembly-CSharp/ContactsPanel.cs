using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007BF RID: 1983
public class ContactsPanel : SingletonComponent<ContactsPanel>
{
	// Token: 0x04002BEB RID: 11243
	public RectTransform alliesBucket;

	// Token: 0x04002BEC RID: 11244
	public RectTransform seenBucket;

	// Token: 0x04002BED RID: 11245
	public RectTransform enemiesBucket;

	// Token: 0x04002BEE RID: 11246
	public RectTransform contentsBucket;

	// Token: 0x04002BEF RID: 11247
	public ContactsEntry contactEntryPrefab;

	// Token: 0x04002BF0 RID: 11248
	public RawImage mugshotTest;

	// Token: 0x04002BF1 RID: 11249
	public RawImage fullBodyTest;

	// Token: 0x04002BF2 RID: 11250
	public RustButton[] filterButtons;

	// Token: 0x04002BF3 RID: 11251
	public RelationshipManager.RelationshipType selectedRelationshipType = RelationshipManager.RelationshipType.Friend;

	// Token: 0x04002BF4 RID: 11252
	public RustButton lastSeenToggle;

	// Token: 0x04002BF5 RID: 11253
	public Translate.Phrase sortingByLastSeenPhrase;

	// Token: 0x04002BF6 RID: 11254
	public Translate.Phrase sortingByFirstSeen;

	// Token: 0x04002BF7 RID: 11255
	public RustText sortText;

	// Token: 0x02000E20 RID: 3616
	public enum SortMode
	{
		// Token: 0x04004950 RID: 18768
		None,
		// Token: 0x04004951 RID: 18769
		RecentlySeen
	}
}
