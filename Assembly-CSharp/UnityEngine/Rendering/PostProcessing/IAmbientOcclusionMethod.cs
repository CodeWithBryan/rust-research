using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A08 RID: 2568
	internal interface IAmbientOcclusionMethod
	{
		// Token: 0x06003D44 RID: 15684
		DepthTextureMode GetCameraFlags();

		// Token: 0x06003D45 RID: 15685
		void RenderAfterOpaque(PostProcessRenderContext context);

		// Token: 0x06003D46 RID: 15686
		void RenderAmbientOnly(PostProcessRenderContext context);

		// Token: 0x06003D47 RID: 15687
		void CompositeAmbientOnly(PostProcessRenderContext context);

		// Token: 0x06003D48 RID: 15688
		void Release();
	}
}
