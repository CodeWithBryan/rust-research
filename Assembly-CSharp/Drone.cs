using System;
using Network;
using UnityEngine;

// Token: 0x02000100 RID: 256
public class Drone : RemoteControlEntity
{
	// Token: 0x170001B0 RID: 432
	// (get) Token: 0x060014DD RID: 5341 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool RequiresMouse
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060014DE RID: 5342 RVA: 0x000A43CC File Offset: 0x000A25CC
	public override void UserInput(InputState inputState, BasePlayer player)
	{
		this.currentInput.Reset();
		int num = (inputState.IsDown(BUTTON.FORWARD) ? 1 : 0) + (inputState.IsDown(BUTTON.BACKWARD) ? -1 : 0);
		int num2 = (inputState.IsDown(BUTTON.RIGHT) ? 1 : 0) + (inputState.IsDown(BUTTON.LEFT) ? -1 : 0);
		this.currentInput.movement = new Vector3((float)num2, 0f, (float)num).normalized;
		this.currentInput.throttle = (float)((inputState.IsDown(BUTTON.SPRINT) ? 1 : 0) + (inputState.IsDown(BUTTON.DUCK) ? -1 : 0));
		this.currentInput.yaw = inputState.current.mouseDelta.x;
		this.currentInput.pitch = inputState.current.mouseDelta.y;
		this.lastInputTime = Time.time;
	}

	// Token: 0x060014DF RID: 5343 RVA: 0x000A44AC File Offset: 0x000A26AC
	public virtual void Update()
	{
		if (base.IsBeingControlled || this.targetPosition == null)
		{
			return;
		}
		Vector3 position = base.transform.position;
		float height = TerrainMeta.HeightMap.GetHeight(position);
		Vector3 vector = this.targetPosition.Value - this.body.velocity * 0.5f;
		if (this.keepAboveTerrain)
		{
			vector.y = Mathf.Max(vector.y, height + 1f);
		}
		Vector2 a = vector.XZ2D();
		Vector2 b = position.XZ2D();
		Vector3 vector2;
		float num;
		(a - b).XZ3D().ToDirectionAndMagnitude(out vector2, out num);
		this.currentInput.Reset();
		this.lastInputTime = Time.time;
		if (position.y - height > 1f)
		{
			float d = Mathf.Clamp01(num);
			this.currentInput.movement = base.transform.InverseTransformVector(vector2) * d;
			if (num > 0.5f)
			{
				float y = base.transform.rotation.eulerAngles.y;
				float y2 = Quaternion.FromToRotation(Vector3.forward, vector2).eulerAngles.y;
				this.currentInput.yaw = Mathf.Clamp(Mathf.LerpAngle(y, y2, Time.deltaTime) - y, -2f, 2f);
			}
		}
		this.currentInput.throttle = Mathf.Clamp(vector.y - position.y, -1f, 1f);
	}

	// Token: 0x060014E0 RID: 5344 RVA: 0x000A4634 File Offset: 0x000A2834
	public void FixedUpdate()
	{
		if (!base.isServer || this.IsDead())
		{
			return;
		}
		if (!base.IsBeingControlled && this.targetPosition == null)
		{
			return;
		}
		float num = this.WaterFactor();
		if (this.killInWater && num > 0f)
		{
			if (num > 0.99f)
			{
				base.Kill(BaseNetworkable.DestroyMode.None);
			}
			return;
		}
		double currentTimestamp = TimeEx.currentTimestamp;
		object obj = this.lastCollision > 0.0 && currentTimestamp - this.lastCollision < (double)this.collisionDisableTime;
		RaycastHit raycastHit;
		this.isGrounded = (this.enableGrounding && this.body.SweepTest(-base.transform.up, out raycastHit, this.groundTraceDist));
		Vector3 vector = base.transform.TransformDirection(this.currentInput.movement);
		Vector3 a;
		float num2;
		this.body.velocity.WithY(0f).ToDirectionAndMagnitude(out a, out num2);
		float num3 = Mathf.Clamp01(num2 / this.leanMaxVelocity);
		Vector3 a2 = Mathf.Approximately(vector.sqrMagnitude, 0f) ? (-num3 * a) : vector;
		Vector3 normalized = (Vector3.up + a2 * this.leanWeight * num3).normalized;
		Vector3 up = base.transform.up;
		float num4 = Mathf.Max(Vector3.Dot(normalized, up), 0f);
		object obj2 = obj;
		if (obj2 == null || this.isGrounded)
		{
			Vector3 a3 = (this.isGrounded && this.currentInput.throttle <= 0f) ? Vector3.zero : (-1f * base.transform.up * Physics.gravity.y);
			Vector3 b = this.isGrounded ? Vector3.zero : (vector * this.movementAcceleration);
			Vector3 b2 = base.transform.up * this.currentInput.throttle * this.altitudeAcceleration;
			Vector3 a4 = a3 + b + b2;
			this.body.AddForce(a4 * num4, ForceMode.Acceleration);
		}
		if (obj2 == null && !this.isGrounded)
		{
			Vector3 a5 = base.transform.TransformVector(0f, this.currentInput.yaw * this.yawSpeed, 0f);
			Vector3 a6 = Vector3.Cross(Quaternion.Euler(this.body.angularVelocity * this.uprightPrediction) * up, normalized) * this.uprightSpeed;
			float d = (num4 < this.uprightDot) ? 0f : num4;
			Vector3 a7 = a5 * num4 + a6 * d;
			this.body.AddTorque(a7 * num4, ForceMode.Acceleration);
		}
	}

