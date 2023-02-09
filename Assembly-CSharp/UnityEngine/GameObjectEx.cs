using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using Rust.Registry;

namespace UnityEngine
{
	// Token: 0x020009E1 RID: 2529
	public static class GameObjectEx
	{
		// Token: 0x06003B92 RID: 15250 RVA: 0x0015BED5 File Offset: 0x0015A0D5
		public static BaseEntity ToBaseEntity(this GameObject go)
		{
			return go.transform.ToBaseEntity();
		}

		// Token: 0x06003B93 RID: 15251 RVA: 0x0015BEE2 File Offset: 0x0015A0E2
		public static BaseEntity ToBaseEntity(this Collider collider)
		{
			return collider.transform.ToBaseEntity();
		}

		// Token: 0x06003B94 RID: 15252 RVA: 0x0015BEF0 File Offset: 0x0015A0F0
		public static BaseEntity ToBaseEntity(this Transform transform)
		{
			IEntity entity = GameObjectEx.GetEntityFromRegistry(transform);
			if (entity == null && !transform.gameObject.activeInHierarchy)
			{
				entity = GameObjectEx.GetEntityFromComponent(transform);
			}
			return entity as BaseEntity;
		}

		// Token: 0x06003B95 RID: 15253 RVA: 0x0015BF21 File Offset: 0x0015A121
		public static bool IsOnLayer(this GameObject go, Layer rustLayer)
		{
			return go.IsOnLayer((int)rustLayer);
		}

		// Token: 0x06003B96 RID: 15254 RVA: 0x0015BF2A File Offset: 0x0015A12A
		public static bool IsOnLayer(this GameObject go, int layer)
		{
			return go != null && go.layer == layer;
		}

		// Token: 0x06003B97 RID: 15255 RVA: 0x0015BF40 File Offset: 0x0015A140
		private static IEntity GetEntityFromRegistry(Transform transform)
		{
			Transform transform2 = transform;
			IEntity entity = Entity.Get(transform2);
			while (entity == null && transform2.parent != null)
			{
				transform2 = transform2.parent;
				entity = Entity.Get(transform2);
			}
			if (entity != null && !entity.IsDestroyed)
			{
				return entity;
			}
			return null;
		}

		// Token: 0x06003B98 RID: 15256 RVA: 0x0015BF88 File Offset: 0x0015A188
		private static IEntity GetEntityFromComponent(Transform transform)
		{
			Transform transform2 = transform;
			IEntity component = transform2.GetComponent<IEntity>();
			while (component == null && transform2.parent != null)
			{
				transform2 = transform2.parent;
				component = transform2.GetComponent<IEntity>();
			}
			if (component != null && !component.IsDestroyed)
			{
				return component;
			}
			return null;
		}

		// Token: 0x06003B99 RID: 15257 RVA: 0x0015BFCD File Offset: 0x0015A1CD
		public static void SetHierarchyGroup(this GameObject obj, string strRoot, bool groupActive = true, bool persistant = false)
		{
			obj.transform.SetParent(HierarchyUtil.GetRoot(strRoot, groupActive, persistant).transform, true);
		}

		// Token: 0x06003B9A RID: 15258 RVA: 0x0015BFE8 File Offset: 0x0015A1E8
		public static bool HasComponent<T>(this GameObject obj) where T : Component
		{
			return obj.GetComponent<T>() != null;
		}

		// Token: 0x06003B9B RID: 15259 RVA: 0x0015BFFC File Offset: 0x0015A1FC
		public static void SetChildComponentsEnabled<T>(this GameObject gameObject, bool enabled) where T : MonoBehaviour
		{
			List<T> list = Pool.GetList<T>();
			gameObject.GetComponentsInChildren<T>(true, list);
			foreach (T t in list)
			{
				t.enabled = enabled;
			}
			Pool.FreeList<T>(ref list);
		}
	}
}
