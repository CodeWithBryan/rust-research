using System;
using UnityEngine;

// Token: 0x02000203 RID: 515
public class BasePathFinder
{
	// Token: 0x06001A74 RID: 6772 RVA: 0x00029180 File Offset: 0x00027380
	public virtual Vector3 GetRandomPatrolPoint()
	{
		return Vector3.zero;
	}

	// Token: 0x06001A75 RID: 6773 RVA: 0x0002A0CF File Offset: 0x000282CF
	public virtual AIMovePoint GetBestRoamPoint(Vector3 anchorPos, Vector3 currentPos, Vector3 currentDirection, float anchorClampDistance, float lookupMaxRange = 20f)
	{
		return null;
	}

	// Token: 0x06001A76 RID: 6774 RVA: 0x000BAF20 File Offset: 0x000B9120
	public void DebugDraw()
	{
		Color color = Gizmos.color;
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(this.chosenPosition, 5f);
		Gizmos.color = Color.blue;
		Vector3[] array = BasePathFinder.topologySamples;
		for (int i = 0; i < array.Length; i++)
		{
			Gizmos.DrawSphere(array[i], 2.5f);
		}
		Gizmos.color = color;
	}

	// Token: 0x06001A77 RID: 6775 RVA: 0x000BAF84 File Offset: 0x000B9184
	public virtual Vector3 GetRandomPositionAround(Vector3 position, float minDistFrom = 0f, float maxDistFrom = 2f)
	{
		if (maxDistFrom < 0f)
		{
			maxDistFrom = 0f;
		}
		Vector2 vector = UnityEngine.Random.insideUnitCircle * maxDistFrom;
		float x = Mathf.Clamp(Mathf.Max(Mathf.Abs(vector.x), minDistFrom), minDistFrom, maxDistFrom) * Mathf.Sign(vector.x);
		float z = Mathf.Clamp(Mathf.Max(Mathf.Abs(vector.y), minDistFrom), minDistFrom, maxDistFrom) * Mathf.Sign(vector.y);
		return position + new Vector3(x, 0f, z);
	}

	// Token: 0x06001A78 RID: 6776 RVA: 0x000BB008 File Offset: 0x000B9208
	public virtual Vector3 GetBestRoamPosition(BaseNavigator navigator, Vector3 fallbackPos, float minRange, float maxRange)
	{
		float radius = UnityEngine.Random.Range(minRange, maxRange);
		int num = 0;
		int num2 = 0;
		float num3 = UnityEngine.Random.Range(0f, 90f);
		for (float num4 = 0f; num4 < 360f; num4 += 90f)
		{
			Vector3 pointOnCircle = BasePathFinder.GetPointOnCircle(navigator.transform.position, radius, num4 + num3);
			Vector3 vector;
			if (navigator.GetNearestNavmeshPosition(pointOnCircle, out vector, 10f) && navigator.IsPositionABiomeRequirement(vector) && navigator.IsAcceptableWaterDepth(vector) && !navigator.IsPositionPreventTopology(vector))
			{
				BasePathFinder.topologySamples[num] = vector;
				num++;
				if (navigator.IsPositionABiomePreference(vector) && navigator.IsPositionATopologyPreference(vector))
				{
					BasePathFinder.preferedTopologySamples[num2] = vector;
					num2++;
				}
			}
		}
		if (num2 > 0)
		{
			this.chosenPosition = BasePathFinder.preferedTopologySamples[UnityEngine.Random.Range(0, num2)];
		}
		else if (num > 0)
		{
			this.chosenPosition = BasePathFinder.topologySamples[UnityEngine.Random.Range(0, num)];
		}
		else
		{
			this.chosenPosition = fallbackPos;
		}
		return this.chosenPosition;
	}

