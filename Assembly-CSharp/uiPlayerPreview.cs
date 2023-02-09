using System;
using UnityEngine;

// Token: 0x02000879 RID: 2169
public class uiPlayerPreview : SingletonComponent<uiPlayerPreview>
{
	// Token: 0x04003029 RID: 12329
	public Camera previewCamera;

	// Token: 0x0400302A RID: 12330
	public PlayerModel playermodel;

	// Token: 0x0400302B RID: 12331
	public ReflectionProbe reflectionProbe;

	// Token: 0x0400302C RID: 12332
	public SegmentMaskPositioning segmentMask;
}
