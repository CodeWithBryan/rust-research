using System;
using UnityEngine;

// Token: 0x020006CC RID: 1740
public abstract class TerrainPlacement : PrefabAttribute
{
	// Token: 0x060030A3 RID: 12451 RVA: 0x0012BD0C File Offset: 0x00129F0C
	public void Apply(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.ShouldHeight())
		{
			this.ApplyHeight(localToWorld, worldToLocal);
		}
		if (this.ShouldSplat(-1))
		{
			this.ApplySplat(localToWorld, worldToLocal);
		}
		if (this.ShouldAlpha())
		{
			this.ApplyAlpha(localToWorld, worldToLocal);
		}
		if (this.ShouldBiome(-1))
		{
			this.ApplyBiome(localToWorld, worldToLocal);
		}
		if (this.ShouldTopology(-1))
		{
			this.ApplyTopology(localToWorld, worldToLocal);
		}
		if (this.ShouldWater())
		{
			this.ApplyWater(localToWorld, worldToLocal);
		}
	}

	// Token: 0x060030A4 RID: 12452 RVA: 0x0012BD7C File Offset: 0x00129F7C
	protected bool ShouldHeight()
	{
		return this.heightmap.isValid && this.HeightMap;
	}

	// Token: 0x060030A5 RID: 12453 RVA: 0x0012BD93 File Offset: 0x00129F93
	protected bool ShouldSplat(int id = -1)
	{
		return this.splatmap0.isValid && this.splatmap1.isValid && (this.SplatMask & (TerrainSplat.Enum)id) > (TerrainSplat.Enum)0;
	}

	// Token: 0x060030A6 RID: 12454 RVA: 0x0012BDBC File Offset: 0x00129FBC
	protected bool ShouldAlpha()
	{
		return this.alphamap.isValid && this.AlphaMap;
	}

	// Token: 0x060030A7 RID: 12455 RVA: 0x0012BDD3 File Offset: 0x00129FD3
	protected bool ShouldBiome(int id = -1)
	{
		return this.biomemap.isValid && (this.BiomeMask & (TerrainBiome.Enum)id) > (TerrainBiome.Enum)0;
	}

	// Token: 0x060030A8 RID: 12456 RVA: 0x0012BDEF File Offset: 0x00129FEF
	protected bool ShouldTopology(int id = -1)
	{
		return this.topologymap.isValid && (this.TopologyMask & (TerrainTopology.Enum)id) > (TerrainTopology.Enum)0;
	}

	// Token: 0x060030A9 RID: 12457 RVA: 0x0012BE0B File Offset: 0x0012A00B
	protected bool ShouldWater()
	{
		return this.watermap.isValid && this.WaterMap;
	}

	// Token: 0x060030AA RID: 12458
	protected abstract void ApplyHeight(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x060030AB RID: 12459
	protected abstract void ApplySplat(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x060030AC RID: 12460
	protected abstract void ApplyAlpha(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x060030AD RID: 12461
	protected abstract void ApplyBiome(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x060030AE RID: 12462
	protected abstract void ApplyTopology(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x060030AF RID: 12463
	protected abstract void ApplyWater(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x060030B0 RID: 12464 RVA: 0x0012BE22 File Offset: 0x0012A022
	protected override Type GetIndexedType()
	{
		return typeof(TerrainPlacement);
	}

	// Token: 0x04002783 RID: 10115
	[ReadOnly]
	public Vector3 size = Vector3.zero;

	// Token: 0x04002784 RID: 10116
	[ReadOnly]
	public Vector3 extents = Vector3.zero;

	// Token: 0x04002785 RID: 10117
	[ReadOnly]
	public Vector3 offset = Vector3.zero;

	// Token: 0x04002786 RID: 10118
	public bool HeightMap = true;

	// Token: 0x04002787 RID: 10119
	public bool AlphaMap = true;

	// Token: 0x04002788 RID: 10120
	public bool WaterMap;

	// Token: 0x04002789 RID: 10121
	[InspectorFlags]
	public TerrainSplat.Enum SplatMask;

	// Token: 0x0400278A RID: 10122
	[InspectorFlags]
	public TerrainBiome.Enum BiomeMask;

	// Token: 0x0400278B RID: 10123
	[InspectorFlags]
	public TerrainTopology.Enum TopologyMask;

	// Token: 0x0400278C RID: 10124
	[HideInInspector]
	public Texture2DRef heightmap;

	// Token: 0x0400278D RID: 10125
	[HideInInspector]
	public Texture2DRef splatmap0;

	// Token: 0x0400278E RID: 10126
	[HideInInspector]
	public Texture2DRef splatmap1;

	// Token: 0x0400278F RID: 10127
	[HideInInspector]
	public Texture2DRef alphamap;

	// Token: 0x04002790 RID: 10128
	[HideInInspector]
	public Texture2DRef biomemap;

	// Token: 0x04002791 RID: 10129
	[HideInInspector]
	public Texture2DRef topologymap;

	// Token: 0x04002792 RID: 10130
	[HideInInspector]
	public Texture2DRef watermap;

	// Token: 0x04002793 RID: 10131
	[HideInInspector]
	public Texture2DRef blendmap;
}
