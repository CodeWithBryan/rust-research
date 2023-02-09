using System;
using UnityEngine;

// Token: 0x020005BF RID: 1471
public class ItemModDeployable : MonoBehaviour
{
	// Token: 0x06002BB8 RID: 11192 RVA: 0x00106F24 File Offset: 0x00105124
	public Deployable GetDeployable(BaseEntity entity)
	{
		if (entity.gameManager.FindPrefab(this.entityPrefab.resourcePath) == null)
		{
			return null;
		}
		return entity.prefabAttribute.Find<Deployable>(this.entityPrefab.resourceID);
	}

	// Token: 0x06002BB9 RID: 11193 RVA: 0x00106F5C File Offset: 0x0010515C
	internal void OnDeployed(BaseEntity ent, BasePlayer player)
	{
		if (player.IsValid() && !string.IsNullOrEmpty(this.UnlockAchievement))
		{
			player.GiveAchievement(this.UnlockAchievement);
		}
		BuildingPrivlidge buildingPrivlidge;
		if ((buildingPrivlidge = (ent as BuildingPrivlidge)) != null)
		{
			buildingPrivlidge.AddPlayer(player);
		}
	}

	// Token: 0x0400237F RID: 9087
	public GameObjectRef entityPrefab = new GameObjectRef();

	// Token: 0x04002380 RID: 9088
	[Header("Tooltips")]
	public bool showCrosshair;

	// Token: 0x04002381 RID: 9089
	public string UnlockAchievement;
}
