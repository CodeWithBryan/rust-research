using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000456 RID: 1110
public class BaseHelicopterVehicle : BaseVehicle
{
	// Token: 0x06002465 RID: 9317 RVA: 0x000E5F9A File Offset: 0x000E419A
	public virtual float GetServiceCeiling()
	{
		return 1000f;
	}

	// Token: 0x06002466 RID: 9318 RVA: 0x000E2A59 File Offset: 0x000E0C59
	public override float MaxVelocity()
	{
		return 50f;
	}

	// Token: 0x06002467 RID: 9319 RVA: 0x000E5FA1 File Offset: 0x000E41A1
	public override void ServerInit()
	{
		base.ServerInit();
		this.rigidBody.centerOfMass = this.com.localPosition;
	}

	// Token: 0x06002468 RID: 9320 RVA: 0x000E5FBF File Offset: 0x000E41BF
	public float MouseToBinary(float amount)
	{
		return Mathf.Clamp(amount, -1f, 1f);
	}

	// Token: 0x06002469 RID: 9321 RVA: 0x000E5FD4 File Offset: 0x000E41D4
	public virtual void PilotInput(InputState inputState, BasePlayer player)
	{
		this.currentInputState.Reset();
		this.currentInputState.throttle = (inputState.IsDown(BUTTON.FORWARD) ? 1f : 0f);
		this.currentInputState.throttle -= ((inputState.IsDown(BUTTON.BACKWARD) || inputState.IsDown(BUTTON.DUCK)) ? 1f : 0f);
		this.currentInputState.pitch = inputState.current.mouseDelta.y;
		this.currentInputState.roll = -inputState.current.mouseDelta.x;
		this.currentInputState.yaw = (inputState.IsDown(BUTTON.RIGHT) ? 1f : 0f);
		this.currentInputState.yaw -= (inputState.IsDown(BUTTON.LEFT) ? 1f : 0f);
		this.currentInputState.pitch = this.MouseToBinary(this.currentInputState.pitch);
		this.currentInputState.roll = this.MouseToBinary(this.currentInputState.roll);
		this.lastPlayerInputTime = Time.time;
	}

