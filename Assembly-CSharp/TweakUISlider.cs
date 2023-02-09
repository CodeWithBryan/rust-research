using System;
using TMPro;
using UnityEngine.UI;

// Token: 0x0200085B RID: 2139
public class TweakUISlider : TweakUIBase
{
	// Token: 0x06003512 RID: 13586 RVA: 0x0013FC37 File Offset: 0x0013DE37
	protected override void Init()
	{
		base.Init();
		this.ResetToConvar();
	}

	// Token: 0x06003513 RID: 13587 RVA: 0x0013F7A8 File Offset: 0x0013D9A8
	protected void OnEnable()
	{
		this.ResetToConvar();
	}

	// Token: 0x06003514 RID: 13588 RVA: 0x0013FC45 File Offset: 0x0013DE45
	public void OnChanged()
	{
		this.RefreshSliderDisplay(this.sliderControl.value);
		if (this.ApplyImmediatelyOnChange)
		{
			this.SetConvarValue();
		}
	}

	// Token: 0x06003515 RID: 13589 RVA: 0x0013FC68 File Offset: 0x0013DE68
	protected override void SetConvarValue()
	{
		base.SetConvarValue();
		if (this.conVar == null)
		{
			return;
		}
		float value = this.sliderControl.value;
		if (this.conVar.AsFloat == value)
		{
			return;
		}
		this.conVar.Set(value);
		this.RefreshSliderDisplay(this.conVar.AsFloat);
		TweakUISlider.lastConVarChanged = this.conVar.FullName;
		TweakUISlider.timeSinceLastConVarChange = 0f;
	}

	// Token: 0x06003516 RID: 13590 RVA: 0x0013FCDB File Offset: 0x0013DEDB
	public override void ResetToConvar()
	{
		base.ResetToConvar();
		if (this.conVar == null)
		{
			return;
		}
		this.RefreshSliderDisplay(this.conVar.AsFloat);
	}

	// Token: 0x06003517 RID: 13591 RVA: 0x0013FD00 File Offset: 0x0013DF00
	private void RefreshSliderDisplay(float value)
	{
		this.sliderControl.value = value;
		if (this.sliderControl.wholeNumbers)
		{
			this.textControl.text = this.sliderControl.value.ToString("N0");
			return;
		}
		this.textControl.text = this.sliderControl.value.ToString("0.0");
	}

	// Token: 0x04002F7E RID: 12158
	public Slider sliderControl;

	// Token: 0x04002F7F RID: 12159
	public TextMeshProUGUI textControl;

	// Token: 0x04002F80 RID: 12160
	public static string lastConVarChanged;

	// Token: 0x04002F81 RID: 12161
	public static TimeSince timeSinceLastConVarChange;
}
