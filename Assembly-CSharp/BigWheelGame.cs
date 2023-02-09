using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200012F RID: 303
public class BigWheelGame : SpinnerWheel
{
	// Token: 0x060015E6 RID: 5606 RVA: 0x00007074 File Offset: 0x00005274
	public override bool AllowPlayerSpins()
	{
		return false;
	}

	// Token: 0x060015E7 RID: 5607 RVA: 0x00007074 File Offset: 0x00005274
	public override bool CanUpdateSign(BasePlayer player)
	{
		return false;
	}

	// Token: 0x060015E8 RID: 5608 RVA: 0x000A812F File Offset: 0x000A632F
	public override float GetMaxSpinSpeed()
	{
		return 180f;
	}

	// Token: 0x060015E9 RID: 5609 RVA: 0x000A8136 File Offset: 0x000A6336
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.InitBettingTerminals), 3f);
		base.Invoke(new Action(this.DoSpin), 10f);
	}

	// Token: 0x060015EA RID: 5610 RVA: 0x000A816C File Offset: 0x000A636C
	public void DoSpin()
	{
		if (this.velocity > 0f)
		{
			return;
		}
		this.velocity += UnityEngine.Random.Range(7f, 10f);
		this.spinNumber++;
		this.SetTerminalsLocked(true);
	}

	// Token: 0x060015EB RID: 5611 RVA: 0x000A81B8 File Offset: 0x000A63B8
	public void SetTerminalsLocked(bool isLocked)
	{
		foreach (BigWheelBettingTerminal bigWheelBettingTerminal in this.terminals)
		{
			bigWheelBettingTerminal.inventory.SetLocked(isLocked);
		}
	}

	// Token: 0x060015EC RID: 5612 RVA: 0x000A8210 File Offset: 0x000A6410
	public void RemoveTerminal(BigWheelBettingTerminal terminal)
	{
		this.terminals.Remove(terminal);
	}

	// Token: 0x060015ED RID: 5613 RVA: 0x000A8220 File Offset: 0x000A6420
	protected void InitBettingTerminals()
	{
		this.terminals.Clear();
		Vis.Entities<BigWheelBettingTerminal>(base.transform.position, 30f, this.terminals, 256, QueryTriggerInteraction.Collide);
		this.terminals = this.terminals.Distinct<BigWheelBettingTerminal>().ToList<BigWheelBettingTerminal>();
	}

	// Token: 0x060015EE RID: 5614 RVA: 0x000A8270 File Offset: 0x000A6470
	public override void Update_Server()
	{
		float velocity = this.velocity;
		base.Update_Server();
		float velocity2 = this.velocity;
		if (velocity > 0f && velocity2 == 0f && this.spinNumber > this.lastPaidSpinNumber)
		{
			this.Payout();
			this.lastPaidSpinNumber = this.spinNumber;
			this.QueueSpin();
		}
	}

	// Token: 0x060015EF RID: 5615 RVA: 0x000A82C5 File Offset: 0x000A64C5
	public float SpinSpacing()
	{
		return BigWheelGame.spinFrequencySeconds;
	}

	// Token: 0x060015F0 RID: 5616 RVA: 0x000A82CC File Offset: 0x000A64CC
	public void QueueSpin()
	{
		foreach (BigWheelBettingTerminal bigWheelBettingTerminal in this.terminals)
		{
			bigWheelBettingTerminal.ClientRPC<float>(null, "SetTimeUntilNextSpin", this.SpinSpacing());
		}
		base.Invoke(new Action(this.DoSpin), this.SpinSpacing());
	}

	// Token: 0x060015F1 RID: 5617 RVA: 0x000A8340 File Offset: 0x000A6540
	public void Payout()
	{
		HitNumber currentHitType = this.GetCurrentHitType();
		foreach (BigWheelBettingTerminal bigWheelBettingTerminal in this.terminals)
		{
			if (!bigWheelBettingTerminal.isClient)
			{
				bool flag = false;
				bool flag2 = false;
				Item slot = bigWheelBettingTerminal.inventory.GetSlot((int)currentHitType.hitType);
				if (slot != null)
				{
					int num = currentHitType.ColorToMultiplier(currentHitType.hitType);
					slot.amount += slot.amount * num;
					slot.RemoveFromContainer();
					slot.MoveToContainer(bigWheelBettingTerminal.inventory, 5, true, false, null, true);
					flag = true;
				}
				for (int i = 0; i < 5; i++)
				{
					Item slot2 = bigWheelBettingTerminal.inventory.GetSlot(i);
					if (slot2 != null)
					{
						slot2.Remove(0f);
						flag2 = true;
					}
				}
				if (flag || flag2)
				{
					bigWheelBettingTerminal.ClientRPC<bool>(null, "WinOrLoseSound", flag);
				}
			}
		}
		ItemManager.DoRemoves();
		this.SetTerminalsLocked(false);
	}

	// Token: 0x060015F2 RID: 5618 RVA: 0x000A8454 File Offset: 0x000A6654
	public HitNumber GetCurrentHitType()
	{
		HitNumber result = null;
		float num = float.PositiveInfinity;
		foreach (HitNumber hitNumber in this.hitNumbers)
		{
			float num2 = Vector3.Distance(this.indicator.transform.position, hitNumber.transform.position);
			if (num2 < num)
			{
				result = hitNumber;
				num = num2;
			}
		}
		return result;
	}

	// Token: 0x060015F3 RID: 5619 RVA: 0x000A84B4 File Offset: 0x000A66B4
	[ContextMenu("LoadHitNumbers")]
	private void LoadHitNumbers()
	{
		HitNumber[] componentsInChildren = base.GetComponentsInChildren<HitNumber>();
		this.hitNumbers = componentsInChildren;
	}

	// Token: 0x04000E69 RID: 3689
	public HitNumber[] hitNumbers;

	// Token: 0x04000E6A RID: 3690
	public GameObject indicator;

	// Token: 0x04000E6B RID: 3691
	public GameObjectRef winEffect;

	// Token: 0x04000E6C RID: 3692
	[ServerVar]
	public static float spinFrequencySeconds = 45f;

	// Token: 0x04000E6D RID: 3693
	protected int spinNumber;

	// Token: 0x04000E6E RID: 3694
	protected int lastPaidSpinNumber = -1;

	// Token: 0x04000E6F RID: 3695
	protected List<BigWheelBettingTerminal> terminals = new List<BigWheelBettingTerminal>();
}