	// Token: 0x06001A79 RID: 6777 RVA: 0x000BB118 File Offset: 0x000B9318
	public virtual Vector3 GetBestRoamPositionFromAnchor(BaseNavigator navigator, Vector3 anchorPos, Vector3 fallbackPos, float minRange, float maxRange)
	{
		float radius = UnityEngine.Random.Range(minRange, maxRange);
		int num = 0;
		int num2 = 0;
		float num3 = UnityEngine.Random.Range(0f, 90f);
		for (float num4 = 0f; num4 < 360f; num4 += 90f)
		{
			Vector3 pointOnCircle = BasePathFinder.GetPointOnCircle(anchorPos, radius, num4 + num3);
			Vector3 vector;
			if (navigator.GetNearestNavmeshPosition(pointOnCircle, out vector, 10f) && navigator.IsAcceptableWaterDepth(vector))
			{
				BasePathFinder.topologySamples[num] = vector;
				num++;
				if (navigator.IsPositionABiomePreference(vector) && navigator.IsPositionATopologyPreference(vector))
				{
					BasePathFinder.preferedTopologySamples[num2] = vector;
					num2++;
				}
			}
		}
		if (UnityEngine.Random.Range(0f, 1f) <= 0.9f && num2 > 0)
		{
			this.chosenPosition = BasePathFinder.preferedTopologySamples[UnityEngine.Random.Range(0, num2)];
		}
		else if (num > 0)
		{
			this.chosenPosition = BasePathFinder.topologySamples[UnityEngine.Random.Range(0, num)];
		}
		else
		{
			this.chosenPosition = fallbackPos;
		}
		return this.chosenPosition;
	}

	// Token: 0x06001A7A RID: 6778 RVA: 0x000BB220 File Offset: 0x000B9420
	public virtual bool GetBestFleePosition(BaseNavigator navigator, AIBrainSenses senses, BaseEntity fleeFrom, Vector3 fallbackPos, float minRange, float maxRange, out Vector3 result)
	{
		if (fleeFrom == null)
		{
			result = navigator.transform.position;
			return false;
		}
		Vector3 dirFromThreat = Vector3Ex.Direction2D(navigator.transform.position, fleeFrom.transform.position);
		if (this.TestFleeDirection(navigator, dirFromThreat, 0f, minRange, maxRange, out result))
		{
			return true;
		}
		bool flag = UnityEngine.Random.Range(0, 2) == 1;
		return this.TestFleeDirection(navigator, dirFromThreat, flag ? 45f : 315f, minRange, maxRange, out result) || this.TestFleeDirection(navigator, dirFromThreat, flag ? 315f : 45f, minRange, maxRange, out result) || this.TestFleeDirection(navigator, dirFromThreat, flag ? 90f : 270f, minRange, maxRange, out result) || this.TestFleeDirection(navigator, dirFromThreat, flag ? 270f : 90f, minRange, maxRange, out result) || this.TestFleeDirection(navigator, dirFromThreat, 135f + UnityEngine.Random.Range(0f, 90f), minRange, maxRange, out result);
	}

	// Token: 0x06001A7B RID: 6779 RVA: 0x000BB334 File Offset: 0x000B9534
	private bool TestFleeDirection(BaseNavigator navigator, Vector3 dirFromThreat, float offsetDegrees, float minRange, float maxRange, out Vector3 result)
	{
		result = navigator.transform.position;
		Vector3 a = Quaternion.Euler(0f, offsetDegrees, 0f) * dirFromThreat;
		Vector3 target = navigator.transform.position + a * UnityEngine.Random.Range(minRange, maxRange);
		Vector3 vector;
		if (!navigator.GetNearestNavmeshPosition(target, out vector, 20f))
		{
			return false;
		}
		if (!navigator.IsAcceptableWaterDepth(vector))
		{
			return false;
		}
		result = vector;
		return true;
	}

	// Token: 0x06001A7C RID: 6780 RVA: 0x000BB3B0 File Offset: 0x000B95B0
	public static Vector3 GetPointOnCircle(Vector3 center, float radius, float degrees)
	{
		float x = center.x + radius * Mathf.Cos(degrees * 0.017453292f);
		float z = center.z + radius * Mathf.Sin(degrees * 0.017453292f);
		return new Vector3(x, center.y, z);
	}

	// Token: 0x040012BF RID: 4799
	private static Vector3[] preferedTopologySamples = new Vector3[4];

	// Token: 0x040012C0 RID: 4800
	private static Vector3[] topologySamples = new Vector3[4];

	// Token: 0x040012C1 RID: 4801
	private Vector3 chosenPosition;

	// Token: 0x040012C2 RID: 4802
	private const float halfPI = 0.017453292f;
}
