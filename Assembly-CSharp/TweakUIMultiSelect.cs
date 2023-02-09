using System;
using System.Linq;
using UnityEngine.UI;

// Token: 0x0200085A RID: 2138
public class TweakUIMultiSelect : TweakUIBase
{
	// Token: 0x0600350C RID: 13580 RVA: 0x0013FB40 File Offset: 0x0013DD40
	protected override void Init()
	{
		base.Init();
		this.UpdateToggleGroup();
	}

	// Token: 0x0600350D RID: 13581 RVA: 0x0013FB4E File Offset: 0x0013DD4E
	protected void OnEnable()
	{
		this.UpdateToggleGroup();
	}

	// Token: 0x0600350E RID: 13582 RVA: 0x0013FB56 File Offset: 0x0013DD56
	public void OnChanged()
	{
		this.UpdateConVar();
	}

	// Token: 0x0600350F RID: 13583 RVA: 0x0013FB60 File Offset: 0x0013DD60
	private void UpdateToggleGroup()
	{
		if (this.conVar == null)
		{
			return;
		}
		string @string = this.conVar.String;
		foreach (Toggle toggle in this.toggleGroup.GetComponentsInChildren<Toggle>())
		{
			toggle.isOn = (toggle.name == @string);
		}
	}

	// Token: 0x06003510 RID: 13584 RVA: 0x0013FBB0 File Offset: 0x0013DDB0
	private void UpdateConVar()
	{
		if (this.conVar == null)
		{
			return;
		}
		Toggle toggle = (from x in this.toggleGroup.GetComponentsInChildren<Toggle>()
		where x.isOn
		select x).FirstOrDefault<Toggle>();
		if (toggle == null)
		{
			return;
		}
		if (this.conVar.String == toggle.name)
		{
			return;
		}
		this.conVar.Set(toggle.name);
	}

	// Token: 0x04002F7D RID: 12157
	public ToggleGroup toggleGroup;
}
