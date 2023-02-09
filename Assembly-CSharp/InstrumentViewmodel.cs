using System;
using UnityEngine;

// Token: 0x0200012E RID: 302
public class InstrumentViewmodel : MonoBehaviour
{
	// Token: 0x060015E3 RID: 5603 RVA: 0x000A7F34 File Offset: 0x000A6134
	public void UpdateSlots(InstrumentKeyController.AnimationSlot currentSlot, bool recentlyPlayed, bool playedNoteThisFrame)
	{
		if (this.ViewAnimator == null)
		{
			return;
		}
		if (this.UpdateA)
		{
			this.UpdateState(this.note_a, currentSlot == InstrumentKeyController.AnimationSlot.One);
		}
		if (this.UpdateB)
		{
			this.UpdateState(this.note_b, currentSlot == InstrumentKeyController.AnimationSlot.Two);
		}
		if (this.UpdateC)
		{
			this.UpdateState(this.note_c, currentSlot == InstrumentKeyController.AnimationSlot.Three);
		}
		if (this.UpdateD)
		{
			this.UpdateState(this.note_d, currentSlot == InstrumentKeyController.AnimationSlot.Four);
		}
		if (this.UpdateE)
		{
			this.UpdateState(this.note_e, currentSlot == InstrumentKeyController.AnimationSlot.Five);
		}
		if (this.UpdateF)
		{
			this.UpdateState(this.note_f, currentSlot == InstrumentKeyController.AnimationSlot.Six);
		}
		if (this.UpdateG)
		{
			this.UpdateState(this.note_g, currentSlot == InstrumentKeyController.AnimationSlot.Seven);
		}
		if (this.UpdateRecentlyPlayed)
		{
			this.ViewAnimator.SetBool(this.recentlyPlayedHash, recentlyPlayed);
		}
		if (this.UpdatePlayedNoteTrigger && playedNoteThisFrame)
		{
			this.ViewAnimator.SetTrigger(this.playedNoteHash);
		}
	}

	// Token: 0x060015E4 RID: 5604 RVA: 0x000A802D File Offset: 0x000A622D
	private void UpdateState(int param, bool state)
	{
		if (!this.UseTriggers)
		{
			this.ViewAnimator.SetBool(param, state);
			return;
		}
		if (state)
		{
			this.ViewAnimator.SetTrigger(param);
		}
	}

	// Token: 0x04000E55 RID: 3669
	public Animator ViewAnimator;

	// Token: 0x04000E56 RID: 3670
	public bool UpdateA = true;

	// Token: 0x04000E57 RID: 3671
	public bool UpdateB = true;

	// Token: 0x04000E58 RID: 3672
	public bool UpdateC = true;

	// Token: 0x04000E59 RID: 3673
	public bool UpdateD = true;

	// Token: 0x04000E5A RID: 3674
	public bool UpdateE = true;

	// Token: 0x04000E5B RID: 3675
	public bool UpdateF = true;

	// Token: 0x04000E5C RID: 3676
	public bool UpdateG = true;

	// Token: 0x04000E5D RID: 3677
	public bool UpdateRecentlyPlayed = true;

	// Token: 0x04000E5E RID: 3678
	public bool UpdatePlayedNoteTrigger;

	// Token: 0x04000E5F RID: 3679
	public bool UseTriggers;

	// Token: 0x04000E60 RID: 3680
	private readonly int note_a = Animator.StringToHash("play_A");

	// Token: 0x04000E61 RID: 3681
	private readonly int note_b = Animator.StringToHash("play_B");

	// Token: 0x04000E62 RID: 3682
	private readonly int note_c = Animator.StringToHash("play_C");

	// Token: 0x04000E63 RID: 3683
	private readonly int note_d = Animator.StringToHash("play_D");

	// Token: 0x04000E64 RID: 3684
	private readonly int note_e = Animator.StringToHash("play_E");

	// Token: 0x04000E65 RID: 3685
	private readonly int note_f = Animator.StringToHash("play_F");

	// Token: 0x04000E66 RID: 3686
	private readonly int note_g = Animator.StringToHash("play_G");

	// Token: 0x04000E67 RID: 3687
	private readonly int recentlyPlayedHash = Animator.StringToHash("recentlyPlayed");

	// Token: 0x04000E68 RID: 3688
	private readonly int playedNoteHash = Animator.StringToHash("playedNote");
}
