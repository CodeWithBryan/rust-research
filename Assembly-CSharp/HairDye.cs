using System;
using UnityEngine;

// Token: 0x02000721 RID: 1825
[Serializable]
public class HairDye
{
	// Token: 0x060032B8 RID: 12984 RVA: 0x00139800 File Offset: 0x00137A00
	public void Apply(HairDyeCollection collection, MaterialPropertyBlock block)
	{
		if (this.sourceMaterial != null)
		{
			for (int i = 0; i < 8; i++)
			{
				if ((this.copyProperties & (HairDye.CopyPropertyMask)(1 << i)) != (HairDye.CopyPropertyMask)0)
				{
					MaterialPropertyDesc materialPropertyDesc = HairDye.transferableProps[i];
					if (this.sourceMaterial.HasProperty(materialPropertyDesc.nameID))
					{
						if (materialPropertyDesc.type == typeof(Color))
						{
							block.SetColor(materialPropertyDesc.nameID, this.sourceMaterial.GetColor(materialPropertyDesc.nameID));
						}
						else if (materialPropertyDesc.type == typeof(float))
						{
							block.SetFloat(materialPropertyDesc.nameID, this.sourceMaterial.GetFloat(materialPropertyDesc.nameID));
						}
					}
				}
			}
		}
	}

	// Token: 0x060032B9 RID: 12985 RVA: 0x001398CC File Offset: 0x00137ACC
	public void ApplyCap(HairDyeCollection collection, HairType type, MaterialPropertyBlock block)
	{
		if (collection.applyCap)
		{
			if (type == HairType.Head || type == HairType.Armpit || type == HairType.Pubic)
			{
				block.SetColor(HairDye._HairBaseColorUV1, this.capBaseColor.gamma);
				block.SetTexture(HairDye._HairPackedMapUV1, (collection.capMask != null) ? collection.capMask : Texture2D.blackTexture);
				return;
			}
			if (type == HairType.Facial)
			{
				block.SetColor(HairDye._HairBaseColorUV2, this.capBaseColor.gamma);
				block.SetTexture(HairDye._HairPackedMapUV2, (collection.capMask != null) ? collection.capMask : Texture2D.blackTexture);
			}
		}
	}

	// Token: 0x040028FF RID: 10495
	[ColorUsage(false, true)]
	public Color capBaseColor;

	// Token: 0x04002900 RID: 10496
	public Material sourceMaterial;

	// Token: 0x04002901 RID: 10497
	[InspectorFlags]
	public HairDye.CopyPropertyMask copyProperties;

	// Token: 0x04002902 RID: 10498
	private static MaterialPropertyDesc[] transferableProps = new MaterialPropertyDesc[]
	{
		new MaterialPropertyDesc("_DyeColor", typeof(Color)),
		new MaterialPropertyDesc("_RootColor", typeof(Color)),
		new MaterialPropertyDesc("_TipColor", typeof(Color)),
		new MaterialPropertyDesc("_Brightness", typeof(float)),
		new MaterialPropertyDesc("_DyeRoughness", typeof(float)),
		new MaterialPropertyDesc("_DyeScatter", typeof(float)),
		new MaterialPropertyDesc("_HairSpecular", typeof(float)),
		new MaterialPropertyDesc("_HairRoughness", typeof(float))
	};

	// Token: 0x04002903 RID: 10499
	private static int _HairBaseColorUV1 = Shader.PropertyToID("_HairBaseColorUV1");

	// Token: 0x04002904 RID: 10500
	private static int _HairBaseColorUV2 = Shader.PropertyToID("_HairBaseColorUV2");

	// Token: 0x04002905 RID: 10501
	private static int _HairPackedMapUV1 = Shader.PropertyToID("_HairPackedMapUV1");

	// Token: 0x04002906 RID: 10502
	private static int _HairPackedMapUV2 = Shader.PropertyToID("_HairPackedMapUV2");

	// Token: 0x02000E03 RID: 3587
	public enum CopyProperty
	{
		// Token: 0x040048B1 RID: 18609
		DyeColor,
		// Token: 0x040048B2 RID: 18610
		RootColor,
		// Token: 0x040048B3 RID: 18611
		TipColor,
		// Token: 0x040048B4 RID: 18612
		Brightness,
		// Token: 0x040048B5 RID: 18613
		DyeRoughness,
		// Token: 0x040048B6 RID: 18614
		DyeScatter,
		// Token: 0x040048B7 RID: 18615
		Specular,
		// Token: 0x040048B8 RID: 18616
		Roughness,
		// Token: 0x040048B9 RID: 18617
		Count
	}

	// Token: 0x02000E04 RID: 3588
	[Flags]
	public enum CopyPropertyMask
	{
		// Token: 0x040048BB RID: 18619
		DyeColor = 1,
		// Token: 0x040048BC RID: 18620
		RootColor = 2,
		// Token: 0x040048BD RID: 18621
		TipColor = 4,
		// Token: 0x040048BE RID: 18622
		Brightness = 8,
		// Token: 0x040048BF RID: 18623
		DyeRoughness = 16,
		// Token: 0x040048C0 RID: 18624
		DyeScatter = 32,
		// Token: 0x040048C1 RID: 18625
		Specular = 64,
		// Token: 0x040048C2 RID: 18626
		Roughness = 128
	}
}
