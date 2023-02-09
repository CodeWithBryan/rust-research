using System;
using UnityEngine;

// Token: 0x020006FA RID: 1786
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Impostor : MonoBehaviour, IClientComponent
{
	// Token: 0x06003195 RID: 12693 RVA: 0x000059DD File Offset: 0x00003BDD
	private void OnEnable()
	{
	}

	// Token: 0x0400283B RID: 10299
	public ImpostorAsset asset;

	// Token: 0x0400283C RID: 10300
	[Header("Baking")]
	public GameObject reference;

	// Token: 0x0400283D RID: 10301
	public float angle;

	// Token: 0x0400283E RID: 10302
	public int resolution = 1024;

	// Token: 0x0400283F RID: 10303
	public int padding = 32;

	// Token: 0x04002840 RID: 10304
	public bool spriteOutlineAsMesh;
}
