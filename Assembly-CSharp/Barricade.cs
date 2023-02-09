using System;
using Rust;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020003B0 RID: 944
public class Barricade : DecayEntity
{
	// Token: 0x06002073 RID: 8307 RVA: 0x000D3DC4 File Offset: 0x000D1FC4
	public override void ServerInit()
	{
		base.ServerInit();
		if (Barricade.nonWalkableArea < 0)
		{
			Barricade.nonWalkableArea = NavMesh.GetAreaFromName("Not Walkable");
		}
		if (Barricade.animalAgentTypeId < 0)
		{
			Barricade.animalAgentTypeId = NavMesh.GetSettingsByIndex(1).agentTypeID;
		}
		if (this.NavMeshVolumeAnimals == null)
		{
			this.NavMeshVolumeAnimals = base.gameObject.AddComponent<NavMeshModifierVolume>();
			this.NavMeshVolumeAnimals.area = Barricade.nonWalkableArea;
			this.NavMeshVolumeAnimals.AddAgentType(Barricade.animalAgentTypeId);
			this.NavMeshVolumeAnimals.center = Vector3.zero;
			this.NavMeshVolumeAnimals.size = Vector3.one;
		}
		if (!this.canNpcSmash)
		{
			if (Barricade.humanoidAgentTypeId < 0)
			{
				Barricade.humanoidAgentTypeId = NavMesh.GetSettingsByIndex(0).agentTypeID;
			}
			if (this.NavMeshVolumeHumanoids == null)
			{
				this.NavMeshVolumeHumanoids = base.gameObject.AddComponent<NavMeshModifierVolume>();
				this.NavMeshVolumeHumanoids.area = Barricade.nonWalkableArea;
				this.NavMeshVolumeHumanoids.AddAgentType(Barricade.humanoidAgentTypeId);
				this.NavMeshVolumeHumanoids.center = Vector3.zero;
				this.NavMeshVolumeHumanoids.size = Vector3.one;
				return;
			}
		}
		else if (this.NpcTriggerBox == null)
		{
			this.NpcTriggerBox = new GameObject("NpcTriggerBox").AddComponent<NPCBarricadeTriggerBox>();
			this.NpcTriggerBox.Setup(this);
		}
	}

	// Token: 0x06002074 RID: 8308 RVA: 0x000D3F20 File Offset: 0x000D2120
	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer && info.WeaponPrefab is BaseMelee && !info.IsProjectile())
		{
			BasePlayer basePlayer = info.Initiator as BasePlayer;
			if (basePlayer && this.reflectDamage > 0f)
			{
				basePlayer.Hurt(this.reflectDamage * UnityEngine.Random.Range(0.75f, 1.25f), DamageType.Stab, this, true);
				if (this.reflectEffect.isValid)
				{
					Effect.server.Run(this.reflectEffect.resourcePath, basePlayer, StringPool.closest, base.transform.position, Vector3.up, null, false);
				}
			}
		}
		base.OnAttacked(info);
	}

	// Token: 0x04001946 RID: 6470
	public float reflectDamage = 5f;

	// Token: 0x04001947 RID: 6471
	public GameObjectRef reflectEffect;

	// Token: 0x04001948 RID: 6472
	public bool canNpcSmash = true;

	// Token: 0x04001949 RID: 6473
	public NavMeshModifierVolume NavMeshVolumeAnimals;

	// Token: 0x0400194A RID: 6474
	public NavMeshModifierVolume NavMeshVolumeHumanoids;

	// Token: 0x0400194B RID: 6475
	[NonSerialized]
	public NPCBarricadeTriggerBox NpcTriggerBox;

	// Token: 0x0400194C RID: 6476
	private static int nonWalkableArea = -1;

	// Token: 0x0400194D RID: 6477
	private static int animalAgentTypeId = -1;

	// Token: 0x0400194E RID: 6478
	private static int humanoidAgentTypeId = -1;
}
