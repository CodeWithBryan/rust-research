using System;
using UnityEngine;

// Token: 0x020006F1 RID: 1777
[Serializable]
public struct SubsurfaceProfileData
{
	// Token: 0x170003D8 RID: 984
	// (get) Token: 0x0600317C RID: 12668 RVA: 0x00130438 File Offset: 0x0012E638
	public static SubsurfaceProfileData Default
	{
		get
		{
			return new SubsurfaceProfileData
			{
				ScatterRadius = 1.2f,
				SubsurfaceColor = new Color(0.48f, 0.41f, 0.28f),
				FalloffColor = new Color(1f, 0.37f, 0.3f)
			};
		}
	}

	// Token: 0x170003D9 RID: 985
	// (get) Token: 0x0600317D RID: 12669 RVA: 0x00130490 File Offset: 0x0012E690
	public static SubsurfaceProfileData Invalid
	{
		get
		{
			return new SubsurfaceProfileData
			{
				ScatterRadius = 0f,
				SubsurfaceColor = Color.clear,
				FalloffColor = Color.clear
			};
		}
	}

	// Token: 0x04002827 RID: 10279
	[Range(0.1f, 50f)]
	public float ScatterRadius;

	// Token: 0x04002828 RID: 10280
	[ColorUsage(false, true, 1f, 1f, 1f, 1f)]
	public Color SubsurfaceColor;

	// Token: 0x04002829 RID: 10281
	[ColorUsage(false, true, 1f, 1f, 1f, 1f)]
	public Color FalloffColor;
}
