using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000945 RID: 2373
public class ViewModelRenderer : MonoBehaviour
{
	// Token: 0x04003266 RID: 12902
	public List<Texture2D> cachedTextureRefs = new List<Texture2D>();

	// Token: 0x04003267 RID: 12903
	public List<ViewModelDrawEvent> opaqueEvents = new List<ViewModelDrawEvent>();

	// Token: 0x04003268 RID: 12904
	public List<ViewModelDrawEvent> transparentEvents = new List<ViewModelDrawEvent>();

	// Token: 0x04003269 RID: 12905
	public Matrix4x4 prevModelMatrix;

	// Token: 0x0400326A RID: 12906
	private Renderer viewModelRenderer;
}
