using System;
using UnityEngine;

// Token: 0x020003A9 RID: 937
public class EntityItem_RotateWhenOn : EntityComponent<BaseEntity>
{
	// Token: 0x04001933 RID: 6451
	public EntityItem_RotateWhenOn.State on;

	// Token: 0x04001934 RID: 6452
	public EntityItem_RotateWhenOn.State off;

	// Token: 0x04001935 RID: 6453
	internal bool currentlyOn;

	// Token: 0x04001936 RID: 6454
	internal bool stateInitialized;

	// Token: 0x04001937 RID: 6455
	public BaseEntity.Flags targetFlag = BaseEntity.Flags.On;

	// Token: 0x02000C6C RID: 3180
	[Serializable]
	public class State
	{
		// Token: 0x04004240 RID: 16960
		public Vector3 rotation;

		// Token: 0x04004241 RID: 16961
		public float initialDelay;

		// Token: 0x04004242 RID: 16962
		public float timeToTake = 2f;

		// Token: 0x04004243 RID: 16963
		public AnimationCurve animationCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f)
		});

		// Token: 0x04004244 RID: 16964
		public string effectOnStart = "";

		// Token: 0x04004245 RID: 16965
		public string effectOnFinish = "";

		// Token: 0x04004246 RID: 16966
		public SoundDefinition movementLoop;

		// Token: 0x04004247 RID: 16967
		public float movementLoopFadeOutTime = 0.1f;

		// Token: 0x04004248 RID: 16968
		public SoundDefinition startSound;

		// Token: 0x04004249 RID: 16969
		public SoundDefinition stopSound;
	}
}
