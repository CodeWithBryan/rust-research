using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// Token: 0x02000740 RID: 1856
[Serializable]
public class TimelineConvarController : PlayableAsset, ITimelineClipAsset
{
	// Token: 0x0600331B RID: 13083 RVA: 0x0013B368 File Offset: 0x00139568
	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
	{
		ScriptPlayable<TimelineConvarPlayable> playable = ScriptPlayable<TimelineConvarPlayable>.Create(graph, this.template, 0);
		playable.GetBehaviour().convar = this.convarName;
		return playable;
	}

	// Token: 0x170003F7 RID: 1015
	// (get) Token: 0x0600331C RID: 13084 RVA: 0x0004AF67 File Offset: 0x00049167
	public ClipCaps clipCaps
	{
		get
		{
			return ClipCaps.Extrapolation;
		}
	}

	// Token: 0x04002997 RID: 10647
	public string convarName = string.Empty;

	// Token: 0x04002998 RID: 10648
	public TimelineConvarPlayable template = new TimelineConvarPlayable();
}