	// Token: 0x060014E1 RID: 5345 RVA: 0x000A490C File Offset: 0x000A2B0C
	public void OnCollisionEnter(Collision collision)
	{
		if (base.isServer)
		{
			this.lastCollision = TimeEx.currentTimestamp;
			float magnitude = collision.relativeVelocity.magnitude;
			if (magnitude > this.hurtVelocityThreshold)
			{
				base.Hurt(Mathf.Pow(magnitude, this.hurtDamagePower));
			}
		}
	}

	// Token: 0x060014E2 RID: 5346 RVA: 0x000A4956 File Offset: 0x000A2B56
	public void OnCollisionStay()
	{
		if (base.isServer)
		{
			this.lastCollision = TimeEx.currentTimestamp;
		}
	}

	// Token: 0x060014E3 RID: 5347 RVA: 0x000058B6 File Offset: 0x00003AB6
	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	// Token: 0x170001B1 RID: 433
	// (get) Token: 0x060014E4 RID: 5348 RVA: 0x00003A54 File Offset: 0x00001C54
	protected override bool PositionTickFixedTime
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060014E5 RID: 5349 RVA: 0x000A496B File Offset: 0x000A2B6B
	public override Vector3 GetLocalVelocityServer()
	{
		if (this.body == null)
		{
			return Vector3.zero;
		}
		return this.body.velocity;
	}

	// Token: 0x04000D7E RID: 3454
	[Header("Drone")]
	public Rigidbody body;

	// Token: 0x04000D7F RID: 3455
	public bool killInWater = true;

	// Token: 0x04000D80 RID: 3456
	public bool enableGrounding = true;

	// Token: 0x04000D81 RID: 3457
	public bool keepAboveTerrain = true;

	// Token: 0x04000D82 RID: 3458
	public float groundTraceDist = 0.1f;

	// Token: 0x04000D83 RID: 3459
	public float altitudeAcceleration = 10f;

	// Token: 0x04000D84 RID: 3460
	public float movementAcceleration = 10f;

	// Token: 0x04000D85 RID: 3461
	public float yawSpeed = 2f;

	// Token: 0x04000D86 RID: 3462
	public float uprightSpeed = 2f;

	// Token: 0x04000D87 RID: 3463
	public float uprightPrediction = 0.15f;

	// Token: 0x04000D88 RID: 3464
	public float uprightDot = 0.5f;

	// Token: 0x04000D89 RID: 3465
	public float leanWeight = 0.1f;

	// Token: 0x04000D8A RID: 3466
	public float leanMaxVelocity = 5f;

	// Token: 0x04000D8B RID: 3467
	public float hurtVelocityThreshold = 3f;

	// Token: 0x04000D8C RID: 3468
	public float hurtDamagePower = 3f;

	// Token: 0x04000D8D RID: 3469
	public float collisionDisableTime = 0.25f;

	// Token: 0x04000D8E RID: 3470
	[Header("Sound")]
	public SoundDefinition movementLoopSoundDef;

	// Token: 0x04000D8F RID: 3471
	public SoundDefinition movementStartSoundDef;

	// Token: 0x04000D90 RID: 3472
	public SoundDefinition movementStopSoundDef;

	// Token: 0x04000D91 RID: 3473
	public AnimationCurve movementLoopPitchCurve;

	// Token: 0x04000D92 RID: 3474
	protected Vector3? targetPosition;

	// Token: 0x04000D93 RID: 3475
	private Drone.DroneInputState currentInput;

	// Token: 0x04000D94 RID: 3476
	private float lastInputTime;

	// Token: 0x04000D95 RID: 3477
	private double lastCollision = -1000.0;

	// Token: 0x04000D96 RID: 3478
	private bool isGrounded;

	// Token: 0x02000BD1 RID: 3025
	private struct DroneInputState
	{
		// Token: 0x06004B49 RID: 19273 RVA: 0x00191B92 File Offset: 0x0018FD92
		public void Reset()
		{
			this.movement = Vector3.zero;
			this.pitch = 0f;
			this.yaw = 0f;
		}

		// Token: 0x04003FC7 RID: 16327
		public Vector3 movement;

		// Token: 0x04003FC8 RID: 16328
		public float throttle;

		// Token: 0x04003FC9 RID: 16329
		public float pitch;

		// Token: 0x04003FCA RID: 16330
		public float yaw;
	}
}
