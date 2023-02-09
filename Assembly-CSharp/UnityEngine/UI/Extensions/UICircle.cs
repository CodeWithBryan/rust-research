using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x020009EC RID: 2540
	[AddComponentMenu("UI/Extensions/Primitives/UI Circle")]
	public class UICircle : UIPrimitiveBase
	{
		// Token: 0x06003C1C RID: 15388 RVA: 0x0015E788 File Offset: 0x0015C988
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			int num = this.ArcInvert ? -1 : 1;
			float num2 = ((base.rectTransform.rect.width < base.rectTransform.rect.height) ? base.rectTransform.rect.width : base.rectTransform.rect.height) - (float)this.Padding;
			float num3 = -base.rectTransform.pivot.x * num2;
			float num4 = -base.rectTransform.pivot.x * num2 + this.Thickness;
			vh.Clear();
			this.indices.Clear();
			this.vertices.Clear();
			int item = 0;
			int num5 = 1;
			float num6 = this.Arc * 360f / (float)this.ArcSteps;
			this._progress = (float)this.ArcSteps * this.Progress;
			float f = (float)num * 0.017453292f * (float)this.ArcRotation;
			float num7 = Mathf.Cos(f);
			float num8 = Mathf.Sin(f);
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.color = ((this._progress > 0f) ? this.ProgressColor : this.color);
			simpleVert.position = new Vector2(num3 * num7, num3 * num8);
			simpleVert.uv0 = new Vector2(simpleVert.position.x / num2 + 0.5f, simpleVert.position.y / num2 + 0.5f);
			this.vertices.Add(simpleVert);
			Vector2 zero = new Vector2(num4 * num7, num4 * num8);
			if (this.Fill)
			{
				zero = Vector2.zero;
			}
			simpleVert.position = zero;
			simpleVert.uv0 = (this.Fill ? this.uvCenter : new Vector2(simpleVert.position.x / num2 + 0.5f, simpleVert.position.y / num2 + 0.5f));
			this.vertices.Add(simpleVert);
			for (int i = 1; i <= this.ArcSteps; i++)
			{
				float f2 = (float)num * 0.017453292f * ((float)i * num6 + (float)this.ArcRotation);
				num7 = Mathf.Cos(f2);
				num8 = Mathf.Sin(f2);
				simpleVert.color = (((float)i > this._progress) ? this.color : this.ProgressColor);
				simpleVert.position = new Vector2(num3 * num7, num3 * num8);
				simpleVert.uv0 = new Vector2(simpleVert.position.x / num2 + 0.5f, simpleVert.position.y / num2 + 0.5f);
				this.vertices.Add(simpleVert);
				if (!this.Fill)
				{
					simpleVert.position = new Vector2(num4 * num7, num4 * num8);
					simpleVert.uv0 = new Vector2(simpleVert.position.x / num2 + 0.5f, simpleVert.position.y / num2 + 0.5f);
					this.vertices.Add(simpleVert);
					int item2 = num5;
					this.indices.Add(item);
					this.indices.Add(num5 + 1);
					this.indices.Add(num5);
					num5++;
					item = num5;
					num5++;
					this.indices.Add(item);
					this.indices.Add(num5);
					this.indices.Add(item2);
				}
				else
				{
					this.indices.Add(item);
					this.indices.Add(num5 + 1);
					if ((float)i > this._progress)
					{
						this.indices.Add(this.ArcSteps + 2);
					}
					else
					{
						this.indices.Add(1);
					}
					num5++;
					item = num5;
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

		// Token: 0x06003C1D RID: 15389 RVA: 0x0015EBD2 File Offset: 0x0015CDD2
		public void SetProgress(float progress)
		{
			this.Progress = progress;
			this.SetVerticesDirty();
		}

		// Token: 0x06003C1E RID: 15390 RVA: 0x0015EBE1 File Offset: 0x0015CDE1
		public void SetArcSteps(int steps)
		{
			this.ArcSteps = steps;
			this.SetVerticesDirty();
		}

		// Token: 0x06003C1F RID: 15391 RVA: 0x0015EBF0 File Offset: 0x0015CDF0
		public void SetInvertArc(bool invert)
		{
			this.ArcInvert = invert;
			this.SetVerticesDirty();
		}

		// Token: 0x06003C20 RID: 15392 RVA: 0x0015EBFF File Offset: 0x0015CDFF
		public void SetArcRotation(int rotation)
		{
			this.ArcRotation = rotation;
			this.SetVerticesDirty();
		}

		// Token: 0x06003C21 RID: 15393 RVA: 0x0015EC0E File Offset: 0x0015CE0E
		public void SetFill(bool fill)
		{
			this.Fill = fill;
			this.SetVerticesDirty();
		}

		// Token: 0x06003C22 RID: 15394 RVA: 0x0015EC1D File Offset: 0x0015CE1D
		public void SetBaseColor(Color color)
		{
			this.color = color;
			this.SetVerticesDirty();
		}

		// Token: 0x06003C23 RID: 15395 RVA: 0x0015EC2C File Offset: 0x0015CE2C
		public void UpdateBaseAlpha(float value)
		{
			Color color = this.color;
			color.a = value;
			this.color = color;
			this.SetVerticesDirty();
		}

		// Token: 0x06003C24 RID: 15396 RVA: 0x0015EC55 File Offset: 0x0015CE55
		public void SetProgressColor(Color color)
		{
			this.ProgressColor = color;
			this.SetVerticesDirty();
		}

		// Token: 0x06003C25 RID: 15397 RVA: 0x0015EC64 File Offset: 0x0015CE64
		public void UpdateProgressAlpha(float value)
		{
			this.ProgressColor.a = value;
			this.SetVerticesDirty();
		}

		// Token: 0x06003C26 RID: 15398 RVA: 0x0015EC78 File Offset: 0x0015CE78
		public void SetPadding(int padding)
		{
			this.Padding = padding;
			this.SetVerticesDirty();
		}

		// Token: 0x06003C27 RID: 15399 RVA: 0x0015EC87 File Offset: 0x0015CE87
		public void SetThickness(int thickness)
		{
			this.Thickness = (float)thickness;
			this.SetVerticesDirty();
		}

		// Token: 0x04003580 RID: 13696
		[Tooltip("The Arc Invert property will invert the construction of the Arc.")]
		public bool ArcInvert = true;

		// Token: 0x04003581 RID: 13697
		[Tooltip("The Arc property is a percentage of the entire circumference of the circle.")]
		[Range(0f, 1f)]
		public float Arc = 1f;

		// Token: 0x04003582 RID: 13698
		[Tooltip("The Arc Steps property defines the number of segments that the Arc will be divided into.")]
		[Range(0f, 1000f)]
		public int ArcSteps = 100;

		// Token: 0x04003583 RID: 13699
		[Tooltip("The Arc Rotation property permits adjusting the geometry orientation around the Z axis.")]
		[Range(0f, 360f)]
		public int ArcRotation;

		// Token: 0x04003584 RID: 13700
		[Tooltip("The Progress property allows the primitive to be used as a progression indicator.")]
		[Range(0f, 1f)]
		public float Progress;

		// Token: 0x04003585 RID: 13701
		private float _progress;

		// Token: 0x04003586 RID: 13702
		public Color ProgressColor = new Color(255f, 255f, 255f, 255f);

		// Token: 0x04003587 RID: 13703
		public bool Fill = true;

		// Token: 0x04003588 RID: 13704
		public float Thickness = 5f;

		// Token: 0x04003589 RID: 13705
		public int Padding;

		// Token: 0x0400358A RID: 13706
		private List<int> indices = new List<int>();

		// Token: 0x0400358B RID: 13707
		private List<UIVertex> vertices = new List<UIVertex>();

		// Token: 0x0400358C RID: 13708
		private Vector2 uvCenter = new Vector2(0.5f, 0.5f);
	}
}
