using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020004E0 RID: 1248
[CreateAssetMenu(menuName = "Rust/Foliage Placement")]
public class FoliagePlacement : ScriptableObject
{
	// Token: 0x04001FEC RID: 8172
	[Header("Placement")]
	public float Density = 2f;

	// Token: 0x04001FED RID: 8173
	[Header("Filter")]
	public SpawnFilter Filter;

	// Token: 0x04001FEE RID: 8174
	[FormerlySerializedAs("Cutoff")]
	public float FilterCutoff = 0.5f;

	// Token: 0x04001FEF RID: 8175
	public float FilterFade = 0.1f;

	// Token: 0x04001FF0 RID: 8176
	[FormerlySerializedAs("Scaling")]
	public float FilterScaling = 1f;

	// Token: 0x04001FF1 RID: 8177
	[Header("Randomization")]
	public float RandomScaling = 0.2f;

	// Token: 0x04001FF2 RID: 8178
	[Header("Placement Range")]
	[MinMax(0f, 1f)]
	public MinMax Range = new MinMax(0f, 1f);

	// Token: 0x04001FF3 RID: 8179
	public float RangeFade = 0.1f;

	// Token: 0x04001FF4 RID: 8180
	[Header("LOD")]
	[Range(0f, 1f)]
	public float DistanceDensity;

	// Token: 0x04001FF5 RID: 8181
	[Range(1f, 2f)]
	public float DistanceScaling = 2f;

	// Token: 0x04001FF6 RID: 8182
	[Header("Visuals")]
	public Material material;

	// Token: 0x04001FF7 RID: 8183
	[FormerlySerializedAs("mesh")]
	public Mesh mesh0;

	// Token: 0x04001FF8 RID: 8184
	[FormerlySerializedAs("mesh")]
	public Mesh mesh1;

	// Token: 0x04001FF9 RID: 8185
	[FormerlySerializedAs("mesh")]
	public Mesh mesh2;

	// Token: 0x04001FFA RID: 8186
	public const int lods = 5;

	// Token: 0x04001FFB RID: 8187
	public const int octaves = 1;

	// Token: 0x04001FFC RID: 8188
	public const float frequency = 0.05f;

	// Token: 0x04001FFD RID: 8189
	public const float amplitude = 0.5f;

	// Token: 0x04001FFE RID: 8190
	public const float offset = 0.5f;
}
