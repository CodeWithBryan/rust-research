using System;
using UnityEngine;

// Token: 0x020001C2 RID: 450
public class AICoverPointTool : MonoBehaviour
{
	// Token: 0x06001802 RID: 6146 RVA: 0x000B1ADC File Offset: 0x000AFCDC
	[ContextMenu("Place Cover Points")]
	public void PlaceCoverPoints()
	{
		foreach (object obj in base.transform)
		{
			UnityEngine.Object.DestroyImmediate(((Transform)obj).gameObject);
		}
		Vector3 pos = new Vector3(base.transform.position.x - 50f, base.transform.position.y, base.transform.position.z - 50f);
		for (int i = 0; i < 50; i++)
		{
			for (int j = 0; j < 50; j++)
			{
				AICoverPointTool.TestResult testResult = this.TestPoint(pos);
				if (testResult.Valid)
				{
					this.PlacePoint(testResult);
				}
				pos.x += 2f;
			}
			pos.x -= 100f;
			pos.z += 2f;
		}
	}

	// Token: 0x06001803 RID: 6147 RVA: 0x000B1BE4 File Offset: 0x000AFDE4
	private AICoverPointTool.TestResult TestPoint(Vector3 pos)
	{
		pos.y += 0.5f;
		AICoverPointTool.TestResult result = default(AICoverPointTool.TestResult);
		result.Position = pos;
		if (this.HitsCover(new Ray(pos, Vector3.forward), 1218519041, 1f))
		{
			result.Forward = true;
			result.Valid = true;
		}
		if (this.HitsCover(new Ray(pos, Vector3.right), 1218519041, 1f))
		{
			result.Right = true;
			result.Valid = true;
		}
		if (this.HitsCover(new Ray(pos, Vector3.back), 1218519041, 1f))
		{
			result.Backward = true;
			result.Valid = true;
		}
		if (this.HitsCover(new Ray(pos, Vector3.left), 1218519041, 1f))
		{
			result.Left = true;
			result.Valid = true;
		}
		return result;
	}

	// Token: 0x06001804 RID: 6148 RVA: 0x000B1CC8 File Offset: 0x000AFEC8
	private void PlacePoint(AICoverPointTool.TestResult result)
	{
		if (result.Forward)
		{
			this.PlacePoint(result.Position, Vector3.forward);
		}
		if (result.Right)
		{
			this.PlacePoint(result.Position, Vector3.right);
		}
		if (result.Backward)
		{
			this.PlacePoint(result.Position, Vector3.back);
		}
		if (result.Left)
		{
			this.PlacePoint(result.Position, Vector3.left);
		}
	}

	// Token: 0x06001805 RID: 6149 RVA: 0x000B1D39 File Offset: 0x000AFF39
	private void PlacePoint(Vector3 pos, Vector3 dir)
	{
		AICoverPoint aicoverPoint = new GameObject("CP").AddComponent<AICoverPoint>();
		aicoverPoint.transform.position = pos;
		aicoverPoint.transform.forward = dir;
		aicoverPoint.transform.SetParent(base.transform);
	}

	// Token: 0x06001806 RID: 6150 RVA: 0x000B1D74 File Offset: 0x000AFF74
	public bool HitsCover(Ray ray, int layerMask, float maxDistance)
	{
		RaycastHit raycastHit;
		return !ray.origin.IsNaNOrInfinity() && !ray.direction.IsNaNOrInfinity() && !(ray.direction == Vector3.zero) && GamePhysics.Trace(ray, 0f, out raycastHit, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal, null);
	}

	// Token: 0x02000BEC RID: 3052
	private struct TestResult
	{
		// Token: 0x04004032 RID: 16434
		public Vector3 Position;

		// Token: 0x04004033 RID: 16435
		public bool Valid;

		// Token: 0x04004034 RID: 16436
		public bool Forward;

		// Token: 0x04004035 RID: 16437
		public bool Right;

		// Token: 0x04004036 RID: 16438
		public bool Backward;

		// Token: 0x04004037 RID: 16439
		public bool Left;
	}
}
