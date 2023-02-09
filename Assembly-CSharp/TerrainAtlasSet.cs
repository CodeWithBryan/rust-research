using System;
using UnityEngine;

// Token: 0x02000666 RID: 1638
[CreateAssetMenu(menuName = "Rust/Terrain Atlas Set")]
public class TerrainAtlasSet : ScriptableObject
{
	// Token: 0x06002E42 RID: 11842 RVA: 0x0011573C File Offset: 0x0011393C
	public void CheckReset()
	{
		if (this.splatNames == null)
		{
			this.splatNames = new string[]
			{
				"Dirt",
				"Snow",
				"Sand",
				"Rock",
				"Grass",
				"Forest",
				"Stones",
				"Gravel"
			};
		}
		else if (this.splatNames.Length != 8)
		{
			Array.Resize<string>(ref this.splatNames, 8);
		}
		if (this.albedoHighpass == null)
		{
			this.albedoHighpass = new bool[8];
		}
		else if (this.albedoHighpass.Length != 8)
		{
			Array.Resize<bool>(ref this.albedoHighpass, 8);
		}
		if (this.albedoPaths == null)
		{
			this.albedoPaths = new string[8];
		}
		else if (this.albedoPaths.Length != 8)
		{
			Array.Resize<string>(ref this.albedoPaths, 8);
		}
		if (this.defaultValues == null)
		{
			this.defaultValues = new Color[]
			{
				new Color(1f, 1f, 1f, 0.5f),
				new Color(0.5f, 0.5f, 1f, 0f),
				new Color(0f, 0f, 1f, 0.5f)
			};
		}
		else if (this.defaultValues.Length != 3)
		{
			Array.Resize<Color>(ref this.defaultValues, 3);
		}
		if (this.sourceMaps == null)
		{
			this.sourceMaps = new TerrainAtlasSet.SourceMapSet[3];
		}
		else if (this.sourceMaps.Length != 3)
		{
			Array.Resize<TerrainAtlasSet.SourceMapSet>(ref this.sourceMaps, 3);
		}
		for (int i = 0; i < 3; i++)
		{
			this.sourceMaps[i] = ((this.sourceMaps[i] != null) ? this.sourceMaps[i] : new TerrainAtlasSet.SourceMapSet());
			this.sourceMaps[i].CheckReset();
		}
	}

	// Token: 0x04002601 RID: 9729
	public const int SplatCount = 8;

	// Token: 0x04002602 RID: 9730
	public const int SplatSize = 2048;

	// Token: 0x04002603 RID: 9731
	public const int MaxSplatSize = 2047;

	// Token: 0x04002604 RID: 9732
	public const int SplatPadding = 256;

	// Token: 0x04002605 RID: 9733
	public const int AtlasSize = 8192;

	// Token: 0x04002606 RID: 9734
	public const int RegionSize = 2560;

	// Token: 0x04002607 RID: 9735
	public const int SplatsPerLine = 3;

	// Token: 0x04002608 RID: 9736
	public const int SourceTypeCount = 3;

	// Token: 0x04002609 RID: 9737
	public const int AtlasMipCount = 10;

	// Token: 0x0400260A RID: 9738
	public static string[] sourceTypeNames = new string[]
	{
		"Albedo",
		"Normal",
		"Packed"
	};

	// Token: 0x0400260B RID: 9739
	public static string[] sourceTypeNamesExt = new string[]
	{
		"Albedo (rgb)",
		"Normal (rgb)",
		"Metal[ignored]_Height_AO_Gloss (rgba)"
	};

	// Token: 0x0400260C RID: 9740
	public static string[] sourceTypePostfix = new string[]
	{
		"_albedo",
		"_normal",
		"_metal_hm_ao_gloss"
	};

	// Token: 0x0400260D RID: 9741
	public string[] splatNames;

	// Token: 0x0400260E RID: 9742
	public bool[] albedoHighpass;

	// Token: 0x0400260F RID: 9743
	public string[] albedoPaths;

	// Token: 0x04002610 RID: 9744
	public Color[] defaultValues;

	// Token: 0x04002611 RID: 9745
	public TerrainAtlasSet.SourceMapSet[] sourceMaps;

	// Token: 0x04002612 RID: 9746
	public bool highQualityCompression = true;

	// Token: 0x04002613 RID: 9747
	public bool generateTextureAtlases = true;

	// Token: 0x04002614 RID: 9748
	public bool generateTextureArrays;

	// Token: 0x04002615 RID: 9749
	public string splatSearchPrefix = "terrain_";

	// Token: 0x04002616 RID: 9750
	public string splatSearchFolder = "Assets/Content/Nature/Terrain";

	// Token: 0x04002617 RID: 9751
	public string albedoAtlasSavePath = "Assets/Content/Nature/Terrain/Atlas/terrain_albedo_atlas";

	// Token: 0x04002618 RID: 9752
	public string normalAtlasSavePath = "Assets/Content/Nature/Terrain/Atlas/terrain_normal_atlas";

	// Token: 0x04002619 RID: 9753
	public string albedoArraySavePath = "Assets/Content/Nature/Terrain/Atlas/terrain_albedo_array";

	// Token: 0x0400261A RID: 9754
	public string normalArraySavePath = "Assets/Content/Nature/Terrain/Atlas/terrain_normal_array";

	// Token: 0x02000D5E RID: 3422
	public enum SourceType
	{
		// Token: 0x04004646 RID: 17990
		ALBEDO,
		// Token: 0x04004647 RID: 17991
		NORMAL,
		// Token: 0x04004648 RID: 17992
		PACKED,
		// Token: 0x04004649 RID: 17993
		COUNT
	}

	// Token: 0x02000D5F RID: 3423
	[Serializable]
	public class SourceMapSet
	{
		// Token: 0x06004E8C RID: 20108 RVA: 0x0019BF76 File Offset: 0x0019A176
		internal void CheckReset()
		{
			if (this.maps == null)
			{
				this.maps = new Texture2D[8];
				return;
			}
			if (this.maps.Length != 8)
			{
				Array.Resize<Texture2D>(ref this.maps, 8);
			}
		}

		// Token: 0x0400464A RID: 17994
		public Texture2D[] maps;
	}
}
