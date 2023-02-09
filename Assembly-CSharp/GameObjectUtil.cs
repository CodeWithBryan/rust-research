using System;
using UnityEngine;

// Token: 0x02000904 RID: 2308
public static class GameObjectUtil
{
	// Token: 0x060036F0 RID: 14064 RVA: 0x00146B7C File Offset: 0x00144D7C
	public static void GlobalBroadcast(string messageName, object param = null)
	{
		Transform[] rootObjects = TransformUtil.GetRootObjects();
		for (int i = 0; i < rootObjects.Length; i++)
		{
			rootObjects[i].BroadcastMessage(messageName, param, SendMessageOptions.DontRequireReceiver);
		}
	}
}
