using System;
using System.Collections.Generic;

namespace Facepunch.Rust
{
	// Token: 0x02000ABD RID: 2749
	public class EventRecord : Pool.IPooled
	{
		// Token: 0x0600428A RID: 17034 RVA: 0x001845DE File Offset: 0x001827DE
		public void EnterPool()
		{
			this.Timestamp = default(DateTime);
			this.Data.Clear();
		}

		// Token: 0x0600428B RID: 17035 RVA: 0x000059DD File Offset: 0x00003BDD
		public void LeavePool()
		{
		}

		// Token: 0x0600428C RID: 17036 RVA: 0x001845F7 File Offset: 0x001827F7
		public static EventRecord New(string type, bool isServer = true)
		{
			EventRecord eventRecord = Pool.Get<EventRecord>();
			eventRecord.AddField("type", type);
			eventRecord.IsServer = isServer;
			eventRecord.Timestamp = DateTime.UtcNow;
			return eventRecord;
		}

		// Token: 0x0600428D RID: 17037 RVA: 0x0018461D File Offset: 0x0018281D
		public EventRecord AddObject(string key, object data)
		{
			this.Data[key] = data;
			return this;
		}

		// Token: 0x0600428E RID: 17038 RVA: 0x0018462D File Offset: 0x0018282D
		public EventRecord SetTimestamp(DateTime timestamp)
		{
			this.Timestamp = timestamp;
			return this;
		}

		// Token: 0x0600428F RID: 17039 RVA: 0x00184637 File Offset: 0x00182837
		public EventRecord AddField(string key, bool value)
		{
			this.Data[key] = (value ? "true" : "false");
			return this;
		}

		// Token: 0x06004290 RID: 17040 RVA: 0x0018461D File Offset: 0x0018281D
		public EventRecord AddField(string key, string value)
		{
			this.Data[key] = value;
			return this;
		}

		// Token: 0x06004291 RID: 17041 RVA: 0x00184655 File Offset: 0x00182855
		public EventRecord AddField(string key, int value)
		{
			this.Data[key] = value.ToString();
			return this;
		}

		// Token: 0x06004292 RID: 17042 RVA: 0x0018466B File Offset: 0x0018286B
		public EventRecord AddField(string key, uint value)
		{
			this.Data[key] = value.ToString();
			return this;
		}

		// Token: 0x06004293 RID: 17043 RVA: 0x00184681 File Offset: 0x00182881
		public EventRecord AddField(string key, ulong value)
		{
			this.Data[key] = value.ToString();
			return this;
		}

		// Token: 0x06004294 RID: 17044 RVA: 0x00184697 File Offset: 0x00182897
		public EventRecord AddField(string key, long value)
		{
			this.Data[key] = value.ToString();
			return this;
		}

		// Token: 0x06004295 RID: 17045 RVA: 0x001846AD File Offset: 0x001828AD
		public EventRecord AddField(string key, float value)
		{
			this.Data[key] = value.ToString();
			return this;
		}

		// Token: 0x06004296 RID: 17046 RVA: 0x001846C3 File Offset: 0x001828C3
		public EventRecord AddField(string key, double value)
		{
			this.Data[key] = value.ToString();
			return this;
		}

		// Token: 0x06004297 RID: 17047 RVA: 0x001846DC File Offset: 0x001828DC
		public EventRecord AddField(string key, TimeSpan value)
		{
			this.Data[key] = value.TotalSeconds.ToString();
			return this;
		}

		// Token: 0x06004298 RID: 17048 RVA: 0x00184708 File Offset: 0x00182908
		public EventRecord AddField(string key, BaseEntity entity)
		{
			this.Data[key + "_prefab"] = entity.ShortPrefabName;
			this.Data[key + "_pos"] = entity.transform.position.ToString();
			this.Data[key + "_id"] = entity.net.ID.ToString();
			return this;
		}

		// Token: 0x06004299 RID: 17049 RVA: 0x00184787 File Offset: 0x00182987
		public void Submit()
		{
			if (this.IsServer)
			{
				Analytics.AzureWebInterface.server.EnqueueEvent(this);
			}
		}

		// Token: 0x04003AC0 RID: 15040
		public DateTime Timestamp;

		// Token: 0x04003AC1 RID: 15041
		[NonSerialized]
		public bool IsServer;

		// Token: 0x04003AC2 RID: 15042
		public Dictionary<string, object> Data = new Dictionary<string, object>();
	}
}
