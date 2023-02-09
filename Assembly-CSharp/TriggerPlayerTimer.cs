using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000569 RID: 1385
public class TriggerPlayerTimer : TriggerBase
{
	// Token: 0x060029F9 RID: 10745 RVA: 0x000FE220 File Offset: 0x000FC420
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj != null)
		{
			BaseEntity baseEntity = obj.ToBaseEntity();
			BasePlayer basePlayer;
			if ((basePlayer = (baseEntity as BasePlayer)) != null && baseEntity.isServer && !basePlayer.isMounted)
			{
				return baseEntity.gameObject;
			}
		}
		return obj;
	}

	// Token: 0x060029FA RID: 10746 RVA: 0x000FE268 File Offset: 0x000FC468
	internal override void OnObjects()
	{
		base.OnObjects();
		base.Invoke(new Action(this.DamageTarget), this.TimeToDamage);
	}

	// Token: 0x060029FB RID: 10747 RVA: 0x000FE288 File Offset: 0x000FC488
	internal override void OnEmpty()
	{
		base.OnEmpty();
		base.CancelInvoke(new Action(this.DamageTarget));
	}

	// Token: 0x060029FC RID: 10748 RVA: 0x000FE2A4 File Offset: 0x000FC4A4
	private void DamageTarget()
	{
		bool flag = false;
		using (HashSet<BaseEntity>.Enumerator enumerator = this.entityContents.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				BasePlayer basePlayer;
				if ((basePlayer = (enumerator.Current as BasePlayer)) != null && !basePlayer.isMounted)
				{
					flag = true;
				}
			}
		}
		if (flag && this.TargetEntity != null)
		{
			this.TargetEntity.OnAttacked(new HitInfo(null, this.TargetEntity, DamageType.Generic, this.DamageAmount));
		}
		base.Invoke(new Action(this.DamageTarget), this.TimeToDamage);
	}

	// Token: 0x040021EE RID: 8686
	public BaseEntity TargetEntity;

	// Token: 0x040021EF RID: 8687
	public float DamageAmount = 20f;

	// Token: 0x040021F0 RID: 8688
	public float TimeToDamage = 3f;
}
