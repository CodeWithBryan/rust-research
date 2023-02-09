using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000570 RID: 1392
public class TriggerWakeAIZ : TriggerBase, IServerComponent
{
	// Token: 0x06002A1A RID: 10778 RVA: 0x000FE988 File Offset: 0x000FCB88
	public void Init(AIInformationZone zone = null)
	{
		if (zone != null)
		{
			this.aiz = zone;
		}
		else if (this.zones == null || this.zones.Count == 0)
		{
			Transform transform = base.transform.parent;
			if (transform == null)
			{
				transform = base.transform;
			}
			this.aiz = transform.GetComponentInChildren<AIInformationZone>();
		}
		this.SetZonesSleeping(true);
	}

	// Token: 0x06002A1B RID: 10779 RVA: 0x000FE9EB File Offset: 0x000FCBEB
	private void Awake()
	{
		this.Init(null);
	}

	// Token: 0x06002A1C RID: 10780 RVA: 0x000FE9F4 File Offset: 0x000FCBF4
	private void SetZonesSleeping(bool flag)
	{
		if (this.aiz != null)
		{
			if (flag)
			{
				this.aiz.SleepAI();
			}
			else
			{
				this.aiz.WakeAI();
			}
		}
		if (this.zones != null && this.zones.Count > 0)
		{
			foreach (AIInformationZone aiinformationZone in this.zones)
			{
				if (aiinformationZone != null)
				{
					if (flag)
					{
						aiinformationZone.SleepAI();
					}
					else
					{
						aiinformationZone.WakeAI();
					}
				}
			}
		}
	}

	// Token: 0x06002A1D RID: 10781 RVA: 0x000FEA9C File Offset: 0x000FCC9C
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
		BasePlayer basePlayer = baseEntity as BasePlayer;
		if (basePlayer != null && basePlayer.IsNpc)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002A1E RID: 10782 RVA: 0x000FEAFC File Offset: 0x000FCCFC
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (this.aiz == null && (this.zones == null || this.zones.Count == 0))
		{
			return;
		}
		base.CancelInvoke(new Action(this.SleepAI));
		this.SetZonesSleeping(false);
	}

	// Token: 0x06002A1F RID: 10783 RVA: 0x000FEB50 File Offset: 0x000FCD50
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		if (this.aiz == null && (this.zones == null || this.zones.Count == 0))
		{
			return;
		}
		if (this.entityContents == null || this.entityContents.Count == 0)
		{
			this.DelayedSleepAI();
		}
	}

	// Token: 0x06002A20 RID: 10784 RVA: 0x000FEBA3 File Offset: 0x000FCDA3
	private void DelayedSleepAI()
	{
		base.CancelInvoke(new Action(this.SleepAI));
		base.Invoke(new Action(this.SleepAI), this.SleepDelaySeconds);
	}

	// Token: 0x06002A21 RID: 10785 RVA: 0x000FEBCF File Offset: 0x000FCDCF
	private void SleepAI()
	{
		this.SetZonesSleeping(true);
	}

	// Token: 0x04002201 RID: 8705
	public float SleepDelaySeconds = 30f;

	// Token: 0x04002202 RID: 8706
	public List<AIInformationZone> zones = new List<AIInformationZone>();

	// Token: 0x04002203 RID: 8707
	private AIInformationZone aiz;
}
