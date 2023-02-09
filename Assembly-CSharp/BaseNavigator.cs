using System;
using System.Collections.Generic;
using ConVar;
using Rust.AI;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020001EB RID: 491
public class BaseNavigator : BaseMonoBehaviour
{
	// Token: 0x170001EE RID: 494
	// (get) Token: 0x0600199F RID: 6559 RVA: 0x000B7C17 File Offset: 0x000B5E17
	// (set) Token: 0x060019A0 RID: 6560 RVA: 0x000B7C1F File Offset: 0x000B5E1F
	public AIMovePointPath Path { get; set; }

	// Token: 0x170001EF RID: 495
	// (get) Token: 0x060019A1 RID: 6561 RVA: 0x000B7C28 File Offset: 0x000B5E28
	// (set) Token: 0x060019A2 RID: 6562 RVA: 0x000B7C30 File Offset: 0x000B5E30
	public BasePath AStarGraph { get; set; }

	// Token: 0x060019A3 RID: 6563 RVA: 0x000B7C39 File Offset: 0x000B5E39
	public int TopologyPreference()
	{
		return (int)this.topologyPreference;
	}

	// Token: 0x060019A4 RID: 6564 RVA: 0x000B7C41 File Offset: 0x000B5E41
	public int TopologyPrevent()
	{
		return (int)this.topologyPrevent;
	}

	// Token: 0x060019A5 RID: 6565 RVA: 0x000B7C49 File Offset: 0x000B5E49
	public int BiomeRequirement()
	{
		return (int)this.biomeRequirement;
	}

	// Token: 0x170001F0 RID: 496
	// (get) Token: 0x060019A6 RID: 6566 RVA: 0x000B7C51 File Offset: 0x000B5E51
	// (set) Token: 0x060019A7 RID: 6567 RVA: 0x000B7C59 File Offset: 0x000B5E59
	public NavMeshAgent Agent { get; private set; }

	// Token: 0x170001F1 RID: 497
	// (get) Token: 0x060019A8 RID: 6568 RVA: 0x000B7C62 File Offset: 0x000B5E62
	// (set) Token: 0x060019A9 RID: 6569 RVA: 0x000B7C6A File Offset: 0x000B5E6A
	public BaseCombatEntity BaseEntity { get; private set; }

	// Token: 0x170001F2 RID: 498
	// (get) Token: 0x060019AA RID: 6570 RVA: 0x000B7C73 File Offset: 0x000B5E73
	// (set) Token: 0x060019AB RID: 6571 RVA: 0x000B7C7B File Offset: 0x000B5E7B
	public Vector3 Destination { get; protected set; }

	// Token: 0x170001F3 RID: 499
	// (get) Token: 0x060019AC RID: 6572 RVA: 0x000B7C84 File Offset: 0x000B5E84
	public virtual bool IsOnNavMeshLink
	{
		get
		{
			return this.Agent.enabled && this.Agent.isOnOffMeshLink;
		}
	}

	// Token: 0x170001F4 RID: 500
	// (get) Token: 0x060019AD RID: 6573 RVA: 0x000B7CA0 File Offset: 0x000B5EA0
	public bool Moving
	{
		get
		{
			return this.CurrentNavigationType > BaseNavigator.NavigationType.None;
		}
	}

	// Token: 0x170001F5 RID: 501
	// (get) Token: 0x060019AE RID: 6574 RVA: 0x000B7CAB File Offset: 0x000B5EAB
	// (set) Token: 0x060019AF RID: 6575 RVA: 0x000B7CB3 File Offset: 0x000B5EB3
	public BaseNavigator.NavigationType CurrentNavigationType { get; private set; }

	// Token: 0x170001F6 RID: 502
	// (get) Token: 0x060019B0 RID: 6576 RVA: 0x000B7CBC File Offset: 0x000B5EBC
	// (set) Token: 0x060019B1 RID: 6577 RVA: 0x000B7CC4 File Offset: 0x000B5EC4
	public BaseNavigator.NavigationType LastUsedNavigationType { get; private set; }

	// Token: 0x170001F7 RID: 503
	// (get) Token: 0x060019B2 RID: 6578 RVA: 0x000B7CCD File Offset: 0x000B5ECD
	// (set) Token: 0x060019B3 RID: 6579 RVA: 0x000B7CD5 File Offset: 0x000B5ED5
	[HideInInspector]
	public bool StuckOffNavmesh { get; private set; }

	// Token: 0x170001F8 RID: 504
	// (get) Token: 0x060019B4 RID: 6580 RVA: 0x000B7CDE File Offset: 0x000B5EDE
	public virtual bool HasPath
	{
		get
		{
			return !(this.Agent == null) && ((this.Agent.enabled && this.Agent.hasPath) || this.currentAStarPath != null);
		}
	}

	// Token: 0x060019B5 RID: 6581 RVA: 0x000B7D18 File Offset: 0x000B5F18
	public virtual void Init(BaseCombatEntity entity, NavMeshAgent agent)
	{
		this.defaultAreaMask = 1 << NavMesh.GetAreaFromName(this.DefaultArea);
		this.BaseEntity = entity;
		this.Agent = agent;
		if (this.Agent != null)
		{
			this.Agent.acceleration = this.Acceleration;
			this.Agent.angularSpeed = this.TurnSpeed;
		}
		this.navMeshQueryFilter = default(NavMeshQueryFilter);
		this.navMeshQueryFilter.agentTypeID = this.Agent.agentTypeID;
		this.navMeshQueryFilter.areaMask = this.defaultAreaMask;
		this.path = new NavMeshPath();
		this.SetCurrentNavigationType(BaseNavigator.NavigationType.None);
	}

