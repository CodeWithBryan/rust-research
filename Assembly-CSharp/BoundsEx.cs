using System;
using UnityEngine;

// Token: 0x020008EB RID: 2283
public static class BoundsEx
{
	// Token: 0x06003690 RID: 13968 RVA: 0x00144694 File Offset: 0x00142894
	public static Bounds XZ3D(this Bounds bounds)
	{
		return new Bounds(bounds.center.XZ3D(), bounds.size.XZ3D());
	}

	// Token: 0x06003691 RID: 13969 RVA: 0x001446B4 File Offset: 0x001428B4
	public static Bounds Transform(this Bounds bounds, Matrix4x4 matrix)
	{
		Vector3 center = matrix.MultiplyPoint3x4(bounds.center);
		Vector3 extents = bounds.extents;
		Vector3 vector = matrix.MultiplyVector(new Vector3(extents.x, 0f, 0f));
		Vector3 vector2 = matrix.MultiplyVector(new Vector3(0f, extents.y, 0f));
		Vector3 vector3 = matrix.MultiplyVector(new Vector3(0f, 0f, extents.z));
		extents.x = Mathf.Abs(vector.x) + Mathf.Abs(vector2.x) + Mathf.Abs(vector3.x);
		extents.y = Mathf.Abs(vector.y) + Mathf.Abs(vector2.y) + Mathf.Abs(vector3.y);
		extents.z = Mathf.Abs(vector.z) + Mathf.Abs(vector2.z) + Mathf.Abs(vector3.z);
		return new Bounds
		{
			center = center,
			extents = extents
		};
	}

	// Token: 0x06003692 RID: 13970 RVA: 0x001447CC File Offset: 0x001429CC
	public static Rect ToScreenRect(this Bounds b, Camera cam)
	{
		Rect result;
		using (TimeWarning.New("Bounds.ToScreenRect", 0))
		{
			BoundsEx.pts[0] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
			BoundsEx.pts[1] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
			BoundsEx.pts[2] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
			BoundsEx.pts[3] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));
			BoundsEx.pts[4] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
			BoundsEx.pts[5] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
			BoundsEx.pts[6] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
			BoundsEx.pts[7] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));
			Vector3 vector = BoundsEx.pts[0];
			Vector3 vector2 = BoundsEx.pts[0];
			for (int i = 1; i < BoundsEx.pts.Length; i++)
			{
				vector = Vector3.Min(vector, BoundsEx.pts[i]);
				vector2 = Vector3.Max(vector2, BoundsEx.pts[i]);
			}
			result = Rect.MinMaxRect(vector.x, vector.y, vector2.x, vector2.y);
		}
		return result;
	}

	// Token: 0x06003693 RID: 13971 RVA: 0x00144B84 File Offset: 0x00142D84
	public static Rect ToCanvasRect(this Bounds b, RectTransform target, Camera cam)
	{
		Rect result = b.ToScreenRect(cam);
		result.min = result.min.ToCanvas(target, null);
		result.max = result.max.ToCanvas(target, null);
		return result;
	}

	// Token: 0x0400318E RID: 12686
	private static Vector3[] pts = new Vector3[8];
}
