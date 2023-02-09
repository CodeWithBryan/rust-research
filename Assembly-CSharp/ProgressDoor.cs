using System;
using UnityEngine;

// Token: 0x020004B3 RID: 1203
public class ProgressDoor : IOEntity
{
	// Token: 0x060026C5 RID: 9925 RVA: 0x000EFC6C File Offset: 0x000EDE6C
	public override void ResetIOState()
	{
		this.storedEnergy = 0f;
		this.UpdateProgress();
	}

	// Token: 0x060026C6 RID: 9926 RVA: 0x000EFC7F File Offset: 0x000EDE7F
	public override float IOInput(IOEntity from, IOEntity.IOType inputType, float inputAmount, int slot = 0)
	{
		if (inputAmount <= 0f)
		{
			this.NoEnergy();
			return inputAmount;
		}
		this.AddEnergy(inputAmount);
		if (this.storedEnergy == this.energyForOpen)
		{
			return inputAmount;
		}
		return 0f;
	}

	// Token: 0x060026C7 RID: 9927 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void NoEnergy()
	{
	}

	// Token: 0x060026C8 RID: 9928 RVA: 0x000EFCAD File Offset: 0x000EDEAD
	public virtual void AddEnergy(float amount)
	{
		if (amount <= 0f)
		{
			return;
		}
		this.storedEnergy += amount;
		this.storedEnergy = Mathf.Clamp(this.storedEnergy, 0f, this.energyForOpen);
	}

	// Token: 0x060026C9 RID: 9929 RVA: 0x00007338 File Offset: 0x00005538
	public virtual void UpdateProgress()
	{
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x04001F42 RID: 8002
	public float storedEnergy;

	// Token: 0x04001F43 RID: 8003
	public float energyForOpen = 1f;

	// Token: 0x04001F44 RID: 8004
	public float secondsToClose = 1f;

	// Token: 0x04001F45 RID: 8005
	public float openProgress;
}
