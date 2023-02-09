using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F9 RID: 761
public class DevBotSpawner : FacepunchBehaviour
{
	// Token: 0x06001D8C RID: 7564 RVA: 0x000CA118 File Offset: 0x000C8318
	public bool HasFreePopulation()
	{
		for (int i = this._spawned.Count - 1; i >= 0; i--)
		{
			BaseEntity baseEntity = this._spawned[i];
			if (baseEntity == null || baseEntity.Health() <= 0f)
			{
				this._spawned.Remove(baseEntity);
			}
		}
		return this._spawned.Count < this.maxPopulation;
	}

	// Token: 0x06001D8D RID: 7565 RVA: 0x000CA184 File Offset: 0x000C8384
	public void SpawnBot()
	{
		while (this.HasFreePopulation())
		{
			Vector3 position = this.waypoints[0].position;
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.bot.resourcePath, position, default(Quaternion), true);
			if (baseEntity == null)
			{
				return;
			}
			this._spawned.Add(baseEntity);
			baseEntity.SendMessage("SetWaypoints", this.waypoints, SendMessageOptions.DontRequireReceiver);
			baseEntity.Spawn();
		}
	}

	// Token: 0x06001D8E RID: 7566 RVA: 0x000CA1F8 File Offset: 0x000C83F8
	public void Start()
	{
		this.waypoints = this.waypointParent.GetComponentsInChildren<Transform>();
		base.InvokeRepeating(new Action(this.SpawnBot), 5f, this.spawnRate);
	}

	// Token: 0x040016F3 RID: 5875
	public GameObjectRef bot;

	// Token: 0x040016F4 RID: 5876
	public Transform waypointParent;

	// Token: 0x040016F5 RID: 5877
	public bool autoSelectLatestSpawnedGameObject = true;

	// Token: 0x040016F6 RID: 5878
	public float spawnRate = 1f;

	// Token: 0x040016F7 RID: 5879
	public int maxPopulation = 1;

	// Token: 0x040016F8 RID: 5880
	private Transform[] waypoints;

	// Token: 0x040016F9 RID: 5881
	private List<BaseEntity> _spawned = new List<BaseEntity>();
}
