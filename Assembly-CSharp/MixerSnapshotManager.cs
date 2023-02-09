using System;
using UnityEngine.Audio;

// Token: 0x02000216 RID: 534
public class MixerSnapshotManager : SingletonComponent<MixerSnapshotManager>, IClientComponent
{
	// Token: 0x0400133B RID: 4923
	public AudioMixerSnapshot defaultSnapshot;

	// Token: 0x0400133C RID: 4924
	public AudioMixerSnapshot underwaterSnapshot;

	// Token: 0x0400133D RID: 4925
	public AudioMixerSnapshot loadingSnapshot;

	// Token: 0x0400133E RID: 4926
	public AudioMixerSnapshot woundedSnapshot;

	// Token: 0x0400133F RID: 4927
	public AudioMixerSnapshot cctvSnapshot;

	// Token: 0x04001340 RID: 4928
	public SoundDefinition underwaterInSound;

	// Token: 0x04001341 RID: 4929
	public SoundDefinition underwaterOutSound;

	// Token: 0x04001342 RID: 4930
	public AudioMixerSnapshot recordingSnapshot;

	// Token: 0x04001343 RID: 4931
	public SoundDefinition woundedLoop;

	// Token: 0x04001344 RID: 4932
	private Sound woundedLoopSound;

	// Token: 0x04001345 RID: 4933
	public SoundDefinition cctvModeLoopDef;

	// Token: 0x04001346 RID: 4934
	private Sound cctvModeLoop;

	// Token: 0x04001347 RID: 4935
	public SoundDefinition cctvModeStartDef;

	// Token: 0x04001348 RID: 4936
	public SoundDefinition cctvModeStopDef;

	// Token: 0x04001349 RID: 4937
	public float deafness;
}
