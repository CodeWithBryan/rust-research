using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020001C7 RID: 455
public class AIInformationZone : BaseMonoBehaviour, IServerComponent
{
	// Token: 0x0600181E RID: 6174 RVA: 0x000B2540 File Offset: 0x000B0740
	public static AIInformationZone Merge(List<AIInformationZone> zones, GameObject newRoot)
	{
		if (zones == null)
		{
			return null;
		}
		AIInformationZone aiinformationZone = newRoot.AddComponent<AIInformationZone>();
		aiinformationZone.UseCalculatedCoverDistances = false;
		foreach (AIInformationZone aiinformationZone2 in zones)
		{
			if (!(aiinformationZone2 == null))
			{
				foreach (AIMovePoint aimovePoint in aiinformationZone2.movePoints)
				{
					aiinformationZone.AddMovePoint(aimovePoint);
					aimovePoint.transform.SetParent(newRoot.transform);
				}
				foreach (AICoverPoint aicoverPoint in aiinformationZone2.coverPoints)
				{
					aiinformationZone.AddCoverPoint(aicoverPoint);
					aicoverPoint.transform.SetParent(newRoot.transform);
				}
			}
		}
		aiinformationZone.bounds = AIInformationZone.EncapsulateBounds(zones);
		AIInformationZone aiinformationZone3 = aiinformationZone;
		aiinformationZone3.bounds.extents = aiinformationZone3.bounds.extents + new Vector3(5f, 0f, 5f);
		AIInformationZone aiinformationZone4 = aiinformationZone;
		aiinformationZone4.bounds.center = aiinformationZone4.bounds.center - aiinformationZone.transform.position;
		for (int i = zones.Count - 1; i >= 0; i--)
		{
			AIInformationZone aiinformationZone5 = zones[i];
			if (!(aiinformationZone5 == null))
			{
				UnityEngine.Object.Destroy(aiinformationZone5);
			}
		}
		return aiinformationZone;
	}

	// Token: 0x0600181F RID: 6175 RVA: 0x000B26E4 File Offset: 0x000B08E4
	public static Bounds EncapsulateBounds(List<AIInformationZone> zones)
	{
		Bounds result = default(Bounds);
		result.center = zones[0].transform.position;
		foreach (AIInformationZone aiinformationZone in zones)
		{
			if (!(aiinformationZone == null))
			{
				Vector3 center = aiinformationZone.bounds.center + aiinformationZone.transform.position;
				Bounds bounds = aiinformationZone.bounds;
				bounds.center = center;
				result.Encapsulate(bounds);
			}
		}
		return result;
	}

	// Token: 0x170001D2 RID: 466
	// (get) Token: 0x06001820 RID: 6176 RVA: 0x000B278C File Offset: 0x000B098C
	// (set) Token: 0x06001821 RID: 6177 RVA: 0x000B2794 File Offset: 0x000B0994
	public bool Sleeping { get; private set; }

	// Token: 0x170001D3 RID: 467
	// (get) Token: 0x06001822 RID: 6178 RVA: 0x000B279D File Offset: 0x000B099D
	public int SleepingCount
	{
		get
		{
			if (!this.Sleeping)
			{
				return 0;
			}
			return this.sleepables.Count;
		}
	}

	// Token: 0x06001823 RID: 6179 RVA: 0x000B27B4 File Offset: 0x000B09B4
	public void Start()
	{
		this.Init();
	}

	// Token: 0x06001824 RID: 6180 RVA: 0x000B27BC File Offset: 0x000B09BC
	public void Init()
	{
		if (this.initd)
		{
			return;
		}
		this.initd = true;
		this.AddInitialPoints();
		this.areaBox = new OBB(base.transform.position, base.transform.lossyScale, base.transform.rotation, this.bounds);
		AIInformationZone.zones.Add(this);
		this.grid = base.GetComponent<AIInformationGrid>();
		if (this.grid != null)
		{
			this.grid.Init();
		}
	}

