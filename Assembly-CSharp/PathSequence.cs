using System;
using System.Collections.Generic;

// Token: 0x02000656 RID: 1622
public class PathSequence : PrefabAttribute
{
	// Token: 0x06002E21 RID: 11809 RVA: 0x00114BC0 File Offset: 0x00112DC0
	protected override Type GetIndexedType()
	{
		return typeof(PathSequence);
	}

	// Token: 0x06002E22 RID: 11810 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void ApplySequenceReplacement(List<Prefab> sequence, ref Prefab replacement, Prefab[] possibleReplacements, int pathLength, int pathIndex)
	{
	}
}
