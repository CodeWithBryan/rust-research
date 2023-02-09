using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x020002E3 RID: 739
public class ArticulatedOccludee : BaseMonoBehaviour
{
	// Token: 0x1700022C RID: 556
	// (get) Token: 0x06001D24 RID: 7460 RVA: 0x000C6DAA File Offset: 0x000C4FAA
	public bool IsVisible
	{
		get
		{
			return this.isVisible;
		}
	}

	// Token: 0x06001D25 RID: 7461 RVA: 0x000C6DB2 File Offset: 0x000C4FB2
	protected virtual void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.UnregisterFromCulling();
		this.ClearVisibility();
	}

	// Token: 0x06001D26 RID: 7462 RVA: 0x000C6DC8 File Offset: 0x000C4FC8
	private void ClearVisibility()
	{
		if (this.lodGroup != null)
		{
			this.lodGroup.localReferencePoint = Vector3.zero;
			this.lodGroup.RecalculateBounds();
			this.lodGroup = null;
		}
		if (this.renderers != null)
		{
			this.renderers.Clear();
		}
		this.localOccludee = new OccludeeSphere(-1);
	}

	// Token: 0x06001D27 RID: 7463 RVA: 0x000C6E24 File Offset: 0x000C5024
	public void ProcessVisibility(LODGroup lod)
	{
		this.lodGroup = lod;
		if (lod != null)
		{
			this.renderers = new List<Renderer>(16);
			LOD[] lods = lod.GetLODs();
			for (int i = 0; i < lods.Length; i++)
			{
				foreach (Renderer renderer in lods[i].renderers)
				{
					if (renderer != null)
					{
						this.renderers.Add(renderer);
					}
				}
			}
		}
		this.UpdateCullingBounds();
	}

	// Token: 0x06001D28 RID: 7464 RVA: 0x000C6EA0 File Offset: 0x000C50A0
	private void RegisterForCulling(OcclusionCulling.Sphere sphere, bool visible)
	{
		if (this.localOccludee.IsRegistered)
		{
			this.UnregisterFromCulling();
		}
		int num = OcclusionCulling.RegisterOccludee(sphere.position, sphere.radius, visible, 0.25f, false, base.gameObject.layer, new OcclusionCulling.OnVisibilityChanged(this.OnVisibilityChanged));
		if (num >= 0)
		{
			this.localOccludee = new OccludeeSphere(num, this.localOccludee.sphere);
			return;
		}
		this.localOccludee.Invalidate();
		Debug.LogWarning("[OcclusionCulling] Occludee registration failed for " + base.name + ". Too many registered.");
	}

	// Token: 0x06001D29 RID: 7465 RVA: 0x000C6F32 File Offset: 0x000C5132
	private void UnregisterFromCulling()
	{
		if (this.localOccludee.IsRegistered)
		{
			OcclusionCulling.UnregisterOccludee(this.localOccludee.id);
			this.localOccludee.Invalidate();
		}
	}

	// Token: 0x06001D2A RID: 7466 RVA: 0x000C6F5C File Offset: 0x000C515C
	public void UpdateCullingBounds()
	{
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		bool flag = false;
		int num = (this.renderers != null) ? this.renderers.Count : 0;
		int num2 = (this.colliders != null) ? this.colliders.Count : 0;
		if (num > 0 && (num2 == 0 || num < num2))
		{
			for (int i = 0; i < this.renderers.Count; i++)
			{
				if (this.renderers[i].isVisible)
				{
					Bounds bounds = this.renderers[i].bounds;
					Vector3 min = bounds.min;
					Vector3 max = bounds.max;
					if (!flag)
					{
						vector = min;
						vector2 = max;
						flag = true;
					}
					else
					{
						vector.x = ((vector.x < min.x) ? vector.x : min.x);
						vector.y = ((vector.y < min.y) ? vector.y : min.y);
						vector.z = ((vector.z < min.z) ? vector.z : min.z);
						vector2.x = ((vector2.x > max.x) ? vector2.x : max.x);
						vector2.y = ((vector2.y > max.y) ? vector2.y : max.y);
						vector2.z = ((vector2.z > max.z) ? vector2.z : max.z);
					}
				}
			}
		}
		if (!flag && num2 > 0)
		{
			flag = true;
			vector = this.colliders[0].bounds.min;
			vector2 = this.colliders[0].bounds.max;
			for (int j = 1; j < this.colliders.Count; j++)
			{
				Bounds bounds2 = this.colliders[j].bounds;
				Vector3 min2 = bounds2.min;
				Vector3 max2 = bounds2.max;
				vector.x = ((vector.x < min2.x) ? vector.x : min2.x);
				vector.y = ((vector.y < min2.y) ? vector.y : min2.y);
				vector.z = ((vector.z < min2.z) ? vector.z : min2.z);
				vector2.x = ((vector2.x > max2.x) ? vector2.x : max2.x);
				vector2.y = ((vector2.y > max2.y) ? vector2.y : max2.y);
				vector2.z = ((vector2.z > max2.z) ? vector2.z : max2.z);
			}
		}
		if (flag)
		{
			Vector3 vector3 = vector2 - vector;
			Vector3 position = vector + vector3 * 0.5f;
			float radius = Mathf.Max(Mathf.Max(vector3.x, vector3.y), vector3.z) * 0.5f;
			OcclusionCulling.Sphere sphere = new OcclusionCulling.Sphere(position, radius);
			if (this.localOccludee.IsRegistered)
			{
				OcclusionCulling.UpdateDynamicOccludee(this.localOccludee.id, sphere.position, sphere.radius);
				this.localOccludee.sphere = sphere;
				return;
			}
			bool visible = true;
			if (this.lodGroup != null)
			{
				visible = this.lodGroup.enabled;
			}
			this.RegisterForCulling(sphere, visible);
		}
	}

	// Token: 0x06001D2B RID: 7467 RVA: 0x000C7324 File Offset: 0x000C5524
	protected virtual bool CheckVisibility()
	{
		return this.localOccludee.state == null || this.localOccludee.state.isVisible;
	}

	// Token: 0x06001D2C RID: 7468 RVA: 0x000C7348 File Offset: 0x000C5548
	private void ApplyVisibility(bool vis)
	{
		if (this.lodGroup != null)
		{
			float num = (float)(vis ? 0 : 100000);
			if (num != this.lodGroup.localReferencePoint.x)
			{
				this.lodGroup.localReferencePoint = new Vector3(num, num, num);
			}
		}
	}

	// Token: 0x06001D2D RID: 7469 RVA: 0x000C7398 File Offset: 0x000C5598
	protected virtual void OnVisibilityChanged(bool visible)
	{
		if (MainCamera.mainCamera != null && this.localOccludee.IsRegistered)
		{
			float dist = Vector3.Distance(MainCamera.position, base.transform.position);
			this.VisUpdateUsingCulling(dist, visible);
			this.ApplyVisibility(this.isVisible);
		}
	}

	// Token: 0x06001D2E RID: 7470 RVA: 0x000059DD File Offset: 0x00003BDD
	private void UpdateVisibility(float delay)
	{
	}

	// Token: 0x06001D2F RID: 7471 RVA: 0x000059DD File Offset: 0x00003BDD
	private void VisUpdateUsingCulling(float dist, bool visibility)
	{
	}

	// Token: 0x06001D30 RID: 7472 RVA: 0x000C73EC File Offset: 0x000C55EC
	public virtual void TriggerUpdateVisibilityBounds()
	{
		if (base.enabled)
		{
			float sqrMagnitude = (base.transform.position - MainCamera.position).sqrMagnitude;
			float num = 400f;
			float num2;
			if (sqrMagnitude < num)
			{
				num2 = 1f / UnityEngine.Random.Range(5f, 25f);
			}
			else
			{
				float t = Mathf.Clamp01((Mathf.Sqrt(sqrMagnitude) - 20f) * 0.001f);
				float num3 = Mathf.Lerp(0.06666667f, 2f, t);
				num2 = UnityEngine.Random.Range(num3, num3 + 0.06666667f);
			}
			this.UpdateVisibility(num2);
			this.ApplyVisibility(this.isVisible);
			if (this.TriggerUpdateVisibilityBoundsDelegate == null)
			{
				this.TriggerUpdateVisibilityBoundsDelegate = new Action(this.TriggerUpdateVisibilityBounds);
			}
			base.Invoke(this.TriggerUpdateVisibilityBoundsDelegate, num2);
		}
	}

	// Token: 0x040016A2 RID: 5794
	private const float UpdateBoundsFadeStart = 20f;

	// Token: 0x040016A3 RID: 5795
	private const float UpdateBoundsFadeLength = 1000f;

	// Token: 0x040016A4 RID: 5796
	private const float UpdateBoundsMaxFrequency = 15f;

	// Token: 0x040016A5 RID: 5797
	private const float UpdateBoundsMinFrequency = 0.5f;

	// Token: 0x040016A6 RID: 5798
	private LODGroup lodGroup;

	// Token: 0x040016A7 RID: 5799
	public List<Collider> colliders = new List<Collider>();

	// Token: 0x040016A8 RID: 5800
	private OccludeeSphere localOccludee = new OccludeeSphere(-1);

	// Token: 0x040016A9 RID: 5801
	private List<Renderer> renderers = new List<Renderer>();

	// Token: 0x040016AA RID: 5802
	private bool isVisible = true;

	// Token: 0x040016AB RID: 5803
	private Action TriggerUpdateVisibilityBoundsDelegate;
}
