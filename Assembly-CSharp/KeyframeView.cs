using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000789 RID: 1929
public class KeyframeView : MonoBehaviour
{
	// Token: 0x04002A3F RID: 10815
	public ScrollRect Scroller;

	// Token: 0x04002A40 RID: 10816
	public GameObjectRef KeyframePrefab;

	// Token: 0x04002A41 RID: 10817
	public RectTransform KeyframeRoot;

	// Token: 0x04002A42 RID: 10818
	public Transform CurrentPositionIndicator;

	// Token: 0x04002A43 RID: 10819
	public bool LockScrollToCurrentPosition;

	// Token: 0x04002A44 RID: 10820
	public RustText TrackName;
}
