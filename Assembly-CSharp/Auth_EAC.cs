using System;
using System.Collections;
using Network;

// Token: 0x02000712 RID: 1810
public static class Auth_EAC
{
	// Token: 0x06003207 RID: 12807 RVA: 0x001336B8 File Offset: 0x001318B8
	public static IEnumerator Run(Connection connection)
	{
		if (!connection.active)
		{
			yield break;
		}
		if (connection.rejected)
		{
			yield break;
		}
		connection.authStatus = string.Empty;
		EACServer.OnJoinGame(connection);
		while (connection.active && !connection.rejected && connection.authStatus == string.Empty)
		{
			yield return null;
		}
		yield break;
	}
}
