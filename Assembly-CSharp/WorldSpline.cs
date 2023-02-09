using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000922 RID: 2338
public class WorldSpline : MonoBehaviour
{
	// Token: 0x060037B6 RID: 14262 RVA: 0x0014A4E8 File Offset: 0x001486E8
	public WorldSplineData GetData()
	{
		WorldSplineData result;
		if (WorldSplineSharedData.TryGetDataFor(this, out result))
		{
			return result;
		}
		if (Application.isPlaying && this.privateData == null)
		{
			this.privateData = new WorldSplineData(this);
		}
		return this.privateData;
	}

	// Token: 0x060037B7 RID: 14263 RVA: 0x0014A522 File Offset: 0x00148722
	public void SetAll(Vector3[] points, Vector3[] tangents, float lutInterval)
	{
		this.points = points;
		this.tangents = tangents;
		this.lutInterval = lutInterval;
	}

	// Token: 0x060037B8 RID: 14264 RVA: 0x0014A53C File Offset: 0x0014873C
	public void CheckValidity()
	{
		this.lutInterval = Mathf.Clamp(this.lutInterval, 0.05f, 100f);
		if (this.points == null || this.points.Length < 2)
		{
			this.points = new Vector3[2];
			this.points[0] = Vector3.zero;
			this.points[1] = Vector3.zero;
		}
		if (this.tangents == null || this.points.Length != this.tangents.Length)
		{
			Vector3[] array = new Vector3[this.points.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (this.tangents != null && i < this.tangents.Length)
				{
					array[i] = this.tangents[i];
				}
				else
				{
					array[i] = Vector3.forward;
				}
			}
			this.tangents = array;
		}
	}

	// Token: 0x060037B9 RID: 14265 RVA: 0x0014A616 File Offset: 0x00148816
	protected virtual void OnDrawGizmosSelected()
	{
		if (this.showGizmos)
		{
			WorldSpline.DrawSplineGizmo(this, Color.magenta);
		}
	}

