using System;

namespace UnityEngine
{
	// Token: 0x020009E5 RID: 2533
	public static class QuaternionEx
	{
		// Token: 0x06003B9D RID: 15261 RVA: 0x0015C080 File Offset: 0x0015A280
		public static Quaternion AlignToNormal(this Quaternion rot, Vector3 normal)
		{
			return Quaternion.FromToRotation(Vector3.up, normal) * rot;
		}

		// Token: 0x06003B9E RID: 15262 RVA: 0x0015C093 File Offset: 0x0015A293
		public static Quaternion LookRotationWithOffset(Vector3 offset, Vector3 forward, Vector3 up)
		{
			return Quaternion.LookRotation(forward, Vector3.up) * Quaternion.Inverse(Quaternion.LookRotation(offset, Vector3.up));
		}

		// Token: 0x06003B9F RID: 15263 RVA: 0x0015C0B8 File Offset: 0x0015A2B8
		public static Quaternion LookRotationForcedUp(Vector3 forward, Vector3 up)
		{
			if (forward == up)
			{
				return Quaternion.LookRotation(up);
			}
			Vector3 rhs = Vector3.Cross(forward, up);
			forward = Vector3.Cross(up, rhs);
			return Quaternion.LookRotation(forward, up);
		}

		// Token: 0x06003BA0 RID: 15264 RVA: 0x0015C0F0 File Offset: 0x0015A2F0
		public static Quaternion LookRotationGradient(Vector3 normal, Vector3 up)
		{
			Vector3 rhs = (normal == Vector3.up) ? Vector3.forward : Vector3.Cross(normal, Vector3.up);
			return QuaternionEx.LookRotationForcedUp(Vector3.Cross(normal, rhs), up);
		}

		// Token: 0x06003BA1 RID: 15265 RVA: 0x0015C12C File Offset: 0x0015A32C
		public static Quaternion LookRotationNormal(Vector3 normal, Vector3 up = default(Vector3))
		{
			if (up != Vector3.zero)
			{
				return QuaternionEx.LookRotationForcedUp(up, normal);
			}
			if (normal == Vector3.up)
			{
				return QuaternionEx.LookRotationForcedUp(Vector3.forward, normal);
			}
			if (normal == Vector3.down)
			{
				return QuaternionEx.LookRotationForcedUp(Vector3.back, normal);
			}
			if (normal.y == 0f)
			{
				return QuaternionEx.LookRotationForcedUp(Vector3.up, normal);
			}
			Vector3 rhs = Vector3.Cross(normal, Vector3.up);
			return QuaternionEx.LookRotationForcedUp(-Vector3.Cross(normal, rhs), normal);
		}

		// Token: 0x06003BA2 RID: 15266 RVA: 0x0015C1B7 File Offset: 0x0015A3B7
		public static Quaternion EnsureValid(this Quaternion rot, float epsilon = 1E-45f)
		{
			if (Quaternion.Dot(rot, rot) < epsilon)
			{
				return Quaternion.identity;
			}
			return rot;
		}
	}
}
