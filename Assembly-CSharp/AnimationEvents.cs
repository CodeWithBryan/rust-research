using System;
using UnityEngine;

// Token: 0x02000311 RID: 785
public class AnimationEvents : BaseMonoBehaviour
{
	// Token: 0x06001DBD RID: 7613 RVA: 0x000CB016 File Offset: 0x000C9216
	protected void OnEnable()
	{
		if (this.rootObject == null)
		{
			this.rootObject = base.transform;
		}
	}

	// Token: 0x04001724 RID: 5924
	public Transform rootObject;

	// Token: 0x04001725 RID: 5925
	public HeldEntity targetEntity;

	// Token: 0x04001726 RID: 5926
	[Tooltip("Path to the effect folder for these animations. Relative to this object.")]
	public string effectFolder;

	// Token: 0x04001727 RID: 5927
	public bool enforceClipWeights;

	// Token: 0x04001728 RID: 5928
	public string localFolder;

	// Token: 0x04001729 RID: 5929
	[Tooltip("If true the localFolder field won't update with manifest updates, use for custom paths")]
	public bool customLocalFolder;

	// Token: 0x0400172A RID: 5930
	public bool IsBusy;
}
