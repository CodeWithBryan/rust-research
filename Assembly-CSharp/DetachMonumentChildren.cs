using System;
using UnityEngine;

// Token: 0x020004D4 RID: 1236
public class DetachMonumentChildren : MonoBehaviour
{
	// Token: 0x06002795 RID: 10133 RVA: 0x000F3101 File Offset: 0x000F1301
	private void Awake()
	{
		base.transform.DetachChildren();
	}
}
