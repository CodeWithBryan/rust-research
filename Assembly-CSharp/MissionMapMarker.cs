using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007C9 RID: 1993
public class MissionMapMarker : MonoBehaviour
{
	// Token: 0x060033F8 RID: 13304 RVA: 0x0013CD5C File Offset: 0x0013AF5C
	public void Populate(BaseMission.MissionInstance mission)
	{
		BaseMission mission2 = mission.GetMission();
		this.Icon.sprite = mission2.icon;
		this.TooltipComponent.token = mission2.missionName.token;
		this.TooltipComponent.Text = mission2.missionName.english;
	}

	// Token: 0x04002C42 RID: 11330
	public Image Icon;

	// Token: 0x04002C43 RID: 11331
	public Tooltip TooltipComponent;
}
