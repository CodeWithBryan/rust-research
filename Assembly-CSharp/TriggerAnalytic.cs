using System;
using System.Collections.Generic;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x02000553 RID: 1363
public class TriggerAnalytic : TriggerBase, IServerComponent
{
	// Token: 0x06002987 RID: 10631 RVA: 0x000FC398 File Offset: 0x000FA598
	internal override GameObject InterestedInObject(GameObject obj)
	{
		if (!Analytics.Server.Enabled)
		{
			return null;
		}
		BasePlayer basePlayer;
		if ((basePlayer = (obj.ToBaseEntity() as BasePlayer)) != null && !basePlayer.IsNpc && basePlayer.isServer)
		{
			return basePlayer.gameObject;
		}
		return null;
	}

	// Token: 0x06002988 RID: 10632 RVA: 0x000FC3D8 File Offset: 0x000FA5D8
	internal override void OnEntityEnter(BaseEntity ent)
	{
		if (!Analytics.Server.Enabled)
		{
			return;
		}
		base.OnEntityEnter(ent);
		BasePlayer basePlayer = ent.ToPlayer();
		if (basePlayer != null && !basePlayer.IsNpc)
		{
			this.CheckTimeouts();
			if (this.IsPlayerValid(basePlayer))
			{
				Analytics.Server.Trigger(this.AnalyticMessage);
				this.recentEntrances.Add(new TriggerAnalytic.RecentPlayerEntrance
				{
					Player = basePlayer,
					Time = 0f
				});
			}
		}
	}

	// Token: 0x06002989 RID: 10633 RVA: 0x000FC454 File Offset: 0x000FA654
	private void CheckTimeouts()
	{
		for (int i = this.recentEntrances.Count - 1; i >= 0; i--)
		{
			if (this.recentEntrances[i].Time > this.Timeout)
			{
				this.recentEntrances.RemoveAt(i);
			}
		}
	}

	// Token: 0x0600298A RID: 10634 RVA: 0x000FC4A4 File Offset: 0x000FA6A4
	private bool IsPlayerValid(BasePlayer p)
	{
		for (int i = 0; i < this.recentEntrances.Count; i++)
		{
			if (this.recentEntrances[i].Player == p)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x040021A8 RID: 8616
	public string AnalyticMessage;

	// Token: 0x040021A9 RID: 8617
	public float Timeout = 120f;

	// Token: 0x040021AA RID: 8618
	private List<TriggerAnalytic.RecentPlayerEntrance> recentEntrances = new List<TriggerAnalytic.RecentPlayerEntrance>();

	// Token: 0x02000D02 RID: 3330
	private struct RecentPlayerEntrance
	{
		// Token: 0x040044A1 RID: 17569
		public BasePlayer Player;

		// Token: 0x040044A2 RID: 17570
		public TimeSince Time;
	}
}
