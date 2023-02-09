using System;
using UnityEngine;

// Token: 0x020008E6 RID: 2278
public class StartOfFrameHook : MonoBehaviour
{
	// Token: 0x06003683 RID: 13955 RVA: 0x0014450C File Offset: 0x0014270C
	private void OnEnable()
	{
		Action onStartOfFrame = StartOfFrameHook.OnStartOfFrame;
		if (onStartOfFrame == null)
		{
			return;
		}
		onStartOfFrame();
	}

	// Token: 0x06003684 RID: 13956 RVA: 0x0014451D File Offset: 0x0014271D
	private void Update()
	{
		base.gameObject.SetActive(false);
		base.gameObject.SetActive(true);
	}

	// Token: 0x0400318C RID: 12684
	public static Action OnStartOfFrame;
}
