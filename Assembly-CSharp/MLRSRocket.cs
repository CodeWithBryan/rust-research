using System;
using Rust;
using UnityEngine;

// Token: 0x02000463 RID: 1123
public class MLRSRocket : TimedExplosive, SamSite.ISamSiteTarget
{
	// Token: 0x170002C9 RID: 713
	// (get) Token: 0x060024C6 RID: 9414 RVA: 0x000E802F File Offset: 0x000E622F
	public SamSite.SamTargetType SAMTargetType
	{
		get
		{
			return SamSite.targetTypeMissile;
		}
	}

	// Token: 0x060024C7 RID: 9415 RVA: 0x000E8036 File Offset: 0x000E6236
	public override void ServerInit()
	{
		base.ServerInit();
		this.CreateMapMarker();
		Effect.server.Run(this.launchBlastFXPrefab.resourcePath, base.PivotPoint(), base.transform.up, null, true);
	}

	// Token: 0x060024C8 RID: 9416 RVA: 0x000E8068 File Offset: 0x000E6268
	public override void ProjectileImpact(RaycastHit info, Vector3 rayOrigin)
	{
		this.Explode(rayOrigin);
		if (Physics.Raycast(info.point + Vector3.up, Vector3.down, 4f, 1218511121, QueryTriggerInteraction.Ignore))
		{
			Effect.server.Run(this.explosionGroundFXPrefab.resourcePath, info.point, Vector3.up, null, true);
		}
	}

	// Token: 0x060024C9 RID: 9417 RVA: 0x000E80C4 File Offset: 0x000E62C4
	private void CreateMapMarker()
	{
		BaseEntity baseEntity = this.mapMarkerInstanceRef.Get(base.isServer);
		if (baseEntity.IsValid())
		{
			baseEntity.Kill(BaseNetworkable.DestroyMode.None);
		}
		GameManager server = GameManager.server;
		GameObjectRef gameObjectRef = this.mapMarkerPrefab;
		BaseEntity baseEntity2 = server.CreateEntity((gameObjectRef != null) ? gameObjectRef.resourcePath : null, base.transform.position, Quaternion.identity, true);
		baseEntity2.OwnerID = base.OwnerID;
		baseEntity2.Spawn();
		baseEntity2.SetParent(this, true, false);
		this.mapMarkerInstanceRef.Set(baseEntity2);
	}

	// Token: 0x060024CA RID: 9418 RVA: 0x00003A54 File Offset: 0x00001C54
	public bool IsValidSAMTarget(bool staticRespawn)
	{
		return true;
	}

	// Token: 0x060024CB RID: 9419 RVA: 0x000E8147 File Offset: 0x000E6347
	public override Vector3 GetLocalVelocityServer()
	{
		return this.serverProjectile.CurrentVelocity;
	}

	// Token: 0x060024CC RID: 9420 RVA: 0x000E8154 File Offset: 0x000E6354
	private void OnTriggerEnter(Collider other)
	{
		if (!other.IsOnLayer(Layer.Trigger))
		{
			return;
		}
		if (other.CompareTag("MLRSRocketTrigger"))
		{
			this.Explode();
			TimedExplosive componentInParent = other.GetComponentInParent<TimedExplosive>();
			if (componentInParent != null)
			{
				componentInParent.Explode();
				return;
			}
		}
		else if (other.GetComponent<TriggerSafeZone>() != null)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x04001D73 RID: 7539
	[SerializeField]
	private GameObjectRef mapMarkerPrefab;

	// Token: 0x04001D74 RID: 7540
	[SerializeField]
	private GameObjectRef launchBlastFXPrefab;

	// Token: 0x04001D75 RID: 7541
	[SerializeField]
	private GameObjectRef explosionGroundFXPrefab;

	// Token: 0x04001D76 RID: 7542
	[SerializeField]
	private ServerProjectile serverProjectile;

	// Token: 0x04001D77 RID: 7543
	private EntityRef mapMarkerInstanceRef;
}
