using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000F0 RID: 240
public class ZiplineMountable : BaseMountable
{
	// Token: 0x06001494 RID: 5268 RVA: 0x000A2CE4 File Offset: 0x000A0EE4
	private Vector3 ProcessBezierMovement(float distanceToTravel)
	{
		if (this.linePoints == null)
		{
			return Vector3.zero;
		}
		float num = 0f;
		for (int i = 0; i < this.linePoints.Count - 1; i++)
		{
			float num2 = Vector3.Distance(this.linePoints[i], this.linePoints[i + 1]);
			if (num + num2 > distanceToTravel)
			{
				float t = Mathf.Clamp((distanceToTravel - num) / num2, 0f, 1f);
				return Vector3.Lerp(this.linePoints[i], this.linePoints[i + 1], t);
			}
			num += num2;
		}
		return this.linePoints[this.linePoints.Count - 1];
	}

	// Token: 0x06001495 RID: 5269 RVA: 0x000A2D98 File Offset: 0x000A0F98
	private Vector3 GetLineEndPoint(bool applyDismountOffset = false)
	{
		if (applyDismountOffset && this.linePoints != null)
		{
			Vector3 normalized = (this.linePoints[this.linePoints.Count - 2] - this.linePoints[this.linePoints.Count - 1]).normalized;
			return this.linePoints[this.linePoints.Count - 1] + normalized * 1.5f;
		}
		List<Vector3> list = this.linePoints;
		if (list == null)
		{
			return Vector3.zero;
		}
		return list[this.linePoints.Count - 1];
	}

	// Token: 0x06001496 RID: 5270 RVA: 0x000A2E3C File Offset: 0x000A103C
	private Vector3 GetNextLinePoint(Transform forTransform)
	{
		Vector3 position = forTransform.position;
		Vector3 forward = forTransform.forward;
		for (int i = 1; i < this.linePoints.Count - 1; i++)
		{
			Vector3 normalized = (this.linePoints[i + 1] - position).normalized;
			Vector3 normalized2 = (this.linePoints[i - 1] - position).normalized;
			float num = Vector3.Dot(forward, normalized);
			float num2 = Vector3.Dot(forward, normalized2);
			if (num > 0f && num2 < 0f)
			{
				return this.linePoints[i + 1];
			}
		}
		return this.GetLineEndPoint(false);
	}

	// Token: 0x06001497 RID: 5271 RVA: 0x000A2EEA File Offset: 0x000A10EA
	public override void ResetState()
	{
		base.ResetState();
		this.additiveValue = 0f;
		this.currentTravelDistance = 0f;
		this.hasEnded = false;
		this.linePoints = null;
	}

	// Token: 0x06001498 RID: 5272 RVA: 0x000A2F16 File Offset: 0x000A1116
	public override float MaxVelocity()
	{
		return this.MoveSpeed + this.ForwardAdditive;
	}

	// Token: 0x06001499 RID: 5273 RVA: 0x000A2F28 File Offset: 0x000A1128
	public void SetDestination(List<Vector3> targetLinePoints, Vector3 lineStartPos, Quaternion lineStartRot)
	{
		this.linePoints = targetLinePoints;
		this.currentTravelDistance = 0f;
		this.mountTime = 0f;
		GamePhysics.OverlapSphere(base.transform.position, 6f, this.ignoreColliders, 1218511105, QueryTriggerInteraction.Ignore);
		this.startPosition = base.transform.position;
		this.startRotation = base.transform.rotation;
		this.endPosition = lineStartPos;
		this.endRotation = lineStartRot;
		this.elapsedMoveTime = 0f;
		this.isAnimatingIn = true;
		base.InvokeRepeating(new Action(this.MovePlayerToPosition), 0f, 0f);
		Analytics.Server.UsedZipline();
	}

	// Token: 0x0600149A RID: 5274 RVA: 0x000A2FDC File Offset: 0x000A11DC
	private void Update()
	{
		if (this.linePoints == null || base.isClient)
		{
			return;
		}
		if (this.isAnimatingIn)
		{
			return;
		}
		if (this.hasEnded)
		{
			return;
		}
		float num = (this.MoveSpeed + this.additiveValue * this.ForwardAdditive) * Mathf.Clamp(this.mountTime / this.SpeedUpTime, 0f, 1f) * UnityEngine.Time.smoothDeltaTime;
		this.currentTravelDistance += num;
		Vector3 vector = this.ProcessBezierMovement(this.currentTravelDistance);
		List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
		Vector3 position = vector.WithY(vector.y - this.ZipCollider.height * 0.6f);
		Vector3 position2 = vector;
		GamePhysics.CapsuleSweep(position, position2, this.ZipCollider.radius * 0.5f, base.transform.forward, num, list, 1218511105, QueryTriggerInteraction.Ignore);
		foreach (RaycastHit raycastHit in list)
		{
			if (!(raycastHit.collider == this.ZipCollider) && !this.ignoreColliders.Contains(raycastHit.collider) && !(raycastHit.collider.GetComponentInParent<PowerlineNode>() != null))
			{
				global::ZiplineMountable componentInParent = raycastHit.collider.GetComponentInParent<global::ZiplineMountable>();
				if (componentInParent != null)
				{
					componentInParent.EndZipline();
				}
				this.EndZipline();
				Facepunch.Pool.FreeList<RaycastHit>(ref list);
				return;
			}
		}
		Facepunch.Pool.FreeList<RaycastHit>(ref list);
		if (Vector3.Distance(vector, this.GetLineEndPoint(false)) < 0.1f)
		{
			base.transform.position = this.GetLineEndPoint(true);
			this.hasEnded = true;
			return;
		}
		Vector3 normalized = (vector - base.transform.position.WithY(vector.y)).normalized;
		base.transform.position = Vector3.Lerp(base.transform.position, vector, UnityEngine.Time.deltaTime * 12f);
		base.transform.forward = normalized;
	}

