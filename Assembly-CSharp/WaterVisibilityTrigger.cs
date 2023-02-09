using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x020006DE RID: 1758
public class WaterVisibilityTrigger : EnvironmentVolumeTrigger
{
	// Token: 0x0600314E RID: 12622 RVA: 0x0012F894 File Offset: 0x0012DA94
	public static void Reset()
	{
		WaterVisibilityTrigger.ticks = 1L;
		WaterVisibilityTrigger.tracker.Clear();
	}

	// Token: 0x0600314F RID: 12623 RVA: 0x0012F8A7 File Offset: 0x0012DAA7
	protected void OnDestroy()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		WaterVisibilityTrigger.tracker.Remove(this.enteredTick);
	}

	// Token: 0x06003150 RID: 12624 RVA: 0x00007074 File Offset: 0x00005274
	private int GetVisibilityMask()
	{
		return 0;
	}

	// Token: 0x06003151 RID: 12625 RVA: 0x000059DD File Offset: 0x00003BDD
	private void ToggleVisibility()
	{
	}

	// Token: 0x06003152 RID: 12626 RVA: 0x000059DD File Offset: 0x00003BDD
	private void ResetVisibility()
	{
	}

	// Token: 0x06003153 RID: 12627 RVA: 0x0012F8C2 File Offset: 0x0012DAC2
	private void ToggleCollision(Collider other)
	{
		if (this.togglePhysics && WaterSystem.Collision != null)
		{
			WaterSystem.Collision.SetIgnore(other, base.volume.trigger, true);
		}
	}

	// Token: 0x06003154 RID: 12628 RVA: 0x0012F8F0 File Offset: 0x0012DAF0
	private void ResetCollision(Collider other)
	{
		if (this.togglePhysics && WaterSystem.Collision != null)
		{
			WaterSystem.Collision.SetIgnore(other, base.volume.trigger, false);
		}
	}

	// Token: 0x06003155 RID: 12629 RVA: 0x0012F920 File Offset: 0x0012DB20
	protected void OnTriggerEnter(Collider other)
	{
		bool flag = other.gameObject.GetComponent<PlayerWalkMovement>() != null;
		bool flag2 = other.gameObject.CompareTag("MainCamera");
		if ((flag || flag2) && !WaterVisibilityTrigger.tracker.ContainsValue(this))
		{
			long num = WaterVisibilityTrigger.ticks;
			WaterVisibilityTrigger.ticks = num + 1L;
			this.enteredTick = num;
			WaterVisibilityTrigger.tracker.Add(this.enteredTick, this);
			this.ToggleVisibility();
		}
		if (!flag2 && !other.isTrigger)
		{
			this.ToggleCollision(other);
		}
	}

	// Token: 0x06003156 RID: 12630 RVA: 0x0012F9A0 File Offset: 0x0012DBA0
	protected void OnTriggerExit(Collider other)
	{
		bool flag = other.gameObject.GetComponent<PlayerWalkMovement>() != null;
		bool flag2 = other.gameObject.CompareTag("MainCamera");
		if ((flag || flag2) && WaterVisibilityTrigger.tracker.ContainsValue(this))
		{
			WaterVisibilityTrigger.tracker.Remove(this.enteredTick);
			if (WaterVisibilityTrigger.tracker.Count > 0)
			{
				WaterVisibilityTrigger.tracker.Values[WaterVisibilityTrigger.tracker.Count - 1].ToggleVisibility();
			}
			else
			{
				this.ResetVisibility();
			}
		}
		if (!flag2 && !other.isTrigger)
		{
			this.ResetCollision(other);
		}
	}

	// Token: 0x040027F3 RID: 10227
	public bool togglePhysics = true;

	// Token: 0x040027F4 RID: 10228
	public bool toggleVisuals = true;

	// Token: 0x040027F5 RID: 10229
	private long enteredTick;

	// Token: 0x040027F6 RID: 10230
	private static long ticks = 1L;

	// Token: 0x040027F7 RID: 10231
	private static SortedList<long, WaterVisibilityTrigger> tracker = new SortedList<long, WaterVisibilityTrigger>();
}
