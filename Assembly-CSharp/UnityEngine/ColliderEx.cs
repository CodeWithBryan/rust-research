using System;
using Rust;

namespace UnityEngine
{
	// Token: 0x020009DD RID: 2525
	public static class ColliderEx
	{
		// Token: 0x06003B87 RID: 15239 RVA: 0x0015BD17 File Offset: 0x00159F17
		public static PhysicMaterial GetMaterialAt(this Collider obj, Vector3 pos)
		{
			if (obj is TerrainCollider)
			{
				return TerrainMeta.Physics.GetMaterial(pos);
			}
			return obj.sharedMaterial;
		}

		// Token: 0x06003B88 RID: 15240 RVA: 0x0015BD33 File Offset: 0x00159F33
		public static bool IsOnLayer(this Collider col, Layer rustLayer)
		{
			return col != null && col.gameObject.IsOnLayer(rustLayer);
		}

		// Token: 0x06003B89 RID: 15241 RVA: 0x0015BD4C File Offset: 0x00159F4C
		public static bool IsOnLayer(this Collider col, int layer)
		{
			return col != null && col.gameObject.IsOnLayer(layer);
		}

		// Token: 0x06003B8A RID: 15242 RVA: 0x0015BD68 File Offset: 0x00159F68
		public static float GetRadius(this Collider col, Vector3 transformScale)
		{
			float result = 1f;
			SphereCollider sphereCollider;
			BoxCollider boxCollider;
			CapsuleCollider capsuleCollider;
			MeshCollider meshCollider;
			if ((sphereCollider = (col as SphereCollider)) != null)
			{
				result = sphereCollider.radius * transformScale.Max();
			}
			else if ((boxCollider = (col as BoxCollider)) != null)
			{
				result = Vector3.Scale(boxCollider.size, transformScale).Max() * 0.5f;
			}
			else if ((capsuleCollider = (col as CapsuleCollider)) != null)
			{
				int direction = capsuleCollider.direction;
				float num;
				if (direction != 0)
				{
					if (direction != 1)
					{
						num = transformScale.x;
					}
					else
					{
						num = transformScale.x;
					}
				}
				else
				{
					num = transformScale.y;
				}
				result = capsuleCollider.radius * num;
			}
			else if ((meshCollider = (col as MeshCollider)) != null)
			{
				result = Vector3.Scale(meshCollider.bounds.size, transformScale).Max() * 0.5f;
			}
			return result;
		}
	}
}
