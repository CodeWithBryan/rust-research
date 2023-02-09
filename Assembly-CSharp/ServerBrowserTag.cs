using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000862 RID: 2146
public class ServerBrowserTag : MonoBehaviour
{
	// Token: 0x17000400 RID: 1024
	// (get) Token: 0x06003527 RID: 13607 RVA: 0x0013FE6B File Offset: 0x0013E06B
	public bool IsActive
	{
		get
		{
			return this.button != null && this.button.IsPressed;
		}
	}

	// Token: 0x04002FA5 RID: 12197
	public string serverTag;

	// Token: 0x04002FA6 RID: 12198
	public RustButton button;
}
