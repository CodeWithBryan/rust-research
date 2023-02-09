using System;
using UnityEngine;

// Token: 0x020002CC RID: 716
public class StatusLightRenderer : MonoBehaviour, IClientComponent
{
	// Token: 0x06001CBB RID: 7355 RVA: 0x000C4F14 File Offset: 0x000C3114
	protected void Awake()
	{
		this.propertyBlock = new MaterialPropertyBlock();
		this.targetRenderer = base.GetComponent<Renderer>();
		this.targetLight = base.GetComponent<Light>();
		this.colorID = Shader.PropertyToID("_Color");
		this.emissionID = Shader.PropertyToID("_EmissionColor");
	}

	// Token: 0x06001CBC RID: 7356 RVA: 0x000C4F64 File Offset: 0x000C3164
	public void SetOff()
	{
		if (this.targetRenderer)
		{
			this.targetRenderer.sharedMaterial = this.offMaterial;
			this.targetRenderer.SetPropertyBlock(null);
		}
		if (this.targetLight)
		{
			this.targetLight.color = Color.clear;
		}
	}

	// Token: 0x06001CBD RID: 7357 RVA: 0x000C4FB8 File Offset: 0x000C31B8
	public void SetOn()
	{
		if (this.targetRenderer)
		{
			this.targetRenderer.sharedMaterial = this.onMaterial;
			this.targetRenderer.SetPropertyBlock(this.propertyBlock);
		}
		if (this.targetLight)
		{
			this.targetLight.color = this.lightColor;
		}
	}

	// Token: 0x06001CBE RID: 7358 RVA: 0x000C5014 File Offset: 0x000C3214
	public void SetRed()
	{
		this.propertyBlock.Clear();
		this.propertyBlock.SetColor(this.colorID, this.GetColor(197, 46, 0, byte.MaxValue));
		this.propertyBlock.SetColor(this.emissionID, this.GetColor(191, 0, 2, byte.MaxValue, 2.916925f));
		this.lightColor = this.GetColor(byte.MaxValue, 111, 102, byte.MaxValue);
		this.SetOn();
	}

	// Token: 0x06001CBF RID: 7359 RVA: 0x000C5098 File Offset: 0x000C3298
	public void SetGreen()
	{
		this.propertyBlock.Clear();
		this.propertyBlock.SetColor(this.colorID, this.GetColor(19, 191, 13, byte.MaxValue));
		this.propertyBlock.SetColor(this.emissionID, this.GetColor(19, 191, 13, byte.MaxValue, 2.5f));
		this.lightColor = this.GetColor(156, byte.MaxValue, 102, byte.MaxValue);
		this.SetOn();
	}

	// Token: 0x06001CC0 RID: 7360 RVA: 0x000C5122 File Offset: 0x000C3322
	private Color GetColor(byte r, byte g, byte b, byte a)
	{
		return new Color32(r, g, b, a);
	}

	// Token: 0x06001CC1 RID: 7361 RVA: 0x000C5133 File Offset: 0x000C3333
	private Color GetColor(byte r, byte g, byte b, byte a, float intensity)
	{
		return new Color32(r, g, b, a) * intensity;
	}

	// Token: 0x0400166F RID: 5743
	public Material offMaterial;

	// Token: 0x04001670 RID: 5744
	public Material onMaterial;

	// Token: 0x04001671 RID: 5745
	private MaterialPropertyBlock propertyBlock;

	// Token: 0x04001672 RID: 5746
	private Renderer targetRenderer;

	// Token: 0x04001673 RID: 5747
	private Color lightColor;

	// Token: 0x04001674 RID: 5748
	private Light targetLight;

	// Token: 0x04001675 RID: 5749
	private int colorID;

	// Token: 0x04001676 RID: 5750
	private int emissionID;
}
