using System;
using System.Collections.Generic;
using System.Diagnostics;
using Rust;
using UnityEngine;

// Token: 0x020008CF RID: 2255
public class LoadBalancer : SingletonComponent<LoadBalancer>
{
	// Token: 0x06003639 RID: 13881 RVA: 0x00143A68 File Offset: 0x00141C68
	protected void LateUpdate()
	{
		if (Rust.Application.isReceiving)
		{
			return;
		}
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (LoadBalancer.Paused)
		{
			return;
		}
		int num = LoadBalancer.Count();
		float t = Mathf.InverseLerp(1000f, 100000f, (float)num);
		float num2 = Mathf.SmoothStep(1f, 100f, t);
		this.watch.Reset();
		this.watch.Start();
		for (int i = 0; i < this.queues.Length; i++)
		{
			Queue<DeferredAction> queue = this.queues[i];
			while (queue.Count > 0)
			{
				queue.Dequeue().Action();
				if (this.watch.Elapsed.TotalMilliseconds > (double)num2)
				{
					return;
				}
			}
		}
	}

	// Token: 0x0600363A RID: 13882 RVA: 0x00143B1C File Offset: 0x00141D1C
	public static int Count()
	{
		if (!SingletonComponent<LoadBalancer>.Instance)
		{
			return 0;
		}
		Queue<DeferredAction>[] array = SingletonComponent<LoadBalancer>.Instance.queues;
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			num += array[i].Count;
		}
		return num;
	}

	// Token: 0x0600363B RID: 13883 RVA: 0x00143B60 File Offset: 0x00141D60
	public static void ProcessAll()
	{
		if (!SingletonComponent<LoadBalancer>.Instance)
		{
			LoadBalancer.CreateInstance();
		}
		foreach (Queue<DeferredAction> queue in SingletonComponent<LoadBalancer>.Instance.queues)
		{
			while (queue.Count > 0)
			{
				queue.Dequeue().Action();
			}
		}
	}

	// Token: 0x0600363C RID: 13884 RVA: 0x00143BB1 File Offset: 0x00141DB1
	public static void Enqueue(DeferredAction action)
	{
		if (!SingletonComponent<LoadBalancer>.Instance)
		{
			LoadBalancer.CreateInstance();
		}
		SingletonComponent<LoadBalancer>.Instance.queues[action.Index].Enqueue(action);
	}

	// Token: 0x0600363D RID: 13885 RVA: 0x00143BDB File Offset: 0x00141DDB
	private static void CreateInstance()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "LoadBalancer";
		gameObject.AddComponent<LoadBalancer>();
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
	}

	// Token: 0x04003133 RID: 12595
	public static bool Paused;

	// Token: 0x04003134 RID: 12596
	private const float MinMilliseconds = 1f;

	// Token: 0x04003135 RID: 12597
	private const float MaxMilliseconds = 100f;

	// Token: 0x04003136 RID: 12598
	private const int MinBacklog = 1000;

	// Token: 0x04003137 RID: 12599
	private const int MaxBacklog = 100000;

	// Token: 0x04003138 RID: 12600
	private Queue<DeferredAction>[] queues = new Queue<DeferredAction>[]
	{
		new Queue<DeferredAction>(),
		new Queue<DeferredAction>(),
		new Queue<DeferredAction>(),
		new Queue<DeferredAction>(),
		new Queue<DeferredAction>()
	};

	// Token: 0x04003139 RID: 12601
	private Stopwatch watch = Stopwatch.StartNew();
}
