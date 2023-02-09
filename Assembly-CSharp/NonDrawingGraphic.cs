using System;
using UnityEngine.UI;

// Token: 0x020008B2 RID: 2226
public class NonDrawingGraphic : Graphic
{
	// Token: 0x060035F2 RID: 13810 RVA: 0x000059DD File Offset: 0x00003BDD
	public override void SetMaterialDirty()
	{
	}

	// Token: 0x060035F3 RID: 13811 RVA: 0x000059DD File Offset: 0x00003BDD
	public override void SetVerticesDirty()
	{
	}

	// Token: 0x060035F4 RID: 13812 RVA: 0x00142E26 File Offset: 0x00141026
	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
	}
}