	// Token: 0x060037BA RID: 14266 RVA: 0x0014A62C File Offset: 0x0014882C
	protected static void DrawSplineGizmo(WorldSpline ws, Color splineColour)
	{
		if (ws == null)
		{
			return;
		}
		WorldSplineData data = ws.GetData();
		if (data == null)
		{
			return;
		}
		if (ws.points.Length < 2 || ws.points.Length != ws.tangents.Length)
		{
			return;
		}
		Vector3[] pointsWorld = ws.GetPointsWorld();
		Vector3[] tangentsWorld = ws.GetTangentsWorld();
		for (int i = 0; i < pointsWorld.Length; i++)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(pointsWorld[i], 0.25f);
			if (tangentsWorld[i].magnitude > 0f)
			{
				Gizmos.color = Color.cyan;
				Vector3 to = pointsWorld[i] + tangentsWorld[i] + Vector3.up * 0.1f;
				Gizmos.DrawLine(pointsWorld[i] + Vector3.up * 0.1f, to);
			}
		}
		Gizmos.color = splineColour;
		Vector3[] visualSpline = WorldSpline.GetVisualSpline(ws, data, 1f);
		for (int j = 0; j < visualSpline.Length - 1; j++)
		{
			Gizmos.color = Color.Lerp(Color.white, splineColour, (float)j / (float)(visualSpline.Length - 1));
			Gizmos.DrawLine(visualSpline[j], visualSpline[j + 1]);
			Gizmos.DrawLine(visualSpline[j], visualSpline[j] + Vector3.up * 0.25f);
		}
	}

	// Token: 0x060037BB RID: 14267 RVA: 0x0014A79C File Offset: 0x0014899C
	private static Vector3[] GetVisualSpline(WorldSpline ws, WorldSplineData data, float distBetweenPoints)
	{
		WorldSpline.visualSplineList.Clear();
		if (ws != null && ws.points.Length > 1)
		{
			Vector3 startPointWorld = ws.GetStartPointWorld();
			Vector3 endPointWorld = ws.GetEndPointWorld();
			WorldSpline.visualSplineList.Add(startPointWorld);
			for (float num = distBetweenPoints; num <= data.Length - distBetweenPoints; num += distBetweenPoints)
			{
				WorldSpline.visualSplineList.Add(ws.GetPointCubicHermiteWorld(num, data));
			}
			WorldSpline.visualSplineList.Add(endPointWorld);
		}
		return WorldSpline.visualSplineList.ToArray();
	}

	// Token: 0x060037BC RID: 14268 RVA: 0x0014A81A File Offset: 0x00148A1A
	public Vector3 GetStartPointWorld()
	{
		return base.transform.TransformPoint(this.points[0]);
	}

	// Token: 0x060037BD RID: 14269 RVA: 0x0014A833 File Offset: 0x00148A33
	public Vector3 GetEndPointWorld()
	{
		return base.transform.TransformPoint(this.points[this.points.Length - 1]);
	}

	// Token: 0x060037BE RID: 14270 RVA: 0x0014A855 File Offset: 0x00148A55
	public Vector3 GetStartTangentWorld()
	{
		return Vector3.Scale(base.transform.rotation * this.tangents[0], base.transform.localScale);
	}

	// Token: 0x060037BF RID: 14271 RVA: 0x0014A883 File Offset: 0x00148A83
	public Vector3 GetEndTangentWorld()
	{
		return Vector3.Scale(base.transform.rotation * this.tangents[this.tangents.Length - 1], base.transform.localScale);
	}

	// Token: 0x060037C0 RID: 14272 RVA: 0x0014A8BA File Offset: 0x00148ABA
	public Vector3 GetTangentCubicHermiteWorld(float distance)
	{
		return Vector3.Scale(base.transform.rotation * this.GetData().GetTangentCubicHermite(distance), base.transform.localScale);
	}

	// Token: 0x060037C1 RID: 14273 RVA: 0x0014A8E8 File Offset: 0x00148AE8
	public Vector3 GetTangentCubicHermiteWorld(float distance, WorldSplineData data)
	{
		return Vector3.Scale(base.transform.rotation * data.GetTangentCubicHermite(distance), base.transform.localScale);
	}

	// Token: 0x060037C2 RID: 14274 RVA: 0x0014A911 File Offset: 0x00148B11
	public Vector3 GetPointCubicHermiteWorld(float distance)
	{
		return base.transform.TransformPoint(this.GetData().GetPointCubicHermite(distance));
	}

	// Token: 0x060037C3 RID: 14275 RVA: 0x0014A92A File Offset: 0x00148B2A
	public Vector3 GetPointCubicHermiteWorld(float distance, WorldSplineData data)
	{
		return base.transform.TransformPoint(data.GetPointCubicHermite(distance));
	}

	// Token: 0x060037C4 RID: 14276 RVA: 0x0014A940 File Offset: 0x00148B40
	public Vector3 GetPointAndTangentCubicHermiteWorld(float distance, out Vector3 tangent)
	{
		Vector3 pointAndTangentCubicHermite = this.GetData().GetPointAndTangentCubicHermite(distance, out tangent);
		tangent = base.transform.TransformVector(tangent);
		return base.transform.TransformPoint(pointAndTangentCubicHermite);
	}

	// Token: 0x060037C5 RID: 14277 RVA: 0x0014A980 File Offset: 0x00148B80
	public Vector3 GetPointAndTangentCubicHermiteWorld(float distance, WorldSplineData data, out Vector3 tangent)
	{
		Vector3 pointAndTangentCubicHermite = data.GetPointAndTangentCubicHermite(distance, out tangent);
		tangent = base.transform.TransformVector(tangent);
		return base.transform.TransformPoint(pointAndTangentCubicHermite);
	}

	// Token: 0x060037C6 RID: 14278 RVA: 0x0014A9B9 File Offset: 0x00148BB9
	public Vector3[] GetPointsWorld()
	{
		return WorldSpline.PointsToWorld(this.points, base.transform);
	}

	// Token: 0x060037C7 RID: 14279 RVA: 0x0014A9CC File Offset: 0x00148BCC
	public Vector3[] GetTangentsWorld()
	{
		return WorldSpline.TangentsToWorld(this.tangents, base.transform);
	}

	// Token: 0x060037C8 RID: 14280 RVA: 0x0014A9E0 File Offset: 0x00148BE0
	private static Vector3[] PointsToWorld(Vector3[] points, Transform tr)
	{
		Vector3[] array = new Vector3[points.Length];
		for (int i = 0; i < points.Length; i++)
		{
			array[i] = tr.TransformPoint(points[i]);
		}
		return array;
	}

	// Token: 0x060037C9 RID: 14281 RVA: 0x0014AA1C File Offset: 0x00148C1C
	private static Vector3[] TangentsToWorld(Vector3[] tangents, Transform tr)
	{
		Vector3[] array = new Vector3[tangents.Length];
		for (int i = 0; i < tangents.Length; i++)
		{
			array[i] = Vector3.Scale(tr.rotation * tangents[i], tr.localScale);
		}
		return array;
	}

	// Token: 0x040031E3 RID: 12771
	public int dataIndex = -1;

	// Token: 0x040031E4 RID: 12772
	public Vector3[] points;

	// Token: 0x040031E5 RID: 12773
	public Vector3[] tangents;

	// Token: 0x040031E6 RID: 12774
	[Range(0.05f, 100f)]
	public float lutInterval = 0.25f;

	// Token: 0x040031E7 RID: 12775
	[SerializeField]
	private bool showGizmos = true;

	// Token: 0x040031E8 RID: 12776
	private static List<Vector3> visualSplineList = new List<Vector3>();

	// Token: 0x040031E9 RID: 12777
	private WorldSplineData privateData;
}