	// Token: 0x0600149B RID: 5275 RVA: 0x000A31F0 File Offset: 0x000A13F0
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
		if (this.linePoints == null)
		{
			return;
		}
		if (this.hasEnded)
		{
			this.EndZipline();
			return;
		}
		Vector3 position = base.transform.position;
		float num = (this.GetNextLinePoint(base.transform).y < position.y + 0.1f && inputState.IsDown(BUTTON.FORWARD)) ? 1f : 0f;
		this.additiveValue = Mathf.MoveTowards(this.additiveValue, num, (float)Server.tickrate * ((num > 0f) ? 4f : 2f));
		base.SetFlag(global::BaseEntity.Flags.Reserved1, this.additiveValue > 0.5f, false, true);
	}

	// Token: 0x0600149C RID: 5276 RVA: 0x000A32A7 File Offset: 0x000A14A7
	private void EndZipline()
	{
		this.DismountAllPlayers();
	}

	// Token: 0x0600149D RID: 5277 RVA: 0x000A32AF File Offset: 0x000A14AF
	public override void OnPlayerDismounted(global::BasePlayer player)
	{
		base.OnPlayerDismounted(player);
		if (!base.IsDestroyed)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x0600149E RID: 5278 RVA: 0x000A32C7 File Offset: 0x000A14C7
	public override bool ValidDismountPosition(global::BasePlayer player, Vector3 disPos)
	{
		this.ZipCollider.enabled = false;
		bool result = base.ValidDismountPosition(player, disPos);
		this.ZipCollider.enabled = true;
		return result;
	}

	// Token: 0x0600149F RID: 5279 RVA: 0x000A32EC File Offset: 0x000A14EC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (this.linePoints == null)
		{
			return;
		}
		if (info.msg.ziplineMountable == null)
		{
			info.msg.ziplineMountable = Facepunch.Pool.Get<ProtoBuf.ZiplineMountable>();
		}
		info.msg.ziplineMountable.linePoints = Facepunch.Pool.GetList<VectorData>();
		foreach (Vector3 v in this.linePoints)
		{
			info.msg.ziplineMountable.linePoints.Add(v);
		}
	}

	// Token: 0x060014A0 RID: 5280 RVA: 0x000A3398 File Offset: 0x000A1598
	private void MovePlayerToPosition()
	{
		this.elapsedMoveTime += UnityEngine.Time.deltaTime;
		float num = Mathf.Clamp(this.elapsedMoveTime / this.MountEaseInTime, 0f, 1f);
		Vector3 localPosition = Vector3.Lerp(this.startPosition, this.endPosition, this.MountPositionCurve.Evaluate(num));
		Quaternion localRotation = Quaternion.Lerp(this.startRotation, this.endRotation, this.MountRotationCurve.Evaluate(num));
		base.transform.localPosition = localPosition;
		base.transform.localRotation = localRotation;
		if (num >= 1f)
		{
			this.isAnimatingIn = false;
			base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
			this.mountTime = 0f;
			base.CancelInvoke(new Action(this.MovePlayerToPosition));
		}
	}

	// Token: 0x060014A1 RID: 5281 RVA: 0x000A3468 File Offset: 0x000A1668
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && old.HasFlag(global::BaseEntity.Flags.Busy) && !next.HasFlag(global::BaseEntity.Flags.Busy) && !base.IsDestroyed)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x04000D16 RID: 3350
	public float MoveSpeed = 4f;

	// Token: 0x04000D17 RID: 3351
	public float ForwardAdditive = 5f;

	// Token: 0x04000D18 RID: 3352
	public CapsuleCollider ZipCollider;

	// Token: 0x04000D19 RID: 3353
	public Transform ZiplineGrabRoot;

	// Token: 0x04000D1A RID: 3354
	public Transform LeftHandIkPoint;

	// Token: 0x04000D1B RID: 3355
	public Transform RightHandIkPoint;

	// Token: 0x04000D1C RID: 3356
	public float SpeedUpTime = 0.6f;

	// Token: 0x04000D1D RID: 3357
	public bool EditorHoldInPlace;

	// Token: 0x04000D1E RID: 3358
	private List<Vector3> linePoints;

	// Token: 0x04000D1F RID: 3359
	private const global::BaseEntity.Flags PushForward = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000D20 RID: 3360
	public AnimationCurve MountPositionCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000D21 RID: 3361
	public AnimationCurve MountRotationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000D22 RID: 3362
	public float MountEaseInTime = 0.5f;

	// Token: 0x04000D23 RID: 3363
	private const global::BaseEntity.Flags ShowHandle = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000D24 RID: 3364
	private float additiveValue;

	// Token: 0x04000D25 RID: 3365
	private float currentTravelDistance;

	// Token: 0x04000D26 RID: 3366
	private TimeSince mountTime;

	// Token: 0x04000D27 RID: 3367
	private bool hasEnded;

	// Token: 0x04000D28 RID: 3368
	private List<Collider> ignoreColliders = new List<Collider>();

	// Token: 0x04000D29 RID: 3369
	private Vector3 startPosition = Vector3.zero;

	// Token: 0x04000D2A RID: 3370
	private Vector3 endPosition = Vector3.zero;

	// Token: 0x04000D2B RID: 3371
	private Quaternion startRotation = Quaternion.identity;

	// Token: 0x04000D2C RID: 3372
	private Quaternion endRotation = Quaternion.identity;

	// Token: 0x04000D2D RID: 3373
	private float elapsedMoveTime;

	// Token: 0x04000D2E RID: 3374
	private bool isAnimatingIn;
}
