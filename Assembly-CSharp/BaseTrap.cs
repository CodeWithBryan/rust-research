using System;
using UnityEngine;

// Token: 0x0200043E RID: 1086
public class BaseTrap : DecayEntity
{
	// Token: 0x060023BE RID: 9150 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void ObjectEntered(GameObject obj)
	{
	}

	// Token: 0x060023BF RID: 9151 RVA: 0x0006CE35 File Offset: 0x0006B035
	public virtual void Arm()
	{
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060023C0 RID: 9152 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnEmpty()
	{
	}
}
