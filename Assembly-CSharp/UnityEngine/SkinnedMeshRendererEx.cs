using System;

namespace UnityEngine
{
	// Token: 0x020009E7 RID: 2535
	public static class SkinnedMeshRendererEx
	{
		// Token: 0x06003BA7 RID: 15271 RVA: 0x0015C27C File Offset: 0x0015A47C
		public static Transform FindRig(this SkinnedMeshRenderer renderer)
		{
			Transform parent = renderer.transform.parent;
			Transform transform = renderer.rootBone;
			while (transform != null && transform.parent != null && transform.parent != parent)
			{
				transform = transform.parent;
			}
			return transform;
		}
	}
}
