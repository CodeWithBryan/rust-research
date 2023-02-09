using System;
using Facepunch;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x020009C0 RID: 2496
	public abstract class BaseHandler<T> : IHandler, Pool.IPooled where T : class
	{
		// Token: 0x17000497 RID: 1175
		// (get) Token: 0x06003B26 RID: 15142 RVA: 0x00003A54 File Offset: 0x00001C54
		protected virtual int TokenCost
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x06003B27 RID: 15143 RVA: 0x0015AAA2 File Offset: 0x00158CA2
		// (set) Token: 0x06003B28 RID: 15144 RVA: 0x0015AAAA File Offset: 0x00158CAA
		public IConnection Client { get; private set; }

		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x06003B29 RID: 15145 RVA: 0x0015AAB3 File Offset: 0x00158CB3
		// (set) Token: 0x06003B2A RID: 15146 RVA: 0x0015AABB File Offset: 0x00158CBB
		public AppRequest Request { get; private set; }

		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x06003B2B RID: 15147 RVA: 0x0015AAC4 File Offset: 0x00158CC4
		// (set) Token: 0x06003B2C RID: 15148 RVA: 0x0015AACC File Offset: 0x00158CCC
		public T Proto { get; private set; }

		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x06003B2D RID: 15149 RVA: 0x0015AAD5 File Offset: 0x00158CD5
		// (set) Token: 0x06003B2E RID: 15150 RVA: 0x0015AADD File Offset: 0x00158CDD
		private protected ulong UserId { protected get; private set; }

		// Token: 0x1700049C RID: 1180
		// (get) Token: 0x06003B2F RID: 15151 RVA: 0x0015AAE6 File Offset: 0x00158CE6
		// (set) Token: 0x06003B30 RID: 15152 RVA: 0x0015AAEE File Offset: 0x00158CEE
		private protected global::BasePlayer Player { protected get; private set; }

		// Token: 0x06003B31 RID: 15153 RVA: 0x0015AAF7 File Offset: 0x00158CF7
		public void Initialize(TokenBucketList<ulong> playerBuckets, IConnection client, AppRequest request, T proto)
		{
			this._playerBuckets = playerBuckets;
			this.Client = client;
			this.Request = request;
			this.Proto = proto;
		}

		// Token: 0x06003B32 RID: 15154 RVA: 0x0015AB18 File Offset: 0x00158D18
		public virtual void EnterPool()
		{
			this._playerBuckets = null;
			this.Client = null;
			if (this.Request != null)
			{
				this.Request.Dispose();
				this.Request = null;
			}
			this.Proto = default(T);
			this.UserId = 0UL;
			this.Player = null;
		}

		// Token: 0x06003B33 RID: 15155 RVA: 0x000059DD File Offset: 0x00003BDD
		public void LeavePool()
		{
		}

		// Token: 0x06003B34 RID: 15156 RVA: 0x0015AB6C File Offset: 0x00158D6C
		public virtual ValidationResult Validate()
		{
			bool flag;
			int orGenerateAppToken = SingletonComponent<ServerMgr>.Instance.persistance.GetOrGenerateAppToken(this.Request.playerId, out flag);
			if (this.Request.playerId == 0UL || this.Request.playerToken != orGenerateAppToken)
			{
				return ValidationResult.NotFound;
			}
			if (flag)
			{
				return ValidationResult.Banned;
			}
			ServerUsers.User user = ServerUsers.Get(this.Request.playerId);
			if (((user != null) ? user.group : ServerUsers.UserGroup.None) == ServerUsers.UserGroup.Banned)
			{
				return ValidationResult.Banned;
			}
			TokenBucketList<ulong> playerBuckets = this._playerBuckets;
			TokenBucket tokenBucket = (playerBuckets != null) ? playerBuckets.Get(this.Request.playerId) : null;
			if (tokenBucket != null && tokenBucket.TryTake((double)this.TokenCost))
			{
				this.UserId = this.Request.playerId;
				this.Player = (global::BasePlayer.FindByID(this.UserId) ?? global::BasePlayer.FindSleeping(this.UserId));
				this.Client.Subscribe(new PlayerTarget(this.UserId));
				return ValidationResult.Success;
			}
			if (tokenBucket == null || !tokenBucket.IsNaughty)
			{
				return ValidationResult.RateLimit;
			}
			return ValidationResult.Rejected;
		}

		// Token: 0x06003B35 RID: 15157
		public abstract void Execute();

		// Token: 0x06003B36 RID: 15158 RVA: 0x0015AC60 File Offset: 0x00158E60
		protected void SendSuccess()
		{
			AppSuccess success = Pool.Get<AppSuccess>();
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.success = success;
			this.Send(appResponse);
		}

		// Token: 0x06003B37 RID: 15159 RVA: 0x0015AC88 File Offset: 0x00158E88
		public void SendError(string code)
		{
			AppError appError = Pool.Get<AppError>();
			appError.error = code;
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.error = appError;
			this.Send(appResponse);
		}

		// Token: 0x06003B38 RID: 15160 RVA: 0x0015ACB8 File Offset: 0x00158EB8
		public void SendFlag(bool value)
		{
			AppFlag appFlag = Pool.Get<AppFlag>();
			appFlag.value = value;
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.flag = appFlag;
			this.Send(appResponse);
		}

		// Token: 0x06003B39 RID: 15161 RVA: 0x0015ACE6 File Offset: 0x00158EE6
		protected void Send(AppResponse response)
		{
			response.seq = this.Request.seq;
			this.Client.Send(response);
		}

		// Token: 0x04003527 RID: 13607
		private TokenBucketList<ulong> _playerBuckets;
	}
}
