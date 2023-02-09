using System;
using Network;
using Rust;
using UnityEngine;

// Token: 0x02000087 RID: 135
public class Kayak : BaseBoat, IPoolVehicle
{
	// Token: 0x06000CA9 RID: 3241 RVA: 0x0006BC08 File Offset: 0x00069E08
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Kayak.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x0006BC48 File Offset: 0x00069E48
	public override void ServerInit()
	{
		base.ServerInit();
		this.timeSinceLastUsed = 0f;
		base.InvokeRandomized(new Action(this.BoatDecay), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	// Token: 0x06000CAB RID: 3243 RVA: 0x0006BC98 File Offset: 0x00069E98
	public override void OnPlayerMounted()
	{
		base.OnPlayerMounted();
		if (!base.IsInvoking(new Action(this.TravelDistanceUpdate)) && GameInfo.HasAchievements)
		{
			int num = 0;
			foreach (BaseVehicle.MountPointInfo mountPointInfo in base.allMountPoints)
			{
				if (mountPointInfo.mountable != null && mountPointInfo.mountable.AnyMounted())
				{
					num++;
				}
			}
			if (num == 2)
			{
				this.lastTravelPos = base.transform.position.WithY(0f);
				base.InvokeRandomized(new Action(this.TravelDistanceUpdate), 5f, 5f, 3f);
			}
		}
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x0006BD70 File Offset: 0x00069F70
	public override void OnPlayerDismounted(BasePlayer player)
	{
		base.OnPlayerDismounted(player);
		if (base.IsInvoking(new Action(this.TravelDistanceUpdate)))
		{
			base.CancelInvoke(new Action(this.TravelDistanceUpdate));
		}
	}

	// Token: 0x06000CAD RID: 3245 RVA: 0x0006BDA0 File Offset: 0x00069FA0
	public override void DriverInput(InputState inputState, BasePlayer player)
	{
		this.timeSinceLastUsed = 0f;
		if (this.IsPlayerHoldingPaddle(player))
		{
			int playerSeat = base.GetPlayerSeat(player);
			if (this.playerPaddleCooldowns[playerSeat] > this.maxPaddleFrequency)
			{
				bool flag = inputState.IsDown(BUTTON.BACKWARD);
				bool flag2 = false;
				Vector3 a = base.transform.forward;
				if (flag)
				{
					a = -a;
				}
				float num = this.forwardPaddleForce;
				if (base.NumMounted() >= 2)
				{
					num *= this.multiDriverPaddleForceMultiplier;
				}
				if (inputState.IsDown(BUTTON.LEFT) || inputState.IsDown(BUTTON.FIRE_PRIMARY))
				{
					flag2 = true;
					this.rigidBody.AddForceAtPosition(a * num, this.GetPaddlePoint(playerSeat, Kayak.PaddleDirection.Left), ForceMode.Impulse);
					this.rigidBody.angularVelocity += -base.transform.up * this.rotatePaddleForce;
					base.ClientRPC<int, int>(null, "OnPaddled", flag ? 2 : 0, playerSeat);
				}
				else if (inputState.IsDown(BUTTON.RIGHT) || inputState.IsDown(BUTTON.FIRE_SECONDARY))
				{
					flag2 = true;
					this.rigidBody.AddForceAtPosition(a * num, this.GetPaddlePoint(playerSeat, Kayak.PaddleDirection.Right), ForceMode.Impulse);
					this.rigidBody.angularVelocity += base.transform.up * this.rotatePaddleForce;
					base.ClientRPC<int, int>(null, "OnPaddled", flag ? 3 : 1, playerSeat);
				}
				if (flag2)
				{
					this.playerPaddleCooldowns[playerSeat] = 0f;
					if (!flag)
					{
						Vector3 velocity = this.rigidBody.velocity;
						this.rigidBody.velocity = Vector3.Lerp(velocity, a * velocity.magnitude, 0.4f);
					}
				}
			}
		}
	}

	// Token: 0x06000CAE RID: 3246 RVA: 0x0006BF68 File Offset: 0x0006A168
	private void TravelDistanceUpdate()
	{
		Vector3 b = base.transform.position.WithY(0f);
		if (GameInfo.HasAchievements)
		{
			float num = Vector3.Distance(this.lastTravelPos, b) + this.distanceRemainder;
			float num2 = Mathf.Max(Mathf.Floor(num), 0f);
			this.distanceRemainder = num - num2;
			foreach (BaseVehicle.MountPointInfo mountPointInfo in base.allMountPoints)
			{
				if (mountPointInfo.mountable != null && mountPointInfo.mountable.AnyMounted() && (int)num2 > 0)
				{
					mountPointInfo.mountable.GetMounted().stats.Add("kayak_distance_travelled", (int)num2, global::Stats.Steam);
					mountPointInfo.mountable.GetMounted().stats.Save(true);
				}
			}
		}
		this.lastTravelPos = b;
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x00007074 File Offset: 0x00005274
	public override bool EngineOn()
	{
		return false;
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x0006C068 File Offset: 0x0006A268
	protected override void DoPushAction(BasePlayer player)
	{
		if (base.IsFlipped())
		{
			this.rigidBody.AddRelativeTorque(Vector3.forward * 8f, ForceMode.VelocityChange);
		}
		else
		{
			Vector3 vector = Vector3Ex.Direction2D(player.transform.position + player.eyes.BodyForward() * 3f, player.transform.position);
			vector = (Vector3.up * 0.1f + vector).normalized;
			Vector3 position = base.transform.position;
			float num = 5f;
			if (this.IsInWater())
			{
				num *= 0.75f;
			}
			this.rigidBody.AddForceAtPosition(vector * num, position, ForceMode.VelocityChange);
		}
		if (this.IsInWater())
		{
			if (this.pushWaterEffect.isValid)
			{
				Effect.server.Run(this.pushWaterEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
			}
		}
		else if (this.pushLandEffect.isValid)
		{
			Effect.server.Run(this.pushLandEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		}
		base.WakeUp();
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x0006C190 File Offset: 0x0006A390
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (this.fixedDragUpdate == null)
		{
			this.fixedDragUpdate = new TimeCachedValue<float>
			{
				refreshCooldown = 0.5f,
				refreshRandomRange = 0.2f,
				updateValue = new Func<float>(this.CalculateDesiredDrag)
			};
		}
		this.rigidBody.drag = this.fixedDragUpdate.Get(false);
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x0006C1F8 File Offset: 0x0006A3F8
	private float CalculateDesiredDrag()
	{
		int num = base.NumMounted();
		if (num == 0)
		{
			return 0.5f;
		}
		if (num < 2)
		{
			return 0.05f;
		}
		return 0.1f;
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x0006C224 File Offset: 0x0006A424
	private void BoatDecay()
	{
		BaseBoat.WaterVehicleDecay(this, 60f, this.timeSinceLastUsed, MotorRowboat.outsidedecayminutes, MotorRowboat.deepwaterdecayminutes);
	}

	// Token: 0x06000CB4 RID: 3252 RVA: 0x0006C246 File Offset: 0x0006A446
	public override bool CanPickup(BasePlayer player)
	{
		return !base.HasDriver() && base.CanPickup(player);
	}

	// Token: 0x06000CB5 RID: 3253 RVA: 0x0006C259 File Offset: 0x0006A459
	public bool IsPlayerHoldingPaddle(BasePlayer player)
	{
		return player.GetHeldEntity() != null && player.GetHeldEntity().GetItem().info == this.OarItem;
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x0006C288 File Offset: 0x0006A488
	private Vector3 GetPaddlePoint(int index, Kayak.PaddleDirection direction)
	{
		index = Mathf.Clamp(index, 0, this.mountPoints.Count);
		Vector3 pos = this.mountPoints[index].pos;
		if (direction == Kayak.PaddleDirection.Left)
		{
			pos.x -= 1f;
		}
		else if (direction == Kayak.PaddleDirection.Right)
		{
			pos.x += 1f;
		}
		pos.y -= 0.2f;
		return base.transform.TransformPoint(pos);
	}

	// Token: 0x06000CB7 RID: 3255 RVA: 0x0006C300 File Offset: 0x0006A500
	private bool IsInWater()
	{
		return base.isServer && this.buoyancy.timeOutOfWater < 0.1f;
	}

	// Token: 0x04000808 RID: 2056
	public ItemDefinition OarItem;

	// Token: 0x04000809 RID: 2057
	public float maxPaddleFrequency = 0.5f;

	// Token: 0x0400080A RID: 2058
	public float forwardPaddleForce = 5f;

	// Token: 0x0400080B RID: 2059
	public float multiDriverPaddleForceMultiplier = 0.75f;

	// Token: 0x0400080C RID: 2060
	public float rotatePaddleForce = 3f;

	// Token: 0x0400080D RID: 2061
	public GameObjectRef forwardSplashEffect;

	// Token: 0x0400080E RID: 2062
	public GameObjectRef backSplashEffect;

	// Token: 0x0400080F RID: 2063
	public ParticleSystem moveSplashEffect;

	// Token: 0x04000810 RID: 2064
	public float animationLerpSpeed = 6f;

	// Token: 0x04000811 RID: 2065
	[Header("Audio")]
	public BlendedSoundLoops waterLoops;

	// Token: 0x04000812 RID: 2066
	public float waterSoundSpeedDivisor = 10f;

	// Token: 0x04000813 RID: 2067
	public GameObjectRef pushLandEffect;

	// Token: 0x04000814 RID: 2068
	public GameObjectRef pushWaterEffect;

	// Token: 0x04000815 RID: 2069
	public PlayerModel.MountPoses noPaddlePose;

	// Token: 0x04000816 RID: 2070
	private TimeSince[] playerPaddleCooldowns = new TimeSince[2];

	// Token: 0x04000817 RID: 2071
	private TimeCachedValue<float> fixedDragUpdate;

	// Token: 0x04000818 RID: 2072
	private TimeSince timeSinceLastUsed;

	// Token: 0x04000819 RID: 2073
	private const float DECAY_TICK_TIME = 60f;

	// Token: 0x0400081A RID: 2074
	private Vector3 lastTravelPos;

	// Token: 0x0400081B RID: 2075
	private float distanceRemainder;

	// Token: 0x02000B94 RID: 2964
	private enum PaddleDirection
	{
		// Token: 0x04003ED2 RID: 16082
		Left,
		// Token: 0x04003ED3 RID: 16083
		Right,
		// Token: 0x04003ED4 RID: 16084
		LeftBack,
		// Token: 0x04003ED5 RID: 16085
		RightBack
	}
}
