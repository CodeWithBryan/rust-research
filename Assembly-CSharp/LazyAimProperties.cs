using System;
using UnityEngine;

// Token: 0x02000531 RID: 1329
[CreateAssetMenu(menuName = "Rust/LazyAim Properties")]
public class LazyAimProperties : ScriptableObject
{
	// Token: 0x04002112 RID: 8466
	[Range(0f, 10f)]
	public float snapStrength = 6f;

	// Token: 0x04002113 RID: 8467
	[Range(0f, 45f)]
	public float deadzoneAngle = 1f;
}
