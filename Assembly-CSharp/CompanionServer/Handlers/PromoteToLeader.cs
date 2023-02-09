using System;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x020009C8 RID: 2504
	public class PromoteToLeader : BaseHandler<AppPromoteToLeader>
	{
		// Token: 0x06003B4D RID: 15181 RVA: 0x0015B1E0 File Offset: 0x001593E0
		public override void Execute()
		{
			global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindPlayersTeam(base.UserId);
			if (playerTeam == null)
			{
				base.SendError("no_team");
				return;
			}
			if (playerTeam.teamLeader != base.UserId)
			{
				base.SendError("access_denied");
				return;
			}
			if (playerTeam.teamLeader == base.Proto.steamId)
			{
				base.SendSuccess();
				return;
			}
			if (!playerTeam.members.Contains(base.Proto.steamId))
			{
				base.SendError("not_found");
				return;
			}
			playerTeam.SetTeamLeader(base.Proto.steamId);
			base.SendSuccess();
		}
	}
}
