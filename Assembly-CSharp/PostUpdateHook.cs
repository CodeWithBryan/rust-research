using System;
using UnityEngine;

// Token: 0x020008DE RID: 2270
public class PostUpdateHook : MonoBehaviour
{
	// Token: 0x06003666 RID: 13926 RVA: 0x001441C8 File Offset: 0x001423C8
	private void Update()
	{
		Action onUpdate = PostUpdateHook.OnUpdate;
		if (onUpdate == null)
		{
			return;
		}
		onUpdate();
	}

	// Token: 0x06003667 RID: 13927 RVA: 0x001441D9 File Offset: 0x001423D9
	private void LateUpdate()
	{
		Action onLateUpdate = PostUpdateHook.OnLateUpdate;
		if (onLateUpdate == null)
		{
			return;
		}
		onLateUpdate();
	}

	// Token: 0x06003668 RID: 13928 RVA: 0x001441EA File Offset: 0x001423EA
	private void FixedUpdate()
	{
		Action onFixedUpdate = PostUpdateHook.OnFixedUpdate;
		if (onFixedUpdate == null)
		{
			return;
		}
		onFixedUpdate();
	}

	// Token: 0x04003168 RID: 12648
	public static Action OnUpdate;

	// Token: 0x04003169 RID: 12649
	public static Action OnLateUpdate;

	// Token: 0x0400316A RID: 12650
	public static Action OnFixedUpdate;
}
