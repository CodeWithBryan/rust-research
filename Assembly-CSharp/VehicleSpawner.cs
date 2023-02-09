using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x02000194 RID: 404
public class VehicleSpawner : BaseEntity
{
	// Token: 0x06001772 RID: 6002 RVA: 0x000AEF9A File Offset: 0x000AD19A
	public virtual int GetOccupyLayer()
	{
		return 32768;
	}

	// Token: 0x06001773 RID: 6003 RVA: 0x000AEFA4 File Offset: 0x000AD1A4
	public BaseVehicle GetVehicleOccupying()
	{
		BaseVehicle result = null;
		List<BaseVehicle> list = Pool.GetList<BaseVehicle>();
		Vis.Entities<BaseVehicle>(this.spawnOffset.transform.position, this.occupyRadius, list, this.GetOccupyLayer(), QueryTriggerInteraction.Ignore);
		if (list.Count > 0)
		{
			result = list[0];
		}
		Pool.FreeList<BaseVehicle>(ref list);
		return result;
	}

	// Token: 0x06001774 RID: 6004 RVA: 0x000AEFF8 File Offset: 0x000AD1F8
	public bool IsPadOccupied()
	{
		BaseVehicle vehicleOccupying = this.GetVehicleOccupying();
		return vehicleOccupying != null && !vehicleOccupying.IsDespawnEligable();
	}

	// Token: 0x06001775 RID: 6005 RVA: 0x000AF020 File Offset: 0x000AD220
	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		BasePlayer newOwner = null;
		NPCTalking component = from.GetComponent<NPCTalking>();
		if (component)
		{
			newOwner = component.GetActionPlayer();
		}
		foreach (VehicleSpawner.SpawnPair spawnPair in this.objectsToSpawn)
		{
			if (msg == spawnPair.message)
			{
				this.SpawnVehicle(spawnPair.prefabToSpawn.resourcePath, newOwner);
				return;
			}
		}
	}

	// Token: 0x06001776 RID: 6006 RVA: 0x000AF084 File Offset: 0x000AD284
	public BaseVehicle SpawnVehicle(string prefabToSpawn, BasePlayer newOwner)
	{
		this.CleanupArea(this.cleanupRadius);
		this.NudgePlayersInRadius(this.spawnNudgeRadius);
		BaseEntity baseEntity = GameManager.server.CreateEntity(prefabToSpawn, this.spawnOffset.transform.position, this.spawnOffset.transform.rotation, true);
		baseEntity.Spawn();
		BaseVehicle component = baseEntity.GetComponent<BaseVehicle>();
		if (newOwner != null)
		{
			component.SetupOwner(newOwner, this.spawnOffset.transform.position, this.safeRadius);
		}
		VehicleSpawnPoint.AddStartingFuel(component);
		if (this.LogAnalytics)
		{
			Analytics.Server.VehiclePurchased(component.ShortPrefabName);
		}
		return component;
	}

	// Token: 0x170001CA RID: 458
	// (get) Token: 0x06001777 RID: 6007 RVA: 0x00003A54 File Offset: 0x00001C54
	protected virtual bool LogAnalytics
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001778 RID: 6008 RVA: 0x000AF124 File Offset: 0x000AD324
	public void CleanupArea(float radius)
	{
		List<BaseVehicle> list = Pool.GetList<BaseVehicle>();
		Vis.Entities<BaseVehicle>(this.spawnOffset.transform.position, radius, list, 32768, QueryTriggerInteraction.Collide);
		foreach (BaseVehicle baseVehicle in list)
		{
			if (!baseVehicle.isClient && !baseVehicle.IsDestroyed)
			{
				baseVehicle.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
		List<ServerGib> list2 = Pool.GetList<ServerGib>();
		Vis.Entities<ServerGib>(this.spawnOffset.transform.position, radius, list2, 67108865, QueryTriggerInteraction.Collide);
		foreach (ServerGib serverGib in list2)
		{
			if (!serverGib.isClient)
			{
				serverGib.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
		Pool.FreeList<BaseVehicle>(ref list);
		Pool.FreeList<ServerGib>(ref list2);
	}

	// Token: 0x06001779 RID: 6009 RVA: 0x000AF220 File Offset: 0x000AD420
	public void NudgePlayersInRadius(float radius)
	{
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Vis.Entities<BasePlayer>(this.spawnOffset.transform.position, radius, list, 131072, QueryTriggerInteraction.Collide);
		foreach (BasePlayer basePlayer in list)
		{
			if (!basePlayer.IsNpc && !basePlayer.isMounted && basePlayer.IsConnected)
			{
				Vector3 vector = this.spawnOffset.transform.position;
				vector += Vector3Ex.Direction2D(basePlayer.transform.position, this.spawnOffset.transform.position) * radius;
				vector += Vector3.up * 0.1f;
				basePlayer.MovePosition(vector);
				basePlayer.ClientRPCPlayer<Vector3>(null, basePlayer, "ForcePositionTo", vector);
			}
		}
		Pool.FreeList<BasePlayer>(ref list);
	}

	// Token: 0x04001060 RID: 4192
	public float spawnNudgeRadius = 6f;

	// Token: 0x04001061 RID: 4193
	public float cleanupRadius = 10f;

	// Token: 0x04001062 RID: 4194
	public float occupyRadius = 5f;

	// Token: 0x04001063 RID: 4195
	public VehicleSpawner.SpawnPair[] objectsToSpawn;

	// Token: 0x04001064 RID: 4196
	public Transform spawnOffset;

	// Token: 0x04001065 RID: 4197
	public float safeRadius = 10f;

	// Token: 0x02000BE9 RID: 3049
	[Serializable]
	public class SpawnPair
	{
		// Token: 0x0400402A RID: 16426
		public string message;

		// Token: 0x0400402B RID: 16427
		public GameObjectRef prefabToSpawn;
	}
}
