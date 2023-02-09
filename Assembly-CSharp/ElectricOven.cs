using System;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x020003B4 RID: 948
public class ElectricOven : global::BaseOven
{
	// Token: 0x0600207E RID: 8318 RVA: 0x000D401B File Offset: 0x000D221B
	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.SpawnIOEnt();
		}
	}

	// Token: 0x0600207F RID: 8319 RVA: 0x000D4030 File Offset: 0x000D2230
	private void SpawnIOEnt()
	{
		if (this.IoEntity.isValid && this.IoEntityAnchor != null)
		{
			global::IOEntity ioentity = GameManager.server.CreateEntity(this.IoEntity.resourcePath, this.IoEntityAnchor.position, this.IoEntityAnchor.rotation, true) as global::IOEntity;
			ioentity.SetParent(this, true, false);
			ioentity.Spawn();
			this.spawnedIo.Set(ioentity);
		}
	}

	// Token: 0x1700027B RID: 635
	// (get) Token: 0x06002080 RID: 8320 RVA: 0x000D40A5 File Offset: 0x000D22A5
	protected override bool CanRunWithNoFuel
	{
		get
		{
			return this.spawnedIo.IsValid(true) && this.spawnedIo.Get(true).IsPowered();
		}
	}

	// Token: 0x06002081 RID: 8321 RVA: 0x000D40C8 File Offset: 0x000D22C8
	public void OnIOEntityFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		if (!next.HasFlag(global::BaseEntity.Flags.Reserved8) && base.IsOn())
		{
			this.StopCooking();
			this.resumeCookingWhenPowerResumes = true;
			return;
		}
		if (next.HasFlag(global::BaseEntity.Flags.Reserved8) && !base.IsOn() && this.resumeCookingWhenPowerResumes)
		{
			this.StartCooking();
			this.resumeCookingWhenPowerResumes = false;
		}
	}

	// Token: 0x06002082 RID: 8322 RVA: 0x000D4138 File Offset: 0x000D2338
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.simpleUID == null)
		{
			info.msg.simpleUID = Pool.Get<SimpleUID>();
		}
		info.msg.simpleUID.uid = this.spawnedIo.uid;
	}

	// Token: 0x06002083 RID: 8323 RVA: 0x000D4184 File Offset: 0x000D2384
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.simpleUID != null)
		{
			this.spawnedIo.uid = info.msg.simpleUID.uid;
		}
	}

	// Token: 0x06002084 RID: 8324 RVA: 0x000D41B5 File Offset: 0x000D23B5
	protected override bool CanPickupOven()
	{
		return this.children.Count == 1;
	}

	// Token: 0x0400194F RID: 6479
	public GameObjectRef IoEntity;

	// Token: 0x04001950 RID: 6480
	public Transform IoEntityAnchor;

	// Token: 0x04001951 RID: 6481
	private EntityRef<global::IOEntity> spawnedIo;

	// Token: 0x04001952 RID: 6482
	private bool resumeCookingWhenPowerResumes;
}
