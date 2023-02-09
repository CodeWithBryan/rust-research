using System;
using UnityEngine;

// Token: 0x020004DC RID: 1244
public class EnvironmentVolume : MonoBehaviour
{
	// Token: 0x17000328 RID: 808
	// (get) Token: 0x060027A9 RID: 10153 RVA: 0x000F3398 File Offset: 0x000F1598
	// (set) Token: 0x060027AA RID: 10154 RVA: 0x000F33A0 File Offset: 0x000F15A0
	public Collider trigger { get; private set; }

	// Token: 0x060027AB RID: 10155 RVA: 0x000F33A9 File Offset: 0x000F15A9
	protected virtual void Awake()
	{
		this.UpdateTrigger();
	}

	// Token: 0x060027AC RID: 10156 RVA: 0x000F33B1 File Offset: 0x000F15B1
	protected void OnEnable()
	{
		if (this.trigger && !this.trigger.enabled)
		{
			this.trigger.enabled = true;
		}
	}

	// Token: 0x060027AD RID: 10157 RVA: 0x000F33D9 File Offset: 0x000F15D9
	protected void OnDisable()
	{
		if (this.trigger && this.trigger.enabled)
		{
			this.trigger.enabled = false;
		}
	}

	// Token: 0x060027AE RID: 10158 RVA: 0x000F3404 File Offset: 0x000F1604
	public void UpdateTrigger()
	{
		if (!this.trigger)
		{
			this.trigger = base.gameObject.GetComponent<Collider>();
		}
		if (!this.trigger)
		{
			this.trigger = base.gameObject.AddComponent<BoxCollider>();
		}
		this.trigger.isTrigger = true;
		BoxCollider boxCollider = this.trigger as BoxCollider;
		if (boxCollider)
		{
			boxCollider.center = this.Center;
			boxCollider.size = this.Size;
		}
	}

	// Token: 0x04001FE0 RID: 8160
	[InspectorFlags]
	public EnvironmentType Type = EnvironmentType.Underground;

	// Token: 0x04001FE1 RID: 8161
	public Vector3 Center = Vector3.zero;

	// Token: 0x04001FE2 RID: 8162
	public Vector3 Size = Vector3.one;
}
