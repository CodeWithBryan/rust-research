using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
	// Token: 0x02000AF5 RID: 2805
	public class CoverPointVolume : MonoBehaviour, IServerComponent
	{
		// Token: 0x170005E3 RID: 1507
		// (get) Token: 0x06004368 RID: 17256 RVA: 0x00003A54 File Offset: 0x00001C54
		public bool repeat
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004369 RID: 17257 RVA: 0x00186FF4 File Offset: 0x001851F4
		public float? ExecuteUpdate(float deltaTime, float nextInterval)
		{
			if (this.CoverPoints.Count == 0)
			{
				if (this._dynNavMeshBuildCompletionTime < 0f)
				{
					if (SingletonComponent<DynamicNavMesh>.Instance == null || !SingletonComponent<DynamicNavMesh>.Instance.enabled || !SingletonComponent<DynamicNavMesh>.Instance.IsBuilding)
					{
						this._dynNavMeshBuildCompletionTime = Time.realtimeSinceStartup;
					}
				}
				else if (this._genAttempts < 4 && Time.realtimeSinceStartup - this._dynNavMeshBuildCompletionTime > 0.25f)
				{
					this.GenerateCoverPoints(null);
					if (this.CoverPoints.Count != 0)
					{
						return null;
					}
					this._dynNavMeshBuildCompletionTime = Time.realtimeSinceStartup;
					this._genAttempts++;
					if (this._genAttempts >= 4)
					{
						UnityEngine.Object.Destroy(base.gameObject);
						return null;
					}
				}
			}
			return new float?(1f + UnityEngine.Random.value * 2f);
		}

		// Token: 0x0600436A RID: 17258 RVA: 0x001870D7 File Offset: 0x001852D7
		[ContextMenu("Clear Cover Points")]
		private void ClearCoverPoints()
		{
			this.CoverPoints.Clear();
			this._coverPointBlockers.Clear();
		}

		// Token: 0x0600436B RID: 17259 RVA: 0x001870F0 File Offset: 0x001852F0
		public Bounds GetBounds()
		{
			if (Mathf.Approximately(this.bounds.center.sqrMagnitude, 0f))
			{
				this.bounds = new Bounds(base.transform.position, base.transform.localScale);
			}
			return this.bounds;
		}

		// Token: 0x0600436C RID: 17260 RVA: 0x00187143 File Offset: 0x00185343
		[ContextMenu("Pre-Generate Cover Points")]
		public void PreGenerateCoverPoints()
		{
			this.GenerateCoverPoints(null);
		}

		// Token: 0x0600436D RID: 17261 RVA: 0x0018714C File Offset: 0x0018534C
		[ContextMenu("Convert to Manual Cover Points")]
		public void ConvertToManualCoverPoints()
		{
			foreach (CoverPoint coverPoint in this.CoverPoints)
			{
				ManualCoverPoint manualCoverPoint = new GameObject("MCP").AddComponent<ManualCoverPoint>();
				manualCoverPoint.transform.localPosition = Vector3.zero;
				manualCoverPoint.transform.position = coverPoint.Position;
				manualCoverPoint.Normal = coverPoint.Normal;
				manualCoverPoint.NormalCoverType = coverPoint.NormalCoverType;
				manualCoverPoint.Volume = this;
			}
		}

		// Token: 0x0600436E RID: 17262 RVA: 0x001871E8 File Offset: 0x001853E8
		public void GenerateCoverPoints(Transform coverPointGroup)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			this.ClearCoverPoints();
			if (this.ManualCoverPointGroup == null)
			{
				this.ManualCoverPointGroup = coverPointGroup;
			}
			if (this.ManualCoverPointGroup == null)
			{
				this.ManualCoverPointGroup = base.transform;
			}
			if (this.ManualCoverPointGroup.childCount > 0)
			{
				ManualCoverPoint[] componentsInChildren = this.ManualCoverPointGroup.GetComponentsInChildren<ManualCoverPoint>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					CoverPoint item = componentsInChildren[i].ToCoverPoint(this);
					this.CoverPoints.Add(item);
				}
			}
			if (this._coverPointBlockers.Count == 0 && this.BlockerGroup != null)
			{
				CoverPointBlockerVolume[] componentsInChildren2 = this.BlockerGroup.GetComponentsInChildren<CoverPointBlockerVolume>();
				if (componentsInChildren2 != null && componentsInChildren2.Length != 0)
				{
					this._coverPointBlockers.AddRange(componentsInChildren2);
				}
			}
			NavMeshHit navMeshHit;
			if (this.CoverPoints.Count == 0 && NavMesh.SamplePosition(base.transform.position, out navMeshHit, base.transform.localScale.y * CoverPointVolume.cover_point_sample_step_height, -1))
			{
				Vector3 position = base.transform.position;
				Vector3 vector = base.transform.lossyScale * 0.5f;
				for (float num = position.x - vector.x + 1f; num < position.x + vector.x - 1f; num += CoverPointVolume.cover_point_sample_step_size)
				{
					for (float num2 = position.z - vector.z + 1f; num2 < position.z + vector.z - 1f; num2 += CoverPointVolume.cover_point_sample_step_size)
					{
						for (float num3 = position.y - vector.y; num3 < position.y + vector.y; num3 += CoverPointVolume.cover_point_sample_step_height)
						{
							NavMeshHit info;
							if (NavMesh.FindClosestEdge(new Vector3(num, num3, num2), out info, navMeshHit.mask))
							{
								info.position = new Vector3(info.position.x, info.position.y + 0.5f, info.position.z);
								bool flag = true;
								using (List<CoverPoint>.Enumerator enumerator = this.CoverPoints.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										if ((enumerator.Current.Position - info.position).sqrMagnitude < CoverPointVolume.cover_point_sample_step_size * CoverPointVolume.cover_point_sample_step_size)
										{
											flag = false;
											break;
										}
									}
								}
								if (flag)
								{
									CoverPoint coverPoint = this.CalculateCoverPoint(info);
									if (coverPoint != null)
									{
										this.CoverPoints.Add(coverPoint);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600436F RID: 17263 RVA: 0x001874A8 File Offset: 0x001856A8
		private CoverPoint CalculateCoverPoint(NavMeshHit info)
		{
			RaycastHit raycastHit;
			CoverPointVolume.CoverType coverType = this.ProvidesCoverInDir(new Ray(info.position, -info.normal), this.CoverPointRayLength, out raycastHit);
			if (coverType == CoverPointVolume.CoverType.None)
			{
				return null;
			}
			CoverPoint coverPoint = new CoverPoint(this, this.DefaultCoverPointScore)
			{
				Position = info.position,
				Normal = -info.normal
			};
			if (coverType == CoverPointVolume.CoverType.Full)
			{
				coverPoint.NormalCoverType = CoverPoint.CoverType.Full;
			}
			else if (coverType == CoverPointVolume.CoverType.Partial)
			{
				coverPoint.NormalCoverType = CoverPoint.CoverType.Partial;
			}
			return coverPoint;
		}

		// Token: 0x06004370 RID: 17264 RVA: 0x00187528 File Offset: 0x00185728
		internal CoverPointVolume.CoverType ProvidesCoverInDir(Ray ray, float maxDistance, out RaycastHit rayHit)
		{
			rayHit = default(RaycastHit);
			if (ray.origin.IsNaNOrInfinity())
			{
				return CoverPointVolume.CoverType.None;
			}
			if (ray.direction.IsNaNOrInfinity())
			{
				return CoverPointVolume.CoverType.None;
			}
			if (ray.direction == Vector3.zero)
			{
				return CoverPointVolume.CoverType.None;
			}
			ray.origin += PlayerEyes.EyeOffset;
			if (Physics.Raycast(ray.origin, ray.direction, out rayHit, maxDistance, this.CoverLayerMask))
			{
				return CoverPointVolume.CoverType.Full;
			}
			ray.origin += PlayerEyes.DuckOffset;
			if (Physics.Raycast(ray.origin, ray.direction, out rayHit, maxDistance, this.CoverLayerMask))
			{
				return CoverPointVolume.CoverType.Partial;
			}
			return CoverPointVolume.CoverType.None;
		}

		// Token: 0x06004371 RID: 17265 RVA: 0x001875EC File Offset: 0x001857EC
		public bool Contains(Vector3 point)
		{
			Bounds bounds = new Bounds(base.transform.position, base.transform.localScale);
			return bounds.Contains(point);
		}

		// Token: 0x04003BE7 RID: 15335
		public float DefaultCoverPointScore = 1f;

		// Token: 0x04003BE8 RID: 15336
		public float CoverPointRayLength = 1f;

		// Token: 0x04003BE9 RID: 15337
		public LayerMask CoverLayerMask;

		// Token: 0x04003BEA RID: 15338
		public Transform BlockerGroup;

		// Token: 0x04003BEB RID: 15339
		public Transform ManualCoverPointGroup;

		// Token: 0x04003BEC RID: 15340
		[ServerVar(Help = "cover_point_sample_step_size defines the size of the steps we do horizontally for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 6.0)")]
		public static float cover_point_sample_step_size = 6f;

		// Token: 0x04003BED RID: 15341
		[ServerVar(Help = "cover_point_sample_step_height defines the height of the steps we do vertically for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 2.0)")]
		public static float cover_point_sample_step_height = 2f;

		// Token: 0x04003BEE RID: 15342
		public readonly List<CoverPoint> CoverPoints = new List<CoverPoint>();

		// Token: 0x04003BEF RID: 15343
		private readonly List<CoverPointBlockerVolume> _coverPointBlockers = new List<CoverPointBlockerVolume>();

		// Token: 0x04003BF0 RID: 15344
		private float _dynNavMeshBuildCompletionTime = -1f;

		// Token: 0x04003BF1 RID: 15345
		private int _genAttempts;

		// Token: 0x04003BF2 RID: 15346
		private Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

		// Token: 0x02000F34 RID: 3892
		internal enum CoverType
		{
			// Token: 0x04004DA8 RID: 19880
			None,
			// Token: 0x04004DA9 RID: 19881
			Partial,
			// Token: 0x04004DAA RID: 19882
			Full
		}
	}
}
