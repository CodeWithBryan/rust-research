using System;
using UnityEngine;

// Token: 0x020005DE RID: 1502
public class LeavesBlowing : MonoBehaviour
{
	// Token: 0x06002C2A RID: 11306 RVA: 0x000059DD File Offset: 0x00003BDD
	private void Start()
	{
	}

	// Token: 0x06002C2B RID: 11307 RVA: 0x00109014 File Offset: 0x00107214
	private void Update()
	{
		base.transform.RotateAround(base.transform.position, Vector3.up, Time.deltaTime * this.m_flSwirl);
		if (this.m_psLeaves != null)
		{
			this.m_psLeaves.startSpeed = this.m_flSpeed;
			this.m_psLeaves.startSpeed += Mathf.Sin(Time.time * 0.4f) * (this.m_flSpeed * 0.75f);
			this.m_psLeaves.emissionRate = this.m_flEmissionRate + Mathf.Sin(Time.time * 1f) * (this.m_flEmissionRate * 0.3f);
		}
	}

	// Token: 0x040023FC RID: 9212
	public ParticleSystem m_psLeaves;

	// Token: 0x040023FD RID: 9213
	public float m_flSwirl;

	// Token: 0x040023FE RID: 9214
	public float m_flSpeed;

	// Token: 0x040023FF RID: 9215
	public float m_flEmissionRate;
}
