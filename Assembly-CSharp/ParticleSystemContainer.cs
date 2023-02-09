using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020008D9 RID: 2265
public class ParticleSystemContainer : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x06003658 RID: 13912 RVA: 0x000059DD File Offset: 0x00003BDD
	public void Play()
	{
	}

	// Token: 0x06003659 RID: 13913 RVA: 0x000059DD File Offset: 0x00003BDD
	public void Pause()
	{
	}

	// Token: 0x0600365A RID: 13914 RVA: 0x000059DD File Offset: 0x00003BDD
	public void Stop()
	{
	}

	// Token: 0x0600365B RID: 13915 RVA: 0x000059DD File Offset: 0x00003BDD
	public void Clear()
	{
	}

	// Token: 0x0600365C RID: 13916 RVA: 0x00143FC4 File Offset: 0x001421C4
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (this.precached && clientside)
		{
			List<ParticleSystemContainer.ParticleSystemGroup> list = new List<ParticleSystemContainer.ParticleSystemGroup>();
			foreach (ParticleSystem particleSystem in base.GetComponentsInChildren<ParticleSystem>())
			{
				LODComponentParticleSystem[] components = particleSystem.GetComponents<LODComponentParticleSystem>();
				ParticleSystemContainer.ParticleSystemGroup item = new ParticleSystemContainer.ParticleSystemGroup
				{
					system = particleSystem,
					lodComponents = components
				};
				list.Add(item);
			}
			this.particleGroups = list.ToArray();
		}
	}

	// Token: 0x04003158 RID: 12632
	public bool precached;

	// Token: 0x04003159 RID: 12633
	[HideInInspector]
	public ParticleSystemContainer.ParticleSystemGroup[] particleGroups;

	// Token: 0x02000E57 RID: 3671
	[Serializable]
	public struct ParticleSystemGroup
	{
		// Token: 0x04004A1E RID: 18974
		public ParticleSystem system;

		// Token: 0x04004A1F RID: 18975
		public LODComponentParticleSystem[] lodComponents;
	}
}
