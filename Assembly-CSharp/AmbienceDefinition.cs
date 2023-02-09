using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000206 RID: 518
[CreateAssetMenu(menuName = "Rust/Ambience Definition")]
public class AmbienceDefinition : ScriptableObject
{
	// Token: 0x040012C4 RID: 4804
	[Header("Sound")]
	public List<SoundDefinition> sounds;

	// Token: 0x040012C5 RID: 4805
	[Horizontal(2, -1)]
	public AmbienceDefinition.ValueRange stingFrequency = new AmbienceDefinition.ValueRange(15f, 30f);

	// Token: 0x040012C6 RID: 4806
	[Header("Environment")]
	[InspectorFlags]
	public TerrainBiome.Enum biomes = (TerrainBiome.Enum)(-1);

	// Token: 0x040012C7 RID: 4807
	[InspectorFlags]
	public TerrainTopology.Enum topologies = (TerrainTopology.Enum)(-1);

	// Token: 0x040012C8 RID: 4808
	public EnvironmentType environmentType = EnvironmentType.Underground;

	// Token: 0x040012C9 RID: 4809
	public bool useEnvironmentType;

	// Token: 0x040012CA RID: 4810
	public AnimationCurve time = AnimationCurve.Linear(0f, 0f, 24f, 0f);

	// Token: 0x040012CB RID: 4811
	[Horizontal(2, -1)]
	public AmbienceDefinition.ValueRange rain = new AmbienceDefinition.ValueRange(0f, 1f);

	// Token: 0x040012CC RID: 4812
	[Horizontal(2, -1)]
	public AmbienceDefinition.ValueRange wind = new AmbienceDefinition.ValueRange(0f, 1f);

	// Token: 0x040012CD RID: 4813
	[Horizontal(2, -1)]
	public AmbienceDefinition.ValueRange snow = new AmbienceDefinition.ValueRange(0f, 1f);

	// Token: 0x02000C19 RID: 3097
	[Serializable]
	public class ValueRange
	{
		// Token: 0x06004C28 RID: 19496 RVA: 0x00194DBE File Offset: 0x00192FBE
		public ValueRange(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		// Token: 0x040040B2 RID: 16562
		public float min;

		// Token: 0x040040B3 RID: 16563
		public float max;
	}
}