	// Token: 0x060019B6 RID: 6582 RVA: 0x000B7DC0 File Offset: 0x000B5FC0
	public void SetNavMeshEnabled(bool flag)
	{
		if (this.Agent == null)
		{
			return;
		}
		if (this.Agent.enabled == flag)
		{
			return;
		}
		if (AiManager.nav_disable)
		{
			this.Agent.enabled = false;
			return;
		}
		if (this.Agent.enabled)
		{
			if (flag)
			{
				if (this.Agent.isOnNavMesh)
				{
					this.Agent.isStopped = false;
				}
			}
			else if (this.Agent.isOnNavMesh)
			{
				this.Agent.isStopped = true;
			}
		}
		this.Agent.enabled = flag;
		if (flag)
		{
			if (!this.CanEnableNavMeshNavigation())
			{
				return;
			}
			this.PlaceOnNavMesh();
		}
	}

	// Token: 0x060019B7 RID: 6583 RVA: 0x000B7E62 File Offset: 0x000B6062
	protected virtual bool CanEnableNavMeshNavigation()
	{
		return this.CanUseNavMesh;
	}

	// Token: 0x060019B8 RID: 6584 RVA: 0x000B7E6F File Offset: 0x000B606F
	protected virtual bool CanUpdateMovement()
	{
		return !(this.BaseEntity != null) || this.BaseEntity.IsAlive();
	}

	// Token: 0x060019B9 RID: 6585 RVA: 0x000B7E8F File Offset: 0x000B608F
	public void ForceToGround()
	{
		base.CancelInvoke(new Action(this.DelayedForceToGround));
		base.Invoke(new Action(this.DelayedForceToGround), 0.5f);
	}

