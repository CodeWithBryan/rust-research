using System;
using UnityEngine;

// Token: 0x02000536 RID: 1334
public class RiverInfo : MonoBehaviour
{
	// Token: 0x060028C0 RID: 10432 RVA: 0x000F7F45 File Offset: 0x000F6145
	protected void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.RiverObjs.Add(this);
		}
	}
}
