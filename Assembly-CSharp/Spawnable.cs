using System;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x0200053F RID: 1343
public class Spawnable : MonoBehaviour, IServerComponent
{
	// Token: 0x060028F2 RID: 10482 RVA: 0x000F985A File Offset: 0x000F7A5A
	protected void OnEnable()
	{
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		this.Add();
	}

	// Token: 0x060028F3 RID: 10483 RVA: 0x000F986A File Offset: 0x000F7A6A
	protected void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		this.Remove();
	}

	// Token: 0x060028F4 RID: 10484 RVA: 0x000F9884 File Offset: 0x000F7A84
	private void Add()
	{
		this.SpawnPosition = base.transform.position;
		this.SpawnRotation = base.transform.rotation;
		if (SingletonComponent<SpawnHandler>.Instance)
		{
			if (this.Population != null)
			{
				SingletonComponent<SpawnHandler>.Instance.AddInstance(this);
				return;
			}
			if (Rust.Application.isLoading && !Rust.Application.isLoadingSave)
			{
				global::BaseEntity component = base.GetComponent<global::BaseEntity>();
				if (component != null && component.enableSaving && !component.syncPosition)
				{
					SingletonComponent<SpawnHandler>.Instance.AddRespawn(new SpawnIndividual(component.prefabID, this.SpawnPosition, this.SpawnRotation));
				}
			}
		}
	}

	// Token: 0x060028F5 RID: 10485 RVA: 0x000F9928 File Offset: 0x000F7B28
	private void Remove()
	{
		if (SingletonComponent<SpawnHandler>.Instance && this.Population != null)
		{
			SingletonComponent<SpawnHandler>.Instance.RemoveInstance(this);
		}
	}

	// Token: 0x060028F6 RID: 10486 RVA: 0x000F994F File Offset: 0x000F7B4F
	internal void Save(global::BaseNetworkable.SaveInfo info)
	{
		if (this.Population == null)
		{
			return;
		}
		info.msg.spawnable = Pool.Get<ProtoBuf.Spawnable>();
		info.msg.spawnable.population = this.Population.FilenameStringId;
	}

	// Token: 0x060028F7 RID: 10487 RVA: 0x000F998B File Offset: 0x000F7B8B
	internal void Load(global::BaseNetworkable.LoadInfo info)
	{
		if (info.msg.spawnable != null)
		{
			this.Population = FileSystem.Load<SpawnPopulation>(StringPool.Get(info.msg.spawnable.population), true);
		}
		this.Add();
	}

	// Token: 0x060028F8 RID: 10488 RVA: 0x000F99C1 File Offset: 0x000F7BC1
	protected void OnValidate()
	{
		this.Population = null;
	}

	// Token: 0x04002149 RID: 8521
	[ReadOnly]
	public SpawnPopulation Population;

	// Token: 0x0400214A RID: 8522
	[SerializeField]
	private bool ForceSpawnOnly;

	// Token: 0x0400214B RID: 8523
	[SerializeField]
	private string ForceSpawnInfoMessage = string.Empty;

	// Token: 0x0400214C RID: 8524
	internal uint PrefabID;

	// Token: 0x0400214D RID: 8525
	internal bool SpawnIndividual;

	// Token: 0x0400214E RID: 8526
	internal Vector3 SpawnPosition;

	// Token: 0x0400214F RID: 8527
	internal Quaternion SpawnRotation;
}
