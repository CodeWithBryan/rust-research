using System;
using Rust.UI;
using UnityEngine;

// Token: 0x0200083C RID: 2108
public class CompanionSetupScreen : SingletonComponent<CompanionSetupScreen>
{
	// Token: 0x04002EF3 RID: 12019
	public const string PairedKey = "companionPaired";

	// Token: 0x04002EF4 RID: 12020
	public GameObject instructionsBody;

	// Token: 0x04002EF5 RID: 12021
	public GameObject detailsPanel;

	// Token: 0x04002EF6 RID: 12022
	public GameObject loadingMessage;

	// Token: 0x04002EF7 RID: 12023
	public GameObject errorMessage;

	// Token: 0x04002EF8 RID: 12024
	public GameObject notSupportedMessage;

	// Token: 0x04002EF9 RID: 12025
	public GameObject disabledMessage;

	// Token: 0x04002EFA RID: 12026
	public GameObject enabledMessage;

	// Token: 0x04002EFB RID: 12027
	public GameObject refreshButton;

	// Token: 0x04002EFC RID: 12028
	public GameObject enableButton;

	// Token: 0x04002EFD RID: 12029
	public GameObject disableButton;

	// Token: 0x04002EFE RID: 12030
	public GameObject pairButton;

	// Token: 0x04002EFF RID: 12031
	public RustText serverName;

	// Token: 0x04002F00 RID: 12032
	public RustButton helpButton;

	// Token: 0x02000E32 RID: 3634
	public enum ScreenState
	{
		// Token: 0x04004991 RID: 18833
		Loading,
		// Token: 0x04004992 RID: 18834
		Error,
		// Token: 0x04004993 RID: 18835
		NoServer,
		// Token: 0x04004994 RID: 18836
		NotSupported,
		// Token: 0x04004995 RID: 18837
		NotInstalled,
		// Token: 0x04004996 RID: 18838
		Disabled,
		// Token: 0x04004997 RID: 18839
		Enabled,
		// Token: 0x04004998 RID: 18840
		ShowHelp
	}
}
