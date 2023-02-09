using System;
using System.Collections.Generic;
using Facepunch;

namespace CompanionServer
{
	// Token: 0x020009AC RID: 2476
	public class ChatLog
	{
		// Token: 0x06003AA8 RID: 15016 RVA: 0x001584D8 File Offset: 0x001566D8
		public void Record(ulong teamId, ulong steamId, string name, string message, string color, uint time)
		{
			ChatLog.ChatState chatState;
			if (!this.States.TryGetValue(teamId, out chatState))
			{
				chatState = Pool.Get<ChatLog.ChatState>();
				chatState.History = Pool.GetList<ChatLog.Entry>();
				this.States.Add(teamId, chatState);
			}
			while (chatState.History.Count >= 20)
			{
				chatState.History.RemoveAt(0);
			}
			chatState.History.Add(new ChatLog.Entry
			{
				SteamId = steamId,
				Name = name,
				Message = message,
				Color = color,
				Time = time
			});
		}

		// Token: 0x06003AA9 RID: 15017 RVA: 0x00158570 File Offset: 0x00156770
		public void Remove(ulong teamId)
		{
			ChatLog.ChatState chatState;
			if (!this.States.TryGetValue(teamId, out chatState))
			{
				return;
			}
			this.States.Remove(teamId);
			Pool.Free<ChatLog.ChatState>(ref chatState);
		}

		// Token: 0x06003AAA RID: 15018 RVA: 0x001585A4 File Offset: 0x001567A4
		public IReadOnlyList<ChatLog.Entry> GetHistory(ulong teamId)
		{
			ChatLog.ChatState chatState;
			if (!this.States.TryGetValue(teamId, out chatState))
			{
				return null;
			}
			return chatState.History;
		}

		// Token: 0x040034DE RID: 13534
		private const int MaxBacklog = 20;

		// Token: 0x040034DF RID: 13535
		private readonly Dictionary<ulong, ChatLog.ChatState> States = new Dictionary<ulong, ChatLog.ChatState>();

		// Token: 0x02000E88 RID: 3720
		public struct Entry
		{
			// Token: 0x04004AF7 RID: 19191
			public ulong SteamId;

			// Token: 0x04004AF8 RID: 19192
			public string Name;

			// Token: 0x04004AF9 RID: 19193
			public string Message;

			// Token: 0x04004AFA RID: 19194
			public string Color;

			// Token: 0x04004AFB RID: 19195
			public uint Time;
		}

		// Token: 0x02000E89 RID: 3721
		private class ChatState : Pool.IPooled
		{
			// Token: 0x060050E5 RID: 20709 RVA: 0x001A3096 File Offset: 0x001A1296
			public void EnterPool()
			{
				if (this.History != null)
				{
					Pool.FreeList<ChatLog.Entry>(ref this.History);
				}
			}

			// Token: 0x060050E6 RID: 20710 RVA: 0x000059DD File Offset: 0x00003BDD
			public void LeavePool()
			{
			}

			// Token: 0x04004AFC RID: 19196
			public List<ChatLog.Entry> History;
		}
	}
}
