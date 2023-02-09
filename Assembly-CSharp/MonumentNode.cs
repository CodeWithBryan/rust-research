using System;
using UnityEngine;

// Token: 0x02000686 RID: 1670
public class MonumentNode : MonoBehaviour
{
	// Token: 0x06002FC5 RID: 12229 RVA: 0x0011D404 File Offset: 0x0011B604
	protected void Awake()
	{
		if (!(SingletonComponent<WorldSetup>.Instance == null))
		{
			if (SingletonComponent<WorldSetup>.Instance.MonumentNodes == null)
			{
				Debug.LogError("WorldSetup.Instance.MonumentNodes is null.", this);
				return;
			}
			SingletonComponent<WorldSetup>.Instance.MonumentNodes.Add(this);
		}
	}

	// Token: 0x06002FC6 RID: 12230 RVA: 0x0011D43C File Offset: 0x0011B63C
	public void Process(ref uint seed)
	{
		if (World.Networked)
		{
			World.Spawn("Monument", "assets/bundled/prefabs/autospawn/" + this.ResourceFolder + "/");
			return;
		}
		Prefab<MonumentInfo>[] array = Prefab.Load<MonumentInfo>("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, null, null, true);
		if (array == null || array.Length == 0)
		{
			return;
		}
		Prefab<MonumentInfo> random = array.GetRandom(ref seed);
		float height = TerrainMeta.HeightMap.GetHeight(base.transform.position);
		Vector3 position = new Vector3(base.transform.position.x, height, base.transform.position.z);
		Quaternion localRotation = random.Object.transform.localRotation;
		Vector3 localScale = random.Object.transform.localScale;
		random.ApplyDecorComponents(ref position, ref localRotation, ref localScale);
		World.AddPrefab("Monument", random, position, localRotation, localScale);
	}

	// Token: 0x04002687 RID: 9863
	public string ResourceFolder = string.Empty;
}
