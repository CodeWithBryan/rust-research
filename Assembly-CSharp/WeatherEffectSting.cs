using System;

// Token: 0x02000584 RID: 1412
public abstract class WeatherEffectSting : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x04002244 RID: 8772
	public float frequency = 600f;

	// Token: 0x04002245 RID: 8773
	public float variance = 300f;

	// Token: 0x04002246 RID: 8774
	public GameObjectRef[] effects;
}
