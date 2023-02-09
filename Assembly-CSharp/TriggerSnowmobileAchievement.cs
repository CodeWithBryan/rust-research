using System;
using UnityEngine;

// Token: 0x0200056D RID: 1389
public class TriggerSnowmobileAchievement : TriggerBase
{
	// Token: 0x06002A11 RID: 10769 RVA: 0x000FE76C File Offset: 0x000FC96C
	internal override GameObject InterestedInObject(GameObject obj)
	{
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity != null && baseEntity.isServer && baseEntity.ToPlayer() != null)
		{
			return baseEntity.gameObject;
		}
		return null;
	}
}
