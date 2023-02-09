using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008B0 RID: 2224
public class GridLayoutGroupNeat : GridLayoutGroup
{
	// Token: 0x060035E9 RID: 13801 RVA: 0x00142D34 File Offset: 0x00140F34
	private float IdealCellWidth(float cellSize)
	{
		float num = base.rectTransform.rect.x + (float)(base.padding.left + base.padding.right) * 0.5f;
		float num2 = Mathf.Floor(num / cellSize);
		return num / num2 - this.m_Spacing.x;
	}

	// Token: 0x060035EA RID: 13802 RVA: 0x00142D8C File Offset: 0x00140F8C
	public override void SetLayoutHorizontal()
	{
		Vector2 cellSize = this.m_CellSize;
		this.m_CellSize.x = this.IdealCellWidth(cellSize.x);
		base.SetLayoutHorizontal();
		this.m_CellSize = cellSize;
	}

	// Token: 0x060035EB RID: 13803 RVA: 0x00142DC4 File Offset: 0x00140FC4
	public override void SetLayoutVertical()
	{
		Vector2 cellSize = this.m_CellSize;
		this.m_CellSize.x = this.IdealCellWidth(cellSize.x);
		base.SetLayoutVertical();
		this.m_CellSize = cellSize;
	}
}
