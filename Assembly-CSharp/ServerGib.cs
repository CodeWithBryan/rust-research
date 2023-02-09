using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020003FC RID: 1020
public class ServerGib : BaseCombatEntity
{
	// Token: 0x0600226B RID: 8811 RVA: 0x000A4DF3 File Offset: 0x000A2FF3
	public override float BoundsPadding()
	{
		return 3f;
	}

	// Token: 0x0600226C RID: 8812 RVA: 0x000DC97C File Offset: 0x000DAB7C
	public static List<global::ServerGib> CreateGibs(string entityToCreatePath, GameObject creator, GameObject gibSource, Vector3 inheritVelocity, float spreadVelocity)
	{
		List<global::ServerGib> list = new List<global::ServerGib>();
		foreach (MeshRenderer meshRenderer in gibSource.GetComponentsInChildren<MeshRenderer>(true))
		{
			MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
			Vector3 normalized = meshRenderer.transform.localPosition.normalized;
			Vector3 vector = creator.transform.localToWorldMatrix.MultiplyPoint(meshRenderer.transform.localPosition) + normalized * 0.5f;
			Quaternion quaternion = creator.transform.rotation * meshRenderer.transform.localRotation;
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(entityToCreatePath, vector, quaternion, true);
			if (baseEntity)
			{
				global::ServerGib component2 = baseEntity.GetComponent<global::ServerGib>();
				component2.transform.SetPositionAndRotation(vector, quaternion);
				component2._gibName = meshRenderer.name;
				MeshCollider component3 = meshRenderer.GetComponent<MeshCollider>();
				Mesh physicsMesh = (component3 != null) ? component3.sharedMesh : component.sharedMesh;
				component2.PhysicsInit(physicsMesh);
				Vector3 b = meshRenderer.transform.localPosition.normalized * spreadVelocity;
				component2.rigidBody.velocity = inheritVelocity + b;
				component2.rigidBody.angularVelocity = Vector3Ex.Range(-1f, 1f).normalized * 1f;
				component2.rigidBody.WakeUp();
				component2.Spawn();
				list.Add(component2);
			}
		}
		foreach (global::ServerGib serverGib in list)
		{
			foreach (global::ServerGib serverGib2 in list)
			{
				if (!(serverGib == serverGib2))
				{
					Physics.IgnoreCollision(serverGib2.GetCollider(), serverGib.GetCollider(), true);
				}
			}
		}
		return list;
	}

	// Token: 0x0600226D RID: 8813 RVA: 0x000DCB9C File Offset: 0x000DAD9C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk && this._gibName != "")
		{
			info.msg.servergib = Pool.Get<ProtoBuf.ServerGib>();
			info.msg.servergib.gibName = this._gibName;
		}
	}

	// Token: 0x0600226E RID: 8814 RVA: 0x000DCBF0 File Offset: 0x000DADF0
	public MeshCollider GetCollider()
	{
		return this.meshCollider;
	}

	// Token: 0x0600226F RID: 8815 RVA: 0x000DCBF8 File Offset: 0x000DADF8
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.RemoveMe), 1800f);
	}

	// Token: 0x06002270 RID: 8816 RVA: 0x000029D4 File Offset: 0x00000BD4
	public void RemoveMe()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06002271 RID: 8817 RVA: 0x000DCC18 File Offset: 0x000DAE18
	public virtual void PhysicsInit(Mesh physicsMesh)
	{
		Mesh sharedMesh = null;
		MeshFilter component = base.gameObject.GetComponent<MeshFilter>();
		if (component != null)
		{
			sharedMesh = component.sharedMesh;
			component.sharedMesh = physicsMesh;
		}
		this.meshCollider = base.gameObject.AddComponent<MeshCollider>();
		this.meshCollider.sharedMesh = physicsMesh;
		this.meshCollider.convex = true;
		this.meshCollider.material = this.physicsMaterial;
		if (component != null)
		{
			component.sharedMesh = sharedMesh;
		}
		Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
		rigidbody.useGravity = true;
		rigidbody.mass = Mathf.Clamp(this.meshCollider.bounds.size.magnitude * this.meshCollider.bounds.size.magnitude * 20f, 10f, 2000f);
		rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		rigidbody.collisionDetectionMode = (this.useContinuousCollision ? CollisionDetectionMode.Continuous : CollisionDetectionMode.Discrete);
		if (base.isServer)
		{
			rigidbody.drag = 0.1f;
			rigidbody.angularDrag = 0.1f;
		}
		this.rigidBody = rigidbody;
		base.gameObject.layer = LayerMask.NameToLayer("Default");
		if (base.isClient)
		{
			rigidbody.isKinematic = true;
		}
	}

	// Token: 0x04001AEF RID: 6895
	public GameObject _gibSource;

	// Token: 0x04001AF0 RID: 6896
	public string _gibName;

	// Token: 0x04001AF1 RID: 6897
	public PhysicMaterial physicsMaterial;

	// Token: 0x04001AF2 RID: 6898
	public bool useContinuousCollision;

	// Token: 0x04001AF3 RID: 6899
	private MeshCollider meshCollider;

	// Token: 0x04001AF4 RID: 6900
	private Rigidbody rigidBody;
}
