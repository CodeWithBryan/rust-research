using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x020003C3 RID: 963
public abstract class Decay : PrefabAttribute, IServerComponent
{
	// Token: 0x060020E0 RID: 8416 RVA: 0x000D52A4 File Offset: 0x000D34A4
	protected float GetDecayDelay(BuildingGrade.Enum grade)
	{
		if (ConVar.Decay.upkeep)
		{
			if (ConVar.Decay.delay_override > 0f)
			{
				return ConVar.Decay.delay_override;
			}
			switch (grade)
			{
			default:
				return ConVar.Decay.delay_twig * 3600f;
			case BuildingGrade.Enum.Wood:
				return ConVar.Decay.delay_wood * 3600f;
			case BuildingGrade.Enum.Stone:
				return ConVar.Decay.delay_stone * 3600f;
			case BuildingGrade.Enum.Metal:
				return ConVar.Decay.delay_metal * 3600f;
			case BuildingGrade.Enum.TopTier:
				return ConVar.Decay.delay_toptier * 3600f;
			}
		}
		else
		{
			switch (grade)
			{
			default:
				return 3600f;
			case BuildingGrade.Enum.Wood:
				return 64800f;
			case BuildingGrade.Enum.Stone:
				return 64800f;
			case BuildingGrade.Enum.Metal:
				return 64800f;
			case BuildingGrade.Enum.TopTier:
				return 86400f;
			}
		}
	}

	// Token: 0x060020E1 RID: 8417 RVA: 0x000D5358 File Offset: 0x000D3558
	protected float GetDecayDuration(BuildingGrade.Enum grade)
	{
		if (ConVar.Decay.upkeep)
		{
			if (ConVar.Decay.duration_override > 0f)
			{
				return ConVar.Decay.duration_override;
			}
			switch (grade)
			{
			default:
				return ConVar.Decay.duration_twig * 3600f;
			case BuildingGrade.Enum.Wood:
				return ConVar.Decay.duration_wood * 3600f;
			case BuildingGrade.Enum.Stone:
				return ConVar.Decay.duration_stone * 3600f;
			case BuildingGrade.Enum.Metal:
				return ConVar.Decay.duration_metal * 3600f;
			case BuildingGrade.Enum.TopTier:
				return ConVar.Decay.duration_toptier * 3600f;
			}
		}
		else
		{
			switch (grade)
			{
			default:
				return 3600f;
			case BuildingGrade.Enum.Wood:
				return 86400f;
			case BuildingGrade.Enum.Stone:
				return 172800f;
			case BuildingGrade.Enum.Metal:
				return 259200f;
			case BuildingGrade.Enum.TopTier:
				return 432000f;
			}
		}
	}

	// Token: 0x060020E2 RID: 8418 RVA: 0x000D540C File Offset: 0x000D360C
	public static void BuildingDecayTouch(BuildingBlock buildingBlock)
	{
		if (ConVar.Decay.upkeep)
		{
			return;
		}
		List<DecayEntity> list = Facepunch.Pool.GetList<DecayEntity>();
		global::Vis.Entities<DecayEntity>(buildingBlock.transform.position, 40f, list, 2097408, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			DecayEntity decayEntity = list[i];
			BuildingBlock buildingBlock2 = decayEntity as BuildingBlock;
			if (!buildingBlock2 || buildingBlock2.buildingID == buildingBlock.buildingID)
			{
				decayEntity.DecayTouch();
			}
		}
		Facepunch.Pool.FreeList<DecayEntity>(ref list);
	}

	// Token: 0x060020E3 RID: 8419 RVA: 0x000D5486 File Offset: 0x000D3686
	public static void EntityLinkDecayTouch(BaseEntity ent)
	{
		if (ConVar.Decay.upkeep)
		{
			return;
		}
		ent.EntityLinkBroadcast<DecayEntity>(delegate(DecayEntity decayEnt)
		{
			decayEnt.DecayTouch();
		});
	}

	// Token: 0x060020E4 RID: 8420 RVA: 0x000D54B8 File Offset: 0x000D36B8
	public static void RadialDecayTouch(Vector3 pos, float radius, int mask)
	{
		if (ConVar.Decay.upkeep)
		{
			return;
		}
		List<DecayEntity> list = Facepunch.Pool.GetList<DecayEntity>();
		global::Vis.Entities<DecayEntity>(pos, radius, list, mask, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].DecayTouch();
		}
		Facepunch.Pool.FreeList<DecayEntity>(ref list);
	}

	// Token: 0x060020E5 RID: 8421 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool ShouldDecay(BaseEntity entity)
	{
		return true;
	}

	// Token: 0x060020E6 RID: 8422
	public abstract float GetDecayDelay(BaseEntity entity);

	// Token: 0x060020E7 RID: 8423
	public abstract float GetDecayDuration(BaseEntity entity);

	// Token: 0x060020E8 RID: 8424 RVA: 0x000D5501 File Offset: 0x000D3701
	protected override Type GetIndexedType()
	{
		return typeof(global::Decay);
	}

	// Token: 0x04001975 RID: 6517
	private const float hours = 3600f;
}
