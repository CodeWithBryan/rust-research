using System;
using ConVar;
using Facepunch;
using Facepunch.Math;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x020009C5 RID: 2501
	public class Info : BaseHandler<AppEmpty>
	{
		// Token: 0x06003B43 RID: 15171 RVA: 0x0015ADA0 File Offset: 0x00158FA0
		public override void Execute()
		{
			AppInfo appInfo = Facepunch.Pool.Get<AppInfo>();
			appInfo.name = Server.hostname;
			appInfo.headerImage = Server.headerimage;
			appInfo.logoImage = Server.logoimage;
			appInfo.url = Server.url;
			appInfo.map = global::World.Name;
			appInfo.mapSize = global::World.Size;
			appInfo.wipeTime = (uint)Epoch.FromDateTime(SaveRestore.SaveCreatedTime.ToUniversalTime());
			appInfo.players = (uint)global::BasePlayer.activePlayerList.Count;
			appInfo.maxPlayers = (uint)Server.maxplayers;
			appInfo.queuedPlayers = (uint)SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued;
			appInfo.seed = global::World.Seed;
			appInfo.salt = global::World.Salt;
			AppResponse appResponse = Facepunch.Pool.Get<AppResponse>();
			appResponse.info = appInfo;
			base.Send(appResponse);
		}
	}
}
