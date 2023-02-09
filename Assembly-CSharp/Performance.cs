using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using Rust.Workshop;
using UnityEngine;

// Token: 0x0200030B RID: 779
public class Performance : SingletonComponent<global::Performance>
{
	// Token: 0x06001DAC RID: 7596 RVA: 0x000CAA68 File Offset: 0x000C8C68
	private void Update()
	{
		global::Performance.frameTimes[Time.frameCount % 1000] = (int)(1000f * Time.deltaTime);
		using (TimeWarning.New("FPSTimer", 0))
		{
			this.FPSTimer();
		}
	}

	// Token: 0x06001DAD RID: 7597 RVA: 0x000CAAC0 File Offset: 0x000C8CC0
	public List<int> GetFrameTimes(int requestedStart, int maxCount, out int startIndex)
	{
		startIndex = Math.Max(requestedStart, Math.Max(Time.frameCount - 1000 - 1, 0));
		int num = Math.Min(Math.Min(1000, maxCount), Time.frameCount);
		List<int> list = Pool.GetList<int>();
		for (int i = 0; i < num; i++)
		{
			int num2 = (startIndex + i) % 1000;
			list.Add(global::Performance.frameTimes[num2]);
		}
		return list;
	}

	// Token: 0x06001DAE RID: 7598 RVA: 0x000CAB2C File Offset: 0x000C8D2C
	private void FPSTimer()
	{
		this.frames++;
		this.time += Time.unscaledDeltaTime;
		if (this.time < 1f)
		{
			return;
		}
		long memoryCollections = global::Performance.current.memoryCollections;
		global::Performance.current.frameID = Time.frameCount;
		global::Performance.current.frameRate = this.frames;
		global::Performance.current.frameTime = this.time / (float)this.frames * 1000f;
		checked
		{
			global::Performance.frameRateHistory[(int)((IntPtr)(global::Performance.cycles % unchecked((long)global::Performance.frameRateHistory.Length)))] = global::Performance.current.frameRate;
			global::Performance.frameTimeHistory[(int)((IntPtr)(global::Performance.cycles % unchecked((long)global::Performance.frameTimeHistory.Length)))] = global::Performance.current.frameTime;
			global::Performance.current.frameRateAverage = this.AverageFrameRate();
			global::Performance.current.frameTimeAverage = this.AverageFrameTime();
		}
		global::Performance.current.memoryUsageSystem = (long)SystemInfoEx.systemMemoryUsed;
		global::Performance.current.memoryAllocations = Rust.GC.GetTotalMemory();
		global::Performance.current.memoryCollections = (long)Rust.GC.CollectionCount();
		global::Performance.current.loadBalancerTasks = (long)LoadBalancer.Count();
		global::Performance.current.invokeHandlerTasks = (long)InvokeHandler.Count();
		global::Performance.current.workshopSkinsQueued = (long)Rust.Workshop.WorkshopSkin.QueuedCount;
		global::Performance.current.gcTriggered = (memoryCollections != global::Performance.current.memoryCollections);
		this.frames = 0;
		this.time = 0f;
		global::Performance.cycles += 1L;
		global::Performance.report = global::Performance.current;
	}

	// Token: 0x06001DAF RID: 7599 RVA: 0x000CACB0 File Offset: 0x000C8EB0
	private float AverageFrameRate()
	{
		float num = 0f;
		int num2 = Math.Min(global::Performance.frameRateHistory.Length, (int)global::Performance.cycles);
		for (int i = 0; i < num2; i++)
		{
			num += (float)global::Performance.frameRateHistory[i];
		}
		return num / (float)num2;
	}

	// Token: 0x06001DB0 RID: 7600 RVA: 0x000CACF4 File Offset: 0x000C8EF4
	private float AverageFrameTime()
	{
		float num = 0f;
		int num2 = Math.Min(global::Performance.frameTimeHistory.Length, (int)global::Performance.cycles);
		for (int i = 0; i < global::Performance.frameTimeHistory.Length; i++)
		{
			num += global::Performance.frameTimeHistory[i];
		}
		return num / (float)num2;
	}

	// Token: 0x04001711 RID: 5905
	public static global::Performance.Tick current;

	// Token: 0x04001712 RID: 5906
	public static global::Performance.Tick report;

	// Token: 0x04001713 RID: 5907
	public const int FrameHistoryCount = 1000;

	// Token: 0x04001714 RID: 5908
	private const int HistoryLength = 60;

	// Token: 0x04001715 RID: 5909
	private static long cycles = 0L;

	// Token: 0x04001716 RID: 5910
	private static int[] frameRateHistory = new int[60];

	// Token: 0x04001717 RID: 5911
	private static float[] frameTimeHistory = new float[60];

	// Token: 0x04001718 RID: 5912
	private static int[] frameTimes = new int[1000];

	// Token: 0x04001719 RID: 5913
	private int frames;

	// Token: 0x0400171A RID: 5914
	private float time;

	// Token: 0x02000C57 RID: 3159
	public struct Tick
	{
		// Token: 0x040041F0 RID: 16880
		public int frameID;

		// Token: 0x040041F1 RID: 16881
		public int frameRate;

		// Token: 0x040041F2 RID: 16882
		public float frameTime;

		// Token: 0x040041F3 RID: 16883
		public float frameRateAverage;

		// Token: 0x040041F4 RID: 16884
		public float frameTimeAverage;

		// Token: 0x040041F5 RID: 16885
		public long memoryUsageSystem;

		// Token: 0x040041F6 RID: 16886
		public long memoryAllocations;

		// Token: 0x040041F7 RID: 16887
		public long memoryCollections;

		// Token: 0x040041F8 RID: 16888
		public long loadBalancerTasks;

		// Token: 0x040041F9 RID: 16889
		public long invokeHandlerTasks;

		// Token: 0x040041FA RID: 16890
		public long workshopSkinsQueued;

		// Token: 0x040041FB RID: 16891
		public int ping;

		// Token: 0x040041FC RID: 16892
		public bool gcTriggered;

		// Token: 0x040041FD RID: 16893
		public PerformanceSamplePoint performanceSample;
	}

	// Token: 0x02000C58 RID: 3160
	private struct LagSpike
	{
		// Token: 0x040041FE RID: 16894
		public int Index;

		// Token: 0x040041FF RID: 16895
		public int Time;
	}
}
