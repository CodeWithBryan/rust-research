using System;
using UnityEngine;

// Token: 0x02000195 RID: 405
public class CH47FlightTest : MonoBehaviour
{
	// Token: 0x0600177B RID: 6011 RVA: 0x000AF34C File Offset: 0x000AD54C
	public void Awake()
	{
		this.rigidBody.centerOfMass = this.com.localPosition;
	}

	// Token: 0x0600177C RID: 6012 RVA: 0x000AF364 File Offset: 0x000AD564
	public CH47FlightTest.HelicopterInputState_t GetHelicopterInputState()
	{
		CH47FlightTest.HelicopterInputState_t helicopterInputState_t = default(CH47FlightTest.HelicopterInputState_t);
		helicopterInputState_t.throttle = (Input.GetKey(KeyCode.W) ? 1f : 0f);
		helicopterInputState_t.throttle -= (Input.GetKey(KeyCode.S) ? 1f : 0f);
		helicopterInputState_t.pitch = Input.GetAxis("Mouse Y");
		helicopterInputState_t.roll = -Input.GetAxis("Mouse X");
		helicopterInputState_t.yaw = (Input.GetKey(KeyCode.D) ? 1f : 0f);
		helicopterInputState_t.yaw -= (Input.GetKey(KeyCode.A) ? 1f : 0f);
		helicopterInputState_t.pitch = (float)Mathf.RoundToInt(helicopterInputState_t.pitch);
		helicopterInputState_t.roll = (float)Mathf.RoundToInt(helicopterInputState_t.roll);
		return helicopterInputState_t;
	}

	// Token: 0x0600177D RID: 6013 RVA: 0x000AF43C File Offset: 0x000AD63C
	public CH47FlightTest.HelicopterInputState_t GetAIInputState()
	{
		CH47FlightTest.HelicopterInputState_t result = default(CH47FlightTest.HelicopterInputState_t);
		Vector3 vector = Vector3.Cross(Vector3.up, base.transform.right);
		float num = Vector3.Dot(Vector3.Cross(Vector3.up, vector), Vector3Ex.Direction2D(this.AIMoveTarget.position, base.transform.position));
		result.yaw = ((num < 0f) ? 1f : 0f);
		result.yaw -= ((num > 0f) ? 1f : 0f);
		float num2 = Vector3.Dot(Vector3.up, base.transform.right);
		result.roll = ((num2 < 0f) ? 1f : 0f);
		result.roll -= ((num2 > 0f) ? 1f : 0f);
		float num3 = Vector3Ex.Distance2D(base.transform.position, this.AIMoveTarget.position);
		float num4 = Vector3.Dot(vector, Vector3Ex.Direction2D(this.AIMoveTarget.position, base.transform.position));
		float num5 = Vector3.Dot(Vector3.up, base.transform.forward);
		if (num3 > 10f)
		{
			result.pitch = ((num4 > 0.8f) ? -0.25f : 0f);
			result.pitch -= ((num4 < -0.8f) ? -0.25f : 0f);
			if (num5 < -0.35f)
			{
				result.pitch = -1f;
			}
			else if (num5 > 0.35f)
			{
				result.pitch = 1f;
			}
		}
		else if (num5 < --0f)
		{
			result.pitch = -1f;
		}
		else if (num5 > 0f)
		{
			result.pitch = 1f;
		}
		float idealAltitude = this.GetIdealAltitude();
		float y = base.transform.position.y;
		float num6;
		if (y > idealAltitude + CH47FlightTest.altitudeTolerance)
		{
			num6 = -1f;
		}
		else if (y < idealAltitude - CH47FlightTest.altitudeTolerance)
		{
			num6 = 1f;
		}
		else if (num3 > 20f)
		{
			num6 = Mathf.Lerp(0f, 1f, num3 / 20f);
		}
		else
		{
			num6 = 0f;
		}
		Debug.Log("desiredThrottle : " + num6);
		result.throttle = num6 * 1f;
		return result;
	}

