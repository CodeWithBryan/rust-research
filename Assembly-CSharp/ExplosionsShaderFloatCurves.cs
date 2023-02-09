using System;
using UnityEngine;

// Token: 0x02000965 RID: 2405
public class ExplosionsShaderFloatCurves : MonoBehaviour
{
	// Token: 0x060038B3 RID: 14515 RVA: 0x0014E6D0 File Offset: 0x0014C8D0
	private void Start()
	{
		Material[] materials = base.GetComponent<Renderer>().materials;
		if (this.MaterialID >= materials.Length)
		{
			Debug.Log("ShaderColorGradient: Material ID more than shader materials count.");
		}
		this.matInstance = materials[this.MaterialID];
		if (!this.matInstance.HasProperty(this.ShaderProperty))
		{
			Debug.Log("ShaderColorGradient: Shader not have \"" + this.ShaderProperty + "\" property");
		}
		this.propertyID = Shader.PropertyToID(this.ShaderProperty);
	}

	// Token: 0x060038B4 RID: 14516 RVA: 0x0014E74A File Offset: 0x0014C94A
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.canUpdate = true;
	}

	// Token: 0x060038B5 RID: 14517 RVA: 0x0014E760 File Offset: 0x0014C960
	private void Update()
	{
		float num = Time.time - this.startTime;
		if (this.canUpdate)
		{
			float value = this.FloatPropertyCurve.Evaluate(num / this.GraphTimeMultiplier) * this.GraphScaleMultiplier;
			this.matInstance.SetFloat(this.propertyID, value);
		}
		if (num >= this.GraphTimeMultiplier)
		{
			this.canUpdate = false;
		}
	}

	// Token: 0x0400330C RID: 13068
	public string ShaderProperty = "_BumpAmt";

	// Token: 0x0400330D RID: 13069
	public int MaterialID;

	// Token: 0x0400330E RID: 13070
	public AnimationCurve FloatPropertyCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x0400330F RID: 13071
	public float GraphTimeMultiplier = 1f;

	// Token: 0x04003310 RID: 13072
	public float GraphScaleMultiplier = 1f;

	// Token: 0x04003311 RID: 13073
	private bool canUpdate;

	// Token: 0x04003312 RID: 13074
	private Material matInstance;

	// Token: 0x04003313 RID: 13075
	private int propertyID;

	// Token: 0x04003314 RID: 13076
	private float startTime;
}
