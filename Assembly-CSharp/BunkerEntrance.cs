using System;
using UnityEngine;

// Token: 0x02000187 RID: 391
public class BunkerEntrance : BaseEntity, IMissionEntityListener
{
	// Token: 0x060016FF RID: 5887 RVA: 0x000ACF80 File Offset: 0x000AB180
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.portalPrefab.isValid)
		{
			this.portalInstance = GameManager.server.CreateEntity(this.portalPrefab.resourcePath, this.portalSpawnPoint.position, this.portalSpawnPoint.rotation, true).GetComponent<BasePortal>();
			this.portalInstance.SetParent(this, true, false);
			this.portalInstance.Spawn();
		}
		if (this.doorPrefab.isValid)
		{
			this.doorInstance = GameManager.server.CreateEntity(this.doorPrefab.resourcePath, this.doorSpawnPoint.position, this.doorSpawnPoint.rotation, true).GetComponent<Door>();
			this.doorInstance.SetParent(this, true, false);
			this.doorInstance.Spawn();
		}
	}

	// Token: 0x06001700 RID: 5888 RVA: 0x000059DD File Offset: 0x00003BDD
	public void MissionStarted(BasePlayer assignee, BaseMission.MissionInstance instance)
	{
	}

	// Token: 0x06001701 RID: 5889 RVA: 0x000059DD File Offset: 0x00003BDD
	public void MissionEnded(BasePlayer assignee, BaseMission.MissionInstance instance)
	{
	}

	// Token: 0x04001014 RID: 4116
	public GameObjectRef portalPrefab;

	// Token: 0x04001015 RID: 4117
	public GameObjectRef doorPrefab;

	// Token: 0x04001016 RID: 4118
	public Transform portalSpawnPoint;

	// Token: 0x04001017 RID: 4119
	public Transform doorSpawnPoint;

	// Token: 0x04001018 RID: 4120
	public Door doorInstance;

	// Token: 0x04001019 RID: 4121
	public BasePortal portalInstance;
}
