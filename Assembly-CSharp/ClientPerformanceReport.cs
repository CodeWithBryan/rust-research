using System;

// Token: 0x020002F8 RID: 760
public struct ClientPerformanceReport
{
	// Token: 0x040016E3 RID: 5859
	public int request_id;

	// Token: 0x040016E4 RID: 5860
	public string user_id;

	// Token: 0x040016E5 RID: 5861
	public float fps_average;

	// Token: 0x040016E6 RID: 5862
	public int fps;

	// Token: 0x040016E7 RID: 5863
	public int frame_id;

	// Token: 0x040016E8 RID: 5864
	public float frame_time;

	// Token: 0x040016E9 RID: 5865
	public float frame_time_average;

	// Token: 0x040016EA RID: 5866
	public long memory_system;

	// Token: 0x040016EB RID: 5867
	public long memory_collections;

	// Token: 0x040016EC RID: 5868
	public long memory_managed_heap;

	// Token: 0x040016ED RID: 5869
	public float realtime_since_startup;

	// Token: 0x040016EE RID: 5870
	public bool streamer_mode;

	// Token: 0x040016EF RID: 5871
	public int ping;

	// Token: 0x040016F0 RID: 5872
	public int tasks_invokes;

	// Token: 0x040016F1 RID: 5873
	public int tasks_load_balancer;

	// Token: 0x040016F2 RID: 5874
	public int workshop_skins_queued;
}
