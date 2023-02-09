using System;
using UnityEngine;

// Token: 0x020003F7 RID: 1015
public class AIHelicopterAnimation : MonoBehaviour
{
	// Token: 0x0600220F RID: 8719 RVA: 0x000DA298 File Offset: 0x000D8498
	public void Awake()
	{
		this.lastPosition = base.transform.position;
	}

	// Token: 0x06002210 RID: 8720 RVA: 0x000DA2AB File Offset: 0x000D84AB
	public Vector3 GetMoveDirection()
	{
		return this._ai.GetMoveDirection();
	}

	// Token: 0x06002211 RID: 8721 RVA: 0x000DA2B8 File Offset: 0x000D84B8
	public float GetMoveSpeed()
	{
		return this._ai.GetMoveSpeed();
	}

	// Token: 0x06002212 RID: 8722 RVA: 0x000DA2C8 File Offset: 0x000D84C8
	public void Update()
	{
		this.lastPosition = base.transform.position;
		Vector3 moveDirection = this.GetMoveDirection();
		float moveSpeed = this.GetMoveSpeed();
		float num = 0.25f + Mathf.Clamp01(moveSpeed / this._ai.maxSpeed) * 0.75f;
		this.smoothRateOfChange = Mathf.Lerp(this.smoothRateOfChange, moveSpeed - this.oldMoveSpeed, Time.deltaTime * 5f);
		this.oldMoveSpeed = moveSpeed;
		float num2 = Vector3.Angle(moveDirection, base.transform.forward);
		float num3 = Vector3.Angle(moveDirection, -base.transform.forward);
		float num4 = 1f - Mathf.Clamp01(num2 / this.degreeMax);
		float num5 = 1f - Mathf.Clamp01(num3 / this.degreeMax);
		float b = (num4 - num5) * num;
		float num6 = Mathf.Lerp(this.lastForwardBackScalar, b, Time.deltaTime * 2f);
		this.lastForwardBackScalar = num6;
		float num7 = Vector3.Angle(moveDirection, base.transform.right);
		float num8 = Vector3.Angle(moveDirection, -base.transform.right);
		float num9 = 1f - Mathf.Clamp01(num7 / this.degreeMax);
		float num10 = 1f - Mathf.Clamp01(num8 / this.degreeMax);
		float b2 = (num9 - num10) * num;
		float num11 = Mathf.Lerp(this.lastStrafeScalar, b2, Time.deltaTime * 2f);
		this.lastStrafeScalar = num11;
		Vector3 zero = Vector3.zero;
		zero.x += num6 * this.swayAmount;
		zero.z -= num11 * this.swayAmount;
		Quaternion localRotation = Quaternion.identity;
		localRotation = Quaternion.Euler(zero.x, zero.y, zero.z);
		this._ai.helicopterBase.rotorPivot.transform.localRotation = localRotation;
	}

	// Token: 0x04001A95 RID: 6805
	public PatrolHelicopterAI _ai;

	// Token: 0x04001A96 RID: 6806
	public float swayAmount = 1f;

	// Token: 0x04001A97 RID: 6807
	public float lastStrafeScalar;

	// Token: 0x04001A98 RID: 6808
	public float lastForwardBackScalar;

	// Token: 0x04001A99 RID: 6809
	public float degreeMax = 90f;

	// Token: 0x04001A9A RID: 6810
	public Vector3 lastPosition = Vector3.zero;

	// Token: 0x04001A9B RID: 6811
	public float oldMoveSpeed;

	// Token: 0x04001A9C RID: 6812
	public float smoothRateOfChange;

	// Token: 0x04001A9D RID: 6813
	public float flareAmount;
}
