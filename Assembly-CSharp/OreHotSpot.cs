using System;

// Token: 0x0200016C RID: 364
public class OreHotSpot : BaseCombatEntity, ILOD
{
	// Token: 0x06001693 RID: 5779 RVA: 0x000AB5D1 File Offset: 0x000A97D1
	public void OreOwner(OreResourceEntity newOwner)
	{
		this.owner = newOwner;
	}

	// Token: 0x06001694 RID: 5780 RVA: 0x00044673 File Offset: 0x00042873
	public override void ServerInit()
	{
		base.ServerInit();
	}

	// Token: 0x06001695 RID: 5781 RVA: 0x000AB5DA File Offset: 0x000A97DA
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if (base.isClient)
		{
			return;
		}
		if (this.owner)
		{
			this.owner.OnAttacked(info);
		}
	}

	// Token: 0x06001696 RID: 5782 RVA: 0x000AB605 File Offset: 0x000A9805
	public override void OnKilled(HitInfo info)
	{
		this.FireFinishEffect();
		base.OnKilled(info);
	}

	// Token: 0x06001697 RID: 5783 RVA: 0x000AB614 File Offset: 0x000A9814
	public void FireFinishEffect()
	{
		if (this.finishEffect.isValid)
		{
			Effect.server.Run(this.finishEffect.resourcePath, base.transform.position, base.transform.forward, null, false);
		}
	}

	// Token: 0x04000FA5 RID: 4005
	public float visualDistance = 20f;

	// Token: 0x04000FA6 RID: 4006
	public GameObjectRef visualEffect;

	// Token: 0x04000FA7 RID: 4007
	public GameObjectRef finishEffect;

	// Token: 0x04000FA8 RID: 4008
	public GameObjectRef damageEffect;

	// Token: 0x04000FA9 RID: 4009
	public OreResourceEntity owner;
}
