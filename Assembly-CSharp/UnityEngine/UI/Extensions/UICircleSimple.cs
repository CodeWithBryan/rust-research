using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x020009ED RID: 2541
	[AddComponentMenu("UI/Extensions/Primitives/UI Circle Simple")]
	public class UICircleSimple : UIPrimitiveBase
	{
		// Token: 0x06003C29 RID: 15401 RVA: 0x0015ED24 File Offset: 0x0015CF24
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			float num = (base.rectTransform.rect.width < base.rectTransform.rect.height) ? base.rectTransform.rect.width : base.rectTransform.rect.height;
			float num2 = this.ThicknessIsOutside ? (-base.rectTransform.pivot.x * num - this.Thickness) : (-base.rectTransform.pivot.x * num);
			float num3 = this.ThicknessIsOutside ? (-base.rectTransform.pivot.x * num) : (-base.rectTransform.pivot.x * num + this.Thickness);
			vh.Clear();
			this.indices.Clear();
			this.vertices.Clear();
			int item = 0;
			int num4 = 1;
			float num5 = 360f / (float)this.ArcSteps;
			float num6 = Mathf.Cos(0f);
			float num7 = Mathf.Sin(0f);
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.color = this.color;
			simpleVert.position = new Vector2(num2 * num6, num2 * num7);
			simpleVert.uv0 = new Vector2(simpleVert.position.x / num + 0.5f, simpleVert.position.y / num + 0.5f);
			this.vertices.Add(simpleVert);
			Vector2 zero = new Vector2(num3 * num6, num3 * num7);
			if (this.Fill)
			{
				zero = Vector2.zero;
			}
			simpleVert.position = zero;
			simpleVert.uv0 = (this.Fill ? this.uvCenter : new Vector2(simpleVert.position.x / num + 0.5f, simpleVert.position.y / num + 0.5f));
			this.vertices.Add(simpleVert);
			for (int i = 1; i <= this.ArcSteps; i++)
			{
				float f = 0.017453292f * ((float)i * num5);
				num6 = Mathf.Cos(f);
				num7 = Mathf.Sin(f);
				simpleVert.color = this.color;
				simpleVert.position = new Vector2(num2 * num6, num2 * num7);
				simpleVert.uv0 = new Vector2(simpleVert.position.x / num + 0.5f, simpleVert.position.y / num + 0.5f);
				this.vertices.Add(simpleVert);
				if (!this.Fill)
				{
					simpleVert.position = new Vector2(num3 * num6, num3 * num7);
					simpleVert.uv0 = new Vector2(simpleVert.position.x / num + 0.5f, simpleVert.position.y / num + 0.5f);
					this.vertices.Add(simpleVert);
					int item2 = num4;
					this.indices.Add(item);
					this.indices.Add(num4 + 1);
					this.indices.Add(num4);
					num4++;
					item = num4;
					num4++;
					this.indices.Add(item);
					this.indices.Add(num4);
					this.indices.Add(item2);
				}
				else
				{
					this.indices.Add(item);
					this.indices.Add(num4 + 1);
					this.indices.Add(1);
					num4++;
					item = num4;
				}
			}
			if (this.Fill)
			{
				simpleVert.position = zero;
				simpleVert.color = this.color;
				simpleVert.uv0 = this.uvCenter;
				this.vertices.Add(simpleVert);
			}
			vh.AddUIVertexStream(this.vertices, this.indices);
		}

		// Token: 0x06003C2A RID: 15402 RVA: 0x0015F11F File Offset: 0x0015D31F
		public void SetArcSteps(int steps)
		{
			this.ArcSteps = steps;
			this.SetVerticesDirty();
		}

		// Token: 0x06003C2B RID: 15403 RVA: 0x0015F12E File Offset: 0x0015D32E
		public void SetFill(bool fill)
		{
			this.Fill = fill;
			this.SetVerticesDirty();
		}

		// Token: 0x06003C2C RID: 15404 RVA: 0x0015EC1D File Offset: 0x0015CE1D
		public void SetBaseColor(Color color)
		{
			this.color = color;
			this.SetVerticesDirty();
		}

		// Token: 0x06003C2D RID: 15405 RVA: 0x0015F140 File Offset: 0x0015D340
		public void UpdateBaseAlpha(float value)
		{
			Color color = this.color;
			color.a = value;
			this.color = color;
			this.SetVerticesDirty();
		}

		// Token: 0x06003C2E RID: 15406 RVA: 0x0015F169 File Offset: 0x0015D369
		public void SetThickness(int thickness)
		{
			this.Thickness = (float)thickness;
			this.SetVerticesDirty();
		}

		// Token: 0x0400358D RID: 13709
		[Tooltip("The Arc Steps property defines the number of segments that the Arc will be divided into.")]
		[Range(0f, 1000f)]
		public int ArcSteps = 100;

		// Token: 0x0400358E RID: 13710
		public bool Fill = true;

		// Token: 0x0400358F RID: 13711
		public float Thickness = 5f;

		// Token: 0x04003590 RID: 13712
		public bool ThicknessIsOutside;

		// Token: 0x04003591 RID: 13713
		private List<int> indices = new List<int>();

		// Token: 0x04003592 RID: 13714
		private List<UIVertex> vertices = new List<UIVertex>();

		// Token: 0x04003593 RID: 13715
		private Vector2 uvCenter = new Vector2(0.5f, 0.5f);
	}
}
