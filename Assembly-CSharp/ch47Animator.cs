using System;
using UnityEngine;

// Token: 0x02000196 RID: 406
public class ch47Animator : MonoBehaviour
{
	// Token: 0x06001783 RID: 6019 RVA: 0x000AFA04 File Offset: 0x000ADC04
	private void Start()
	{
		this.EnableBlurredRotorBlades(false);
		this.animator.SetBool("rotorblade_stop", false);
	}

	// Token: 0x06001784 RID: 6020 RVA: 0x000AFA1E File Offset: 0x000ADC1E
	public void SetDropDoorOpen(bool isOpen)
	{
		this.bottomDoorOpen = isOpen;
	}

	// Token: 0x06001785 RID: 6021 RVA: 0x000AFA28 File Offset: 0x000ADC28
	private void Update()
	{
		this.animator.SetBool("bottomdoor", this.bottomDoorOpen);
		this.animator.SetBool("landinggear", this.landingGearDown);
		this.animator.SetBool("leftdoor", this.leftDoorOpen);
		this.animator.SetBool("rightdoor", this.rightDoorOpen);
		this.animator.SetBool("reardoor", this.rearDoorOpen);
		this.animator.SetBool("reardoor_extension", this.rearDoorExtensionOpen);
		if (this.rotorBladeSpeed >= this.blurSpeedThreshold && !this.blurredRotorBladesEnabled)
		{
			this.EnableBlurredRotorBlades(true);
		}
		else if (this.rotorBladeSpeed < this.blurSpeedThreshold && this.blurredRotorBladesEnabled)
		{
			this.EnableBlurredRotorBlades(false);
		}
		if (this.rotorBladeSpeed <= 0f)
		{
			this.animator.SetBool("rotorblade_stop", true);
			return;
		}
		this.animator.SetBool("rotorblade_stop", false);
	}

	// Token: 0x06001786 RID: 6022 RVA: 0x000AFB28 File Offset: 0x000ADD28
	private void LateUpdate()
	{
		float num = Time.deltaTime * this.rotorBladeSpeed * 15f;
		Vector3 localEulerAngles = this.frontRotorBlade.localEulerAngles;
		this.frontRotorBlade.localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y + num, localEulerAngles.z);
		localEulerAngles = this.rearRotorBlade.localEulerAngles;
		this.rearRotorBlade.localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y - num, localEulerAngles.z);
	}

	// Token: 0x06001787 RID: 6023 RVA: 0x000AFBA8 File Offset: 0x000ADDA8
	private void EnableBlurredRotorBlades(bool enabled)
	{
		this.blurredRotorBladesEnabled = enabled;
		SkinnedMeshRenderer[] rotorBlades = this.blurredRotorBlades;
		for (int i = 0; i < rotorBlades.Length; i++)
		{
			rotorBlades[i].enabled = enabled;
		}
		rotorBlades = this.RotorBlades;
		for (int i = 0; i < rotorBlades.Length; i++)
		{
			rotorBlades[i].enabled = !enabled;
		}
	}

	// Token: 0x04001071 RID: 4209
	public Animator animator;

	// Token: 0x04001072 RID: 4210
	public bool bottomDoorOpen;

	// Token: 0x04001073 RID: 4211
	public bool landingGearDown;

	// Token: 0x04001074 RID: 4212
	public bool leftDoorOpen;

	// Token: 0x04001075 RID: 4213
	public bool rightDoorOpen;

	// Token: 0x04001076 RID: 4214
	public bool rearDoorOpen;

	// Token: 0x04001077 RID: 4215
	public bool rearDoorExtensionOpen;

	// Token: 0x04001078 RID: 4216
	public Transform rearRotorBlade;

	// Token: 0x04001079 RID: 4217
	public Transform frontRotorBlade;

	// Token: 0x0400107A RID: 4218
	public float rotorBladeSpeed;

	// Token: 0x0400107B RID: 4219
	public float wheelTurnSpeed;

	// Token: 0x0400107C RID: 4220
	public float wheelTurnAngle;

	// Token: 0x0400107D RID: 4221
	public SkinnedMeshRenderer[] blurredRotorBlades;

	// Token: 0x0400107E RID: 4222
	public SkinnedMeshRenderer[] RotorBlades;

	// Token: 0x0400107F RID: 4223
	private bool blurredRotorBladesEnabled;

	// Token: 0x04001080 RID: 4224
	public float blurSpeedThreshold = 100f;
}
