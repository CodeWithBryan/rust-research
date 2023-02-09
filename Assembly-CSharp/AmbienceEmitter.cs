using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000208 RID: 520
public class AmbienceEmitter : MonoBehaviour, IClientComponent, IComparable<AmbienceEmitter>
{
	// Token: 0x17000204 RID: 516
	// (get) Token: 0x06001A88 RID: 6792 RVA: 0x000BB75F File Offset: 0x000B995F
	// (set) Token: 0x06001A89 RID: 6793 RVA: 0x000BB767 File Offset: 0x000B9967
	public TerrainTopology.Enum currentTopology { get; private set; }

	// Token: 0x17000205 RID: 517
	// (get) Token: 0x06001A8A RID: 6794 RVA: 0x000BB770 File Offset: 0x000B9970
	// (set) Token: 0x06001A8B RID: 6795 RVA: 0x000BB778 File Offset: 0x000B9978
	public TerrainBiome.Enum currentBiome { get; private set; }

	// Token: 0x06001A8C RID: 6796 RVA: 0x000BB781 File Offset: 0x000B9981
	public int CompareTo(AmbienceEmitter other)
	{
		return this.cameraDistanceSq.CompareTo(other.cameraDistanceSq);
	}

	// Token: 0x040012CF RID: 4815
	public AmbienceDefinitionList baseAmbience;

	// Token: 0x040012D0 RID: 4816
	public AmbienceDefinitionList stings;

	// Token: 0x040012D1 RID: 4817
	public bool isStatic = true;

	// Token: 0x040012D2 RID: 4818
	public bool followCamera;

	// Token: 0x040012D3 RID: 4819
	public bool isBaseEmitter;

	// Token: 0x040012D4 RID: 4820
	public bool active;

	// Token: 0x040012D5 RID: 4821
	public float cameraDistanceSq = float.PositiveInfinity;

	// Token: 0x040012D6 RID: 4822
	public BoundingSphere boundingSphere;

	// Token: 0x040012D7 RID: 4823
	public float crossfadeTime = 2f;

	// Token: 0x040012DA RID: 4826
	public Dictionary<AmbienceDefinition, float> nextStingTime = new Dictionary<AmbienceDefinition, float>();

	// Token: 0x040012DB RID: 4827
	public float deactivateTime = float.PositiveInfinity;

	// Token: 0x040012DC RID: 4828
	public bool playUnderwater = true;

	// Token: 0x040012DD RID: 4829
	public bool playAbovewater = true;
}
