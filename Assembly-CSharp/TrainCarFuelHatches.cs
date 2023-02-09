using System;
using UnityEngine;

// Token: 0x0200048C RID: 1164
public class TrainCarFuelHatches : MonoBehaviour
{
	// Token: 0x060025CD RID: 9677 RVA: 0x000EC3B8 File Offset: 0x000EA5B8
	public void LinedUpStateChanged(bool linedUp)
	{
		this.openingQueued = linedUp;
		if (!this.isMoving)
		{
			this.opening = linedUp;
			bool flag = this.opening;
			this.isMoving = true;
			InvokeHandler.InvokeRepeating(this, new Action(this.MoveTick), 0f, 0f);
		}
	}

	// Token: 0x060025CE RID: 9678 RVA: 0x000EC408 File Offset: 0x000EA608
	private void MoveTick()
	{
		if (this.opening)
		{
			this._hatchLerp += Time.deltaTime * this.animSpeed;
			if (this._hatchLerp >= 1f)
			{
				this.EndMove();
				return;
			}
			this.SetAngleOnAll(this._hatchLerp, false);
			return;
		}
		else
		{
			this._hatchLerp += Time.deltaTime * this.animSpeed;
			if (this._hatchLerp >= 1f)
			{
				this.EndMove();
				return;
			}
			this.SetAngleOnAll(this._hatchLerp, true);
			return;
		}
	}

	// Token: 0x060025CF RID: 9679 RVA: 0x000EC494 File Offset: 0x000EA694
	private void EndMove()
	{
		this._hatchLerp = 0f;
		if (this.openingQueued == this.opening)
		{
			InvokeHandler.CancelInvoke(this, new Action(this.MoveTick));
			this.isMoving = false;
			return;
		}
		this.opening = this.openingQueued;
	}

	// Token: 0x060025D0 RID: 9680 RVA: 0x000EC4E0 File Offset: 0x000EA6E0
	private void SetAngleOnAll(float lerpT, bool closing)
	{
		float angle;
		float angle2;
		float angle3;
		if (closing)
		{
			angle = LeanTween.easeOutBounce(-145f, 0f, Mathf.Clamp01(this._hatchLerp * 1.15f));
			angle2 = LeanTween.easeOutBounce(-145f, 0f, this._hatchLerp);
			angle3 = LeanTween.easeOutBounce(-145f, 0f, Mathf.Clamp01(this._hatchLerp * 1.25f));
		}
		else
		{
			angle = LeanTween.easeOutBounce(0f, -145f, Mathf.Clamp01(this._hatchLerp * 1.15f));
			angle2 = LeanTween.easeOutBounce(0f, -145f, this._hatchLerp);
			angle3 = LeanTween.easeOutBounce(0f, -145f, Mathf.Clamp01(this._hatchLerp * 1.25f));
		}
		this.SetAngle(this.hatch1Col, angle);
		this.SetAngle(this.hatch2Col, angle2);
		this.SetAngle(this.hatch3Col, angle3);
	}

	// Token: 0x060025D1 RID: 9681 RVA: 0x000EC5C9 File Offset: 0x000EA7C9
	private void SetAngle(Transform transform, float angle)
	{
		this._angles.x = angle;
		transform.localEulerAngles = this._angles;
	}

	// Token: 0x04001E9A RID: 7834
	[SerializeField]
	private TrainCar owner;

	// Token: 0x04001E9B RID: 7835
	[SerializeField]
	private float animSpeed = 1f;

	// Token: 0x04001E9C RID: 7836
	[SerializeField]
	private Transform hatch1Col;

	// Token: 0x04001E9D RID: 7837
	[SerializeField]
	private Transform hatch1Vis;

	// Token: 0x04001E9E RID: 7838
	[SerializeField]
	private Transform hatch2Col;

	// Token: 0x04001E9F RID: 7839
	[SerializeField]
	private Transform hatch2Vis;

	// Token: 0x04001EA0 RID: 7840
	[SerializeField]
	private Transform hatch3Col;

	// Token: 0x04001EA1 RID: 7841
	[SerializeField]
	private Transform hatch3Vis;

	// Token: 0x04001EA2 RID: 7842
	private const float closedXAngle = 0f;

	// Token: 0x04001EA3 RID: 7843
	private const float openXAngle = -145f;

	// Token: 0x04001EA4 RID: 7844
	[SerializeField]
	private SoundDefinition hatchOpenSoundDef;

	// Token: 0x04001EA5 RID: 7845
	[SerializeField]
	private SoundDefinition hatchCloseSoundDef;

	// Token: 0x04001EA6 RID: 7846
	private Vector3 _angles = Vector3.zero;

	// Token: 0x04001EA7 RID: 7847
	private float _hatchLerp;

	// Token: 0x04001EA8 RID: 7848
	private bool opening;

	// Token: 0x04001EA9 RID: 7849
	private bool openingQueued;

	// Token: 0x04001EAA RID: 7850
	private bool isMoving;
}
