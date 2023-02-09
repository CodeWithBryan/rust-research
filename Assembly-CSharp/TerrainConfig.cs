using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000629 RID: 1577
[CreateAssetMenu(menuName = "Rust/Terrain Config")]
public class TerrainConfig : ScriptableObject
{
	// Token: 0x17000374 RID: 884
	// (get) Token: 0x06002D5D RID: 11613 RVA: 0x0011198A File Offset: 0x0010FB8A
	public Texture AlbedoArray
	{
		get
		{
			return this.AlbedoArrays[Mathf.Clamp(QualitySettings.masterTextureLimit, 0, 2)];
		}
	}

	// Token: 0x17000375 RID: 885
	// (get) Token: 0x06002D5E RID: 11614 RVA: 0x0011199F File Offset: 0x0010FB9F
	public Texture NormalArray
	{
		get
		{
			return this.NormalArrays[Mathf.Clamp(QualitySettings.masterTextureLimit, 0, 2)];
		}
	}

	// Token: 0x06002D5F RID: 11615 RVA: 0x001119B4 File Offset: 0x0010FBB4
	public PhysicMaterial[] GetPhysicMaterials()
	{
		PhysicMaterial[] array = new PhysicMaterial[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].Material;
		}
		return array;
	}

	// Token: 0x06002D60 RID: 11616 RVA: 0x001119F4 File Offset: 0x0010FBF4
	public Color[] GetAridColors()
	{
		Color[] array = new Color[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].AridColor;
		}
		return array;
	}

	// Token: 0x06002D61 RID: 11617 RVA: 0x00111A38 File Offset: 0x0010FC38
	public void GetAridOverlayConstants(out Color[] color, out Vector4[] param)
	{
		color = new Color[this.Splats.Length];
		param = new Vector4[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			TerrainConfig.SplatOverlay aridOverlay = this.Splats[i].AridOverlay;
			color[i] = aridOverlay.Color.linear;
			param[i] = new Vector4(aridOverlay.Smoothness, aridOverlay.NormalIntensity, aridOverlay.BlendFactor, aridOverlay.BlendFalloff);
		}
	}

	// Token: 0x06002D62 RID: 11618 RVA: 0x00111ABC File Offset: 0x0010FCBC
	public Color[] GetTemperateColors()
	{
		Color[] array = new Color[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].TemperateColor;
		}
		return array;
	}

	// Token: 0x06002D63 RID: 11619 RVA: 0x00111B00 File Offset: 0x0010FD00
	public void GetTemperateOverlayConstants(out Color[] color, out Vector4[] param)
	{
		color = new Color[this.Splats.Length];
		param = new Vector4[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			TerrainConfig.SplatOverlay temperateOverlay = this.Splats[i].TemperateOverlay;
			color[i] = temperateOverlay.Color.linear;
			param[i] = new Vector4(temperateOverlay.Smoothness, temperateOverlay.NormalIntensity, temperateOverlay.BlendFactor, temperateOverlay.BlendFalloff);
		}
	}

	// Token: 0x06002D64 RID: 11620 RVA: 0x00111B84 File Offset: 0x0010FD84
	public Color[] GetTundraColors()
	{
		Color[] array = new Color[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].TundraColor;
		}
		return array;
	}

	// Token: 0x06002D65 RID: 11621 RVA: 0x00111BC8 File Offset: 0x0010FDC8
	public void GetTundraOverlayConstants(out Color[] color, out Vector4[] param)
	{
		color = new Color[this.Splats.Length];
		param = new Vector4[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			TerrainConfig.SplatOverlay tundraOverlay = this.Splats[i].TundraOverlay;
			color[i] = tundraOverlay.Color.linear;
			param[i] = new Vector4(tundraOverlay.Smoothness, tundraOverlay.NormalIntensity, tundraOverlay.BlendFactor, tundraOverlay.BlendFalloff);
		}
	}

	// Token: 0x06002D66 RID: 11622 RVA: 0x00111C4C File Offset: 0x0010FE4C
	public Color[] GetArcticColors()
	{
		Color[] array = new Color[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].ArcticColor;
		}
		return array;
	}

	// Token: 0x06002D67 RID: 11623 RVA: 0x00111C90 File Offset: 0x0010FE90
	public void GetArcticOverlayConstants(out Color[] color, out Vector4[] param)
	{
		color = new Color[this.Splats.Length];
		param = new Vector4[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			TerrainConfig.SplatOverlay arcticOverlay = this.Splats[i].ArcticOverlay;
			color[i] = arcticOverlay.Color.linear;
			param[i] = new Vector4(arcticOverlay.Smoothness, arcticOverlay.NormalIntensity, arcticOverlay.BlendFactor, arcticOverlay.BlendFalloff);
		}
	}

	// Token: 0x06002D68 RID: 11624 RVA: 0x00111D14 File Offset: 0x0010FF14
	public float[] GetSplatTiling()
	{
		float[] array = new float[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].SplatTiling;
		}
		return array;
	}

	// Token: 0x06002D69 RID: 11625 RVA: 0x00111D54 File Offset: 0x0010FF54
	public float GetMaxSplatTiling()
	{
		float num = float.MinValue;
		for (int i = 0; i < this.Splats.Length; i++)
		{
			if (this.Splats[i].SplatTiling > num)
			{
				num = this.Splats[i].SplatTiling;
			}
		}
		return num;
	}

	// Token: 0x06002D6A RID: 11626 RVA: 0x00111D9C File Offset: 0x0010FF9C
	public float GetMinSplatTiling()
	{
		float num = float.MaxValue;
		for (int i = 0; i < this.Splats.Length; i++)
		{
			if (this.Splats[i].SplatTiling < num)
			{
				num = this.Splats[i].SplatTiling;
			}
		}
		return num;
	}

	// Token: 0x06002D6B RID: 11627 RVA: 0x00111DE4 File Offset: 0x0010FFE4
	public Vector3[] GetPackedUVMIX()
	{
		Vector3[] array = new Vector3[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = new Vector3(this.Splats[i].UVMIXMult, this.Splats[i].UVMIXStart, this.Splats[i].UVMIXDist);
		}
		return array;
	}

	// Token: 0x06002D6C RID: 11628 RVA: 0x00111E48 File Offset: 0x00110048
	public TerrainConfig.GroundType GetCurrentGroundType(bool isGrounded, RaycastHit hit)
	{
		if (string.IsNullOrEmpty(this.grassMatName))
		{
			this.dirtMatNames = new List<string>();
			this.stoneyMatNames = new List<string>();
			TerrainConfig.SplatType[] splats = this.Splats;
			int i = 0;
			while (i < splats.Length)
			{
				TerrainConfig.SplatType splatType = splats[i];
				string text = splatType.Name.ToLower();
				string name = splatType.Material.name;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 2296799147U)
				{
					if (num <= 1328097888U)
					{
						if (num != 1180566432U)
						{
							if (num == 1328097888U)
							{
								if (text == "forest")
								{
									goto IL_183;
								}
							}
						}
						else if (text == "dirt")
						{
							goto IL_183;
						}
					}
					else if (num != 2223183858U)
					{
						if (num == 2296799147U)
						{
							if (text == "stones")
							{
								goto IL_192;
							}
						}
					}
					else if (text == "snow")
					{
						this.snowMatName = name;
					}
				}
				else if (num <= 3000956154U)
				{
					if (num != 2993663101U)
					{
						if (num == 3000956154U)
						{
							if (text == "gravel")
							{
								goto IL_192;
							}
						}
					}
					else if (text == "grass")
					{
						this.grassMatName = name;
					}
				}
				else if (num != 3189014883U)
				{
					if (num == 3912378421U)
					{
						if (text == "tundra")
						{
							goto IL_183;
						}
					}
				}
				else if (text == "sand")
				{
					this.sandMatName = name;
				}
				IL_19F:
				i++;
				continue;
				IL_183:
				this.dirtMatNames.Add(name);
				goto IL_19F;
				IL_192:
				this.stoneyMatNames.Add(name);
				goto IL_19F;
			}
		}
		if (!isGrounded)
		{
			return TerrainConfig.GroundType.None;
		}
		if (hit.collider == null)
		{
			return TerrainConfig.GroundType.HardSurface;
		}
		PhysicMaterial materialAt = hit.collider.GetMaterialAt(hit.point);
		if (materialAt == null)
		{
			return TerrainConfig.GroundType.HardSurface;
		}
		string name2 = materialAt.name;
		if (name2 == this.grassMatName)
		{
			return TerrainConfig.GroundType.Grass;
		}
		if (name2 == this.sandMatName)
		{
			return TerrainConfig.GroundType.Sand;
		}
		if (name2 == this.snowMatName)
		{
			return TerrainConfig.GroundType.Snow;
		}
		for (int j = 0; j < this.dirtMatNames.Count; j++)
		{
			if (this.dirtMatNames[j] == name2)
			{
				return TerrainConfig.GroundType.Dirt;
			}
		}
		for (int k = 0; k < this.stoneyMatNames.Count; k++)
		{
			if (this.stoneyMatNames[k] == name2)
			{
				return TerrainConfig.GroundType.Gravel;
			}
		}
		return TerrainConfig.GroundType.HardSurface;
	}

	// Token: 0x04002526 RID: 9510
	public bool CastShadows = true;

	// Token: 0x04002527 RID: 9511
	public LayerMask GroundMask = 0;

	// Token: 0x04002528 RID: 9512
	public LayerMask WaterMask = 0;

	// Token: 0x04002529 RID: 9513
	public PhysicMaterial GenericMaterial;

	// Token: 0x0400252A RID: 9514
	public Material Material;

	// Token: 0x0400252B RID: 9515
	public Material MarginMaterial;

	// Token: 0x0400252C RID: 9516
	public Texture[] AlbedoArrays = new Texture[3];

	// Token: 0x0400252D RID: 9517
	public Texture[] NormalArrays = new Texture[3];

	// Token: 0x0400252E RID: 9518
	public float HeightMapErrorMin = 5f;

	// Token: 0x0400252F RID: 9519
	public float HeightMapErrorMax = 100f;

	// Token: 0x04002530 RID: 9520
	public float BaseMapDistanceMin = 100f;

	// Token: 0x04002531 RID: 9521
	public float BaseMapDistanceMax = 500f;

	// Token: 0x04002532 RID: 9522
	public float ShaderLodMin = 100f;

	// Token: 0x04002533 RID: 9523
	public float ShaderLodMax = 600f;

	// Token: 0x04002534 RID: 9524
	public TerrainConfig.SplatType[] Splats = new TerrainConfig.SplatType[8];

	// Token: 0x04002535 RID: 9525
	private string snowMatName;

	// Token: 0x04002536 RID: 9526
	private string grassMatName;

	// Token: 0x04002537 RID: 9527
	private string sandMatName;

	// Token: 0x04002538 RID: 9528
	private List<string> dirtMatNames;

	// Token: 0x04002539 RID: 9529
	private List<string> stoneyMatNames;

	// Token: 0x02000D55 RID: 3413
	[Serializable]
	public class SplatOverlay
	{
		// Token: 0x0400460F RID: 17935
		public Color Color = new Color(1f, 1f, 1f, 0f);

		// Token: 0x04004610 RID: 17936
		[Range(0f, 1f)]
		public float Smoothness;

		// Token: 0x04004611 RID: 17937
		[Range(0f, 1f)]
		public float NormalIntensity = 1f;

		// Token: 0x04004612 RID: 17938
		[Range(0f, 8f)]
		public float BlendFactor = 0.5f;

		// Token: 0x04004613 RID: 17939
		[Range(0.01f, 32f)]
		public float BlendFalloff = 0.5f;
	}

	// Token: 0x02000D56 RID: 3414
	[Serializable]
	public class SplatType
	{
		// Token: 0x04004614 RID: 17940
		public string Name = "";

		// Token: 0x04004615 RID: 17941
		[FormerlySerializedAs("WarmColor")]
		public Color AridColor = Color.white;

		// Token: 0x04004616 RID: 17942
		public TerrainConfig.SplatOverlay AridOverlay = new TerrainConfig.SplatOverlay();

		// Token: 0x04004617 RID: 17943
		[FormerlySerializedAs("Color")]
		public Color TemperateColor = Color.white;

		// Token: 0x04004618 RID: 17944
		public TerrainConfig.SplatOverlay TemperateOverlay = new TerrainConfig.SplatOverlay();

		// Token: 0x04004619 RID: 17945
		[FormerlySerializedAs("ColdColor")]
		public Color TundraColor = Color.white;

		// Token: 0x0400461A RID: 17946
		public TerrainConfig.SplatOverlay TundraOverlay = new TerrainConfig.SplatOverlay();

		// Token: 0x0400461B RID: 17947
		[FormerlySerializedAs("ColdColor")]
		public Color ArcticColor = Color.white;

		// Token: 0x0400461C RID: 17948
		public TerrainConfig.SplatOverlay ArcticOverlay = new TerrainConfig.SplatOverlay();

		// Token: 0x0400461D RID: 17949
		public PhysicMaterial Material;

		// Token: 0x0400461E RID: 17950
		public float SplatTiling = 5f;

		// Token: 0x0400461F RID: 17951
		[Range(0f, 1f)]
		public float UVMIXMult = 0.15f;

		// Token: 0x04004620 RID: 17952
		public float UVMIXStart;

		// Token: 0x04004621 RID: 17953
		public float UVMIXDist = 100f;
	}

	// Token: 0x02000D57 RID: 3415
	public enum GroundType
	{
		// Token: 0x04004623 RID: 17955
		None,
		// Token: 0x04004624 RID: 17956
		HardSurface,
		// Token: 0x04004625 RID: 17957
		Grass,
		// Token: 0x04004626 RID: 17958
		Sand,
		// Token: 0x04004627 RID: 17959
		Snow,
		// Token: 0x04004628 RID: 17960
		Dirt,
		// Token: 0x04004629 RID: 17961
		Gravel
	}
}
