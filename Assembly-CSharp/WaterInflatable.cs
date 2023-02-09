using System;
using System.Collections.Generic;
using Network;
using UnityEngine;

// Token: 0x020000E6 RID: 230
public class WaterInflatable : BaseMountable, IPoolVehicle, INotifyTrigger
{
	// Token: 0x0600140C RID: 5132 RVA: 0x0009E144 File Offset: 0x0009C344
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("WaterInflatable.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600140D RID: 5133 RVA: 0x0009E184 File Offset: 0x0009C384
	public override void ServerInit()
	{
		base.ServerInit();
		this.rigidBody.centerOfMass = this.centerOfMass.localPosition;
		this.prevSleeping = false;
	}

	// Token: 0x0600140E RID: 5134 RVA: 0x0009E1AC File Offset: 0x0009C3AC
	public override void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		if (deployedBy != null)
		{
			Vector3 estimatedVelocity = deployedBy.estimatedVelocity;
			float value = Vector3.Dot(base.transform.forward, estimatedVelocity.normalized);
			Vector3 vector = Vector3.Lerp(Vector3.zero, estimatedVelocity, Mathf.Clamp(value, 0f, 1f));
			vector *= this.inheritVelocityMultiplier;
			this.rigidBody.AddForce(vector, ForceMode.VelocityChange);
		}
	}

