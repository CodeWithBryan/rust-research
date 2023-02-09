using System;
using UnityEngine;

// Token: 0x02000539 RID: 1337
public class SceneToPrefabTag : MonoBehaviour, IEditorComponent
{
	// Token: 0x04002125 RID: 8485
	public SceneToPrefabTag.TagType Type;

	// Token: 0x04002126 RID: 8486
	public int SpecificLOD;

	// Token: 0x02000CF7 RID: 3319
	public enum TagType
	{
		// Token: 0x04004479 RID: 17529
		ForceInclude,
		// Token: 0x0400447A RID: 17530
		ForceExclude,
		// Token: 0x0400447B RID: 17531
		SingleMaterial,
		// Token: 0x0400447C RID: 17532
		UseSpecificLOD
	}
}
