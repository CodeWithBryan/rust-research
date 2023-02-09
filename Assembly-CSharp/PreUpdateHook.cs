using System;
using UnityEngine;

// Token: 0x020008E0 RID: 2272
public class PreUpdateHook : MonoBehaviour
{
	// Token: 0x0600366A RID: 13930 RVA: 0x001441FB File Offset: 0x001423FB
	private void Update()
	{
		Action onUpdate = PreUpdateHook.OnUpdate;
		if (onUpdate == null)
		{
			return;
		}
		onUpdate();
	}

	// Token: 0x0600366B RID: 13931 RVA: 0x0014420C File Offset: 0x0014240C
	private void LateUpdate()
	{
		Action onLateUpdate = PreUpdateHook.OnLateUpdate;
		if (onLateUpdate == null)
		{
			return;
		}
		onLateUpdate();
	}

	// Token: 0x0600366C RID: 13932 RVA: 0x0014421D File Offset: 0x0014241D
	private void FixedUpdate()
	{
		Action onFixedUpdate = PreUpdateHook.OnFixedUpdate;
		if (onFixedUpdate == null)
		{
			return;
		}
		onFixedUpdate();
	}

	// Token: 0x04003174 RID: 12660
	public static Action OnUpdate;

	// Token: 0x04003175 RID: 12661
	public static Action OnLateUpdate;

	// Token: 0x04003176 RID: 12662
	public static Action OnFixedUpdate;
}
