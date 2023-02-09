using System;
using Rust.Instruments;
using UnityEngine;

// Token: 0x020003D7 RID: 983
public class InstrumentDebugInput : MonoBehaviour
{
	// Token: 0x040019B6 RID: 6582
	public InstrumentKeyController KeyController;

	// Token: 0x040019B7 RID: 6583
	public InstrumentKeyController.KeySet Note = new InstrumentKeyController.KeySet
	{
		Note = Notes.A,
		NoteType = InstrumentKeyController.NoteType.Regular,
		OctaveShift = 3
	};

	// Token: 0x040019B8 RID: 6584
	public float Frequency = 0.75f;

	// Token: 0x040019B9 RID: 6585
	public float StopAfter = 0.1f;

	// Token: 0x040019BA RID: 6586
	public SoundDefinition OverrideDefinition;
}
