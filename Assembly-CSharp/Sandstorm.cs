using System;
using UnityEngine;

// Token: 0x0200032F RID: 815
public class Sandstorm : MonoBehaviour
{
	// Token: 0x06001DE4 RID: 7652 RVA: 0x000059DD File Offset: 0x00003BDD
	private void Start()
	{
	}

	// Token: 0x06001DE5 RID: 7653 RVA: 0x000CB884 File Offset: 0x000C9A84
	private void Update()
	{
		base.transform.RotateAround(base.transform.position, Vector3.up, Time.deltaTime * this.m_flSwirl);
		Vector3 eulerAngles = base.transform.eulerAngles;
		eulerAngles.x = -7f + Mathf.Sin(Time.time * 2.5f) * 7f;
		base.transform.eulerAngles = eulerAngles;
		if (this.m_psSandStorm != null)
		{
			this.m_psSandStorm.startSpeed = this.m_flSpeed;
			this.m_psSandStorm.startSpeed += Mathf.Sin(Time.time * 0.4f) * (this.m_flSpeed * 0.75f);
			this.m_psSandStorm.emissionRate = this.m_flEmissionRate + Mathf.Sin(Time.time * 1f) * (this.m_flEmissionRate * 0.3f);
		}
	}

	// Token: 0x0400179E RID: 6046
	public ParticleSystem m_psSandStorm;

	// Token: 0x0400179F RID: 6047
	public float m_flSpeed;

	// Token: 0x040017A0 RID: 6048
	public float m_flSwirl;

	// Token: 0x040017A1 RID: 6049
	public float m_flEmissionRate;
}
