using System;
using UnityEngine;

// Token: 0x02000401 RID: 1025
public class StaticRespawnArea : SleepingBag
{
	// Token: 0x06002285 RID: 8837 RVA: 0x000DD08D File Offset: 0x000DB28D
	public override bool ValidForPlayer(ulong playerID, bool ignoreTimers)
	{
		return ignoreTimers || this.allowHostileSpawns || BasePlayer.FindByID(playerID).GetHostileDuration() <= 0f;
	}

	// Token: 0x06002286 RID: 8838 RVA: 0x000DD0B4 File Offset: 0x000DB2B4
	public override void GetSpawnPos(out Vector3 pos, out Quaternion rot)
	{
		Transform transform = this.spawnAreas[UnityEngine.Random.Range(0, this.spawnAreas.Length)];
		pos = transform.transform.position + this.spawnOffset;
		rot = Quaternion.Euler(0f, transform.transform.rotation.eulerAngles.y, 0f);
	}

	// Token: 0x06002287 RID: 8839 RVA: 0x000DD120 File Offset: 0x000DB320
	public override void SetUnlockTime(float newTime)
	{
		this.unlockTime = 0f;
	}

	// Token: 0x06002288 RID: 8840 RVA: 0x000DD130 File Offset: 0x000DB330
	public override float GetUnlockSeconds(ulong playerID)
	{
		BasePlayer basePlayer = BasePlayer.FindByID(playerID);
		if (basePlayer == null || this.allowHostileSpawns)
		{
			return base.unlockSeconds;
		}
		return Mathf.Max(basePlayer.GetHostileDuration(), base.unlockSeconds);
	}

	// Token: 0x04001B03 RID: 6915
	public Transform[] spawnAreas;

	// Token: 0x04001B04 RID: 6916
	public bool allowHostileSpawns;
}
