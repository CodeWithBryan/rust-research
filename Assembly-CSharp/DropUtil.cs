using System;
using ConVar;
using UnityEngine;

// Token: 0x020003EE RID: 1006
public class DropUtil
{
	// Token: 0x060021E1 RID: 8673 RVA: 0x000D8F6C File Offset: 0x000D716C
	public static void DropItems(ItemContainer container, Vector3 position)
	{
		if (!Server.dropitems)
		{
			return;
		}
		if (container == null)
		{
			return;
		}
		if (container.itemList == null)
		{
			return;
		}
		float num = 0.25f;
		foreach (Item item in container.itemList.ToArray())
		{
			float num2 = UnityEngine.Random.Range(0f, 2f);
			item.RemoveFromContainer();
			BaseEntity baseEntity = item.CreateWorldObject(position + new Vector3(UnityEngine.Random.Range(-num, num), 1f, UnityEngine.Random.Range(-num, num)), default(Quaternion), null, 0U);
			if (baseEntity == null)
			{
				item.Remove(0f);
			}
			else if (num2 > 0f)
			{
				baseEntity.SetVelocity(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(-1f, 1f)) * num2);
				baseEntity.SetAngularVelocity(new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f)) * num2);
			}
		}
	}
}
