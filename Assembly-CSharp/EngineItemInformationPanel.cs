using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007EF RID: 2031
public class EngineItemInformationPanel : ItemInformationPanel
{
	// Token: 0x04002D0A RID: 11530
	[SerializeField]
	private Text tier;

	// Token: 0x04002D0B RID: 11531
	[SerializeField]
	private Translate.Phrase low;

	// Token: 0x04002D0C RID: 11532
	[SerializeField]
	private Translate.Phrase medium;

	// Token: 0x04002D0D RID: 11533
	[SerializeField]
	private Translate.Phrase high;

	// Token: 0x04002D0E RID: 11534
	[SerializeField]
	private GameObject accelerationRoot;

	// Token: 0x04002D0F RID: 11535
	[SerializeField]
	private GameObject topSpeedRoot;

	// Token: 0x04002D10 RID: 11536
	[SerializeField]
	private GameObject fuelEconomyRoot;
}
