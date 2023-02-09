using System;
using UnityEngine;

// Token: 0x0200068C RID: 1676
public class GenerateDecorTopology : ProceduralComponent
{
	// Token: 0x06002FD8 RID: 12248 RVA: 0x0011D848 File Offset: 0x0011BA48
	public override void Process(uint seed)
	{
		TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
		int topores = topomap.res;
		Parallel.For(0, topores, delegate(int z)
		{
			for (int i = 0; i < topores; i++)
			{
				if (topomap.GetTopology(i, z, 4194306))
				{
					topomap.AddTopology(i, z, 512);
				}
				else if (!this.KeepExisting)
				{
					topomap.RemoveTopology(i, z, 512);
				}
			}
		});
	}

	// Token: 0x0400268D RID: 9869
	public bool KeepExisting = true;
}