	// Token: 0x06001825 RID: 6181 RVA: 0x000B2841 File Offset: 0x000B0A41
	public void RegisterSleepableEntity(IAISleepable sleepable)
	{
		if (sleepable == null)
		{
			return;
		}
		if (!sleepable.AllowedToSleep())
		{
			return;
		}
		if (this.sleepables.Contains(sleepable))
		{
			return;
		}
		this.sleepables.Add(sleepable);
		if (this.Sleeping && sleepable.AllowedToSleep())
		{
			sleepable.SleepAI();
		}
	}

	// Token: 0x06001826 RID: 6182 RVA: 0x000B2881 File Offset: 0x000B0A81
	public void UnregisterSleepableEntity(IAISleepable sleepable)
	{
		if (sleepable == null)
		{
			return;
		}
		this.sleepables.Remove(sleepable);
	}

	// Token: 0x06001827 RID: 6183 RVA: 0x000B2894 File Offset: 0x000B0A94
	public void SleepAI()
	{
		if (!AI.sleepwake)
		{
			return;
		}
		if (!this.ShouldSleepAI)
		{
			return;
		}
		foreach (IAISleepable iaisleepable in this.sleepables)
		{
			if (iaisleepable != null)
			{
				iaisleepable.SleepAI();
			}
		}
		this.Sleeping = true;
	}

	// Token: 0x06001828 RID: 6184 RVA: 0x000B2904 File Offset: 0x000B0B04
	public void WakeAI()
	{
		foreach (IAISleepable iaisleepable in this.sleepables)
		{
			if (iaisleepable != null)
			{
				iaisleepable.WakeAI();
			}
		}
		this.Sleeping = false;
	}

	// Token: 0x06001829 RID: 6185 RVA: 0x000B2960 File Offset: 0x000B0B60
	private void AddCoverPoint(AICoverPoint point)
	{
		if (this.coverPoints.Contains(point))
		{
			return;
		}
		this.coverPoints.Add(point);
		this.MarkDirty(false);
	}

	// Token: 0x0600182A RID: 6186 RVA: 0x000B2984 File Offset: 0x000B0B84
	private void RemoveCoverPoint(AICoverPoint point, bool markDirty = true)
	{
		this.coverPoints.Remove(point);
		if (markDirty)
		{
			this.MarkDirty(false);
		}
	}

	// Token: 0x0600182B RID: 6187 RVA: 0x000B299D File Offset: 0x000B0B9D
	private void AddMovePoint(AIMovePoint point)
	{
		if (this.movePoints.Contains(point))
		{
			return;
		}
		this.movePoints.Add(point);
		this.MarkDirty(false);
	}

	// Token: 0x0600182C RID: 6188 RVA: 0x000B29C1 File Offset: 0x000B0BC1
	private void RemoveMovePoint(AIMovePoint point, bool markDirty = true)
	{
		this.movePoints.Remove(point);
		if (markDirty)
		{
			this.MarkDirty(false);
		}
	}

	// Token: 0x0600182D RID: 6189 RVA: 0x000B29DC File Offset: 0x000B0BDC
	public void MarkDirty(bool completeRefresh = false)
	{
		this.isDirty = true;
		this.processIndex = 0;
		this.halfPaths = 0;
		this.pathSuccesses = 0;
		this.pathFails = 0;
		if (completeRefresh)
		{
			Debug.Log("AIInformationZone performing complete refresh, please wait...");
			foreach (AIMovePoint aimovePoint in this.movePoints)
			{
				aimovePoint.distances.Clear();
				aimovePoint.distancesToCover.Clear();
			}
		}
	}

	// Token: 0x0600182E RID: 6190 RVA: 0x000B2A6C File Offset: 0x000B0C6C
	private bool PassesBudget(float startTime, float budgetSeconds)
	{
		return UnityEngine.Time.realtimeSinceStartup - startTime <= budgetSeconds;
	}

	// Token: 0x0600182F RID: 6191 RVA: 0x00003A54 File Offset: 0x00001C54
	public bool ProcessDistancesAttempt()
	{
		return true;
	}

