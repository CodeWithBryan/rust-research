using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A57 RID: 2647
	internal static class MeshUtilities
	{
		// Token: 0x06003EA0 RID: 16032 RVA: 0x0016F9A4 File Offset: 0x0016DBA4
		internal static Mesh GetColliderMesh(Collider collider)
		{
			Type type = collider.GetType();
			if (type == typeof(MeshCollider))
			{
				return ((MeshCollider)collider).sharedMesh;
			}
			Assert.IsTrue(MeshUtilities.s_ColliderPrimitives.ContainsKey(type), "Unknown collider");
			return MeshUtilities.GetPrimitive(MeshUtilities.s_ColliderPrimitives[type]);
		}

		// Token: 0x06003EA1 RID: 16033 RVA: 0x0016F9FC File Offset: 0x0016DBFC
		internal static Mesh GetPrimitive(PrimitiveType primitiveType)
		{
			Mesh builtinMesh;
			if (!MeshUtilities.s_Primitives.TryGetValue(primitiveType, out builtinMesh))
			{
				builtinMesh = MeshUtilities.GetBuiltinMesh(primitiveType);
				MeshUtilities.s_Primitives.Add(primitiveType, builtinMesh);
			}
			return builtinMesh;
		}

		// Token: 0x06003EA2 RID: 16034 RVA: 0x0016FA2C File Offset: 0x0016DC2C
		private static Mesh GetBuiltinMesh(PrimitiveType primitiveType)
		{
			GameObject gameObject = GameObject.CreatePrimitive(primitiveType);
			Mesh sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
			RuntimeUtilities.Destroy(gameObject);
			return sharedMesh;
		}

		// Token: 0x04003799 RID: 14233
		private static Dictionary<PrimitiveType, Mesh> s_Primitives = new Dictionary<PrimitiveType, Mesh>();

		// Token: 0x0400379A RID: 14234
		private static Dictionary<Type, PrimitiveType> s_ColliderPrimitives = new Dictionary<Type, PrimitiveType>
		{
			{
				typeof(BoxCollider),
				PrimitiveType.Cube
			},
			{
				typeof(SphereCollider),
				PrimitiveType.Sphere
			},
			{
				typeof(CapsuleCollider),
				PrimitiveType.Capsule
			}
		};
	}
}
