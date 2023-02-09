using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020004A3 RID: 1187
public class ElectricGenerator : global::IOEntity
{
	// Token: 0x0600267C RID: 9852 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x0600267D RID: 9853 RVA: 0x000EF39B File Offset: 0x000ED59B
	public override int MaximalPowerOutput()
	{
		return Mathf.FloorToInt(this.electricAmount);
	}

	// Token: 0x0600267E RID: 9854 RVA: 0x00007074 File Offset: 0x00005274
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x0600267F RID: 9855 RVA: 0x000EF3A8 File Offset: 0x000ED5A8
	public override int GetCurrentEnergy()
	{
		return (int)this.electricAmount;
	}

	// Token: 0x06002680 RID: 9856 RVA: 0x000EF3B1 File Offset: 0x000ED5B1
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return this.GetCurrentEnergy();
	}

	// Token: 0x06002681 RID: 9857 RVA: 0x000EF3BC File Offset: 0x000ED5BC
	public override void UpdateOutputs()
	{
		this.currentEnergy = this.GetCurrentEnergy();
		foreach (global::IOEntity.IOSlot ioslot in this.outputs)
		{
			if (ioslot.connectedTo.Get(true) != null)
			{
				ioslot.connectedTo.Get(true).UpdateFromInput(this.currentEnergy, ioslot.connectedToSlot);
			}
		}
	}

	// Token: 0x06002682 RID: 9858 RVA: 0x00053CF9 File Offset: 0x00051EF9
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x06002683 RID: 9859 RVA: 0x000EF41F File Offset: 0x000ED61F
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.Invoke(new Action(this.ForcePuzzleReset), 1f);
	}

	// Token: 0x06002684 RID: 9860 RVA: 0x000EF440 File Offset: 0x000ED640
	private void ForcePuzzleReset()
	{
		global::PuzzleReset component = base.GetComponent<global::PuzzleReset>();
		if (component != null)
		{
			component.DoReset();
			component.ResetTimer();
		}
	}

	// Token: 0x06002685 RID: 9861 RVA: 0x000EF46C File Offset: 0x000ED66C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			global::PuzzleReset component = base.GetComponent<global::PuzzleReset>();
			if (component)
			{
				info.msg.puzzleReset = Pool.Get<ProtoBuf.PuzzleReset>();
				info.msg.puzzleReset.playerBlocksReset = component.playersBlockReset;
				if (component.playerDetectionOrigin != null)
				{
					info.msg.puzzleReset.playerDetectionOrigin = component.playerDetectionOrigin.position;
				}
				info.msg.puzzleReset.playerDetectionRadius = component.playerDetectionRadius;
				info.msg.puzzleReset.scaleWithServerPopulation = component.scaleWithServerPopulation;
				info.msg.puzzleReset.timeBetweenResets = component.timeBetweenResets;
			}
		}
	}

	// Token: 0x06002686 RID: 9862 RVA: 0x000EF530 File Offset: 0x000ED730
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.puzzleReset != null)
		{
			global::PuzzleReset component = base.GetComponent<global::PuzzleReset>();
			if (component != null)
			{
				component.playersBlockReset = info.msg.puzzleReset.playerBlocksReset;
				if (component.playerDetectionOrigin != null)
				{
					component.playerDetectionOrigin.position = info.msg.puzzleReset.playerDetectionOrigin;
				}
				component.playerDetectionRadius = info.msg.puzzleReset.playerDetectionRadius;
				component.scaleWithServerPopulation = info.msg.puzzleReset.scaleWithServerPopulation;
				component.timeBetweenResets = info.msg.puzzleReset.timeBetweenResets;
				component.ResetTimer();
			}
		}
	}

	// Token: 0x04001F21 RID: 7969
	public float electricAmount = 8f;
}
