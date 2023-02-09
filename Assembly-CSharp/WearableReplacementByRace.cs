using System;
using UnityEngine;

// Token: 0x0200057C RID: 1404
public class WearableReplacementByRace : MonoBehaviour
{
	// Token: 0x06002A5D RID: 10845 RVA: 0x001002F8 File Offset: 0x000FE4F8
	public GameObjectRef GetReplacement(int meshIndex)
	{
		int num = Mathf.Clamp(meshIndex, 0, this.replacements.Length - 1);
		return this.replacements[num];
	}

	// Token: 0x04002237 RID: 8759
	public GameObjectRef[] replacements;
}
