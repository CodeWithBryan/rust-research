using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007D8 RID: 2008
public class UICompass : MonoBehaviour
{
	// Token: 0x04002C86 RID: 11398
	public RawImage compassStrip;

	// Token: 0x04002C87 RID: 11399
	public CanvasGroup compassGroup;

	// Token: 0x04002C88 RID: 11400
	public CompassMapMarker CompassMarker;

	// Token: 0x04002C89 RID: 11401
	public CompassMapMarker TeamLeaderCompassMarker;

	// Token: 0x04002C8A RID: 11402
	public List<CompassMissionMarker> MissionMarkers;
}
