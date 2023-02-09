using System;
using System.Collections.Generic;
using UnityEngine.Sprites;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x020009F2 RID: 2546
	[AddComponentMenu("UI/Extensions/Primitives/UILineRendererList")]
	[RequireComponent(typeof(RectTransform))]
	public class UILineRendererList : UIPrimitiveBase
	{
		// Token: 0x170004D1 RID: 1233
		// (get) Token: 0x06003C6B RID: 15467 RVA: 0x00160E4A File Offset: 0x0015F04A
		// (set) Token: 0x06003C6C RID: 15468 RVA: 0x00160E52 File Offset: 0x0015F052
		public float LineThickness
		{
			get
			{
				return this.lineThickness;
			}
			set
			{
				this.lineThickness = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x170004D2 RID: 1234
		// (get) Token: 0x06003C6D RID: 15469 RVA: 0x00160E61 File Offset: 0x0015F061
		// (set) Token: 0x06003C6E RID: 15470 RVA: 0x00160E69 File Offset: 0x0015F069
		public bool RelativeSize
		{
			get
			{
				return this.relativeSize;
			}
			set
			{
				this.relativeSize = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x170004D3 RID: 1235
		// (get) Token: 0x06003C6F RID: 15471 RVA: 0x00160E78 File Offset: 0x0015F078
		// (set) Token: 0x06003C70 RID: 15472 RVA: 0x00160E80 File Offset: 0x0015F080
		public bool LineList
		{
			get
			{
				return this.lineList;
			}
			set
			{
				this.lineList = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x06003C71 RID: 15473 RVA: 0x00160E8F File Offset: 0x0015F08F
		// (set) Token: 0x06003C72 RID: 15474 RVA: 0x00160E97 File Offset: 0x0015F097
		public bool LineCaps
		{
			get
			{
				return this.lineCaps;
			}
			set
			{
				this.lineCaps = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x06003C73 RID: 15475 RVA: 0x00160EA6 File Offset: 0x0015F0A6
		// (set) Token: 0x06003C74 RID: 15476 RVA: 0x00160EAE File Offset: 0x0015F0AE
		public int BezierSegmentsPerCurve
		{
			get
			{
				return this.bezierSegmentsPerCurve;
			}
			set
			{
				this.bezierSegmentsPerCurve = value;
			}
		}

		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x06003C75 RID: 15477 RVA: 0x00160EB7 File Offset: 0x0015F0B7
		// (set) Token: 0x06003C76 RID: 15478 RVA: 0x00160EBF File Offset: 0x0015F0BF
		public List<Vector2> Points
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

		// Token: 0x06003C77 RID: 15479 RVA: 0x00160ED8 File Offset: 0x0015F0D8
		public void AddPoint(Vector2 pointToAdd)
		{
			this.m_points.Add(pointToAdd);
			this.SetAllDirty();
		}

		// Token: 0x06003C78 RID: 15480 RVA: 0x00160EEC File Offset: 0x0015F0EC
		public void RemovePoint(Vector2 pointToRemove)
		{
			this.m_points.Remove(pointToRemove);
			this.SetAllDirty();
		}

		// Token: 0x06003C79 RID: 15481 RVA: 0x00160F01 File Offset: 0x0015F101
		public void ClearPoints()
		{
			this.m_points.Clear();
			this.SetAllDirty();
		}

		// Token: 0x06003C7A RID: 15482 RVA: 0x00160F14 File Offset: 0x0015F114
		private void PopulateMesh(VertexHelper vh, List<Vector2> pointsToDraw)
		{
			if (this.BezierMode != UILineRendererList.BezierType.None && this.BezierMode != UILineRendererList.BezierType.Catenary && pointsToDraw.Count > 3)
			{
				BezierPath bezierPath = new BezierPath();
				bezierPath.SetControlPoints(pointsToDraw);
				bezierPath.SegmentsPerCurve = this.bezierSegmentsPerCurve;
				UILineRendererList.BezierType bezierMode = this.BezierMode;
				List<Vector2> list;
				if (bezierMode != UILineRendererList.BezierType.Basic)
				{
					if (bezierMode != UILineRendererList.BezierType.Improved)
					{
						list = bezierPath.GetDrawingPoints2();
					}
					else
					{
						list = bezierPath.GetDrawingPoints1();
					}
				}
				else
				{
					list = bezierPath.GetDrawingPoints0();
				}
				pointsToDraw = list;
			}
			if (this.BezierMode == UILineRendererList.BezierType.Catenary && pointsToDraw.Count == 2)
			{
				CableCurve cableCurve = new CableCurve(pointsToDraw);
				cableCurve.slack = base.Resoloution;
				cableCurve.steps = this.BezierSegmentsPerCurve;
				pointsToDraw.Clear();
				pointsToDraw.AddRange(cableCurve.Points());
			}
			if (base.ImproveResolution != ResolutionMode.None)
			{
				pointsToDraw = base.IncreaseResolution(pointsToDraw);
			}
			float num = (!this.relativeSize) ? 1f : base.rectTransform.rect.width;
			float num2 = (!this.relativeSize) ? 1f : base.rectTransform.rect.height;
			float num3 = -base.rectTransform.pivot.x * num;
			float num4 = -base.rectTransform.pivot.y * num2;
			List<UIVertex[]> list2 = new List<UIVertex[]>();
			if (this.lineList)
			{
				for (int i = 1; i < pointsToDraw.Count; i += 2)
				{
					Vector2 vector = pointsToDraw[i - 1];
					Vector2 vector2 = pointsToDraw[i];
					vector = new Vector2(vector.x * num + num3, vector.y * num2 + num4);
					vector2 = new Vector2(vector2.x * num + num3, vector2.y * num2 + num4);
					if (this.lineCaps)
					{
						list2.Add(this.CreateLineCap(vector, vector2, UILineRendererList.SegmentType.Start));
					}
					list2.Add(this.CreateLineSegment(vector, vector2, UILineRendererList.SegmentType.Middle));
					if (this.lineCaps)
					{
						list2.Add(this.CreateLineCap(vector, vector2, UILineRendererList.SegmentType.End));
					}
				}
			}
			else
			{
				for (int j = 1; j < pointsToDraw.Count; j++)
				{
					Vector2 vector3 = pointsToDraw[j - 1];
					Vector2 vector4 = pointsToDraw[j];
					vector3 = new Vector2(vector3.x * num + num3, vector3.y * num2 + num4);
					vector4 = new Vector2(vector4.x * num + num3, vector4.y * num2 + num4);
					if (this.lineCaps && j == 1)
					{
						list2.Add(this.CreateLineCap(vector3, vector4, UILineRendererList.SegmentType.Start));
					}
					list2.Add(this.CreateLineSegment(vector3, vector4, UILineRendererList.SegmentType.Middle));
					if (this.lineCaps && j == pointsToDraw.Count - 1)
					{
						list2.Add(this.CreateLineCap(vector3, vector4, UILineRendererList.SegmentType.End));
					}
				}
			}
			for (int k = 0; k < list2.Count; k++)
			{
				if (!this.lineList && k < list2.Count - 1)
				{
					Vector3 v = list2[k][1].position - list2[k][2].position;
					Vector3 v2 = list2[k + 1][2].position - list2[k + 1][1].position;
					float num5 = Vector2.Angle(v, v2) * 0.017453292f;
					float num6 = Mathf.Sign(Vector3.Cross(v.normalized, v2.normalized).z);
					float num7 = this.lineThickness / (2f * Mathf.Tan(num5 / 2f));
					Vector3 position = list2[k][2].position - v.normalized * num7 * num6;
					Vector3 position2 = list2[k][3].position + v.normalized * num7 * num6;
					UILineRendererList.JoinType joinType = this.LineJoins;
					if (joinType == UILineRendererList.JoinType.Miter)
					{
						if (num7 < v.magnitude / 2f && num7 < v2.magnitude / 2f && num5 > 0.2617994f)
						{
							list2[k][2].position = position;
							list2[k][3].position = position2;
							list2[k + 1][0].position = position2;
							list2[k + 1][1].position = position;
						}
						else
						{
							joinType = UILineRendererList.JoinType.Bevel;
						}
					}
					if (joinType == UILineRendererList.JoinType.Bevel)
					{
						if (num7 < v.magnitude / 2f && num7 < v2.magnitude / 2f && num5 > 0.5235988f)
						{
							if (num6 < 0f)
							{
								list2[k][2].position = position;
								list2[k + 1][1].position = position;
							}
							else
							{
								list2[k][3].position = position2;
								list2[k + 1][0].position = position2;
							}
						}
						UIVertex[] verts = new UIVertex[]
						{
							list2[k][2],
							list2[k][3],
							list2[k + 1][0],
							list2[k + 1][1]
						};
						vh.AddUIVertexQuad(verts);
					}
				}
				vh.AddUIVertexQuad(list2[k]);
			}
			if (vh.currentVertCount > 64000)
			{
				Debug.LogError("Max Verticies size is 64000, current mesh vertcies count is [" + vh.currentVertCount + "] - Cannot Draw");
				vh.Clear();
				return;
			}
		}

		// Token: 0x06003C7B RID: 15483 RVA: 0x00161515 File Offset: 0x0015F715
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (this.m_points != null && this.m_points.Count > 0)
			{
				this.GeneratedUVs();
				vh.Clear();
				this.PopulateMesh(vh, this.m_points);
			}
		}

		// Token: 0x06003C7C RID: 15484 RVA: 0x00161548 File Offset: 0x0015F748
		private UIVertex[] CreateLineCap(Vector2 start, Vector2 end, UILineRendererList.SegmentType type)
		{
			if (type == UILineRendererList.SegmentType.Start)
			{
				Vector2 start2 = start - (end - start).normalized * this.lineThickness / 2f;
				return this.CreateLineSegment(start2, start, UILineRendererList.SegmentType.Start);
			}
			if (type == UILineRendererList.SegmentType.End)
			{
				Vector2 end2 = end + (end - start).normalized * this.lineThickness / 2f;
				return this.CreateLineSegment(end, end2, UILineRendererList.SegmentType.End);
			}
			Debug.LogError("Bad SegmentType passed in to CreateLineCap. Must be SegmentType.Start or SegmentType.End");
			return null;
		}

		// Token: 0x06003C7D RID: 15485 RVA: 0x001615D4 File Offset: 0x0015F7D4
		private UIVertex[] CreateLineSegment(Vector2 start, Vector2 end, UILineRendererList.SegmentType type)
		{
			Vector2 b = new Vector2(start.y - end.y, end.x - start.x).normalized * this.lineThickness / 2f;
			Vector2 vector = start - b;
			Vector2 vector2 = start + b;
			Vector2 vector3 = end + b;
			Vector2 vector4 = end - b;
			switch (type)
			{
			case UILineRendererList.SegmentType.Start:
				return base.SetVbo(new Vector2[]
				{
					vector,
					vector2,
					vector3,
					vector4
				}, UILineRendererList.startUvs);
			case UILineRendererList.SegmentType.End:
				return base.SetVbo(new Vector2[]
				{
					vector,
					vector2,
					vector3,
					vector4
				}, UILineRendererList.endUvs);
			case UILineRendererList.SegmentType.Full:
				return base.SetVbo(new Vector2[]
				{
					vector,
					vector2,
					vector3,
					vector4
				}, UILineRendererList.fullUvs);
			}
			return base.SetVbo(new Vector2[]
			{
				vector,
				vector2,
				vector3,
				vector4
			}, UILineRendererList.middleUvs);
		}

		// Token: 0x06003C7E RID: 15486 RVA: 0x00161728 File Offset: 0x0015F928
		protected override void GeneratedUVs()
		{
			if (base.activeSprite != null)
			{
				Vector4 outerUV = DataUtility.GetOuterUV(base.activeSprite);
				Vector4 innerUV = DataUtility.GetInnerUV(base.activeSprite);
				UILineRendererList.UV_TOP_LEFT = new Vector2(outerUV.x, outerUV.y);
				UILineRendererList.UV_BOTTOM_LEFT = new Vector2(outerUV.x, outerUV.w);
				UILineRendererList.UV_TOP_CENTER_LEFT = new Vector2(innerUV.x, innerUV.y);
				UILineRendererList.UV_TOP_CENTER_RIGHT = new Vector2(innerUV.z, innerUV.y);
				UILineRendererList.UV_BOTTOM_CENTER_LEFT = new Vector2(innerUV.x, innerUV.w);
				UILineRendererList.UV_BOTTOM_CENTER_RIGHT = new Vector2(innerUV.z, innerUV.w);
				UILineRendererList.UV_TOP_RIGHT = new Vector2(outerUV.z, outerUV.y);
				UILineRendererList.UV_BOTTOM_RIGHT = new Vector2(outerUV.z, outerUV.w);
			}
			else
			{
				UILineRendererList.UV_TOP_LEFT = Vector2.zero;
				UILineRendererList.UV_BOTTOM_LEFT = new Vector2(0f, 1f);
				UILineRendererList.UV_TOP_CENTER_LEFT = new Vector2(0.5f, 0f);
				UILineRendererList.UV_TOP_CENTER_RIGHT = new Vector2(0.5f, 0f);
				UILineRendererList.UV_BOTTOM_CENTER_LEFT = new Vector2(0.5f, 1f);
				UILineRendererList.UV_BOTTOM_CENTER_RIGHT = new Vector2(0.5f, 1f);
				UILineRendererList.UV_TOP_RIGHT = new Vector2(1f, 0f);
				UILineRendererList.UV_BOTTOM_RIGHT = Vector2.one;
			}
			UILineRendererList.startUvs = new Vector2[]
			{
				UILineRendererList.UV_TOP_LEFT,
				UILineRendererList.UV_BOTTOM_LEFT,
				UILineRendererList.UV_BOTTOM_CENTER_LEFT,
				UILineRendererList.UV_TOP_CENTER_LEFT
			};
			UILineRendererList.middleUvs = new Vector2[]
			{
				UILineRendererList.UV_TOP_CENTER_LEFT,
				UILineRendererList.UV_BOTTOM_CENTER_LEFT,
				UILineRendererList.UV_BOTTOM_CENTER_RIGHT,
				UILineRendererList.UV_TOP_CENTER_RIGHT
			};
			UILineRendererList.endUvs = new Vector2[]
			{
				UILineRendererList.UV_TOP_CENTER_RIGHT,
				UILineRendererList.UV_BOTTOM_CENTER_RIGHT,
				UILineRendererList.UV_BOTTOM_RIGHT,
				UILineRendererList.UV_TOP_RIGHT
			};
			UILineRendererList.fullUvs = new Vector2[]
			{
				UILineRendererList.UV_TOP_LEFT,
				UILineRendererList.UV_BOTTOM_LEFT,
				UILineRendererList.UV_BOTTOM_RIGHT,
				UILineRendererList.UV_TOP_RIGHT
			};
		}

		// Token: 0x06003C7F RID: 15487 RVA: 0x0016198C File Offset: 0x0015FB8C
		protected override void ResolutionToNativeSize(float distance)
		{
			if (base.UseNativeSize)
			{
				this.m_Resolution = distance / (base.activeSprite.rect.width / base.pixelsPerUnit);
				this.lineThickness = base.activeSprite.rect.height / base.pixelsPerUnit;
			}
		}

		// Token: 0x040035BD RID: 13757
		private const float MIN_MITER_JOIN = 0.2617994f;

		// Token: 0x040035BE RID: 13758
		private const float MIN_BEVEL_NICE_JOIN = 0.5235988f;

		// Token: 0x040035BF RID: 13759
		private static Vector2 UV_TOP_LEFT;

		// Token: 0x040035C0 RID: 13760
		private static Vector2 UV_BOTTOM_LEFT;

		// Token: 0x040035C1 RID: 13761
		private static Vector2 UV_TOP_CENTER_LEFT;

		// Token: 0x040035C2 RID: 13762
		private static Vector2 UV_TOP_CENTER_RIGHT;

		// Token: 0x040035C3 RID: 13763
		private static Vector2 UV_BOTTOM_CENTER_LEFT;

		// Token: 0x040035C4 RID: 13764
		private static Vector2 UV_BOTTOM_CENTER_RIGHT;

		// Token: 0x040035C5 RID: 13765
		private static Vector2 UV_TOP_RIGHT;

		// Token: 0x040035C6 RID: 13766
		private static Vector2 UV_BOTTOM_RIGHT;

		// Token: 0x040035C7 RID: 13767
		private static Vector2[] startUvs;

		// Token: 0x040035C8 RID: 13768
		private static Vector2[] middleUvs;

		// Token: 0x040035C9 RID: 13769
		private static Vector2[] endUvs;

		// Token: 0x040035CA RID: 13770
		private static Vector2[] fullUvs;

		// Token: 0x040035CB RID: 13771
		[SerializeField]
		[Tooltip("Points to draw lines between\n Can be improved using the Resolution Option")]
		internal List<Vector2> m_points;

		// Token: 0x040035CC RID: 13772
		[SerializeField]
		[Tooltip("Thickness of the line")]
		internal float lineThickness = 2f;

		// Token: 0x040035CD RID: 13773
		[SerializeField]
		[Tooltip("Use the relative bounds of the Rect Transform (0,0 -> 0,1) or screen space coordinates")]
		internal bool relativeSize;

		// Token: 0x040035CE RID: 13774
		[SerializeField]
		[Tooltip("Do the points identify a single line or split pairs of lines")]
		internal bool lineList;

		// Token: 0x040035CF RID: 13775
		[SerializeField]
		[Tooltip("Add end caps to each line\nMultiple caps when used with Line List")]
		internal bool lineCaps;

		// Token: 0x040035D0 RID: 13776
		[SerializeField]
		[Tooltip("Resolution of the Bezier curve, different to line Resolution")]
		internal int bezierSegmentsPerCurve = 10;

		// Token: 0x040035D1 RID: 13777
		[Tooltip("The type of Join used between lines, Square/Mitre or Curved/Bevel")]
		public UILineRendererList.JoinType LineJoins;

		// Token: 0x040035D2 RID: 13778
		[Tooltip("Bezier method to apply to line, see docs for options\nCan't be used in conjunction with Resolution as Bezier already changes the resolution")]
		public UILineRendererList.BezierType BezierMode;

		// Token: 0x040035D3 RID: 13779
		[HideInInspector]
		public bool drivenExternally;

		// Token: 0x02000EB1 RID: 3761
		private enum SegmentType
		{
			// Token: 0x04004B9E RID: 19358
			Start,
			// Token: 0x04004B9F RID: 19359
			Middle,
			// Token: 0x04004BA0 RID: 19360
			End,
			// Token: 0x04004BA1 RID: 19361
			Full
		}

		// Token: 0x02000EB2 RID: 3762
		public enum JoinType
		{
			// Token: 0x04004BA3 RID: 19363
			Bevel,
			// Token: 0x04004BA4 RID: 19364
			Miter
		}

		// Token: 0x02000EB3 RID: 3763
		public enum BezierType
		{
			// Token: 0x04004BA6 RID: 19366
			None,
			// Token: 0x04004BA7 RID: 19367
			Quick,
			// Token: 0x04004BA8 RID: 19368
			Basic,
			// Token: 0x04004BA9 RID: 19369
			Improved,
			// Token: 0x04004BAA RID: 19370
			Catenary
		}
	}
}
