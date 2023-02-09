using System;
using UnityEngine;

// Token: 0x020003D5 RID: 981
public interface IItemContainerEntity : IIdealSlotEntity
{
	// Token: 0x17000295 RID: 661
	// (get) Token: 0x06002175 RID: 8565
	ItemContainer inventory { get; }

	// Token: 0x17000296 RID: 662
	// (get) Token: 0x06002176 RID: 8566
	Transform Transform { get; }

	// Token: 0x17000297 RID: 663
	// (get) Token: 0x06002177 RID: 8567
	bool DropsLoot { get; }

	// Token: 0x17000298 RID: 664
	// (get) Token: 0x06002178 RID: 8568
	bool DropFloats { get; }

	// Token: 0x06002179 RID: 8569
	void DropItems(BaseEntity initiator = null);

	// Token: 0x0600217A RID: 8570
	bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true);

	// Token: 0x0600217B RID: 8571
	bool ShouldDropItemsIndividually();

	// Token: 0x0600217C RID: 8572
	void DropBonusItems(BaseEntity initiator, ItemContainer container);

	// Token: 0x0600217D RID: 8573
	Vector3 GetDropPosition();
}
