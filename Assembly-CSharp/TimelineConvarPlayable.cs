using System;
using UnityEngine.Playables;

// Token: 0x02000741 RID: 1857
[Serializable]
public class TimelineConvarPlayable : PlayableBehaviour
{
	// Token: 0x0600331E RID: 13086 RVA: 0x000059DD File Offset: 0x00003BDD
	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
	}

	// Token: 0x04002999 RID: 10649
	[NonSerialized]
	public string convar;

	// Token: 0x0400299A RID: 10650
	public float ConvarValue;
}
