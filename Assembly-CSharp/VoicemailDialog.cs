using System;
using Rust.UI;
using UnityEngine;

// Token: 0x020007A0 RID: 1952
public class VoicemailDialog : MonoBehaviour
{
	// Token: 0x04002B44 RID: 11076
	public GameObject RecordingRoot;

	// Token: 0x04002B45 RID: 11077
	public RustSlider RecordingProgress;

	// Token: 0x04002B46 RID: 11078
	public GameObject BrowsingRoot;

	// Token: 0x04002B47 RID: 11079
	public PhoneDialler ParentDialler;

	// Token: 0x04002B48 RID: 11080
	public GameObjectRef VoicemailEntry;

	// Token: 0x04002B49 RID: 11081
	public Transform VoicemailEntriesRoot;

	// Token: 0x04002B4A RID: 11082
	public GameObject NoVoicemailRoot;

	// Token: 0x04002B4B RID: 11083
	public GameObject NoCassetteRoot;
}
