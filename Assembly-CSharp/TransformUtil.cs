using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

// Token: 0x02000921 RID: 2337
public static class TransformUtil
{
	// Token: 0x060037AC RID: 14252 RVA: 0x0014A219 File Offset: 0x00148419
	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hit, Transform ignoreTransform = null)
	{
		return TransformUtil.GetGroundInfo(startPos, out hit, 100f, -1, ignoreTransform);
	}

	// Token: 0x060037AD RID: 14253 RVA: 0x0014A22E File Offset: 0x0014842E
	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hit, float range, Transform ignoreTransform = null)
	{
		return TransformUtil.GetGroundInfo(startPos, out hit, range, -1, ignoreTransform);
	}

	// Token: 0x060037AE RID: 14254 RVA: 0x0014A240 File Offset: 0x00148440
	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hitOut, float range, LayerMask mask, Transform ignoreTransform = null)
	{
		startPos.y += 0.25f;
		range += 0.25f;
		hitOut = default(RaycastHit);
		RaycastHit raycastHit;
		if (!Physics.Raycast(new Ray(startPos, Vector3.down), out raycastHit, range, mask))
		{
			return false;
		}
		if (ignoreTransform != null && (raycastHit.collider.transform == ignoreTransform || raycastHit.collider.transform.IsChildOf(ignoreTransform)))
		{
			return TransformUtil.GetGroundInfo(startPos - new Vector3(0f, 0.01f, 0f), out hitOut, range, mask, ignoreTransform);
		}
		hitOut = raycastHit;
		return true;
	}

	// Token: 0x060037AF RID: 14255 RVA: 0x0014A2EB File Offset: 0x001484EB
	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, Transform ignoreTransform = null)
	{
		return TransformUtil.GetGroundInfo(startPos, out pos, out normal, 100f, -1, ignoreTransform);
	}

	// Token: 0x060037B0 RID: 14256 RVA: 0x0014A301 File Offset: 0x00148501
	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, Transform ignoreTransform = null)
	{
		return TransformUtil.GetGroundInfo(startPos, out pos, out normal, range, -1, ignoreTransform);
	}

	// Token: 0x060037B1 RID: 14257 RVA: 0x0014A314 File Offset: 0x00148514
	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, LayerMask mask, Transform ignoreTransform = null)
	{
		startPos.y += 0.25f;
		range += 0.25f;
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(startPos, Vector3.down), 0f, list, range, mask, QueryTriggerInteraction.Ignore, null);
		foreach (RaycastHit raycastHit in list)
		{
			if (!(ignoreTransform != null) || (!(raycastHit.collider.transform == ignoreTransform) && !raycastHit.collider.transform.IsChildOf(ignoreTransform)))
			{
				pos = raycastHit.point;
				normal = raycastHit.normal;
				Pool.FreeList<RaycastHit>(ref list);
				return true;
			}
		}
		pos = startPos;
		normal = Vector3.up;
		Pool.FreeList<RaycastHit>(ref list);
		return false;
	}

	// Token: 0x060037B2 RID: 14258 RVA: 0x0014A410 File Offset: 0x00148610
	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal)
	{
		return TransformUtil.GetGroundInfoTerrainOnly(startPos, out pos, out normal, 100f, -1);
	}

	// Token: 0x060037B3 RID: 14259 RVA: 0x0014A425 File Offset: 0x00148625
	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range)
	{
		return TransformUtil.GetGroundInfoTerrainOnly(startPos, out pos, out normal, range, -1);
	}

	// Token: 0x060037B4 RID: 14260 RVA: 0x0014A438 File Offset: 0x00148638
	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, LayerMask mask)
	{
		startPos.y += 0.25f;
		range += 0.25f;
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(startPos, Vector3.down), out raycastHit, range, mask) && raycastHit.collider is TerrainCollider)
		{
			pos = raycastHit.point;
			normal = raycastHit.normal;
			return true;
		}
		pos = startPos;
		normal = Vector3.up;
		return false;
	}

	// Token: 0x060037B5 RID: 14261 RVA: 0x0014A4B7 File Offset: 0x001486B7
	public static Transform[] GetRootObjects()
	{
		return (from x in UnityEngine.Object.FindObjectsOfType<Transform>()
		where x.transform == x.transform.root
		select x).ToArray<Transform>();
	}
}
