using System;
using UnityEngine;

// Token: 0x020008D2 RID: 2258
[CreateAssetMenu(menuName = "Rust/Material Config")]
public class MaterialConfig : ScriptableObject
{
	// Token: 0x06003648 RID: 13896 RVA: 0x00143D00 File Offset: 0x00141F00
	public MaterialPropertyBlock GetMaterialPropertyBlock(Material mat, Vector3 pos, Vector3 scale)
	{
		if (this.properties == null)
		{
			this.properties = new MaterialPropertyBlock();
		}
		this.properties.Clear();
		for (int i = 0; i < this.Floats.Length; i++)
		{
			MaterialConfig.ShaderParametersFloat shaderParametersFloat = this.Floats[i];
			float a;
			float b;
			float t = shaderParametersFloat.FindBlendParameters(pos, out a, out b);
			this.properties.SetFloat(shaderParametersFloat.Name, Mathf.Lerp(a, b, t));
		}
		for (int j = 0; j < this.Colors.Length; j++)
		{
			MaterialConfig.ShaderParametersColor shaderParametersColor = this.Colors[j];
			Color a2;
			Color b2;
			float t2 = shaderParametersColor.FindBlendParameters(pos, out a2, out b2);
			this.properties.SetColor(shaderParametersColor.Name, Color.Lerp(a2, b2, t2));
		}
		for (int k = 0; k < this.Textures.Length; k++)
		{
			MaterialConfig.ShaderParametersTexture shaderParametersTexture = this.Textures[k];
			Texture texture = shaderParametersTexture.FindBlendParameters(pos);
			if (texture)
			{
				this.properties.SetTexture(shaderParametersTexture.Name, texture);
			}
		}
		for (int l = 0; l < this.ScaleUV.Length; l++)
		{
			Vector4 vector = mat.GetVector(this.ScaleUV[l]);
			vector = new Vector4(vector.x * scale.y, vector.y * scale.y, vector.z, vector.w);
			this.properties.SetVector(this.ScaleUV[l], vector);
		}
		return this.properties;
	}

	// Token: 0x04003144 RID: 12612
	[Horizontal(4, 0)]
	public MaterialConfig.ShaderParametersFloat[] Floats;

	// Token: 0x04003145 RID: 12613
	[Horizontal(4, 0)]
	public MaterialConfig.ShaderParametersColor[] Colors;

	// Token: 0x04003146 RID: 12614
	[Horizontal(4, 0)]
	public MaterialConfig.ShaderParametersTexture[] Textures;

	// Token: 0x04003147 RID: 12615
	public string[] ScaleUV;

	// Token: 0x04003148 RID: 12616
	private MaterialPropertyBlock properties;

	// Token: 0x02000E53 RID: 3667
	public class ShaderParameters<T>
	{
		// Token: 0x0600504B RID: 20555 RVA: 0x001A1324 File Offset: 0x0019F524
		public float FindBlendParameters(Vector3 pos, out T src, out T dst)
		{
			if (TerrainMeta.BiomeMap == null)
			{
				src = this.Temperate;
				dst = this.Tundra;
				return 0f;
			}
			if (this.climates == null || this.climates.Length == 0)
			{
				this.climates = new T[]
				{
					this.Arid,
					this.Temperate,
					this.Tundra,
					this.Arctic
				};
			}
			int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, -1);
			int biomeMaxType2 = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, ~biomeMaxType);
			src = this.climates[TerrainBiome.TypeToIndex(biomeMaxType)];
			dst = this.climates[TerrainBiome.TypeToIndex(biomeMaxType2)];
			return TerrainMeta.BiomeMap.GetBiome(pos, biomeMaxType2);
		}

		// Token: 0x0600504C RID: 20556 RVA: 0x001A1404 File Offset: 0x0019F604
		public T FindBlendParameters(Vector3 pos)
		{
			if (TerrainMeta.BiomeMap == null)
			{
				return this.Temperate;
			}
			if (this.climates == null || this.climates.Length == 0)
			{
				this.climates = new T[]
				{
					this.Arid,
					this.Temperate,
					this.Tundra,
					this.Arctic
				};
			}
			int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, -1);
			return this.climates[TerrainBiome.TypeToIndex(biomeMaxType)];
		}

		// Token: 0x04004A18 RID: 18968
		public string Name;

		// Token: 0x04004A19 RID: 18969
		public T Arid;

		// Token: 0x04004A1A RID: 18970
		public T Temperate;

		// Token: 0x04004A1B RID: 18971
		public T Tundra;

		// Token: 0x04004A1C RID: 18972
		public T Arctic;

		// Token: 0x04004A1D RID: 18973
		private T[] climates;
	}

	// Token: 0x02000E54 RID: 3668
	[Serializable]
	public class ShaderParametersFloat : MaterialConfig.ShaderParameters<float>
	{
	}

	// Token: 0x02000E55 RID: 3669
	[Serializable]
	public class ShaderParametersColor : MaterialConfig.ShaderParameters<Color>
	{
	}

	// Token: 0x02000E56 RID: 3670
	[Serializable]
	public class ShaderParametersTexture : MaterialConfig.ShaderParameters<Texture>
	{
	}
}
