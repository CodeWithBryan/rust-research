using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000425 RID: 1061
public class PlayerNameTag : MonoBehaviour
{
	// Token: 0x04001BF1 RID: 7153
	public CanvasGroup canvasGroup;

	// Token: 0x04001BF2 RID: 7154
	public Text text;

	// Token: 0x04001BF3 RID: 7155
	public Gradient color;

	// Token: 0x04001BF4 RID: 7156
	public float minDistance = 3f;

	// Token: 0x04001BF5 RID: 7157
	public float maxDistance = 10f;

	// Token: 0x04001BF6 RID: 7158
	public Vector3 positionOffset;

	// Token: 0x04001BF7 RID: 7159
	public Transform parentBone;
}
