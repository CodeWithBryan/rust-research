using System;
using UnityEngine;

// Token: 0x020008F2 RID: 2290
public static class ParticleSystemEx
{
	// Token: 0x060036A2 RID: 13986 RVA: 0x001452A6 File Offset: 0x001434A6
	public static void SetPlayingState(this ParticleSystem ps, bool play)
	{
		if (ps == null)
		{
			return;
		}
		if (play && !ps.isPlaying)
		{
			ps.Play();
			return;
		}
		if (!play && ps.isPlaying)
		{
			ps.Stop();
		}
	}

	// Token: 0x060036A3 RID: 13987 RVA: 0x001452D8 File Offset: 0x001434D8
	public static void SetEmitterState(this ParticleSystem ps, bool enable)
	{
		if (enable != ps.emission.enabled)
		{
			ps.emission.enabled = enable;
		}
	}
}
