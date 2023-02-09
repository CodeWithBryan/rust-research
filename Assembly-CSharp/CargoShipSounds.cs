using System;
using UnityEngine;

// Token: 0x02000021 RID: 33
public class CargoShipSounds : MonoBehaviour, IClientComponent
{
	// Token: 0x040000DF RID: 223
	public SoundDefinition waveSoundDef;

	// Token: 0x040000E0 RID: 224
	public AnimationCurve waveSoundYGainCurve;

	// Token: 0x040000E1 RID: 225
	public AnimationCurve waveSoundEdgeDistanceGainCurve;

	// Token: 0x040000E2 RID: 226
	private Sound waveSoundL;

	// Token: 0x040000E3 RID: 227
	private Sound waveSoundR;

	// Token: 0x040000E4 RID: 228
	private SoundModulation.Modulator waveSoundLGainMod;

	// Token: 0x040000E5 RID: 229
	private SoundModulation.Modulator waveSoundRGainMod;

	// Token: 0x040000E6 RID: 230
	public SoundDefinition sternWakeSoundDef;

	// Token: 0x040000E7 RID: 231
	private Sound sternWakeSound;

	// Token: 0x040000E8 RID: 232
	private SoundModulation.Modulator sternWakeSoundGainMod;

	// Token: 0x040000E9 RID: 233
	public SoundDefinition engineHumSoundDef;

	// Token: 0x040000EA RID: 234
	private Sound engineHumSound;

	// Token: 0x040000EB RID: 235
	public GameObject engineHumTarget;

	// Token: 0x040000EC RID: 236
	public SoundDefinition hugeRumbleSoundDef;

	// Token: 0x040000ED RID: 237
	public AnimationCurve hugeRumbleYDiffCurve;

	// Token: 0x040000EE RID: 238
	public AnimationCurve hugeRumbleRelativeSpeedCurve;

	// Token: 0x040000EF RID: 239
	private Sound hugeRumbleSound;

	// Token: 0x040000F0 RID: 240
	private SoundModulation.Modulator hugeRumbleGainMod;

	// Token: 0x040000F1 RID: 241
	private Vector3 lastCameraPos;

	// Token: 0x040000F2 RID: 242
	private Vector3 lastRumblePos;

	// Token: 0x040000F3 RID: 243
	private Vector3 lastRumbleLocalPos;

	// Token: 0x040000F4 RID: 244
	public Collider soundFollowCollider;

	// Token: 0x040000F5 RID: 245
	public Collider soundFollowColliderL;

	// Token: 0x040000F6 RID: 246
	public Collider soundFollowColliderR;

	// Token: 0x040000F7 RID: 247
	public Collider sternSoundFollowCollider;
}
