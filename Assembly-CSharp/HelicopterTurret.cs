using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;

// Token: 0x020003F9 RID: 1017
public class HelicopterTurret : MonoBehaviour
{
	// Token: 0x06002219 RID: 8729 RVA: 0x000DA6DB File Offset: 0x000D88DB
	public void SetTarget(BaseCombatEntity newTarget)
	{
		this._target = newTarget;
		this.UpdateTargetVisibility();
	}

	// Token: 0x0600221A RID: 8730 RVA: 0x000DA6EA File Offset: 0x000D88EA
	public bool NeedsNewTarget()
	{
		return !this.HasTarget() || (!this.targetVisible && this.TimeSinceTargetLastSeen() > this.loseTargetAfter);
	}

	// Token: 0x0600221B RID: 8731 RVA: 0x000DA710 File Offset: 0x000D8910
	public bool UpdateTargetFromList(List<PatrolHelicopterAI.targetinfo> newTargetList)
	{
		int num = UnityEngine.Random.Range(0, newTargetList.Count);
		int i = newTargetList.Count;
		while (i >= 0)
		{
			i--;
			PatrolHelicopterAI.targetinfo targetinfo = newTargetList[num];
			if (targetinfo != null && targetinfo.ent != null && targetinfo.IsVisible() && this.InFiringArc(targetinfo.ply))
			{
				this.SetTarget(targetinfo.ply);
				return true;
			}
			num++;
			if (num >= newTargetList.Count)
			{
				num = 0;
			}
		}
		return false;
	}

	// Token: 0x0600221C RID: 8732 RVA: 0x000DA788 File Offset: 0x000D8988
	public bool TargetVisible()
	{
		this.UpdateTargetVisibility();
		return this.targetVisible;
	}

	// Token: 0x0600221D RID: 8733 RVA: 0x000DA796 File Offset: 0x000D8996
	public float TimeSinceTargetLastSeen()
	{
		return UnityEngine.Time.realtimeSinceStartup - this.lastSeenTargetTime;
	}

	// Token: 0x0600221E RID: 8734 RVA: 0x000DA7A4 File Offset: 0x000D89A4
	public bool HasTarget()
	{
		return this._target != null;
	}

	// Token: 0x0600221F RID: 8735 RVA: 0x000DA7B2 File Offset: 0x000D89B2
	public void ClearTarget()
	{
		this._target = null;
		this.targetVisible = false;
	}

	// Token: 0x06002220 RID: 8736 RVA: 0x000DA7C4 File Offset: 0x000D89C4
	public void TurretThink()
	{
		if (this.HasTarget() && this.TimeSinceTargetLastSeen() > this.loseTargetAfter * 2f)
		{
			this.ClearTarget();
		}
		if (!this.HasTarget())
		{
			return;
		}
		if (UnityEngine.Time.time - this.lastBurstTime > this.burstLength + this.timeBetweenBursts && this.TargetVisible())
		{
			this.lastBurstTime = UnityEngine.Time.time;
		}
		if (UnityEngine.Time.time < this.lastBurstTime + this.burstLength && UnityEngine.Time.time - this.lastFireTime >= this.fireRate && this.InFiringArc(this._target))
		{
			this.lastFireTime = UnityEngine.Time.time;
			this.FireGun();
		}
	}

	// Token: 0x06002221 RID: 8737 RVA: 0x000DA874 File Offset: 0x000D8A74
	public void FireGun()
	{
		this._heliAI.FireGun(this._target.transform.position + new Vector3(0f, 0.25f, 0f), PatrolHelicopter.bulletAccuracy, this.left);
	}

	// Token: 0x06002222 RID: 8738 RVA: 0x000DA8C0 File Offset: 0x000D8AC0
	public Vector3 GetPositionForEntity(BaseCombatEntity potentialtarget)
	{
		return potentialtarget.transform.position;
	}

	// Token: 0x06002223 RID: 8739 RVA: 0x000DA8D0 File Offset: 0x000D8AD0
	public float AngleToTarget(BaseCombatEntity potentialtarget)
	{
		Vector3 positionForEntity = this.GetPositionForEntity(potentialtarget);
		Vector3 position = this.muzzleTransform.position;
		Vector3 normalized = (positionForEntity - position).normalized;
		return Vector3.Angle(this.left ? (-this._heliAI.transform.right) : this._heliAI.transform.right, normalized);
	}

	// Token: 0x06002224 RID: 8740 RVA: 0x000DA934 File Offset: 0x000D8B34
	public bool InFiringArc(BaseCombatEntity potentialtarget)
	{
		return this.AngleToTarget(potentialtarget) < 80f;
	}

	// Token: 0x06002225 RID: 8741 RVA: 0x000DA944 File Offset: 0x000D8B44
	public void UpdateTargetVisibility()
	{
		if (!this.HasTarget())
		{
			return;
		}
		Vector3 position = this._target.transform.position;
		BasePlayer basePlayer = this._target as BasePlayer;
		if (basePlayer)
		{
			position = basePlayer.eyes.position;
		}
		bool flag = false;
		float num = Vector3.Distance(position, this.muzzleTransform.position);
		Vector3 normalized = (position - this.muzzleTransform.position).normalized;
		RaycastHit raycastHit;
		if (num < this.maxTargetRange && this.InFiringArc(this._target) && GamePhysics.Trace(new Ray(this.muzzleTransform.position + normalized * 6f, normalized), 0f, out raycastHit, num * 1.1f, 1218652417, QueryTriggerInteraction.UseGlobal, null) && raycastHit.collider.gameObject.ToBaseEntity() == this._target)
		{
			flag = true;
		}
		if (flag)
		{
			this.lastSeenTargetTime = UnityEngine.Time.realtimeSinceStartup;
		}
		this.targetVisible = flag;
	}

	// Token: 0x04001AA4 RID: 6820
	public PatrolHelicopterAI _heliAI;

	// Token: 0x04001AA5 RID: 6821
	public float fireRate = 0.125f;

	// Token: 0x04001AA6 RID: 6822
	public float burstLength = 3f;

	// Token: 0x04001AA7 RID: 6823
	public float timeBetweenBursts = 3f;

	// Token: 0x04001AA8 RID: 6824
	public float maxTargetRange = 300f;

	// Token: 0x04001AA9 RID: 6825
	public float loseTargetAfter = 5f;

	// Token: 0x04001AAA RID: 6826
	public Transform gun_yaw;

	// Token: 0x04001AAB RID: 6827
	public Transform gun_pitch;

	// Token: 0x04001AAC RID: 6828
	public Transform muzzleTransform;

	// Token: 0x04001AAD RID: 6829
	public bool left;

	// Token: 0x04001AAE RID: 6830
	public BaseCombatEntity _target;

	// Token: 0x04001AAF RID: 6831
	private float lastBurstTime = float.NegativeInfinity;

	// Token: 0x04001AB0 RID: 6832
	private float lastFireTime = float.NegativeInfinity;

	// Token: 0x04001AB1 RID: 6833
	private float lastSeenTargetTime = float.NegativeInfinity;

	// Token: 0x04001AB2 RID: 6834
	private bool targetVisible;
}
