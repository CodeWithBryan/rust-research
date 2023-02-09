using System;
using UnityEngine;

// Token: 0x0200001E RID: 30
public class CargoMoveTest : FacepunchBehaviour
{
	// Token: 0x060000A0 RID: 160 RVA: 0x000055A0 File Offset: 0x000037A0
	private void Awake()
	{
		base.Invoke(new Action(this.FindInitialNode), 2f);
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x000055B9 File Offset: 0x000037B9
	public void FindInitialNode()
	{
		this.targetNodeIndex = this.GetClosestNodeToUs();
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x000055C7 File Offset: 0x000037C7
	private void Update()
	{
		this.UpdateMovement();
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x000055D0 File Offset: 0x000037D0
	public void UpdateMovement()
	{
		if (TerrainMeta.Path.OceanPatrolFar == null || TerrainMeta.Path.OceanPatrolFar.Count == 0)
		{
			return;
		}
		if (this.targetNodeIndex == -1)
		{
			return;
		}
		Vector3 vector = TerrainMeta.Path.OceanPatrolFar[this.targetNodeIndex];
		Vector3 normalized = (vector - base.transform.position).normalized;
		float value = Vector3.Dot(base.transform.forward, normalized);
		float b = Mathf.InverseLerp(0.5f, 1f, value);
		float num = Vector3.Dot(base.transform.right, normalized);
		float num2 = 5f;
		float b2 = Mathf.InverseLerp(0.05f, 0.5f, Mathf.Abs(num));
		this.turnScale = Mathf.Lerp(this.turnScale, b2, Time.deltaTime * 0.2f);
		float num3 = (float)((num < 0f) ? -1 : 1);
		base.transform.Rotate(Vector3.up, num2 * Time.deltaTime * this.turnScale * num3, Space.World);
		this.currentThrottle = Mathf.Lerp(this.currentThrottle, b, Time.deltaTime * 0.2f);
		base.transform.position += base.transform.forward * 5f * Time.deltaTime * this.currentThrottle;
		if (Vector3.Distance(base.transform.position, vector) < 60f)
		{
			this.targetNodeIndex++;
			if (this.targetNodeIndex >= TerrainMeta.Path.OceanPatrolFar.Count)
			{
				this.targetNodeIndex = 0;
			}
		}
	}

	// Token: 0x060000A4 RID: 164 RVA: 0x00005784 File Offset: 0x00003984
	public int GetClosestNodeToUs()
	{
		int result = 0;
		float num = float.PositiveInfinity;
		for (int i = 0; i < TerrainMeta.Path.OceanPatrolFar.Count; i++)
		{
			Vector3 b = TerrainMeta.Path.OceanPatrolFar[i];
			float num2 = Vector3.Distance(base.transform.position, b);
			if (num2 < num)
			{
				result = i;
				num = num2;
			}
		}
		return result;
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x000057E4 File Offset: 0x000039E4
	public void OnDrawGizmosSelected()
	{
		if (TerrainMeta.Path.OceanPatrolFar != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(TerrainMeta.Path.OceanPatrolFar[this.targetNodeIndex], 10f);
			for (int i = 0; i < TerrainMeta.Path.OceanPatrolFar.Count; i++)
			{
				Vector3 vector = TerrainMeta.Path.OceanPatrolFar[i];
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(vector, 3f);
				Vector3 to = (i + 1 == TerrainMeta.Path.OceanPatrolFar.Count) ? TerrainMeta.Path.OceanPatrolFar[0] : TerrainMeta.Path.OceanPatrolFar[i + 1];
				Gizmos.DrawLine(vector, to);
			}
		}
	}

	// Token: 0x040000B3 RID: 179
	public int targetNodeIndex = -1;

	// Token: 0x040000B4 RID: 180
	private float currentThrottle;

	// Token: 0x040000B5 RID: 181
	private float turnScale;
}
