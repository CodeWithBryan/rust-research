using System;
using UnityEngine;

// Token: 0x020002DF RID: 735
public class HitboxDefinition : MonoBehaviour
{
	// Token: 0x1700022A RID: 554
	// (get) Token: 0x06001D13 RID: 7443 RVA: 0x000C66E1 File Offset: 0x000C48E1
	// (set) Token: 0x06001D14 RID: 7444 RVA: 0x000C66E9 File Offset: 0x000C48E9
	public Vector3 Scale
	{
		get
		{
			return this.scale;
		}
		set
		{
			this.scale = new Vector3(Mathf.Abs(value.x), Mathf.Abs(value.y), Mathf.Abs(value.z));
		}
	}

	// Token: 0x1700022B RID: 555
	// (get) Token: 0x06001D15 RID: 7445 RVA: 0x000C6717 File Offset: 0x000C4917
	public Matrix4x4 LocalMatrix
	{
		get
		{
			return Matrix4x4.TRS(this.center, Quaternion.Euler(this.rotation), this.scale);
		}
	}

	// Token: 0x06001D16 RID: 7446 RVA: 0x000C6735 File Offset: 0x000C4935
	private void OnValidate()
	{
		this.Scale = this.Scale;
	}

	// Token: 0x06001D17 RID: 7447 RVA: 0x000C6744 File Offset: 0x000C4944
	protected virtual void OnDrawGizmosSelected()
	{
		HitboxDefinition.Type type = this.type;
		if (type == HitboxDefinition.Type.BOX)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.matrix *= Matrix4x4.TRS(this.center, Quaternion.Euler(this.rotation), this.scale);
			Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
			Gizmos.DrawCube(Vector3.zero, Vector3.one);
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
			Gizmos.color = Color.white;
			Gizmos.matrix = Matrix4x4.identity;
			return;
		}
		if (type != HitboxDefinition.Type.CAPSULE)
		{
			return;
		}
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.matrix *= Matrix4x4.TRS(this.center, Quaternion.Euler(this.rotation), Vector3.one);
		Gizmos.color = Color.green;
		GizmosUtil.DrawWireCapsuleY(Vector3.zero, this.scale.x, this.scale.y);
		Gizmos.color = Color.white;
		Gizmos.matrix = Matrix4x4.identity;
	}

	// Token: 0x06001D18 RID: 7448 RVA: 0x000C6874 File Offset: 0x000C4A74
	protected virtual void OnDrawGizmos()
	{
		HitboxDefinition.Type type = this.type;
		if (type == HitboxDefinition.Type.BOX)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.matrix *= Matrix4x4.TRS(this.center, Quaternion.Euler(this.rotation), this.scale);
			Gizmos.color = Color.black;
			Gizmos.DrawSphere(Vector3.zero, 0.005f);
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
			Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
			Gizmos.DrawCube(Vector3.zero, Vector3.one);
			Gizmos.color = Color.white;
			Gizmos.matrix = Matrix4x4.identity;
			return;
		}
		if (type != HitboxDefinition.Type.CAPSULE)
		{
			return;
		}
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.matrix *= Matrix4x4.TRS(this.center, Quaternion.Euler(this.rotation), Vector3.one);
		Gizmos.color = Color.black;
		Gizmos.DrawSphere(Vector3.zero, 0.005f);
		GizmosUtil.DrawWireCapsuleY(Vector3.zero, this.scale.x, this.scale.y);
		Gizmos.color = Color.white;
		Gizmos.matrix = Matrix4x4.identity;
	}

	// Token: 0x0400169B RID: 5787
	public Vector3 center;

	// Token: 0x0400169C RID: 5788
	public Vector3 rotation;

	// Token: 0x0400169D RID: 5789
	public HitboxDefinition.Type type;

	// Token: 0x0400169E RID: 5790
	public int priority;

	// Token: 0x0400169F RID: 5791
	public PhysicMaterial physicMaterial;

	// Token: 0x040016A0 RID: 5792
	[SerializeField]
	private Vector3 scale = Vector3.one;

	// Token: 0x02000C4D RID: 3149
	public enum Type
	{
		// Token: 0x040041B3 RID: 16819
		BOX,
		// Token: 0x040041B4 RID: 16820
		CAPSULE
	}
}
