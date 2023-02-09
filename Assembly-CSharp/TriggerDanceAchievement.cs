using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000557 RID: 1367
public class TriggerDanceAchievement : TriggerBase
{
	// Token: 0x060029A7 RID: 10663 RVA: 0x000FCCD7 File Offset: 0x000FAED7
	public void OnPuzzleReset()
	{
		this.Reset();
	}

	// Token: 0x060029A8 RID: 10664 RVA: 0x000FCCDF File Offset: 0x000FAEDF
	public void Reset()
	{
		this.triggeredPlayers.Clear();
	}

	// Token: 0x060029A9 RID: 10665 RVA: 0x000FCCEC File Offset: 0x000FAEEC
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
		if (!(baseEntity is BasePlayer))
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x060029AA RID: 10666 RVA: 0x000FCD3C File Offset: 0x000FAF3C
	public void NotifyDanceStarted()
	{
		if (this.entityContents == null)
		{
			return;
		}
		int num = 0;
		foreach (BaseEntity baseEntity in this.entityContents)
		{
			if (baseEntity.ToPlayer() != null && baseEntity.ToPlayer().CurrentGestureIsDance)
			{
				num++;
				if (num >= this.RequiredPlayerCount)
				{
					break;
				}
			}
		}
		if (num >= this.RequiredPlayerCount)
		{
			foreach (BaseEntity baseEntity2 in this.entityContents)
			{
				if (!this.triggeredPlayers.Contains((ulong)baseEntity2.net.ID) && baseEntity2.ToPlayer() != null)
				{
					baseEntity2.ToPlayer().GiveAchievement(this.AchievementName);
					this.triggeredPlayers.Add((ulong)baseEntity2.net.ID);
				}
			}
		}
	}

	// Token: 0x040021B4 RID: 8628
	public int RequiredPlayerCount = 3;

	// Token: 0x040021B5 RID: 8629
	public string AchievementName;

	// Token: 0x040021B6 RID: 8630
	[NonSerialized]
	private List<ulong> triggeredPlayers = new List<ulong>();
}
