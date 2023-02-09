using System;
using UnityEngine;

// Token: 0x0200016D RID: 365
public class PlacementTest : MonoBehaviour
{
	// Token: 0x06001699 RID: 5785 RVA: 0x000AB660 File Offset: 0x000A9860
	public Vector3 RandomHemisphereDirection(Vector3 input, float degreesOffset)
	{
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		Vector3 b = new Vector3(insideUnitCircle.x * degreesOffset, UnityEngine.Random.Range(-1f, 1f) * degreesOffset, insideUnitCircle.y * degreesOffset);
		return (input + b).normalized;
	}

	// Token: 0x0600169A RID: 5786 RVA: 0x000AB6C4 File Offset: 0x000A98C4
	public Vector3 RandomCylinderPointAroundVector(Vector3 input, float distance, float minHeight = 0f, float maxHeight = 0f)
	{
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		Vector3 normalized = new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y).normalized;
		return new Vector3(normalized.x * distance, UnityEngine.Random.Range(minHeight, maxHeight), normalized.z * distance);
	}

	// Token: 0x0600169B RID: 5787 RVA: 0x000AB714 File Offset: 0x000A9914
	public Vector3 ClampToHemisphere(Vector3 hemiInput, float degreesOffset, Vector3 inputVec)
	{
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector3 normalized = (hemiInput + Vector3.one * degreesOffset).normalized;
		Vector3 normalized2 = (hemiInput + Vector3.one * -degreesOffset).normalized;
		for (int i = 0; i < 3; i++)
		{
			inputVec[i] = Mathf.Clamp(inputVec[i], normalized2[i], normalized[i]);
		}
		return inputVec.normalized;
	}

	// Token: 0x0600169C RID: 5788 RVA: 0x000AB7A8 File Offset: 0x000A99A8
	private void Update()
	{
		if (Time.realtimeSinceStartup < this.nextTest)
		{
			return;
		}
		this.nextTest = Time.realtimeSinceStartup + 0f;
		Vector3 position = this.RandomCylinderPointAroundVector(Vector3.up, 0.5f, 0.25f, 0.5f);
		position = base.transform.TransformPoint(position);
		this.testTransform.transform.position = position;
		if (this.testTransform != null && this.visualTest != null)
		{
			Vector3 position2 = base.transform.position;
			RaycastHit raycastHit;
			if (this.myMeshCollider.Raycast(new Ray(this.testTransform.position, (base.transform.position - this.testTransform.position).normalized), out raycastHit, 5f))
			{
				position2 = raycastHit.point;
			}
			else
			{
				Debug.LogError("Missed");
			}
			this.visualTest.transform.position = position2;
		}
	}

	// Token: 0x0600169D RID: 5789 RVA: 0x000059DD File Offset: 0x00003BDD
	public void OnDrawGizmos()
	{
	}

	// Token: 0x04000FAA RID: 4010
	public MeshCollider myMeshCollider;

	// Token: 0x04000FAB RID: 4011
	public Transform testTransform;

	// Token: 0x04000FAC RID: 4012
	public Transform visualTest;

	// Token: 0x04000FAD RID: 4013
	public float hemisphere = 45f;

	// Token: 0x04000FAE RID: 4014
	public float clampTest = 45f;

	// Token: 0x04000FAF RID: 4015
	public float testDist = 2f;

	// Token: 0x04000FB0 RID: 4016
	private float nextTest;
}
