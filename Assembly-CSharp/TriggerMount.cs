using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200055E RID: 1374
public class TriggerMount : TriggerBase, IServerComponent
{
	// Token: 0x060029C5 RID: 10693 RVA: 0x000FD5DC File Offset: 0x000FB7DC
	internal override GameObject InterestedInObject(GameObject obj)
	{
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		BasePlayer basePlayer = baseEntity.ToPlayer();
		if (basePlayer == null || basePlayer.IsNpc)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x060029C6 RID: 10694 RVA: 0x000FD61C File Offset: 0x000FB81C
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (this.entryInfo == null)
		{
			this.entryInfo = new Dictionary<BaseEntity, TriggerMount.EntryInfo>();
		}
		this.entryInfo.Add(ent, new TriggerMount.EntryInfo(Time.time, ent.transform.position));
		base.Invoke(new Action(this.CheckForMount), 3.6f);
	}

	// Token: 0x060029C7 RID: 10695 RVA: 0x000FD67B File Offset: 0x000FB87B
	internal override void OnEntityLeave(BaseEntity ent)
	{
		if (ent != null && this.entryInfo != null)
		{
			this.entryInfo.Remove(ent);
		}
		base.OnEntityLeave(ent);
	}

	// Token: 0x060029C8 RID: 10696 RVA: 0x000FD6A4 File Offset: 0x000FB8A4
	private void CheckForMount()
	{
		if (this.entityContents == null || this.entryInfo == null)
		{
			return;
		}
		foreach (KeyValuePair<BaseEntity, TriggerMount.EntryInfo> keyValuePair in this.entryInfo)
		{
			BaseEntity key = keyValuePair.Key;
			if (key.IsValid())
			{
				TriggerMount.EntryInfo value = keyValuePair.Value;
				BasePlayer basePlayer = key.ToPlayer();
				bool flag = (basePlayer.IsAdmin || basePlayer.IsDeveloper) && basePlayer.IsFlying;
				if (basePlayer != null && basePlayer.IsAlive() && !flag)
				{
					bool flag2 = false;
					if (!basePlayer.isMounted && !basePlayer.IsSleeping() && value.entryTime + 3.5f < Time.time && Vector3.Distance(key.transform.position, value.entryPos) < 0.5f)
					{
						BaseVehicle componentInParent = base.GetComponentInParent<BaseVehicle>();
						if (componentInParent != null && !componentInParent.IsDead())
						{
							componentInParent.AttemptMount(basePlayer, true);
							flag2 = true;
						}
					}
					if (!flag2)
					{
						value.Set(Time.time, key.transform.position);
						base.Invoke(new Action(this.CheckForMount), 3.6f);
					}
				}
			}
		}
	}

	// Token: 0x040021CF RID: 8655
	private const float MOUNT_DELAY = 3.5f;

	// Token: 0x040021D0 RID: 8656
	private const float MAX_MOVE = 0.5f;

	// Token: 0x040021D1 RID: 8657
	private Dictionary<BaseEntity, TriggerMount.EntryInfo> entryInfo;

	// Token: 0x02000D04 RID: 3332
	private class EntryInfo
	{
		// Token: 0x06004E0A RID: 19978 RVA: 0x0019A255 File Offset: 0x00198455
		public EntryInfo(float entryTime, Vector3 entryPos)
		{
			this.entryTime = entryTime;
			this.entryPos = entryPos;
		}

		// Token: 0x06004E0B RID: 19979 RVA: 0x0019A26B File Offset: 0x0019846B
		public void Set(float entryTime, Vector3 entryPos)
		{
			this.entryTime = entryTime;
			this.entryPos = entryPos;
		}

		// Token: 0x040044A3 RID: 17571
		public float entryTime;

		// Token: 0x040044A4 RID: 17572
		public Vector3 entryPos;
	}
}
