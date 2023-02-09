using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200086A RID: 2154
public class MissionsHUD : SingletonComponent<MissionsHUD>
{
	// Token: 0x04002FC2 RID: 12226
	public SoundDefinition listComplete;

	// Token: 0x04002FC3 RID: 12227
	public SoundDefinition itemComplete;

	// Token: 0x04002FC4 RID: 12228
	public SoundDefinition popup;

	// Token: 0x04002FC5 RID: 12229
	public Canvas Canvas;

	// Token: 0x04002FC6 RID: 12230
	public Text titleText;

	// Token: 0x04002FC7 RID: 12231
	public GameObject timerObject;

	// Token: 0x04002FC8 RID: 12232
	public RustText timerText;
}
