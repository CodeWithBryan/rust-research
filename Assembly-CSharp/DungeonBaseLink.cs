using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200063D RID: 1597
public class DungeonBaseLink : MonoBehaviour
{
	// Token: 0x1700038C RID: 908
	// (get) Token: 0x06002DF2 RID: 11762 RVA: 0x0011406B File Offset: 0x0011226B
	internal List<DungeonBaseSocket> Sockets
	{
		get
		{
			if (this.sockets == null)
			{
				this.sockets = new List<DungeonBaseSocket>();
				base.GetComponentsInChildren<DungeonBaseSocket>(true, this.sockets);
			}
			return this.sockets;
		}
	}

	// Token: 0x1700038D RID: 909
	// (get) Token: 0x06002DF3 RID: 11763 RVA: 0x00114093 File Offset: 0x00112293
	internal List<DungeonVolume> Volumes
	{
		get
		{
			if (this.volumes == null)
			{
				this.volumes = new List<DungeonVolume>();
				base.GetComponentsInChildren<DungeonVolume>(true, this.volumes);
			}
			return this.volumes;
		}
	}

	// Token: 0x06002DF4 RID: 11764 RVA: 0x001140BC File Offset: 0x001122BC
	protected void Start()
	{
		if (TerrainMeta.Path == null)
		{
			return;
		}
		this.Dungeon = TerrainMeta.Path.FindClosest<DungeonBaseInfo>(TerrainMeta.Path.DungeonBaseEntrances, base.transform.position);
		if (this.Dungeon == null)
		{
			return;
		}
		this.Dungeon.Add(this);
	}

	// Token: 0x04002574 RID: 9588
	public DungeonBaseLinkType Type;

	// Token: 0x04002575 RID: 9589
	public int Cost = 1;

	// Token: 0x04002576 RID: 9590
	public int MaxFloor = -1;

	// Token: 0x04002577 RID: 9591
	public int MaxCountLocal = -1;

	// Token: 0x04002578 RID: 9592
	public int MaxCountGlobal = -1;

	// Token: 0x04002579 RID: 9593
	[Tooltip("If set to a positive number, all segments with the same MaxCountIdentifier are counted towards MaxCountLocal and MaxCountGlobal")]
	public int MaxCountIdentifier = -1;

	// Token: 0x0400257A RID: 9594
	internal DungeonBaseInfo Dungeon;

	// Token: 0x0400257B RID: 9595
	public MeshRenderer[] MapRenderers;

	// Token: 0x0400257C RID: 9596
	private List<DungeonBaseSocket> sockets;

	// Token: 0x0400257D RID: 9597
	private List<DungeonVolume> volumes;
}
