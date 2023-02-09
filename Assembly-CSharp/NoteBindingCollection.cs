using System;
using Rust.Instruments;
using UnityEngine;

// Token: 0x020003DB RID: 987
[CreateAssetMenu]
public class NoteBindingCollection : ScriptableObject
{
	// Token: 0x06002187 RID: 8583 RVA: 0x000D7778 File Offset: 0x000D5978
	public bool FindNoteData(Notes note, int octave, InstrumentKeyController.NoteType type, out NoteBindingCollection.NoteData data, out int noteIndex)
	{
		for (int i = 0; i < this.BaseBindings.Length; i++)
		{
			NoteBindingCollection.NoteData noteData = this.BaseBindings[i];
			if (noteData.Note == note && noteData.Type == type && noteData.NoteOctave == octave)
			{
				data = noteData;
				noteIndex = i;
				return true;
			}
		}
		data = default(NoteBindingCollection.NoteData);
		noteIndex = -1;
		return false;
	}

	// Token: 0x06002188 RID: 8584 RVA: 0x000D77DC File Offset: 0x000D59DC
	public bool FindNoteDataIndex(Notes note, int octave, InstrumentKeyController.NoteType type, out int noteIndex)
	{
		for (int i = 0; i < this.BaseBindings.Length; i++)
		{
			NoteBindingCollection.NoteData noteData = this.BaseBindings[i];
			if (noteData.Note == note && noteData.Type == type && noteData.NoteOctave == octave)
			{
				noteIndex = i;
				return true;
			}
		}
		noteIndex = -1;
		return false;
	}

	// Token: 0x06002189 RID: 8585 RVA: 0x000D7830 File Offset: 0x000D5A30
	public NoteBindingCollection.NoteData CreateMidiBinding(NoteBindingCollection.NoteData basedOn, int octave, int midiCode)
	{
		NoteBindingCollection.NoteData result = basedOn;
		result.NoteOctave = octave;
		result.MidiNoteNumber = midiCode;
		int num = octave - basedOn.NoteOctave;
		if (octave > basedOn.NoteOctave)
		{
			result.PitchOffset = (float)num * 2f;
		}
		else
		{
			result.PitchOffset = 1f - Mathf.Abs((float)num * 0.1f);
		}
		return result;
	}

	// Token: 0x040019D7 RID: 6615
	public NoteBindingCollection.NoteData[] BaseBindings;

	// Token: 0x040019D8 RID: 6616
	public float MinimumNoteTime;

	// Token: 0x040019D9 RID: 6617
	public float MaximumNoteLength;

	// Token: 0x040019DA RID: 6618
	public bool AllowAutoplay = true;

	// Token: 0x040019DB RID: 6619
	public float AutoplayLoopDelay = 0.25f;

	// Token: 0x040019DC RID: 6620
	public string NotePlayedStatName;

	// Token: 0x040019DD RID: 6621
	public string KeyMidiMapShortname = "";

	// Token: 0x040019DE RID: 6622
	public bool AllowSustain;

	// Token: 0x040019DF RID: 6623
	public bool AllowFullKeyboardInput = true;

	// Token: 0x040019E0 RID: 6624
	public string InstrumentShortName = "";

	// Token: 0x040019E1 RID: 6625
	public InstrumentKeyController.InstrumentType NotePlayType;

	// Token: 0x040019E2 RID: 6626
	public int MaxConcurrentNotes = 3;

	// Token: 0x040019E3 RID: 6627
	public bool LoopSounds;

	// Token: 0x040019E4 RID: 6628
	public float SoundFadeInTime;

	// Token: 0x040019E5 RID: 6629
	public float minimumSoundFadeOutTime = 0.1f;

	// Token: 0x040019E6 RID: 6630
	public InstrumentKeyController.KeySet PrimaryClickNote;

	// Token: 0x040019E7 RID: 6631
	public InstrumentKeyController.KeySet SecondaryClickNote = new InstrumentKeyController.KeySet
	{
		Note = Notes.B
	};

	// Token: 0x040019E8 RID: 6632
	public bool RunInstrumentAnimationController;

	// Token: 0x040019E9 RID: 6633
	public bool PlayRepeatAnimations = true;

	// Token: 0x040019EA RID: 6634
	public float AnimationDeadTime = 1f;

	// Token: 0x040019EB RID: 6635
	public float AnimationResetDelay;

	// Token: 0x040019EC RID: 6636
	public float RecentlyPlayedThreshold = 1f;

	// Token: 0x040019ED RID: 6637
	[Range(0f, 1f)]
	public float CrossfadeNormalizedAnimationTarget;

	// Token: 0x040019EE RID: 6638
	public float AnimationCrossfadeDuration = 0.15f;

	// Token: 0x040019EF RID: 6639
	public float CrossfadePlayerSpeedMulti = 1f;

	// Token: 0x040019F0 RID: 6640
	public int DefaultOctave;

	// Token: 0x040019F1 RID: 6641
	public int ShiftedOctave = 1;

	// Token: 0x040019F2 RID: 6642
	public bool UseClosestMidiNote = true;

	// Token: 0x040019F3 RID: 6643
	private const float MidiNoteUpOctaveShift = 2f;

	// Token: 0x040019F4 RID: 6644
	private const float MidiNoteDownOctaveShift = 0.1f;

	// Token: 0x02000C7F RID: 3199
	[Serializable]
	public struct NoteData
	{
		// Token: 0x06004CFD RID: 19709 RVA: 0x00196C68 File Offset: 0x00194E68
		public bool MatchMidiCode(int code)
		{
			if (this.MidiNoteNumber == code)
			{
				return true;
			}
			if (this.AdditionalMidiTargets != null)
			{
				int[] additionalMidiTargets = this.AdditionalMidiTargets;
				for (int i = 0; i < additionalMidiTargets.Length; i++)
				{
					if (additionalMidiTargets[i] == code)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06004CFE RID: 19710 RVA: 0x00196CA6 File Offset: 0x00194EA6
		public string ToNoteString()
		{
			return string.Format("{0}{1}{2}", this.Note, (this.Type == InstrumentKeyController.NoteType.Sharp) ? "#" : string.Empty, this.NoteOctave);
		}

		// Token: 0x04004284 RID: 17028
		public SoundDefinition NoteSound;

		// Token: 0x04004285 RID: 17029
		public SoundDefinition NoteStartSound;

		// Token: 0x04004286 RID: 17030
		public Notes Note;

		// Token: 0x04004287 RID: 17031
		public InstrumentKeyController.NoteType Type;

		// Token: 0x04004288 RID: 17032
		public int MidiNoteNumber;

		// Token: 0x04004289 RID: 17033
		public int NoteOctave;

		// Token: 0x0400428A RID: 17034
		[InstrumentIKTarget]
		public InstrumentKeyController.IKNoteTarget NoteIKTarget;

		// Token: 0x0400428B RID: 17035
		public InstrumentKeyController.AnimationSlot AnimationSlot;

		// Token: 0x0400428C RID: 17036
		public int NoteSoundPositionTarget;

		// Token: 0x0400428D RID: 17037
		public int[] AdditionalMidiTargets;

		// Token: 0x0400428E RID: 17038
		public float PitchOffset;
	}
}
