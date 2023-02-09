using System;
using UnityEngine;

// Token: 0x020008C4 RID: 2244
public class CameraUpdateHook : MonoBehaviour
{
	// Token: 0x06003625 RID: 13861 RVA: 0x001434D0 File Offset: 0x001416D0
	private void Awake()
	{
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(delegate(Camera args)
		{
			Action preRender = CameraUpdateHook.PreRender;
			if (preRender == null)
			{
				return;
			}
			preRender();
		}));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(delegate(Camera args)
		{
			Action postRender = CameraUpdateHook.PostRender;
			if (postRender == null)
			{
				return;
			}
			postRender();
		}));
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(delegate(Camera args)
		{
			Action preCull = CameraUpdateHook.PreCull;
			if (preCull == null)
			{
				return;
			}
			preCull();
		}));
	}

	// Token: 0x04003117 RID: 12567
	public static Action PreCull;

	// Token: 0x04003118 RID: 12568
	public static Action PreRender;

	// Token: 0x04003119 RID: 12569
	public static Action PostRender;

	// Token: 0x0400311A RID: 12570
	public static Action RustCamera_PreRender;
}
