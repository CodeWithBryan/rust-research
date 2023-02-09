using System;
using UnityEngine;

// Token: 0x02000151 RID: 337
public class JiggleBone : BaseMonoBehaviour
{
	// Token: 0x06001649 RID: 5705 RVA: 0x000A98D8 File Offset: 0x000A7AD8
	private void Awake()
	{
		Vector3 vector = base.transform.position + base.transform.TransformDirection(new Vector3(this.boneAxis.x * this.targetDistance, this.boneAxis.y * this.targetDistance, this.boneAxis.z * this.targetDistance));
		this.dynamicPos = vector;
	}

	// Token: 0x0600164A RID: 5706 RVA: 0x000A9944 File Offset: 0x000A7B44
	private void LateUpdate()
	{
		base.transform.rotation = default(Quaternion);
		Vector3 dir = base.transform.TransformDirection(new Vector3(this.boneAxis.x * this.targetDistance, this.boneAxis.y * this.targetDistance, this.boneAxis.z * this.targetDistance));
		Vector3 vector = base.transform.TransformDirection(new Vector3(0f, 1f, 0f));
		Vector3 vector2 = base.transform.position + base.transform.TransformDirection(new Vector3(this.boneAxis.x * this.targetDistance, this.boneAxis.y * this.targetDistance, this.boneAxis.z * this.targetDistance));
		this.force.x = (vector2.x - this.dynamicPos.x) * this.bStiffness;
		this.acc.x = this.force.x / this.bMass;
		this.vel.x = this.vel.x + this.acc.x * (1f - this.bDamping);
		this.force.y = (vector2.y - this.dynamicPos.y) * this.bStiffness;
		this.force.y = this.force.y - this.bGravity / 10f;
		this.acc.y = this.force.y / this.bMass;
		this.vel.y = this.vel.y + this.acc.y * (1f - this.bDamping);
		this.force.z = (vector2.z - this.dynamicPos.z) * this.bStiffness;
		this.acc.z = this.force.z / this.bMass;
		this.vel.z = this.vel.z + this.acc.z * (1f - this.bDamping);
		this.dynamicPos += this.vel + this.force;
		base.transform.LookAt(this.dynamicPos, vector);
		if (this.SquashAndStretch)
		{
			float magnitude = (this.dynamicPos - vector2).magnitude;
			float x;
			if (this.boneAxis.x == 0f)
			{
				x = 1f + -magnitude * this.sideStretch;
			}
			else
			{
				x = 1f + magnitude * this.frontStretch;
			}
			float y;
			if (this.boneAxis.y == 0f)
			{
				y = 1f + -magnitude * this.sideStretch;
			}
			else
			{
				y = 1f + magnitude * this.frontStretch;
			}
			float z;
			if (this.boneAxis.z == 0f)
			{
				z = 1f + -magnitude * this.sideStretch;
			}
			else
			{
				z = 1f + magnitude * this.frontStretch;
			}
			base.transform.localScale = new Vector3(x, y, z);
		}
		if (this.debugMode)
		{
			Debug.DrawRay(base.transform.position, dir, Color.blue);
			Debug.DrawRay(base.transform.position, vector, Color.green);
			Debug.DrawRay(vector2, Vector3.up * 0.2f, Color.yellow);
			Debug.DrawRay(this.dynamicPos, Vector3.up * 0.2f, Color.red);
		}
	}

	// Token: 0x04000F23 RID: 3875
	public bool debugMode = true;

	// Token: 0x04000F24 RID: 3876
	private Vector3 targetPos;

	// Token: 0x04000F25 RID: 3877
	private Vector3 dynamicPos;

	// Token: 0x04000F26 RID: 3878
	public Vector3 boneAxis = new Vector3(0f, 0f, 1f);

	// Token: 0x04000F27 RID: 3879
	public float targetDistance = 2f;

	// Token: 0x04000F28 RID: 3880
	public float bStiffness = 0.1f;

	// Token: 0x04000F29 RID: 3881
	public float bMass = 0.9f;

	// Token: 0x04000F2A RID: 3882
	public float bDamping = 0.75f;

	// Token: 0x04000F2B RID: 3883
	public float bGravity = 0.75f;

	// Token: 0x04000F2C RID: 3884
	private Vector3 force;

	// Token: 0x04000F2D RID: 3885
	private Vector3 acc;

	// Token: 0x04000F2E RID: 3886
	private Vector3 vel;

	// Token: 0x04000F2F RID: 3887
	public bool SquashAndStretch = true;

	// Token: 0x04000F30 RID: 3888
	public float sideStretch = 0.15f;

	// Token: 0x04000F31 RID: 3889
	public float frontStretch = 0.2f;

	// Token: 0x04000F32 RID: 3890
	public float disableDistance = 20f;
}
