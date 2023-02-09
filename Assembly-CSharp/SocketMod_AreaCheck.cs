using System;
using UnityEngine;

// Token: 0x02000258 RID: 600
public class SocketMod_AreaCheck : SocketMod
{
	// Token: 0x06001B79 RID: 7033 RVA: 0x000BF56C File Offset: 0x000BD76C
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		bool flag = true;
		if (!this.wantsInside)
		{
			flag = !flag;
		}
		Gizmos.color = (flag ? Color.green.WithAlpha(0.5f) : Color.red.WithAlpha(0.5f));
		Gizmos.DrawCube(this.bounds.center, this.bounds.size);
	}

	// Token: 0x06001B7A RID: 7034 RVA: 0x000BF5DB File Offset: 0x000BD7DB
	public static bool IsInArea(Vector3 position, Quaternion rotation, Bounds bounds, LayerMask layerMask, BaseEntity entity = null)
	{
		return GamePhysics.CheckOBBAndEntity(new OBB(position, rotation, bounds), layerMask.value, QueryTriggerInteraction.UseGlobal, entity);
	}

	// Token: 0x06001B7B RID: 7035 RVA: 0x000BF5F4 File Offset: 0x000BD7F4
	public bool DoCheck(Vector3 position, Quaternion rotation, BaseEntity entity = null)
	{
		Vector3 position2 = position + rotation * this.worldPosition;
		Quaternion rotation2 = rotation * this.worldRotation;
		return SocketMod_AreaCheck.IsInArea(position2, rotation2, this.bounds, this.layerMask, entity) == this.wantsInside;
	}

	// Token: 0x06001B7C RID: 7036 RVA: 0x000BF63B File Offset: 0x000BD83B
	public override bool DoCheck(Construction.Placement place)
	{
		if (this.DoCheck(place.position, place.rotation, null))
		{
			return true;
		}
		Construction.lastPlacementError = "Failed Check: IsInArea (" + this.hierachyName + ")";
		return false;
	}

	// Token: 0x04001482 RID: 5250
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 0.1f);

	// Token: 0x04001483 RID: 5251
	public LayerMask layerMask;

	// Token: 0x04001484 RID: 5252
	public bool wantsInside = true;
}
