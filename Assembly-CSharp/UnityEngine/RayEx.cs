using System;

namespace UnityEngine
{
	// Token: 0x020009E6 RID: 2534
	public static class RayEx
	{
		// Token: 0x06003BA3 RID: 15267 RVA: 0x0015C1CA File Offset: 0x0015A3CA
		public static Vector3 ClosestPoint(this Ray ray, Vector3 pos)
		{
			return ray.origin + Vector3.Dot(pos - ray.origin, ray.direction) * ray.direction;
		}

		// Token: 0x06003BA4 RID: 15268 RVA: 0x0015C200 File Offset: 0x0015A400
		public static float Distance(this Ray ray, Vector3 pos)
		{
			return Vector3.Cross(ray.direction, pos - ray.origin).magnitude;
		}

		// Token: 0x06003BA5 RID: 15269 RVA: 0x0015C230 File Offset: 0x0015A430
		public static float SqrDistance(this Ray ray, Vector3 pos)
		{
			return Vector3.Cross(ray.direction, pos - ray.origin).sqrMagnitude;
		}

		// Token: 0x06003BA6 RID: 15270 RVA: 0x0015C25E File Offset: 0x0015A45E
		public static bool IsNaNOrInfinity(this Ray r)
		{
			return r.origin.IsNaNOrInfinity() || r.direction.IsNaNOrInfinity();
		}
	}
}
