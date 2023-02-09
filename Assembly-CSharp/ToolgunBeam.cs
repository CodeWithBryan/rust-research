using System;
using UnityEngine;

// Token: 0x020001BE RID: 446
public class ToolgunBeam : MonoBehaviour
{
	// Token: 0x060017FB RID: 6139 RVA: 0x000B1860 File Offset: 0x000AFA60
	public void Update()
	{
		if (this.fadeColor.a <= 0f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		this.electricalBeam.sharedMaterial.SetTextureOffset("_MainTex", new Vector2(Time.time * this.scrollSpeed, 0f));
		this.fadeColor.a = this.fadeColor.a - Time.deltaTime * this.fadeSpeed;
		this.electricalBeam.startColor = this.fadeColor;
		this.electricalBeam.endColor = this.fadeColor;
	}

	// Token: 0x04001141 RID: 4417
	public LineRenderer electricalBeam;

	// Token: 0x04001142 RID: 4418
	public float scrollSpeed = -8f;

	// Token: 0x04001143 RID: 4419
	private Color fadeColor = new Color(1f, 1f, 1f, 1f);

	// Token: 0x04001144 RID: 4420
	public float fadeSpeed = 4f;
}
