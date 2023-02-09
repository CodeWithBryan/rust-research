using System;
using ConVar;
using UnityEngine;

// Token: 0x0200049B RID: 1179
public class DroppedItem : WorldItem
{
	// Token: 0x0600263C RID: 9788 RVA: 0x000058B6 File Offset: 0x00003AB6
	public override float GetNetworkTime()
	{
		return UnityEngine.Time.fixedTime;
	}

	// Token: 0x0600263D RID: 9789 RVA: 0x000EE41F File Offset: 0x000EC61F
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.GetDespawnDuration() < float.PositiveInfinity)
		{
			base.Invoke(new Action(this.IdleDestroy), this.GetDespawnDuration());
		}
		base.ReceiveCollisionMessages(true);
	}

	// Token: 0x0600263E RID: 9790 RVA: 0x000EE453 File Offset: 0x000EC653
	public virtual float GetDespawnDuration()
	{
		Item item = this.item;
		if (item == null)
		{
			return Server.itemdespawn;
		}
		return item.GetDespawnDuration();
	}

	// Token: 0x0600263F RID: 9791 RVA: 0x000EE46A File Offset: 0x000EC66A
	public void IdleDestroy()
	{
		base.DestroyItem();
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06002640 RID: 9792 RVA: 0x000EE47C File Offset: 0x000EC67C
	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		if (this.item == null)
		{
			return;
		}
		if (this.item.MaxStackable() <= 1)
		{
			return;
		}
		DroppedItem droppedItem = hitEntity as DroppedItem;
		if (droppedItem == null)
		{
			return;
		}
		if (droppedItem.item == null)
		{
			return;
		}
		if (droppedItem.item.info != this.item.info)
		{
			return;
		}
		droppedItem.OnDroppedOn(this);
	}

	// Token: 0x06002641 RID: 9793 RVA: 0x000EE4E0 File Offset: 0x000EC6E0
	public void OnDroppedOn(DroppedItem di)
	{
		if (this.item == null)
		{
			return;
		}
		if (di.item == null)
		{
			return;
		}
		if (di.item.info != this.item.info)
		{
			return;
		}
		if (di.item.IsBlueprint() && di.item.blueprintTarget != this.item.blueprintTarget)
		{
			return;
		}
		if ((di.item.hasCondition && di.item.condition != di.item.maxCondition) || (this.item.hasCondition && this.item.condition != this.item.maxCondition))
		{
			return;
		}
		if (di.item.info != null)
		{
			if (di.item.info.amountType == ItemDefinition.AmountType.Genetics)
			{
				int num = (di.item.instanceData != null) ? di.item.instanceData.dataInt : -1;
				int num2 = (this.item.instanceData != null) ? this.item.instanceData.dataInt : -1;
				if (num != num2)
				{
					return;
				}
			}
			if (di.item.info.GetComponent<ItemModSign>() != null && ItemModAssociatedEntity<SignContent>.GetAssociatedEntity(di.item, true) != null)
			{
				return;
			}
			if (this.item.info != null && this.item.info.GetComponent<ItemModSign>() != null && ItemModAssociatedEntity<SignContent>.GetAssociatedEntity(this.item, true) != null)
			{
				return;
			}
		}
		int num3 = di.item.amount + this.item.amount;
		if (num3 > this.item.MaxStackable())
		{
			return;
		}
		if (num3 == 0)
		{
			return;
		}
		di.DestroyItem();
		di.Kill(BaseNetworkable.DestroyMode.None);
		this.item.amount = num3;
		this.item.MarkDirty();
		if (this.GetDespawnDuration() < float.PositiveInfinity)
		{
			base.Invoke(new Action(this.IdleDestroy), this.GetDespawnDuration());
		}
		Effect.server.Run("assets/bundled/prefabs/fx/notice/stack.world.fx.prefab", this, 0U, Vector3.zero, Vector3.zero, null, false);
	}

	// Token: 0x06002642 RID: 9794 RVA: 0x000EE6F8 File Offset: 0x000EC8F8
	internal override void OnParentRemoved()
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component == null)
		{
			base.OnParentRemoved();
			return;
		}
		Vector3 vector = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		base.SetParent(null, false, false);
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(vector + Vector3.up * 2f, Vector3.down, out raycastHit, 2f, 27328512) && vector.y < raycastHit.point.y)
		{
			vector += Vector3.up * 1.5f;
		}
		base.transform.position = vector;
		base.transform.rotation = rotation;
		ConVar.Physics.ApplyDropped(component);
		component.isKinematic = false;
		component.useGravity = true;
		component.WakeUp();
		if (this.GetDespawnDuration() < float.PositiveInfinity)
		{
			base.Invoke(new Action(this.IdleDestroy), this.GetDespawnDuration());
		}
	}

	// Token: 0x06002643 RID: 9795 RVA: 0x000EE7EC File Offset: 0x000EC9EC
	public override void PostInitShared()
	{
		base.PostInitShared();
		GameObject gameObject;
		if (this.item != null && this.item.info.worldModelPrefab.isValid)
		{
			gameObject = this.item.info.worldModelPrefab.Instantiate(null);
		}
		else
		{
			gameObject = UnityEngine.Object.Instantiate<GameObject>(this.itemModel);
		}
		gameObject.transform.SetParent(base.transform, false);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.SetLayerRecursive(base.gameObject.layer);
		this.childCollider = gameObject.GetComponent<Collider>();
		if (this.childCollider)
		{
			this.childCollider.enabled = false;
			this.childCollider.enabled = true;
		}
		if (base.isServer)
		{
			WorldModel component = gameObject.GetComponent<WorldModel>();
			float mass = component ? component.mass : 1f;
			float drag = 0.1f;
			float angularDrag = 0.1f;
			Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
			rigidbody.mass = mass;
			rigidbody.drag = drag;
			rigidbody.angularDrag = angularDrag;
			rigidbody.interpolation = RigidbodyInterpolation.None;
			ConVar.Physics.ApplyDropped(rigidbody);
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}
		if (this.item != null)
		{
			PhysicsEffects component2 = base.gameObject.GetComponent<PhysicsEffects>();
			if (component2 != null)
			{
				component2.entity = this;
				if (this.item.info.physImpactSoundDef != null)
				{
					component2.physImpactSoundDef = this.item.info.physImpactSoundDef;
				}
			}
		}
		gameObject.SetActive(true);
	}

	// Token: 0x06002644 RID: 9796 RVA: 0x00007074 File Offset: 0x00005274
	public override bool ShouldInheritNetworkGroup()
	{
		return false;
	}

	// Token: 0x04001F0F RID: 7951
	[Header("DroppedItem")]
	public GameObject itemModel;

	// Token: 0x04001F10 RID: 7952
	private Collider childCollider;
}
