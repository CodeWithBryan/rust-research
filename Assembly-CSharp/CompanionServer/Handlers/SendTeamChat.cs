using System;
using ConVar;
using Facepunch.Extend;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x020009C9 RID: 2505
	public class SendTeamChat : BaseHandler<AppSendMessage>
	{
		// Token: 0x1700049F RID: 1183
		// (get) Token: 0x06003B4F RID: 15183 RVA: 0x0004AF67 File Offset: 0x00049167
		protected override int TokenCost
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06003B50 RID: 15184 RVA: 0x0015B284 File Offset: 0x00159484
		public override void Execute()
		{
			string message = base.Proto.message;
			string text = (message != null) ? message.Trim() : null;
			if (string.IsNullOrWhiteSpace(text))
			{
				base.SendSuccess();
				return;
			}
			text = text.Truncate(256, "…");
			global::BasePlayer player = base.Player;
			string text2;
			if ((text2 = ((player != null) ? player.displayName : null)) == null)
			{
				text2 = (SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(base.UserId) ?? "[unknown]");
			}
			string username = text2;
			if (Chat.sayAs(Chat.ChatChannel.Team, base.UserId, username, text, base.Player))
			{
				base.SendSuccess();
				return;
			}
			base.SendError("message_not_sent");
		}
	}
}
