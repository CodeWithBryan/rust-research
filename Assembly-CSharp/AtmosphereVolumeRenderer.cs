using System;
using UnityEngine;

// Token: 0x020006E5 RID: 1765
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class AtmosphereVolumeRenderer : MonoBehaviour
{
	// Token: 0x170003D4 RID: 980
	// (get) Token: 0x06003161 RID: 12641 RVA: 0x0012FBB9 File Offset: 0x0012DDB9
	private static bool isSupported
	{
		get
		{
			return Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.OSXPlayer;
		}
	}

	// Token: 0x0400280A RID: 10250
	public FogMode Mode = FogMode.ExponentialSquared;

	// Token: 0x0400280B RID: 10251
	public bool DistanceFog = true;

	// Token: 0x0400280C RID: 10252
	public bool HeightFog = true;

	// Token: 0x0400280D RID: 10253
	public AtmosphereVolume Volume;
}
