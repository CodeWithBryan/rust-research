using System;
using System.Collections.Generic;
using UnityEngine.Sprites;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x020009F1 RID: 2545
	[AddComponentMenu("UI/Extensions/Primitives/UILineRenderer")]
	[RequireComponent(typeof(RectTransform))]
	public class UILineRenderer : UIPrimitiveBase
	{
		// Token: 0x170004CA RID: 1226
		// (get) Token: 0x06003C52 RID: 15442 RVA: 0x00160071 File Offset: 0x0015E271
		// (set) Token: 0x06003C53 RID: 15443 RVA: 0x00160079 File Offset: 0x0015E279
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

		// Token: 0x170004CB RID: 1227
		// (get) Token: 0x06003C54 RID: 15444 RVA: 0x00160088 File Offset: 0x0015E288
		// (set) Token: 0x06003C55 RID: 15445 RVA: 0x00160090 File Offset: 0x0015E290
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

		// Token: 0x170004CC RID: 1228
		// (get) Token: 0x06003C56 RID: 15446 RVA: 0x0016009F File Offset: 0x0015E29F
		// (set) Token: 0x06003C57 RID: 15447 RVA: 0x001600A7 File Offset: 0x0015E2A7
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

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x06003C58 RID: 15448 RVA: 0x001600B6 File Offset: 0x0015E2B6
		// (set) Token: 0x06003C59 RID: 15449 RVA: 0x001600BE File Offset: 0x0015E2BE
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

		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x06003C5A RID: 15450 RVA: 0x001600CD File Offset: 0x0015E2CD
		// (set) Token: 0x06003C5B RID: 15451 RVA: 0x001600D5 File Offset: 0x0015E2D5
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

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x06003C5C RID: 15452 RVA: 0x001600DE File Offset: 0x0015E2DE
		// (set) Token: 0x06003C5D RID: 15453 RVA: 0x001600E6 File Offset: 0x0015E2E6
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

		// Token: 0x170004D0 RID: 1232
		// (get) Token: 0x06003C5E RID: 15454 RVA: 0x001600FF File Offset: 0x0015E2FF
		// (set) Token: 0x06003C5F RID: 15455 RVA: 0x00160107 File Offset: 0x0015E307
		public List<Vector2[]> Segments
		{
			get
			{
				return this.m_segments;
			}
			set
			{
				this.m_segments = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x06003C60 RID: 15456 RVA: 0x00160118 File Offset: 0x0015E318
		private void PopulateMesh(VertexHelper vh, Vector2[] pointsToDraw)
		{
			if (this.BezierMode != UILineRenderer.BezierType.None && this.BezierMode != UILineRenderer.BezierType.Catenary && pointsToDraw.Length > 3)
			{
				BezierPath bezierPath = new BezierPath();
				bezierPath.SetControlPoints(pointsToDraw);
				bezierPath.SegmentsPerCurve = this.bezierSegmentsPerCurve;
				UILineRenderer.BezierType bezierMode = this.BezierMode;
				List<Vector2> list;
				if (bezierMode != UILineRenderer.BezierType.Basic)
				{
					if (bezierMode != UILineRenderer.BezierType.Improved)
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
				pointsToDraw = list.ToArray();
			}
			if (this.BezierMode == UILineRenderer.BezierType.Catenary && pointsToDraw.Length == 2)
			{
				pointsToDraw = new CableCurve(pointsToDraw)
				{
					slack = base.Resoloution,
					steps = this.BezierSegmentsPerCurve
				}.Points();
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
				for (int i = 1; i < pointsToDraw.Length; i += 2)
				{
					Vector2 vector = pointsToDraw[i - 1];
					Vector2 vector2 = pointsToDraw[i];
					vector = new Vector2(vector.x * num + num3, vector.y * num2 + num4);
					vector2 = new Vector2(vector2.x * num + num3, vector2.y * num2 + num4);
					if (this.lineCaps)
					{
						list2.Add(this.CreateLineCap(vector, vector2, UILineRenderer.SegmentType.Start));
					}
					list2.Add(this.CreateLineSegment(vector, vector2, UILineRenderer.SegmentType.Middle, (list2.Count > 1) ? list2[list2.Count - 2] : null));
					if (this.lineCaps)
					{
						list2.Add(this.CreateLineCap(vector, vector2, UILineRenderer.SegmentType.End));
					}
				}
			}
			else
			{
				for (int j = 1; j < pointsToDraw.Length; j++)
				{
					Vector2 vector3 = pointsToDraw[j - 1];
					Vector2 vector4 = pointsToDraw[j];
					vector3 = new Vector2(vector3.x * num + num3, vector3.y * num2 + num4);
					vector4 = new Vector2(vector4.x * num + num3, vector4.y * num2 + num4);
					if (this.lineCaps && j == 1)
					{
						list2.Add(this.CreateLineCap(vector3, vector4, UILineRenderer.SegmentType.Start));
					}
					list2.Add(this.CreateLineSegment(vector3, vector4, UILineRenderer.SegmentType.Middle, null));
					if (this.lineCaps && j == pointsToDraw.Length - 1)
					{
						list2.Add(this.CreateLineCap(vector3, vector4, UILineRenderer.SegmentType.End));
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
					UILineRenderer.JoinType joinType = this.LineJoins;
					if (joinType == UILineRenderer.JoinType.Miter)
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
							joinType = UILineRenderer.JoinType.Bevel;
						}
					}
					if (joinType == UILineRenderer.JoinType.Bevel)
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

		// Token: 0x06003C61 RID: 15457 RVA: 0x00160720 File Offset: 0x0015E920
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (this.m_points != null && this.m_points.Length != 0)
			{
				this.GeneratedUVs();
				vh.Clear();
				this.PopulateMesh(vh, this.m_points);
				return;
			}
			if (this.m_segments != null && this.m_segments.Count > 0)
			{
				this.GeneratedUVs();
				vh.Clear();
				for (int i = 0; i < this.m_segments.Count; i++)
				{
					Vector2[] pointsToDraw = this.m_segments[i];
					this.PopulateMesh(vh, pointsToDraw);
				}
			}
		}

		// Token: 0x06003C62 RID: 15458 RVA: 0x001607A8 File Offset: 0x0015E9A8
		private UIVertex[] CreateLineCap(Vector2 start, Vector2 end, UILineRenderer.SegmentType type)
		{
			if (type == UILineRenderer.SegmentType.Start)
			{
				Vector2 start2 = start - (end - start).normalized * this.lineThickness / 2f;
				return this.CreateLineSegment(start2, start, UILineRenderer.SegmentType.Start, null);
			}
			if (type == UILineRenderer.SegmentType.End)
			{
				Vector2 end2 = end + (end - start).normalized * this.lineThickness / 2f;
				return this.CreateLineSegment(end, end2, UILineRenderer.SegmentType.End, null);
			}
			Debug.LogError("Bad SegmentType passed in to CreateLineCap. Must be SegmentType.Start or SegmentType.End");
			return null;
		}

		// Token: 0x06003C63 RID: 15459 RVA: 0x00160834 File Offset: 0x0015EA34
		private UIVertex[] CreateLineSegment(Vector2 start, Vector2 end, UILineRenderer.SegmentType type, UIVertex[] previousVert = null)
		{
			Vector2 b = new Vector2(start.y - end.y, end.x - start.x).normalized * this.lineThickness / 2f;
			Vector2 vector = Vector2.zero;
			Vector2 vector2 = Vector2.zero;
			if (previousVert != null)
			{
				vector = new Vector2(previousVert[3].position.x, previousVert[3].position.y);
				vector2 = new Vector2(previousVert[2].position.x, previousVert[2].position.y);
			}
			else
			{
				vector = start - b;
				vector2 = start + b;
			}
			Vector2 vector3 = end + b;
			Vector2 vector4 = end - b;
			switch (type)
			{
			case UILineRenderer.SegmentType.Start:
				return base.SetVbo(new Vector2[]
				{
					vector,
					vector2,
					vector3,
					vector4
				}, UILineRenderer.startUvs);
			case UILineRenderer.SegmentType.End:
				return base.SetVbo(new Vector2[]
				{
					vector,
					vector2,
					vector3,
					vector4
				}, UILineRenderer.endUvs);
			case UILineRenderer.SegmentType.Full:
				return base.SetVbo(new Vector2[]
				{
					vector,
					vector2,
					vector3,
					vector4
				}, UILineRenderer.fullUvs);
			}
			return base.SetVbo(new Vector2[]
			{
				vector,
				vector2,
				vector3,
				vector4
			}, UILineRenderer.middleUvs);
		}

		// Token: 0x06003C64 RID: 15460 RVA: 0x001609F0 File Offset: 0x0015EBF0
		protected override void GeneratedUVs()
		{
			if (base.activeSprite != null)
			{
				Vector4 outerUV = DataUtility.GetOuterUV(base.activeSprite);
				Vector4 innerUV = DataUtility.GetInnerUV(base.activeSprite);
				UILineRenderer.UV_TOP_LEFT = new Vector2(outerUV.x, outerUV.y);
				UILineRenderer.UV_BOTTOM_LEFT = new Vector2(outerUV.x, outerUV.w);
				UILineRenderer.UV_TOP_CENTER_LEFT = new Vector2(innerUV.x, innerUV.y);
				UILineRenderer.UV_TOP_CENTER_RIGHT = new Vector2(innerUV.z, innerUV.y);
				UILineRenderer.UV_BOTTOM_CENTER_LEFT = new Vector2(innerUV.x, innerUV.w);
				UILineRenderer.UV_BOTTOM_CENTER_RIGHT = new Vector2(innerUV.z, innerUV.w);
				UILineRenderer.UV_TOP_RIGHT = new Vector2(outerUV.z, outerUV.y);
				UILineRenderer.UV_BOTTOM_RIGHT = new Vector2(outerUV.z, outerUV.w);
			}
			else
			{
				UILineRenderer.UV_TOP_LEFT = Vector2.zero;
				UILineRenderer.UV_BOTTOM_LEFT = new Vector2(0f, 1f);
				UILineRenderer.UV_TOP_CENTER_LEFT = new Vector2(0.5f, 0f);
				UILineRenderer.UV_TOP_CENTER_RIGHT = new Vector2(0.5f, 0f);
				UILineRenderer.UV_BOTTOM_CENTER_LEFT = new Vector2(0.5f, 1f);
				UILineRenderer.UV_BOTTOM_CENTER_RIGHT = new Vector2(0.5f, 1f);
				UILineRenderer.UV_TOP_RIGHT = new Vector2(1f, 0f);
				UILineRenderer.UV_BOTTOM_RIGHT = Vector2.one;
			}
			UILineRenderer.startUvs = new Vector2[]
			{
				UILineRenderer.UV_TOP_LEFT,
				UILineRenderer.UV_BOTTOM_LEFT,
				UILineRenderer.UV_BOTTOM_CENTER_LEFT,
				UILineRenderer.UV_TOP_CENTER_LEFT
			};
			UILineRenderer.middleUvs = new Vector2[]
			{
				UILineRenderer.UV_TOP_CENTER_LEFT,
				UILineRenderer.UV_BOTTOM_CENTER_LEFT,
				UILineRenderer.UV_BOTTOM_CENTER_RIGHT,
				UILineRenderer.UV_TOP_CENTER_RIGHT
			};
			UILineRenderer.endUvs = new Vector2[]
			{
				UILineRenderer.UV_TOP_CENTER_RIGHT,
				UILineRenderer.UV_BOTTOM_CENTER_RIGHT,
				UILineRenderer.UV_BOTTOM_RIGHT,
				UILineRenderer.UV_TOP_RIGHT
			};
			UILineRenderer.fullUvs = new Vector2[]
			{
				UILineRenderer.UV_TOP_LEFT,
				UILineRenderer.UV_BOTTOM_LEFT,
				UILineRenderer.UV_BOTTOM_RIGHT,
				UILineRenderer.UV_TOP_RIGHT
			};
		}

		// Token: 0x06003C65 RID: 15461 RVA: 0x00160C54 File Offset: 0x0015EE54
		protected override void ResolutionToNativeSize(float distance)
		{
			if (base.UseNativeSize)
			{
				this.m_Resolution = distance / (base.activeSprite.rect.width / base.pixelsPerUnit);
				this.lineThickness = base.activeSprite.rect.height / base.pixelsPerUnit;
			}
		}

		// Token: 0x06003C66 RID: 15462 RVA: 0x00160CAC File Offset: 0x0015EEAC
		private int GetSegmentPointCount()
		{
			List<Vector2[]> segments = this.Segments;
			if (segments != null && segments.Count > 0)
			{
				int num = 0;
				foreach (Vector2[] array in this.Segments)
				{
					num += array.Length;
				}
				return num;
			}
			return this.Points.Length;
		}

		// Token: 0x06003C67 RID: 15463 RVA: 0x00160D24 File Offset: 0x0015EF24
		public Vector2 GetPosition(int index, int segmentIndex = 0)
		{
			if (segmentIndex > 0)
			{
				return this.Segments[segmentIndex - 1][index - 1];
			}
			if (this.Segments.Count > 0)
			{
				int num = 0;
				int num2 = index;
				foreach (Vector2[] array in this.Segments)
				{
					if (num2 - array.Length <= 0)
					{
						break;
					}
					num2 -= array.Length;
					num++;
				}
				return this.Segments[num][num2 - 1];
			}
			return this.Points[index - 1];
		}

		// Token: 0x06003C68 RID: 15464 RVA: 0x00160DD4 File Offset: 0x0015EFD4
		public Vector2 GetPositionBySegment(int index, int segment)
		{
			return this.Segments[segment][index - 1];
		}

		// Token: 0x06003C69 RID: 15465 RVA: 0x00160DEC File Offset: 0x0015EFEC
		public Vector2 GetClosestPoint(Vector2 p1, Vector2 p2, Vector2 p3)
		{
			Vector2 lhs = p3 - p1;
			Vector2 a = p2 - p1;
			float d = Mathf.Clamp01(Vector2.Dot(lhs, a.normalized) / a.magnitude);
			return p1 + a * d;
		}

		// Token: 0x040035A5 RID: 13733
		private const float MIN_MITER_JOIN = 0.2617994f;

		// Token: 0x040035A6 RID: 13734
		private const float MIN_BEVEL_NICE_JOIN = 0.5235988f;

		// Token: 0x040035A7 RID: 13735
		private static Vector2 UV_TOP_LEFT;

		// Token: 0x040035A8 RID: 13736
		private static Vector2 UV_BOTTOM_LEFT;

		// Token: 0x040035A9 RID: 13737
		private static Vector2 UV_TOP_CENTER_LEFT;

		// Token: 0x040035AA RID: 13738
		private static Vector2 UV_TOP_CENTER_RIGHT;

		// Token: 0x040035AB RID: 13739
		private static Vector2 UV_BOTTOM_CENTER_LEFT;

		// Token: 0x040035AC RID: 13740
		private static Vector2 UV_BOTTOM_CENTER_RIGHT;

		// Token: 0x040035AD RID: 13741
		private static Vector2 UV_TOP_RIGHT;

		// Token: 0x040035AE RID: 13742
		private static Vector2 UV_BOTTOM_RIGHT;

		// Token: 0x040035AF RID: 13743
		private static Vector2[] startUvs;

		// Token: 0x040035B0 RID: 13744
		private static Vector2[] middleUvs;

		// Token: 0x040035B1 RID: 13745
		private static Vector2[] endUvs;

		// Token: 0x040035B2 RID: 13746
		private static Vector2[] fullUvs;

		// Token: 0x040035B3 RID: 13747
		[SerializeField]
		[Tooltip("Points to draw lines between\n Can be improved using the Resolution Option")]
		internal Vector2[] m_points;

		// Token: 0x040035B4 RID: 13748
		[SerializeField]
		[Tooltip("Segments to be drawn\n This is a list of arrays of points")]
		internal List<Vector2[]> m_segments;

		// Token: 0x040035B5 RID: 13749
		[SerializeField]
		[Tooltip("Thickness of the line")]
		internal float lineThickness = 2f;

		// Token: 0x040035B6 RID: 13750
		[SerializeField]
		[Tooltip("Use the relative bounds of the Rect Transform (0,0 -> 0,1) or screen space coordinates")]
		internal bool relativeSize;

		// Token: 0x040035B7 RID: 13751
		[SerializeField]
		[Tooltip("Do the points identify a single line or split pairs of lines")]
		internal bool lineList;

		// Token: 0x040035B8 RID: 13752
		[SerializeField]
		[Tooltip("Add end caps to each line\nMultiple caps when used with Line List")]
		internal bool lineCaps;

		// Token: 0x040035B9 RID: 13753
		[SerializeField]
		[Tooltip("Resolution of the Bezier curve, different to line Resolution")]
		internal int bezierSegmentsPerCurve = 10;

		// Token: 0x040035BA RID: 13754
		[Tooltip("The type of Join used between lines, Square/Mitre or Curved/Bevel")]
		public UILineRenderer.JoinType LineJoins;

		// Token: 0x040035BB RID: 13755
		[Tooltip("Bezier method to apply to line, see docs for options\nCan't be used in conjunction with Resolution as Bezier already changes the resolution")]
		public UILineRenderer.BezierType BezierMode;

		// Token: 0x040035BC RID: 13756
		[HideInInspector]
		public bool drivenExternally;

		// Token: 0x02000EAE RID: 3758
		private enum SegmentType
		{
			// Token: 0x04004B90 RID: 19344
			Start,
			// Token: 0x04004B91 RID: 19345
			Middle,
			// Token: 0x04004B92 RID: 19346
			End,
			// Token: 0x04004B93 RID: 19347
			Full
		}

		// Token: 0x02000EAF RID: 3759
		public enum JoinType
		{
			// Token: 0x04004B95 RID: 19349
			Bevel,
			// Token: 0x04004B96 RID: 19350
			Miter
		}

		// Token: 0x02000EB0 RID: 3760
		public enum BezierType
		{
			// Token: 0x04004B98 RID: 19352
			None,
			// Token: 0x04004B99 RID: 19353
			Quick,
			// Token: 0x04004B9A RID: 19354
			Basic,
			// Token: 0x04004B9B RID: 19355
			Improved,
			// Token: 0x04004B9C RID: 19356
			Catenary
		}
	}
}
