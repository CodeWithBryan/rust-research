using System;
using UnityEngine;

// Token: 0x02000669 RID: 1641
public class TerrainCollisionTrigger : EnvironmentVolumeTrigger
{
	// Token: 0x06002E4F RID: 11855 RVA: 0x00115C1F File Offset: 0x00113E1F
	protected void OnTriggerEnter(Collider other)
	{
		if (!TerrainMeta.Collision || other.isTrigger)
		{
			return;
		}
		this.UpdateCollider(other, true);
	}

	// Token: 0x06002E50 RID: 11856 RVA: 0x00115C3E File Offset: 0x00113E3E
	protected void OnTriggerExit(Collider other)
	{
		if (!TerrainMeta.Collision || other.isTrigger)
		{
			return;
		}
		this.UpdateCollider(other, false);
	}

	// Token: 0x06002E51 RID: 11857 RVA: 0x00115C60 File Offset: 0x00113E60
	private void UpdateCollider(Collider other, bool state)
	{
		TerrainMeta.Collision.SetIgnore(other, base.volume.trigger, state);
		TerrainCollisionProxy component = other.GetComponent<TerrainCollisionProxy>();
		if (component)
		{
			for (int i = 0; i < component.colliders.Length; i++)
			{
				TerrainMeta.Collision.SetIgnore(component.colliders[i], base.volume.trigger, state);
			}
		}
	}
}
