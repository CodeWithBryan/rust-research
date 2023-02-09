using System;
using UnityEngine;

// Token: 0x0200096B RID: 2411
public class Occludee : MonoBehaviour
{
	// Token: 0x060038D2 RID: 14546 RVA: 0x0014EFA4 File Offset: 0x0014D1A4
	protected virtual void Awake()
	{
		this.renderer = base.GetComponent<Renderer>();
		this.collider = base.GetComponent<Collider>();
	}

	// Token: 0x060038D3 RID: 14547 RVA: 0x0014EFBE File Offset: 0x0014D1BE
	public void OnEnable()
	{
		if (this.autoRegister && this.collider != null)
		{
			this.Register();
		}
	}

	// Token: 0x060038D4 RID: 14548 RVA: 0x0014EFDC File Offset: 0x0014D1DC
	public void OnDisable()
	{
		if (this.autoRegister && this.occludeeId >= 0)
		{
			this.Unregister();
		}
	}

	// Token: 0x060038D5 RID: 14549 RVA: 0x0014EFF8 File Offset: 0x0014D1F8
	public void Register()
	{
		this.center = this.collider.bounds.center;
		this.radius = Mathf.Max(Mathf.Max(this.collider.bounds.extents.x, this.collider.bounds.extents.y), this.collider.bounds.extents.z);
		this.occludeeId = OcclusionCulling.RegisterOccludee(this.center, this.radius, this.renderer.enabled, this.minTimeVisible, this.isStatic, base.gameObject.layer, new OcclusionCulling.OnVisibilityChanged(this.OnVisibilityChanged));
		if (this.occludeeId < 0)
		{
			Debug.LogWarning("[OcclusionCulling] Occludee registration failed for " + base.name + ". Too many registered.");
		}
		this.state = OcclusionCulling.GetStateById(this.occludeeId);
	}

	// Token: 0x060038D6 RID: 14550 RVA: 0x0014F0F0 File Offset: 0x0014D2F0
	public void Unregister()
	{
		OcclusionCulling.UnregisterOccludee(this.occludeeId);
	}

	// Token: 0x060038D7 RID: 14551 RVA: 0x0014F0FD File Offset: 0x0014D2FD
	protected virtual void OnVisibilityChanged(bool visible)
	{
		if (this.renderer != null)
		{
			this.renderer.enabled = visible;
		}
	}

	// Token: 0x04003339 RID: 13113
	public float minTimeVisible = 0.1f;

	// Token: 0x0400333A RID: 13114
	public bool isStatic = true;

	// Token: 0x0400333B RID: 13115
	public bool autoRegister;

	// Token: 0x0400333C RID: 13116
	public bool stickyGizmos;

	// Token: 0x0400333D RID: 13117
	public OccludeeState state;

	// Token: 0x0400333E RID: 13118
	protected int occludeeId = -1;

	// Token: 0x0400333F RID: 13119
	protected Vector3 center;

	// Token: 0x04003340 RID: 13120
	protected float radius;

	// Token: 0x04003341 RID: 13121
	protected Renderer renderer;

	// Token: 0x04003342 RID: 13122
	protected Collider collider;
}
