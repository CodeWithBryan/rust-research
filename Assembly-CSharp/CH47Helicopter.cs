using System;
using UnityEngine;

// Token: 0x02000459 RID: 1113
public class CH47Helicopter : BaseHelicopterVehicle
{
	// Token: 0x06002487 RID: 9351 RVA: 0x000E6BB3 File Offset: 0x000E4DB3
	public override void ServerInit()
	{
		this.rigidBody.isKinematic = false;
		base.ServerInit();
		this.CreateMapMarker();
	}

	// Token: 0x06002488 RID: 9352 RVA: 0x000E6BCD File Offset: 0x000E4DCD
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
	}

	// Token: 0x06002489 RID: 9353 RVA: 0x000E6BD8 File Offset: 0x000E4DD8
	public void CreateMapMarker()
	{
		if (this.mapMarkerInstance)
		{
			this.mapMarkerInstance.Kill(BaseNetworkable.DestroyMode.None);
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.mapMarkerEntityPrefab.resourcePath, Vector3.zero, Quaternion.identity, true);
		baseEntity.Spawn();
		baseEntity.SetParent(this, false, false);
		this.mapMarkerInstance = baseEntity;
	}

	// Token: 0x0600248A RID: 9354 RVA: 0x00007074 File Offset: 0x00005274
	protected override bool CanPushNow(BasePlayer pusher)
	{
		return false;
	}

	// Token: 0x04001D2B RID: 7467
	public GameObjectRef mapMarkerEntityPrefab;

	// Token: 0x04001D2C RID: 7468
	private BaseEntity mapMarkerInstance;
}
