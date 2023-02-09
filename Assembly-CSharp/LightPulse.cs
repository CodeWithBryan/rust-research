using System;
using UnityEngine;

// Token: 0x02000326 RID: 806
public class LightPulse : MonoBehaviour, IClientComponent
{
	// Token: 0x04001777 RID: 6007
	public Light TargetLight;

	// Token: 0x04001778 RID: 6008
	public float PulseSpeed = 1f;

	// Token: 0x04001779 RID: 6009
	public float Lifetime = 3f;

	// Token: 0x0400177A RID: 6010
	public float MaxIntensity = 3f;

	// Token: 0x0400177B RID: 6011
	public float FadeOutSpeed = 2f;
}
