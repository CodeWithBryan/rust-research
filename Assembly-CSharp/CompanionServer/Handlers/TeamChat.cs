using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x020009CC RID: 2508
	public class TeamChat : BaseHandler<AppEmpty>
	{
		// Token: 0x06003B56 RID: 15190 RVA: 0x0015B3EC File Offset: 0x001595EC
		public override void Execute()
		{
			global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindPlayersTeam(base.UserId);
			if (playerTeam == null)
			{
				base.SendError("no_team");
				return;
			}
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.teamChat = Pool.Get<AppTeamChat>();
			appResponse.teamChat.messages = Pool.GetList<AppChatMessage>();
			IReadOnlyList<ChatLog.Entry> history = Server.TeamChat.GetHistory(playerTeam.teamID);
			if (history != null)
			{
				foreach (ChatLog.Entry entry in history)
				{
					AppChatMessage appChatMessage = Pool.Get<AppChatMessage>();
					appChatMessage.steamId = entry.SteamId;
					appChatMessage.name = entry.Name;
					appChatMessage.message = entry.Message;
					appChatMessage.color = entry.Color;
					appChatMessage.time = entry.Time;
					appResponse.teamChat.messages.Add(appChatMessage);
				}
			}
			base.Send(appResponse);
		}
	}
}
