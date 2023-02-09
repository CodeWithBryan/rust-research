using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x020001D7 RID: 471
public class BasePet : NPCPlayer, IThinker
{
	// Token: 0x170001DF RID: 479
	// (get) Token: 0x060018B6 RID: 6326 RVA: 0x000B511D File Offset: 0x000B331D
	// (set) Token: 0x060018B7 RID: 6327 RVA: 0x000B5125 File Offset: 0x000B3325
	public PetBrain Brain { get; protected set; }

	// Token: 0x060018B8 RID: 6328 RVA: 0x000271CD File Offset: 0x000253CD
	public override float StartHealth()
	{
		return this.startHealth;
	}

	// Token: 0x060018B9 RID: 6329 RVA: 0x000271CD File Offset: 0x000253CD
	public override float StartMaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x060018BA RID: 6330 RVA: 0x000271C5 File Offset: 0x000253C5
	public override float MaxHealth()
	{
		return this._maxHealth;
	}

	// Token: 0x060018BB RID: 6331 RVA: 0x000B5130 File Offset: 0x000B3330
	public static void ProcessMovementQueue()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = BasePet.movementupdatebudgetms / 1000f;
		while (BasePet._movementProcessQueue.Count > 0 && Time.realtimeSinceStartup < realtimeSinceStartup + num)
		{
			BasePet basePet = BasePet._movementProcessQueue.Dequeue();
			if (basePet != null)
			{
				basePet.DoBudgetedMoveUpdate();
				basePet.inQueue = false;
			}
		}
	}

	// Token: 0x060018BC RID: 6332 RVA: 0x000B518A File Offset: 0x000B338A
	public void DoBudgetedMoveUpdate()
	{
		if (this.Brain != null)
		{
			this.Brain.DoMovementTick();
		}
	}

	// Token: 0x060018BD RID: 6333 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsLoadBalanced()
	{
		return true;
	}

	// Token: 0x060018BE RID: 6334 RVA: 0x000B51A5 File Offset: 0x000B33A5
	public override void ServerInit()
	{
		base.ServerInit();
		this.Brain = base.GetComponent<PetBrain>();
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.AddPet(this);
	}

	// Token: 0x060018BF RID: 6335 RVA: 0x000B51C8 File Offset: 0x000B33C8
	public void CreateMapMarker()
	{
		if (this._mapMarkerInstance != null)
		{
			this._mapMarkerInstance.Kill(BaseNetworkable.DestroyMode.None);
		}
		GameManager server = GameManager.server;
		GameObjectRef gameObjectRef = this.mapMarkerPrefab;
		BaseEntity baseEntity = server.CreateEntity((gameObjectRef != null) ? gameObjectRef.resourcePath : null, Vector3.zero, Quaternion.identity, true);
		baseEntity.OwnerID = base.OwnerID;
		baseEntity.Spawn();
		baseEntity.SetParent(this, false, false);
		this._mapMarkerInstance = baseEntity;
	}

	// Token: 0x060018C0 RID: 6336 RVA: 0x000B5239 File Offset: 0x000B3439
	internal override void DoServerDestroy()
	{
		if (this.Brain.OwningPlayer != null)
		{
			this.Brain.OwningPlayer.ClearClientPetLink();
		}
		AIThinkManager.RemovePet(this);
		base.DoServerDestroy();
	}

	// Token: 0x060018C1 RID: 6337 RVA: 0x000B526A File Offset: 0x000B346A
	public virtual void TryThink()
	{
		base.ServerThink_Internal();
	}

	// Token: 0x060018C2 RID: 6338 RVA: 0x000B5272 File Offset: 0x000B3472
	public override void ServerThink(float delta)
	{
		base.ServerThink(delta);
		if (this.Brain.ShouldServerThink())
		{
			this.Brain.DoThink();
		}
	}

	// Token: 0x060018C3 RID: 6339 RVA: 0x000B5294 File Offset: 0x000B3494
	public void ApplyPetStatModifiers()
	{
		if (this.inventory == null)
		{
			return;
		}
		for (int i = 0; i < this.inventory.containerWear.capacity; i++)
		{
			Item slot = this.inventory.containerWear.GetSlot(i);
			if (slot != null)
			{
				ItemModPetStats component = slot.info.GetComponent<ItemModPetStats>();
				if (component != null)
				{
					component.Apply(this);
				}
			}
		}
		this.Heal(this.MaxHealth());
	}

	// Token: 0x060018C4 RID: 6340 RVA: 0x000B5308 File Offset: 0x000B3508
	private void OnPhysicsNeighbourChanged()
	{
		if (this.Brain != null && this.Brain.Navigator != null)
		{
			this.Brain.Navigator.ForceToGround();
		}
	}

	// Token: 0x040011B9 RID: 4537
	public static Dictionary<ulong, BasePet> ActivePetByOwnerID = new Dictionary<ulong, BasePet>();

	// Token: 0x040011BA RID: 4538
	[ServerVar]
	public static bool queuedMovementsAllowed = true;

	// Token: 0x040011BB RID: 4539
	[ServerVar]
	public static bool onlyQueueBaseNavMovements = true;

	// Token: 0x040011BC RID: 4540
	[ServerVar]
	[Help("How many miliseconds to budget for processing pet movements per frame")]
	public static float movementupdatebudgetms = 1f;

	// Token: 0x040011BD RID: 4541
	public float BaseAttackRate = 2f;

	// Token: 0x040011BE RID: 4542
	public float BaseAttackDamge = 20f;

	// Token: 0x040011BF RID: 4543
	public DamageType AttackDamageType = DamageType.Slash;

	// Token: 0x040011C1 RID: 4545
	public GameObjectRef mapMarkerPrefab;

	// Token: 0x040011C2 RID: 4546
	private BaseEntity _mapMarkerInstance;

	// Token: 0x040011C3 RID: 4547
	[HideInInspector]
	public bool inQueue;

	// Token: 0x040011C4 RID: 4548
	public static Queue<BasePet> _movementProcessQueue = new Queue<BasePet>();
}
