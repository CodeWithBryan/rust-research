using System;
using Rust;
using UnityEngine;

// Token: 0x0200048D RID: 1165
public class TrainCarUnloadableLoot : TrainCarUnloadable
{
	// Token: 0x060025D3 RID: 9683 RVA: 0x000EC604 File Offset: 0x000EA804
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			int num = UnityEngine.Random.Range(0, this.lootLayouts.Length);
			for (int i = 0; i < this.lootLayouts[num].crates.Length; i++)
			{
				GameObjectRef gameObjectRef = this.lootLayouts[num].crates[i];
				BaseEntity baseEntity = GameManager.server.CreateEntity(gameObjectRef.resourcePath, this.lootPositions[i].localPosition, this.lootPositions[i].localRotation, true);
				if (baseEntity != null)
				{
					baseEntity.Spawn();
					baseEntity.SetParent(this, false, false);
				}
			}
		}
	}

	// Token: 0x04001EAB RID: 7851
	[SerializeField]
	private TrainCarUnloadableLoot.LootCrateSet[] lootLayouts;

	// Token: 0x04001EAC RID: 7852
	[SerializeField]
	private Transform[] lootPositions;

	// Token: 0x02000CB9 RID: 3257
	[Serializable]
	public class LootCrateSet
	{
		// Token: 0x04004395 RID: 17301
		public GameObjectRef[] crates;
	}
}
