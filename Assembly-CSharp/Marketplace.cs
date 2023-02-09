using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x02000163 RID: 355
public class Marketplace : global::BaseEntity
{
	// Token: 0x06001682 RID: 5762 RVA: 0x000AB1D4 File Offset: 0x000A93D4
	public uint SendDrone(global::BasePlayer player, global::MarketTerminal sourceTerminal, global::VendingMachine vendingMachine)
	{
		if (sourceTerminal == null || vendingMachine == null)
		{
			return 0U;
		}
		GameManager server = GameManager.server;
		GameObjectRef gameObjectRef = this.deliveryDronePrefab;
		global::BaseEntity baseEntity = server.CreateEntity((gameObjectRef != null) ? gameObjectRef.resourcePath : null, this.droneLaunchPoint.position, this.droneLaunchPoint.rotation, true);
		global::DeliveryDrone deliveryDrone;
		if ((deliveryDrone = (baseEntity as global::DeliveryDrone)) == null)
		{
			baseEntity.Kill(global::BaseNetworkable.DestroyMode.None);
			return 0U;
		}
		deliveryDrone.OwnerID = player.userID;
		deliveryDrone.Spawn();
		deliveryDrone.Setup(this, sourceTerminal, vendingMachine);
		return deliveryDrone.net.ID;
	}

	// Token: 0x06001683 RID: 5763 RVA: 0x000AB264 File Offset: 0x000A9464
	public void ReturnDrone(global::DeliveryDrone deliveryDrone)
	{
		global::MarketTerminal marketTerminal;
		if (deliveryDrone.sourceTerminal.TryGet(true, out marketTerminal))
		{
			marketTerminal.CompleteOrder(deliveryDrone.targetVendingMachine.uid);
		}
		deliveryDrone.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06001684 RID: 5764 RVA: 0x000AB299 File Offset: 0x000A9499
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			this.SpawnSubEntities();
		}
	}

	// Token: 0x06001685 RID: 5765 RVA: 0x000AB2B0 File Offset: 0x000A94B0
	private void SpawnSubEntities()
	{
		if (!base.isServer)
		{
			return;
		}
		if (this.terminalEntities != null && this.terminalEntities.Length > this.terminalPoints.Length)
		{
			for (int i = this.terminalPoints.Length; i < this.terminalEntities.Length; i++)
			{
				global::MarketTerminal marketTerminal;
				if (this.terminalEntities[i].TryGet(true, out marketTerminal))
				{
					marketTerminal.Kill(global::BaseNetworkable.DestroyMode.None);
				}
			}
		}
		Array.Resize<EntityRef<global::MarketTerminal>>(ref this.terminalEntities, this.terminalPoints.Length);
		for (int j = 0; j < this.terminalPoints.Length; j++)
		{
			Transform transform = this.terminalPoints[j];
			global::MarketTerminal marketTerminal2;
			if (!this.terminalEntities[j].TryGet(true, out marketTerminal2))
			{
				GameManager server = GameManager.server;
				GameObjectRef gameObjectRef = this.terminalPrefab;
				global::BaseEntity baseEntity = server.CreateEntity((gameObjectRef != null) ? gameObjectRef.resourcePath : null, transform.position, transform.rotation, true);
				baseEntity.SetParent(this, true, false);
				baseEntity.Spawn();
				global::MarketTerminal marketTerminal3;
				if ((marketTerminal3 = (baseEntity as global::MarketTerminal)) == null)
				{
					Debug.LogError("Marketplace.terminalPrefab did not spawn a MarketTerminal (it spawned " + baseEntity.GetType().FullName + ")");
					baseEntity.Kill(global::BaseNetworkable.DestroyMode.None);
				}
				else
				{
					marketTerminal3.Setup(this);
					this.terminalEntities[j].Set(marketTerminal3);
				}
			}
		}
	}

	// Token: 0x06001686 RID: 5766 RVA: 0x000AB3F4 File Offset: 0x000A95F4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.subEntityList != null)
		{
			List<uint> subEntityIds = info.msg.subEntityList.subEntityIds;
			Array.Resize<EntityRef<global::MarketTerminal>>(ref this.terminalEntities, subEntityIds.Count);
			for (int i = 0; i < subEntityIds.Count; i++)
			{
				this.terminalEntities[i] = new EntityRef<global::MarketTerminal>(subEntityIds[i]);
			}
		}
		this.SpawnSubEntities();
	}

	// Token: 0x06001687 RID: 5767 RVA: 0x000AB468 File Offset: 0x000A9668
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.subEntityList = Pool.Get<SubEntityList>();
		info.msg.subEntityList.subEntityIds = Pool.GetList<uint>();
		if (this.terminalEntities != null)
		{
			for (int i = 0; i < this.terminalEntities.Length; i++)
			{
				info.msg.subEntityList.subEntityIds.Add(this.terminalEntities[i].uid);
			}
		}
	}

	// Token: 0x04000F6F RID: 3951
	[Header("Marketplace")]
	public GameObjectRef terminalPrefab;

	// Token: 0x04000F70 RID: 3952
	public Transform[] terminalPoints;

	// Token: 0x04000F71 RID: 3953
	public Transform droneLaunchPoint;

	// Token: 0x04000F72 RID: 3954
	public GameObjectRef deliveryDronePrefab;

	// Token: 0x04000F73 RID: 3955
	public EntityRef<global::MarketTerminal>[] terminalEntities;
}
