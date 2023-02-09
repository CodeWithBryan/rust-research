using System;
using UnityEngine;

// Token: 0x020008BD RID: 2237
public class ForceChildSingletonSetup : MonoBehaviour
{
	// Token: 0x0600360F RID: 13839 RVA: 0x00143204 File Offset: 0x00141404
	[ComponentHelp("Any child objects of this object that contain SingletonComponents will be registered - even if they're not enabled")]
	private void Awake()
	{
		SingletonComponent[] componentsInChildren = base.GetComponentsInChildren<SingletonComponent>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SingletonSetup();
		}
	}
}
