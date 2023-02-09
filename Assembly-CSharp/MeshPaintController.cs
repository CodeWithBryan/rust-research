using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002B1 RID: 689
public class MeshPaintController : MonoBehaviour, IClientComponent
{
	// Token: 0x040015AC RID: 5548
	public Camera pickerCamera;

	// Token: 0x040015AD RID: 5549
	public Texture2D brushTexture;

	// Token: 0x040015AE RID: 5550
	public Vector2 brushScale = new Vector2(8f, 8f);

	// Token: 0x040015AF RID: 5551
	public Color brushColor = Color.white;

	// Token: 0x040015B0 RID: 5552
	public float brushSpacing = 2f;

	// Token: 0x040015B1 RID: 5553
	public RawImage brushImage;

	// Token: 0x040015B2 RID: 5554
	public float brushPreviewScaleMultiplier = 1f;

	// Token: 0x040015B3 RID: 5555
	public bool applyDefaults;

	// Token: 0x040015B4 RID: 5556
	public Texture2D defaltBrushTexture;

	// Token: 0x040015B5 RID: 5557
	public float defaultBrushSize = 16f;

	// Token: 0x040015B6 RID: 5558
	public Color defaultBrushColor = Color.black;

	// Token: 0x040015B7 RID: 5559
	public float defaultBrushAlpha = 0.5f;

	// Token: 0x040015B8 RID: 5560
	public Toggle lastBrush;

	// Token: 0x040015B9 RID: 5561
	public Button UndoButton;

	// Token: 0x040015BA RID: 5562
	public Button RedoButton;

	// Token: 0x040015BB RID: 5563
	private Vector3 lastPosition;
}
