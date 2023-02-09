using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000574 RID: 1396
public static class Vis
{
	// Token: 0x06002A2D RID: 10797 RVA: 0x000FEDBC File Offset: 0x000FCFBC
	private static void Buffer(Vector3 position, float radius, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide)
	{
		layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
		int num = Vis.colCount;
		Vis.colCount = Physics.OverlapSphereNonAlloc(position, radius, Vis.colBuffer, layerMask, triggerInteraction);
		for (int i = Vis.colCount; i < num; i++)
		{
			Vis.colBuffer[i] = null;
		}
		if (Vis.colCount >= Vis.colBuffer.Length)
		{
			Debug.LogWarning("Vis query is exceeding collider buffer length.");
			Vis.colCount = Vis.colBuffer.Length;
		}
	}

	// Token: 0x06002A2E RID: 10798 RVA: 0x000FEE27 File Offset: 0x000FD027
	public static bool AnyColliders(Vector3 position, float radius, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		Vis.Buffer(position, radius, layerMask, triggerInteraction);
		return Vis.colCount > 0;
	}

	// Token: 0x06002A2F RID: 10799 RVA: 0x000FEE3C File Offset: 0x000FD03C
	public static void Colliders<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : Collider
	{
		Vis.Buffer(position, radius, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			T t = Vis.colBuffer[i] as T;
			if (!(t == null) && t.enabled)
			{
				list.Add(t);
			}
		}
	}

	// Token: 0x06002A30 RID: 10800 RVA: 0x000FEE98 File Offset: 0x000FD098
	public static void Components<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : Component
	{
		Vis.Buffer(position, radius, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				T component = collider.GetComponent<T>();
				if (!(component == null))
				{
					list.Add(component);
				}
			}
		}
	}

	// Token: 0x06002A31 RID: 10801 RVA: 0x000FEEF4 File Offset: 0x000FD0F4
	public static void Entities<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : class
	{
		Vis.Buffer(position, radius, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				T t = collider.ToBaseEntity() as T;
				if (t != null)
				{
					list.Add(t);
				}
			}
		}
	}

	// Token: 0x06002A32 RID: 10802 RVA: 0x000FEF54 File Offset: 0x000FD154
	public static void EntityComponents<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : EntityComponentBase
	{
		Vis.Buffer(position, radius, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				BaseEntity baseEntity = collider.ToBaseEntity();
				if (!(baseEntity == null))
				{
					T component = baseEntity.GetComponent<T>();
					if (!(component == null))
					{
						list.Add(component);
					}
				}
			}
		}
	}

	// Token: 0x06002A33 RID: 10803 RVA: 0x000FEFC0 File Offset: 0x000FD1C0
	private static void Buffer(OBB bounds, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide)
	{
		layerMask = GamePhysics.HandleTerrainCollision(bounds.position, layerMask);
		int num = Vis.colCount;
		Vis.colCount = Physics.OverlapBoxNonAlloc(bounds.position, bounds.extents, Vis.colBuffer, bounds.rotation, layerMask, triggerInteraction);
		for (int i = Vis.colCount; i < num; i++)
		{
			Vis.colBuffer[i] = null;
		}
		if (Vis.colCount >= Vis.colBuffer.Length)
		{
			Debug.LogWarning("Vis query is exceeding collider buffer length.");
			Vis.colCount = Vis.colBuffer.Length;
		}
	}

	// Token: 0x06002A34 RID: 10804 RVA: 0x000FF040 File Offset: 0x000FD240
	public static void Colliders<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : Collider
	{
		Vis.Buffer(bounds, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			T t = Vis.colBuffer[i] as T;
			if (!(t == null) && t.enabled)
			{
				list.Add(t);
			}
		}
	}

	// Token: 0x06002A35 RID: 10805 RVA: 0x000FF09C File Offset: 0x000FD29C
	public static void Components<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : Component
	{
		Vis.Buffer(bounds, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				T component = collider.GetComponent<T>();
				if (!(component == null))
				{
					list.Add(component);
				}
			}
		}
	}

	// Token: 0x06002A36 RID: 10806 RVA: 0x000FF0F8 File Offset: 0x000FD2F8
	public static void Entities<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : BaseEntity
	{
		Vis.Buffer(bounds, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				T t = collider.ToBaseEntity() as T;
				if (!(t == null))
				{
					list.Add(t);
				}
			}
		}
	}

	// Token: 0x06002A37 RID: 10807 RVA: 0x000FF15C File Offset: 0x000FD35C
	public static void EntityComponents<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : EntityComponentBase
	{
		Vis.Buffer(bounds, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				BaseEntity baseEntity = collider.ToBaseEntity();
				if (!(baseEntity == null))
				{
					T component = baseEntity.GetComponent<T>();
					if (!(component == null))
					{
						list.Add(component);
					}
				}
			}
		}
	}

	// Token: 0x06002A38 RID: 10808 RVA: 0x000FF1C8 File Offset: 0x000FD3C8
	private static void Buffer(Vector3 startPosition, Vector3 endPosition, float radius, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide)
	{
		layerMask = GamePhysics.HandleTerrainCollision(startPosition, layerMask);
		int num = Vis.colCount;
		Vis.colCount = Physics.OverlapCapsuleNonAlloc(startPosition, endPosition, radius, Vis.colBuffer, layerMask, triggerInteraction);
		for (int i = Vis.colCount; i < num; i++)
		{
			Vis.colBuffer[i] = null;
		}
		if (Vis.colCount >= Vis.colBuffer.Length)
		{
			Debug.LogWarning("Vis query is exceeding collider buffer length.");
			Vis.colCount = Vis.colBuffer.Length;
		}
	}

	// Token: 0x06002A39 RID: 10809 RVA: 0x000FF238 File Offset: 0x000FD438
	public static void Entities<T>(Vector3 startPosition, Vector3 endPosition, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : BaseEntity
	{
		Vis.Buffer(startPosition, endPosition, radius, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				T t = collider.ToBaseEntity() as T;
				if (!(t == null))
				{
					list.Add(t);
				}
			}
		}
	}

	// Token: 0x0400220B RID: 8715
	private static int colCount = 0;

	// Token: 0x0400220C RID: 8716
	private static Collider[] colBuffer = new Collider[8192];
}
