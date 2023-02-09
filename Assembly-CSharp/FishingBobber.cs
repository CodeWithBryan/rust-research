using System;
using UnityEngine;

// Token: 0x020001A1 RID: 417
public class FishingBobber : BaseCombatEntity
{
	// Token: 0x170001CC RID: 460
	// (get) Token: 0x060017B9 RID: 6073 RVA: 0x000B07AA File Offset: 0x000AE9AA
	// (set) Token: 0x060017BA RID: 6074 RVA: 0x000B07B2 File Offset: 0x000AE9B2
	public float TireAmount { get; private set; }

	// Token: 0x060017BB RID: 6075 RVA: 0x000B07BB File Offset: 0x000AE9BB
	public override void ServerInit()
	{
		this.myRigidBody.centerOfMass = this.centerOfMass.localPosition;
		base.ServerInit();
	}

	// Token: 0x060017BC RID: 6076 RVA: 0x000B07DC File Offset: 0x000AE9DC
	public void InitialiseBobber(BasePlayer forPlayer, WaterBody forBody, Vector3 targetPos)
	{
		this.initialDirection = forPlayer.eyes.HeadForward().WithY(0f);
		this.spawnPosition = base.transform.position;
		this.initialTargetPosition = targetPos;
		this.initialCastTime = 0f;
		this.initialDistance = Vector3.Distance(targetPos, forPlayer.transform.position.WithY(targetPos.y));
		base.InvokeRepeating(new Action(this.ProcessInitialCast), 0f, 0f);
	}

	// Token: 0x060017BD RID: 6077 RVA: 0x000B086C File Offset: 0x000AEA6C
	private void ProcessInitialCast()
	{
		float num = 0.8f;
		if (this.initialCastTime > num)
		{
			base.transform.position = this.initialTargetPosition;
			base.CancelInvoke(new Action(this.ProcessInitialCast));
			return;
		}
		float t = this.initialCastTime / num;
		Vector3 vector = Vector3.Lerp(this.spawnPosition, this.initialTargetPosition, 0.5f);
		vector.y += 1.5f;
		Vector3 position = Vector3.Lerp(Vector3.Lerp(this.spawnPosition, vector, t), Vector3.Lerp(vector, this.initialTargetPosition, t), t);
		base.transform.position = position;
	}

	// Token: 0x060017BE RID: 6078 RVA: 0x000B0914 File Offset: 0x000AEB14
	public void ServerMovementUpdate(bool inputLeft, bool inputRight, bool inputBack, ref BaseFishingRod.FishState state, Vector3 playerPos, ItemModFishable fishableModifier)
	{
		Vector3 normalized = (playerPos - base.transform.position).normalized;
		Vector3 vector = Vector3.zero;
		this.bobberForcePingPong = Mathf.Clamp(Mathf.PingPong(Time.time, 2f), 0.2f, 2f);
		if (state.Contains(BaseFishingRod.FishState.PullingLeft))
		{
			vector = base.transform.right * (Time.deltaTime * this.HorizontalMoveSpeed * this.bobberForcePingPong * fishableModifier.MoveMultiplier * (inputRight ? 0.5f : 1f));
		}
		if (state.Contains(BaseFishingRod.FishState.PullingRight))
		{
			vector = -base.transform.right * (Time.deltaTime * this.HorizontalMoveSpeed * this.bobberForcePingPong * fishableModifier.MoveMultiplier * (inputLeft ? 0.5f : 1f));
		}
		if (state.Contains(BaseFishingRod.FishState.PullingBack))
		{
			vector += -base.transform.forward * (Time.deltaTime * this.PullAwayMoveSpeed * this.bobberForcePingPong * fishableModifier.MoveMultiplier * (inputBack ? 0.5f : 1f));
		}
		if (inputLeft || inputRight)
		{
			float num = 0.8f;
			if ((inputLeft && state == BaseFishingRod.FishState.PullingRight) || (inputRight && state == BaseFishingRod.FishState.PullingLeft))
			{
				num = 1.25f;
			}
			this.TireAmount += Time.deltaTime * num;
		}
		else
		{
			this.TireAmount -= Time.deltaTime * 0.1f;
		}
		if (inputLeft && !state.Contains(BaseFishingRod.FishState.PullingLeft))
		{
			vector += base.transform.right * (Time.deltaTime * this.SidewaysInputForce);
		}
		else if (inputRight && !state.Contains(BaseFishingRod.FishState.PullingRight))
		{
			vector += -base.transform.right * (Time.deltaTime * this.SidewaysInputForce);
		}
		if (inputBack)
		{
			float num2 = Mathx.RemapValClamped(this.TireAmount, 0f, 5f, 1f, 3f);
			vector += normalized * (this.ReelInMoveSpeed * fishableModifier.ReelInSpeedMultiplier * num2 * Time.deltaTime);
		}
		base.transform.LookAt(playerPos.WithY(base.transform.position.y));
		Vector3 vector2 = base.transform.position + vector;
		if (!this.IsDirectionValid(vector2, vector.magnitude, playerPos))
		{
			state = state.FlipHorizontal();
			return;
		}
		base.transform.position = vector2;
	}

	// Token: 0x060017BF RID: 6079 RVA: 0x000B0BB0 File Offset: 0x000AEDB0
	private bool IsDirectionValid(Vector3 pos, float checkLength, Vector3 playerPos)
	{
		if (Vector3.Angle((pos - playerPos).normalized.WithY(0f), this.initialDirection) > 60f)
		{
			return false;
		}
		Vector3 position = base.transform.position;
		RaycastHit raycastHit;
		return !GamePhysics.Trace(new Ray(position, (pos - position).normalized), 0.1f, out raycastHit, checkLength, 1218511105, QueryTriggerInteraction.UseGlobal, null);
	}

	// Token: 0x040010B6 RID: 4278
	public Transform centerOfMass;

	// Token: 0x040010B7 RID: 4279
	public Rigidbody myRigidBody;

	// Token: 0x040010B8 RID: 4280
	public Transform lineAttachPoint;

	// Token: 0x040010B9 RID: 4281
	public Transform bobberRoot;

	// Token: 0x040010BA RID: 4282
	public const BaseEntity.Flags CaughtFish = BaseEntity.Flags.Reserved1;

	// Token: 0x040010BB RID: 4283
	public float HorizontalMoveSpeed = 1f;

	// Token: 0x040010BC RID: 4284
	public float PullAwayMoveSpeed = 1f;

	// Token: 0x040010BD RID: 4285
	public float SidewaysInputForce = 1f;

	// Token: 0x040010BE RID: 4286
	public float ReelInMoveSpeed = 1f;

	// Token: 0x040010BF RID: 4287
	private float bobberForcePingPong;

	// Token: 0x040010C0 RID: 4288
	private Vector3 initialDirection;

	// Token: 0x040010C2 RID: 4290
	private Vector3 initialTargetPosition;

	// Token: 0x040010C3 RID: 4291
	private Vector3 spawnPosition;

	// Token: 0x040010C4 RID: 4292
	private TimeSince initialCastTime;

	// Token: 0x040010C5 RID: 4293
	private float initialDistance;
}
