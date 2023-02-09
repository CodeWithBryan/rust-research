using System;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x020009EE RID: 2542
	[AddComponentMenu("UI/Extensions/Primitives/Cut Corners")]
	public class UICornerCut : UIPrimitiveBase
	{
		// Token: 0x170004BF RID: 1215
		// (get) Token: 0x06003C30 RID: 15408 RVA: 0x0015F1D4 File Offset: 0x0015D3D4
		// (set) Token: 0x06003C31 RID: 15409 RVA: 0x0015F1DC File Offset: 0x0015D3DC
		public bool CutUL
		{
			get
			{
				return this.m_cutUL;
			}
			set
			{
				this.m_cutUL = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x170004C0 RID: 1216
		// (get) Token: 0x06003C32 RID: 15410 RVA: 0x0015F1EB File Offset: 0x0015D3EB
		// (set) Token: 0x06003C33 RID: 15411 RVA: 0x0015F1F3 File Offset: 0x0015D3F3
		public bool CutUR
		{
			get
			{
				return this.m_cutUR;
			}
			set
			{
				this.m_cutUR = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x170004C1 RID: 1217
		// (get) Token: 0x06003C34 RID: 15412 RVA: 0x0015F202 File Offset: 0x0015D402
		// (set) Token: 0x06003C35 RID: 15413 RVA: 0x0015F20A File Offset: 0x0015D40A
		public bool CutLL
		{
			get
			{
				return this.m_cutLL;
			}
			set
			{
				this.m_cutLL = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x170004C2 RID: 1218
		// (get) Token: 0x06003C36 RID: 15414 RVA: 0x0015F219 File Offset: 0x0015D419
		// (set) Token: 0x06003C37 RID: 15415 RVA: 0x0015F221 File Offset: 0x0015D421
		public bool CutLR
		{
			get
			{
				return this.m_cutLR;
			}
			set
			{
				this.m_cutLR = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x170004C3 RID: 1219
		// (get) Token: 0x06003C38 RID: 15416 RVA: 0x0015F230 File Offset: 0x0015D430
		// (set) Token: 0x06003C39 RID: 15417 RVA: 0x0015F238 File Offset: 0x0015D438
		public bool MakeColumns
		{
			get
			{
				return this.m_makeColumns;
			}
			set
			{
				this.m_makeColumns = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x170004C4 RID: 1220
		// (get) Token: 0x06003C3A RID: 15418 RVA: 0x0015F247 File Offset: 0x0015D447
		// (set) Token: 0x06003C3B RID: 15419 RVA: 0x0015F24F File Offset: 0x0015D44F
		public bool UseColorUp
		{
			get
			{
				return this.m_useColorUp;
			}
			set
			{
				this.m_useColorUp = value;
			}
		}

		// Token: 0x170004C5 RID: 1221
		// (get) Token: 0x06003C3C RID: 15420 RVA: 0x0015F258 File Offset: 0x0015D458
		// (set) Token: 0x06003C3D RID: 15421 RVA: 0x0015F260 File Offset: 0x0015D460
		public Color32 ColorUp
		{
			get
			{
				return this.m_colorUp;
			}
			set
			{
				this.m_colorUp = value;
			}
		}

		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x06003C3E RID: 15422 RVA: 0x0015F269 File Offset: 0x0015D469
		// (set) Token: 0x06003C3F RID: 15423 RVA: 0x0015F271 File Offset: 0x0015D471
		public bool UseColorDown
		{
			get
			{
				return this.m_useColorDown;
			}
			set
			{
				this.m_useColorDown = value;
			}
		}

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x06003C40 RID: 15424 RVA: 0x0015F27A File Offset: 0x0015D47A
		// (set) Token: 0x06003C41 RID: 15425 RVA: 0x0015F282 File Offset: 0x0015D482
		public Color32 ColorDown
		{
			get
			{
				return this.m_colorDown;
			}
			set
			{
				this.m_colorDown = value;
			}
		}

		// Token: 0x06003C42 RID: 15426 RVA: 0x0015F28C File Offset: 0x0015D48C
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			Rect rect = base.rectTransform.rect;
			Rect rect2 = rect;
			Color32 color = this.color;
			bool flag = this.m_cutUL | this.m_cutUR;
			bool flag2 = this.m_cutLL | this.m_cutLR;
			bool flag3 = this.m_cutLL | this.m_cutUL;
			bool flag4 = this.m_cutLR | this.m_cutUR;
			if ((flag || flag2) && this.cornerSize.sqrMagnitude > 0f)
			{
				vh.Clear();
				if (flag3)
				{
					rect2.xMin += this.cornerSize.x;
				}
				if (flag2)
				{
					rect2.yMin += this.cornerSize.y;
				}
				if (flag)
				{
					rect2.yMax -= this.cornerSize.y;
				}
				if (flag4)
				{
					rect2.xMax -= this.cornerSize.x;
				}
				if (this.m_makeColumns)
				{
					Vector2 vector = new Vector2(rect.xMin, this.m_cutUL ? rect2.yMax : rect.yMax);
					Vector2 vector2 = new Vector2(rect.xMax, this.m_cutUR ? rect2.yMax : rect.yMax);
					Vector2 vector3 = new Vector2(rect.xMin, this.m_cutLL ? rect2.yMin : rect.yMin);
					Vector2 vector4 = new Vector2(rect.xMax, this.m_cutLR ? rect2.yMin : rect.yMin);
					if (flag3)
					{
						UICornerCut.AddSquare(vector3, vector, new Vector2(rect2.xMin, rect.yMax), new Vector2(rect2.xMin, rect.yMin), rect, this.m_useColorUp ? this.m_colorUp : color, vh);
					}
					if (flag4)
					{
						UICornerCut.AddSquare(vector2, vector4, new Vector2(rect2.xMax, rect.yMin), new Vector2(rect2.xMax, rect.yMax), rect, this.m_useColorDown ? this.m_colorDown : color, vh);
					}
				}
				else
				{
					Vector2 vector = new Vector2(this.m_cutUL ? rect2.xMin : rect.xMin, rect.yMax);
					Vector2 vector2 = new Vector2(this.m_cutUR ? rect2.xMax : rect.xMax, rect.yMax);
					Vector2 vector3 = new Vector2(this.m_cutLL ? rect2.xMin : rect.xMin, rect.yMin);
					Vector2 vector4 = new Vector2(this.m_cutLR ? rect2.xMax : rect.xMax, rect.yMin);
					if (flag2)
					{
						UICornerCut.AddSquare(vector4, vector3, new Vector2(rect.xMin, rect2.yMin), new Vector2(rect.xMax, rect2.yMin), rect, this.m_useColorDown ? this.m_colorDown : color, vh);
					}
					if (flag)
					{
						UICornerCut.AddSquare(vector, vector2, new Vector2(rect.xMax, rect2.yMax), new Vector2(rect.xMin, rect2.yMax), rect, this.m_useColorUp ? this.m_colorUp : color, vh);
					}
				}
				if (this.m_makeColumns)
				{
					UICornerCut.AddSquare(new Rect(rect2.xMin, rect.yMin, rect2.width, rect.height), rect, color, vh);
					return;
				}
				UICornerCut.AddSquare(new Rect(rect.xMin, rect2.yMin, rect.width, rect2.height), rect, color, vh);
			}
		}

		// Token: 0x06003C43 RID: 15427 RVA: 0x0015F63C File Offset: 0x0015D83C
		private static void AddSquare(Rect rect, Rect rectUV, Color32 color32, VertexHelper vh)
		{
			int num = UICornerCut.AddVert(rect.xMin, rect.yMin, rectUV, color32, vh);
			int idx = UICornerCut.AddVert(rect.xMin, rect.yMax, rectUV, color32, vh);
			int num2 = UICornerCut.AddVert(rect.xMax, rect.yMax, rectUV, color32, vh);
			int idx2 = UICornerCut.AddVert(rect.xMax, rect.yMin, rectUV, color32, vh);
			vh.AddTriangle(num, idx, num2);
			vh.AddTriangle(num2, idx2, num);
		}

		// Token: 0x06003C44 RID: 15428 RVA: 0x0015F6B8 File Offset: 0x0015D8B8
		private static void AddSquare(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Rect rectUV, Color32 color32, VertexHelper vh)
		{
			int num = UICornerCut.AddVert(a.x, a.y, rectUV, color32, vh);
			int idx = UICornerCut.AddVert(b.x, b.y, rectUV, color32, vh);
			int num2 = UICornerCut.AddVert(c.x, c.y, rectUV, color32, vh);
			int idx2 = UICornerCut.AddVert(d.x, d.y, rectUV, color32, vh);
			vh.AddTriangle(num, idx, num2);
			vh.AddTriangle(num2, idx2, num);
		}

		// Token: 0x06003C45 RID: 15429 RVA: 0x0015F73C File Offset: 0x0015D93C
		private static int AddVert(float x, float y, Rect area, Color32 color32, VertexHelper vh)
		{
			Vector2 uv = new Vector2(Mathf.InverseLerp(area.xMin, area.xMax, x), Mathf.InverseLerp(area.yMin, area.yMax, y));
			vh.AddVert(new Vector3(x, y), color32, uv);
			return vh.currentVertCount - 1;
		}

		// Token: 0x04003594 RID: 13716
		public Vector2 cornerSize = new Vector2(16f, 16f);

		// Token: 0x04003595 RID: 13717
		[Header("Corners to cut")]
		[SerializeField]
		private bool m_cutUL = true;

		// Token: 0x04003596 RID: 13718
		[SerializeField]
		private bool m_cutUR;

		// Token: 0x04003597 RID: 13719
		[SerializeField]
		private bool m_cutLL;

		// Token: 0x04003598 RID: 13720
		[SerializeField]
		private bool m_cutLR;

		// Token: 0x04003599 RID: 13721
		[Tooltip("Up-Down colors become Left-Right colors")]
		[SerializeField]
		private bool m_makeColumns;

		// Token: 0x0400359A RID: 13722
		[Header("Color the cut bars differently")]
		[SerializeField]
		private bool m_useColorUp;

		// Token: 0x0400359B RID: 13723
		[SerializeField]
		private Color32 m_colorUp;

		// Token: 0x0400359C RID: 13724
		[SerializeField]
		private bool m_useColorDown;

		// Token: 0x0400359D RID: 13725
		[SerializeField]
		private Color32 m_colorDown;
	}
}
