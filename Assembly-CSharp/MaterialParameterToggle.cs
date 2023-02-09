using System;
using UnityEngine;

// Token: 0x020002AF RID: 687
public class MaterialParameterToggle : MonoBehaviour
{
	// Token: 0x040015A9 RID: 5545
	[InspectorFlags]
	public MaterialParameterToggle.ToggleMode Toggle;

	// Token: 0x040015AA RID: 5546
	public Renderer[] TargetRenderers = new Renderer[0];

	// Token: 0x040015AB RID: 5547
	[ColorUsage(true, true)]
	public Color EmissionColor;

	// Token: 0x02000C43 RID: 3139
	[Flags]
	public enum ToggleMode
	{
		// Token: 0x04004192 RID: 16786
		Detail = 0,
		// Token: 0x04004193 RID: 16787
		Emission = 1
	}
}
