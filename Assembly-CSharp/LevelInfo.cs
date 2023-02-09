using System;
using UnityEngine;

// Token: 0x02000521 RID: 1313
public class LevelInfo : SingletonComponent<LevelInfo>
{
	// Token: 0x040020CF RID: 8399
	public string shortName;

	// Token: 0x040020D0 RID: 8400
	public string displayName;

	// Token: 0x040020D1 RID: 8401
	[TextArea]
	public string description;

	// Token: 0x040020D2 RID: 8402
	[Tooltip("A background image to be shown when loading the map")]
	public Texture2D image;

	// Token: 0x040020D3 RID: 8403
	[Space(10f)]
	[Tooltip("You should incrememnt this version when you make changes to the map that will invalidate old saves")]
	public int version = 1;
}
