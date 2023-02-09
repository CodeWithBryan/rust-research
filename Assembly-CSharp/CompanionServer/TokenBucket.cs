using System;
using Network;

namespace CompanionServer
{
	// Token: 0x020009BB RID: 2491
	public class TokenBucket
	{
		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x06003B07 RID: 15111 RVA: 0x0015A0F9 File Offset: 0x001582F9
		public bool IsFull
		{
			get
			{
				this.Update();
				return this._tokens >= this.Settings.MaxTokens;
			}
		}

		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x06003B08 RID: 15112 RVA: 0x0015A117 File Offset: 0x00158317
		public bool IsNaughty
		{
			get
			{
				this.Update();
				return this._tokens <= -10.0;
			}
		}

		// Token: 0x06003B09 RID: 15113 RVA: 0x0015A133 File Offset: 0x00158333
		public void Reset()
		{
			this._lastUpdate = TimeEx.realtimeSinceStartup;
			ITokenBucketSettings settings = this.Settings;
			this._tokens = ((settings != null) ? settings.MaxTokens : 0.0);
		}

		// Token: 0x06003B0A RID: 15114 RVA: 0x0015A160 File Offset: 0x00158360
		public bool TryTake(double requestedTokens)
		{
			this.Update();
			if (requestedTokens > this._tokens)
			{
				this._tokens -= 1.0;
				return false;
			}
			this._tokens -= requestedTokens;
			return true;
		}

		// Token: 0x06003B0B RID: 15115 RVA: 0x0015A198 File Offset: 0x00158398
		private void Update()
		{
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			double num = realtimeSinceStartup - this._lastUpdate;
			this._lastUpdate = realtimeSinceStartup;
			double num2 = num * this.Settings.TokensPerSec;
			this._tokens = Math.Min(this._tokens + num2, this.Settings.MaxTokens);
		}

		// Token: 0x04003518 RID: 13592
		private double _lastUpdate;

		// Token: 0x04003519 RID: 13593
		private double _tokens;

		// Token: 0x0400351A RID: 13594
		public ITokenBucketSettings Settings;
	}
}
