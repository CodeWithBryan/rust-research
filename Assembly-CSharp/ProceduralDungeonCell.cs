using System;
using UnityEngine;

// Token: 0x0200018B RID: 395
public class ProceduralDungeonCell : BaseMonoBehaviour
{
	// Token: 0x06001726 RID: 5926 RVA: 0x000ADBDC File Offset: 0x000ABDDC
	public void Awake()
	{
		this.spawnGroups = base.GetComponentsInChildren<SpawnGroup>();
	}

	// Token: 0x0400103A RID: 4154
	public bool north;

	// Token: 0x0400103B RID: 4155
	public bool east;

	// Token: 0x0400103C RID: 4156
	public bool south;

	// Token: 0x0400103D RID: 4157
	public bool west;

	// Token: 0x0400103E RID: 4158
	public bool entrance;

	// Token: 0x0400103F RID: 4159
	public bool hasSpawn;

	// Token: 0x04001040 RID: 4160
	public Transform exitPointHack;

	// Token: 0x04001041 RID: 4161
	public SpawnGroup[] spawnGroups;

	// Token: 0x04001042 RID: 4162
	public MeshRenderer[] mapRenderers;
}
