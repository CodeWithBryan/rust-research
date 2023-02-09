using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020004DA RID: 1242
public class EnvironmentManager : SingletonComponent<EnvironmentManager>
{
	// Token: 0x060027A3 RID: 10147 RVA: 0x000F32C0 File Offset: 0x000F14C0
	public static EnvironmentType Get(OBB obb)
	{
		EnvironmentType environmentType = (EnvironmentType)0;
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		GamePhysics.OverlapOBB<EnvironmentVolume>(obb, list, 262144, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			environmentType |= list[i].Type;
		}
		Pool.FreeList<EnvironmentVolume>(ref list);
		return environmentType;
	}

	// Token: 0x060027A4 RID: 10148 RVA: 0x000F330C File Offset: 0x000F150C
	public static EnvironmentType Get(Vector3 pos, ref List<EnvironmentVolume> list)
	{
		EnvironmentType environmentType = (EnvironmentType)0;
		GamePhysics.OverlapSphere<EnvironmentVolume>(pos, 0.01f, list, 262144, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			environmentType |= list[i].Type;
		}
		return environmentType;
	}

	// Token: 0x060027A5 RID: 10149 RVA: 0x000F3354 File Offset: 0x000F1554
	public static EnvironmentType Get(Vector3 pos)
	{
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		EnvironmentType result = EnvironmentManager.Get(pos, ref list);
		Pool.FreeList<EnvironmentVolume>(ref list);
		return result;
	}

	// Token: 0x060027A6 RID: 10150 RVA: 0x000F3376 File Offset: 0x000F1576
	public static bool Check(OBB obb, EnvironmentType type)
	{
		return (EnvironmentManager.Get(obb) & type) > (EnvironmentType)0;
	}

	// Token: 0x060027A7 RID: 10151 RVA: 0x000F3383 File Offset: 0x000F1583
	public static bool Check(Vector3 pos, EnvironmentType type)
	{
		return (EnvironmentManager.Get(pos) & type) > (EnvironmentType)0;
	}
}
