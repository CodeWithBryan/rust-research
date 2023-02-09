using System;
using UnityEngine;

// Token: 0x020004D3 RID: 1235
public class DestroyOnGroundMissing : MonoBehaviour, IServerComponent
{
	// Token: 0x06002793 RID: 10131 RVA: 0x000F30C0 File Offset: 0x000F12C0
	private void OnGroundMissing()
	{
		BaseEntity baseEntity = base.gameObject.ToBaseEntity();
		if (baseEntity != null)
		{
			BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
			if (baseCombatEntity != null)
			{
				baseCombatEntity.Die(null);
				return;
			}
			baseEntity.Kill(BaseNetworkable.DestroyMode.Gib);
		}
	}
}
