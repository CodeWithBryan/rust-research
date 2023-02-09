using System;
using UnityEngine;

// Token: 0x02000017 RID: 23
public class PatternFireworkStar : MonoBehaviour, IClientComponent
{
	// Token: 0x0600004C RID: 76 RVA: 0x00003444 File Offset: 0x00001644
	public void Initialize(Color color)
	{
		if (this.Pixel != null)
		{
			this.Pixel.SetActive(true);
		}
		if (this.Explosion != null)
		{
			this.Explosion.SetActive(false);
		}
		if (this.ParticleSystems != null)
		{
			foreach (ParticleSystem particleSystem in this.ParticleSystems)
			{
				if (!(particleSystem == null))
				{
					particleSystem.main.startColor = new ParticleSystem.MinMaxGradient(color);
				}
			}
		}
	}

	// Token: 0x0600004D RID: 77 RVA: 0x000034C3 File Offset: 0x000016C3
	public void Explode()
	{
		if (this.Pixel != null)
		{
			this.Pixel.SetActive(false);
		}
		if (this.Explosion != null)
		{
			this.Explosion.SetActive(true);
		}
	}

	// Token: 0x04000055 RID: 85
	public GameObject Pixel;

	// Token: 0x04000056 RID: 86
	public GameObject Explosion;

	// Token: 0x04000057 RID: 87
	public ParticleSystem[] ParticleSystems;
}
