using System;
using Rust.UI;
using UnityEngine.UI;

// Token: 0x0200085F RID: 2143
public class ServerBrowserInfo : SingletonComponent<ServerBrowserInfo>
{
	// Token: 0x04002F88 RID: 12168
	public bool isMain;

	// Token: 0x04002F89 RID: 12169
	public Text serverName;

	// Token: 0x04002F8A RID: 12170
	public Text serverMeta;

	// Token: 0x04002F8B RID: 12171
	public Text serverText;

	// Token: 0x04002F8C RID: 12172
	public Button viewWebpage;

	// Token: 0x04002F8D RID: 12173
	public Button refresh;

	// Token: 0x04002F8E RID: 12174
	public ServerInfo? currentServer;

	// Token: 0x04002F8F RID: 12175
	public HttpImage headerImage;

	// Token: 0x04002F90 RID: 12176
	public HttpImage logoImage;
}
