using System;
using UnityEngine;

// Token: 0x0200020F RID: 527
public class BlendedLoopEngineSound : MonoBehaviour, IClientComponent
{
	// Token: 0x06001A95 RID: 6805 RVA: 0x000BB8FE File Offset: 0x000B9AFE
	public BlendedLoopEngineSound.EngineLoop[] GetEngineLoops()
	{
		return this.engineLoops;
	}

	// Token: 0x06001A96 RID: 6806 RVA: 0x000BB906 File Offset: 0x000B9B06
	public float GetLoopGain(int idx)
	{
		if (this.engineLoops != null && this.engineLoops[idx] != null && this.engineLoops[idx].gainMod != null)
		{
			return this.engineLoops[idx].gainMod.value;
		}
		return 0f;
	}

	// Token: 0x06001A97 RID: 6807 RVA: 0x000BB941 File Offset: 0x000B9B41
	public float GetLoopPitch(int idx)
	{
		if (this.engineLoops != null && this.engineLoops[idx] != null && this.engineLoops[idx].pitchMod != null)
		{
			return this.engineLoops[idx].pitchMod.value;
		}
		return 0f;
	}

	// Token: 0x17000206 RID: 518
	// (get) Token: 0x06001A98 RID: 6808 RVA: 0x000BB97C File Offset: 0x000B9B7C
	public float maxDistance
	{
		get
		{
			return this.loopDefinition.engineLoops[0].soundDefinition.maxDistance;
		}
	}

	// Token: 0x040012FD RID: 4861
	public BlendedEngineLoopDefinition loopDefinition;

	// Token: 0x040012FE RID: 4862
	public bool engineOn;

	// Token: 0x040012FF RID: 4863
	[Range(0f, 1f)]
	public float RPMControl;

	// Token: 0x04001300 RID: 4864
	public float smoothedRPMControl;

	// Token: 0x04001301 RID: 4865
	private BlendedLoopEngineSound.EngineLoop[] engineLoops;

	// Token: 0x02000C1D RID: 3101
	public class EngineLoop
	{
		// Token: 0x040040BF RID: 16575
		public BlendedEngineLoopDefinition.EngineLoopDefinition definition;

		// Token: 0x040040C0 RID: 16576
		public BlendedLoopEngineSound parent;

		// Token: 0x040040C1 RID: 16577
		public Sound sound;

		// Token: 0x040040C2 RID: 16578
		public SoundModulation.Modulator gainMod;

		// Token: 0x040040C3 RID: 16579
		public SoundModulation.Modulator pitchMod;
	}
}
