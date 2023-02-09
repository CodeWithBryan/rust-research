using System;
using ConVar;
using UnityEngine;

// Token: 0x020002CD RID: 717
public class SunSettings : MonoBehaviour, IClientComponent
{
	// Token: 0x06001CC3 RID: 7363 RVA: 0x000C514B File Offset: 0x000C334B
	private void OnEnable()
	{
		this.light = base.GetComponent<Light>();
	}

	// Token: 0x06001CC4 RID: 7364 RVA: 0x000C515C File Offset: 0x000C335C
	private void Update()
	{
		LightShadows lightShadows = (LightShadows)Mathf.Clamp(ConVar.Graphics.shadowmode, 1, 2);
		if (this.light.shadows != lightShadows)
		{
			this.light.shadows = lightShadows;
		}
	}

	// Token: 0x04001677 RID: 5751
	private Light light;
}
