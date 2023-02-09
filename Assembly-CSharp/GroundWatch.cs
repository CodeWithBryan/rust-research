using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x020004EE RID: 1262
public class GroundWatch : BaseMonoBehaviour, IServerComponent
{
	// Token: 0x0600280B RID: 10251 RVA: 0x000F51D1 File Offset: 0x000F33D1
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(this.groundPosition, this.radius);
	}

	// Token: 0x0600280C RID: 10252 RVA: 0x000F5200 File Offset: 0x000F3400
	public static void PhysicsChanged(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		Collider component = obj.GetComponent<Collider>();
		if (!component)
		{
			return;
		}
		Bounds bounds = component.bounds;
		List<BaseEntity> list = Facepunch.Pool.GetList<BaseEntity>();
		global::Vis.Entities<BaseEntity>(bounds.center, bounds.extents.magnitude + 1f, list, 2263296, QueryTriggerInteraction.Collide);
		foreach (BaseEntity baseEntity in list)
		{
			if (!baseEntity.IsDestroyed && !baseEntity.isClient && !(baseEntity is BuildingBlock))
			{
				baseEntity.BroadcastMessage("OnPhysicsNeighbourChanged", SendMessageOptions.DontRequireReceiver);
			}
		}
		Facepunch.Pool.FreeList<BaseEntity>(ref list);
	}

	// Token: 0x0600280D RID: 10253 RVA: 0x000F52C8 File Offset: 0x000F34C8
	public static void PhysicsChanged(Vector3 origin, float radius, int layerMask)
	{
		List<BaseEntity> list = Facepunch.Pool.GetList<BaseEntity>();
		global::Vis.Entities<BaseEntity>(origin, radius, list, layerMask, QueryTriggerInteraction.Collide);
		foreach (BaseEntity baseEntity in list)
		{
			if (!baseEntity.IsDestroyed && !baseEntity.isClient && !(baseEntity is BuildingBlock))
			{
				baseEntity.BroadcastMessage("OnPhysicsNeighbourChanged", SendMessageOptions.DontRequireReceiver);
			}
		}
		Facepunch.Pool.FreeList<BaseEntity>(ref list);
	}

	// Token: 0x0600280E RID: 10254 RVA: 0x000F534C File Offset: 0x000F354C
	private void OnPhysicsNeighbourChanged()
	{
		if (!this.OnGround())
		{
			this.fails++;
			if (this.fails < ConVar.Physics.groundwatchfails)
			{
				if (ConVar.Physics.groundwatchdebug)
				{
					Debug.Log("GroundWatch retry: " + this.fails);
				}
				base.Invoke(new Action(this.OnPhysicsNeighbourChanged), ConVar.Physics.groundwatchdelay);
				return;
			}
			BaseEntity baseEntity = base.gameObject.ToBaseEntity();
			if (baseEntity)
			{
				baseEntity.transform.BroadcastMessage("OnGroundMissing", SendMessageOptions.DontRequireReceiver);
				return;
			}
		}
		else
		{
			this.fails = 0;
		}
	}

	// Token: 0x0600280F RID: 10255 RVA: 0x000F53E4 File Offset: 0x000F35E4
	private bool OnGround()
	{
		BaseEntity component = base.GetComponent<BaseEntity>();
		if (component)
		{
			Construction construction = PrefabAttribute.server.Find<Construction>(component.prefabID);
			if (construction)
			{
				Socket_Base[] allSockets = construction.allSockets;
				for (int i = 0; i < allSockets.Length; i++)
				{
					SocketMod[] socketMods = allSockets[i].socketMods;
					for (int j = 0; j < socketMods.Length; j++)
					{
						SocketMod_AreaCheck socketMod_AreaCheck = socketMods[j] as SocketMod_AreaCheck;
						if (socketMod_AreaCheck && socketMod_AreaCheck.wantsInside && !socketMod_AreaCheck.DoCheck(component.transform.position, component.transform.rotation, component))
						{
							if (ConVar.Physics.groundwatchdebug)
							{
								Debug.Log("GroundWatch failed: " + socketMod_AreaCheck.hierachyName);
							}
							return false;
						}
					}
				}
			}
		}
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		global::Vis.Colliders<Collider>(base.transform.TransformPoint(this.groundPosition), this.radius, list, this.layers, QueryTriggerInteraction.Collide);
		foreach (Collider collider in list)
		{
			BaseEntity baseEntity = collider.gameObject.ToBaseEntity();
			if (!baseEntity || (!(baseEntity == component) && !baseEntity.IsDestroyed && !baseEntity.isClient))
			{
				if (this.whitelist != null && this.whitelist.Length != 0)
				{
					bool flag = false;
					foreach (BaseEntity baseEntity2 in this.whitelist)
					{
						if (baseEntity.prefabID == baseEntity2.prefabID)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						continue;
					}
				}
				DecayEntity decayEntity = component as DecayEntity;
				DecayEntity decayEntity2 = baseEntity as DecayEntity;
				if (!decayEntity || decayEntity.buildingID == 0U || !decayEntity2 || decayEntity2.buildingID == 0U || decayEntity.buildingID == decayEntity2.buildingID)
				{
					Facepunch.Pool.FreeList<Collider>(ref list);
					return true;
				}
			}
		}
		if (ConVar.Physics.groundwatchdebug)
		{
			Debug.Log("GroundWatch failed: Legacy radius check");
		}
		Facepunch.Pool.FreeList<Collider>(ref list);
		return false;
	}

	// Token: 0x04002047 RID: 8263
	public Vector3 groundPosition = Vector3.zero;

	// Token: 0x04002048 RID: 8264
	public LayerMask layers = 27328512;

	// Token: 0x04002049 RID: 8265
	public float radius = 0.1f;

	// Token: 0x0400204A RID: 8266
	[Header("Whitelist")]
	public BaseEntity[] whitelist;

	// Token: 0x0400204B RID: 8267
	private int fails;
}
