using System;
using UnityEngine;

// Token: 0x02000239 RID: 569
public class ConditionalModel : PrefabAttribute
{
	// Token: 0x06001AFD RID: 6909 RVA: 0x000BCF5B File Offset: 0x000BB15B
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.conditions = base.GetComponentsInChildren<ModelConditionTest>(true);
	}

	// Token: 0x06001AFE RID: 6910 RVA: 0x000BCF78 File Offset: 0x000BB178
	public bool RunTests(BaseEntity parent)
	{
		for (int i = 0; i < this.conditions.Length; i++)
		{
			if (!this.conditions[i].DoTest(parent))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001AFF RID: 6911 RVA: 0x000BCFAC File Offset: 0x000BB1AC
	public GameObject InstantiateSkin(BaseEntity parent)
	{
		if (!this.onServer && this.isServer)
		{
			return null;
		}
		GameObject gameObject = this.gameManager.CreatePrefab(this.prefab.resourcePath, parent.transform, false);
		if (gameObject)
		{
			gameObject.transform.localPosition = this.worldPosition;
			gameObject.transform.localRotation = this.worldRotation;
			gameObject.AwakeFromInstantiate();
		}
		return gameObject;
	}

	// Token: 0x06001B00 RID: 6912 RVA: 0x000BD01A File Offset: 0x000BB21A
	protected override Type GetIndexedType()
	{
		return typeof(ConditionalModel);
	}

	// Token: 0x04001406 RID: 5126
	public GameObjectRef prefab;

	// Token: 0x04001407 RID: 5127
	public bool onClient = true;

	// Token: 0x04001408 RID: 5128
	public bool onServer = true;

	// Token: 0x04001409 RID: 5129
	[NonSerialized]
	public ModelConditionTest[] conditions;
}
