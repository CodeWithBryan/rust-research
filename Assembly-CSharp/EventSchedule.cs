using System;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x020004BC RID: 1212
public class EventSchedule : BaseMonoBehaviour
{
	// Token: 0x06002701 RID: 9985 RVA: 0x000F0DC5 File Offset: 0x000EEFC5
	private void OnEnable()
	{
		this.hoursRemaining = UnityEngine.Random.Range(this.minimumHoursBetween, this.maxmumHoursBetween);
		base.InvokeRepeating(new Action(this.RunSchedule), 1f, 1f);
	}

	// Token: 0x06002702 RID: 9986 RVA: 0x000F0DFA File Offset: 0x000EEFFA
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		base.CancelInvoke(new Action(this.RunSchedule));
	}

	// Token: 0x06002703 RID: 9987 RVA: 0x000F0E16 File Offset: 0x000EF016
	private void RunSchedule()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!ConVar.Server.events)
		{
			return;
		}
		this.CountHours();
		if (this.hoursRemaining > 0f)
		{
			return;
		}
		this.Trigger();
	}

	// Token: 0x06002704 RID: 9988 RVA: 0x000F0E44 File Offset: 0x000EF044
	private void Trigger()
	{
		this.hoursRemaining = UnityEngine.Random.Range(this.minimumHoursBetween, this.maxmumHoursBetween);
		TriggeredEvent[] components = base.GetComponents<TriggeredEvent>();
		if (components.Length == 0)
		{
			return;
		}
		TriggeredEvent triggeredEvent = components[UnityEngine.Random.Range(0, components.Length)];
		if (triggeredEvent == null)
		{
			return;
		}
		triggeredEvent.SendMessage("RunEvent", SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x06002705 RID: 9989 RVA: 0x000F0E98 File Offset: 0x000EF098
	private void CountHours()
	{
		if (!TOD_Sky.Instance)
		{
			return;
		}
		if (this.lastRun != 0L)
		{
			TimeSpan timeSpan = TOD_Sky.Instance.Cycle.DateTime.Subtract(DateTime.FromBinary(this.lastRun));
			this.hoursRemaining -= (float)timeSpan.TotalSeconds / 60f / 60f;
		}
		this.lastRun = TOD_Sky.Instance.Cycle.DateTime.ToBinary();
	}

	// Token: 0x04001F73 RID: 8051
	[Tooltip("The minimum amount of hours between events")]
	public float minimumHoursBetween = 12f;

	// Token: 0x04001F74 RID: 8052
	[Tooltip("The maximum amount of hours between events")]
	public float maxmumHoursBetween = 24f;

	// Token: 0x04001F75 RID: 8053
	private float hoursRemaining;

	// Token: 0x04001F76 RID: 8054
	private long lastRun;
}
