using System;
using UnityEngine;

// Token: 0x02000571 RID: 1393
public class TriggerWetness : TriggerBase
{
	// Token: 0x06002A23 RID: 10787 RVA: 0x000FEBF8 File Offset: 0x000FCDF8
	public float WorkoutWetness(Vector3 position)
	{
		if (this.ApplyLocalHeightCheck && base.transform.InverseTransformPoint(position).y < this.MinLocalHeight)
		{
			return 0f;
		}
		float num = Vector3Ex.Distance2D(this.OriginTransform.position, position) / this.TargetCollider.radius;
		num = Mathf.Clamp01(num);
		num = 1f - num;
		return Mathf.Lerp(0f, this.Wetness, num);
	}

	// Token: 0x06002A24 RID: 10788 RVA: 0x000FEC6C File Offset: 0x000FCE6C
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x04002204 RID: 8708
	public float Wetness = 0.25f;

	// Token: 0x04002205 RID: 8709
	public SphereCollider TargetCollider;

	// Token: 0x04002206 RID: 8710
	public Transform OriginTransform;

	// Token: 0x04002207 RID: 8711
	public bool ApplyLocalHeightCheck;

	// Token: 0x04002208 RID: 8712
	public float MinLocalHeight;
}
