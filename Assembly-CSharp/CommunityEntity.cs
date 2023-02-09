using System;
using Network;

// Token: 0x02000058 RID: 88
public class CommunityEntity : PointEntity
{
	// Token: 0x06000966 RID: 2406 RVA: 0x00058038 File Offset: 0x00056238
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CommunityEntity.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x00058078 File Offset: 0x00056278
	public override void InitShared()
	{
		if (base.isServer)
		{
			CommunityEntity.ServerInstance = this;
		}
		else
		{
			CommunityEntity.ClientInstance = this;
		}
		base.InitShared();
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x00058096 File Offset: 0x00056296
	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer)
		{
			CommunityEntity.ServerInstance = null;
			return;
		}
		CommunityEntity.ClientInstance = null;
	}

	// Token: 0x04000638 RID: 1592
	public static CommunityEntity ServerInstance;

	// Token: 0x04000639 RID: 1593
	public static CommunityEntity ClientInstance;
}
