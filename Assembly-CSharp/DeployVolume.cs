using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020004CB RID: 1227
public abstract class DeployVolume : PrefabAttribute
{
	// Token: 0x17000326 RID: 806
	// (get) Token: 0x0600276D RID: 10093 RVA: 0x000F297D File Offset: 0x000F0B7D
	// (set) Token: 0x0600276E RID: 10094 RVA: 0x000F2985 File Offset: 0x000F0B85
	public bool IsBuildingBlock { get; set; }

	// Token: 0x0600276F RID: 10095 RVA: 0x000F298E File Offset: 0x000F0B8E
	protected override Type GetIndexedType()
	{
		return typeof(DeployVolume);
	}

	// Token: 0x06002770 RID: 10096 RVA: 0x000F299A File Offset: 0x000F0B9A
	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		this.IsBuildingBlock = (rootObj.GetComponent<BuildingBlock>() != null);
	}

	// Token: 0x06002771 RID: 10097
	protected abstract bool Check(Vector3 position, Quaternion rotation, int mask = -1);

	// Token: 0x06002772 RID: 10098
	protected abstract bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1);

	// Token: 0x06002773 RID: 10099 RVA: 0x000F29C0 File Offset: 0x000F0BC0
	public static bool Check(Vector3 position, Quaternion rotation, DeployVolume[] volumes, int mask = -1)
	{
		for (int i = 0; i < volumes.Length; i++)
		{
			if (volumes[i].Check(position, rotation, mask))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002774 RID: 10100 RVA: 0x000F29EC File Offset: 0x000F0BEC
	public static bool Check(Vector3 position, Quaternion rotation, DeployVolume[] volumes, OBB test, int mask = -1)
	{
		for (int i = 0; i < volumes.Length; i++)
		{
			if (volumes[i].Check(position, rotation, test, mask))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002775 RID: 10101 RVA: 0x000F2A1C File Offset: 0x000F0C1C
	public static bool CheckSphere(Vector3 pos, float radius, int layerMask, DeployVolume volume)
	{
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, radius, list, layerMask, QueryTriggerInteraction.Collide);
		bool result = DeployVolume.CheckFlags(list, volume);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x06002776 RID: 10102 RVA: 0x000F2A48 File Offset: 0x000F0C48
	public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layerMask, DeployVolume volume)
	{
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapCapsule(start, end, radius, list, layerMask, QueryTriggerInteraction.Collide);
		bool result = DeployVolume.CheckFlags(list, volume);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x06002777 RID: 10103 RVA: 0x000F2A78 File Offset: 0x000F0C78
	public static bool CheckOBB(OBB obb, int layerMask, DeployVolume volume)
	{
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapOBB(obb, list, layerMask, QueryTriggerInteraction.Collide);
		bool result = DeployVolume.CheckFlags(list, volume);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x06002778 RID: 10104 RVA: 0x000F2AA4 File Offset: 0x000F0CA4
	public static bool CheckBounds(Bounds bounds, int layerMask, DeployVolume volume)
	{
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapBounds(bounds, list, layerMask, QueryTriggerInteraction.Collide);
		bool result = DeployVolume.CheckFlags(list, volume);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x17000327 RID: 807
	// (get) Token: 0x06002779 RID: 10105 RVA: 0x000F2ACE File Offset: 0x000F0CCE
	// (set) Token: 0x0600277A RID: 10106 RVA: 0x000F2AD5 File Offset: 0x000F0CD5
	public static Collider LastDeployHit { get; private set; }

	// Token: 0x0600277B RID: 10107 RVA: 0x000F2AE0 File Offset: 0x000F0CE0
	private static bool CheckFlags(List<Collider> list, DeployVolume volume)
	{
		DeployVolume.LastDeployHit = null;
		for (int i = 0; i < list.Count; i++)
		{
			DeployVolume.LastDeployHit = list[i];
			GameObject gameObject = list[i].gameObject;
			if (!gameObject.CompareTag("DeployVolumeIgnore"))
			{
				ColliderInfo component = gameObject.GetComponent<ColliderInfo>();
				if ((!(component != null) || !component.HasFlag(ColliderInfo.Flags.OnlyBlockBuildingBlock) || volume.IsBuildingBlock) && (component == null || volume.ignore == (ColliderInfo.Flags)0 || !component.HasFlag(volume.ignore)))
				{
					if (volume.entityList.Length == 0)
					{
						return true;
					}
					BaseEntity baseEntity = list[i].ToBaseEntity();
					bool flag = false;
					if (baseEntity != null)
					{
						foreach (BaseEntity baseEntity2 in volume.entityList)
						{
							if (baseEntity.prefabID == baseEntity2.prefabID)
							{
								flag = true;
								break;
							}
						}
					}
					if (volume.entityMode == DeployVolume.EntityMode.IncludeList)
					{
						if (flag)
						{
							return true;
						}
					}
					else if (volume.entityMode == DeployVolume.EntityMode.ExcludeList && !flag)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x04001FB3 RID: 8115
	public LayerMask layers = 537001984;

	// Token: 0x04001FB4 RID: 8116
	[global::InspectorFlags]
	public ColliderInfo.Flags ignore;

	// Token: 0x04001FB5 RID: 8117
	public DeployVolume.EntityMode entityMode;

	// Token: 0x04001FB6 RID: 8118
	[FormerlySerializedAs("entities")]
	public BaseEntity[] entityList;

	// Token: 0x02000CD8 RID: 3288
	public enum EntityMode
	{
		// Token: 0x040043FF RID: 17407
		ExcludeList,
		// Token: 0x04004400 RID: 17408
		IncludeList
	}
}
