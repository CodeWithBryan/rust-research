using System;
using UnityEngine;

// Token: 0x0200001C RID: 28
public class OreHopper : PercentFullStorageContainer
{
	// Token: 0x06000098 RID: 152 RVA: 0x0000542F File Offset: 0x0000362F
	protected override void OnPercentFullChanged(float newPercentFull)
	{
		this.VisualLerpToOreLevel();
	}

	// Token: 0x06000099 RID: 153 RVA: 0x00005438 File Offset: 0x00003638
	private void SetVisualOreLevel(float percentFull)
	{
		this._oreScale.y = Mathf.Clamp01(percentFull);
		this.oreOutputMesh.localScale = this._oreScale;
		this.oreOutputMesh.gameObject.SetActive(percentFull > 0f);
		this.visualPercentFull = percentFull;
	}

	// Token: 0x0600009A RID: 154 RVA: 0x00005486 File Offset: 0x00003686
	public void VisualLerpToOreLevel()
	{
		if (base.GetPercentFull() == this.visualPercentFull)
		{
			return;
		}
		base.InvokeRepeating(new Action(this.OreVisualLerpUpdate), 0f, 0f);
	}

	// Token: 0x0600009B RID: 155 RVA: 0x000054B4 File Offset: 0x000036B4
	private void OreVisualLerpUpdate()
	{
		float percentFull = base.GetPercentFull();
		if (Mathf.Abs(this.visualPercentFull - percentFull) < 0.005f)
		{
			this.SetVisualOreLevel(percentFull);
			base.CancelInvoke(new Action(this.OreVisualLerpUpdate));
			return;
		}
		float visualOreLevel = Mathf.Lerp(this.visualPercentFull, percentFull, Time.deltaTime * 1.5f);
		this.SetVisualOreLevel(visualOreLevel);
	}

	// Token: 0x0600009C RID: 156 RVA: 0x00005515 File Offset: 0x00003715
	public override void ServerInit()
	{
		base.ServerInit();
		this.SetVisualOreLevel(base.GetPercentFull());
	}

	// Token: 0x040000AF RID: 175
	[SerializeField]
	private Transform oreOutputMesh;

	// Token: 0x040000B0 RID: 176
	private float visualPercentFull;

	// Token: 0x040000B1 RID: 177
	private Vector3 _oreScale = new Vector3(1f, 0f, 1f);
}
