using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000AEB RID: 2795
	public class ItemModVehicleModule : ItemMod, VehicleModuleInformationPanel.IVehicleModuleInfo
	{
		// Token: 0x170005D6 RID: 1494
		// (get) Token: 0x0600432E RID: 17198 RVA: 0x00186526 File Offset: 0x00184726
		public int SocketsTaken
		{
			get
			{
				return this.socketsTaken;
			}
		}

		// Token: 0x0600432F RID: 17199 RVA: 0x00186530 File Offset: 0x00184730
		public BaseVehicleModule CreateModuleEntity(BaseEntity parent, Vector3 position, Quaternion rotation)
		{
			if (!this.entityPrefab.isValid)
			{
				Debug.LogError("Invalid entity prefab for module");
				return null;
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.entityPrefab.resourcePath, position, rotation, true);
			BaseVehicleModule baseVehicleModule = null;
			if (baseEntity != null)
			{
				if (parent != null)
				{
					baseEntity.SetParent(parent, true, false);
					baseEntity.canTriggerParent = false;
				}
				baseEntity.Spawn();
				baseVehicleModule = baseEntity.GetComponent<BaseVehicleModule>();
				if (this.doNonUserSpawn)
				{
					this.doNonUserSpawn = false;
					baseVehicleModule.NonUserSpawn();
				}
			}
			return baseVehicleModule;
		}

		// Token: 0x04003BC2 RID: 15298
		public GameObjectRef entityPrefab;

		// Token: 0x04003BC3 RID: 15299
		[Range(1f, 2f)]
		public int socketsTaken = 1;

		// Token: 0x04003BC4 RID: 15300
		public bool doNonUserSpawn;
	}
}
