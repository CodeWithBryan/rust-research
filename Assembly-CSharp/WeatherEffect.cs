using System;
using UnityEngine;

// Token: 0x0200057E RID: 1406
public abstract class WeatherEffect : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x04002239 RID: 8761
	public ParticleSystem[] emitOnStart;

	// Token: 0x0400223A RID: 8762
	public ParticleSystem[] emitOnStop;

	// Token: 0x0400223B RID: 8763
	public ParticleSystem[] emitOnLoop;
}
