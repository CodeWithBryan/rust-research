using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200090E RID: 2318
public static class HierarchyUtil
{
	// Token: 0x0600370A RID: 14090 RVA: 0x001476E4 File Offset: 0x001458E4
	public static GameObject GetRoot(string strName, bool groupActive = true, bool persistant = false)
	{
		GameObject gameObject;
		if (HierarchyUtil.rootDict.TryGetValue(strName, out gameObject))
		{
			if (gameObject != null)
			{
				return gameObject;
			}
			HierarchyUtil.rootDict.Remove(strName);
		}
		gameObject = new GameObject(strName);
		gameObject.SetActive(groupActive);
		HierarchyUtil.rootDict.Add(strName, gameObject);
		if (persistant)
		{
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
		return gameObject;
	}

	// Token: 0x040031A5 RID: 12709
	public static Dictionary<string, GameObject> rootDict = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);
}
