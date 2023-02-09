using System;
using UnityEngine;

// Token: 0x020006CB RID: 1739
public class Mountain : TerrainPlacement
{
	// Token: 0x0600309B RID: 12443 RVA: 0x0012B638 File Offset: 0x00129838
	protected void OnDrawGizmosSelected()
	{
		Vector3 b = Vector3.up * (0.5f * this.Fade);
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		Gizmos.DrawCube(base.transform.position + b, new Vector3(this.size.x, this.Fade, this.size.z));
		Gizmos.DrawWireCube(base.transform.position + b, new Vector3(this.size.x, this.Fade, this.size.z));
	}

	// Token: 0x0600309C RID: 12444 RVA: 0x0012B6E8 File Offset: 0x001298E8
	protected override void ApplyHeight(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		Vector3 position = localToWorld.MultiplyPoint3x4(Vector3.zero);
		TextureData heightdata = new TextureData(this.heightmap.Get());
		Vector3 v = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.extents.x, 0f, -this.extents.z));
		Vector3 v2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.extents.x, 0f, -this.extents.z));
		Vector3 v3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.extents.x, 0f, this.extents.z));
		Vector3 v4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.extents.x, 0f, this.extents.z));
		TerrainMeta.HeightMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			float normZ = TerrainMeta.HeightMap.Coordinate(z);
			float normX = TerrainMeta.HeightMap.Coordinate(x);
			Vector3 point = new Vector3(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 vector = worldToLocal.MultiplyPoint3x4(point) - this.offset;
			float num = position.y + this.offset.y + heightdata.GetInterpolatedHalf((vector.x + this.extents.x) / this.size.x, (vector.z + this.extents.z) / this.size.z) * this.size.y;
			float num2 = Mathf.InverseLerp(position.y, position.y + this.Fade, num);
			if (num2 == 0f)
			{
				return;
			}
			float num3 = TerrainMeta.NormalizeY(num);
			num3 = Mathx.SmoothMax(TerrainMeta.HeightMap.GetHeight01(x, z), num3, 0.1f);
			TerrainMeta.HeightMap.SetHeight(x, z, num3, num2);
		});
	}

	// Token: 0x0600309D RID: 12445 RVA: 0x0012B820 File Offset: 0x00129A20
	protected override void ApplySplat(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		bool should0 = base.ShouldSplat(1);
		bool should1 = base.ShouldSplat(2);
		bool should2 = base.ShouldSplat(4);
		bool should3 = base.ShouldSplat(8);
		bool should4 = base.ShouldSplat(16);
		bool should5 = base.ShouldSplat(32);
		bool should6 = base.ShouldSplat(64);
		bool should7 = base.ShouldSplat(128);
		if (!should0 && !should1 && !should2 && !should3 && !should4 && !should5 && !should6 && !should7)
		{
			return;
		}
		Vector3 position = localToWorld.MultiplyPoint3x4(Vector3.zero);
		TextureData heightdata = new TextureData(this.heightmap.Get());
		TextureData splat0data = new TextureData(this.splatmap0.Get());
		TextureData splat1data = new TextureData(this.splatmap1.Get());
		Vector3 v = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.extents.x, 0f, -this.extents.z));
		Vector3 v2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.extents.x, 0f, -this.extents.z));
		Vector3 v3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.extents.x, 0f, this.extents.z));
		Vector3 v4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.extents.x, 0f, this.extents.z));
		TerrainMeta.SplatMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			float normZ = TerrainMeta.SplatMap.Coordinate(z);
			float normX = TerrainMeta.SplatMap.Coordinate(x);
			Vector3 point = new Vector3(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 vector = worldToLocal.MultiplyPoint3x4(point) - this.offset;
			float value = position.y + this.offset.y + heightdata.GetInterpolatedHalf((vector.x + this.extents.x) / this.size.x, (vector.z + this.extents.z) / this.size.z) * this.size.y;
			float num = Mathf.InverseLerp(position.y, position.y + this.Fade, value);
			if (num == 0f)
			{
				return;
			}
			Vector4 interpolatedVector = splat0data.GetInterpolatedVector((vector.x + this.extents.x) / this.size.x, (vector.z + this.extents.z) / this.size.z);
			Vector4 interpolatedVector2 = splat1data.GetInterpolatedVector((vector.x + this.extents.x) / this.size.x, (vector.z + this.extents.z) / this.size.z);
			if (!should0)
			{
				interpolatedVector.x = 0f;
			}
			if (!should1)
			{
				interpolatedVector.y = 0f;
			}
			if (!should2)
			{
				interpolatedVector.z = 0f;
			}
			if (!should3)
			{
				interpolatedVector.w = 0f;
			}
			if (!should4)
			{
				interpolatedVector2.x = 0f;
			}
			if (!should5)
			{
				interpolatedVector2.y = 0f;
			}
			if (!should6)
			{
				interpolatedVector2.z = 0f;
			}
			if (!should7)
			{
				interpolatedVector2.w = 0f;
			}
			TerrainMeta.SplatMap.SetSplatRaw(x, z, interpolatedVector, interpolatedVector2, num);
		});
	}

	// Token: 0x0600309E RID: 12446 RVA: 0x000059DD File Offset: 0x00003BDD
	protected override void ApplyAlpha(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
	}

	// Token: 0x0600309F RID: 12447 RVA: 0x0012BA34 File Offset: 0x00129C34
	protected override void ApplyBiome(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		bool should0 = base.ShouldBiome(1);
		bool should1 = base.ShouldBiome(2);
		bool should2 = base.ShouldBiome(4);
		bool should3 = base.ShouldBiome(8);
		if (!should0 && !should1 && !should2 && !should3)
		{
			return;
		}
		Vector3 position = localToWorld.MultiplyPoint3x4(Vector3.zero);
		TextureData heightdata = new TextureData(this.heightmap.Get());
		TextureData biomedata = new TextureData(this.biomemap.Get());
		Vector3 v = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.extents.x, 0f, -this.extents.z));
		Vector3 v2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.extents.x, 0f, -this.extents.z));
		Vector3 v3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.extents.x, 0f, this.extents.z));
		Vector3 v4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.extents.x, 0f, this.extents.z));
		TerrainMeta.BiomeMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			float normZ = TerrainMeta.BiomeMap.Coordinate(z);
			float normX = TerrainMeta.BiomeMap.Coordinate(x);
			Vector3 point = new Vector3(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 vector = worldToLocal.MultiplyPoint3x4(point) - this.offset;
			float value = position.y + this.offset.y + heightdata.GetInterpolatedHalf((vector.x + this.extents.x) / this.size.x, (vector.z + this.extents.z) / this.size.z) * this.size.y;
			float num = Mathf.InverseLerp(position.y, position.y + this.Fade, value);
			if (num == 0f)
			{
				return;
			}
			Vector4 interpolatedVector = biomedata.GetInterpolatedVector((vector.x + this.extents.x) / this.size.x, (vector.z + this.extents.z) / this.size.z);
			if (!should0)
			{
				interpolatedVector.x = 0f;
			}
			if (!should1)
			{
				interpolatedVector.y = 0f;
			}
			if (!should2)
			{
				interpolatedVector.z = 0f;
			}
			if (!should3)
			{
				interpolatedVector.w = 0f;
			}
			TerrainMeta.BiomeMap.SetBiomeRaw(x, z, interpolatedVector, num);
		});
	}

	// Token: 0x060030A0 RID: 12448 RVA: 0x0012BBD4 File Offset: 0x00129DD4
	protected override void ApplyTopology(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		TextureData topologydata = new TextureData(this.topologymap.Get());
		Vector3 v = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.extents.x, 0f, -this.extents.z));
		Vector3 v2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.extents.x, 0f, -this.extents.z));
		Vector3 v3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.extents.x, 0f, this.extents.z));
		Vector3 v4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.extents.x, 0f, this.extents.z));
		TerrainMeta.TopologyMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			GenerateCliffTopology.Process(x, z);
			float normZ = TerrainMeta.TopologyMap.Coordinate(z);
			float normX = TerrainMeta.TopologyMap.Coordinate(x);
			Vector3 point = new Vector3(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 vector = worldToLocal.MultiplyPoint3x4(point) - this.offset;
			int interpolatedInt = topologydata.GetInterpolatedInt((vector.x + this.extents.x) / this.size.x, (vector.z + this.extents.z) / this.size.z);
			if (this.ShouldTopology(interpolatedInt))
			{
				TerrainMeta.TopologyMap.AddTopology(x, z, interpolatedInt & (int)this.TopologyMask);
			}
		});
	}

	// Token: 0x060030A1 RID: 12449 RVA: 0x000059DD File Offset: 0x00003BDD
	protected override void ApplyWater(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
	}

	// Token: 0x04002782 RID: 10114
	public float Fade = 10f;
}
