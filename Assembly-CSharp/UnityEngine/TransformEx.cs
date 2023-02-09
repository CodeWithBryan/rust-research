using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;

namespace UnityEngine
{
	// Token: 0x020009E8 RID: 2536
	public static class TransformEx
	{
		// Token: 0x06003BA8 RID: 15272 RVA: 0x0015C2CC File Offset: 0x0015A4CC
		public static string GetRecursiveName(this Transform transform, string strEndName = "")
		{
			string text = transform.name;
			if (!string.IsNullOrEmpty(strEndName))
			{
				text = text + "/" + strEndName;
			}
			if (transform.parent != null)
			{
				text = transform.parent.GetRecursiveName(text);
			}
			return text;
		}

		// Token: 0x06003BA9 RID: 15273 RVA: 0x0015C314 File Offset: 0x0015A514
		public static void RemoveComponent<T>(this Transform transform) where T : Component
		{
			T component = transform.GetComponent<T>();
			if (component == null)
			{
				return;
			}
			GameManager.Destroy(component, 0f);
		}

		// Token: 0x06003BAA RID: 15274 RVA: 0x0015C348 File Offset: 0x0015A548
		public static void RetireAllChildren(this Transform transform, GameManager gameManager)
		{
			List<GameObject> list = Pool.GetList<GameObject>();
			foreach (object obj in transform)
			{
				Transform transform2 = (Transform)obj;
				if (!transform2.CompareTag("persist"))
				{
					list.Add(transform2.gameObject);
				}
			}
			foreach (GameObject instance in list)
			{
				gameManager.Retire(instance);
			}
			Pool.FreeList<GameObject>(ref list);
		}

		// Token: 0x06003BAB RID: 15275 RVA: 0x0015C3FC File Offset: 0x0015A5FC
		public static List<Transform> GetChildren(this Transform transform)
		{
			return transform.Cast<Transform>().ToList<Transform>();
		}

		// Token: 0x06003BAC RID: 15276 RVA: 0x0015C40C File Offset: 0x0015A60C
		public static void OrderChildren(this Transform tx, Func<Transform, object> selector)
		{
			foreach (Transform transform in tx.Cast<Transform>().OrderBy(selector))
			{
				transform.SetAsLastSibling();
			}
		}

		// Token: 0x06003BAD RID: 15277 RVA: 0x0015C45C File Offset: 0x0015A65C
		public static List<Transform> GetAllChildren(this Transform transform)
		{
			List<Transform> list = new List<Transform>();
			if (transform != null)
			{
				transform.AddAllChildren(list);
			}
			return list;
		}

		// Token: 0x06003BAE RID: 15278 RVA: 0x0015C480 File Offset: 0x0015A680
		public static void AddAllChildren(this Transform transform, List<Transform> list)
		{
			list.Add(transform);
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (!(child == null))
				{
					child.AddAllChildren(list);
				}
			}
		}

		// Token: 0x06003BAF RID: 15279 RVA: 0x0015C4C0 File Offset: 0x0015A6C0
		public static Transform[] GetChildrenWithTag(this Transform transform, string strTag)
		{
			return (from x in transform.GetAllChildren()
			where x.CompareTag(strTag)
			select x).ToArray<Transform>();
		}

		// Token: 0x06003BB0 RID: 15280 RVA: 0x0015C4F6 File Offset: 0x0015A6F6
		public static void Identity(this GameObject go)
		{
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
		}

