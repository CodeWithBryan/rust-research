using System;
using UnityEngine;

// Token: 0x020008E3 RID: 2275
public class RotateCameraAroundObject : MonoBehaviour
{
	// Token: 0x0600367E RID: 13950 RVA: 0x00144468 File Offset: 0x00142668
	private void FixedUpdate()
	{
		if (this.m_goObjectToRotateAround != null)
		{
			base.transform.LookAt(this.m_goObjectToRotateAround.transform.position + Vector3.up * 0.75f);
			base.transform.Translate(Vector3.right * this.m_flRotateSpeed * Time.deltaTime);
		}
	}

	// Token: 0x04003188 RID: 12680
	public GameObject m_goObjectToRotateAround;

	// Token: 0x04003189 RID: 12681
	public float m_flRotateSpeed = 10f;
}
