using System;

// Token: 0x02000467 RID: 1127
public class MapMarkerMLRSRocket : MapMarker
{
	// Token: 0x060024D4 RID: 9428 RVA: 0x000AB1B5 File Offset: 0x000A93B5
	public override void ServerInit()
	{
		base.ServerInit();
		base.limitNetworking = true;
	}

	// Token: 0x060024D5 RID: 9429 RVA: 0x000AB1C4 File Offset: 0x000A93C4
	public override bool ShouldNetworkTo(BasePlayer player)
	{
		return player.userID == base.OwnerID;
	}
}
