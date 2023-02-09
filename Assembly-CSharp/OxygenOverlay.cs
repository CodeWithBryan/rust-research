using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000281 RID: 641
public class OxygenOverlay : MonoBehaviour
{
	// Token: 0x0400151E RID: 5406
	[SerializeField]
	private PostProcessVolume postProcessVolume;

	// Token: 0x0400151F RID: 5407
	[SerializeField]
	private float smoothTime = 1f;

	// Token: 0x04001520 RID: 5408
	[Tooltip("If true, only show this effect when the player is mounted in a submarine.")]
	[SerializeField]
	private bool submarinesOnly;
}
