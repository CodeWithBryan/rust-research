using System;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000093 RID: 147
public class MapMarkerGenericRadius : MapMarker
{
	// Token: 0x06000D72 RID: 3442 RVA: 0x00070DB4 File Offset: 0x0006EFB4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MapMarkerGenericRadius.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D73 RID: 3443 RVA: 0x00070DF4 File Offset: 0x0006EFF4
	public void SendUpdate(bool fullUpdate = true)
	{
		float a = this.color1.a;
		Vector3 arg = new Vector3(this.color1.r, this.color1.g, this.color1.b);
		Vector3 arg2 = new Vector3(this.color2.r, this.color2.g, this.color2.b);
		base.ClientRPC<Vector3, float, Vector3, float, float>(null, "MarkerUpdate", arg, a, arg2, this.alpha, this.radius);
	}

	// Token: 0x06000D74 RID: 3444 RVA: 0x00070E78 File Offset: 0x0006F078
	public override AppMarker GetAppMarkerData()
	{
		AppMarker appMarkerData = base.GetAppMarkerData();
		appMarkerData.radius = this.radius;
		appMarkerData.color1 = this.color1;
		appMarkerData.color2 = this.color2;
		appMarkerData.alpha = this.alpha;
		return appMarkerData;
	}

	// Token: 0x040008A7 RID: 2215
	public float radius;

	// Token: 0x040008A8 RID: 2216
	public Color color1;

	// Token: 0x040008A9 RID: 2217
	public Color color2;

	// Token: 0x040008AA RID: 2218
	public float alpha;
}
