using System;
using UnityEngine;

// Token: 0x0200095E RID: 2398
public class ExplosionsFPS : MonoBehaviour
{
	// Token: 0x06003899 RID: 14489 RVA: 0x0014E069 File Offset: 0x0014C269
	private void Awake()
	{
		this.guiStyleHeader.fontSize = 14;
		this.guiStyleHeader.normal.textColor = new Color(1f, 1f, 1f);
	}

	// Token: 0x0600389A RID: 14490 RVA: 0x0014E09C File Offset: 0x0014C29C
	private void OnGUI()
	{
		GUI.Label(new Rect(0f, 0f, 30f, 30f), "FPS: " + (int)this.fps, this.guiStyleHeader);
	}

	// Token: 0x0600389B RID: 14491 RVA: 0x0014E0D8 File Offset: 0x0014C2D8
	private void Update()
	{
		this.timeleft -= Time.deltaTime;
		this.frames++;
		if ((double)this.timeleft <= 0.0)
		{
			this.fps = (float)this.frames;
			this.timeleft = 1f;
			this.frames = 0;
		}
	}

	// Token: 0x040032E5 RID: 13029
	private readonly GUIStyle guiStyleHeader = new GUIStyle();

	// Token: 0x040032E6 RID: 13030
	private float timeleft;

	// Token: 0x040032E7 RID: 13031
	private float fps;

	// Token: 0x040032E8 RID: 13032
	private int frames;
}
