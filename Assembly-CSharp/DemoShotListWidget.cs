using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000786 RID: 1926
public class DemoShotListWidget : SingletonComponent<DemoShotListWidget>
{
	// Token: 0x04002A1A RID: 10778
	public GameObjectRef ShotListEntry;

	// Token: 0x04002A1B RID: 10779
	public GameObjectRef FolderEntry;

	// Token: 0x04002A1C RID: 10780
	public Transform ShotListParent;

	// Token: 0x04002A1D RID: 10781
	public RustInput FolderNameInput;

	// Token: 0x04002A1E RID: 10782
	public GameObject ShotsRoot;

	// Token: 0x04002A1F RID: 10783
	public GameObject NoShotsRoot;

	// Token: 0x04002A20 RID: 10784
	public GameObject TopUpArrow;

	// Token: 0x04002A21 RID: 10785
	public GameObject TopDownArrow;

	// Token: 0x04002A22 RID: 10786
	public Canvas DragCanvas;
}
