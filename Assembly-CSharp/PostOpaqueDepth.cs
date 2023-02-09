using System;
using UnityEngine;

// Token: 0x02000705 RID: 1797
[ExecuteInEditMode]
[RequireComponent(typeof(CommandBufferManager))]
public class PostOpaqueDepth : MonoBehaviour
{
	// Token: 0x170003E7 RID: 999
	// (get) Token: 0x060031C0 RID: 12736 RVA: 0x00130F96 File Offset: 0x0012F196
	public RenderTexture PostOpaque
	{
		get
		{
			return this.postOpaqueDepth;
		}
	}

	// Token: 0x0400285F RID: 10335
	public RenderTexture postOpaqueDepth;
}
