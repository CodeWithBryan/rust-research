using System;
using UnityEngine;

// Token: 0x0200045D RID: 1117
public class CH47ReinforcementListener : BaseEntity
{
	// Token: 0x060024BD RID: 9405 RVA: 0x000E7ED0 File Offset: 0x000E60D0
	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		if (msg == this.listenString)
		{
			this.Call();
		}
	}

	// Token: 0x060024BE RID: 9406 RVA: 0x000E7EE8 File Offset: 0x000E60E8
	public void Call()
	{
		CH47HelicopterAIController component = GameManager.server.CreateEntity(this.heliPrefab.resourcePath, default(Vector3), default(Quaternion), true).GetComponent<CH47HelicopterAIController>();
		if (component)
		{
			Vector3 size = TerrainMeta.Size;
			CH47LandingZone closest = CH47LandingZone.GetClosest(base.transform.position);
			Vector3 zero = Vector3.zero;
			zero.y = closest.transform.position.y;
			Vector3 a = Vector3Ex.Direction2D(closest.transform.position, zero);
			Vector3 position = closest.transform.position + a * this.startDist;
			position.y = closest.transform.position.y;
			component.transform.position = position;
			component.SetLandingTarget(closest.transform.position);
			component.Spawn();
		}
	}

	// Token: 0x04001D46 RID: 7494
	public string listenString;

	// Token: 0x04001D47 RID: 7495
	public GameObjectRef heliPrefab;

	// Token: 0x04001D48 RID: 7496
	public float startDist = 300f;
}
