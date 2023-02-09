using System;
using UnityEngine;

// Token: 0x020006E8 RID: 1768
[ExecuteInEditMode]
public class DeferredDecal : MonoBehaviour
{
	// Token: 0x04002811 RID: 10257
	public Mesh mesh;

	// Token: 0x04002812 RID: 10258
	public Material material;

	// Token: 0x04002813 RID: 10259
	public DeferredDecalQueue queue;

	// Token: 0x04002814 RID: 10260
	public bool applyImmediately = true;
}