	// Token: 0x0600140F RID: 5135 RVA: 0x0009E220 File Offset: 0x0009C420
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		bool flag = this.rigidBody.IsSleeping();
		if (this.prevSleeping && !flag && this.buoyancy != null)
		{
			this.buoyancy.Wake();
		}
		this.prevSleeping = flag;
		if (this.rigidBody.velocity.magnitude > this.maxSpeed)
		{
			this.rigidBody.velocity = Vector3.ClampMagnitude(this.rigidBody.velocity, this.maxSpeed);
		}
		if (this.AnyMounted() && this.headSpaceCheckPosition != null)
		{
			Vector3 position = base.transform.position;
			if (this.forceClippingCheck || Vector3.Distance(position, this.lastClipCheckPosition) > this.headSpaceCheckRadius * 0.5f)
			{
				this.forceClippingCheck = false;
				this.lastClipCheckPosition = position;
				if (GamePhysics.CheckSphere(this.headSpaceCheckPosition.position, this.headSpaceCheckRadius, 1218511105, QueryTriggerInteraction.UseGlobal))
				{
					this.DismountAllPlayers();
				}
			}
		}
	}

	// Token: 0x06001410 RID: 5136 RVA: 0x0009E31C File Offset: 0x0009C51C
	public override void OnPlayerMounted()
	{
		base.OnPlayerMounted();
		this.lastPos = base.transform.position;
		this.forceClippingCheck = true;
	}

	// Token: 0x06001411 RID: 5137 RVA: 0x0009E33C File Offset: 0x0009C53C
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
		if (Vector3.Dot(base.transform.up, Vector3.up) < 0.1f)
		{
			this.DismountAllPlayers();
			return;
		}
		if (this.lastPaddle < this.maxPaddleFrequency)
		{
			return;
		}
		if (this.buoyancy != null && this.IsOutOfWaterServer())
		{
			return;
		}
		if (player.GetHeldEntity() == null)
		{
			if (inputState.IsDown(BUTTON.FORWARD))
			{
				if (this.rigidBody.velocity.magnitude < this.maxSpeed)
				{
					this.rigidBody.AddForce(base.transform.forward * this.forwardPushForce, ForceMode.Impulse);
				}
				this.rigidBody.angularVelocity = Vector3.Lerp(this.rigidBody.angularVelocity, base.transform.forward, 0.5f);
				this.lastPaddle = 0f;
				base.ClientRPC<int>(null, "OnPaddled", 0);
			}
			if (inputState.IsDown(BUTTON.BACKWARD))
			{
				this.rigidBody.AddForce(-base.transform.forward * this.rearPushForce, ForceMode.Impulse);
				this.rigidBody.angularVelocity = Vector3.Lerp(this.rigidBody.angularVelocity, -base.transform.forward, 0.5f);
				this.lastPaddle = 0f;
				base.ClientRPC<int>(null, "OnPaddled", 3);
			}
			if (inputState.IsDown(BUTTON.LEFT))
			{
				this.PaddleTurn(WaterInflatable.PaddleDirection.Left);
			}
			if (inputState.IsDown(BUTTON.RIGHT))
			{
				this.PaddleTurn(WaterInflatable.PaddleDirection.Right);
			}
		}
		if (this.inPoolCheck > 2f)
		{
			this.isInPool = base.IsInWaterVolume(base.transform.position);
			this.inPoolCheck = 0f;
		}
		if (this.additiveDownhillVelocity > 0f && !this.isInPool)
		{
			Vector3 vector = base.transform.TransformPoint(Vector3.forward);
			Vector3 position = base.transform.position;
			if (vector.y < position.y)
			{
				float num = this.additiveDownhillVelocity * (position.y - vector.y);
				this.rigidBody.AddForce(num * Time.fixedDeltaTime * base.transform.forward, ForceMode.Acceleration);
			}
			Vector3 velocity = this.rigidBody.velocity;
			this.rigidBody.velocity = Vector3.Lerp(velocity, base.transform.forward * velocity.magnitude, 0.4f);
		}
		if (this.driftTowardsIsland && this.landFacingCheck > 2f && !this.isInPool)
		{
			this.isFacingLand = false;
			this.landFacingCheck = 0f;
			Vector3 position2 = base.transform.position;
			if (!WaterResource.IsFreshWater(position2))
			{
				int num2 = 5;
				Vector3 forward = base.transform.forward;
				forward.y = 0f;
				for (int i = 1; i <= num2; i++)
				{
					int mask = 128;
					if (!TerrainMeta.TopologyMap.GetTopology(position2 + (float)i * 15f * forward, mask))
					{
						this.isFacingLand = true;
						break;
					}
				}
			}
		}
		if (this.driftTowardsIsland && this.isFacingLand && !this.isInPool)
		{
			this.landPushAcceleration = Mathf.Clamp(this.landPushAcceleration + Time.deltaTime, 0f, 3f);
			this.rigidBody.AddForce(base.transform.forward * (Time.deltaTime * this.landPushAcceleration), ForceMode.VelocityChange);
		}
		else
		{
			this.landPushAcceleration = 0f;
		}
		this.lastPos = base.transform.position;
	}

	// Token: 0x06001412 RID: 5138 RVA: 0x0009E70C File Offset: 0x0009C90C
	private void PaddleTurn(WaterInflatable.PaddleDirection direction)
	{
		if (direction == WaterInflatable.PaddleDirection.Forward || direction == WaterInflatable.PaddleDirection.Back)
		{
			return;
		}
		this.rigidBody.AddRelativeTorque(this.rotationForce * ((direction == WaterInflatable.PaddleDirection.Left) ? (-Vector3.up) : Vector3.up), ForceMode.Impulse);
		this.lastPaddle = 0f;
		base.ClientRPC<int>(null, "OnPaddled", (int)direction);
	}

	// Token: 0x06001413 RID: 5139 RVA: 0x00026FFC File Offset: 0x000251FC
	public override float WaterFactorForPlayer(BasePlayer player)
	{
		return 0f;
	}

	// Token: 0x06001414 RID: 5140 RVA: 0x0009E76C File Offset: 0x0009C96C
	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		BaseVehicle baseVehicle;
		if ((baseVehicle = (hitEntity as BaseVehicle)) != null && (baseVehicle.HasDriver() || baseVehicle.IsMoving() || baseVehicle.HasFlag(BaseEntity.Flags.On)))
		{
			base.Kill(BaseNetworkable.DestroyMode.Gib);
		}
	}

	// Token: 0x06001415 RID: 5141 RVA: 0x0009E7A3 File Offset: 0x0009C9A3
	private bool IsOutOfWaterServer()
	{
		return this.buoyancy.timeOutOfWater > 0.2f;
	}

	// Token: 0x06001416 RID: 5142 RVA: 0x00026D90 File Offset: 0x00024F90
	public void OnPoolDestroyed()
	{
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x06001417 RID: 5143 RVA: 0x0009E7B8 File Offset: 0x0009C9B8
	public void WakeUp()
	{
		if (this.rigidBody != null)
		{
			this.rigidBody.WakeUp();
			this.rigidBody.AddForce(Vector3.up * 0.1f, ForceMode.Impulse);
		}
		if (this.buoyancy != null)
		{
			this.buoyancy.Wake();
		}
	}

	// Token: 0x170001A8 RID: 424
	// (get) Token: 0x06001418 RID: 5144 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsSummerDlcVehicle
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001419 RID: 5145 RVA: 0x0009E814 File Offset: 0x0009CA14
	public void OnObjects(TriggerNotify trigger)
	{
		if (base.isClient)
		{
			return;
		}
		using (HashSet<BaseEntity>.Enumerator enumerator = trigger.entityContents.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				BaseVehicle baseVehicle;
				if ((baseVehicle = (enumerator.Current as BaseVehicle)) != null && (baseVehicle.HasDriver() || baseVehicle.IsMoving() || baseVehicle.HasFlag(BaseEntity.Flags.On)))
				{
					base.Kill(BaseNetworkable.DestroyMode.Gib);
					break;
				}
			}
		}
	}

	// Token: 0x0600141A RID: 5146 RVA: 0x000059DD File Offset: 0x00003BDD
	public void OnEmpty()
	{
	}

	// Token: 0x04000C8C RID: 3212
	public Rigidbody rigidBody;

	// Token: 0x04000C8D RID: 3213
	public Transform centerOfMass;

	// Token: 0x04000C8E RID: 3214
	public float forwardPushForce = 5f;

	// Token: 0x04000C8F RID: 3215
	public float rearPushForce = 5f;

	// Token: 0x04000C90 RID: 3216
	public float rotationForce = 5f;

	// Token: 0x04000C91 RID: 3217
	public float maxSpeed = 3f;

	// Token: 0x04000C92 RID: 3218
	public float maxPaddleFrequency = 0.5f;

	// Token: 0x04000C93 RID: 3219
	public SoundDefinition paddleSfx;

	// Token: 0x04000C94 RID: 3220
	public SoundDefinition smallPlayerMovementSound;

	// Token: 0x04000C95 RID: 3221
	public SoundDefinition largePlayerMovementSound;

	// Token: 0x04000C96 RID: 3222
	public BlendedSoundLoops waterLoops;

	// Token: 0x04000C97 RID: 3223
	public float waterSoundSpeedDivisor = 1f;

	// Token: 0x04000C98 RID: 3224
	public float additiveDownhillVelocity;

	// Token: 0x04000C99 RID: 3225
	public GameObjectRef handSplashForwardEffect;

	// Token: 0x04000C9A RID: 3226
	public GameObjectRef handSplashBackEffect;

	// Token: 0x04000C9B RID: 3227
	public GameObjectRef footSplashEffect;

	// Token: 0x04000C9C RID: 3228
	public float animationLerpSpeed = 1f;

	// Token: 0x04000C9D RID: 3229
	public Transform smoothedEyePosition;

	// Token: 0x04000C9E RID: 3230
	public float smoothedEyeSpeed = 1f;

	// Token: 0x04000C9F RID: 3231
	public Buoyancy buoyancy;

	// Token: 0x04000CA0 RID: 3232
	public bool driftTowardsIsland;

	// Token: 0x04000CA1 RID: 3233
	public GameObjectRef mountEffect;

	// Token: 0x04000CA2 RID: 3234
	[Range(0f, 1f)]
	public float handSplashOffset = 1f;

	// Token: 0x04000CA3 RID: 3235
	public float velocitySplashMultiplier = 4f;

	// Token: 0x04000CA4 RID: 3236
	public Vector3 modifyEyeOffset = Vector3.zero;

	// Token: 0x04000CA5 RID: 3237
	[Range(0f, 1f)]
	public float inheritVelocityMultiplier;

	// Token: 0x04000CA6 RID: 3238
	private TimeSince lastPaddle;

	// Token: 0x04000CA7 RID: 3239
	public ParticleSystem[] movingParticleSystems;

	// Token: 0x04000CA8 RID: 3240
	public float movingParticlesThreshold = 0.0005f;

	// Token: 0x04000CA9 RID: 3241
	public Transform headSpaceCheckPosition;

	// Token: 0x04000CAA RID: 3242
	public float headSpaceCheckRadius = 0.4f;

	// Token: 0x04000CAB RID: 3243
	private TimeSince landFacingCheck;

	// Token: 0x04000CAC RID: 3244
	private bool isFacingLand;

	// Token: 0x04000CAD RID: 3245
	private float landPushAcceleration;

	// Token: 0x04000CAE RID: 3246
	private TimeSince inPoolCheck;

	// Token: 0x04000CAF RID: 3247
	private bool isInPool;

	// Token: 0x04000CB0 RID: 3248
	private Vector3 lastPos = Vector3.zero;

	// Token: 0x04000CB1 RID: 3249
	private Vector3 lastClipCheckPosition;

	// Token: 0x04000CB2 RID: 3250
	private bool forceClippingCheck;

	// Token: 0x04000CB3 RID: 3251
	private bool prevSleeping;

	// Token: 0x02000BCB RID: 3019
	private enum PaddleDirection
	{
		// Token: 0x04003FA3 RID: 16291
		Forward,
		// Token: 0x04003FA4 RID: 16292
		Left,
		// Token: 0x04003FA5 RID: 16293
		Right,
		// Token: 0x04003FA6 RID: 16294
		Back
	}
}
