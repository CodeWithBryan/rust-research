using System;
using UnityEngine;

// Token: 0x02000197 RID: 407
public class rottest : MonoBehaviour
{
	// Token: 0x06001789 RID: 6025 RVA: 0x000059DD File Offset: 0x00003BDD
	private void Start()
	{
	}

	// Token: 0x0600178A RID: 6026 RVA: 0x000AFC0E File Offset: 0x000ADE0E
	private void Update()
	{
		this.aimDir = new Vector3(0f, 45f * Mathf.Sin(Time.time * 6f), 0f);
		this.UpdateAiming();
	}

	// Token: 0x0600178B RID: 6027 RVA: 0x000AFC44 File Offset: 0x000ADE44
	public void UpdateAiming()
	{
		if (this.aimDir == Vector3.zero)
		{
			return;
		}
		Quaternion quaternion = Quaternion.Euler(0f, this.aimDir.y, 0f);
		if (base.transform.localRotation != quaternion)
		{
			base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, quaternion, Time.deltaTime * 8f);
		}
	}

	// Token: 0x04001081 RID: 4225
	public Transform turretBase;

	// Token: 0x04001082 RID: 4226
	public Vector3 aimDir;
}