		// Token: 0x06003BB1 RID: 15281 RVA: 0x0015C528 File Offset: 0x0015A728
		public static GameObject CreateChild(this GameObject go)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = go.transform;
			gameObject.Identity();
			return gameObject;
		}

		// Token: 0x06003BB2 RID: 15282 RVA: 0x0015C546 File Offset: 0x0015A746
		public static GameObject InstantiateChild(this GameObject go, GameObject prefab)
		{
			GameObject gameObject = Instantiate.GameObject(prefab, null);
			gameObject.transform.SetParent(go.transform, false);
			gameObject.Identity();
			return gameObject;
		}

		// Token: 0x06003BB3 RID: 15283 RVA: 0x0015C568 File Offset: 0x0015A768
		public static void SetLayerRecursive(this GameObject go, int Layer)
		{
			if (go.layer != Layer)
			{
				go.layer = Layer;
			}
			for (int i = 0; i < go.transform.childCount; i++)
			{
				go.transform.GetChild(i).gameObject.SetLayerRecursive(Layer);
			}
		}

		// Token: 0x06003BB4 RID: 15284 RVA: 0x0015C5B4 File Offset: 0x0015A7B4
		public static bool DropToGround(this Transform transform, bool alignToNormal = false, float fRange = 100f)
		{
			Vector3 position;
			Vector3 upwards;
			if (transform.GetGroundInfo(out position, out upwards, fRange))
			{
				transform.position = position;
				if (alignToNormal)
				{
					transform.rotation = Quaternion.LookRotation(transform.forward, upwards);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06003BB5 RID: 15285 RVA: 0x0015C5ED File Offset: 0x0015A7ED
		public static bool GetGroundInfo(this Transform transform, out Vector3 pos, out Vector3 normal, float range = 100f)
		{
			return TransformUtil.GetGroundInfo(transform.position, out pos, out normal, range, transform);
		}

		// Token: 0x06003BB6 RID: 15286 RVA: 0x0015C5FE File Offset: 0x0015A7FE
		public static bool GetGroundInfoTerrainOnly(this Transform transform, out Vector3 pos, out Vector3 normal, float range = 100f)
		{
			return TransformUtil.GetGroundInfoTerrainOnly(transform.position, out pos, out normal, range);
		}

		// Token: 0x06003BB7 RID: 15287 RVA: 0x0015C610 File Offset: 0x0015A810
		public static Bounds WorkoutRenderBounds(this Transform tx)
		{
			Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
			foreach (Renderer renderer in tx.GetComponentsInChildren<Renderer>())
			{
				if (!(renderer is ParticleSystemRenderer))
				{
					if (bounds.center == Vector3.zero)
					{
						bounds = renderer.bounds;
					}
					else
					{
						bounds.Encapsulate(renderer.bounds);
					}
				}
			}
			return bounds;
		}

		// Token: 0x06003BB8 RID: 15288 RVA: 0x0015C67C File Offset: 0x0015A87C
		public static List<T> GetSiblings<T>(this Transform transform, bool includeSelf = false)
		{
			List<T> list = new List<T>();
			if (transform.parent == null)
			{
				return list;
			}
			for (int i = 0; i < transform.parent.childCount; i++)
			{
				Transform child = transform.parent.GetChild(i);
				if (includeSelf || !(child == transform))
				{
					T component = child.GetComponent<T>();
					if (component != null)
					{
						list.Add(component);
					}
				}
			}
			return list;
		}

		// Token: 0x06003BB9 RID: 15289 RVA: 0x0015C6E8 File Offset: 0x0015A8E8
		public static void DestroyChildren(this Transform transform)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				GameManager.Destroy(transform.GetChild(i).gameObject, 0f);
			}
		}

		// Token: 0x06003BBA RID: 15290 RVA: 0x0015C71C File Offset: 0x0015A91C
		public static void SetChildrenActive(this Transform transform, bool b)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.SetActive(b);
			}
		}

		// Token: 0x06003BBB RID: 15291 RVA: 0x0015C74C File Offset: 0x0015A94C
		public static Transform ActiveChild(this Transform transform, string name, bool bDisableOthers)
		{
			Transform result = null;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child.name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					result = child;
					child.gameObject.SetActive(true);
				}
				else if (bDisableOthers)
				{
					child.gameObject.SetActive(false);
				}
			}
			return result;
		}

		// Token: 0x06003BBC RID: 15292 RVA: 0x0015C7A4 File Offset: 0x0015A9A4
		public static T GetComponentInChildrenIncludeDisabled<T>(this Transform transform) where T : Component
		{
			List<T> list = Pool.GetList<T>();
			transform.GetComponentsInChildren<T>(true, list);
			T result = (list.Count > 0) ? list[0] : default(T);
			Pool.FreeList<T>(ref list);
			return result;
		}

		// Token: 0x06003BBD RID: 15293 RVA: 0x0015C7E4 File Offset: 0x0015A9E4
		public static bool HasComponentInChildrenIncludeDisabled<T>(this Transform transform) where T : Component
		{
			List<T> list = Pool.GetList<T>();
			transform.GetComponentsInChildren<T>(true, list);
			bool result = list.Count > 0;
			Pool.FreeList<T>(ref list);
			return result;
		}

		// Token: 0x06003BBE RID: 15294 RVA: 0x0015C80F File Offset: 0x0015AA0F
		public static void SetHierarchyGroup(this Transform transform, string strRoot, bool groupActive = true, bool persistant = false)
		{
			transform.SetParent(HierarchyUtil.GetRoot(strRoot, groupActive, persistant).transform, true);
		}

		// Token: 0x06003BBF RID: 15295 RVA: 0x0015C828 File Offset: 0x0015AA28
		public static Bounds GetBounds(this Transform transform, bool includeRenderers = true, bool includeColliders = true, bool includeInactive = true)
		{
			Bounds result = new Bounds(Vector3.zero, Vector3.zero);
			if (includeRenderers)
			{
				foreach (MeshFilter meshFilter in transform.GetComponentsInChildren<MeshFilter>(includeInactive))
				{
					if (meshFilter.sharedMesh)
					{
						Matrix4x4 matrix = transform.worldToLocalMatrix * meshFilter.transform.localToWorldMatrix;
						Bounds bounds = meshFilter.sharedMesh.bounds;
						result.Encapsulate(bounds.Transform(matrix));
					}
				}
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in transform.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive))
				{
					if (skinnedMeshRenderer.sharedMesh)
					{
						Matrix4x4 matrix2 = transform.worldToLocalMatrix * skinnedMeshRenderer.transform.localToWorldMatrix;
						Bounds bounds2 = skinnedMeshRenderer.sharedMesh.bounds;
						result.Encapsulate(bounds2.Transform(matrix2));
					}
				}
			}
			if (includeColliders)
			{
				foreach (MeshCollider meshCollider in transform.GetComponentsInChildren<MeshCollider>(includeInactive))
				{
					if (meshCollider.sharedMesh && !meshCollider.isTrigger)
					{
						Matrix4x4 matrix3 = transform.worldToLocalMatrix * meshCollider.transform.localToWorldMatrix;
						Bounds bounds3 = meshCollider.sharedMesh.bounds;
						result.Encapsulate(bounds3.Transform(matrix3));
					}
				}
			}
			return result;
		}
	}
}