	// Token: 0x0600177E RID: 6014 RVA: 0x000AF6AF File Offset: 0x000AD8AF
	public float GetIdealAltitude()
	{
		return this.AIMoveTarget.transform.position.y;
	}

	// Token: 0x0600177F RID: 6015 RVA: 0x000AF6C8 File Offset: 0x000AD8C8
	public void FixedUpdate()
	{
		CH47FlightTest.HelicopterInputState_t aiinputState = this.GetAIInputState();
		this.currentThrottle = Mathf.Lerp(this.currentThrottle, aiinputState.throttle, 2f * Time.fixedDeltaTime);
		this.currentThrottle = Mathf.Clamp(this.currentThrottle, -0.2f, 1f);
		this.rigidBody.AddRelativeTorque(new Vector3(aiinputState.pitch * this.torqueScale.x, aiinputState.yaw * this.torqueScale.y, aiinputState.roll * this.torqueScale.z) * Time.fixedDeltaTime, ForceMode.Force);
		this.avgThrust = Mathf.Lerp(this.avgThrust, this.engineThrustMax * this.currentThrottle, Time.fixedDeltaTime);
		float value = Mathf.Clamp01(Vector3.Dot(base.transform.up, Vector3.up));
		float num = Mathf.InverseLerp(this.liftDotMax, 1f, value);
		Vector3 force = Vector3.up * this.engineThrustMax * 0.5f * this.currentThrottle * num;
		Vector3 force2 = (base.transform.up - Vector3.up).normalized * this.engineThrustMax * this.currentThrottle * (1f - num);
		float d = this.rigidBody.mass * -Physics.gravity.y;
		this.rigidBody.AddForce(base.transform.up * d * num * 0.99f, ForceMode.Force);
		this.rigidBody.AddForce(force, ForceMode.Force);
		this.rigidBody.AddForce(force2, ForceMode.Force);
		for (int i = 0; i < this.GroundEffects.Length; i++)
		{
			Component component = this.GroundPoints[i];
			Transform transform = this.GroundEffects[i];
			RaycastHit raycastHit;
			if (Physics.Raycast(component.transform.position, Vector3.down, out raycastHit, 50f, 8388608))
			{
				transform.gameObject.SetActive(true);
				transform.transform.position = raycastHit.point + new Vector3(0f, 1f, 0f);
			}
			else
			{
				transform.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06001780 RID: 6016 RVA: 0x000AF928 File Offset: 0x000ADB28
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(this.AIMoveTarget.transform.position, 1f);
		Vector3 vector = Vector3.Cross(base.transform.right, Vector3.up);
		Vector3 a = Vector3.Cross(vector, Vector3.up);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(base.transform.position, base.transform.position + vector * 10f);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(base.transform.position, base.transform.position + a * 10f);
	}

	// Token: 0x04001066 RID: 4198
	public Rigidbody rigidBody;

	// Token: 0x04001067 RID: 4199
	public float engineThrustMax;

	// Token: 0x04001068 RID: 4200
	public Vector3 torqueScale;

	// Token: 0x04001069 RID: 4201
	public Transform com;

	// Token: 0x0400106A RID: 4202
	public Transform[] GroundPoints;

	// Token: 0x0400106B RID: 4203
	public Transform[] GroundEffects;

	// Token: 0x0400106C RID: 4204
	public float currentThrottle;

	// Token: 0x0400106D RID: 4205
	public float avgThrust;

	// Token: 0x0400106E RID: 4206
	public float liftDotMax = 0.75f;

	// Token: 0x0400106F RID: 4207
	public Transform AIMoveTarget;

	// Token: 0x04001070 RID: 4208
	private static float altitudeTolerance = 1f;

	// Token: 0x02000BEA RID: 3050
	public struct HelicopterInputState_t
	{
		// Token: 0x0400402C RID: 16428
		public float throttle;

		// Token: 0x0400402D RID: 16429
		public float roll;

		// Token: 0x0400402E RID: 16430
		public float yaw;

		// Token: 0x0400402F RID: 16431
		public float pitch;
	}
}
