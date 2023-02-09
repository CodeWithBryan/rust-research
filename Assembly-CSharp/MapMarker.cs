using System;
using System.Collections.Generic;
using CompanionServer;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020003F5 RID: 1013
public class MapMarker : global::BaseEntity
{
	// Token: 0x06002209 RID: 8713 RVA: 0x000DA1EF File Offset: 0x000D83EF
	public override void InitShared()
	{
		if (base.isServer && !MapMarker.serverMapMarkers.Contains(this))
		{
			MapMarker.serverMapMarkers.Add(this);
		}
		base.InitShared();
	}

	// Token: 0x0600220A RID: 8714 RVA: 0x000DA217 File Offset: 0x000D8417
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			MapMarker.serverMapMarkers.Remove(this);
		}
		base.DestroyShared();
	}

	// Token: 0x0600220B RID: 8715 RVA: 0x000DA234 File Offset: 0x000D8434
	public virtual AppMarker GetAppMarkerData()
	{
		AppMarker appMarker = Pool.Get<AppMarker>();
		Vector2 vector = CompanionServer.Util.WorldToMap(base.transform.position);
		appMarker.id = this.net.ID;
		appMarker.type = this.appType;
		appMarker.x = vector.x;
		appMarker.y = vector.y;
		return appMarker;
	}

	// Token: 0x04001A91 RID: 6801
	public AppMarkerType appType;

	// Token: 0x04001A92 RID: 6802
	public GameObjectRef markerObj;

	// Token: 0x04001A93 RID: 6803
	public static readonly List<MapMarker> serverMapMarkers = new List<MapMarker>();

	// Token: 0x02000C88 RID: 3208
	public enum ClusterType
	{
		// Token: 0x040042C4 RID: 17092
		None,
		// Token: 0x040042C5 RID: 17093
		Vending
	}
}
