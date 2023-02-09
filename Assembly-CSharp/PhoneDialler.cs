using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200079E RID: 1950
public class PhoneDialler : UIDialog
{
	// Token: 0x04002B1F RID: 11039
	public GameObject DialingRoot;

	// Token: 0x04002B20 RID: 11040
	public GameObject CallInProcessRoot;

	// Token: 0x04002B21 RID: 11041
	public GameObject IncomingCallRoot;

	// Token: 0x04002B22 RID: 11042
	public RustText ThisPhoneNumber;

	// Token: 0x04002B23 RID: 11043
	public RustInput PhoneNameInput;

	// Token: 0x04002B24 RID: 11044
	public RustText textDisplay;

	// Token: 0x04002B25 RID: 11045
	public RustText CallTimeText;

	// Token: 0x04002B26 RID: 11046
	public RustButton DefaultDialViewButton;

	// Token: 0x04002B27 RID: 11047
	public RustText[] IncomingCallNumber;

	// Token: 0x04002B28 RID: 11048
	public GameObject NumberDialRoot;

	// Token: 0x04002B29 RID: 11049
	public GameObject PromptVoicemailRoot;

	// Token: 0x04002B2A RID: 11050
	public RustButton ContactsButton;

	// Token: 0x04002B2B RID: 11051
	public RustText FailText;

	// Token: 0x04002B2C RID: 11052
	public NeedsCursor CursorController;

	// Token: 0x04002B2D RID: 11053
	public NeedsKeyboard KeyboardController;

	// Token: 0x04002B2E RID: 11054
	public Translate.Phrase WrongNumberPhrase;

	// Token: 0x04002B2F RID: 11055
	public Translate.Phrase NetworkBusy;

	// Token: 0x04002B30 RID: 11056
	public Translate.Phrase Engaged;

	// Token: 0x04002B31 RID: 11057
	public GameObjectRef DirectoryEntryPrefab;

	// Token: 0x04002B32 RID: 11058
	public Transform DirectoryRoot;

	// Token: 0x04002B33 RID: 11059
	public GameObject NoDirectoryRoot;

	// Token: 0x04002B34 RID: 11060
	public RustButton DirectoryPageUp;

	// Token: 0x04002B35 RID: 11061
	public RustButton DirectoryPageDown;

	// Token: 0x04002B36 RID: 11062
	public Transform ContactsRoot;

	// Token: 0x04002B37 RID: 11063
	public RustInput ContactsNameInput;

	// Token: 0x04002B38 RID: 11064
	public RustInput ContactsNumberInput;

	// Token: 0x04002B39 RID: 11065
	public GameObject NoContactsRoot;

	// Token: 0x04002B3A RID: 11066
	public RustButton AddContactButton;

	// Token: 0x04002B3B RID: 11067
	public SoundDefinition DialToneSfx;

	// Token: 0x04002B3C RID: 11068
	public Button[] NumberButtons;

	// Token: 0x04002B3D RID: 11069
	public Translate.Phrase AnsweringMachine;

	// Token: 0x04002B3E RID: 11070
	public VoicemailDialog Voicemail;

	// Token: 0x04002B3F RID: 11071
	public GameObject VoicemailRoot;
}
