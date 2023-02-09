using System;
using UnityEngine;

// Token: 0x02000538 RID: 1336
public class SceneToPrefab : MonoBehaviour, IEditorComponent
{
	// Token: 0x04002122 RID: 8482
	public bool flattenHierarchy;

	// Token: 0x04002123 RID: 8483
	public GameObject outputPrefab;

	// Token: 0x04002124 RID: 8484
	[Tooltip("If true the HLOD generation will be skipped and the previous results will be used, good to use if non-visual changes were made (eg.triggers)")]
	public bool skipAllHlod;
}
