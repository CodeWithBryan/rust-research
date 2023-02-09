using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x020002DC RID: 732
public static class GamePhysics
{
	// Token: 0x06001CE7 RID: 7399 RVA: 0x000C5CFA File Offset: 0x000C3EFA
	public static bool CheckSphere(Vector3 position, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
		return UnityEngine.Physics.CheckSphere(position, radius, layerMask, triggerInteraction);
	}

	// Token: 0x06001CE8 RID: 7400 RVA: 0x000C5D0E File Offset: 0x000C3F0E
	public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		layerMask = GamePhysics.HandleTerrainCollision((start + end) * 0.5f, layerMask);
		return UnityEngine.Physics.CheckCapsule(start, end, radius, layerMask, triggerInteraction);
	}

	// Token: 0x06001CE9 RID: 7401 RVA: 0x000C5D34 File Offset: 0x000C3F34
	public static bool CheckOBB(OBB obb, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		layerMask = GamePhysics.HandleTerrainCollision(obb.position, layerMask);
		return UnityEngine.Physics.CheckBox(obb.position, obb.extents, obb.rotation, layerMask, triggerInteraction);
	}

	// Token: 0x06001CEA RID: 7402 RVA: 0x000C5D60 File Offset: 0x000C3F60
	public static bool CheckOBBAndEntity(OBB obb, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal, BaseEntity ignoreEntity = null)
	{
		layerMask = GamePhysics.HandleTerrainCollision(obb.position, layerMask);
		int num = UnityEngine.Physics.OverlapBoxNonAlloc(obb.position, obb.extents, GamePhysics.colBuffer, obb.rotation, layerMask, triggerInteraction);
		for (int i = 0; i < num; i++)
		{
			BaseEntity baseEntity = GamePhysics.colBuffer[i].ToBaseEntity();
			if (!(baseEntity != null) || !(ignoreEntity != null) || (baseEntity.isServer == ignoreEntity.isServer && !(baseEntity == ignoreEntity)))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001CEB RID: 7403 RVA: 0x000C5DDF File Offset: 0x000C3FDF
	public static bool CheckBounds(Bounds bounds, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		layerMask = GamePhysics.HandleTerrainCollision(bounds.center, layerMask);
		return UnityEngine.Physics.CheckBox(bounds.center, bounds.extents, Quaternion.identity, layerMask, triggerInteraction);
	}

	// Token: 0x06001CEC RID: 7404 RVA: 0x000C5E0C File Offset: 0x000C400C
	public static bool CheckInsideNonConvexMesh(Vector3 point, int layerMask = -5)
	{
		bool queriesHitBackfaces = UnityEngine.Physics.queriesHitBackfaces;
		UnityEngine.Physics.queriesHitBackfaces = true;
		int num = UnityEngine.Physics.RaycastNonAlloc(point, Vector3.up, GamePhysics.hitBuffer, 100f, layerMask);
		int num2 = UnityEngine.Physics.RaycastNonAlloc(point, -Vector3.up, GamePhysics.hitBufferB, 100f, layerMask);
		if (num >= GamePhysics.hitBuffer.Length)
		{
			Debug.LogWarning("CheckInsideNonConvexMesh query is exceeding hitBuffer length.");
			return false;
		}
		if (num2 > GamePhysics.hitBufferB.Length)
		{
			Debug.LogWarning("CheckInsideNonConvexMesh query is exceeding hitBufferB length.");
			return false;
		}
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				if (GamePhysics.hitBuffer[i].collider == GamePhysics.hitBufferB[j].collider)
				{
					UnityEngine.Physics.queriesHitBackfaces = queriesHitBackfaces;
					return true;
				}
			}
		}
		UnityEngine.Physics.queriesHitBackfaces = queriesHitBackfaces;
		return false;
	}

	// Token: 0x06001CED RID: 7405 RVA: 0x000C5ED7 File Offset: 0x000C40D7
	public static bool CheckInsideAnyCollider(Vector3 point, int layerMask = -5)
	{
		return UnityEngine.Physics.CheckSphere(point, 0f, layerMask) || GamePhysics.CheckInsideNonConvexMesh(point, layerMask) || (TerrainMeta.HeightMap != null && TerrainMeta.HeightMap.GetHeight(point) > point.y);
	}

	// Token: 0x06001CEE RID: 7406 RVA: 0x000C5F17 File Offset: 0x000C4117
	public static void OverlapSphere(Vector3 position, float radius, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
		GamePhysics.BufferToList(UnityEngine.Physics.OverlapSphereNonAlloc(position, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001CEF RID: 7407 RVA: 0x000C5F37 File Offset: 0x000C4137
	public static void CapsuleSweep(Vector3 position0, Vector3 position1, float radius, Vector3 direction, float distance, List<RaycastHit> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		layerMask = GamePhysics.HandleTerrainCollision(position1, layerMask);
		layerMask = GamePhysics.HandleTerrainCollision(position1, layerMask);
		GamePhysics.HitBufferToList(UnityEngine.Physics.CapsuleCastNonAlloc(position0, position1, radius, direction, GamePhysics.hitBuffer, distance, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001CF0 RID: 7408 RVA: 0x000C5F68 File Offset: 0x000C4168
	public static void OverlapCapsule(Vector3 point0, Vector3 point1, float radius, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		layerMask = GamePhysics.HandleTerrainCollision(point0, layerMask);
		layerMask = GamePhysics.HandleTerrainCollision(point1, layerMask);
		GamePhysics.BufferToList(UnityEngine.Physics.OverlapCapsuleNonAlloc(point0, point1, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001CF1 RID: 7409 RVA: 0x000C5F95 File Offset: 0x000C4195
	public static void OverlapOBB(OBB obb, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		layerMask = GamePhysics.HandleTerrainCollision(obb.position, layerMask);
		GamePhysics.BufferToList(UnityEngine.Physics.OverlapBoxNonAlloc(obb.position, obb.extents, GamePhysics.colBuffer, obb.rotation, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001CF2 RID: 7410 RVA: 0x000C5FC9 File Offset: 0x000C41C9
	public static void OverlapBounds(Bounds bounds, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		layerMask = GamePhysics.HandleTerrainCollision(bounds.center, layerMask);
		GamePhysics.BufferToList(UnityEngine.Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, GamePhysics.colBuffer, Quaternion.identity, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001CF3 RID: 7411 RVA: 0x000C6000 File Offset: 0x000C4200
	private static void BufferToList(int count, List<Collider> list)
	{
		if (count >= GamePhysics.colBuffer.Length)
		{
			Debug.LogWarning("Physics query is exceeding collider buffer length.");
		}
		for (int i = 0; i < count; i++)
		{
			list.Add(GamePhysics.colBuffer[i]);
			GamePhysics.colBuffer[i] = null;
		}
	}

	// Token: 0x06001CF4 RID: 7412 RVA: 0x000C6044 File Offset: 0x000C4244
	public static bool CheckSphere<T>(Vector3 pos, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, radius, list, layerMask, triggerInteraction);
		bool result = GamePhysics.CheckComponent<T>(list);
		Facepunch.Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x06001CF5 RID: 7413 RVA: 0x000C6070 File Offset: 0x000C4270
	public static bool CheckCapsule<T>(Vector3 start, Vector3 end, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapCapsule(start, end, radius, list, layerMask, triggerInteraction);
		bool result = GamePhysics.CheckComponent<T>(list);
		Facepunch.Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x06001CF6 RID: 7414 RVA: 0x000C609C File Offset: 0x000C429C
	public static bool CheckOBB<T>(OBB obb, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapOBB(obb, list, layerMask, triggerInteraction);
		bool result = GamePhysics.CheckComponent<T>(list);
		Facepunch.Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x06001CF7 RID: 7415 RVA: 0x000C60C8 File Offset: 0x000C42C8
	public static bool CheckBounds<T>(Bounds bounds, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapBounds(bounds, list, layerMask, triggerInteraction);
		bool result = GamePhysics.CheckComponent<T>(list);
		Facepunch.Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x06001CF8 RID: 7416 RVA: 0x000C60F4 File Offset: 0x000C42F4
	private static bool CheckComponent<T>(List<Collider> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].gameObject.GetComponent<T>() != null)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001CF9 RID: 7417 RVA: 0x000C612D File Offset: 0x000C432D
	public static void OverlapSphere<T>(Vector3 position, float radius, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
		GamePhysics.BufferToList<T>(UnityEngine.Physics.OverlapSphereNonAlloc(position, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001CFA RID: 7418 RVA: 0x000C614D File Offset: 0x000C434D
	public static void OverlapCapsule<T>(Vector3 point0, Vector3 point1, float radius, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		layerMask = GamePhysics.HandleTerrainCollision(point0, layerMask);
		layerMask = GamePhysics.HandleTerrainCollision(point1, layerMask);
		GamePhysics.BufferToList<T>(UnityEngine.Physics.OverlapCapsuleNonAlloc(point0, point1, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001CFB RID: 7419 RVA: 0x000C617A File Offset: 0x000C437A
	public static void OverlapOBB<T>(OBB obb, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		layerMask = GamePhysics.HandleTerrainCollision(obb.position, layerMask);
		GamePhysics.BufferToList<T>(UnityEngine.Physics.OverlapBoxNonAlloc(obb.position, obb.extents, GamePhysics.colBuffer, obb.rotation, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001CFC RID: 7420 RVA: 0x000C61AE File Offset: 0x000C43AE
	public static void OverlapBounds<T>(Bounds bounds, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		layerMask = GamePhysics.HandleTerrainCollision(bounds.center, layerMask);
		GamePhysics.BufferToList<T>(UnityEngine.Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, GamePhysics.colBuffer, Quaternion.identity, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001CFD RID: 7421 RVA: 0x000C61E4 File Offset: 0x000C43E4
	private static void BufferToList<T>(int count, List<T> list) where T : Component
	{
		if (count >= GamePhysics.colBuffer.Length)
		{
			Debug.LogWarning("Physics query is exceeding collider buffer length.");
		}
		for (int i = 0; i < count; i++)
		{
			T component = GamePhysics.colBuffer[i].gameObject.GetComponent<T>();
			if (component)
			{
				list.Add(component);
			}
			GamePhysics.colBuffer[i] = null;
		}
	}

	// Token: 0x06001CFE RID: 7422 RVA: 0x000C6240 File Offset: 0x000C4440
	private static void HitBufferToList(int count, List<RaycastHit> list)
	{
		if (count >= GamePhysics.hitBuffer.Length)
		{
			Debug.LogWarning("Physics query is exceeding collider buffer length.");
		}
		for (int i = 0; i < count; i++)
		{
			list.Add(GamePhysics.hitBuffer[i]);
		}
	}

	// Token: 0x06001CFF RID: 7423 RVA: 0x000C6280 File Offset: 0x000C4480
	public static bool Trace(Ray ray, float radius, out RaycastHit hitInfo, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal, BaseEntity ignoreEntity = null)
	{
		List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
		GamePhysics.TraceAllUnordered(ray, radius, list, maxDistance, layerMask, triggerInteraction, ignoreEntity);
		if (list.Count == 0)
		{
			hitInfo = default(RaycastHit);
			Facepunch.Pool.FreeList<RaycastHit>(ref list);
			return false;
		}
		GamePhysics.Sort(list);
		hitInfo = list[0];
		Facepunch.Pool.FreeList<RaycastHit>(ref list);
		return true;
	}

	// Token: 0x06001D00 RID: 7424 RVA: 0x000C62D5 File Offset: 0x000C44D5
	public static void TraceAll(Ray ray, float radius, List<RaycastHit> hits, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal, BaseEntity ignoreEntity = null)
	{
		GamePhysics.TraceAllUnordered(ray, radius, hits, maxDistance, layerMask, triggerInteraction, ignoreEntity);
		GamePhysics.Sort(hits);
	}

	// Token: 0x06001D01 RID: 7425 RVA: 0x000C62EC File Offset: 0x000C44EC
	public static void TraceAllUnordered(Ray ray, float radius, List<RaycastHit> hits, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal, BaseEntity ignoreEntity = null)
	{
		int num;
		if (radius == 0f)
		{
			num = UnityEngine.Physics.RaycastNonAlloc(ray, GamePhysics.hitBuffer, maxDistance, layerMask, triggerInteraction);
		}
		else
		{
			num = UnityEngine.Physics.SphereCastNonAlloc(ray, radius, GamePhysics.hitBuffer, maxDistance, layerMask, triggerInteraction);
		}
		if (num == 0)
		{
			return;
		}
		if (num >= GamePhysics.hitBuffer.Length)
		{
			Debug.LogWarning("Physics query is exceeding hit buffer length.");
		}
		for (int i = 0; i < num; i++)
		{
			RaycastHit raycastHit = GamePhysics.hitBuffer[i];
			if (GamePhysics.Verify(raycastHit, ignoreEntity))
			{
				hits.Add(raycastHit);
			}
		}
	}

	// Token: 0x06001D02 RID: 7426 RVA: 0x000C6369 File Offset: 0x000C4569
	public static bool LineOfSightRadius(Vector3 p0, Vector3 p1, int layerMask, float radius, float padding0, float padding1, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.LineOfSightInternal(p0, p1, layerMask, radius, padding0, padding1, ignoreEntity);
	}

	// Token: 0x06001D03 RID: 7427 RVA: 0x000C637A File Offset: 0x000C457A
	public static bool LineOfSightRadius(Vector3 p0, Vector3 p1, int layerMask, float radius, float padding, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.LineOfSightInternal(p0, p1, layerMask, radius, padding, padding, ignoreEntity);
	}

	// Token: 0x06001D04 RID: 7428 RVA: 0x000C638B File Offset: 0x000C458B
	public static bool LineOfSightRadius(Vector3 p0, Vector3 p1, int layerMask, float radius, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.LineOfSightInternal(p0, p1, layerMask, radius, 0f, 0f, ignoreEntity);
	}

	// Token: 0x06001D05 RID: 7429 RVA: 0x000C63A2 File Offset: 0x000C45A2
	public static bool LineOfSight(Vector3 p0, Vector3 p1, int layerMask, float padding0, float padding1, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.LineOfSightRadius(p0, p1, layerMask, 0f, padding0, padding1, ignoreEntity);
	}

	// Token: 0x06001D06 RID: 7430 RVA: 0x000C63B6 File Offset: 0x000C45B6
	public static bool LineOfSight(Vector3 p0, Vector3 p1, int layerMask, float padding, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.LineOfSightRadius(p0, p1, layerMask, 0f, padding, padding, ignoreEntity);
	}

	// Token: 0x06001D07 RID: 7431 RVA: 0x000C63C9 File Offset: 0x000C45C9
	public static bool LineOfSight(Vector3 p0, Vector3 p1, int layerMask, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.LineOfSightRadius(p0, p1, layerMask, 0f, 0f, 0f, ignoreEntity);
	}

	// Token: 0x06001D08 RID: 7432 RVA: 0x000C63E4 File Offset: 0x000C45E4
	private static bool LineOfSightInternal(Vector3 p0, Vector3 p1, int layerMask, float radius, float padding0, float padding1, BaseEntity ignoreEntity = null)
	{
		if (!ValidBounds.Test(p0))
		{
			return false;
		}
		if (!ValidBounds.Test(p1))
		{
			return false;
		}
		Vector3 a = p1 - p0;
		float magnitude = a.magnitude;
		if (magnitude <= padding0 + padding1)
		{
			return true;
		}
		Vector3 vector = a / magnitude;
		Ray ray = new Ray(p0 + vector * padding0, vector);
		float maxDistance = magnitude - padding0 - padding1;
		RaycastHit raycastHit;
		bool flag;
		if (!ignoreEntity.IsRealNull() || (layerMask & 8388608) != 0)
		{
			flag = GamePhysics.Trace(ray, 0f, out raycastHit, maxDistance, layerMask, QueryTriggerInteraction.Ignore, ignoreEntity);
			if (radius > 0f && !flag)
			{
				flag = GamePhysics.Trace(ray, radius, out raycastHit, maxDistance, layerMask, QueryTriggerInteraction.Ignore, ignoreEntity);
			}
		}
		else
		{
			flag = UnityEngine.Physics.Raycast(ray, out raycastHit, maxDistance, layerMask, QueryTriggerInteraction.Ignore);
			if (radius > 0f && !flag)
			{
				flag = UnityEngine.Physics.SphereCast(ray, radius, out raycastHit, maxDistance, layerMask, QueryTriggerInteraction.Ignore);
			}
		}
		if (!flag)
		{
			if (ConVar.Vis.lineofsight)
			{
				ConsoleNetwork.BroadcastToAllClients("ddraw.line", new object[]
				{
					60f,
					Color.green,
					p0,
					p1
				});
			}
			return true;
		}
		if (ConVar.Vis.lineofsight)
		{
			ConsoleNetwork.BroadcastToAllClients("ddraw.line", new object[]
			{
				60f,
				Color.red,
				p0,
				p1
			});
			ConsoleNetwork.BroadcastToAllClients("ddraw.text", new object[]
			{
				60f,
				Color.white,
				raycastHit.point,
				raycastHit.collider.name
			});
		}
		return false;
	}

	// Token: 0x06001D09 RID: 7433 RVA: 0x000C6593 File Offset: 0x000C4793
	public static bool Verify(RaycastHit hitInfo, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.Verify(hitInfo.collider, hitInfo.point, ignoreEntity);
	}

	// Token: 0x06001D0A RID: 7434 RVA: 0x000C65A9 File Offset: 0x000C47A9
	public static bool Verify(Collider collider, Vector3 point, BaseEntity ignoreEntity = null)
	{
		return (!(collider is TerrainCollider) || !TerrainMeta.Collision || !TerrainMeta.Collision.GetIgnore(point, 0.01f)) && !GamePhysics.CompareEntity(collider.ToBaseEntity(), ignoreEntity) && collider.enabled;
	}

	// Token: 0x06001D0B RID: 7435 RVA: 0x000C65E9 File Offset: 0x000C47E9
	private static bool CompareEntity(BaseEntity a, BaseEntity b)
	{
		return !a.IsRealNull() && !b.IsRealNull() && a == b;
	}

	// Token: 0x06001D0C RID: 7436 RVA: 0x000C660C File Offset: 0x000C480C
	public static int HandleTerrainCollision(Vector3 position, int layerMask)
	{
		int num = 8388608;
		if ((layerMask & num) != 0 && TerrainMeta.Collision && TerrainMeta.Collision.GetIgnore(position, 0.01f))
		{
			layerMask &= ~num;
		}
		return layerMask;
	}

	// Token: 0x06001D0D RID: 7437 RVA: 0x000C6649 File Offset: 0x000C4849
	public static void Sort(List<RaycastHit> hits)
	{
		hits.Sort((RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance));
	}

	// Token: 0x06001D0E RID: 7438 RVA: 0x000C6670 File Offset: 0x000C4870
	public static void Sort(RaycastHit[] hits)
	{
		Array.Sort<RaycastHit>(hits, (RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance));
	}

	// Token: 0x04001696 RID: 5782
	public const int BufferLength = 8192;

	// Token: 0x04001697 RID: 5783
	private static RaycastHit[] hitBuffer = new RaycastHit[8192];

	// Token: 0x04001698 RID: 5784
	private static RaycastHit[] hitBufferB = new RaycastHit[8192];

	// Token: 0x04001699 RID: 5785
	private static Collider[] colBuffer = new Collider[8192];
}
