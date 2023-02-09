using System;
using UnityEngine;

// Token: 0x02000439 RID: 1081
public class TreeMarkerData : PrefabAttribute, IServerComponent
{
	// Token: 0x06002389 RID: 9097 RVA: 0x000E106E File Offset: 0x000DF26E
	protected override Type GetIndexedType()
	{
		return typeof(TreeMarkerData);
	}

	// Token: 0x0600238A RID: 9098 RVA: 0x000E107C File Offset: 0x000DF27C
	public Vector3 GetNearbyPoint(Vector3 point, ref int ignoreIndex, out Vector3 normal)
	{
		int num = this.Markers.Length;
		if (ignoreIndex != -1 && this.ProcessAngleChecks)
		{
			ignoreIndex++;
			if (ignoreIndex >= num)
			{
				ignoreIndex = 0;
			}
			normal = this.Markers[ignoreIndex].LocalNormal;
			return this.Markers[ignoreIndex].LocalPosition;
		}
		int num2 = UnityEngine.Random.Range(0, num);
		float num3 = float.MaxValue;
		int num4 = -1;
		for (int i = 0; i < num; i++)
		{
			if (ignoreIndex != num2)
			{
				TreeMarkerData.MarkerLocation markerLocation = this.Markers[num2];
				if (markerLocation.LocalPosition.y >= this.MinY)
				{
					Vector3 localPosition = markerLocation.LocalPosition;
					localPosition.y = Mathf.Lerp(localPosition.y, point.y, 0.5f);
					float num5 = (localPosition - point).sqrMagnitude;
					num5 *= UnityEngine.Random.Range(0.95f, 1.05f);
					if (num5 < num3)
					{
						num3 = num5;
						num4 = num2;
					}
					num2++;
					if (num2 >= num)
					{
						num2 = 0;
					}
				}
			}
		}
		if (num4 > -1)
		{
			normal = this.Markers[num4].LocalNormal;
			ignoreIndex = num4;
			return this.Markers[num4].LocalPosition;
		}
		normal = this.Markers[0].LocalNormal;
		return this.Markers[0].LocalPosition;
	}

	// Token: 0x04001C36 RID: 7222
	public TreeMarkerData.GenerationArc[] GenerationArcs;

	// Token: 0x04001C37 RID: 7223
	public TreeMarkerData.MarkerLocation[] Markers;

	// Token: 0x04001C38 RID: 7224
	public Vector3 GenerationStartPoint = Vector3.up * 2f;

	// Token: 0x04001C39 RID: 7225
	public float GenerationRadius = 2f;

	// Token: 0x04001C3A RID: 7226
	public float MaxY = 1.7f;

	// Token: 0x04001C3B RID: 7227
	public float MinY = 0.2f;

	// Token: 0x04001C3C RID: 7228
	public bool ProcessAngleChecks;

	// Token: 0x02000C97 RID: 3223
	[Serializable]
	public struct MarkerLocation
	{
		// Token: 0x04004330 RID: 17200
		public Vector3 LocalPosition;

		// Token: 0x04004331 RID: 17201
		public Vector3 LocalNormal;
	}

	// Token: 0x02000C98 RID: 3224
	[Serializable]
	public struct GenerationArc
	{
		// Token: 0x04004332 RID: 17202
		public Vector3 CentrePoint;

		// Token: 0x04004333 RID: 17203
		public float Radius;

		// Token: 0x04004334 RID: 17204
		public Vector3 Rotation;

		// Token: 0x04004335 RID: 17205
		public int OverrideCount;
	}
}
