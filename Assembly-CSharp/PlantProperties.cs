using System;
using UnityEngine;

// Token: 0x0200041A RID: 1050
[CreateAssetMenu(menuName = "Rust/Plant Properties")]
public class PlantProperties : ScriptableObject
{
	// Token: 0x04001B6F RID: 7023
	public Translate.Phrase Description;

	// Token: 0x04001B70 RID: 7024
	public GrowableGeneProperties Genes;

	// Token: 0x04001B71 RID: 7025
	[ArrayIndexIsEnum(enumType = typeof(PlantProperties.State))]
	public PlantProperties.Stage[] stages = new PlantProperties.Stage[8];

	// Token: 0x04001B72 RID: 7026
	[Header("Metabolism")]
	public AnimationCurve timeOfDayHappiness = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(12f, 1f),
		new Keyframe(24f, 0f)
	});

	// Token: 0x04001B73 RID: 7027
	public AnimationCurve temperatureHappiness = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(-10f, -1f),
		new Keyframe(1f, 0f),
		new Keyframe(30f, 1f),
		new Keyframe(50f, 0f),
		new Keyframe(80f, -1f)
	});

	// Token: 0x04001B74 RID: 7028
	public AnimationCurve temperatureWaterRequirementMultiplier = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(-10f, 1f),
		new Keyframe(0f, 1f),
		new Keyframe(30f, 1f),
		new Keyframe(50f, 1f),
		new Keyframe(80f, 1f)
	});

	// Token: 0x04001B75 RID: 7029
	public AnimationCurve fruitVisualScaleCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.75f, 1f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x04001B76 RID: 7030
	public int MaxSeasons = 1;

	// Token: 0x04001B77 RID: 7031
	public float WaterIntake = 20f;

	// Token: 0x04001B78 RID: 7032
	public float OptimalLightQuality = 1f;

	// Token: 0x04001B79 RID: 7033
	public float OptimalWaterQuality = 1f;

	// Token: 0x04001B7A RID: 7034
	public float OptimalGroundQuality = 1f;

	// Token: 0x04001B7B RID: 7035
	public float OptimalTemperatureQuality = 1f;

	// Token: 0x04001B7C RID: 7036
	[Header("Harvesting")]
	public BaseEntity.Menu.Option pickOption;

	// Token: 0x04001B7D RID: 7037
	public BaseEntity.Menu.Option pickAllOption;

	// Token: 0x04001B7E RID: 7038
	public BaseEntity.Menu.Option eatOption;

	// Token: 0x04001B7F RID: 7039
	public ItemDefinition pickupItem;

	// Token: 0x04001B80 RID: 7040
	public BaseEntity.Menu.Option cloneOption;

	// Token: 0x04001B81 RID: 7041
	public BaseEntity.Menu.Option cloneAllOption;

	// Token: 0x04001B82 RID: 7042
	public BaseEntity.Menu.Option removeDyingOption;

	// Token: 0x04001B83 RID: 7043
	public BaseEntity.Menu.Option removeDyingAllOption;

	// Token: 0x04001B84 RID: 7044
	public ItemDefinition removeDyingItem;

	// Token: 0x04001B85 RID: 7045
	public GameObjectRef removeDyingEffect;

	// Token: 0x04001B86 RID: 7046
	public int pickupMultiplier = 1;

	// Token: 0x04001B87 RID: 7047
	public GameObjectRef pickEffect;

	// Token: 0x04001B88 RID: 7048
	public int maxHarvests = 1;

	// Token: 0x04001B89 RID: 7049
	public bool disappearAfterHarvest;

	// Token: 0x04001B8A RID: 7050
	[Header("Seeds")]
	public GameObjectRef CrossBreedEffect;

	// Token: 0x04001B8B RID: 7051
	public ItemDefinition SeedItem;

	// Token: 0x04001B8C RID: 7052
	public ItemDefinition CloneItem;

	// Token: 0x04001B8D RID: 7053
	public int BaseCloneCount = 1;

	// Token: 0x04001B8E RID: 7054
	[Header("Market")]
	public int BaseMarketValue = 10;

	// Token: 0x02000C90 RID: 3216
	public enum State
	{
		// Token: 0x040042E7 RID: 17127
		Seed,
		// Token: 0x040042E8 RID: 17128
		Seedling,
		// Token: 0x040042E9 RID: 17129
		Sapling,
		// Token: 0x040042EA RID: 17130
		Crossbreed,
		// Token: 0x040042EB RID: 17131
		Mature,
		// Token: 0x040042EC RID: 17132
		Fruiting,
		// Token: 0x040042ED RID: 17133
		Ripe,
		// Token: 0x040042EE RID: 17134
		Dying
	}

	// Token: 0x02000C91 RID: 3217
	[Serializable]
	public struct Stage
	{
		// Token: 0x1700062A RID: 1578
		// (get) Token: 0x06004D0E RID: 19726 RVA: 0x00196E7C File Offset: 0x0019507C
		public float lifeLengthSeconds
		{
			get
			{
				return this.lifeLength * 60f;
			}
		}

		// Token: 0x040042EF RID: 17135
		public PlantProperties.State nextState;

		// Token: 0x040042F0 RID: 17136
		public float lifeLength;

		// Token: 0x040042F1 RID: 17137
		public float health;

		// Token: 0x040042F2 RID: 17138
		public float resources;

		// Token: 0x040042F3 RID: 17139
		public float yield;

		// Token: 0x040042F4 RID: 17140
		public GameObjectRef skinObject;

		// Token: 0x040042F5 RID: 17141
		public bool IgnoreConditions;
	}
}
