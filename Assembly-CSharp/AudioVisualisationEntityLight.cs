using System;
using UnityEngine;

// Token: 0x02000376 RID: 886
public class AudioVisualisationEntityLight : AudioVisualisationEntity
{
	// Token: 0x04001874 RID: 6260
	public Light TargetLight;

	// Token: 0x04001875 RID: 6261
	public Light SecondaryLight;

	// Token: 0x04001876 RID: 6262
	public MeshRenderer[] TargetRenderer;

	// Token: 0x04001877 RID: 6263
	public AudioVisualisationEntityLight.LightColourSet RedColour;

	// Token: 0x04001878 RID: 6264
	public AudioVisualisationEntityLight.LightColourSet GreenColour;

	// Token: 0x04001879 RID: 6265
	public AudioVisualisationEntityLight.LightColourSet BlueColour;

	// Token: 0x0400187A RID: 6266
	public AudioVisualisationEntityLight.LightColourSet YellowColour;

	// Token: 0x0400187B RID: 6267
	public AudioVisualisationEntityLight.LightColourSet PinkColour;

	// Token: 0x0400187C RID: 6268
	public float lightMinIntensity = 0.05f;

	// Token: 0x0400187D RID: 6269
	public float lightMaxIntensity = 1f;

	// Token: 0x02000C62 RID: 3170
	[Serializable]
	public struct LightColourSet
	{
		// Token: 0x04004226 RID: 16934
		[ColorUsage(true, true)]
		public Color LightColor;

		// Token: 0x04004227 RID: 16935
		[ColorUsage(true, true)]
		public Color SecondaryLightColour;

		// Token: 0x04004228 RID: 16936
		[ColorUsage(true, true)]
		public Color EmissionColour;
	}
}
