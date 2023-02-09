using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x0200097B RID: 2427
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(VolumetricLightBeam))]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-dynocclusion/")]
	public class DynamicOcclusion : MonoBehaviour
	{
		// Token: 0x0600397F RID: 14719 RVA: 0x00153EE5 File Offset: 0x001520E5
		private void OnValidate()
		{
			this.minOccluderArea = Mathf.Max(this.minOccluderArea, 0f);
			this.waitFrameCount = Mathf.Clamp(this.waitFrameCount, 1, 60);
		}

		// Token: 0x06003980 RID: 14720 RVA: 0x00153F11 File Offset: 0x00152111
		private void OnEnable()
		{
			this.m_Master = base.GetComponent<VolumetricLightBeam>();
			Debug.Assert(this.m_Master);
		}

		// Token: 0x06003981 RID: 14721 RVA: 0x00153F2F File Offset: 0x0015212F
		private void OnDisable()
		{
			this.SetHitNull();
		}

		// Token: 0x06003982 RID: 14722 RVA: 0x00153F38 File Offset: 0x00152138
		private void Start()
		{
			if (Application.isPlaying)
			{
				TriggerZone component = base.GetComponent<TriggerZone>();
				if (component)
				{
					this.m_RangeMultiplier = Mathf.Max(1f, component.rangeMultiplier);
				}
			}
		}

		// Token: 0x06003983 RID: 14723 RVA: 0x00153F71 File Offset: 0x00152171
		private void LateUpdate()
		{
			if (this.m_FrameCountToWait <= 0)
			{
				this.ProcessRaycasts();
				this.m_FrameCountToWait = this.waitFrameCount;
			}
			this.m_FrameCountToWait--;
		}

		// Token: 0x06003984 RID: 14724 RVA: 0x00153F9C File Offset: 0x0015219C
		private Vector3 GetRandomVectorAround(Vector3 direction, float angleDiff)
		{
			float num = angleDiff * 0.5f;
			return Quaternion.Euler(UnityEngine.Random.Range(-num, num), UnityEngine.Random.Range(-num, num), UnityEngine.Random.Range(-num, num)) * direction;
		}

		// Token: 0x06003985 RID: 14725 RVA: 0x00153FD4 File Offset: 0x001521D4
		private RaycastHit GetBestHit(Vector3 rayPos, Vector3 rayDir)
		{
			RaycastHit[] array = Physics.RaycastAll(rayPos, rayDir, this.m_Master.fadeEnd * this.m_RangeMultiplier, this.layerMask.value);
			int num = -1;
			float num2 = float.MaxValue;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].collider.isTrigger && array[i].collider.bounds.GetMaxArea2D() >= this.minOccluderArea && array[i].distance < num2)
				{
					num2 = array[i].distance;
					num = i;
				}
			}
			if (num != -1)
			{
				return array[num];
			}
			return default(RaycastHit);
		}

		// Token: 0x06003986 RID: 14726 RVA: 0x00154084 File Offset: 0x00152284
		private Vector3 GetDirection(uint dirInt)
		{
			dirInt %= (uint)Enum.GetValues(typeof(DynamicOcclusion.Direction)).Length;
			switch (dirInt)
			{
			case 0U:
				return base.transform.up;
			case 1U:
				return base.transform.right;
			case 2U:
				return -base.transform.up;
			case 3U:
				return -base.transform.right;
			default:
				return Vector3.zero;
			}
		}

		// Token: 0x06003987 RID: 14727 RVA: 0x00154100 File Offset: 0x00152300
		private bool IsHitValid(RaycastHit hit)
		{
			return hit.collider && Vector3.Dot(hit.normal, -base.transform.forward) >= this.maxSurfaceDot;
		}

		// Token: 0x06003988 RID: 14728 RVA: 0x0015413C File Offset: 0x0015233C
		private void ProcessRaycasts()
		{
			RaycastHit hit = this.GetBestHit(base.transform.position, base.transform.forward);
			if (this.IsHitValid(hit))
			{
				if (this.minSurfaceRatio > 0.5f)
				{
					for (uint num = 0U; num < (uint)Enum.GetValues(typeof(DynamicOcclusion.Direction)).Length; num += 1U)
					{
						Vector3 direction = this.GetDirection(num + this.m_PrevNonSubHitDirectionId);
						Vector3 vector = base.transform.position + direction * this.m_Master.coneRadiusStart * (this.minSurfaceRatio * 2f - 1f);
						Vector3 a = base.transform.position + base.transform.forward * this.m_Master.fadeEnd + direction * this.m_Master.coneRadiusEnd * (this.minSurfaceRatio * 2f - 1f);
						RaycastHit bestHit = this.GetBestHit(vector, a - vector);
						if (!this.IsHitValid(bestHit))
						{
							this.m_PrevNonSubHitDirectionId = num;
							this.SetHitNull();
							return;
						}
						if (bestHit.distance > hit.distance)
						{
							hit = bestHit;
						}
					}
				}
				this.SetHit(hit);
				return;
			}
			this.SetHitNull();
		}

		// Token: 0x06003989 RID: 14729 RVA: 0x00154298 File Offset: 0x00152498
		private void SetHit(RaycastHit hit)
		{
			PlaneAlignment planeAlignment = this.planeAlignment;
			if (planeAlignment != PlaneAlignment.Surface && planeAlignment == PlaneAlignment.Beam)
			{
				this.SetClippingPlane(new Plane(-base.transform.forward, hit.point));
				return;
			}
			this.SetClippingPlane(new Plane(hit.normal, hit.point));
		}

		// Token: 0x0600398A RID: 14730 RVA: 0x001542EF File Offset: 0x001524EF
		private void SetHitNull()
		{
			this.SetClippingPlaneOff();
		}

		// Token: 0x0600398B RID: 14731 RVA: 0x001542F7 File Offset: 0x001524F7
		private void SetClippingPlane(Plane planeWS)
		{
			planeWS = planeWS.TranslateCustom(planeWS.normal * this.planeOffset);
			this.m_Master.SetClippingPlane(planeWS);
		}

		// Token: 0x0600398C RID: 14732 RVA: 0x0015431F File Offset: 0x0015251F
		private void SetClippingPlaneOff()
		{
			this.m_Master.SetClippingPlaneOff();
		}

		// Token: 0x04003417 RID: 13335
		public LayerMask layerMask = -1;

		// Token: 0x04003418 RID: 13336
		public float minOccluderArea;

		// Token: 0x04003419 RID: 13337
		public int waitFrameCount = 3;

		// Token: 0x0400341A RID: 13338
		public float minSurfaceRatio = 0.5f;

		// Token: 0x0400341B RID: 13339
		public float maxSurfaceDot = 0.25f;

		// Token: 0x0400341C RID: 13340
		public PlaneAlignment planeAlignment;

		// Token: 0x0400341D RID: 13341
		public float planeOffset = 0.1f;

		// Token: 0x0400341E RID: 13342
		private VolumetricLightBeam m_Master;

		// Token: 0x0400341F RID: 13343
		private int m_FrameCountToWait;

		// Token: 0x04003420 RID: 13344
		private float m_RangeMultiplier = 1f;

		// Token: 0x04003421 RID: 13345
		private uint m_PrevNonSubHitDirectionId;

		// Token: 0x02000E83 RID: 3715
		private enum Direction
		{
			// Token: 0x04004ADC RID: 19164
			Up,
			// Token: 0x04004ADD RID: 19165
			Right,
			// Token: 0x04004ADE RID: 19166
			Down,
			// Token: 0x04004ADF RID: 19167
			Left
		}
	}
}
