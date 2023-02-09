using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x020009EF RID: 2543
	public class UIGoldenRatioRenderer : UILineRenderer
	{
		// Token: 0x06003C47 RID: 15431 RVA: 0x0015F7B8 File Offset: 0x0015D9B8
		private void DrawSpiral(VertexHelper vh)
		{
			this._points.Clear();
			this._rects.Clear();
			float num = (1f + Mathf.Sqrt(5f)) / 2f;
			this.canvasWidth = (int)base.canvas.pixelRect.width;
			this.canvasHeight = (int)base.canvas.pixelRect.height;
			UIGoldenRatioRenderer.Orientations orientation;
			float num2;
			float num3;
			if (this.canvasWidth > this.canvasHeight)
			{
				orientation = UIGoldenRatioRenderer.Orientations.Left;
				if ((float)this.canvasWidth / (float)this.canvasHeight > num)
				{
					num2 = (float)this.canvasHeight;
					num3 = num2 * num;
				}
				else
				{
					num3 = (float)this.canvasWidth;
					num2 = num3 / num;
				}
			}
			else
			{
				orientation = UIGoldenRatioRenderer.Orientations.Top;
				if ((float)this.canvasHeight / (float)this.canvasWidth > num)
				{
					num3 = (float)this.canvasWidth;
					num2 = num3 * num;
				}
				else
				{
					num2 = (float)this.canvasHeight;
					num3 = num2 / num;
				}
			}
			float num4 = (float)(-(float)this.canvasWidth / 2);
			float num5 = (float)(this.canvasHeight / 2);
			num4 += ((float)this.canvasWidth - num3) / 2f;
			num5 += ((float)this.canvasHeight - num2) / 2f;
			List<Vector2> list = new List<Vector2>();
			this.DrawPhiRectangles(vh, list, num4, num5, num3, num2, orientation);
			if (list.Count > 1)
			{
				Vector2 vector = list[0];
				Vector2 vector2 = list[list.Count - 1];
				float num6 = vector.x - vector2.x;
				float num7 = vector.y - vector2.y;
				float num8 = Mathf.Sqrt(num6 * num6 + num7 * num7);
				float num9 = Mathf.Atan2(num7, num6);
				float num10 = 0.06283186f;
				float num11 = 1f - 1f / num / 25f * 0.78f;
				while (num8 > 32f)
				{
					Vector2 item = new Vector2(vector2.x + num8 * Mathf.Cos(num9), (float)this.canvasHeight - (vector2.y + num8 * Mathf.Sin(num9)));
					this._points.Add(item);
					num9 += num10;
					num8 *= num11;
				}
			}
		}

		// Token: 0x06003C48 RID: 15432 RVA: 0x0015F9C8 File Offset: 0x0015DBC8
		private void DrawPhiRectangles(VertexHelper vh, List<Vector2> points, float x, float y, float width, float height, UIGoldenRatioRenderer.Orientations orientation)
		{
			if (width < 1f || height < 1f)
			{
				return;
			}
			if (width >= 10f && height >= 10f)
			{
				this._rects.Add(new Rect(x, y, width, height));
			}
			switch (orientation)
			{
			case UIGoldenRatioRenderer.Orientations.Left:
				points.Add(new Vector2(x, y + height));
				x += height;
				width -= height;
				orientation = UIGoldenRatioRenderer.Orientations.Top;
				break;
			case UIGoldenRatioRenderer.Orientations.Top:
				points.Add(new Vector2(x, y));
				y += width;
				height -= width;
				orientation = UIGoldenRatioRenderer.Orientations.Right;
				break;
			case UIGoldenRatioRenderer.Orientations.Right:
				points.Add(new Vector2(x + width, y));
				width -= height;
				orientation = UIGoldenRatioRenderer.Orientations.Bottom;
				break;
			case UIGoldenRatioRenderer.Orientations.Bottom:
				points.Add(new Vector2(x + width, y + height));
				height -= width;
				orientation = UIGoldenRatioRenderer.Orientations.Left;
				break;
			}
			this.DrawPhiRectangles(vh, points, x, y, width, height, orientation);
		}

		// Token: 0x06003C49 RID: 15433 RVA: 0x0015FABC File Offset: 0x0015DCBC
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (base.canvas == null)
			{
				return;
			}
			this.relativeSize = false;
			this.DrawSpiral(vh);
			this.m_points = this._points.ToArray();
			base.OnPopulateMesh(vh);
			foreach (Rect rect in this._rects)
			{
				this.DrawRect(vh, new Rect(rect.x, rect.y - this.lineThickness2 * 0.5f, rect.width, this.lineThickness2));
				this.DrawRect(vh, new Rect(rect.x - this.lineThickness2 * 0.5f, rect.y, this.lineThickness2, rect.height));
				this.DrawRect(vh, new Rect(rect.x, rect.y + rect.height - this.lineThickness2 * 0.5f, rect.width, this.lineThickness2));
				this.DrawRect(vh, new Rect(rect.x + rect.width - this.lineThickness2 * 0.5f, rect.y, this.lineThickness2, rect.height));
			}
		}

		// Token: 0x06003C4A RID: 15434 RVA: 0x0015FC24 File Offset: 0x0015DE24
		private void DrawRect(VertexHelper vh, Rect rect)
		{
			Vector2[] array = new Vector2[]
			{
				new Vector2(rect.x, rect.y),
				new Vector2(rect.x + rect.width, rect.y),
				new Vector2(rect.x + rect.width, rect.y + rect.height),
				new Vector2(rect.x, rect.y + rect.height)
			};
			UIVertex[] array2 = new UIVertex[4];
			for (int i = 0; i < array2.Length; i++)
			{
				UIVertex simpleVert = UIVertex.simpleVert;
				simpleVert.color = this.color;
				simpleVert.position = array[i].WithY(base.canvas.pixelRect.height - array[i].y);
				array2[i] = simpleVert;
			}
			vh.AddUIVertexQuad(array2);
		}

		// Token: 0x0400359E RID: 13726
		private readonly List<Vector2> _points = new List<Vector2>();

		// Token: 0x0400359F RID: 13727
		private readonly List<Rect> _rects = new List<Rect>();

		// Token: 0x040035A0 RID: 13728
		private int canvasWidth;

		// Token: 0x040035A1 RID: 13729
		private int canvasHeight;

		// Token: 0x040035A2 RID: 13730
		public float lineThickness2 = 1f;

		// Token: 0x02000EAD RID: 3757
		private enum Orientations
		{
			// Token: 0x04004B8B RID: 19339
			Left,
			// Token: 0x04004B8C RID: 19340
			Top,
			// Token: 0x04004B8D RID: 19341
			Right,
			// Token: 0x04004B8E RID: 19342
			Bottom
		}
	}
}
