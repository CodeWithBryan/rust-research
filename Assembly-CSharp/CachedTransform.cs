using System;
using UnityEngine;

// Token: 0x020008C3 RID: 2243
public struct CachedTransform<T> where T : Component
{
	// Token: 0x0600361C RID: 13852 RVA: 0x001432F8 File Offset: 0x001414F8
	public CachedTransform(T instance)
	{
		this.component = instance;
		if (this.component)
		{
			this.position = this.component.transform.position;
			this.rotation = this.component.transform.rotation;
			this.localScale = this.component.transform.localScale;
			return;
		}
		this.position = Vector3.zero;
		this.rotation = Quaternion.identity;
		this.localScale = Vector3.one;
	}

	// Token: 0x0600361D RID: 13853 RVA: 0x00143394 File Offset: 0x00141594
	public void Apply()
	{
		if (this.component)
		{
			this.component.transform.SetPositionAndRotation(this.position, this.rotation);
			this.component.transform.localScale = this.localScale;
		}
	}

	// Token: 0x0600361E RID: 13854 RVA: 0x001433F0 File Offset: 0x001415F0
	public void RotateAround(Vector3 center, Vector3 axis, float angle)
	{
		Quaternion rhs = Quaternion.AngleAxis(angle, axis);
		Vector3 b = rhs * (this.position - center);
		this.position = center + b;
		this.rotation *= Quaternion.Inverse(this.rotation) * rhs * this.rotation;
	}

	// Token: 0x17000413 RID: 1043
	// (get) Token: 0x0600361F RID: 13855 RVA: 0x00143452 File Offset: 0x00141652
	public Matrix4x4 localToWorldMatrix
	{
		get
		{
			return Matrix4x4.TRS(this.position, this.rotation, this.localScale);
		}
	}

	// Token: 0x17000414 RID: 1044
	// (get) Token: 0x06003620 RID: 13856 RVA: 0x0014346C File Offset: 0x0014166C
	public Matrix4x4 worldToLocalMatrix
	{
		get
		{
			return this.localToWorldMatrix.inverse;
		}
	}

	// Token: 0x17000415 RID: 1045
	// (get) Token: 0x06003621 RID: 13857 RVA: 0x00143487 File Offset: 0x00141687
	public Vector3 forward
	{
		get
		{
			return this.rotation * Vector3.forward;
		}
	}

	// Token: 0x17000416 RID: 1046
	// (get) Token: 0x06003622 RID: 13858 RVA: 0x00143499 File Offset: 0x00141699
	public Vector3 up
	{
		get
		{
			return this.rotation * Vector3.up;
		}
	}

	// Token: 0x17000417 RID: 1047
	// (get) Token: 0x06003623 RID: 13859 RVA: 0x001434AB File Offset: 0x001416AB
	public Vector3 right
	{
		get
		{
			return this.rotation * Vector3.right;
		}
	}

	// Token: 0x06003624 RID: 13860 RVA: 0x001434BD File Offset: 0x001416BD
	public static implicit operator bool(CachedTransform<T> instance)
	{
		return instance.component != null;
	}

	// Token: 0x04003113 RID: 12563
	public T component;

	// Token: 0x04003114 RID: 12564
	public Vector3 position;

	// Token: 0x04003115 RID: 12565
	public Quaternion rotation;

	// Token: 0x04003116 RID: 12566
	public Vector3 localScale;
}
