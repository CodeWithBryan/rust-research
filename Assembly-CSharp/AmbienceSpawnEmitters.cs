using System;
using UnityEngine;

// Token: 0x0200020B RID: 523
public class AmbienceSpawnEmitters : MonoBehaviour, IClientComponent
{
	// Token: 0x040012ED RID: 4845
	public int baseEmitterCount = 5;

	// Token: 0x040012EE RID: 4846
	public int baseEmitterDistance = 10;

	// Token: 0x040012EF RID: 4847
	public GameObjectRef emitterPrefab;
}
