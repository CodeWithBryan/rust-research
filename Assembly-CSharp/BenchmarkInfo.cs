using System;
using System.Text;
using Facepunch;
using Rust.Workshop;
using TMPro;
using UnityEngine;

// Token: 0x02000277 RID: 631
public class BenchmarkInfo : SingletonComponent<BenchmarkInfo>
{
	// Token: 0x06001BDC RID: 7132 RVA: 0x000C14F4 File Offset: 0x000BF6F4
	private void Start()
	{
		string text = Environment.MachineName + "\n";
		text = text + SystemInfo.operatingSystem + "\n";
		text += string.Format("{0:0}GB RAM\n", (double)SystemInfo.systemMemorySize / 1024.0);
		text = text + SystemInfo.processorType + "\n";
		text += string.Format("{0} ({1:0}GB)\n", SystemInfo.graphicsDeviceName, (double)SystemInfo.graphicsMemorySize / 1024.0);
		text += "\n";
		text = string.Concat(new string[]
		{
			text,
			BuildInfo.Current.Build.Node,
			" / ",
			BuildInfo.Current.Scm.Date,
			"\n"
		});
		text = string.Concat(new string[]
		{
			text,
			BuildInfo.Current.Scm.Repo,
			"/",
			BuildInfo.Current.Scm.Branch,
			"#",
			BuildInfo.Current.Scm.ChangeId,
			"\n"
		});
		text = string.Concat(new string[]
		{
			text,
			BuildInfo.Current.Scm.Author,
			" - ",
			BuildInfo.Current.Scm.Comment,
			"\n"
		});
		this.SystemInfoText.text = text;
	}

	// Token: 0x06001BDD RID: 7133 RVA: 0x000C1684 File Offset: 0x000BF884
	private void Update()
	{
		if (this.timeSinceUpdated < 0.25f)
		{
			return;
		}
		this.timeSinceUpdated = 0f;
		this.sb.Clear();
		this.sb.AppendLine(BenchmarkInfo.BenchmarkTitle);
		this.sb.AppendLine(BenchmarkInfo.BenchmarkSubtitle);
		this.sb.AppendLine();
		this.sb.Append(global::Performance.current.frameRate).Append(" FPS");
		this.sb.Append(" / ").Append(global::Performance.current.frameTime.ToString("0.0")).Append("ms");
		this.sb.AppendLine().Append(global::Performance.current.memoryAllocations).Append(" MB");
		this.sb.Append(" / ").Append(global::Performance.current.memoryCollections).Append(" GC");
		this.sb.AppendLine().Append(global::Performance.current.memoryUsageSystem).Append(" RAM");
		this.sb.AppendLine().Append(global::Performance.current.loadBalancerTasks).Append(" TASKS");
		this.sb.Append(" / ").Append(Rust.Workshop.WorkshopSkin.QueuedCount).Append(" SKINS");
		this.sb.Append(" / ").Append(global::Performance.current.invokeHandlerTasks).Append(" INVOKES");
		this.sb.AppendLine();
		this.sb.AppendLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
		this.PerformanceText.text = this.sb.ToString();
	}

	// Token: 0x040014FF RID: 5375
	public static string BenchmarkTitle;

	// Token: 0x04001500 RID: 5376
	public static string BenchmarkSubtitle;

	// Token: 0x04001501 RID: 5377
	public TextMeshProUGUI PerformanceText;

	// Token: 0x04001502 RID: 5378
	public TextMeshProUGUI SystemInfoText;

	// Token: 0x04001503 RID: 5379
	private StringBuilder sb = new StringBuilder();

	// Token: 0x04001504 RID: 5380
	private RealTimeSince timeSinceUpdated;
}
