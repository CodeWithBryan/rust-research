using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ConVar;
using Steamworks;
using UnityEngine;

namespace Facepunch.Rust
{
	// Token: 0x02000ABE RID: 2750
	public class PerformanceLogging
	{
		// Token: 0x0600429A RID: 17050 RVA: 0x0018479C File Offset: 0x0018299C
		public PerformanceLogging(bool client)
		{
			this.isClient = client;
		}

		// Token: 0x0600429B RID: 17051 RVA: 0x00184836 File Offset: 0x00182A36
		private TimeSpan GetLagSpikeThreshold()
		{
			if (!this.isClient)
			{
				return TimeSpan.FromMilliseconds(200.0);
			}
			return TimeSpan.FromMilliseconds(100.0);
		}

		// Token: 0x0600429C RID: 17052 RVA: 0x00184860 File Offset: 0x00182A60
		public void OnFrame()
		{
			TimeSpan elapsed = this.frameWatch.Elapsed;
			this.Frametimes.Add(elapsed);
			this.frameWatch.Restart();
			DateTime utcNow = DateTime.UtcNow;
			int num = System.GC.CollectionCount(0);
			bool flag = this.lastFrameGC != num;
			this.lastFrameGC = num;
			if (flag)
			{
				this.garbageCollections.Add(new PerformanceLogging.GarbageCollect
				{
					FrameIndex = this.Frametimes.Count - 1,
					Time = elapsed
				});
			}
			if (elapsed > this.GetLagSpikeThreshold())
			{
				this.lagSpikes.Add(new PerformanceLogging.LagSpike
				{
					FrameIndex = this.Frametimes.Count - 1,
					Time = elapsed,
					WasGC = flag
				});
			}
			if (utcNow > this.nextFlushTime)
			{
				if (this.nextFlushTime == default(DateTime))
				{
					DateTime.UtcNow.Add(this.Interval);
					return;
				}
				this.Flush();
			}
		}

