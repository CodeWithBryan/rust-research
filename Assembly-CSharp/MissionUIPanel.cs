using System;
using Rust.UI;
using UnityEngine;

// Token: 0x020005E9 RID: 1513
public class MissionUIPanel : MonoBehaviour
{
	// Token: 0x0400243D RID: 9277
	public GameObject activeMissionParent;

	// Token: 0x0400243E RID: 9278
	public RustText missionTitleText;

	// Token: 0x0400243F RID: 9279
	public RustText missionDescText;

	// Token: 0x04002440 RID: 9280
	public VirtualItemIcon[] rewardIcons;

	// Token: 0x04002441 RID: 9281
	public Translate.Phrase noMissionText;
}
