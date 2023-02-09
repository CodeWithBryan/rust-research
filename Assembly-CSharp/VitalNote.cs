using System;
using TMPro;
using UnityEngine;

// Token: 0x020008B7 RID: 2231
public class VitalNote : MonoBehaviour, IClientComponent, IVitalNotice
{
	// Token: 0x040030F7 RID: 12535
	public VitalNote.Vital VitalType;

	// Token: 0x040030F8 RID: 12536
	public FloatConditions showIf;

	// Token: 0x040030F9 RID: 12537
	public TextMeshProUGUI valueText;

	// Token: 0x02000E4F RID: 3663
	public enum Vital
	{
		// Token: 0x04004A00 RID: 18944
		Comfort,
		// Token: 0x04004A01 RID: 18945
		Radiation,
		// Token: 0x04004A02 RID: 18946
		Poison,
		// Token: 0x04004A03 RID: 18947
		Cold,
		// Token: 0x04004A04 RID: 18948
		Bleeding,
		// Token: 0x04004A05 RID: 18949
		Hot,
		// Token: 0x04004A06 RID: 18950
		Oxygen,
		// Token: 0x04004A07 RID: 18951
		Wet,
		// Token: 0x04004A08 RID: 18952
		Hygiene,
		// Token: 0x04004A09 RID: 18953
		Starving,
		// Token: 0x04004A0A RID: 18954
		Dehydration
	}
}
