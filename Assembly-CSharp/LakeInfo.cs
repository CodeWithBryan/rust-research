using System;
using UnityEngine;

// Token: 0x0200051F RID: 1311
public class LakeInfo : MonoBehaviour
{
	// Token: 0x06002856 RID: 10326 RVA: 0x000F6207 File Offset: 0x000F4407
	protected void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.LakeObjs.Add(this);
		}
	}
}
