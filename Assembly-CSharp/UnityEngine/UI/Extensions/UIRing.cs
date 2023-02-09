using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x020009F8 RID: 2552
	[AddComponentMenu("UI/Extensions/Primitives/UI Ring")]
	public class UIRing : UIPrimitiveBase
	{
		// Token: 0x06003CB9 RID: 15545 RVA: 0x00162BE8 File Offset: 0x00160DE8
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			float num = this.innerRadius * 2f;
			float num2 = this.outerRadius * 2f;
			vh.Clear();
			this.indices.Clear();
			this.vertices.Clear();
			int item = 0;
			int num3 = 1;
			float num4 = 360f / (float)this.ArcSteps;
			float num5 = Mathf.Cos(0f);
			float num6 = Mathf.Sin(0f);
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.color = this.color;
			simpleVert.position = new Vector2(num2 * num5, num2 * num6);
			this.vertices.Add(simpleVert);
			Vector2 v = new Vector2(num * num5, num * num6);
			simpleVert.position = v;
			this.vertices.Add(simpleVert);
			for (int i = 1; i <= this.ArcSteps; i++)
			{
				float f = 0.017453292f * ((float)i * num4);
				num5 = Mathf.Cos(f);
				num6 = Mathf.Sin(f);
				simpleVert.color = this.color;
				simpleVert.position = new Vector2(num2 * num5, num2 * num6);
				this.vertices.Add(simpleVert);
				simpleVert.position = new Vector2(num * num5, num * num6);
				this.vertices.Add(simpleVert);
				int item2 = num3;
				this.indices.Add(item);
				this.indices.Add(num3 + 1);
				this.indices.Add(num3);
				num3++;
				item = num3;
				num3++;
				this.indices.Add(item);
				this.indices.Add(num3);
				this.indices.Add(item2);
			}
			vh.AddUIVertexStream(this.vertices, this.indices);
		}

		// Token: 0x06003CBA RID: 15546 RVA: 0x00162DBD File Offset: 0x00160FBD
		public void SetArcSteps(int steps)
		{
			this.ArcSteps = steps;
			this.SetVerticesDirty();
		}

		// Token: 0x06003CBB RID: 15547 RVA: 0x0015EC1D File Offset: 0x0015CE1D
		public void SetBaseColor(Color color)
		{
			this.color = color;
			this.SetVerticesDirty();
		}

		// Token: 0x06003CBC RID: 15548 RVA: 0x00162DCC File Offset: 0x00160FCC
		public void UpdateBaseAlpha(float value)
		{
			Color color = this.color;
			color.a = value;
			this.color = color;
			this.SetVerticesDirty();
		}

		// Token: 0x040035EC RID: 13804
		public float innerRadius = 16f;

		// Token: 0x040035ED RID: 13805
		public float outerRadius = 32f;

		// Token: 0x040035EE RID: 13806
		[Tooltip("The Arc Steps property defines the number of segments that the Arc will be divided into.")]
		[Range(0f, 1000f)]
		public int ArcSteps = 100;

		// Token: 0x040035EF RID: 13807
		private List<int> indices = new List<int>();

		// Token: 0x040035F0 RID: 13808
		private List<UIVertex> vertices = new List<UIVertex>();
	}
}