	// Token: 0x06001830 RID: 6192 RVA: 0x000B2A7C File Offset: 0x000B0C7C
	private bool ProcessDistances()
	{
		if (!this.UseCalculatedCoverDistances)
		{
			return true;
		}
		float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
		float budgetSeconds = AIThinkManager.framebudgetms / 1000f * 0.25f;
		if (realtimeSinceStartup < AIInformationZone.lastNavmeshBuildTime + 60f)
		{
			budgetSeconds = 0.1f;
		}
		int areaMask = 1 << NavMesh.GetAreaFromName("HumanNPC");
		NavMeshPath navMeshPath = new NavMeshPath();
		while (this.PassesBudget(realtimeSinceStartup, budgetSeconds))
		{
			AIMovePoint aimovePoint = this.movePoints[this.processIndex];
			bool flag = true;
			int num = 0;
			for (int i = aimovePoint.distances.Keys.Count - 1; i >= 0; i--)
			{
				AIMovePoint aimovePoint2 = aimovePoint.distances.Keys[i];
				if (!this.movePoints.Contains(aimovePoint2))
				{
					aimovePoint.distances.Remove(aimovePoint2);
				}
			}
			for (int j = aimovePoint.distancesToCover.Keys.Count - 1; j >= 0; j--)
			{
				AICoverPoint aicoverPoint = aimovePoint.distancesToCover.Keys[j];
				if (!this.coverPoints.Contains(aicoverPoint))
				{
					num++;
					aimovePoint.distancesToCover.Remove(aicoverPoint);
				}
			}
			foreach (AICoverPoint aicoverPoint2 in this.coverPoints)
			{
				if (!(aicoverPoint2 == null) && !aimovePoint.distancesToCover.Contains(aicoverPoint2))
				{
					float val;
					if (Vector3.Distance(aimovePoint.transform.position, aicoverPoint2.transform.position) > 40f)
					{
						val = -2f;
					}
					else if (NavMesh.CalculatePath(aimovePoint.transform.position, aicoverPoint2.transform.position, areaMask, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
					{
						int num2 = navMeshPath.corners.Length;
						if (num2 > 1)
						{
							Vector3 a = navMeshPath.corners[0];
							float num3 = 0f;
							for (int k = 0; k < num2; k++)
							{
								Vector3 vector = navMeshPath.corners[k];
								num3 += Vector3.Distance(a, vector);
								a = vector;
							}
							val = num3;
							this.pathSuccesses++;
						}
						else
						{
							val = Vector3.Distance(aimovePoint.transform.position, aicoverPoint2.transform.position);
							this.halfPaths++;
						}
					}
					else
					{
						this.pathFails++;
						val = -2f;
					}
					aimovePoint.distancesToCover.Add(aicoverPoint2, val);
					if (!this.PassesBudget(realtimeSinceStartup, budgetSeconds))
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				this.processIndex++;
			}
			if (this.processIndex >= this.movePoints.Count - 1)
			{
				break;
			}
		}
		return this.processIndex >= this.movePoints.Count - 1;
	}

	// Token: 0x06001831 RID: 6193 RVA: 0x000B2D9C File Offset: 0x000B0F9C
	public static void BudgetedTick()
	{
		if (!AI.move)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup < AIInformationZone.buildTimeTest)
		{
			return;
		}
		bool flag = false;
		foreach (AIInformationZone aiinformationZone in AIInformationZone.zones)
		{
			if (aiinformationZone.isDirty)
			{
				flag = true;
				bool flag2 = aiinformationZone.isDirty;
				aiinformationZone.isDirty = !aiinformationZone.ProcessDistancesAttempt();
				break;
			}
		}
		if (Global.developer > 0)
		{
			if (flag && !AIInformationZone.lastFrameAnyDirty)
			{
				Debug.Log("AIInformationZones rebuilding...");
				AIInformationZone.rebuildStartTime = UnityEngine.Time.realtimeSinceStartup;
			}
			if (AIInformationZone.lastFrameAnyDirty && !flag)
			{
				Debug.Log("AIInformationZone rebuild complete! Duration : " + (UnityEngine.Time.realtimeSinceStartup - AIInformationZone.rebuildStartTime) + " seconds.");
			}
		}
		AIInformationZone.lastFrameAnyDirty = flag;
	}

	// Token: 0x06001832 RID: 6194 RVA: 0x000B2E7C File Offset: 0x000B107C
	public void NavmeshBuildingComplete()
	{
		AIInformationZone.lastNavmeshBuildTime = UnityEngine.Time.realtimeSinceStartup;
		AIInformationZone.buildTimeTest = UnityEngine.Time.realtimeSinceStartup + 15f;
		this.MarkDirty(true);
	}

	// Token: 0x06001833 RID: 6195 RVA: 0x000B2E9F File Offset: 0x000B109F
	public Vector3 ClosestPointTo(Vector3 target)
	{
		return this.areaBox.ClosestPoint(target);
	}

	// Token: 0x06001834 RID: 6196 RVA: 0x000B2EB0 File Offset: 0x000B10B0
	public void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		Gizmos.DrawCube(this.bounds.center, this.bounds.size);
	}

	// Token: 0x06001835 RID: 6197 RVA: 0x000B2F08 File Offset: 0x000B1108
	public void AddInitialPoints()
	{
		foreach (AICoverPoint point in base.transform.GetComponentsInChildren<AICoverPoint>())
		{
			this.AddCoverPoint(point);
		}
		foreach (AIMovePoint point2 in base.transform.GetComponentsInChildren<AIMovePoint>(true))
		{
			this.AddMovePoint(point2);
		}
		this.RefreshPointArrays();
		NavMeshLink[] componentsInChildren3 = base.transform.GetComponentsInChildren<NavMeshLink>(true);
		this.navMeshLinks.AddRange(componentsInChildren3);
		AIMovePointPath[] componentsInChildren4 = base.transform.GetComponentsInChildren<AIMovePointPath>();
		this.paths.AddRange(componentsInChildren4);
	}

	// Token: 0x06001836 RID: 6198 RVA: 0x000B2F9E File Offset: 0x000B119E
	private void RefreshPointArrays()
	{
		List<AIMovePoint> list = this.movePoints;
		this.movePointArray = ((list != null) ? list.ToArray() : null);
		List<AICoverPoint> list2 = this.coverPoints;
		this.coverPointArray = ((list2 != null) ? list2.ToArray() : null);
	}

	// Token: 0x06001837 RID: 6199 RVA: 0x000B2FD0 File Offset: 0x000B11D0
	public void AddDynamicAIPoints(AIMovePoint[] movePoints, AICoverPoint[] coverPoints, Func<Vector3, bool> validatePoint = null)
	{
		if (movePoints != null)
		{
			foreach (AIMovePoint aimovePoint in movePoints)
			{
				if (!(aimovePoint == null) && (validatePoint == null || (validatePoint != null && validatePoint(aimovePoint.transform.position))))
				{
					this.AddMovePoint(aimovePoint);
				}
			}
		}
		if (coverPoints != null)
		{
			foreach (AICoverPoint aicoverPoint in coverPoints)
			{
				if (!(aicoverPoint == null) && (validatePoint == null || (validatePoint != null && validatePoint(aicoverPoint.transform.position))))
				{
					this.AddCoverPoint(aicoverPoint);
				}
			}
		}
		this.RefreshPointArrays();
	}

	// Token: 0x06001838 RID: 6200 RVA: 0x000B3068 File Offset: 0x000B1268
	public void RemoveDynamicAIPoints(AIMovePoint[] movePoints, AICoverPoint[] coverPoints)
	{
		if (movePoints != null)
		{
			foreach (AIMovePoint aimovePoint in movePoints)
			{
				if (!(aimovePoint == null))
				{
					this.RemoveMovePoint(aimovePoint, false);
				}
			}
		}
		if (coverPoints != null)
		{
			foreach (AICoverPoint aicoverPoint in coverPoints)
			{
				if (!(aicoverPoint == null))
				{
					this.RemoveCoverPoint(aicoverPoint, false);
				}
			}
		}
		this.MarkDirty(false);
		this.RefreshPointArrays();
	}

	// Token: 0x06001839 RID: 6201 RVA: 0x000B30D8 File Offset: 0x000B12D8
	public AIMovePointPath GetNearestPath(Vector3 position)
	{
		if (this.paths == null || this.paths.Count == 0)
		{
			return null;
		}
		float num = float.MaxValue;
		AIMovePointPath result = null;
		foreach (AIMovePointPath aimovePointPath in this.paths)
		{
			foreach (AIMovePoint aimovePoint in aimovePointPath.Points)
			{
				float num2 = Vector3.SqrMagnitude(aimovePoint.transform.position - position);
				if (num2 < num)
				{
					num = num2;
					result = aimovePointPath;
				}
			}
		}
		return result;
	}

	// Token: 0x0600183A RID: 6202 RVA: 0x000B31A0 File Offset: 0x000B13A0
	public static AIInformationZone GetForPoint(Vector3 point, bool fallBackToNearest = true)
	{
		if (AIInformationZone.zones == null || AIInformationZone.zones.Count == 0)
		{
			return null;
		}
		foreach (AIInformationZone aiinformationZone in AIInformationZone.zones)
		{
			if (!(aiinformationZone == null) && !aiinformationZone.Virtual && aiinformationZone.areaBox.Contains(point))
			{
				return aiinformationZone;
			}
		}
		if (!fallBackToNearest)
		{
			return null;
		}
		float num = float.PositiveInfinity;
		AIInformationZone aiinformationZone2 = AIInformationZone.zones[0];
		foreach (AIInformationZone aiinformationZone3 in AIInformationZone.zones)
		{
			if (!(aiinformationZone3 == null) && !(aiinformationZone3.transform == null) && !aiinformationZone3.Virtual)
			{
				float num2 = Vector3.Distance(aiinformationZone3.transform.position, point);
				if (num2 < num)
				{
					num = num2;
					aiinformationZone2 = aiinformationZone3;
				}
			}
		}
		if (aiinformationZone2.Virtual)
		{
			aiinformationZone2 = null;
		}
		return aiinformationZone2;
	}

	// Token: 0x0600183B RID: 6203 RVA: 0x000B32C8 File Offset: 0x000B14C8
	public bool PointInside(Vector3 point)
	{
		return this.areaBox.Contains(point);
	}

	// Token: 0x0600183C RID: 6204 RVA: 0x000B32D8 File Offset: 0x000B14D8
	public AIMovePoint GetBestMovePointNear(Vector3 targetPosition, Vector3 fromPosition, float minRange, float maxRange, bool checkLOS = false, BaseEntity forObject = null, bool returnClosest = false)
	{
		AIPoint aipoint = null;
		AIPoint aipoint2 = null;
		float num = -1f;
		float num2 = float.PositiveInfinity;
		int num3;
		AIPoint[] movePointsInRange = this.GetMovePointsInRange(targetPosition, maxRange, out num3);
		if (movePointsInRange == null || num3 <= 0)
		{
			return null;
		}
		for (int i = 0; i < num3; i++)
		{
			AIPoint aipoint3 = movePointsInRange[i];
			if (aipoint3.transform.parent.gameObject.activeSelf && (fromPosition.y < WaterSystem.OceanLevel || aipoint3.transform.position.y >= WaterSystem.OceanLevel))
			{
				float num4 = 0f;
				Vector3 position = aipoint3.transform.position;
				float num5 = Vector3.Distance(targetPosition, position);
				if (num5 < num2)
				{
					aipoint2 = aipoint3;
					num2 = num5;
				}
				if (num5 <= maxRange)
				{
					num4 += (aipoint3.CanBeUsedBy(forObject) ? 100f : 0f);
					num4 += (1f - Mathf.InverseLerp(minRange, maxRange, num5)) * 100f;
					if (num4 >= num && (!checkLOS || !UnityEngine.Physics.Linecast(targetPosition + Vector3.up * 1f, position + Vector3.up * 1f, 1218519297, QueryTriggerInteraction.Ignore)) && num4 > num)
					{
						aipoint = aipoint3;
						num = num4;
					}
				}
			}
		}
		if (aipoint == null && returnClosest)
		{
			return aipoint2 as AIMovePoint;
		}
		return aipoint as AIMovePoint;
	}

	// Token: 0x0600183D RID: 6205 RVA: 0x000B3444 File Offset: 0x000B1644
	public AIPoint[] GetMovePointsInRange(Vector3 currentPos, float maxRange, out int pointCount)
	{
		pointCount = 0;
		AIMovePoint[] movePointsInRange;
		if (this.grid != null && AI.usegrid)
		{
			movePointsInRange = this.grid.GetMovePointsInRange(currentPos, maxRange, out pointCount);
		}
		else
		{
			movePointsInRange = this.movePointArray;
			if (movePointsInRange != null)
			{
				pointCount = movePointsInRange.Length;
			}
		}
		return movePointsInRange;
	}

	// Token: 0x0600183E RID: 6206 RVA: 0x000B348C File Offset: 0x000B168C
	private AIMovePoint GetClosestRaw(Vector3 pos, bool onlyIncludeWithCover = false)
	{
		AIMovePoint result = null;
		float num = float.PositiveInfinity;
		foreach (AIMovePoint aimovePoint in this.movePoints)
		{
			if (!onlyIncludeWithCover || aimovePoint.distancesToCover.Count != 0)
			{
				float num2 = Vector3.Distance(aimovePoint.transform.position, pos);
				if (num2 < num)
				{
					num = num2;
					result = aimovePoint;
				}
			}
		}
		return result;
	}

	// Token: 0x0600183F RID: 6207 RVA: 0x000B3510 File Offset: 0x000B1710
	public AICoverPoint GetBestCoverPoint(Vector3 currentPosition, Vector3 hideFromPosition, float minRange = 0f, float maxRange = 20f, BaseEntity forObject = null, bool allowObjectToReuse = true)
	{
		AICoverPoint aicoverPoint = null;
		float num = 0f;
		AIMovePoint closestRaw = this.GetClosestRaw(currentPosition, true);
		int num2;
		AICoverPoint[] coverPointsInRange = this.GetCoverPointsInRange(currentPosition, maxRange, out num2);
		if (coverPointsInRange == null || num2 <= 0)
		{
			return null;
		}
		for (int i = 0; i < num2; i++)
		{
			AICoverPoint aicoverPoint2 = coverPointsInRange[i];
			Vector3 position = aicoverPoint2.transform.position;
			Vector3 normalized = (hideFromPosition - position).normalized;
			float num3 = Vector3.Dot(aicoverPoint2.transform.forward, normalized);
			if (num3 >= 1f - aicoverPoint2.coverDot)
			{
				float num4;
				if (this.UseCalculatedCoverDistances && closestRaw != null && closestRaw.distancesToCover.Contains(aicoverPoint2) && !this.isDirty)
				{
					num4 = closestRaw.distancesToCover[aicoverPoint2];
					if (num4 == -2f)
					{
						goto IL_20D;
					}
				}
				else
				{
					num4 = Vector3.Distance(currentPosition, position);
				}
				float num5 = 0f;
				if (aicoverPoint2.InUse())
				{
					bool flag = aicoverPoint2.IsUsedBy(forObject);
					if (!allowObjectToReuse || !flag)
					{
						num5 -= 1000f;
					}
				}
				if (minRange > 0f)
				{
					num5 -= (1f - Mathf.InverseLerp(0f, minRange, num4)) * 100f;
				}
				float value = Mathf.Abs(position.y - currentPosition.y);
				num5 += (1f - Mathf.InverseLerp(1f, 5f, value)) * 500f;
				num5 += Mathf.InverseLerp(1f - aicoverPoint2.coverDot, 1f, num3) * 50f;
				num5 += (1f - Mathf.InverseLerp(2f, maxRange, num4)) * 100f;
				float num6 = 1f - Mathf.InverseLerp(4f, 10f, Vector3.Distance(currentPosition, hideFromPosition));
				float value2 = Vector3.Dot((aicoverPoint2.transform.position - currentPosition).normalized, normalized);
				num5 -= Mathf.InverseLerp(-1f, 0.25f, value2) * 50f * num6;
				if (num5 > num)
				{
					aicoverPoint = aicoverPoint2;
					num = num5;
				}
			}
			IL_20D:;
		}
		if (aicoverPoint)
		{
			return aicoverPoint;
		}
		return null;
	}

	// Token: 0x06001840 RID: 6208 RVA: 0x000B3744 File Offset: 0x000B1944
	private AICoverPoint[] GetCoverPointsInRange(Vector3 position, float maxRange, out int pointCount)
	{
		pointCount = 0;
		AICoverPoint[] coverPointsInRange;
		if (this.grid != null && AI.usegrid)
		{
			coverPointsInRange = this.grid.GetCoverPointsInRange(position, maxRange, out pointCount);
		}
		else
		{
			coverPointsInRange = this.coverPointArray;
			if (coverPointsInRange != null)
			{
				pointCount = coverPointsInRange.Length;
			}
		}
		return coverPointsInRange;
	}

	// Token: 0x06001841 RID: 6209 RVA: 0x000B378C File Offset: 0x000B198C
	public NavMeshLink GetClosestNavMeshLink(Vector3 pos)
	{
		NavMeshLink result = null;
		float num = float.PositiveInfinity;
		foreach (NavMeshLink navMeshLink in this.navMeshLinks)
		{
			float num2 = Vector3.Distance(navMeshLink.gameObject.transform.position, pos);
			if (num2 < num)
			{
				result = navMeshLink;
				num = num2;
				if (num2 < 0.25f)
				{
					break;
				}
			}
		}
		return result;
	}

	// Token: 0x0400115D RID: 4445
	public bool ShouldSleepAI;

	// Token: 0x0400115E RID: 4446
	public bool Virtual;

	// Token: 0x0400115F RID: 4447
	public bool UseCalculatedCoverDistances = true;

	// Token: 0x04001160 RID: 4448
	public static List<AIInformationZone> zones = new List<AIInformationZone>();

	// Token: 0x04001161 RID: 4449
	public List<AICoverPoint> coverPoints = new List<AICoverPoint>();

	// Token: 0x04001162 RID: 4450
	public List<AIMovePoint> movePoints = new List<AIMovePoint>();

	// Token: 0x04001163 RID: 4451
	private AICoverPoint[] coverPointArray;

	// Token: 0x04001164 RID: 4452
	private AIMovePoint[] movePointArray;

	// Token: 0x04001165 RID: 4453
	public List<NavMeshLink> navMeshLinks = new List<NavMeshLink>();

	// Token: 0x04001166 RID: 4454
	public List<AIMovePointPath> paths = new List<AIMovePointPath>();

	// Token: 0x04001167 RID: 4455
	public Bounds bounds;

	// Token: 0x04001168 RID: 4456
	private AIInformationGrid grid;

	// Token: 0x0400116A RID: 4458
	private List<IAISleepable> sleepables = new List<IAISleepable>();

	// Token: 0x0400116B RID: 4459
	private OBB areaBox;

	// Token: 0x0400116C RID: 4460
	private bool isDirty = true;

	// Token: 0x0400116D RID: 4461
	private int processIndex;

	// Token: 0x0400116E RID: 4462
	private int halfPaths;

	// Token: 0x0400116F RID: 4463
	private int pathSuccesses;

	// Token: 0x04001170 RID: 4464
	private int pathFails;

	// Token: 0x04001171 RID: 4465
	private bool initd;

	// Token: 0x04001172 RID: 4466
	private static bool lastFrameAnyDirty = false;

	// Token: 0x04001173 RID: 4467
	private static float rebuildStartTime = 0f;

	// Token: 0x04001174 RID: 4468
	public static float buildTimeTest = 0f;

	// Token: 0x04001175 RID: 4469
	private static float lastNavmeshBuildTime = 0f;
}
