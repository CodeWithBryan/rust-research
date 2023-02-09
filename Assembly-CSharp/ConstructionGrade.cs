using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200023B RID: 571
public class ConstructionGrade : PrefabAttribute
{
	// Token: 0x1700020F RID: 527
	// (get) Token: 0x06001B0D RID: 6925 RVA: 0x000BD8C9 File Offset: 0x000BBAC9
	public float maxHealth
	{
		get
		{
			if (!this.gradeBase || !this.construction)
			{
				return 0f;
			}
			return this.gradeBase.baseHealth * this.construction.healthMultiplier;
		}
	}

	// Token: 0x17000210 RID: 528
	// (get) Token: 0x06001B0E RID: 6926 RVA: 0x000BD904 File Offset: 0x000BBB04
	public List<ItemAmount> costToBuild
	{
		get
		{
			if (this._costToBuild != null)
			{
				return this._costToBuild;
			}
			this._costToBuild = new List<ItemAmount>();
			foreach (ItemAmount itemAmount in this.gradeBase.baseCost)
			{
				this._costToBuild.Add(new ItemAmount(itemAmount.itemDef, Mathf.Ceil(itemAmount.amount * this.construction.costMultiplier)));
			}
			return this._costToBuild;
		}
	}

	// Token: 0x06001B0F RID: 6927 RVA: 0x000BD9A4 File Offset: 0x000BBBA4
	protected override Type GetIndexedType()
	{
		return typeof(ConstructionGrade);
	}

	// Token: 0x04001424 RID: 5156
	[NonSerialized]
	public Construction construction;

	// Token: 0x04001425 RID: 5157
	public BuildingGrade gradeBase;

	// Token: 0x04001426 RID: 5158
	public GameObjectRef skinObject;

	// Token: 0x04001427 RID: 5159
	internal List<ItemAmount> _costToBuild;
}
