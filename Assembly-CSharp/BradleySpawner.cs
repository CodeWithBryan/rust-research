using System;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x0200019B RID: 411
public class BradleySpawner : MonoBehaviour, IServerComponent
{
	// Token: 0x060017A1 RID: 6049 RVA: 0x000B042C File Offset: 0x000AE62C
	public void Start()
	{
		BradleySpawner.singleton = this;
		base.Invoke("DelayedStart", 3f);
	}

	// Token: 0x060017A2 RID: 6050 RVA: 0x000B0444 File Offset: 0x000AE644
	public void DelayedStart()
	{
		if (this.initialSpawn)
		{
			this.DoRespawn();
		}
		base.InvokeRepeating("CheckIfRespawnNeeded", 0f, 5f);
	}

	// Token: 0x060017A3 RID: 6051 RVA: 0x000B0469 File Offset: 0x000AE669
	public void CheckIfRespawnNeeded()
	{
		if (!this.pendingRespawn && (this.spawned == null || !this.spawned.IsAlive()))
		{
			this.ScheduleRespawn();
		}
	}

	// Token: 0x060017A4 RID: 6052 RVA: 0x000B0494 File Offset: 0x000AE694
	public void ScheduleRespawn()
	{
		base.CancelInvoke("DoRespawn");
		base.Invoke("DoRespawn", UnityEngine.Random.Range(Bradley.respawnDelayMinutes - Bradley.respawnDelayVariance, Bradley.respawnDelayMinutes + Bradley.respawnDelayVariance) * 60f);
		this.pendingRespawn = true;
	}

	// Token: 0x060017A5 RID: 6053 RVA: 0x000B04D4 File Offset: 0x000AE6D4
	public void DoRespawn()
	{
		if (!Rust.Application.isLoading && !Rust.Application.isLoadingSave)
		{
			this.SpawnBradley();
		}
		this.pendingRespawn = false;
	}

	// Token: 0x060017A6 RID: 6054 RVA: 0x000B04F4 File Offset: 0x000AE6F4
	public void SpawnBradley()
	{
		if (this.spawned != null)
		{
			Debug.LogWarning("Bradley attempting to spawn but one already exists!");
			return;
		}
		if (!Bradley.enabled)
		{
			return;
		}
		Vector3 position = this.path.interestZones[UnityEngine.Random.Range(0, this.path.interestZones.Count)].transform.position;
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.bradleyPrefab.resourcePath, position, default(Quaternion), true);
		BradleyAPC component = baseEntity.GetComponent<BradleyAPC>();
		if (component)
		{
			baseEntity.Spawn();
			component.InstallPatrolPath(this.path);
		}
		else
		{
			baseEntity.Kill(BaseNetworkable.DestroyMode.None);
		}
		Debug.Log("BradleyAPC Spawned at :" + position);
		this.spawned = component;
	}

	// Token: 0x04001099 RID: 4249
	public BasePath path;

	// Token: 0x0400109A RID: 4250
	public GameObjectRef bradleyPrefab;

	// Token: 0x0400109B RID: 4251
	[NonSerialized]
	public BradleyAPC spawned;

	// Token: 0x0400109C RID: 4252
	public bool initialSpawn;

	// Token: 0x0400109D RID: 4253
	public float minRespawnTimeMinutes = 5f;

	// Token: 0x0400109E RID: 4254
	public float maxRespawnTimeMinutes = 5f;

	// Token: 0x0400109F RID: 4255
	public static BradleySpawner singleton;

	// Token: 0x040010A0 RID: 4256
	private bool pendingRespawn;
}
