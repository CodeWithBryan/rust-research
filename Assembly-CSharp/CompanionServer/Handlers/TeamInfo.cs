using System;
using Facepunch;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x020009CD RID: 2509
	public class TeamInfo : BaseHandler<AppEmpty>
	{
		// Token: 0x06003B58 RID: 15192 RVA: 0x0015B4EC File Offset: 0x001596EC
		public override void Execute()
		{
			global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindPlayersTeam(base.UserId);
			AppTeamInfo appTeamInfo;
			if (playerTeam != null)
			{
				appTeamInfo = playerTeam.GetAppTeamInfo(base.UserId);
			}
			else
			{
				appTeamInfo = base.Player.GetAppTeamInfo(base.UserId);
			}
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.teamInfo = appTeamInfo;
			base.Send(appResponse);
		}
	}
}
