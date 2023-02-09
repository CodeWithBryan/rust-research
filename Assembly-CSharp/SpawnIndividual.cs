using System;
using UnityEngine;

// Token: 0x0200053D RID: 1341
public struct SpawnIndividual
{
	// Token: 0x060028E5 RID: 10469 RVA: 0x000F9490 File Offset: 0x000F7690
	public SpawnIndividual(uint prefabID, Vector3 position, Quaternion rotation)
	{
		this.PrefabID = prefabID;
		this.Position = position;
		this.Rotation = rotation;
	}

	// Token: 0x04002132 RID: 8498
	public uint PrefabID;

	// Token: 0x04002133 RID: 8499
	public Vector3 Position;

	// Token: 0x04002134 RID: 8500
	public Quaternion Rotation;
}
