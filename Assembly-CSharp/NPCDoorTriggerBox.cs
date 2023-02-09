using System;
using ConVar;
using UnityEngine;

// Token: 0x020001F6 RID: 502
public class NPCDoorTriggerBox : MonoBehaviour
{
	// Token: 0x06001A30 RID: 6704 RVA: 0x000BA6A0 File Offset: 0x000B88A0
	public void Setup(Door d)
	{
		this.door = d;
		base.transform.SetParent(this.door.transform, false);
		base.gameObject.layer = 18;
		BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
		boxCollider.isTrigger = true;
		boxCollider.center = Vector3.zero;
		boxCollider.size = Vector3.one * AI.npc_door_trigger_size;
	}

	// Token: 0x06001A31 RID: 6705 RVA: 0x000BA70C File Offset: 0x000B890C
	private void OnTriggerEnter(Collider other)
	{
		if (this.door == null || this.door.isClient || this.door.IsLocked())
		{
			return;
		}
		if (!this.door.isSecurityDoor && this.door.IsOpen())
		{
			return;
		}
		if (this.door.isSecurityDoor && !this.door.IsOpen())
		{
			return;
		}
		if (NPCDoorTriggerBox.playerServerLayer < 0)
		{
			NPCDoorTriggerBox.playerServerLayer = LayerMask.NameToLayer("Player (Server)");
		}
		if ((other.gameObject.layer & NPCDoorTriggerBox.playerServerLayer) > 0)
		{
			BasePlayer component = other.gameObject.GetComponent<BasePlayer>();
			if (component != null && component.IsNpc && !this.door.isSecurityDoor)
			{
				this.door.SetOpen(true, false);
			}
		}
	}

	// Token: 0x0400128E RID: 4750
	private Door door;

	// Token: 0x0400128F RID: 4751
	private static int playerServerLayer = -1;
}
