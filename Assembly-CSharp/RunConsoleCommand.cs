using System;
using UnityEngine;

// Token: 0x020008E4 RID: 2276
public class RunConsoleCommand : MonoBehaviour
{
	// Token: 0x06003680 RID: 13952 RVA: 0x001444EA File Offset: 0x001426EA
	public void ClientRun(string command)
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, command, Array.Empty<object>());
	}
}
