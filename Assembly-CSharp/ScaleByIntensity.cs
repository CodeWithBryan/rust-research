using System;
using UnityEngine;

// Token: 0x020002C7 RID: 711
public class ScaleByIntensity : MonoBehaviour
{
	// Token: 0x06001CB0 RID: 7344 RVA: 0x000C4CE9 File Offset: 0x000C2EE9
	private void Start()
	{
		this.initialScale = base.transform.localScale;
	}

	// Token: 0x06001CB1 RID: 7345 RVA: 0x000C4CFC File Offset: 0x000C2EFC
	private void Update()
	{
		base.transform.localScale = (this.intensitySource.enabled ? (this.initialScale * this.intensitySource.intensity / this.maxIntensity) : Vector3.zero);
	}

	// Token: 0x0400164F RID: 5711
	public Vector3 initialScale = Vector3.zero;

	// Token: 0x04001650 RID: 5712
	public Light intensitySource;

	// Token: 0x04001651 RID: 5713
	public float maxIntensity = 1f;
}
