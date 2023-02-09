using System;
using Rust.Instruments;
using UnityEngine;

// Token: 0x020003DA RID: 986
public class InstrumentKeyController : MonoBehaviour
{
	// Token: 0x17000299 RID: 665
	// (get) Token: 0x06002183 RID: 8579 RVA: 0x000D771C File Offset: 0x000D591C
	// (set) Token: 0x06002184 RID: 8580 RVA: 0x000D7724 File Offset: 0x000D5924
	public bool PlayedNoteThisFrame { get; private set; }

	// Token: 0x06002185 RID: 8581 RVA: 0x000D772D File Offset: 0x000D592D
	public void ProcessServerPlayedNote(BasePlayer forPlayer)
	{
		if (forPlayer == null)
		{
			return;
		}
		forPlayer.stats.Add(this.Bindings.NotePlayedStatName, 1, (Stats)5);
		forPlayer.stats.Add("played_notes", 1, (Stats)5);
	}

	// Token: 0x040019CB RID: 6603
	public const float DEFAULT_NOTE_VELOCITY = 1f;

	// Token: 0x040019CC RID: 6604
	public NoteBindingCollection Bindings;

	// Token: 0x040019CD RID: 6605
	public InstrumentKeyController.NoteBinding[] NoteBindings = new InstrumentKeyController.NoteBinding[0];

	// Token: 0x040019CE RID: 6606
	public Transform[] NoteSoundPositions;

	// Token: 0x040019CF RID: 6607
	public InstrumentIKController IKController;

	// Token: 0x040019D0 RID: 6608
	public Transform LeftHandProp;

	// Token: 0x040019D1 RID: 6609
	public Transform RightHandProp;

	// Token: 0x040019D2 RID: 6610
	public Animator InstrumentAnimator;

	// Token: 0x040019D3 RID: 6611
	public BaseEntity RPCHandler;

	// Token: 0x040019D4 RID: 6612
	public uint overrideAchievementId;

	// Token: 0x040019D6 RID: 6614
	private const string ALL_NOTES_STATNAME = "played_notes";

	// Token: 0x02000C77 RID: 3191
	public struct NoteBinding
	{
	}

	// Token: 0x02000C78 RID: 3192
	public enum IKType
	{
		// Token: 0x0400426B RID: 17003
		LeftHand,
		// Token: 0x0400426C RID: 17004
		RightHand,
		// Token: 0x0400426D RID: 17005
		RightFoot
	}

	// Token: 0x02000C79 RID: 3193
	public enum NoteType
	{
		// Token: 0x0400426F RID: 17007
		Regular,
		// Token: 0x04004270 RID: 17008
		Sharp
	}

	// Token: 0x02000C7A RID: 3194
	public enum InstrumentType
	{
		// Token: 0x04004272 RID: 17010
		Note,
		// Token: 0x04004273 RID: 17011
		Hold
	}

	// Token: 0x02000C7B RID: 3195
	public enum AnimationSlot
	{
		// Token: 0x04004275 RID: 17013
		None,
		// Token: 0x04004276 RID: 17014
		One,
		// Token: 0x04004277 RID: 17015
		Two,
		// Token: 0x04004278 RID: 17016
		Three,
		// Token: 0x04004279 RID: 17017
		Four,
		// Token: 0x0400427A RID: 17018
		Five,
		// Token: 0x0400427B RID: 17019
		Six,
		// Token: 0x0400427C RID: 17020
		Seven
	}

	// Token: 0x02000C7C RID: 3196
	[Serializable]
	public struct KeySet
	{
		// Token: 0x06004CFC RID: 19708 RVA: 0x00196C2F File Offset: 0x00194E2F
		public override string ToString()
		{
			return string.Format("{0}{1}{2}", this.Note, (this.NoteType == InstrumentKeyController.NoteType.Sharp) ? "#" : string.Empty, this.OctaveShift);
		}

		// Token: 0x0400427D RID: 17021
		public Notes Note;

		// Token: 0x0400427E RID: 17022
		public InstrumentKeyController.NoteType NoteType;

		// Token: 0x0400427F RID: 17023
		public int OctaveShift;
	}

	// Token: 0x02000C7D RID: 3197
	public struct NoteOverride
	{
		// Token: 0x04004280 RID: 17024
		public bool Override;

		// Token: 0x04004281 RID: 17025
		public InstrumentKeyController.KeySet Note;
	}

	// Token: 0x02000C7E RID: 3198
	[Serializable]
	public struct IKNoteTarget
	{
		// Token: 0x04004282 RID: 17026
		public InstrumentKeyController.IKType TargetType;

		// Token: 0x04004283 RID: 17027
		public int IkIndex;
	}
}
