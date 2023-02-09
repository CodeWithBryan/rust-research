using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200063B RID: 1595
[Serializable]
public class DungeonBaseFloor
{
	// Token: 0x06002DEC RID: 11756 RVA: 0x00113EFB File Offset: 0x001120FB
	public float Distance(Vector3 position)
	{
		return Mathf.Abs(this.Links[0].transform.position.y - position.y);
	}

	// Token: 0x06002DED RID: 11757 RVA: 0x00113F24 File Offset: 0x00112124
	public float SignedDistance(Vector3 position)
	{
		return this.Links[0].transform.position.y - position.y;
	}

	// Token: 0x04002571 RID: 9585
	public List<DungeonBaseLink> Links = new List<DungeonBaseLink>();
}
