using System;
using UnityEngine;

// Token: 0x020001A5 RID: 421
public class MapMarkerExplosion : MapMarker
{
	// Token: 0x060017CB RID: 6091 RVA: 0x000B0F98 File Offset: 0x000AF198
	public void SetDuration(float newDuration)
	{
		this.duration = newDuration;
		if (base.IsInvoking(new Action(this.DelayedDestroy)))
		{
			base.CancelInvoke(new Action(this.DelayedDestroy));
		}
		base.Invoke(new Action(this.DelayedDestroy), this.duration * 60f);
	}

	// Token: 0x060017CC RID: 6092 RVA: 0x000B0FF0 File Offset: 0x000AF1F0
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk)
		{
			Debug.LogWarning("Loaded explosion marker from disk, cleaning up");
			base.Invoke(new Action(this.DelayedDestroy), 3f);
		}
	}

	// Token: 0x060017CD RID: 6093 RVA: 0x000029D4 File Offset: 0x00000BD4
	public void DelayedDestroy()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x040010D0 RID: 4304
	private float duration = 10f;
}
