using System;
using UnityEngine;

// Token: 0x02000647 RID: 1607
public class DungeonGridLink : MonoBehaviour
{
	// Token: 0x06002E03 RID: 11779 RVA: 0x00114380 File Offset: 0x00112580
	protected void Start()
	{
		if (TerrainMeta.Path == null)
		{
			return;
		}
		DungeonGridInfo dungeonGridInfo = TerrainMeta.Path.FindClosest<DungeonGridInfo>(TerrainMeta.Path.DungeonGridEntrances, base.transform.position);
		if (dungeonGridInfo == null)
		{
			return;
		}
		dungeonGridInfo.Links.Add(base.gameObject);
	}

	// Token: 0x040025A4 RID: 9636
	public Transform UpSocket;

	// Token: 0x040025A5 RID: 9637
	public Transform DownSocket;

	// Token: 0x040025A6 RID: 9638
	public DungeonGridLinkType UpType;

	// Token: 0x040025A7 RID: 9639
	public DungeonGridLinkType DownType;

	// Token: 0x040025A8 RID: 9640
	public int Priority;

	// Token: 0x040025A9 RID: 9641
	public int Rotation;
}
