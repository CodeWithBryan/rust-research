using System;
using ConVar;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000387 RID: 903
public class BaseCorpse : BaseCombatEntity
{
	// Token: 0x06001F9A RID: 8090 RVA: 0x000D09F0 File Offset: 0x000CEBF0
	public override void ServerInit()
	{
		this.SetupRigidBody();
		this.ResetRemovalTime();
		this.resourceDispenser = base.GetComponent<ResourceDispenser>();
		base.ServerInit();
	}

	// Token: 0x06001F9B RID: 8091 RVA: 0x000D0A11 File Offset: 0x000CEC11
	public virtual void InitCorpse(global::BaseEntity pr)
	{
		this.parentEnt = pr;
		base.transform.SetPositionAndRotation(this.parentEnt.CenterPoint(), this.parentEnt.transform.rotation);
	}

	// Token: 0x06001F9C RID: 8092 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool CanRemove()
	{
		return true;
	}

	// Token: 0x06001F9D RID: 8093 RVA: 0x000D0A40 File Offset: 0x000CEC40
	public void RemoveCorpse()
	{
		if (!this.CanRemove())
		{
			this.ResetRemovalTime();
			return;
		}
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06001F9E RID: 8094 RVA: 0x000D0A58 File Offset: 0x000CEC58
	public void ResetRemovalTime(float dur)
	{
		using (TimeWarning.New("ResetRemovalTime", 0))
		{
			if (base.IsInvoking(new Action(this.RemoveCorpse)))
			{
				base.CancelInvoke(new Action(this.RemoveCorpse));
			}
			base.Invoke(new Action(this.RemoveCorpse), dur);
		}
	}

	// Token: 0x06001F9F RID: 8095 RVA: 0x000D0AC8 File Offset: 0x000CECC8
	public virtual float GetRemovalTime()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode != null)
		{
			return activeGameMode.CorpseRemovalTime(this);
		}
		return Server.corpsedespawn;
	}

	// Token: 0x06001FA0 RID: 8096 RVA: 0x000D0AF2 File Offset: 0x000CECF2
	public void ResetRemovalTime()
	{
		this.ResetRemovalTime(this.GetRemovalTime());
	}

	// Token: 0x06001FA1 RID: 8097 RVA: 0x000D0B00 File Offset: 0x000CED00
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.corpse = Facepunch.Pool.Get<Corpse>();
		if (this.parentEnt.IsValid())
		{
			info.msg.corpse.parentID = this.parentEnt.net.ID;
		}
	}

	// Token: 0x06001FA2 RID: 8098 RVA: 0x000D0B54 File Offset: 0x000CED54
	public void TakeChildren(global::BaseEntity takeChildrenFrom)
	{
		if (takeChildrenFrom.children == null)
		{
			return;
		}
		using (TimeWarning.New("Corpse.TakeChildren", 0))
		{
			global::BaseEntity[] array = takeChildrenFrom.children.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SwitchParent(this);
			}
		}
	}

	// Token: 0x06001FA3 RID: 8099 RVA: 0x000059DD File Offset: 0x00003BDD
	public override void ApplyInheritedVelocity(Vector3 velocity)
	{
	}

	// Token: 0x06001FA4 RID: 8100 RVA: 0x000D0BB8 File Offset: 0x000CEDB8
	private Rigidbody SetupRigidBody()
	{
		if (base.isServer)
		{
			GameObject gameObject = base.gameManager.FindPrefab(this.prefabRagdoll.resourcePath);
			if (gameObject == null)
			{
				return null;
			}
			Ragdoll component = gameObject.GetComponent<Ragdoll>();
			if (component == null)
			{
				return null;
			}
			if (component.primaryBody == null)
			{
				Debug.LogError("[BaseCorpse] ragdoll.primaryBody isn't set!" + component.gameObject.name);
				return null;
			}
			BoxCollider component2 = component.primaryBody.GetComponent<BoxCollider>();
			if (component2 == null)
			{
				Debug.LogError("Ragdoll has unsupported primary collider (make it supported) ", component);
				return null;
			}
			BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
			boxCollider.size = component2.size * 2f;
			boxCollider.center = component2.center;
			boxCollider.sharedMaterial = component2.sharedMaterial;
		}
		Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
		if (rigidbody == null)
		{
			Debug.LogError("[BaseCorpse] already has a RigidBody defined - and it shouldn't!" + base.gameObject.name);
			return null;
		}
		rigidbody.mass = 10f;
		rigidbody.useGravity = true;
		rigidbody.drag = 0.5f;
		rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
		if (base.isServer)
		{
			Buoyancy component3 = base.GetComponent<Buoyancy>();
			if (component3 != null)
			{
				component3.rigidBody = rigidbody;
			}
			ConVar.Physics.ApplyDropped(rigidbody);
			Vector3 velocity = Vector3Ex.Range(-1f, 1f);
			velocity.y += 1f;
			rigidbody.velocity = velocity;
			rigidbody.angularVelocity = Vector3Ex.Range(-10f, 10f);
		}
		return rigidbody;
	}

	// Token: 0x06001FA5 RID: 8101 RVA: 0x000D0D44 File Offset: 0x000CEF44
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.corpse != null)
		{
			this.Load(info.msg.corpse);
		}
	}

	// Token: 0x06001FA6 RID: 8102 RVA: 0x000D0D6B File Offset: 0x000CEF6B
	private void Load(Corpse corpse)
	{
		if (base.isServer)
		{
			this.parentEnt = (global::BaseNetworkable.serverEntities.Find(corpse.parentID) as global::BaseEntity);
		}
		bool isClient = base.isClient;
	}

	// Token: 0x06001FA7 RID: 8103 RVA: 0x000D0D97 File Offset: 0x000CEF97
	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer)
		{
			this.ResetRemovalTime();
			if (this.resourceDispenser)
			{
				this.resourceDispenser.OnAttacked(info);
			}
			if (!info.DidGather)
			{
				base.OnAttacked(info);
			}
		}
	}

	// Token: 0x06001FA8 RID: 8104 RVA: 0x000D0DCF File Offset: 0x000CEFCF
	public override string Categorize()
	{
		return "corpse";
	}

	// Token: 0x1700026D RID: 621
	// (get) Token: 0x06001FA9 RID: 8105 RVA: 0x000D0DD6 File Offset: 0x000CEFD6
	public override global::BaseEntity.TraitFlag Traits
	{
		get
		{
			return base.Traits | global::BaseEntity.TraitFlag.Food | global::BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x06001FAA RID: 8106 RVA: 0x000D0DE4 File Offset: 0x000CEFE4
	public override void Eat(BaseNpc baseNpc, float timeSpent)
	{
		this.ResetRemovalTime();
		base.Hurt(timeSpent * 5f);
		baseNpc.AddCalories(timeSpent * 2f);
	}

	// Token: 0x06001FAB RID: 8107 RVA: 0x00007074 File Offset: 0x00005274
	public override bool ShouldInheritNetworkGroup()
	{
		return false;
	}

	// Token: 0x040018E4 RID: 6372
	public GameObjectRef prefabRagdoll;

	// Token: 0x040018E5 RID: 6373
	public global::BaseEntity parentEnt;

	// Token: 0x040018E6 RID: 6374
	[NonSerialized]
	internal ResourceDispenser resourceDispenser;
}
