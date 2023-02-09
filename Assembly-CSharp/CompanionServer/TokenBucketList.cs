using System;
using System.Collections.Generic;
using Facepunch;

namespace CompanionServer
{
	// Token: 0x020009BD RID: 2493
	public class TokenBucketList<TKey> : ITokenBucketSettings
	{
		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x06003B0F RID: 15119 RVA: 0x0015A1E5 File Offset: 0x001583E5
		public double MaxTokens { get; }

		// Token: 0x17000495 RID: 1173
		// (get) Token: 0x06003B10 RID: 15120 RVA: 0x0015A1ED File Offset: 0x001583ED
		public double TokensPerSec { get; }

		// Token: 0x06003B11 RID: 15121 RVA: 0x0015A1F5 File Offset: 0x001583F5
		public TokenBucketList(double maxTokens, double tokensPerSec)
		{
			this._buckets = new Dictionary<TKey, TokenBucket>();
			this.MaxTokens = maxTokens;
			this.TokensPerSec = tokensPerSec;
		}

		// Token: 0x06003B12 RID: 15122 RVA: 0x0015A218 File Offset: 0x00158418
		public TokenBucket Get(TKey key)
		{
			TokenBucket result;
			if (this._buckets.TryGetValue(key, out result))
			{
				return result;
			}
			TokenBucket tokenBucket = Pool.Get<TokenBucket>();
			tokenBucket.Settings = this;
			tokenBucket.Reset();
			this._buckets.Add(key, tokenBucket);
			return tokenBucket;
		}

		// Token: 0x06003B13 RID: 15123 RVA: 0x0015A258 File Offset: 0x00158458
		public void Cleanup()
		{
			List<TKey> list = Pool.GetList<TKey>();
			foreach (KeyValuePair<TKey, TokenBucket> keyValuePair in this._buckets)
			{
				if (keyValuePair.Value.IsFull)
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (TKey key in list)
			{
				TokenBucket tokenBucket;
				if (this._buckets.TryGetValue(key, out tokenBucket))
				{
					Pool.Free<TokenBucket>(ref tokenBucket);
					this._buckets.Remove(key);
				}
			}
			Pool.FreeList<TKey>(ref list);
		}

		// Token: 0x0400351B RID: 13595
		private readonly Dictionary<TKey, TokenBucket> _buckets;
	}
}
