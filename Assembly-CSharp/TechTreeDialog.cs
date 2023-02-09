using System;
using System.Collections.Generic;
using Rust.UI;
using UnityEngine;

// Token: 0x020007A9 RID: 1961
public class TechTreeDialog : UIDialog, IInventoryChanged
{
	// Token: 0x04002B69 RID: 11113
	public TechTreeData data;

	// Token: 0x04002B6A RID: 11114
	public float graphScale = 1f;

	// Token: 0x04002B6B RID: 11115
	public TechTreeEntry entryPrefab;

	// Token: 0x04002B6C RID: 11116
	public TechTreeGroup groupPrefab;

	// Token: 0x04002B6D RID: 11117
	public TechTreeLine linePrefab;

	// Token: 0x04002B6E RID: 11118
	public RectTransform contents;

	// Token: 0x04002B6F RID: 11119
	public RectTransform contentParent;

	// Token: 0x04002B70 RID: 11120
	public TechTreeSelectedNodeUI selectedNodeUI;

	// Token: 0x04002B71 RID: 11121
	public float nodeSize = 128f;

	// Token: 0x04002B72 RID: 11122
	public float gridSize = 64f;

	// Token: 0x04002B73 RID: 11123
	public GameObjectRef unlockEffect;

	// Token: 0x04002B74 RID: 11124
	public RustText scrapCount;

	// Token: 0x04002B75 RID: 11125
	private Vector2 startPos = Vector2.zero;

	// Token: 0x04002B76 RID: 11126
	public List<int> processed = new List<int>();

	// Token: 0x04002B77 RID: 11127
	public Dictionary<int, TechTreeWidget> widgets = new Dictionary<int, TechTreeWidget>();

	// Token: 0x04002B78 RID: 11128
	public List<TechTreeLine> lines = new List<TechTreeLine>();

	// Token: 0x04002B79 RID: 11129
	public ScrollRectZoom zoom;
}
