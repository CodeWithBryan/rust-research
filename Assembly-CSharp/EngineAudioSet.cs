using System;
using UnityEngine;

// Token: 0x0200046E RID: 1134
[CreateAssetMenu(fileName = "Engine Audio Preset", menuName = "Rust/Vehicles/Engine Audio Preset")]
public class EngineAudioSet : ScriptableObject
{
	// Token: 0x06002523 RID: 9507 RVA: 0x000E91D0 File Offset: 0x000E73D0
	public BlendedEngineLoopDefinition GetEngineLoopDef(int numEngines)
	{
		int num = (numEngines - 1) % this.engineAudioLoops.Length;
		return this.engineAudioLoops[num];
	}

	// Token: 0x04001DB4 RID: 7604
	public BlendedEngineLoopDefinition[] engineAudioLoops;

	// Token: 0x04001DB5 RID: 7605
	public int priority;

	// Token: 0x04001DB6 RID: 7606
	public float idleVolume = 0.4f;

	// Token: 0x04001DB7 RID: 7607
	public float maxVolume = 0.6f;

	// Token: 0x04001DB8 RID: 7608
	public float volumeChangeRateUp = 48f;

	// Token: 0x04001DB9 RID: 7609
	public float volumeChangeRateDown = 16f;

	// Token: 0x04001DBA RID: 7610
	public float idlePitch = 0.25f;

	// Token: 0x04001DBB RID: 7611
	public float maxPitch = 1.5f;

	// Token: 0x04001DBC RID: 7612
	public float idleRpm = 600f;

	// Token: 0x04001DBD RID: 7613
	public float gearUpRpm = 5000f;

	// Token: 0x04001DBE RID: 7614
	public float gearDownRpm = 2500f;

	// Token: 0x04001DBF RID: 7615
	public int numGears = 5;

	// Token: 0x04001DC0 RID: 7616
	public float maxRpm = 6000f;

	// Token: 0x04001DC1 RID: 7617
	public float gearUpRpmRate = 5f;

	// Token: 0x04001DC2 RID: 7618
	public float gearDownRpmRate = 6f;

	// Token: 0x04001DC3 RID: 7619
	public SoundDefinition badPerformanceLoop;
}
