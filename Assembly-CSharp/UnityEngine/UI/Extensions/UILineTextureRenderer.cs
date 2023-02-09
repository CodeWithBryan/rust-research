using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x020009F3 RID: 2547
	[AddComponentMenu("UI/Extensions/Primitives/UILineTextureRenderer")]
	public class UILineTextureRenderer : UIPrimitiveBase
	{
		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x06003C81 RID: 15489 RVA: 0x001619FE File Offset: 0x0015FBFE
		// (set) Token: 0x06003C82 RID: 15490 RVA: 0x00161A06 File Offset: 0x0015FC06
		public Rect uvRect
		{
			get
			{
				return this.m_UVRect;
			}
			set
			{
				if (this.m_UVRect == value)
				{
					return;
				}
				this.m_UVRect = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170004D8 RID: 1240
		// (get) Token: 0x06003C83 RID: 15491 RVA: 0x00161A24 File Offset: 0x0015FC24
		// (set) Token: 0x06003C84 RID: 15492 RVA: 0x00161A2C File Offset: 0x0015FC2C
		public Vector2[] Points
		{
			get
			{
				return this.m_points;
			}
			set
			{
				if (this.m_points == value)
				{
					return;
				}
				this.m_points = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x06003C85 RID: 15493 RVA: 0x00161A48 File Offset: 0x0015FC48
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (this.m_points == null || this.m_points.Length < 2)
			{
				this.m_points = new Vector2[]
				{
					new Vector2(0f, 0f),
					new Vector2(1f, 1f)
				};
			}
			int num = 24;
			float num2 = base.rectTransform.rect.width;
			float num3 = base.rectTransform.rect.height;
			float num4 = -base.rectTransform.pivot.x * base.rectTransform.rect.width;
			float num5 = -base.rectTransform.pivot.y * base.rectTransform.rect.height;
			if (!this.relativeSize)
			{
				num2 = 1f;
				num3 = 1f;
			}
			List<Vector2> list = new List<Vector2>();
			list.Add(this.m_points[0]);
			Vector2 item = this.m_points[0] + (this.m_points[1] - this.m_points[0]).normalized * (float)num;
			list.Add(item);
			for (int i = 1; i < this.m_points.Length - 1; i++)
			{
				list.Add(this.m_points[i]);
			}
			item = this.m_points[this.m_points.Length - 1] - (this.m_points[this.m_points.Length - 1] - this.m_points[this.m_points.Length - 2]).normalized * (float)num;
			list.Add(item);
			list.Add(this.m_points[this.m_points.Length - 1]);
			Vector2[] array = list.ToArray();
			if (this.UseMargins)
			{
				num2 -= this.Margin.x;
				num3 -= this.Margin.y;
				num4 += this.Margin.x / 2f;
				num5 += this.Margin.y / 2f;
			}
			vh.Clear();
			Vector2 vector = Vector2.zero;
			Vector2 vector2 = Vector2.zero;
			for (int j = 1; j < array.Length; j++)
			{
				Vector2 vector3 = array[j - 1];
				Vector2 vector4 = array[j];
				vector3 = new Vector2(vector3.x * num2 + num4, vector3.y * num3 + num5);
				vector4 = new Vector2(vector4.x * num2 + num4, vector4.y * num3 + num5);
				float z = Mathf.Atan2(vector4.y - vector3.y, vector4.x - vector3.x) * 180f / 3.1415927f;
				Vector2 vector5 = vector3 + new Vector2(0f, -this.LineThickness / 2f);
				Vector2 vector6 = vector3 + new Vector2(0f, this.LineThickness / 2f);
				Vector2 vector7 = vector4 + new Vector2(0f, this.LineThickness / 2f);
				Vector2 vector8 = vector4 + new Vector2(0f, -this.LineThickness / 2f);
				vector5 = this.RotatePointAroundPivot(vector5, vector3, new Vector3(0f, 0f, z));
				vector6 = this.RotatePointAroundPivot(vector6, vector3, new Vector3(0f, 0f, z));
				vector7 = this.RotatePointAroundPivot(vector7, vector4, new Vector3(0f, 0f, z));
				vector8 = this.RotatePointAroundPivot(vector8, vector4, new Vector3(0f, 0f, z));
				Vector2 zero = Vector2.zero;
				Vector2 vector9 = new Vector2(0f, 1f);
				Vector2 vector10 = new Vector2(0.5f, 0f);
				Vector2 vector11 = new Vector2(0.5f, 1f);
				Vector2 vector12 = new Vector2(1f, 0f);
				Vector2 vector13 = new Vector2(1f, 1f);
				Vector2[] uvs = new Vector2[]
				{
					vector10,
					vector11,
					vector11,
					vector10
				};
				if (j > 1)
				{
					vh.AddUIVertexQuad(base.SetVbo(new Vector2[]
					{
						vector,
						vector2,
						vector5,
						vector6
					}, uvs));
				}
				if (j == 1)
				{
					uvs = new Vector2[]
					{
						zero,
						vector9,
						vector11,
						vector10
					};
				}
				else if (j == array.Length - 1)
				{
					uvs = new Vector2[]
					{
						vector10,
						vector11,
						vector13,
						vector12
					};
				}
				vh.AddUIVertexQuad(base.SetVbo(new Vector2[]
				{
					vector5,
					vector6,
					vector7,
					vector8
				}, uvs));
				vector = vector7;
				vector2 = vector8;
			}
		}

		// Token: 0x06003C86 RID: 15494 RVA: 0x00161FE8 File Offset: 0x001601E8
		public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
		{
			Vector3 vector = point - pivot;
			vector = Quaternion.Euler(angles) * vector;
			point = vector + pivot;
			return point;
		}

		// Token: 0x040035D4 RID: 13780
		[SerializeField]
		private Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

		// Token: 0x040035D5 RID: 13781
		[SerializeField]
		private Vector2[] m_points;

		// Token: 0x040035D6 RID: 13782
		public float LineThickness = 2f;

		// Token: 0x040035D7 RID: 13783
		public bool UseMargins;

		// Token: 0x040035D8 RID: 13784
		public Vector2 Margin;

		// Token: 0x040035D9 RID: 13785
		public bool relativeSize;
	}
}
