using System;
using UnityEngine;

// Token: 0x02000328 RID: 808
public class MuzzleFlash_Flamelet : MonoBehaviour
{
	// Token: 0x06001DD8 RID: 7640 RVA: 0x000CB4E0 File Offset: 0x000C96E0
	private void OnEnable()
	{
		this.flameletParticle.shape.angle = (float)UnityEngine.Random.Range(6, 13);
		float num = UnityEngine.Random.Range(7f, 9f);
		this.flameletParticle.startSpeed = UnityEngine.Random.Range(2.5f, num);
		this.flameletParticle.startSize = UnityEngine.Random.Range(0.05f, num * 0.015f);
	}

	// Token: 0x0400177F RID: 6015
	public ParticleSystem flameletParticle;
}
