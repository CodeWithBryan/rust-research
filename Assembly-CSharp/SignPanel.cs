using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000172 RID: 370
public class SignPanel : MonoBehaviour, IImageReceiver
{
	// Token: 0x04000FB9 RID: 4025
	public RawImage Image;

	// Token: 0x04000FBA RID: 4026
	public RectTransform ImageContainer;

	// Token: 0x04000FBB RID: 4027
	public RustText DisabledSignsMessage;
}
