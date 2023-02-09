using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000876 RID: 2166
[ExecuteInEditMode]
public class CameraEx : MonoBehaviour
{
	// Token: 0x04003014 RID: 12308
	public bool overrideAmbientLight;

	// Token: 0x04003015 RID: 12309
	public AmbientMode ambientMode;

	// Token: 0x04003016 RID: 12310
	public Color ambientGroundColor;

	// Token: 0x04003017 RID: 12311
	public Color ambientEquatorColor;

	// Token: 0x04003018 RID: 12312
	public Color ambientLight;

	// Token: 0x04003019 RID: 12313
	public float ambientIntensity;

	// Token: 0x0400301A RID: 12314
	public ReflectionProbe reflectionProbe;

	// Token: 0x0400301B RID: 12315
	internal Color old_ambientLight;

	// Token: 0x0400301C RID: 12316
	internal Color old_ambientGroundColor;

	// Token: 0x0400301D RID: 12317
	internal Color old_ambientEquatorColor;

	// Token: 0x0400301E RID: 12318
	internal float old_ambientIntensity;

	// Token: 0x0400301F RID: 12319
	internal AmbientMode old_ambientMode;

	// Token: 0x04003020 RID: 12320
	public float aspect;
}
