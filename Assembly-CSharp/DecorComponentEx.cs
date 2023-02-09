using System;
using UnityEngine;

// Token: 0x0200062E RID: 1582
public static class DecorComponentEx
{
	// Token: 0x06002DCC RID: 11724 RVA: 0x00113590 File Offset: 0x00111790
	public static void ApplyDecorComponents(this Transform transform, DecorComponent[] components, ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		foreach (DecorComponent decorComponent in components)
		{
			if (!decorComponent.isRoot)
			{
				return;
			}
			decorComponent.Apply(ref pos, ref rot, ref scale);
		}
	}

	// Token: 0x06002DCD RID: 11725 RVA: 0x001135C4 File Offset: 0x001117C4
	public static void ApplyDecorComponents(this Transform transform, DecorComponent[] components)
	{
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		Vector3 localScale = transform.localScale;
		transform.ApplyDecorComponents(components, ref position, ref rotation, ref localScale);
		transform.position = position;
		transform.rotation = rotation;
		transform.localScale = localScale;
	}

	// Token: 0x06002DCE RID: 11726 RVA: 0x00113608 File Offset: 0x00111808
	public static void ApplyDecorComponentsScaleOnly(this Transform transform, DecorComponent[] components)
	{
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		Vector3 localScale = transform.localScale;
		transform.ApplyDecorComponents(components, ref position, ref rotation, ref localScale);
		transform.localScale = localScale;
	}
}
