using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000552 RID: 1362
public class TriggerAchievement : TriggerBase
{
	// Token: 0x06002982 RID: 10626 RVA: 0x000FC1FB File Offset: 0x000FA3FB
	public void OnPuzzleReset()
	{
		this.Reset();
	}

	// Token: 0x06002983 RID: 10627 RVA: 0x000FC203 File Offset: 0x000FA403
	public void Reset()
	{
		this.triggeredPlayers.Clear();
	}

	// Token: 0x06002984 RID: 10628 RVA: 0x000FC210 File Offset: 0x000FA410
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
		if (baseEntity.isClient && this.serverSide)
		{
			return null;
		}
		if (baseEntity.isServer && !this.serverSide)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002985 RID: 10629 RVA: 0x000FC270 File Offset: 0x000FA470
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (ent == null)
		{
			return;
		}
		BasePlayer component = ent.GetComponent<BasePlayer>();
		if (component == null || !component.IsAlive() || component.IsSleeping() || component.IsNpc)
		{
			return;
		}
		if (this.triggeredPlayers.Contains(component.userID))
		{
			return;
		}
		if (!string.IsNullOrEmpty(this.requiredVehicleName))
		{
			BaseVehicle mountedVehicle = component.GetMountedVehicle();
			if (mountedVehicle == null)
			{
				return;
			}
			if (!mountedVehicle.ShortPrefabName.Contains(this.requiredVehicleName))
			{
				return;
			}
		}
		if (this.serverSide)
		{
			if (!string.IsNullOrEmpty(this.achievementOnEnter))
			{
				component.GiveAchievement(this.achievementOnEnter);
			}
			if (!string.IsNullOrEmpty(this.statToIncrease))
			{
				component.stats.Add(this.statToIncrease, 1, Stats.Steam);
				component.stats.Save(true);
			}
			this.triggeredPlayers.Add(component.userID);
		}
	}

	// Token: 0x040021A3 RID: 8611
	public string statToIncrease = "";

	// Token: 0x040021A4 RID: 8612
	public string achievementOnEnter = "";

	// Token: 0x040021A5 RID: 8613
	public string requiredVehicleName = "";

	// Token: 0x040021A6 RID: 8614
	[Tooltip("Always set to true, clientside does not work, currently")]
	public bool serverSide = true;

	// Token: 0x040021A7 RID: 8615
	[NonSerialized]
	private List<ulong> triggeredPlayers = new List<ulong>();
}
