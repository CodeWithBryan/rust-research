using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000307 RID: 775
public class GameContentList : MonoBehaviour
{
	// Token: 0x0400170F RID: 5903
	public GameContentList.ResourceType resourceType;

	// Token: 0x04001710 RID: 5904
	public List<UnityEngine.Object> foundObjects;

	// Token: 0x02000C56 RID: 3158
	public enum ResourceType
	{
		// Token: 0x040041ED RID: 16877
		Audio,
		// Token: 0x040041EE RID: 16878
		Textures,
		// Token: 0x040041EF RID: 16879
		Models
	}
}
