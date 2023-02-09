using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007C6 RID: 1990
public class MapInterface : SingletonComponent<MapInterface>
{
	// Token: 0x04002C18 RID: 11288
	public static bool IsOpen;

	// Token: 0x04002C19 RID: 11289
	public Image cameraPositon;

	// Token: 0x04002C1A RID: 11290
	public ScrollRectEx scrollRect;

	// Token: 0x04002C1B RID: 11291
	public Toggle showGridToggle;

	// Token: 0x04002C1C RID: 11292
	public Button FocusButton;

	// Token: 0x04002C1D RID: 11293
	public CanvasGroup CanvasGroup;

	// Token: 0x04002C1E RID: 11294
	public SoundDefinition PlaceMarkerSound;

	// Token: 0x04002C1F RID: 11295
	public SoundDefinition ClearMarkerSound;

	// Token: 0x04002C20 RID: 11296
	public MapView View;

	// Token: 0x04002C21 RID: 11297
	public Color[] PointOfInterestColours;

	// Token: 0x04002C22 RID: 11298
	public Sprite[] PointOfInterestSprites;

	// Token: 0x04002C23 RID: 11299
	public bool DebugStayOpen;
}
