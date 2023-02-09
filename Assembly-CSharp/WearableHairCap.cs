using System;
using UnityEngine;

// Token: 0x020002D9 RID: 729
[RequireComponent(typeof(Wearable))]
public class WearableHairCap : MonoBehaviour
{
	// Token: 0x06001CE2 RID: 7394 RVA: 0x000C5BF0 File Offset: 0x000C3DF0
	public void ApplyHairCap(MaterialPropertyBlock block)
	{
		if (this.Type == HairType.Head || this.Type == HairType.Armpit || this.Type == HairType.Pubic)
		{
			Texture texture = block.GetTexture(WearableHairCap._HairPackedMapUV1);
			block.SetColor(WearableHairCap._HairBaseColorUV1, this.BaseColor.gamma);
			block.SetTexture(WearableHairCap._HairPackedMapUV1, (this.Mask != null) ? this.Mask : texture);
			return;
		}
		if (this.Type == HairType.Facial)
		{
			Texture texture2 = block.GetTexture(WearableHairCap._HairPackedMapUV2);
			block.SetColor(WearableHairCap._HairBaseColorUV2, this.BaseColor.gamma);
			block.SetTexture(WearableHairCap._HairPackedMapUV2, (this.Mask != null) ? this.Mask : texture2);
		}
	}

	// Token: 0x0400168C RID: 5772
	public HairType Type;

	// Token: 0x0400168D RID: 5773
	[ColorUsage(false, true)]
	public Color BaseColor = Color.black;

	// Token: 0x0400168E RID: 5774
	public Texture Mask;

	// Token: 0x0400168F RID: 5775
	private static MaterialPropertyBlock block;

	// Token: 0x04001690 RID: 5776
	private static int _HairBaseColorUV1 = Shader.PropertyToID("_HairBaseColorUV1");

	// Token: 0x04001691 RID: 5777
	private static int _HairBaseColorUV2 = Shader.PropertyToID("_HairBaseColorUV2");

	// Token: 0x04001692 RID: 5778
	private static int _HairPackedMapUV1 = Shader.PropertyToID("_HairPackedMapUV1");

	// Token: 0x04001693 RID: 5779
	private static int _HairPackedMapUV2 = Shader.PropertyToID("_HairPackedMapUV2");
}