	// Token: 0x060019BA RID: 6586 RVA: 0x000B7EBC File Offset: 0x000B60BC
	private void DelayedForceToGround()
	{
		int layerMask = 10551296;
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(base.transform.position + Vector3.up * 0.5f, Vector3.down, out raycastHit, 1000f, layerMask))
		{
			this.BaseEntity.ServerPosition = raycastHit.point;
		}
	}

	// Token: 0x060019BB RID: 6587 RVA: 0x000B7F14 File Offset: 0x000B6114
	public bool PlaceOnNavMesh()
	{
		if (this.Agent.isOnNavMesh)
		{
			return true;
		}
		float maxRange = this.IsSwimming() ? 30f : 6f;
		Vector3 position;
		bool result;
		if (this.GetNearestNavmeshPosition(base.transform.position + Vector3.one * 2f, out position, maxRange))
		{
			result = this.Warp(position);
		}
		else
		{
			result = false;
			this.StuckOffNavmesh = true;
			Debug.LogWarning(string.Concat(new object[]
			{
				base.gameObject.name,
				" failed to sample navmesh at position ",
				base.transform.position,
				" on area: ",
				this.DefaultArea
			}), base.gameObject);
		}
		return result;
	}

	// Token: 0x060019BC RID: 6588 RVA: 0x000B7FD8 File Offset: 0x000B61D8
	private bool Warp(Vector3 position)
	{
		this.Agent.Warp(position);
		this.Agent.enabled = true;
		base.transform.position = position;
		if (!this.Agent.isOnNavMesh)
		{
			Debug.LogWarning("Agent still not on navmesh after a warp. No navmesh areas matching agent type? Agent type: " + this.Agent.agentTypeID, base.gameObject);
			this.StuckOffNavmesh = true;
			return false;
		}
		this.StuckOffNavmesh = false;
		return true;
	}

	// Token: 0x060019BD RID: 6589 RVA: 0x000B8050 File Offset: 0x000B6250
	public bool GetNearestNavmeshPosition(Vector3 target, out Vector3 position, float maxRange)
	{
		position = base.transform.position;
		bool result = true;
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(target, out navMeshHit, maxRange, this.defaultAreaMask))
		{
			position = navMeshHit.position;
		}
		else
		{
			result = false;
		}
		return result;
	}

	// Token: 0x060019BE RID: 6590 RVA: 0x000B8093 File Offset: 0x000B6293
	public bool SetBaseDestination(Vector3 pos, float speedFraction)
	{
		if (!AI.move)
		{
			return false;
		}
		if (!AI.navthink)
		{
			return false;
		}
		this.paused = false;
		this.currentSpeedFraction = speedFraction;
		if (this.ReachedPosition(pos))
		{
			return true;
		}
		this.Destination = pos;
		this.SetCurrentNavigationType(BaseNavigator.NavigationType.Base);
		return true;
	}

	// Token: 0x060019BF RID: 6591 RVA: 0x000B80D0 File Offset: 0x000B62D0
	public bool SetDestination(BasePath path, BasePathNode newTargetNode, float speedFraction)
	{
		if (!AI.move)
		{
			return false;
		}
		if (!AI.navthink)
		{
			return false;
		}
		this.paused = false;
		if (!this.CanUseAStar)
		{
			return false;
		}
		if (newTargetNode == this.targetNode && this.HasPath)
		{
			return true;
		}
		if (this.ReachedPosition(newTargetNode.transform.position))
		{
			return true;
		}
		BasePathNode closestToPoint = path.GetClosestToPoint(base.transform.position);
		if (closestToPoint == null || closestToPoint.transform == null)
		{
			return false;
		}
		float num;
		if (AStarPath.FindPath(closestToPoint, newTargetNode, out this.currentAStarPath, out num))
		{
			this.currentSpeedFraction = speedFraction;
			this.targetNode = newTargetNode;
			this.SetCurrentNavigationType(BaseNavigator.NavigationType.AStar);
			this.Destination = newTargetNode.transform.position;
			return true;
		}
		return false;
	}

	// Token: 0x060019C0 RID: 6592 RVA: 0x000B8192 File Offset: 0x000B6392
	public bool SetDestination(Vector3 pos, BaseNavigator.NavigationSpeed speed, float updateInterval = 0f, float navmeshSampleDistance = 0f)
	{
		return this.SetDestination(pos, this.GetSpeedFraction(speed), updateInterval, navmeshSampleDistance);
	}

	// Token: 0x060019C1 RID: 6593 RVA: 0x000B81A5 File Offset: 0x000B63A5
	protected virtual bool SetCustomDestination(Vector3 pos, float speedFraction = 1f, float updateInterval = 0f)
	{
		if (!AI.move)
		{
			return false;
		}
		if (!AI.navthink)
		{
			return false;
		}
		if (!this.CanUseCustomNav)
		{
			return false;
		}
		this.paused = false;
		if (this.ReachedPosition(pos))
		{
			return true;
		}
		this.currentSpeedFraction = speedFraction;
		this.SetCurrentNavigationType(BaseNavigator.NavigationType.Custom);
		return true;
	}

	// Token: 0x060019C2 RID: 6594 RVA: 0x000B81E4 File Offset: 0x000B63E4
	public bool SetDestination(Vector3 pos, float speedFraction = 1f, float updateInterval = 0f, float navmeshSampleDistance = 0f)
	{
		if (!AI.move)
		{
			return false;
		}
		if (!AI.navthink)
		{
			return false;
		}
		if (updateInterval > 0f && !this.UpdateIntervalElapsed(updateInterval))
		{
			return true;
		}
		this.lastSetDestinationTime = UnityEngine.Time.time;
		this.paused = false;
		this.currentSpeedFraction = speedFraction;
		if (this.ReachedPosition(pos))
		{
			return true;
		}
		BaseNavigator.NavigationType navigationType = BaseNavigator.NavigationType.NavMesh;
		if (this.CanUseBaseNav && this.CanUseNavMesh)
		{
			Vector3 position;
			BaseNavigator.NavigationType navigationType2 = this.DetermineNavigationType(base.transform.position, out position);
			Vector3 vector;
			BaseNavigator.NavigationType navigationType3 = this.DetermineNavigationType(pos, out vector);
			if (navigationType3 == BaseNavigator.NavigationType.NavMesh && navigationType2 == BaseNavigator.NavigationType.NavMesh && (this.CurrentNavigationType == BaseNavigator.NavigationType.None || this.CurrentNavigationType == BaseNavigator.NavigationType.Base))
			{
				this.Warp(position);
			}
			if (navigationType3 == BaseNavigator.NavigationType.Base && navigationType2 != BaseNavigator.NavigationType.Base)
			{
				BasePet basePet = this.BaseEntity as BasePet;
				if (basePet != null)
				{
					BasePlayer basePlayer = basePet.Brain.Events.Memory.Entity.Get(5) as BasePlayer;
					if (basePlayer != null)
					{
						BuildingPrivlidge buildingPrivilege = basePlayer.GetBuildingPrivilege(new OBB(pos, base.transform.rotation, this.BaseEntity.bounds));
						if (buildingPrivilege != null && !buildingPrivilege.IsAuthed(basePlayer) && buildingPrivilege.AnyAuthed())
						{
							return false;
						}
					}
				}
			}
			if (navigationType3 == BaseNavigator.NavigationType.Base)
			{
				if (navigationType2 != BaseNavigator.NavigationType.Base)
				{
					if (Vector3.Distance(this.BaseEntity.ServerPosition, pos) <= 10f && Mathf.Abs(this.BaseEntity.ServerPosition.y - pos.y) <= 3f)
					{
						navigationType = BaseNavigator.NavigationType.Base;
					}
					else
					{
						navigationType = BaseNavigator.NavigationType.NavMesh;
					}
				}
				else
				{
					navigationType = BaseNavigator.NavigationType.Base;
				}
			}
			else if (navigationType3 == BaseNavigator.NavigationType.NavMesh)
			{
				if (navigationType2 != BaseNavigator.NavigationType.NavMesh)
				{
					navigationType = BaseNavigator.NavigationType.Base;
				}
				else
				{
					navigationType = BaseNavigator.NavigationType.NavMesh;
				}
			}
		}
		else
		{
			navigationType = (this.CanUseNavMesh ? BaseNavigator.NavigationType.NavMesh : BaseNavigator.NavigationType.AStar);
		}
		if (navigationType == BaseNavigator.NavigationType.Base)
		{
			return this.SetBaseDestination(pos, speedFraction);
		}
		if (navigationType == BaseNavigator.NavigationType.AStar)
		{
			if (this.AStarGraph != null)
			{
				return this.SetDestination(this.AStarGraph, this.AStarGraph.GetClosestToPoint(pos), speedFraction);
			}
			return this.CanUseCustomNav && this.SetCustomDestination(pos, speedFraction, updateInterval);
		}
		else
		{
			if (AiManager.nav_disable)
			{
				return false;
			}
			if (navmeshSampleDistance > 0f && AI.setdestinationsamplenavmesh)
			{
				NavMeshHit navMeshHit;
				if (!NavMesh.SamplePosition(pos, out navMeshHit, navmeshSampleDistance, this.defaultAreaMask))
				{
					return false;
				}
				pos = navMeshHit.position;
			}
			this.SetCurrentNavigationType(BaseNavigator.NavigationType.NavMesh);
			if (!this.Agent.isOnNavMesh)
			{
				return false;
			}
			if (!this.Agent.isActiveAndEnabled)
			{
				return false;
			}
			this.Destination = pos;
			bool flag;
			if (AI.usecalculatepath)
			{
				flag = NavMesh.CalculatePath(base.transform.position, this.Destination, this.navMeshQueryFilter, this.path);
				if (flag)
				{
					this.Agent.SetPath(this.path);
				}
				else if (AI.usesetdestinationfallback)
				{
					flag = this.Agent.SetDestination(this.Destination);
				}
			}
			else
			{
				flag = this.Agent.SetDestination(this.Destination);
			}
			if (flag && this.SpeedBasedAvoidancePriority)
			{
				this.Agent.avoidancePriority = UnityEngine.Random.Range(0, 21) + Mathf.FloorToInt(speedFraction * 80f);
			}
			return flag;
		}
	}

	// Token: 0x060019C3 RID: 6595 RVA: 0x000B84E0 File Offset: 0x000B66E0
	private BaseNavigator.NavigationType DetermineNavigationType(Vector3 location, out Vector3 navMeshPos)
	{
		navMeshPos = location;
		int layerMask = 2097152;
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(location + Vector3.up * BaseNavigator.navTypeHeightOffset, Vector3.down, out raycastHit, BaseNavigator.navTypeDistance, layerMask))
		{
			return BaseNavigator.NavigationType.Base;
		}
		Vector3 vector;
		BaseNavigator.NavigationType result = this.GetNearestNavmeshPosition(location + Vector3.up * BaseNavigator.navTypeHeightOffset, out vector, BaseNavigator.navTypeDistance) ? BaseNavigator.NavigationType.NavMesh : BaseNavigator.NavigationType.Base;
		navMeshPos = vector;
		return result;
	}

	// Token: 0x060019C4 RID: 6596 RVA: 0x000B8554 File Offset: 0x000B6754
	public void SetCurrentSpeed(BaseNavigator.NavigationSpeed speed)
	{
		this.currentSpeedFraction = this.GetSpeedFraction(speed);
	}

	// Token: 0x060019C5 RID: 6597 RVA: 0x000B8563 File Offset: 0x000B6763
	public bool UpdateIntervalElapsed(float updateInterval)
	{
		return updateInterval <= 0f || UnityEngine.Time.time - this.lastSetDestinationTime >= updateInterval;
	}

	// Token: 0x060019C6 RID: 6598 RVA: 0x000B8581 File Offset: 0x000B6781
	public float GetSpeedFraction(BaseNavigator.NavigationSpeed speed)
	{
		switch (speed)
		{
		case BaseNavigator.NavigationSpeed.Slowest:
			return this.SlowestSpeedFraction;
		case BaseNavigator.NavigationSpeed.Slow:
			return this.SlowSpeedFraction;
		case BaseNavigator.NavigationSpeed.Normal:
			return this.NormalSpeedFraction;
		case BaseNavigator.NavigationSpeed.Fast:
			return this.FastSpeedFraction;
		default:
			return 1f;
		}
	}

	// Token: 0x060019C7 RID: 6599 RVA: 0x000B85BC File Offset: 0x000B67BC
	protected void SetCurrentNavigationType(BaseNavigator.NavigationType navType)
	{
		if (this.CurrentNavigationType == BaseNavigator.NavigationType.None)
		{
			this.stuckCheckPosition = base.transform.position;
			this.stuckTimer = 0f;
		}
		this.CurrentNavigationType = navType;
		if (this.CurrentNavigationType != BaseNavigator.NavigationType.None)
		{
			this.LastUsedNavigationType = this.CurrentNavigationType;
		}
		if (navType == BaseNavigator.NavigationType.None)
		{
			this.stuckTimer = 0f;
			return;
		}
		if (navType != BaseNavigator.NavigationType.NavMesh)
		{
			return;
		}
		this.SetNavMeshEnabled(true);
	}

	// Token: 0x060019C8 RID: 6600 RVA: 0x000B8623 File Offset: 0x000B6823
	public void Pause()
	{
		if (this.CurrentNavigationType == BaseNavigator.NavigationType.None)
		{
			return;
		}
		this.Stop();
		this.paused = true;
	}

	// Token: 0x060019C9 RID: 6601 RVA: 0x000B863B File Offset: 0x000B683B
	public void Resume()
	{
		if (!this.paused)
		{
			return;
		}
		this.SetDestination(this.Destination, this.currentSpeedFraction, 0f, 0f);
		this.paused = false;
	}

	// Token: 0x060019CA RID: 6602 RVA: 0x000B866C File Offset: 0x000B686C
	public void Stop()
	{
		switch (this.CurrentNavigationType)
		{
		case BaseNavigator.NavigationType.NavMesh:
			this.StopNavMesh();
			break;
		case BaseNavigator.NavigationType.AStar:
			this.StopAStar();
			break;
		case BaseNavigator.NavigationType.Custom:
			this.StopCustom();
			break;
		}
		this.SetCurrentNavigationType(BaseNavigator.NavigationType.None);
		this.paused = false;
	}

	// Token: 0x060019CB RID: 6603 RVA: 0x000B86BA File Offset: 0x000B68BA
	private void StopNavMesh()
	{
		this.SetNavMeshEnabled(false);
	}

	// Token: 0x060019CC RID: 6604 RVA: 0x000B86C3 File Offset: 0x000B68C3
	private void StopAStar()
	{
		this.currentAStarPath = null;
		this.targetNode = null;
	}

	// Token: 0x060019CD RID: 6605 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void StopCustom()
	{
	}

	// Token: 0x060019CE RID: 6606 RVA: 0x000B86D3 File Offset: 0x000B68D3
	public void Think(float delta)
	{
		if (!AI.move)
		{
			return;
		}
		if (!AI.navthink)
		{
			return;
		}
		if (this.BaseEntity == null)
		{
			return;
		}
		this.UpdateNavigation(delta);
	}

	// Token: 0x060019CF RID: 6607 RVA: 0x000B86FB File Offset: 0x000B68FB
	public void UpdateNavigation(float delta)
	{
		this.UpdateMovement(delta);
	}

	// Token: 0x060019D0 RID: 6608 RVA: 0x000B8704 File Offset: 0x000B6904
	private void UpdateMovement(float delta)
	{
		if (!AI.move)
		{
			return;
		}
		if (!this.CanUpdateMovement())
		{
			return;
		}
		Vector3 moveToPosition = base.transform.position;
		if (this.TriggerStuckEvent)
		{
			this.stuckTimer += delta;
			if (this.CurrentNavigationType != BaseNavigator.NavigationType.None && this.stuckTimer >= BaseNavigator.stuckTriggerDuration)
			{
				if (Vector3.Distance(base.transform.position, this.stuckCheckPosition) <= this.StuckDistance)
				{
					this.OnStuck();
				}
				this.stuckTimer = 0f;
				this.stuckCheckPosition = base.transform.position;
			}
		}
		if (this.CurrentNavigationType == BaseNavigator.NavigationType.Base)
		{
			moveToPosition = this.Destination;
		}
		else if (this.IsOnNavMeshLink)
		{
			this.HandleNavMeshLinkTraversal(delta, ref moveToPosition);
		}
		else if (this.HasPath)
		{
			moveToPosition = this.GetNextPathPosition();
		}
		else if (this.CurrentNavigationType == BaseNavigator.NavigationType.Custom)
		{
			moveToPosition = this.Destination;
		}
		if (!this.ValidateNextPosition(ref moveToPosition))
		{
			return;
		}
		bool swimming = this.IsSwimming();
		this.UpdateSpeed(delta, swimming);
		this.UpdatePositionAndRotation(moveToPosition, delta);
	}

	// Token: 0x060019D1 RID: 6609 RVA: 0x000B8804 File Offset: 0x000B6A04
	public virtual void OnStuck()
	{
		BasePet basePet = this.BaseEntity as BasePet;
		if (basePet != null && basePet.Brain != null)
		{
			basePet.Brain.LoadDefaultAIDesign();
		}
	}

	// Token: 0x060019D2 RID: 6610 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool IsSwimming()
	{
		return false;
	}

	// Token: 0x060019D3 RID: 6611 RVA: 0x000B8840 File Offset: 0x000B6A40
	private Vector3 GetNextPathPosition()
	{
		if (this.currentAStarPath != null && this.currentAStarPath.Count > 0)
		{
			return this.currentAStarPath.Peek().transform.position;
		}
		return this.Agent.nextPosition;
	}

	// Token: 0x060019D4 RID: 6612 RVA: 0x000B887C File Offset: 0x000B6A7C
	private bool ValidateNextPosition(ref Vector3 moveToPosition)
	{
		bool flag = ValidBounds.Test(moveToPosition);
		if (this.BaseEntity != null && !flag && base.transform != null && !this.BaseEntity.IsDestroyed)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Invalid NavAgent Position: ",
				this,
				" ",
				moveToPosition.ToString(),
				" (destroying)"
			}));
			this.BaseEntity.Kill(BaseNetworkable.DestroyMode.None);
			return false;
		}
		return true;
	}

	// Token: 0x060019D5 RID: 6613 RVA: 0x000B890C File Offset: 0x000B6B0C
	private void UpdateSpeed(float delta, bool swimming)
	{
		float num = this.GetTargetSpeed();
		if (this.LowHealthSpeedReductionTriggerFraction > 0f && this.BaseEntity.healthFraction <= this.LowHealthSpeedReductionTriggerFraction)
		{
			num = Mathf.Min(num, this.Speed * this.LowHealthMaxSpeedFraction);
		}
		this.Agent.speed = num * (swimming ? this.SwimmingSpeedMultiplier : 1f);
	}

	// Token: 0x060019D6 RID: 6614 RVA: 0x000B8971 File Offset: 0x000B6B71
	protected virtual float GetTargetSpeed()
	{
		return this.Speed * this.currentSpeedFraction;
	}

	// Token: 0x060019D7 RID: 6615 RVA: 0x000B8980 File Offset: 0x000B6B80
	protected virtual void UpdatePositionAndRotation(Vector3 moveToPosition, float delta)
	{
		if (this.CurrentNavigationType == BaseNavigator.NavigationType.AStar && this.currentAStarPath != null && this.currentAStarPath.Count > 0)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, moveToPosition, this.Agent.speed * delta);
			this.BaseEntity.ServerPosition = base.transform.localPosition;
			if (this.ReachedPosition(moveToPosition))
			{
				this.currentAStarPath.Pop();
				if (this.currentAStarPath.Count == 0)
				{
					this.Stop();
					return;
				}
				moveToPosition = this.currentAStarPath.Peek().transform.position;
			}
		}
		if (this.CurrentNavigationType == BaseNavigator.NavigationType.NavMesh)
		{
			if (this.ReachedPosition(this.Agent.destination))
			{
				this.Stop();
			}
			if (this.BaseEntity != null)
			{
				this.BaseEntity.ServerPosition = moveToPosition;
			}
		}
		if (this.CurrentNavigationType == BaseNavigator.NavigationType.Base)
		{
			this.frameCount++;
			this.accumDelta += delta;
			if (this.frameCount < BaseNavigator.baseNavMovementFrameInterval)
			{
				return;
			}
			this.frameCount = 0;
			delta = this.accumDelta;
			this.accumDelta = 0f;
			int layerMask = 10551552;
			Vector3 a = Vector3Ex.Direction2D(this.Destination, this.BaseEntity.ServerPosition);
			Vector3 a2 = this.BaseEntity.ServerPosition + a * delta * this.Agent.speed;
			Vector3 vector = this.BaseEntity.ServerPosition + Vector3.up * BaseNavigator.maxStepUpDistance;
			Vector3 direction = Vector3Ex.Direction(a2 + Vector3.up * BaseNavigator.maxStepUpDistance, this.BaseEntity.ServerPosition + Vector3.up * BaseNavigator.maxStepUpDistance);
			float maxDistance = Vector3.Distance(vector, a2 + Vector3.up * BaseNavigator.maxStepUpDistance) + 0.25f;
			RaycastHit raycastHit;
			if (UnityEngine.Physics.Raycast(vector, direction, out raycastHit, maxDistance, layerMask))
			{
				return;
			}
			if (!UnityEngine.Physics.SphereCast(a2 + Vector3.up * (BaseNavigator.maxStepUpDistance + 0.3f), 0.25f, Vector3.down, out raycastHit, 10f, layerMask))
			{
				return;
			}
			Vector3 point = raycastHit.point;
			if (point.y - this.BaseEntity.ServerPosition.y > BaseNavigator.maxStepUpDistance)
			{
				return;
			}
			this.BaseEntity.ServerPosition = point;
			if (this.ReachedPosition(moveToPosition))
			{
				this.Stop();
			}
		}
		if (this.overrideFacingDirectionMode != BaseNavigator.OverrideFacingDirectionMode.None)
		{
			this.ApplyFacingDirectionOverride();
		}
	}

	// Token: 0x060019D8 RID: 6616 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void ApplyFacingDirectionOverride()
	{
	}

	// Token: 0x060019D9 RID: 6617 RVA: 0x000B8C19 File Offset: 0x000B6E19
	public void SetFacingDirectionEntity(BaseEntity entity)
	{
		this.overrideFacingDirectionMode = BaseNavigator.OverrideFacingDirectionMode.Entity;
		this.facingDirectionEntity = entity;
	}

	// Token: 0x060019DA RID: 6618 RVA: 0x000B8C29 File Offset: 0x000B6E29
	public void SetFacingDirectionOverride(Vector3 direction)
	{
		this.overrideFacingDirectionMode = BaseNavigator.OverrideFacingDirectionMode.Direction;
		this.overrideFacingDirection = true;
		this.facingDirectionOverride = direction;
	}

	// Token: 0x060019DB RID: 6619 RVA: 0x000B8C40 File Offset: 0x000B6E40
	public void ClearFacingDirectionOverride()
	{
		this.overrideFacingDirectionMode = BaseNavigator.OverrideFacingDirectionMode.None;
		this.overrideFacingDirection = false;
		this.facingDirectionEntity = null;
	}

	// Token: 0x170001F9 RID: 505
	// (get) Token: 0x060019DC RID: 6620 RVA: 0x000B8C57 File Offset: 0x000B6E57
	public bool IsOverridingFacingDirection
	{
		get
		{
			return this.overrideFacingDirectionMode > BaseNavigator.OverrideFacingDirectionMode.None;
		}
	}

	// Token: 0x170001FA RID: 506
	// (get) Token: 0x060019DD RID: 6621 RVA: 0x000B8C62 File Offset: 0x000B6E62
	public Vector3 FacingDirectionOverride
	{
		get
		{
			return this.facingDirectionOverride;
		}
	}

	// Token: 0x060019DE RID: 6622 RVA: 0x000B8C6A File Offset: 0x000B6E6A
	protected bool ReachedPosition(Vector3 position)
	{
		return Vector3.Distance(position, base.transform.position) <= this.StoppingDistance;
	}

	// Token: 0x060019DF RID: 6623 RVA: 0x000B8C88 File Offset: 0x000B6E88
	private void HandleNavMeshLinkTraversal(float delta, ref Vector3 moveToPosition)
	{
		if (!this.traversingNavMeshLink)
		{
			this.HandleNavMeshLinkTraversalStart(delta);
		}
		this.HandleNavMeshLinkTraversalTick(delta, ref moveToPosition);
		if (this.IsNavMeshLinkTraversalComplete(delta, ref moveToPosition))
		{
			this.CompleteNavMeshLink();
		}
	}

	// Token: 0x060019E0 RID: 6624 RVA: 0x000B8CB4 File Offset: 0x000B6EB4
	private bool HandleNavMeshLinkTraversalStart(float delta)
	{
		OffMeshLinkData currentOffMeshLinkData = this.Agent.currentOffMeshLinkData;
		if (!currentOffMeshLinkData.valid || !currentOffMeshLinkData.activated)
		{
			return false;
		}
		Vector3 normalized = (currentOffMeshLinkData.endPos - currentOffMeshLinkData.startPos).normalized;
		normalized.y = 0f;
		Vector3 desiredVelocity = this.Agent.desiredVelocity;
		desiredVelocity.y = 0f;
		if (Vector3.Dot(desiredVelocity, normalized) < 0.1f)
		{
			this.CompleteNavMeshLink();
			return false;
		}
		this.currentNavMeshLinkName = currentOffMeshLinkData.linkType.ToString();
		Vector3 a = (this.BaseEntity != null) ? this.BaseEntity.ServerPosition : base.transform.position;
		if ((a - currentOffMeshLinkData.startPos).sqrMagnitude > (a - currentOffMeshLinkData.endPos).sqrMagnitude)
		{
			this.currentNavMeshLinkEndPos = currentOffMeshLinkData.startPos;
		}
		else
		{
			this.currentNavMeshLinkEndPos = currentOffMeshLinkData.endPos;
		}
		this.traversingNavMeshLink = true;
		this.Agent.ActivateCurrentOffMeshLink(false);
		this.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
		if (!(this.currentNavMeshLinkName == "OpenDoorLink") && !(this.currentNavMeshLinkName == "JumpRockLink"))
		{
			this.currentNavMeshLinkName == "JumpFoundationLink";
		}
		return true;
	}

	// Token: 0x060019E1 RID: 6625 RVA: 0x000B8E1C File Offset: 0x000B701C
	private void HandleNavMeshLinkTraversalTick(float delta, ref Vector3 moveToPosition)
	{
		if (this.currentNavMeshLinkName == "OpenDoorLink")
		{
			moveToPosition = Vector3.MoveTowards(moveToPosition, this.currentNavMeshLinkEndPos, this.Agent.speed * delta);
			return;
		}
		if (this.currentNavMeshLinkName == "JumpRockLink")
		{
			moveToPosition = Vector3.MoveTowards(moveToPosition, this.currentNavMeshLinkEndPos, this.Agent.speed * delta);
			return;
		}
		if (this.currentNavMeshLinkName == "JumpFoundationLink")
		{
			moveToPosition = Vector3.MoveTowards(moveToPosition, this.currentNavMeshLinkEndPos, this.Agent.speed * delta);
			return;
		}
		moveToPosition = Vector3.MoveTowards(moveToPosition, this.currentNavMeshLinkEndPos, this.Agent.speed * delta);
	}

	// Token: 0x060019E2 RID: 6626 RVA: 0x000B8EF4 File Offset: 0x000B70F4
	private bool IsNavMeshLinkTraversalComplete(float delta, ref Vector3 moveToPosition)
	{
		if ((moveToPosition - this.currentNavMeshLinkEndPos).sqrMagnitude < 0.01f)
		{
			moveToPosition = this.currentNavMeshLinkEndPos;
			this.traversingNavMeshLink = false;
			this.currentNavMeshLinkName = string.Empty;
			this.CompleteNavMeshLink();
			return true;
		}
		return false;
	}

	// Token: 0x060019E3 RID: 6627 RVA: 0x000B8F48 File Offset: 0x000B7148
	private void CompleteNavMeshLink()
	{
		this.Agent.ActivateCurrentOffMeshLink(true);
		this.Agent.CompleteOffMeshLink();
		this.Agent.isStopped = false;
		this.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
	}

	// Token: 0x060019E4 RID: 6628 RVA: 0x000B8F7C File Offset: 0x000B717C
	public bool IsPositionATopologyPreference(Vector3 position)
	{
		if (TerrainMeta.TopologyMap != null)
		{
			int topology = TerrainMeta.TopologyMap.GetTopology(position);
			if ((this.TopologyPreference() & topology) != 0)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060019E5 RID: 6629 RVA: 0x000B8FB0 File Offset: 0x000B71B0
	public bool IsPositionPreventTopology(Vector3 position)
	{
		if (TerrainMeta.TopologyMap != null)
		{
			int topology = TerrainMeta.TopologyMap.GetTopology(position);
			if ((this.TopologyPrevent() & topology) != 0)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060019E6 RID: 6630 RVA: 0x000B8FE4 File Offset: 0x000B71E4
	public bool IsPositionABiomePreference(Vector3 position)
	{
		if (!this.UseBiomePreference)
		{
			return true;
		}
		if (TerrainMeta.BiomeMap != null)
		{
			int num = (int)this.biomePreference;
			if ((TerrainMeta.BiomeMap.GetBiomeMaxType(position, -1) & num) != 0)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060019E7 RID: 6631 RVA: 0x000B9024 File Offset: 0x000B7224
	public bool IsPositionABiomeRequirement(Vector3 position)
	{
		if (this.biomeRequirement == (TerrainBiome.Enum)0)
		{
			return true;
		}
		if (TerrainMeta.BiomeMap != null)
		{
			int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(position, -1);
			if ((this.BiomeRequirement() & biomeMaxType) != 0)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060019E8 RID: 6632 RVA: 0x000B9062 File Offset: 0x000B7262
	public bool IsAcceptableWaterDepth(Vector3 pos)
	{
		return WaterLevel.GetOverallWaterDepth(pos, true, null, false) <= this.MaxWaterDepth;
	}

	// Token: 0x060019E9 RID: 6633 RVA: 0x000B9078 File Offset: 0x000B7278
	public void SetBrakingEnabled(bool flag)
	{
		this.Agent.autoBraking = flag;
	}

	// Token: 0x04001207 RID: 4615
	[ServerVar(Help = "The max step-up height difference for pet base navigation")]
	public static float maxStepUpDistance = 1.7f;

	// Token: 0x04001208 RID: 4616
	[ServerVar(Help = "How many frames between base navigation movement updates")]
	public static int baseNavMovementFrameInterval = 2;

	// Token: 0x04001209 RID: 4617
	[ServerVar(Help = "How long we are not moving for before trigger the stuck event")]
	public static float stuckTriggerDuration = 10f;

	// Token: 0x0400120A RID: 4618
	[ServerVar]
	public static float navTypeHeightOffset = 0.5f;

	// Token: 0x0400120B RID: 4619
	[ServerVar]
	public static float navTypeDistance = 1f;

	// Token: 0x0400120C RID: 4620
	[Header("General")]
	public bool CanNavigateMounted;

	// Token: 0x0400120D RID: 4621
	public bool CanUseNavMesh = true;

	// Token: 0x0400120E RID: 4622
	public bool CanUseAStar = true;

	// Token: 0x0400120F RID: 4623
	public bool CanUseBaseNav;

	// Token: 0x04001210 RID: 4624
	public bool CanUseCustomNav;

	// Token: 0x04001211 RID: 4625
	public float StoppingDistance = 0.5f;

	// Token: 0x04001212 RID: 4626
	public string DefaultArea = "Walkable";

	// Token: 0x04001213 RID: 4627
	[Header("Stuck Detection")]
	public bool TriggerStuckEvent;

	// Token: 0x04001214 RID: 4628
	public float StuckDistance = 1f;

	// Token: 0x04001215 RID: 4629
	[Header("Speed")]
	public float Speed = 5f;

	// Token: 0x04001216 RID: 4630
	public float Acceleration = 5f;

	// Token: 0x04001217 RID: 4631
	public float TurnSpeed = 10f;

	// Token: 0x04001218 RID: 4632
	public BaseNavigator.NavigationSpeed MoveTowardsSpeed = BaseNavigator.NavigationSpeed.Normal;

	// Token: 0x04001219 RID: 4633
	public bool FaceMoveTowardsTarget;

	// Token: 0x0400121A RID: 4634
	[Header("Speed Fractions")]
	public float SlowestSpeedFraction = 0.16f;

	// Token: 0x0400121B RID: 4635
	public float SlowSpeedFraction = 0.3f;

	// Token: 0x0400121C RID: 4636
	public float NormalSpeedFraction = 0.5f;

	// Token: 0x0400121D RID: 4637
	public float FastSpeedFraction = 1f;

	// Token: 0x0400121E RID: 4638
	public float LowHealthSpeedReductionTriggerFraction;

	// Token: 0x0400121F RID: 4639
	public float LowHealthMaxSpeedFraction = 0.5f;

	// Token: 0x04001220 RID: 4640
	public float SwimmingSpeedMultiplier = 0.25f;

	// Token: 0x04001221 RID: 4641
	[Header("AIPoint Usage")]
	public float BestMovementPointMaxDistance = 10f;

	// Token: 0x04001222 RID: 4642
	public float BestCoverPointMaxDistance = 20f;

	// Token: 0x04001223 RID: 4643
	public float BestRoamPointMaxDistance = 20f;

	// Token: 0x04001224 RID: 4644
	public float MaxRoamDistanceFromHome = -1f;

	// Token: 0x04001225 RID: 4645
	[Header("Misc")]
	public float MaxWaterDepth = 0.75f;

	// Token: 0x04001226 RID: 4646
	public bool SpeedBasedAvoidancePriority;

	// Token: 0x04001227 RID: 4647
	private NavMeshPath path;

	// Token: 0x04001228 RID: 4648
	private NavMeshQueryFilter navMeshQueryFilter;

	// Token: 0x0400122B RID: 4651
	private int defaultAreaMask;

	// Token: 0x0400122C RID: 4652
	[InspectorFlags]
	public TerrainBiome.Enum biomePreference = (TerrainBiome.Enum)12;

	// Token: 0x0400122D RID: 4653
	public bool UseBiomePreference;

	// Token: 0x0400122E RID: 4654
	[InspectorFlags]
	public TerrainTopology.Enum topologyPreference = (TerrainTopology.Enum)96;

	// Token: 0x0400122F RID: 4655
	[InspectorFlags]
	public TerrainTopology.Enum topologyPrevent;

	// Token: 0x04001230 RID: 4656
	[InspectorFlags]
	public TerrainBiome.Enum biomeRequirement;

	// Token: 0x04001236 RID: 4662
	private float stuckTimer;

	// Token: 0x04001237 RID: 4663
	private Vector3 stuckCheckPosition;

	// Token: 0x04001239 RID: 4665
	protected bool traversingNavMeshLink;

	// Token: 0x0400123A RID: 4666
	protected string currentNavMeshLinkName;

	// Token: 0x0400123B RID: 4667
	protected Vector3 currentNavMeshLinkEndPos;

	// Token: 0x0400123C RID: 4668
	protected Stack<BasePathNode> currentAStarPath;

	// Token: 0x0400123D RID: 4669
	protected BasePathNode targetNode;

	// Token: 0x0400123E RID: 4670
	protected float currentSpeedFraction = 1f;

	// Token: 0x0400123F RID: 4671
	private float lastSetDestinationTime;

	// Token: 0x04001240 RID: 4672
	protected BaseNavigator.OverrideFacingDirectionMode overrideFacingDirectionMode;

	// Token: 0x04001241 RID: 4673
	protected BaseEntity facingDirectionEntity;

	// Token: 0x04001242 RID: 4674
	protected bool overrideFacingDirection;

	// Token: 0x04001243 RID: 4675
	protected Vector3 facingDirectionOverride;

	// Token: 0x04001244 RID: 4676
	protected bool paused;

	// Token: 0x04001245 RID: 4677
	private int frameCount;

	// Token: 0x04001246 RID: 4678
	private float accumDelta;

	// Token: 0x02000C10 RID: 3088
	public enum NavigationType
	{
		// Token: 0x04004076 RID: 16502
		None,
		// Token: 0x04004077 RID: 16503
		NavMesh,
		// Token: 0x04004078 RID: 16504
		AStar,
		// Token: 0x04004079 RID: 16505
		Custom,
		// Token: 0x0400407A RID: 16506
		Base
	}

	// Token: 0x02000C11 RID: 3089
	public enum NavigationSpeed
	{
		// Token: 0x0400407C RID: 16508
		Slowest,
		// Token: 0x0400407D RID: 16509
		Slow,
		// Token: 0x0400407E RID: 16510
		Normal,
		// Token: 0x0400407F RID: 16511
		Fast
	}

	// Token: 0x02000C12 RID: 3090
	protected enum OverrideFacingDirectionMode
	{
		// Token: 0x04004081 RID: 16513
		None,
		// Token: 0x04004082 RID: 16514
		Direction,
		// Token: 0x04004083 RID: 16515
		Entity
	}
}
