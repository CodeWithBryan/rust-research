using System;
using UnityEngine;

// Token: 0x02000881 RID: 2177
public class RepairCostIndicator : SingletonComponent<RepairCostIndicator>, IClientComponent
{
	// Token: 0x04003041 RID: 12353
	public RepairCostIndicatorRow[] Rows;

	// Token: 0x04003042 RID: 12354
	public CanvasGroup Fader;
}
