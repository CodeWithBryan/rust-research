using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004E6 RID: 1254
[CreateAssetMenu(menuName = "Rust/Game Mode Manifest")]
public class GameModeManifest : ScriptableObject
{
	// Token: 0x060027DB RID: 10203 RVA: 0x000F4467 File Offset: 0x000F2667
	public static GameModeManifest Get()
	{
		if (GameModeManifest.instance == null)
		{
			GameModeManifest.instance = Resources.Load<GameModeManifest>("GameModeManifest");
		}
		return GameModeManifest.instance;
	}

	// Token: 0x04002019 RID: 8217
	public static GameModeManifest instance;

	// Token: 0x0400201A RID: 8218
	public List<GameObjectRef> gameModePrefabs;
}
