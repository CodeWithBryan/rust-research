using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200023A RID: 570
public class Construction : PrefabAttribute
{
	// Token: 0x06001B02 RID: 6914 RVA: 0x000BD03C File Offset: 0x000BB23C
	public bool UpdatePlacement(Transform transform, Construction common, ref Construction.Target target)
	{
		if (!target.valid)
		{
			return false;
		}
		if (!common.canBypassBuildingPermission && !target.player.CanBuild())
		{
			Construction.lastPlacementError = "You don't have permission to build here";
			return false;
		}
		List<Socket_Base> list = Pool.GetList<Socket_Base>();
		common.FindMaleSockets(target, list);
		foreach (Socket_Base socket_Base in list)
		{
			Construction.Placement placement = null;
			if (!(target.entity != null) || !(target.socket != null) || !target.entity.IsOccupied(target.socket))
			{
				if (placement == null)
				{
					placement = socket_Base.DoPlacement(target);
				}
				if (placement != null)
				{
					if (!socket_Base.CheckSocketMods(placement))
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
					}
					else if (!this.TestPlacingThroughRock(ref placement, target))
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
						Construction.lastPlacementError = "Placing through rock";
					}
					else if (!Construction.TestPlacingThroughWall(ref placement, transform, common, target))
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
						Construction.lastPlacementError = "Placing through wall";
					}
					else if (!this.TestPlacingCloseToRoad(ref placement, target))
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
						Construction.lastPlacementError = "Placing too close to road";
					}
					else if (Vector3.Distance(placement.position, target.player.eyes.position) > common.maxplaceDistance + 1f)
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
						Construction.lastPlacementError = "Too far away";
					}
					else
					{
						DeployVolume[] volumes = PrefabAttribute.server.FindAll<DeployVolume>(this.prefabID);
						if (DeployVolume.Check(placement.position, placement.rotation, volumes, -1))
						{
							transform.position = placement.position;
							transform.rotation = placement.rotation;
							Construction.lastPlacementError = "Not enough space";
						}
						else if (BuildingProximity.Check(target.player, this, placement.position, placement.rotation))
						{
							transform.position = placement.position;
							transform.rotation = placement.rotation;
						}
						else if (common.isBuildingPrivilege && !target.player.CanPlaceBuildingPrivilege(placement.position, placement.rotation, common.bounds))
						{
							transform.position = placement.position;
							transform.rotation = placement.rotation;
							Construction.lastPlacementError = "Cannot stack building privileges";
						}
						else
						{
							bool flag = target.player.IsBuildingBlocked(placement.position, placement.rotation, common.bounds);
							if (common.canBypassBuildingPermission || !flag)
							{
								target.inBuildingPrivilege = flag;
								transform.SetPositionAndRotation(placement.position, placement.rotation);
								Pool.FreeList<Socket_Base>(ref list);
								return true;
							}
							transform.position = placement.position;
							transform.rotation = placement.rotation;
							Construction.lastPlacementError = "You don't have permission to build here";
						}
					}
				}
			}
		}
		Pool.FreeList<Socket_Base>(ref list);
		return false;
	}

	// Token: 0x06001B03 RID: 6915 RVA: 0x000BD38C File Offset: 0x000BB58C
	private bool TestPlacingThroughRock(ref Construction.Placement placement, Construction.Target target)
	{
		OBB obb = new OBB(placement.position, Vector3.one, placement.rotation, this.bounds);
		Vector3 center = target.player.GetCenter(true);
		Vector3 origin = target.ray.origin;
		if (Physics.Linecast(center, origin, 65536, QueryTriggerInteraction.Ignore))
		{
			return false;
		}
		RaycastHit raycastHit;
		Vector3 end = obb.Trace(target.ray, out raycastHit, float.PositiveInfinity) ? raycastHit.point : obb.ClosestPoint(origin);
		return !Physics.Linecast(origin, end, 65536, QueryTriggerInteraction.Ignore);
	}

	// Token: 0x06001B04 RID: 6916 RVA: 0x000BD41C File Offset: 0x000BB61C
	private static bool TestPlacingThroughWall(ref Construction.Placement placement, Transform transform, Construction common, Construction.Target target)
	{
		Vector3 a = placement.position;
		if (common.deployOffset != null)
		{
			a += placement.rotation * common.deployOffset.localPosition;
		}
		Vector3 vector = a - target.ray.origin;
		RaycastHit hit;
		if (!Physics.Raycast(target.ray.origin, vector.normalized, out hit, vector.magnitude, 2097152))
		{
			return true;
		}
		StabilityEntity stabilityEntity = hit.GetEntity() as StabilityEntity;
		if (stabilityEntity != null && target.entity == stabilityEntity)
		{
			return true;
		}
		if (vector.magnitude - hit.distance < 0.2f)
		{
			return true;
		}
		Construction.lastPlacementError = "object in placement path";
		transform.SetPositionAndRotation(hit.point, placement.rotation);
		return false;
	}

	// Token: 0x06001B05 RID: 6917 RVA: 0x000BD4F8 File Offset: 0x000BB6F8
	private bool TestPlacingCloseToRoad(ref Construction.Placement placement, Construction.Target target)
	{
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		if (heightMap == null)
		{
			return true;
		}
		if (topologyMap == null)
		{
			return true;
		}
		OBB obb = new OBB(placement.position, Vector3.one, placement.rotation, this.bounds);
		float num = Mathf.Abs(heightMap.GetHeight(obb.position) - obb.position.y);
		if (num > 9f)
		{
			return true;
		}
		float radius = Mathf.Lerp(3f, 0f, num / 9f);
		Vector3 position = obb.position;
		Vector3 point = obb.GetPoint(-1f, 0f, -1f);
		Vector3 point2 = obb.GetPoint(-1f, 0f, 1f);
		Vector3 point3 = obb.GetPoint(1f, 0f, -1f);
		Vector3 point4 = obb.GetPoint(1f, 0f, 1f);
		int topology = topologyMap.GetTopology(position, radius);
		int topology2 = topologyMap.GetTopology(point, radius);
		int topology3 = topologyMap.GetTopology(point2, radius);
		int topology4 = topologyMap.GetTopology(point3, radius);
		int topology5 = topologyMap.GetTopology(point4, radius);
		return ((topology | topology2 | topology3 | topology4 | topology5) & 526336) == 0;
	}

	// Token: 0x06001B06 RID: 6918 RVA: 0x000BD644 File Offset: 0x000BB844
	public virtual bool ShowAsNeutral(Construction.Target target)
	{
		return target.inBuildingPrivilege;
	}

	// Token: 0x06001B07 RID: 6919 RVA: 0x000BD64C File Offset: 0x000BB84C
	public BaseEntity CreateConstruction(Construction.Target target, bool bNeedsValidPlacement = false)
	{
		GameObject gameObject = GameManager.server.CreatePrefab(this.fullName, Vector3.zero, Quaternion.identity, false);
		bool flag = this.UpdatePlacement(gameObject.transform, this, ref target);
		BaseEntity baseEntity = gameObject.ToBaseEntity();
		if (bNeedsValidPlacement && !flag)
		{
			if (baseEntity.IsValid())
			{
				baseEntity.Kill(BaseNetworkable.DestroyMode.None);
			}
			else
			{
				GameManager.Destroy(gameObject, 0f);
			}
			return null;
		}
		DecayEntity decayEntity = baseEntity as DecayEntity;
		if (decayEntity)
		{
			decayEntity.AttachToBuilding(target.entity as DecayEntity);
		}
		return baseEntity;
	}

	// Token: 0x06001B08 RID: 6920 RVA: 0x000BD6D4 File Offset: 0x000BB8D4
	public bool HasMaleSockets(Construction.Target target)
	{
		foreach (Socket_Base socket_Base in this.allSockets)
		{
			if (socket_Base.male && !socket_Base.maleDummy && socket_Base.TestTarget(target))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001B09 RID: 6921 RVA: 0x000BD718 File Offset: 0x000BB918
	public void FindMaleSockets(Construction.Target target, List<Socket_Base> sockets)
	{
		foreach (Socket_Base socket_Base in this.allSockets)
		{
			if (socket_Base.male && !socket_Base.maleDummy && socket_Base.TestTarget(target))
			{
				sockets.Add(socket_Base);
			}
		}
	}

	// Token: 0x06001B0A RID: 6922 RVA: 0x000BD760 File Offset: 0x000BB960
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.isBuildingPrivilege = rootObj.GetComponent<BuildingPrivlidge>();
		this.isSleepingBag = rootObj.GetComponent<SleepingBag>();
		this.bounds = rootObj.GetComponent<BaseEntity>().bounds;
		this.deployable = base.GetComponent<Deployable>();
		this.placeholder = base.GetComponentInChildren<ConstructionPlaceholder>();
		this.allSockets = base.GetComponentsInChildren<Socket_Base>(true);
		this.allProximities = base.GetComponentsInChildren<BuildingProximity>(true);
		this.socketHandle = base.GetComponentsInChildren<SocketHandle>(true).FirstOrDefault<SocketHandle>();
		ConstructionGrade[] components = rootObj.GetComponents<ConstructionGrade>();
		this.grades = new ConstructionGrade[5];
		foreach (ConstructionGrade constructionGrade in components)
		{
			constructionGrade.construction = this;
			this.grades[(int)constructionGrade.gradeBase.type] = constructionGrade;
		}
		for (int j = 0; j < this.grades.Length; j++)
		{
			if (!(this.grades[j] == null))
			{
				this.defaultGrade = this.grades[j];
				return;
			}
		}
	}

	// Token: 0x06001B0B RID: 6923 RVA: 0x000BD862 File Offset: 0x000BBA62
	protected override Type GetIndexedType()
	{
		return typeof(Construction);
	}

	// Token: 0x0400140A RID: 5130
	public static string lastPlacementError;

	// Token: 0x0400140B RID: 5131
	public BaseEntity.Menu.Option info;

	// Token: 0x0400140C RID: 5132
	public bool canBypassBuildingPermission;

	// Token: 0x0400140D RID: 5133
	[FormerlySerializedAs("canRotate")]
	public bool canRotateBeforePlacement;

	// Token: 0x0400140E RID: 5134
	[FormerlySerializedAs("canRotate")]
	public bool canRotateAfterPlacement;

	// Token: 0x0400140F RID: 5135
	public bool checkVolumeOnRotate;

	// Token: 0x04001410 RID: 5136
	public bool checkVolumeOnUpgrade;

	// Token: 0x04001411 RID: 5137
	public bool canPlaceAtMaxDistance;

	// Token: 0x04001412 RID: 5138
	public bool placeOnWater;

	// Token: 0x04001413 RID: 5139
	public Vector3 rotationAmount = new Vector3(0f, 90f, 0f);

	// Token: 0x04001414 RID: 5140
	public Vector3 applyStartingRotation = Vector3.zero;

	// Token: 0x04001415 RID: 5141
	public Transform deployOffset;

	// Token: 0x04001416 RID: 5142
	[Range(0f, 10f)]
	public float healthMultiplier = 1f;

	// Token: 0x04001417 RID: 5143
	[Range(0f, 10f)]
	public float costMultiplier = 1f;

	// Token: 0x04001418 RID: 5144
	[Range(1f, 50f)]
	public float maxplaceDistance = 4f;

	// Token: 0x04001419 RID: 5145
	public Mesh guideMesh;

	// Token: 0x0400141A RID: 5146
	[NonSerialized]
	public Socket_Base[] allSockets;

	// Token: 0x0400141B RID: 5147
	[NonSerialized]
	public BuildingProximity[] allProximities;

	// Token: 0x0400141C RID: 5148
	[NonSerialized]
	public ConstructionGrade defaultGrade;

	// Token: 0x0400141D RID: 5149
	[NonSerialized]
	public SocketHandle socketHandle;

	// Token: 0x0400141E RID: 5150
	[NonSerialized]
	public Bounds bounds;

	// Token: 0x0400141F RID: 5151
	[NonSerialized]
	public bool isBuildingPrivilege;

	// Token: 0x04001420 RID: 5152
	[NonSerialized]
	public bool isSleepingBag;

	// Token: 0x04001421 RID: 5153
	[NonSerialized]
	public ConstructionGrade[] grades;

	// Token: 0x04001422 RID: 5154
	[NonSerialized]
	public Deployable deployable;

	// Token: 0x04001423 RID: 5155
	[NonSerialized]
	public ConstructionPlaceholder placeholder;

	// Token: 0x02000C31 RID: 3121
	public struct Target
	{
		// Token: 0x06004C4E RID: 19534 RVA: 0x00195524 File Offset: 0x00193724
		public Quaternion GetWorldRotation(bool female)
		{
			Quaternion rhs = this.socket.rotation;
			if (this.socket.male && this.socket.female && female)
			{
				rhs = this.socket.rotation * Quaternion.Euler(180f, 0f, 180f);
			}
			return this.entity.transform.rotation * rhs;
		}

		// Token: 0x06004C4F RID: 19535 RVA: 0x00195598 File Offset: 0x00193798
		public Vector3 GetWorldPosition()
		{
			return this.entity.transform.localToWorldMatrix.MultiplyPoint3x4(this.socket.position);
		}

		// Token: 0x0400412A RID: 16682
		public bool valid;

		// Token: 0x0400412B RID: 16683
		public Ray ray;

		// Token: 0x0400412C RID: 16684
		public BaseEntity entity;

		// Token: 0x0400412D RID: 16685
		public Socket_Base socket;

		// Token: 0x0400412E RID: 16686
		public bool onTerrain;

		// Token: 0x0400412F RID: 16687
		public Vector3 position;

		// Token: 0x04004130 RID: 16688
		public Vector3 normal;

		// Token: 0x04004131 RID: 16689
		public Vector3 rotation;

		// Token: 0x04004132 RID: 16690
		public BasePlayer player;

		// Token: 0x04004133 RID: 16691
		public bool inBuildingPrivilege;
	}

	// Token: 0x02000C32 RID: 3122
	public class Placement
	{
		// Token: 0x04004134 RID: 16692
		public Vector3 position;

		// Token: 0x04004135 RID: 16693
		public Quaternion rotation;
	}

	// Token: 0x02000C33 RID: 3123
	public class Grade
	{
		// Token: 0x17000619 RID: 1561
		// (get) Token: 0x06004C51 RID: 19537 RVA: 0x001955C8 File Offset: 0x001937C8
		public PhysicMaterial physicMaterial
		{
			get
			{
				return this.grade.physicMaterial;
			}
		}

		// Token: 0x1700061A RID: 1562
		// (get) Token: 0x06004C52 RID: 19538 RVA: 0x001955D5 File Offset: 0x001937D5
		public ProtectionProperties damageProtecton
		{
			get
			{
				return this.grade.damageProtecton;
			}
		}

		// Token: 0x04004136 RID: 16694
		public BuildingGrade grade;

		// Token: 0x04004137 RID: 16695
		public float maxHealth;

		// Token: 0x04004138 RID: 16696
		public List<ItemAmount> costToBuild;
	}
}
