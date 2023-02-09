using System;
using UnityEngine;

// Token: 0x020004B5 RID: 1205
public class NoPlayersIOReset : FacepunchBehaviour
{
	// Token: 0x060026D3 RID: 9939 RVA: 0x000EFF9F File Offset: 0x000EE19F
	protected void OnEnable()
	{
		base.InvokeRandomized(new Action(this.Check), this.timeBetweenChecks, this.timeBetweenChecks, this.timeBetweenChecks * 0.1f);
	}

	// Token: 0x060026D4 RID: 9940 RVA: 0x000EFFCB File Offset: 0x000EE1CB
	protected void OnDisable()
	{
		base.CancelInvoke(new Action(this.Check));
	}

	// Token: 0x060026D5 RID: 9941 RVA: 0x000EFFDF File Offset: 0x000EE1DF
	private void Check()
	{
		if (!PuzzleReset.AnyPlayersWithinDistance(base.transform, this.radius))
		{
			this.Reset();
		}
	}

	// Token: 0x060026D6 RID: 9942 RVA: 0x000EFFFC File Offset: 0x000EE1FC
	private void Reset()
	{
		foreach (IOEntity ioentity in this.entitiesToReset)
		{
			if (ioentity.IsValid() && ioentity.isServer)
			{
				ioentity.ResetIOState();
				ioentity.MarkDirty();
			}
		}
	}

	// Token: 0x04001F4C RID: 8012
	[SerializeField]
	private IOEntity[] entitiesToReset;

	// Token: 0x04001F4D RID: 8013
	[SerializeField]
	private float radius;

	// Token: 0x04001F4E RID: 8014
	[SerializeField]
	private float timeBetweenChecks;
}
