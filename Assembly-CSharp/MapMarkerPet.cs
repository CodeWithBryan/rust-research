using System;

// Token: 0x020001E5 RID: 485
public class MapMarkerPet : MapMarker
{
	// Token: 0x06001964 RID: 6500 RVA: 0x000AB1B5 File Offset: 0x000A93B5
	public override void ServerInit()
	{
		base.ServerInit();
		base.limitNetworking = true;
	}

	// Token: 0x06001965 RID: 6501 RVA: 0x000AB1C4 File Offset: 0x000A93C4
	public override bool ShouldNetworkTo(BasePlayer player)
	{
		return player.userID == base.OwnerID;
	}
}
