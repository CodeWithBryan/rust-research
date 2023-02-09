using System;
using UnityEngine;

// Token: 0x02000546 RID: 1350
public class IndividualSpawner : BaseMonoBehaviour, IServerComponent, ISpawnPointUser, ISpawnGroup
{
	// Token: 0x1700033A RID: 826
	// (get) Token: 0x06002916 RID: 10518 RVA: 0x000F9EC9 File Offset: 0x000F80C9
	public int currentPopulation
	{
		get
		{
			if (!(this.spawnInstance == null))
			{
				return 1;
			}
			return 0;
		}
	}

	// Token: 0x1700033B RID: 827
	// (get) Token: 0x06002917 RID: 10519 RVA: 0x000F9EDC File Offset: 0x000F80DC
	private bool IsSpawned
	{
		get
		{
			return this.spawnInstance != null;
		}
	}

	// Token: 0x06002918 RID: 10520 RVA: 0x000F9EEA File Offset: 0x000F80EA
	protected void Awake()
	{
		if (SingletonComponent<SpawnHandler>.Instance)
		{
			SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Add(this);
			return;
		}
		Debug.LogWarning(base.GetType().Name + ": SpawnHandler instance not found.");
	}

	// Token: 0x06002919 RID: 10521 RVA: 0x000F9F24 File Offset: 0x000F8124
	protected void OnDrawGizmosSelected()
	{
		Bounds bounds;
		if (this.TryGetEntityBounds(out bounds))
		{
			Gizmos.color = Color.yellow;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawCube(bounds.center, bounds.size);
		}
	}

	// Token: 0x0600291A RID: 10522 RVA: 0x000F9F68 File Offset: 0x000F8168
	public void ObjectSpawned(SpawnPointInstance instance)
	{
		this.spawnInstance = instance;
	}

	// Token: 0x0600291B RID: 10523 RVA: 0x000F9F71 File Offset: 0x000F8171
	public void ObjectRetired(SpawnPointInstance instance)
	{
		this.spawnInstance = null;
		this.nextSpawnTime = Time.time + UnityEngine.Random.Range(this.respawnDelayMin, this.respawnDelayMax);
	}

	// Token: 0x0600291C RID: 10524 RVA: 0x000F9F97 File Offset: 0x000F8197
	public void Fill()
	{
		if (this.oneTimeSpawner)
		{
			return;
		}
		this.TrySpawnEntity();
	}

	// Token: 0x0600291D RID: 10525 RVA: 0x000F9FA8 File Offset: 0x000F81A8
	public void SpawnInitial()
	{
		this.TrySpawnEntity();
	}

	// Token: 0x0600291E RID: 10526 RVA: 0x000F9FB0 File Offset: 0x000F81B0
	public void Clear()
	{
		if (this.IsSpawned)
		{
			BaseEntity baseEntity = this.spawnInstance.gameObject.ToBaseEntity();
			if (baseEntity != null)
			{
				baseEntity.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
	}

	// Token: 0x0600291F RID: 10527 RVA: 0x000F9FE6 File Offset: 0x000F81E6
	public void SpawnRepeating()
	{
		if (this.IsSpawned || this.oneTimeSpawner)
		{
			return;
		}
		if (Time.time >= this.nextSpawnTime)
		{
			this.TrySpawnEntity();
		}
	}

	// Token: 0x06002920 RID: 10528 RVA: 0x000FA00C File Offset: 0x000F820C
	public bool HasSpaceToSpawn()
	{
		if (this.useCustomBoundsCheckMask)
		{
			return SpawnHandler.CheckBounds(this.entityPrefab.Get(), base.transform.position, base.transform.rotation, Vector3.one, this.customBoundsCheckMask);
		}
		return SingletonComponent<SpawnHandler>.Instance.CheckBounds(this.entityPrefab.Get(), base.transform.position, base.transform.rotation, Vector3.one);
	}

	// Token: 0x06002921 RID: 10529 RVA: 0x000FA084 File Offset: 0x000F8284
	private void TrySpawnEntity()
	{
		if (!this.isSpawnerActive)
		{
			return;
		}
		if (this.IsSpawned)
		{
			return;
		}
		if (!this.HasSpaceToSpawn())
		{
			this.nextSpawnTime = Time.time + UnityEngine.Random.Range(this.respawnDelayMin, this.respawnDelayMax);
			return;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.entityPrefab.resourcePath, base.transform.position, base.transform.rotation, false);
		if (baseEntity != null)
		{
			if (!this.oneTimeSpawner)
			{
				baseEntity.enableSaving = false;
			}
			baseEntity.gameObject.AwakeFromInstantiate();
			baseEntity.Spawn();
			SpawnPointInstance spawnPointInstance = baseEntity.gameObject.AddComponent<SpawnPointInstance>();
			spawnPointInstance.parentSpawnPointUser = this;
			spawnPointInstance.Notify();
			return;
		}
		Debug.LogError("IndividualSpawner failed to spawn entity.", base.gameObject);
	}

	// Token: 0x06002922 RID: 10530 RVA: 0x000FA148 File Offset: 0x000F8348
	private bool TryGetEntityBounds(out Bounds result)
	{
		if (this.entityPrefab != null)
		{
			GameObject gameObject = this.entityPrefab.Get();
			if (gameObject != null)
			{
				BaseEntity component = gameObject.GetComponent<BaseEntity>();
				if (component != null)
				{
					result = component.bounds;
					return true;
				}
			}
		}
		result = default(Bounds);
		return false;
	}

	// Token: 0x04002158 RID: 8536
	public GameObjectRef entityPrefab;

	// Token: 0x04002159 RID: 8537
	public float respawnDelayMin = 10f;

	// Token: 0x0400215A RID: 8538
	public float respawnDelayMax = 20f;

	// Token: 0x0400215B RID: 8539
	public bool useCustomBoundsCheckMask;

	// Token: 0x0400215C RID: 8540
	public LayerMask customBoundsCheckMask;

	// Token: 0x0400215D RID: 8541
	[Tooltip("Simply spawns the entity once. No respawning. Entity can be saved if desired.")]
	[SerializeField]
	private bool oneTimeSpawner;

	// Token: 0x0400215E RID: 8542
	internal bool isSpawnerActive = true;

	// Token: 0x0400215F RID: 8543
	private SpawnPointInstance spawnInstance;

	// Token: 0x04002160 RID: 8544
	private float nextSpawnTime = -1f;
}
