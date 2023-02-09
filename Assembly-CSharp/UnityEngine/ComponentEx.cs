using System;
using Facepunch;

namespace UnityEngine
{
	// Token: 0x020009DF RID: 2527
	public static class ComponentEx
	{
		// Token: 0x06003B8C RID: 15244 RVA: 0x0015BE3C File Offset: 0x0015A03C
		public static T Instantiate<T>(this T component) where T : Component
		{
			return Facepunch.Instantiate.GameObject(component.gameObject, null).GetComponent<T>();
		}

		// Token: 0x06003B8D RID: 15245 RVA: 0x0015BE54 File Offset: 0x0015A054
		public static bool HasComponent<T>(this Component component) where T : Component
		{
			return component.GetComponent<T>() != null;
		}
	}
}
