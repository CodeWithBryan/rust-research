using System;
using UnityEngine;

// Token: 0x0200063C RID: 1596
[RequireComponent(typeof(DungeonBaseLink))]
public class DungeonBaseLandmarkInfo : LandmarkInfo
{
	// Token: 0x06002DEF RID: 11759 RVA: 0x00113F5B File Offset: 0x0011215B
	protected override void Awake()
	{
		base.Awake();
		this.baseLink = base.GetComponent<DungeonBaseLink>();
	}

	// Token: 0x1700038B RID: 907
	// (get) Token: 0x06002DF0 RID: 11760 RVA: 0x00113F70 File Offset: 0x00112170
	public override MapLayer MapLayer
	{
		get
		{
			if (this.layer != null)
			{
				return this.layer.Value;
			}
			DungeonBaseInfo dungeonBaseInfo = TerrainMeta.Path.FindClosest<DungeonBaseInfo>(TerrainMeta.Path.DungeonBaseEntrances, this.baseLink.transform.position);
			if (dungeonBaseInfo == null)
			{
				Debug.LogWarning("Couldn't determine which underwater lab a DungeonBaseLandmarkInfo belongs to", this);
				this.shouldDisplayOnMap = false;
				this.layer = new MapLayer?(MapLayer.Overworld);
				return this.layer.Value;
			}
			int num = -1;
			for (int i = 0; i < dungeonBaseInfo.Floors.Count; i++)
			{
				if (dungeonBaseInfo.Floors[i].Links.Contains(this.baseLink))
				{
					num = i;
				}
			}
			if (num >= 0)
			{
				this.layer = new MapLayer?(MapLayer.Underwater1 + num);
			}
			else
			{
				Debug.LogWarning("Couldn't determine the floor of a DungeonBaseLandmarkInfo", this);
				this.shouldDisplayOnMap = false;
				this.layer = new MapLayer?(MapLayer.Overworld);
			}
			return this.layer.Value;
		}
	}

	// Token: 0x04002572 RID: 9586
	private DungeonBaseLink baseLink;

	// Token: 0x04002573 RID: 9587
	private MapLayer? layer;
}
