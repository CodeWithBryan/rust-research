using System;
using UnityEngine;

// Token: 0x02000329 RID: 809
public class Muzzleflash_AlphaRandom : MonoBehaviour
{
	// Token: 0x06001DDA RID: 7642 RVA: 0x000059DD File Offset: 0x00003BDD
	private void Start()
	{
	}

	// Token: 0x06001DDB RID: 7643 RVA: 0x000CB54C File Offset: 0x000C974C
	private void OnEnable()
	{
		this.gck[0].color = Color.white;
		this.gck[0].time = 0f;
		this.gck[1].color = Color.white;
		this.gck[1].time = 0.6f;
		this.gck[2].color = Color.black;
		this.gck[2].time = 0.75f;
		float alpha = UnityEngine.Random.Range(0.2f, 0.85f);
		this.gak[0].alpha = alpha;
		this.gak[0].time = 0f;
		this.gak[1].alpha = alpha;
		this.gak[1].time = 0.45f;
		this.gak[2].alpha = 0f;
		this.gak[2].time = 0.5f;
		this.grad.SetKeys(this.gck, this.gak);
		foreach (ParticleSystem particleSystem in this.muzzleflashParticles)
		{
			if (particleSystem == null)
			{
				Debug.LogWarning("Muzzleflash_AlphaRandom : null particle system in " + base.gameObject.name);
			}
			else
			{
				particleSystem.colorOverLifetime.color = this.grad;
			}
		}
	}

	// Token: 0x04001780 RID: 6016
	public ParticleSystem[] muzzleflashParticles;

	// Token: 0x04001781 RID: 6017
	private Gradient grad = new Gradient();

	// Token: 0x04001782 RID: 6018
	private GradientColorKey[] gck = new GradientColorKey[3];

	// Token: 0x04001783 RID: 6019
	private GradientAlphaKey[] gak = new GradientAlphaKey[3];
}
