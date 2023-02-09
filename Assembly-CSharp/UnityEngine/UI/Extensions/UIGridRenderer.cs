using System;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x020009F0 RID: 2544
	[AddComponentMenu("UI/Extensions/Primitives/UIGridRenderer")]
	public class UIGridRenderer : UILineRenderer
	{
		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x06003C4C RID: 15436 RVA: 0x0015FD5E File Offset: 0x0015DF5E
		// (set) Token: 0x06003C4D RID: 15437 RVA: 0x0015FD66 File Offset: 0x0015DF66
		public int GridColumns
		{
			get
			{
				return this.m_GridColumns;
			}
			set
			{
				if (this.m_GridColumns == value)
				{
					return;
				}
				this.m_GridColumns = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x170004C9 RID: 1225
		// (get) Token: 0x06003C4E RID: 15438 RVA: 0x0015FD7F File Offset: 0x0015DF7F
		// (set) Token: 0x06003C4F RID: 15439 RVA: 0x0015FD87 File Offset: 0x0015DF87
		public int GridRows
		{
			get
			{
				return this.m_GridRows;
			}
			set
			{
				if (this.m_GridRows == value)
				{
					return;
				}
				this.m_GridRows = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x06003C50 RID: 15440 RVA: 0x0015FDA0 File Offset: 0x0015DFA0
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			this.relativeSize = true;
			int num = this.GridRows * 3 + 1;
			if (this.GridRows % 2 == 0)
			{
				num++;
			}
			num += this.GridColumns * 3 + 1;
			this.m_points = new Vector2[num];
			int num2 = 0;
			for (int i = 0; i < this.GridRows; i++)
			{
				float x = 1f;
				float x2 = 0f;
				if (i % 2 == 0)
				{
					x = 0f;
					x2 = 1f;
				}
				float y = (float)i / (float)this.GridRows;
				this.m_points[num2].x = x;
				this.m_points[num2].y = y;
				num2++;
				this.m_points[num2].x = x2;
				this.m_points[num2].y = y;
				num2++;
				this.m_points[num2].x = x2;
				this.m_points[num2].y = (float)(i + 1) / (float)this.GridRows;
				num2++;
			}
			if (this.GridRows % 2 == 0)
			{
				this.m_points[num2].x = 1f;
				this.m_points[num2].y = 1f;
				num2++;
			}
			this.m_points[num2].x = 0f;
			this.m_points[num2].y = 1f;
			num2++;
			for (int j = 0; j < this.GridColumns; j++)
			{
				float y2 = 1f;
				float y3 = 0f;
				if (j % 2 == 0)
				{
					y2 = 0f;
					y3 = 1f;
				}
				float x3 = (float)j / (float)this.GridColumns;
				this.m_points[num2].x = x3;
				this.m_points[num2].y = y2;
				num2++;
				this.m_points[num2].x = x3;
				this.m_points[num2].y = y3;
				num2++;
				this.m_points[num2].x = (float)(j + 1) / (float)this.GridColumns;
				this.m_points[num2].y = y3;
				num2++;
			}
			if (this.GridColumns % 2 == 0)
			{
				this.m_points[num2].x = 1f;
				this.m_points[num2].y = 1f;
			}
			else
			{
				this.m_points[num2].x = 1f;
				this.m_points[num2].y = 0f;
			}
			base.OnPopulateMesh(vh);
		}

		// Token: 0x040035A3 RID: 13731
		[SerializeField]
		private int m_GridColumns = 10;

		// Token: 0x040035A4 RID: 13732
		[SerializeField]
		private int m_GridRows = 10;
	}
}
