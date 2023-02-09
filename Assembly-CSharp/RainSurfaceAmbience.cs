using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200020D RID: 525
public class RainSurfaceAmbience : SingletonComponent<RainSurfaceAmbience>, IClientComponent
{
	// Token: 0x040012F5 RID: 4853
	public List<RainSurfaceAmbience.SurfaceSound> surfaces = new List<RainSurfaceAmbience.SurfaceSound>();

	// Token: 0x040012F6 RID: 4854
	public GameObjectRef emitterPrefab;

	// Token: 0x040012F7 RID: 4855
	public Dictionary<ParticlePatch, AmbienceEmitter> spawnedEmitters = new Dictionary<ParticlePatch, AmbienceEmitter>();

	// Token: 0x02000C1B RID: 3099
	[Serializable]
	public class SurfaceSound
	{
		// Token: 0x040040B7 RID: 16567
		public AmbienceDefinitionList baseAmbience;

		// Token: 0x040040B8 RID: 16568
		public List<PhysicMaterial> materials = new List<PhysicMaterial>();
	}
}
