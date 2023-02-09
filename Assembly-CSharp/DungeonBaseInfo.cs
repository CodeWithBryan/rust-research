using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200063A RID: 1594
public class DungeonBaseInfo : LandmarkInfo
{
	// Token: 0x06002DE5 RID: 11749 RVA: 0x00113D60 File Offset: 0x00111F60
	public float Distance(Vector3 position)
	{
		return (base.transform.position - position).magnitude;
	}

	// Token: 0x06002DE6 RID: 11750 RVA: 0x00113D88 File Offset: 0x00111F88
	public float SqrDistance(Vector3 position)
	{
		return (base.transform.position - position).sqrMagnitude;
	}

	// Token: 0x06002DE7 RID: 11751 RVA: 0x00113DB0 File Offset: 0x00111FB0
	public void Add(DungeonBaseLink link)
	{
		this.Links.Add(link.gameObject);
		if (link.Type == DungeonBaseLinkType.End)
		{
			return;
		}
		DungeonBaseFloor dungeonBaseFloor = null;
		float num = float.MaxValue;
		for (int i = 0; i < this.Floors.Count; i++)
		{
			DungeonBaseFloor dungeonBaseFloor2 = this.Floors[i];
			float num2 = dungeonBaseFloor2.Distance(link.transform.position);
			if (num2 < 1f && num2 < num)
			{
				dungeonBaseFloor = dungeonBaseFloor2;
				num = num2;
			}
		}
		if (dungeonBaseFloor == null)
		{
			dungeonBaseFloor = new DungeonBaseFloor();
			dungeonBaseFloor.Links.Add(link);
			this.Floors.Add(dungeonBaseFloor);
			this.Floors.Sort((DungeonBaseFloor l, DungeonBaseFloor r) => l.SignedDistance(base.transform.position).CompareTo(r.SignedDistance(base.transform.position)));
			return;
		}
		dungeonBaseFloor.Links.Add(link);
	}

	// Token: 0x06002DE8 RID: 11752 RVA: 0x00113E6E File Offset: 0x0011206E
	protected override void Awake()
	{
		base.Awake();
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.DungeonBaseEntrances.Add(this);
		}
	}

	// Token: 0x06002DE9 RID: 11753 RVA: 0x00113E92 File Offset: 0x00112092
	protected void Start()
	{
		base.transform.SetHierarchyGroup("DungeonBase", true, false);
	}

	// Token: 0x0400256F RID: 9583
	internal List<GameObject> Links = new List<GameObject>();

	// Token: 0x04002570 RID: 9584
	internal List<DungeonBaseFloor> Floors = new List<DungeonBaseFloor>();
}
