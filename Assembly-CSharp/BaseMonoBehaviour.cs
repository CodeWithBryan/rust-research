using System;
using ConVar;
using UnityEngine;

// Token: 0x020008BB RID: 2235
public abstract class BaseMonoBehaviour : FacepunchBehaviour
{
	// Token: 0x06003603 RID: 13827 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool IsDebugging()
	{
		return false;
	}

	// Token: 0x06003604 RID: 13828 RVA: 0x00143053 File Offset: 0x00141253
	public virtual string GetLogColor()
	{
		return "yellow";
	}

	// Token: 0x06003605 RID: 13829 RVA: 0x0014305C File Offset: 0x0014125C
	public void LogEntry(BaseMonoBehaviour.LogEntryType log, int level, string str, object arg1)
	{
		if (!this.IsDebugging() && Global.developer < level)
		{
			return;
		}
		string text = string.Format(str, arg1);
		Debug.Log(string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", new object[]
		{
			log.ToString().PadRight(10),
			this.ToString(),
			text,
			this.GetLogColor()
		}), base.gameObject);
	}

	// Token: 0x06003606 RID: 13830 RVA: 0x001430CC File Offset: 0x001412CC
	public void LogEntry(BaseMonoBehaviour.LogEntryType log, int level, string str, object arg1, object arg2)
	{
		if (!this.IsDebugging() && Global.developer < level)
		{
			return;
		}
		string text = string.Format(str, arg1, arg2);
		Debug.Log(string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", new object[]
		{
			log.ToString().PadRight(10),
			this.ToString(),
			text,
			this.GetLogColor()
		}), base.gameObject);
	}

	// Token: 0x06003607 RID: 13831 RVA: 0x00143140 File Offset: 0x00141340
	public void LogEntry(BaseMonoBehaviour.LogEntryType log, int level, string str)
	{
		if (!this.IsDebugging() && Global.developer < level)
		{
			return;
		}
		Debug.Log(string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", new object[]
		{
			log.ToString().PadRight(10),
			this.ToString(),
			str,
			this.GetLogColor()
		}), base.gameObject);
	}

	// Token: 0x02000E50 RID: 3664
	public enum LogEntryType
	{
		// Token: 0x04004A0C RID: 18956
		General,
		// Token: 0x04004A0D RID: 18957
		Network,
		// Token: 0x04004A0E RID: 18958
		Hierarchy,
		// Token: 0x04004A0F RID: 18959
		Serialization
	}
}