	// Token: 0x0600246A RID: 9322 RVA: 0x000E60FF File Offset: 0x000E42FF
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (base.IsDriver(player))
		{
			this.PilotInput(inputState, player);
		}
	}

	// Token: 0x0600246B RID: 9323 RVA: 0x000E6114 File Offset: 0x000E4314
	public virtual void SetDefaultInputState()
	{
		this.currentInputState.Reset();
		if (base.HasDriver())
		{
			float num = Vector3.Dot(Vector3.up, base.transform.right);
			float num2 = Vector3.Dot(Vector3.up, base.transform.forward);
			this.currentInputState.roll = ((num < 0f) ? 1f : 0f);
			this.currentInputState.roll -= ((num > 0f) ? 1f : 0f);
			if (num2 < --0f)
			{
				this.currentInputState.pitch = -1f;
				return;
			}
			if (num2 > 0f)
			{
				this.currentInputState.pitch = 1f;
				return;
			}
		}
		else
		{
			this.currentInputState.throttle = -1f;
		}
	}

	// Token: 0x0600246C RID: 9324 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool IsEnginePowered()
	{
		return true;
	}

	// Token: 0x0600246D RID: 9325 RVA: 0x000E61EC File Offset: 0x000E43EC
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (Time.time > this.lastPlayerInputTime + 0.5f)
		{
			this.SetDefaultInputState();
		}
		base.EnableGlobalBroadcast(this.IsEngineOn());
		this.MovementUpdate();
		base.SetFlag(BaseEntity.Flags.Reserved6, TOD_Sky.Instance.IsNight, false, true);
		foreach (GameObject gameObject in this.killTriggers)
		{
			bool active = this.rigidBody.velocity.y < 0f;
			gameObject.SetActive(active);
		}
	}

	// Token: 0x0600246E RID: 9326 RVA: 0x000E6277 File Offset: 0x000E4477
	public override void LightToggle(BasePlayer player)
	{
		if (base.IsDriver(player))
		{
			base.SetFlag(BaseEntity.Flags.Reserved5, !base.HasFlag(BaseEntity.Flags.Reserved5), false, true);
		}
	}

	// Token: 0x0600246F RID: 9327 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool ShouldApplyHoverForce()
	{
		return true;
	}

	// Token: 0x06002470 RID: 9328 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool IsEngineOn()
	{
		return true;
	}

	// Token: 0x06002471 RID: 9329 RVA: 0x000E629D File Offset: 0x000E449D
	public void ClearDamageTorque()
	{
		this.SetDamageTorque(Vector3.zero);
	}

	// Token: 0x06002472 RID: 9330 RVA: 0x000E62AA File Offset: 0x000E44AA
	public void SetDamageTorque(Vector3 newTorque)
	{
		this.damageTorque = newTorque;
	}

	// Token: 0x06002473 RID: 9331 RVA: 0x000E62B3 File Offset: 0x000E44B3
	public void AddDamageTorque(Vector3 torqueToAdd)
	{
		this.damageTorque += torqueToAdd;
	}

	// Token: 0x06002474 RID: 9332 RVA: 0x000E62C8 File Offset: 0x000E44C8
	public virtual void MovementUpdate()
	{
		if (!this.IsEngineOn())
		{
			return;
		}
		BaseHelicopterVehicle.HelicopterInputState helicopterInputState = this.currentInputState;
		this.currentThrottle = Mathf.Lerp(this.currentThrottle, helicopterInputState.throttle, 2f * Time.fixedDeltaTime);
		this.currentThrottle = Mathf.Clamp(this.currentThrottle, -0.8f, 1f);
		if (helicopterInputState.pitch != 0f || helicopterInputState.roll != 0f || helicopterInputState.yaw != 0f)
		{
			this.rigidBody.AddRelativeTorque(new Vector3(helicopterInputState.pitch * this.torqueScale.x, helicopterInputState.yaw * this.torqueScale.y, helicopterInputState.roll * this.torqueScale.z), ForceMode.Force);
		}
		if (this.damageTorque != Vector3.zero)
		{
			this.rigidBody.AddRelativeTorque(new Vector3(this.damageTorque.x, this.damageTorque.y, this.damageTorque.z), ForceMode.Force);
		}
		this.avgThrust = Mathf.Lerp(this.avgThrust, this.engineThrustMax * this.currentThrottle, Time.fixedDeltaTime * this.thrustLerpSpeed);
		float value = Mathf.Clamp01(Vector3.Dot(base.transform.up, Vector3.up));
		float num = Mathf.InverseLerp(this.liftDotMax, 1f, value);
		float serviceCeiling = this.GetServiceCeiling();
		this.avgTerrainHeight = Mathf.Lerp(this.avgTerrainHeight, TerrainMeta.HeightMap.GetHeight(base.transform.position), Time.deltaTime);
		float num2 = 1f - Mathf.InverseLerp(this.avgTerrainHeight + serviceCeiling - 20f, this.avgTerrainHeight + serviceCeiling, base.transform.position.y);
		num *= num2;
		float d = 1f - Mathf.InverseLerp(this.altForceDotMin, 1f, value);
		Vector3 force = Vector3.up * this.engineThrustMax * this.liftFraction * this.currentThrottle * num;
		Vector3 force2 = (base.transform.up - Vector3.up).normalized * this.engineThrustMax * this.currentThrottle * d;
		if (this.ShouldApplyHoverForce())
		{
			float d2 = this.rigidBody.mass * -Physics.gravity.y;
			this.rigidBody.AddForce(base.transform.up * d2 * num * this.hoverForceScale, ForceMode.Force);
		}
		this.rigidBody.AddForce(force, ForceMode.Force);
		this.rigidBody.AddForce(force2, ForceMode.Force);
	}

	// Token: 0x06002475 RID: 9333 RVA: 0x000E6588 File Offset: 0x000E4788
	public void DelayedImpactDamage()
	{
		float explosionForceMultiplier = this.explosionForceMultiplier;
		this.explosionForceMultiplier = 0f;
		base.Hurt(this.pendingImpactDamage * this.MaxHealth(), DamageType.Explosion, this, false);
		this.pendingImpactDamage = 0f;
		this.explosionForceMultiplier = explosionForceMultiplier;
	}

	// Token: 0x06002476 RID: 9334 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool CollisionDamageEnabled()
	{
		return true;
	}

	// Token: 0x06002477 RID: 9335 RVA: 0x000E65D0 File Offset: 0x000E47D0
	public void ProcessCollision(Collision collision)
	{
		if (base.isClient)
		{
			return;
		}
		if (!this.CollisionDamageEnabled())
		{
			return;
		}
		if (Time.time < this.nextDamageTime)
		{
			return;
		}
		float magnitude = collision.relativeVelocity.magnitude;
		if (collision.gameObject && (1 << collision.collider.gameObject.layer & 1218543873) <= 0)
		{
			return;
		}
		float num = Mathf.InverseLerp(5f, 30f, magnitude);
		if (num > 0f)
		{
			this.pendingImpactDamage += Mathf.Max(num, 0.15f);
			if (Vector3.Dot(base.transform.up, Vector3.up) < 0.5f)
			{
				this.pendingImpactDamage *= 5f;
			}
			if (Time.time > this.nextEffectTime)
			{
				this.nextEffectTime = Time.time + 0.25f;
				if (this.impactEffectSmall.isValid)
				{
					Vector3 vector = collision.GetContact(0).point;
					vector += (base.transform.position - vector) * 0.25f;
					Effect.server.Run(this.impactEffectSmall.resourcePath, vector, base.transform.up, null, false);
				}
			}
			this.rigidBody.AddForceAtPosition(collision.GetContact(0).normal * (1f + 3f * num), collision.GetContact(0).point, ForceMode.VelocityChange);
			this.nextDamageTime = Time.time + 0.333f;
			base.Invoke(new Action(this.DelayedImpactDamage), 0.015f);
		}
	}

	// Token: 0x06002478 RID: 9336 RVA: 0x000E6782 File Offset: 0x000E4982
	private void OnCollisionEnter(Collision collision)
	{
		this.ProcessCollision(collision);
	}

	// Token: 0x06002479 RID: 9337 RVA: 0x000E678C File Offset: 0x000E498C
	public override void OnKilled(HitInfo info)
	{
		if (base.isClient)
		{
			base.OnKilled(info);
			return;
		}
		if (this.explosionEffect.isValid)
		{
			Effect.server.Run(this.explosionEffect.resourcePath, base.transform.position, Vector3.up, null, true);
		}
		Vector3 vector = this.rigidBody.velocity * 0.25f;
		List<ServerGib> list = null;
		if (this.serverGibs.isValid)
		{
			GameObject gibSource = this.serverGibs.Get().GetComponent<ServerGib>()._gibSource;
			list = ServerGib.CreateGibs(this.serverGibs.resourcePath, base.gameObject, gibSource, vector, 3f);
		}
		Vector3 vector2 = base.CenterPoint();
		if (this.fireBall.isValid && !base.InSafeZone())
		{
			for (int i = 0; i < 12; i++)
			{
				BaseEntity baseEntity = GameManager.server.CreateEntity(this.fireBall.resourcePath, vector2, base.transform.rotation, true);
				if (baseEntity)
				{
					float min = 3f;
					float max = 10f;
					Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
					onUnitSphere.Normalize();
					float num = UnityEngine.Random.Range(0.5f, 4f);
					RaycastHit raycastHit;
					bool flag = Physics.Raycast(vector2, onUnitSphere, out raycastHit, num, 1218652417);
					Vector3 vector3 = raycastHit.point;
					if (!flag)
					{
						vector3 = vector2 + onUnitSphere * num;
					}
					vector3 -= onUnitSphere * 0.5f;
					baseEntity.transform.position = vector3;
					Collider component = baseEntity.GetComponent<Collider>();
					baseEntity.Spawn();
					baseEntity.SetVelocity(vector + onUnitSphere * UnityEngine.Random.Range(min, max));
					if (list != null)
					{
						foreach (ServerGib serverGib in list)
						{
							Physics.IgnoreCollision(component, serverGib.GetCollider(), true);
						}
					}
				}
			}
		}
		base.OnKilled(info);
	}

	// Token: 0x04001D08 RID: 7432
	[Header("Helicopter")]
	public float engineThrustMax;

	// Token: 0x04001D09 RID: 7433
	public Vector3 torqueScale;

	// Token: 0x04001D0A RID: 7434
	public Transform com;

	// Token: 0x04001D0B RID: 7435
	public GameObject[] killTriggers;

	// Token: 0x04001D0C RID: 7436
	[Header("Effects")]
	public Transform[] GroundPoints;

	// Token: 0x04001D0D RID: 7437
	public Transform[] GroundEffects;

	// Token: 0x04001D0E RID: 7438
	public GameObjectRef serverGibs;

	// Token: 0x04001D0F RID: 7439
	public GameObjectRef explosionEffect;

	// Token: 0x04001D10 RID: 7440
	public GameObjectRef fireBall;

	// Token: 0x04001D11 RID: 7441
	public GameObjectRef impactEffectSmall;

	// Token: 0x04001D12 RID: 7442
	public GameObjectRef impactEffectLarge;

	// Token: 0x04001D13 RID: 7443
	[Header("Sounds")]
	public SoundDefinition flightEngineSoundDef;

	// Token: 0x04001D14 RID: 7444
	public SoundDefinition flightThwopsSoundDef;

	// Token: 0x04001D15 RID: 7445
	public float rotorGainModSmoothing = 0.25f;

	// Token: 0x04001D16 RID: 7446
	public float engineGainMin = 0.5f;

	// Token: 0x04001D17 RID: 7447
	public float engineGainMax = 1f;

	// Token: 0x04001D18 RID: 7448
	public float thwopGainMin = 0.5f;

	// Token: 0x04001D19 RID: 7449
	public float thwopGainMax = 1f;

	// Token: 0x04001D1A RID: 7450
	public float currentThrottle;

	// Token: 0x04001D1B RID: 7451
	public float avgThrust;

	// Token: 0x04001D1C RID: 7452
	public float liftDotMax = 0.75f;

	// Token: 0x04001D1D RID: 7453
	public float altForceDotMin = 0.85f;

	// Token: 0x04001D1E RID: 7454
	public float liftFraction = 0.25f;

	// Token: 0x04001D1F RID: 7455
	public float thrustLerpSpeed = 1f;

	// Token: 0x04001D20 RID: 7456
	private float avgTerrainHeight;

	// Token: 0x04001D21 RID: 7457
	public const BaseEntity.Flags Flag_InternalLights = BaseEntity.Flags.Reserved6;

	// Token: 0x04001D22 RID: 7458
	protected BaseHelicopterVehicle.HelicopterInputState currentInputState = new BaseHelicopterVehicle.HelicopterInputState();

	// Token: 0x04001D23 RID: 7459
	protected float lastPlayerInputTime;

	// Token: 0x04001D24 RID: 7460
	protected float hoverForceScale = 0.99f;

	// Token: 0x04001D25 RID: 7461
	protected Vector3 damageTorque;

	// Token: 0x04001D26 RID: 7462
	private float nextDamageTime;

	// Token: 0x04001D27 RID: 7463
	private float nextEffectTime;

	// Token: 0x04001D28 RID: 7464
	private float pendingImpactDamage;

	// Token: 0x02000CA7 RID: 3239
	public class HelicopterInputState
	{
		// Token: 0x06004D2D RID: 19757 RVA: 0x00197140 File Offset: 0x00195340
		public void Reset()
		{
			this.throttle = 0f;
			this.roll = 0f;
			this.yaw = 0f;
			this.pitch = 0f;
			this.groundControl = false;
		}

		// Token: 0x04004363 RID: 17251
		public float throttle;

		// Token: 0x04004364 RID: 17252
		public float roll;

		// Token: 0x04004365 RID: 17253
		public float yaw;

		// Token: 0x04004366 RID: 17254
		public float pitch;

		// Token: 0x04004367 RID: 17255
		public bool groundControl;
	}
}
