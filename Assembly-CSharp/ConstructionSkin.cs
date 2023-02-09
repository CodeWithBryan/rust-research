using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200023D RID: 573
public class ConstructionSkin : BasePrefab
{
	// Token: 0x06001B14 RID: 6932 RVA: 0x000BDA50 File Offset: 0x000BBC50
	public int DetermineConditionalModelState(BuildingBlock parent)
	{
		ConditionalModel[] array = PrefabAttribute.server.FindAll<ConditionalModel>(this.prefabID);
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].RunTests(parent))
			{
				num |= 1 << i;
			}
		}
		return num;
	}

	// Token: 0x06001B15 RID: 6933 RVA: 0x000BDA94 File Offset: 0x000BBC94
	private void CreateConditionalModels(BuildingBlock parent)
	{
		ConditionalModel[] array = PrefabAttribute.server.FindAll<ConditionalModel>(this.prefabID);
		for (int i = 0; i < array.Length; i++)
		{
			if (parent.GetConditionalModel(i))
			{
				GameObject gameObject = array[i].InstantiateSkin(parent);
				if (!(gameObject == null))
				{
					if (this.conditionals == null)
					{
						this.conditionals = new List<GameObject>();
					}
					this.conditionals.Add(gameObject);
				}
			}
		}
	}

	// Token: 0x06001B16 RID: 6934 RVA: 0x000BDAFC File Offset: 0x000BBCFC
	private void DestroyConditionalModels(BuildingBlock parent)
	{
		if (this.conditionals == null)
		{
			return;
		}
		for (int i = 0; i < this.conditionals.Count; i++)
		{
			parent.gameManager.Retire(this.conditionals[i]);
		}
		this.conditionals.Clear();
	}

	// Token: 0x06001B17 RID: 6935 RVA: 0x000BDB4A File Offset: 0x000BBD4A
	public void Refresh(BuildingBlock parent)
	{
		this.DestroyConditionalModels(parent);
		this.CreateConditionalModels(parent);
	}

	// Token: 0x06001B18 RID: 6936 RVA: 0x000BDB5A File Offset: 0x000BBD5A
	public void Destroy(BuildingBlock parent)
	{
		this.DestroyConditionalModels(parent);
		parent.gameManager.Retire(base.gameObject);
	}

	// Token: 0x0400142C RID: 5164
	private List<GameObject> conditionals;
}
