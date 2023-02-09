using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000924 RID: 2340
[CreateAssetMenu(menuName = "Rust/Vehicles/WorldSpline Shared Data", fileName = "WorldSpline Prefab Shared Data")]
public class WorldSplineSharedData : ScriptableObject
{
	// Token: 0x060037DA RID: 14298 RVA: 0x0014AFB5 File Offset: 0x001491B5
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		WorldSplineSharedData.instance = Resources.Load<WorldSplineSharedData>("WorldSpline Prefab Shared Data");
	}

	// Token: 0x060037DB RID: 14299 RVA: 0x0014AFC8 File Offset: 0x001491C8
	public static bool TryGetDataFor(WorldSpline worldSpline, out WorldSplineData data)
	{
		if (WorldSplineSharedData.instance == null)
		{
			Debug.LogError("No instance of WorldSplineSharedData found.");
			data = null;
			return false;
		}
		if (worldSpline.dataIndex < 0 || worldSpline.dataIndex >= WorldSplineSharedData.instance.dataList.Count)
		{
			data = null;
			return false;
		}
		data = WorldSplineSharedData.instance.dataList[worldSpline.dataIndex];
		return true;
	}

	// Token: 0x040031F0 RID: 12784
	[SerializeField]
	private List<WorldSplineData> dataList;

	// Token: 0x040031F1 RID: 12785
	public static WorldSplineSharedData instance;

	// Token: 0x040031F2 RID: 12786
	private static string[] worldSplineFolders = new string[]
	{
		"Assets/Content/Structures",
		"Assets/bundled/Prefabs/autospawn"
	};
}
