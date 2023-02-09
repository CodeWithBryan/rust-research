using System;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x020009EB RID: 2539
	[AddComponentMenu("UI/Extensions/Primitives/Diamond Graph")]
	public class DiamondGraph : UIPrimitiveBase
	{
		// Token: 0x170004BB RID: 1211
		// (get) Token: 0x06003C12 RID: 15378 RVA: 0x0015E5A0 File Offset: 0x0015C7A0
		// (set) Token: 0x06003C13 RID: 15379 RVA: 0x0015E5A8 File Offset: 0x0015C7A8
		public float A
		{
			get
			{
				return this.m_a;
			}
			set
			{
				this.m_a = value;
			}
		}

		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x06003C14 RID: 15380 RVA: 0x0015E5B1 File Offset: 0x0015C7B1
		// (set) Token: 0x06003C15 RID: 15381 RVA: 0x0015E5B9 File Offset: 0x0015C7B9
		public float B
		{
			get
			{
				return this.m_b;
			}
			set
			{
				this.m_b = value;
			}
		}

		// Token: 0x170004BD RID: 1213
		// (get) Token: 0x06003C16 RID: 15382 RVA: 0x0015E5C2 File Offset: 0x0015C7C2
		// (set) Token: 0x06003C17 RID: 15383 RVA: 0x0015E5CA File Offset: 0x0015C7CA
		public float C
		{
			get
			{
				return this.m_c;
			}
			set
			{
				this.m_c = value;
			}
		}

		// Token: 0x170004BE RID: 1214
		// (get) Token: 0x06003C18 RID: 15384 RVA: 0x0015E5D3 File Offset: 0x0015C7D3
		// (set) Token: 0x06003C19 RID: 15385 RVA: 0x0015E5DB File Offset: 0x0015C7DB
		public float D
		{
			get
			{
				return this.m_d;
			}
			set
			{
				this.m_d = value;
			}
		}

		// Token: 0x06003C1A RID: 15386 RVA: 0x0015E5E4 File Offset: 0x0015C7E4
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			float num = base.rectTransform.rect.width / 2f;
			this.m_a = Math.Min(1f, Math.Max(0f, this.m_a));
			this.m_b = Math.Min(1f, Math.Max(0f, this.m_b));
			this.m_c = Math.Min(1f, Math.Max(0f, this.m_c));
			this.m_d = Math.Min(1f, Math.Max(0f, this.m_d));
			Color32 color = this.color;
			vh.AddVert(new Vector3(-num * this.m_a, 0f), color, new Vector2(0f, 0f));
			vh.AddVert(new Vector3(0f, num * this.m_b), color, new Vector2(0f, 1f));
			vh.AddVert(new Vector3(num * this.m_c, 0f), color, new Vector2(1f, 1f));
			vh.AddVert(new Vector3(0f, -num * this.m_d), color, new Vector2(1f, 0f));
			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(2, 3, 0);
		}

		// Token: 0x0400357C RID: 13692
		[SerializeField]
		private float m_a = 1f;

		// Token: 0x0400357D RID: 13693
		[SerializeField]
		private float m_b = 1f;

		// Token: 0x0400357E RID: 13694
		[SerializeField]
		private float m_c = 1f;

		// Token: 0x0400357F RID: 13695
		[SerializeField]
		private float m_d = 1f;
	}
}
