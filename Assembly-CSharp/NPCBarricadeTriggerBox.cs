using System;
using ConVar;
using UnityEngine;

// Token: 0x020001F5 RID: 501
public class NPCBarricadeTriggerBox : MonoBehaviour
{
	// Token: 0x06001A2C RID: 6700 RVA: 0x000BA580 File Offset: 0x000B8780
	public void Setup(Barricade t)
	{
		this.target = t;
		base.transform.SetParent(this.target.transform, false);
		base.gameObject.layer = 18;
		BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
		boxCollider.isTrigger = true;
		boxCollider.center = Vector3.zero;
		boxCollider.size = Vector3.one * AI.npc_door_trigger_size + Vector3.right * this.target.bounds.size.x;
	}

	// Token: 0x06001A2D RID: 6701 RVA: 0x000BA610 File Offset: 0x000B8810
	private void OnTriggerEnter(Collider other)
	{
		if (this.target == null || this.target.isClient)
		{
			return;
		}
		if (NPCBarricadeTriggerBox.playerServerLayer < 0)
		{
			NPCBarricadeTriggerBox.playerServerLayer = LayerMask.NameToLayer("Player (Server)");
		}
		if ((other.gameObject.layer & NPCBarricadeTriggerBox.playerServerLayer) > 0)
		{
			BasePlayer component = other.gameObject.GetComponent<BasePlayer>();
			if (component != null && component.IsNpc && !(component is BasePet))
			{
				this.target.Kill(BaseNetworkable.DestroyMode.Gib);
			}
		}
	}

	// Token: 0x0400128C RID: 4748
	private Barricade target;

	// Token: 0x0400128D RID: 4749
	private static int playerServerLayer = -1;
}
