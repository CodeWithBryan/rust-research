using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200083D RID: 2109
public class ConnectionScreen : SingletonComponent<ConnectionScreen>
{
	// Token: 0x04002F01 RID: 12033
	public Text statusText;

	// Token: 0x04002F02 RID: 12034
	public GameObject disconnectButton;

	// Token: 0x04002F03 RID: 12035
	public GameObject retryButton;

	// Token: 0x04002F04 RID: 12036
	public ServerBrowserInfo browserInfo;
}
