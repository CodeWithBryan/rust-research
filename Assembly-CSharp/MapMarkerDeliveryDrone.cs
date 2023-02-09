using System;

// Token: 0x02000162 RID: 354
public class MapMarkerDeliveryDrone : MapMarker
{
	// Token: 0x0600167F RID: 5759 RVA: 0x000AB1B5 File Offset: 0x000A93B5
	public override void ServerInit()
	{
		base.ServerInit();
		base.limitNetworking = true;
	}

	// Token: 0x06001680 RID: 5760 RVA: 0x000AB1C4 File Offset: 0x000A93C4
	public override bool ShouldNetworkTo(BasePlayer player)
	{
		return player.userID == base.OwnerID;
	}
}
