using System;
using UnityEngine.UI;

// Token: 0x0200085C RID: 2140
public class TweakUIToggle : TweakUIBase
{
	// Token: 0x06003519 RID: 13593 RVA: 0x0013FC37 File Offset: 0x0013DE37
	protected override void Init()
	{
		base.Init();
		this.ResetToConvar();
	}

	// Token: 0x0600351A RID: 13594 RVA: 0x0013F7A8 File Offset: 0x0013D9A8
	protected void OnEnable()
	{
		this.ResetToConvar();
	}

	// Token: 0x0600351B RID: 13595 RVA: 0x0013F8F4 File Offset: 0x0013DAF4
	public void OnToggleChanged()
	{
		if (this.ApplyImmediatelyOnChange)
		{
			this.SetConvarValue();
		}
	}

	// Token: 0x0600351C RID: 13596 RVA: 0x0013FD70 File Offset: 0x0013DF70
	protected override void SetConvarValue()
	{
		base.SetConvarValue();
		if (this.conVar == null)
		{
			return;
		}
		bool flag = this.toggleControl.isOn;
		if (this.inverse)
		{
			flag = !flag;
		}
		if (this.conVar.AsBool == flag)
		{
			return;
		}
		TweakUIToggle.lastConVarChanged = this.conVar.FullName;
		TweakUIToggle.timeSinceLastConVarChange = 0f;
		this.conVar.Set(flag);
	}

	// Token: 0x0600351D RID: 13597 RVA: 0x0013FDE0 File Offset: 0x0013DFE0
	public override void ResetToConvar()
	{
		base.ResetToConvar();
		if (this.conVar == null)
		{
			return;
		}
		bool flag = this.conVar.AsBool;
		if (this.inverse)
		{
			flag = !flag;
		}
		this.toggleControl.isOn = flag;
	}

	// Token: 0x04002F82 RID: 12162
	public Toggle toggleControl;

	// Token: 0x04002F83 RID: 12163
	public bool inverse;

	// Token: 0x04002F84 RID: 12164
	public static string lastConVarChanged;

	// Token: 0x04002F85 RID: 12165
	public static TimeSince timeSinceLastConVarChange;
}
