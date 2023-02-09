using System;

namespace UnityEngine
{
	// Token: 0x020009DE RID: 2526
	public static class CollisionEx
	{
		// Token: 0x06003B8B RID: 15243 RVA: 0x0015BE2F File Offset: 0x0015A02F
		public static BaseEntity GetEntity(this Collision col)
		{
			return col.transform.ToBaseEntity();
		}
	}
}
