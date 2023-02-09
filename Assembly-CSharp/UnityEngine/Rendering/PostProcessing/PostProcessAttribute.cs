using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A01 RID: 2561
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class PostProcessAttribute : Attribute
	{
		// Token: 0x06003D3D RID: 15677 RVA: 0x00165BB8 File Offset: 0x00163DB8
		public PostProcessAttribute(Type renderer, PostProcessEvent eventType, string menuItem, bool allowInSceneView = true)
		{
			this.renderer = renderer;
			this.eventType = eventType;
			this.menuItem = menuItem;
			this.allowInSceneView = allowInSceneView;
			this.builtinEffect = false;
		}

		// Token: 0x06003D3E RID: 15678 RVA: 0x00165BE4 File Offset: 0x00163DE4
		internal PostProcessAttribute(Type renderer, string menuItem, bool allowInSceneView = true)
		{
			this.renderer = renderer;
			this.menuItem = menuItem;
			this.allowInSceneView = allowInSceneView;
			this.builtinEffect = true;
		}

		// Token: 0x04003646 RID: 13894
		public readonly Type renderer;

		// Token: 0x04003647 RID: 13895
		public readonly PostProcessEvent eventType;

		// Token: 0x04003648 RID: 13896
		public readonly string menuItem;

		// Token: 0x04003649 RID: 13897
		public readonly bool allowInSceneView;

		// Token: 0x0400364A RID: 13898
		internal readonly bool builtinEffect;
	}
}
