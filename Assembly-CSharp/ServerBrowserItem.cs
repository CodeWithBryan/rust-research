using System;
using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000860 RID: 2144
public class ServerBrowserItem : MonoBehaviour
{
	// Token: 0x04002F91 RID: 12177
	public TextMeshProUGUI serverName;

	// Token: 0x04002F92 RID: 12178
	public RustFlexText mapName;

	// Token: 0x04002F93 RID: 12179
	public TextMeshProUGUI playerCount;

	// Token: 0x04002F94 RID: 12180
	public TextMeshProUGUI ping;

	// Token: 0x04002F95 RID: 12181
	public Toggle favourited;

	// Token: 0x04002F96 RID: 12182
	public ServerBrowserTagList serverTagList;
}