		// Token: 0x0600429D RID: 17053 RVA: 0x0018496C File Offset: 0x00182B6C
		public void Flush()
		{
			PerformanceLogging.<>c__DisplayClass25_0 CS$<>8__locals1 = new PerformanceLogging.<>c__DisplayClass25_0();
			CS$<>8__locals1.<>4__this = this;
			this.nextFlushTime = DateTime.UtcNow.Add(this.Interval);
			CS$<>8__locals1.record = EventRecord.New(this.isClient ? "client_performance" : "server_performance", !this.isClient);
			CS$<>8__locals1.record.AddField("lag_spike_count", this.lagSpikes.Count).AddField("lag_spike_threshold", this.GetLagSpikeThreshold()).AddField("gc_count", this.garbageCollections.Count).AddField("ram_managed", System.GC.GetTotalMemory(false)).AddField("ram_total", SystemInfoEx.systemMemoryUsed).AddField("total_session_id", this.totalSessionId.ToString("N")).AddField("uptime", (int)UnityEngine.Time.realtimeSinceStartup).AddField("map_url", global::World.Url).AddField("world_size", global::World.Size).AddField("world_seed", global::World.Seed);
			if (!this.isClient)
			{
				CS$<>8__locals1.record.AddField("is_official", Server.official && Server.stats);
			}
			string value;
			string value2;
			using (SHA256 sha = SHA256.Create())
			{
				value = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(SystemInfo.deviceUniqueIdentifier)));
				value2 = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(SteamClient.SteamId.ToString() + "random_junk_here_489327534")));
			}
			if (this.isClient)
			{
				CS$<>8__locals1.record.AddField("steam_id_hash", value2);
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["device_name"] = SystemInfo.deviceName;
			dictionary["device_hash"] = value;
			dictionary["gpu_name"] = SystemInfo.graphicsDeviceName;
			dictionary["gpu_ram"] = SystemInfo.graphicsMemorySize.ToString();
			dictionary["gpu_vendor"] = SystemInfo.graphicsDeviceVendor;
			dictionary["gpu_version"] = SystemInfo.graphicsDeviceVersion;
			dictionary["cpu_cores"] = SystemInfo.processorCount.ToString();
			dictionary["cpu_frequency"] = SystemInfo.processorFrequency.ToString();
			dictionary["cpu_name"] = SystemInfo.processorType.Trim();
			dictionary["system_memory"] = SystemInfo.systemMemorySize.ToString();
			dictionary["os"] = SystemInfo.operatingSystem;
			Dictionary<string, string> data = dictionary;
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			dictionary2["unity"] = (Application.unityVersion ?? "editor");
			dictionary2["changeset"] = (BuildInfo.Current.Scm.ChangeId ?? "editor");
			dictionary2["branch"] = (BuildInfo.Current.Scm.Branch ?? "editor");
			dictionary2["network_version"] = 2370.ToString();
			Dictionary<string, string> data2 = dictionary2;
			CS$<>8__locals1.record.AddObject("hardware", data).AddObject("application", data2);
			CS$<>8__locals1.frametimes = this.Frametimes;
			CS$<>8__locals1.ping = this.PingHistory;
			Task.Run(delegate()
			{
				PerformanceLogging.<>c__DisplayClass25_0.<<Flush>b__0>d <<Flush>b__0>d;
				<<Flush>b__0>d.<>4__this = CS$<>8__locals1;
				<<Flush>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<Flush>b__0>d.<>1__state = -1;
				AsyncTaskMethodBuilder <>t__builder = <<Flush>b__0>d.<>t__builder;
				<>t__builder.Start<PerformanceLogging.<>c__DisplayClass25_0.<<Flush>b__0>d>(ref <<Flush>b__0>d);
				return <<Flush>b__0>d.<>t__builder.Task;
			});
			this.ResetMeasurements();
		}

		// Token: 0x0600429E RID: 17054 RVA: 0x00184CE0 File Offset: 0x00182EE0
		private void ResetMeasurements()
		{
			this.nextFlushTime = DateTime.UtcNow.Add(this.Interval);
			if (this.Frametimes.Count == 0)
			{
				return;
			}
			PerformanceLogging.PerformancePool performancePool;
			while (this.pool.TryDequeue(out performancePool))
			{
				Pool.Free<List<TimeSpan>>(ref performancePool.Frametimes);
				Pool.Free<List<int>>(ref performancePool.Ping);
			}
			this.Frametimes = Pool.GetList<TimeSpan>();
			this.PingHistory = Pool.GetList<int>();
			this.garbageCollections.Clear();
		}

		// Token: 0x0600429F RID: 17055 RVA: 0x00184D5C File Offset: 0x00182F5C
		private Task ProcessPerformanceData(EventRecord record, List<TimeSpan> frametimes, List<int> ping)
		{
			if (frametimes.Count <= 1)
			{
				return Task.CompletedTask;
			}
			this.sortedList.Clear();
			this.sortedList.AddRange(frametimes);
			this.sortedList.Sort();
			int count = frametimes.Count;
			Mathf.Max(1, frametimes.Count / 100);
			Mathf.Max(1, frametimes.Count / 1000);
			TimeSpan timeSpan = default(TimeSpan);
			for (int i = 0; i < count; i++)
			{
				TimeSpan t = this.sortedList[i];
				timeSpan += t;
			}
			double frametime_average = timeSpan.TotalMilliseconds / (double)count;
			double value = Math.Sqrt(this.sortedList.Sum((TimeSpan x) => Math.Pow(x.TotalMilliseconds - frametime_average, 2.0)) / (double)this.sortedList.Count - 1.0);
			record.AddField("total_time", timeSpan).AddField("frames", count).AddField("frametime_average", timeSpan.TotalSeconds / (double)count).AddField("frametime_99_9", this.sortedList[Mathf.Clamp(count - count / 1000, 0, count - 1)]).AddField("frametime_99", this.sortedList[Mathf.Clamp(count - count / 100, 0, count - 1)]).AddField("frametime_90", this.sortedList[Mathf.Clamp(count - count / 10, 0, count - 1)]).AddField("frametime_75", this.sortedList[Mathf.Clamp(count - count / 4, 0, count - 1)]).AddField("frametime_50", this.sortedList[count / 2]).AddField("frametime_25", this.sortedList[count / 4]).AddField("frametime_10", this.sortedList[count / 10]).AddField("frametime_1", this.sortedList[count / 100]).AddField("frametime_0_1", this.sortedList[count / 1000]).AddField("frametime_std_dev", value).AddField("gc_generations", System.GC.MaxGeneration).AddField("gc_total", System.GC.CollectionCount(System.GC.MaxGeneration));
			if (this.isClient)
			{
				record.AddField("ping_average", (ping.Count == 0) ? 0 : ((int)ping.Average())).AddField("ping_count", ping.Count);
			}
			record.Submit();
			frametimes.Clear();
			ping.Clear();
			this.pool.Enqueue(new PerformanceLogging.PerformancePool
			{
				Frametimes = frametimes,
				Ping = ping
			});
			return Task.CompletedTask;
		}

		// Token: 0x04003AC3 RID: 15043
		public static PerformanceLogging server = new PerformanceLogging(false);

		// Token: 0x04003AC4 RID: 15044
		public static PerformanceLogging client = new PerformanceLogging(true);

		// Token: 0x04003AC5 RID: 15045
		private readonly TimeSpan Interval = TimeSpan.FromMinutes(10.0);

		// Token: 0x04003AC6 RID: 15046
		private readonly TimeSpan PingInterval = TimeSpan.FromSeconds(5.0);

		// Token: 0x04003AC7 RID: 15047
		private List<TimeSpan> Frametimes = new List<TimeSpan>();

		// Token: 0x04003AC8 RID: 15048
		private List<int> PingHistory = new List<int>();

		// Token: 0x04003AC9 RID: 15049
		private List<PerformanceLogging.LagSpike> lagSpikes = new List<PerformanceLogging.LagSpike>();

		// Token: 0x04003ACA RID: 15050
		private List<PerformanceLogging.GarbageCollect> garbageCollections = new List<PerformanceLogging.GarbageCollect>();

		// Token: 0x04003ACB RID: 15051
		private bool isClient;

		// Token: 0x04003ACC RID: 15052
		private Stopwatch frameWatch = new Stopwatch();

		// Token: 0x04003ACD RID: 15053
		private DateTime nextPingTime;

		// Token: 0x04003ACE RID: 15054
		private DateTime nextFlushTime;

		// Token: 0x04003ACF RID: 15055
		private DateTime connectedTime;

		// Token: 0x04003AD0 RID: 15056
		private int serverIndex;

		// Token: 0x04003AD1 RID: 15057
		private Guid totalSessionId = Guid.NewGuid();

		// Token: 0x04003AD2 RID: 15058
		private Guid sessionId;

		// Token: 0x04003AD3 RID: 15059
		private int lastFrameGC;

		// Token: 0x04003AD4 RID: 15060
		private ConcurrentQueue<PerformanceLogging.PerformancePool> pool = new ConcurrentQueue<PerformanceLogging.PerformancePool>();

		// Token: 0x04003AD5 RID: 15061
		private List<TimeSpan> sortedList = new List<TimeSpan>();

		// Token: 0x02000F21 RID: 3873
		private struct LagSpike
		{
			// Token: 0x04004D64 RID: 19812
			public int FrameIndex;

			// Token: 0x04004D65 RID: 19813
			public TimeSpan Time;

			// Token: 0x04004D66 RID: 19814
			public bool WasGC;
		}

		// Token: 0x02000F22 RID: 3874
		private struct GarbageCollect
		{
			// Token: 0x04004D67 RID: 19815
			public int FrameIndex;

			// Token: 0x04004D68 RID: 19816
			public TimeSpan Time;
		}

		// Token: 0x02000F23 RID: 3875
		private class PerformancePool
		{
			// Token: 0x04004D69 RID: 19817
			public List<TimeSpan> Frametimes;

			// Token: 0x04004D6A RID: 19818
			public List<int> Ping;
		}
	}
}
