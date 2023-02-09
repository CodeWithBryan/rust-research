using System;
using UnityEngine;

// Token: 0x02000950 RID: 2384
public class flamethrowerFire : MonoBehaviour
{
	// Token: 0x0600384B RID: 14411 RVA: 0x0014C964 File Offset: 0x0014AB64
	public void PilotLightOn()
	{
		this.pilotLightFX.enableEmission = true;
		this.SetFlameStatus(false);
	}

	// Token: 0x0600384C RID: 14412 RVA: 0x0014C97C File Offset: 0x0014AB7C
	public void SetFlameStatus(bool status)
	{
		ParticleSystem[] array = this.flameFX;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = status;
		}
	}

	// Token: 0x0600384D RID: 14413 RVA: 0x0014C9A7 File Offset: 0x0014ABA7
	public void ShutOff()
	{
		this.pilotLightFX.enableEmission = false;
		this.SetFlameStatus(false);
	}

	// Token: 0x0600384E RID: 14414 RVA: 0x0014C9BC File Offset: 0x0014ABBC
	public void FlameOn()
	{
		this.pilotLightFX.enableEmission = false;
		this.SetFlameStatus(true);
	}

	// Token: 0x0600384F RID: 14415 RVA: 0x0014C9D4 File Offset: 0x0014ABD4
	private void Start()
	{
		this.previousflameState = (this.flameState = flamethrowerState.OFF);
	}

	// Token: 0x06003850 RID: 14416 RVA: 0x0014C9F4 File Offset: 0x0014ABF4
	private void Update()
	{
		if (this.previousflameState != this.flameState)
		{
			switch (this.flameState)
			{
			case flamethrowerState.OFF:
				this.ShutOff();
				break;
			case flamethrowerState.PILOT_LIGHT:
				this.PilotLightOn();
				break;
			case flamethrowerState.FLAME_ON:
				this.FlameOn();
				break;
			}
			this.previousflameState = this.flameState;
			this.jet.SetOn(this.flameState == flamethrowerState.FLAME_ON);
		}
	}

	// Token: 0x04003285 RID: 12933
	public ParticleSystem pilotLightFX;

	// Token: 0x04003286 RID: 12934
	public ParticleSystem[] flameFX;

	// Token: 0x04003287 RID: 12935
	public FlameJet jet;

	// Token: 0x04003288 RID: 12936
	public AudioSource oneShotSound;

	// Token: 0x04003289 RID: 12937
	public AudioSource loopSound;

	// Token: 0x0400328A RID: 12938
	public AudioClip pilotlightIdle;

	// Token: 0x0400328B RID: 12939
	public AudioClip flameLoop;

	// Token: 0x0400328C RID: 12940
	public AudioClip flameStart;

	// Token: 0x0400328D RID: 12941
	public flamethrowerState flameState;

	// Token: 0x0400328E RID: 12942
	private flamethrowerState previousflameState;
}
