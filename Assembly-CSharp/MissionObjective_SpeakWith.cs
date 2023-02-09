using System;
using UnityEngine;

// Token: 0x020005F0 RID: 1520
[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/SpeakWith")]
public class MissionObjective_SpeakWith : MissionObjective
{
	// Token: 0x06002C88 RID: 11400 RVA: 0x0010A6C8 File Offset: 0x001088C8
	public override void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		BaseEntity baseEntity = instance.ProviderEntity();
		if (baseEntity)
		{
			instance.missionLocation = baseEntity.transform.position;
			playerFor.MissionDirty(true);
		}
		base.ObjectiveStarted(playerFor, index, instance);
	}

	// Token: 0x06002C89 RID: 11401 RVA: 0x0010A708 File Offset: 0x00108908
	public override void ProcessMissionEvent(BasePlayer playerFor, BaseMission.MissionInstance instance, int index, BaseMission.MissionEventType type, string identifier, float amount)
	{
		if (base.IsCompleted(index, instance))
		{
			return;
		}
		if (!base.CanProgress(index, instance))
		{
			return;
		}
		if (type == BaseMission.MissionEventType.CONVERSATION)
		{
			BaseEntity baseEntity = instance.ProviderEntity();
			if (baseEntity)
			{
				IMissionProvider component = baseEntity.GetComponent<IMissionProvider>();
				if (component != null && component.ProviderID().ToString() == identifier && amount == 1f)
				{
					bool flag = true;
					if (this.requiredReturnItems != null && this.requiredReturnItems.Length != 0)
					{
						foreach (ItemAmount itemAmount in this.requiredReturnItems)
						{
							if ((float)playerFor.inventory.GetAmount(itemAmount.itemDef.itemid) < itemAmount.amount)
							{
								flag = false;
								break;
							}
						}
						if (flag && this.destroyReturnItems)
						{
							foreach (ItemAmount itemAmount2 in this.requiredReturnItems)
							{
								playerFor.inventory.Take(null, itemAmount2.itemDef.itemid, (int)itemAmount2.amount);
							}
						}
					}
					if (this.requiredReturnItems == null || this.requiredReturnItems.Length == 0 || flag)
					{
						this.CompleteObjective(index, instance, playerFor);
						playerFor.MissionDirty(true);
					}
				}
			}
		}
		base.ProcessMissionEvent(playerFor, instance, index, type, identifier, amount);
	}

	// Token: 0x0400244E RID: 9294
	public ItemAmount[] requiredReturnItems;

	// Token: 0x0400244F RID: 9295
	public bool destroyReturnItems;
}
