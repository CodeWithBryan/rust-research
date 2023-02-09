using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ConVar;
using Facepunch;
using Fleck;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	// Token: 0x020009AD RID: 2477
	public class Connection : IConnection
	{
		// Token: 0x1700048B RID: 1163
		// (get) Token: 0x06003AAC RID: 15020 RVA: 0x001585DC File Offset: 0x001567DC
		public IPAddress Address
		{
			get
			{
				return this._connection.ConnectionInfo.ClientIpAddress;
			}
		}

		// Token: 0x06003AAD RID: 15021 RVA: 0x001585EE File Offset: 0x001567EE
		public Connection(Listener listener, IWebSocketConnection connection)
		{
			this._listener = listener;
			this._connection = connection;
			this._subscribedPlayers = new HashSet<PlayerTarget>();
			this._subscribedEntities = new HashSet<EntityTarget>();
		}

		// Token: 0x06003AAE RID: 15022 RVA: 0x0015861C File Offset: 0x0015681C
		public void OnClose()
		{
			foreach (PlayerTarget key in this._subscribedPlayers)
			{
				this._listener.PlayerSubscribers.Remove(key, this);
			}
			foreach (EntityTarget key2 in this._subscribedEntities)
			{
				this._listener.EntitySubscribers.Remove(key2, this);
			}
		}

		// Token: 0x06003AAF RID: 15023 RVA: 0x001586C8 File Offset: 0x001568C8
		public void OnMessage(System.Span<byte> data)
		{
			if (!App.update || App.queuelimit <= 0)
			{
				return;
			}
			MemoryBuffer buffer = new MemoryBuffer(data.Length);
			data.CopyTo(buffer);
			this._listener.Enqueue(this, buffer.Slice(data.Length));
		}

		// Token: 0x06003AB0 RID: 15024 RVA: 0x0015871A File Offset: 0x0015691A
		public void Close()
		{
			IWebSocketConnection connection = this._connection;
			if (connection == null)
			{
				return;
			}
			connection.Close();
		}

		// Token: 0x06003AB1 RID: 15025 RVA: 0x0015872C File Offset: 0x0015692C
		public void Send(AppResponse response)
		{
			AppMessage appMessage = Facepunch.Pool.Get<AppMessage>();
			appMessage.response = response;
			Connection.MessageStream.Position = 0L;
			appMessage.ToProto(Connection.MessageStream);
			int num = (int)Connection.MessageStream.Position;
			Connection.MessageStream.Position = 0L;
			MemoryBuffer memoryBuffer = new MemoryBuffer(num);
			Connection.MessageStream.Read(memoryBuffer.Data, 0, num);
			if (appMessage.ShouldPool)
			{
				appMessage.Dispose();
			}
			this.SendRaw(memoryBuffer.Slice(num));
		}

		// Token: 0x06003AB2 RID: 15026 RVA: 0x001587AD File Offset: 0x001569AD
		public void Subscribe(PlayerTarget target)
		{
			if (this._subscribedPlayers.Add(target))
			{
				this._listener.PlayerSubscribers.Add(target, this);
			}
		}

		// Token: 0x06003AB3 RID: 15027 RVA: 0x001587CF File Offset: 0x001569CF
		public void Unsubscribe(PlayerTarget target)
		{
			if (this._subscribedPlayers.Remove(target))
			{
				this._listener.PlayerSubscribers.Remove(target, this);
			}
		}

		// Token: 0x06003AB4 RID: 15028 RVA: 0x001587F1 File Offset: 0x001569F1
		public void Subscribe(EntityTarget target)
		{
			if (this._subscribedEntities.Add(target))
			{
				this._listener.EntitySubscribers.Add(target, this);
			}
		}

		// Token: 0x06003AB5 RID: 15029 RVA: 0x00158813 File Offset: 0x00156A13
		public void Unsubscribe(EntityTarget target)
		{
			if (this._subscribedEntities.Remove(target))
			{
				this._listener.EntitySubscribers.Remove(target, this);
			}
		}

		// Token: 0x06003AB6 RID: 15030 RVA: 0x00158838 File Offset: 0x00156A38
		public void SendRaw(MemoryBuffer data)
		{
			try
			{
				this._connection.Send(data);
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Failed to send message to app client {0}: {1}", this._connection.ConnectionInfo.ClientIpAddress, arg));
			}
		}

		// Token: 0x040034E0 RID: 13536
		private static readonly MemoryStream MessageStream = new MemoryStream(1048576);

		// Token: 0x040034E1 RID: 13537
		private readonly Listener _listener;

		// Token: 0x040034E2 RID: 13538
		private readonly IWebSocketConnection _connection;

		// Token: 0x040034E3 RID: 13539
		private readonly HashSet<PlayerTarget> _subscribedPlayers;

		// Token: 0x040034E4 RID: 13540
		private readonly HashSet<EntityTarget> _subscribedEntities;
	}
}
