using System;
using UnityEngine;

// Token: 0x02000490 RID: 1168
public class TrainEngineAudio : TrainCarAudio
{
	// Token: 0x04001EBE RID: 7870
	[SerializeField]
	private TrainEngine trainEngine;

	// Token: 0x04001EBF RID: 7871
	[SerializeField]
	private Transform cockpitSoundPosition;

	// Token: 0x04001EC0 RID: 7872
	[SerializeField]
	private Transform hornSoundPosition;

	// Token: 0x04001EC1 RID: 7873
	[Header("Engine")]
	[SerializeField]
	private SoundDefinition engineStartSound;

	// Token: 0x04001EC2 RID: 7874
	[SerializeField]
	private SoundDefinition engineStopSound;

	// Token: 0x04001EC3 RID: 7875
	[SerializeField]
	private SoundDefinition engineActiveLoopDef;

	// Token: 0x04001EC4 RID: 7876
	[SerializeField]
	private AnimationCurve engineActiveLoopPitchCurve;

	// Token: 0x04001EC5 RID: 7877
	[SerializeField]
	private float engineActiveLoopChangeSpeed = 0.2f;

	// Token: 0x04001EC6 RID: 7878
	private Sound engineActiveLoop;

	// Token: 0x04001EC7 RID: 7879
	private SoundModulation.Modulator engineActiveLoopPitch;

	// Token: 0x04001EC8 RID: 7880
	[SerializeField]
	private BlendedLoopEngineSound engineLoops;

	// Token: 0x04001EC9 RID: 7881
	[SerializeField]
	private TrainEngineAudio.EngineReflection[] engineReflections;

	// Token: 0x04001ECA RID: 7882
	[SerializeField]
	private LayerMask reflectionLayerMask;

	// Token: 0x04001ECB RID: 7883
	[SerializeField]
	private float reflectionMaxDistance = 20f;

	// Token: 0x04001ECC RID: 7884
	[SerializeField]
	private float reflectionGainChangeSpeed = 10f;

	// Token: 0x04001ECD RID: 7885
	[SerializeField]
	private float reflectionPositionChangeSpeed = 10f;

	// Token: 0x04001ECE RID: 7886
	[SerializeField]
	private float reflectionRayOffset = 0.5f;

	// Token: 0x04001ECF RID: 7887
	[Header("Horn")]
	[SerializeField]
	private SoundDefinition hornLoop;

	// Token: 0x04001ED0 RID: 7888
	[SerializeField]
	private SoundDefinition hornStart;

	// Token: 0x04001ED1 RID: 7889
	[Header("Other")]
	[SerializeField]
	private SoundDefinition lightsToggleSound;

	// Token: 0x04001ED2 RID: 7890
	[SerializeField]
	private SoundDefinition proximityAlertDef;

	// Token: 0x04001ED3 RID: 7891
	private Sound proximityAlertSound;

	// Token: 0x04001ED4 RID: 7892
	[SerializeField]
	private SoundDefinition damagedLoopDef;

	// Token: 0x04001ED5 RID: 7893
	private Sound damagedLoop;

	// Token: 0x04001ED6 RID: 7894
	[SerializeField]
	private SoundDefinition changeThrottleDef;

	// Token: 0x04001ED7 RID: 7895
	[SerializeField]
	private SoundDefinition changeCouplingDef;

	// Token: 0x04001ED8 RID: 7896
	[SerializeField]
	private SoundDefinition unloadableStartDef;

	// Token: 0x04001ED9 RID: 7897
	[SerializeField]
	private SoundDefinition unloadableEndDef;

	// Token: 0x04001EDA RID: 7898
	[SerializeField]
	private GameObject bellObject;

	// Token: 0x04001EDB RID: 7899
	[SerializeField]
	private SoundDefinition bellRingDef;

	// Token: 0x04001EDC RID: 7900
	[SerializeField]
	private SoundPlayer brakeSound;

	// Token: 0x02000CBA RID: 3258
	[Serializable]
	public class EngineReflection
	{
		// Token: 0x04004396 RID: 17302
		public Vector3 direction;

		// Token: 0x04004397 RID: 17303
		public Vector3 offset;

		// Token: 0x04004398 RID: 17304
		public SoundDefinition soundDef;

		// Token: 0x04004399 RID: 17305
		public Sound sound;

		// Token: 0x0400439A RID: 17306
		public SoundModulation.Modulator pitchMod;

		// Token: 0x0400439B RID: 17307
		public SoundModulation.Modulator gainMod;

		// Token: 0x0400439C RID: 17308
		public float distance = 20f;
	}
}
