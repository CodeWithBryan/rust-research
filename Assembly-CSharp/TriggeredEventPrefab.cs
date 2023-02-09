using System;
using UnityEngine;

// Token: 0x020004BE RID: 1214
public class TriggeredEventPrefab : TriggeredEvent
{
	// Token: 0x06002708 RID: 9992 RVA: 0x000F0F3C File Offset: 0x000EF13C
	private void RunEvent()
	{
		Debug.Log("[event] " + this.targetPrefab.resourcePath);
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.targetPrefab.resourcePath, default(Vector3), default(Quaternion), true);
		if (baseEntity)
		{
			baseEntity.SendMessage("TriggeredEventSpawn", SendMessageOptions.DontRequireReceiver);
			baseEntity.Spawn();
			if (this.shouldBroadcastSpawn)
			{
				foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
				{
					if (basePlayer && basePlayer.IsConnected)
					{
						basePlayer.ShowToast(GameTip.Styles.Server_Event, this.spawnPhrase, Array.Empty<string>());
					}
				}
			}
		}
	}

	// Token: 0x04001F77 RID: 8055
	public GameObjectRef targetPrefab;

	// Token: 0x04001F78 RID: 8056
	public bool shouldBroadcastSpawn;

	// Token: 0x04001F79 RID: 8057
	public Translate.Phrase spawnPhrase;
}
