using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000556 RID: 1366
public class TriggerComfort : TriggerBase
{
	// Token: 0x060029A1 RID: 10657 RVA: 0x000FCB0C File Offset: 0x000FAD0C
	private void OnValidate()
	{
		this.triggerSize = base.GetComponent<SphereCollider>().radius * base.transform.localScale.y;
	}

	// Token: 0x060029A2 RID: 10658 RVA: 0x000FCB30 File Offset: 0x000FAD30
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x060029A3 RID: 10659 RVA: 0x000FCB74 File Offset: 0x000FAD74
	public float CalculateComfort(Vector3 position, BasePlayer forPlayer = null)
	{
		float num = Vector3.Distance(base.gameObject.transform.position, position);
		float num2 = 1f - Mathf.Clamp(num - this.minComfortRange, 0f, num / (this.triggerSize - this.minComfortRange));
		float num3 = 0f;
		foreach (BasePlayer basePlayer in this._players)
		{
			if (!(basePlayer == forPlayer))
			{
				num3 += 0.25f * (basePlayer.IsSleeping() ? 0.5f : 1f) * (basePlayer.IsAlive() ? 1f : 0f);
			}
		}
		float num4 = 0f + num3;
		return (this.baseComfort + num4) * num2;
	}

	// Token: 0x060029A4 RID: 10660 RVA: 0x000FCC58 File Offset: 0x000FAE58
	internal override void OnEntityEnter(BaseEntity ent)
	{
		BasePlayer basePlayer = ent as BasePlayer;
		if (!basePlayer)
		{
			return;
		}
		this._players.Add(basePlayer);
	}

	// Token: 0x060029A5 RID: 10661 RVA: 0x000FCC84 File Offset: 0x000FAE84
	internal override void OnEntityLeave(BaseEntity ent)
	{
		BasePlayer basePlayer = ent as BasePlayer;
		if (!basePlayer)
		{
			return;
		}
		this._players.Remove(basePlayer);
	}

	// Token: 0x040021AE RID: 8622
	public float triggerSize;

	// Token: 0x040021AF RID: 8623
	public float baseComfort = 0.5f;

	// Token: 0x040021B0 RID: 8624
	public float minComfortRange = 2.5f;

	// Token: 0x040021B1 RID: 8625
	private const float perPlayerComfortBonus = 0.25f;

	// Token: 0x040021B2 RID: 8626
	private const float bonusComfort = 0f;

	// Token: 0x040021B3 RID: 8627
	private List<BasePlayer> _players = new List<BasePlayer>();
}
