using System;
using ConVar;
using UnityEngine;

// Token: 0x0200056F RID: 1391
public class TriggerTemperature : TriggerBase
{
	// Token: 0x06002A16 RID: 10774 RVA: 0x000FE81C File Offset: 0x000FCA1C
	private void OnValidate()
	{
		if (base.GetComponent<SphereCollider>() != null)
		{
			this.triggerSize = base.GetComponent<SphereCollider>().radius * base.transform.localScale.y;
			return;
		}
		Vector3 v = Vector3.Scale(base.GetComponent<BoxCollider>().size, base.transform.localScale);
		this.triggerSize = v.Max() * 0.5f;
	}

	// Token: 0x06002A17 RID: 10775 RVA: 0x000FE888 File Offset: 0x000FCA88
	public float WorkoutTemperature(Vector3 position, float oldTemperature)
	{
		if (this.sunlightBlocker)
		{
			float time = Env.time;
			if (time >= this.blockMinHour && time <= this.blockMaxHour)
			{
				Vector3 position2 = TOD_Sky.Instance.Components.SunTransform.position;
				if (!GamePhysics.LineOfSight(position, position2, 256, null))
				{
					return oldTemperature - this.sunlightBlockAmount;
				}
			}
			return oldTemperature;
		}
		float value = Vector3.Distance(base.gameObject.transform.position, position);
		float t = Mathf.InverseLerp(this.triggerSize, this.minSize, value);
		return Mathf.Lerp(oldTemperature, this.Temperature, t);
	}

	// Token: 0x06002A18 RID: 10776 RVA: 0x000FE91C File Offset: 0x000FCB1C
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

	// Token: 0x040021FA RID: 8698
	public float Temperature = 50f;

	// Token: 0x040021FB RID: 8699
	public float triggerSize;

	// Token: 0x040021FC RID: 8700
	public float minSize;

	// Token: 0x040021FD RID: 8701
	public bool sunlightBlocker;

	// Token: 0x040021FE RID: 8702
	public float sunlightBlockAmount;

	// Token: 0x040021FF RID: 8703
	[Range(0f, 24f)]
	public float blockMinHour = 8.5f;

	// Token: 0x04002200 RID: 8704
	[Range(0f, 24f)]
	public float blockMaxHour = 18.5f;
}
