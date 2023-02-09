using System;
using UnityEngine;

// Token: 0x02000858 RID: 2136
public class TweakUIBase : MonoBehaviour
{
	// Token: 0x060034F9 RID: 13561 RVA: 0x0013F722 File Offset: 0x0013D922
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x060034FA RID: 13562 RVA: 0x0013F72C File Offset: 0x0013D92C
	protected virtual void Init()
	{
		this.conVar = ConsoleSystem.Index.Client.Find(this.convarName);
		if (this.conVar == null)
		{
			Debug.LogWarning("TweakUI Convar Missing: " + this.convarName, base.gameObject);
			return;
		}
		this.conVar.OnValueChanged += this.OnConVarChanged;
	}

	// Token: 0x060034FB RID: 13563 RVA: 0x0013F786 File Offset: 0x0013D986
	public virtual void OnApplyClicked()
	{
		if (this.ApplyImmediatelyOnChange)
		{
			return;
		}
		this.SetConvarValue();
	}

	// Token: 0x060034FC RID: 13564 RVA: 0x0013F797 File Offset: 0x0013D997
	public virtual void UnapplyChanges()
	{
		if (this.ApplyImmediatelyOnChange)
		{
			return;
		}
		this.ResetToConvar();
	}

	// Token: 0x060034FD RID: 13565 RVA: 0x0013F7A8 File Offset: 0x0013D9A8
	protected virtual void OnConVarChanged(ConsoleSystem.Command obj)
	{
		this.ResetToConvar();
	}

	// Token: 0x060034FE RID: 13566 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void ResetToConvar()
	{
	}

	// Token: 0x060034FF RID: 13567 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void SetConvarValue()
	{
	}

	// Token: 0x06003500 RID: 13568 RVA: 0x0013F7B0 File Offset: 0x0013D9B0
	private void OnDestroy()
	{
		if (this.conVar != null)
		{
			this.conVar.OnValueChanged -= this.OnConVarChanged;
		}
	}

	// Token: 0x04002F70 RID: 12144
	public string convarName = "effects.motionblur";

	// Token: 0x04002F71 RID: 12145
	public bool ApplyImmediatelyOnChange = true;

	// Token: 0x04002F72 RID: 12146
	internal ConsoleSystem.Command conVar;
}
