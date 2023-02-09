using System;
using UnityEngine;

// Token: 0x02000964 RID: 2404
public class ExplosionsShaderColorGradient : MonoBehaviour
{
	// Token: 0x060038AF RID: 14511 RVA: 0x0014E59C File Offset: 0x0014C79C
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
		this.oldColor = this.matInstance.GetColor(this.propertyID);
	}

	// Token: 0x060038B0 RID: 14512 RVA: 0x0014E62D File Offset: 0x0014C82D
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.canUpdate = true;
	}

	// Token: 0x060038B1 RID: 14513 RVA: 0x0014E644 File Offset: 0x0014C844
	private void Update()
	{
		float num = Time.time - this.startTime;
		if (this.canUpdate)
		{
			Color a = this.Color.Evaluate(num / this.TimeMultiplier);
			this.matInstance.SetColor(this.propertyID, a * this.oldColor);
		}
		if (num >= this.TimeMultiplier)
		{
			this.canUpdate = false;
		}
	}

	// Token: 0x04003303 RID: 13059
	public string ShaderProperty = "_TintColor";

	// Token: 0x04003304 RID: 13060
	public int MaterialID;

	// Token: 0x04003305 RID: 13061
	public Gradient Color = new Gradient();

	// Token: 0x04003306 RID: 13062
	public float TimeMultiplier = 1f;

	// Token: 0x04003307 RID: 13063
	private bool canUpdate;

	// Token: 0x04003308 RID: 13064
	private Material matInstance;

	// Token: 0x04003309 RID: 13065
	private int propertyID;

	// Token: 0x0400330A RID: 13066
	private float startTime;

	// Token: 0x0400330B RID: 13067
	private Color oldColor;
}
