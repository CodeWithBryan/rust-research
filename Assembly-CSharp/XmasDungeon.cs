using System;
using UnityEngine;

// Token: 0x0200018A RID: 394
public class XmasDungeon : HalloweenDungeon
{
	// Token: 0x06001721 RID: 5921 RVA: 0x000ADA93 File Offset: 0x000ABC93
	public override float GetLifetime()
	{
		return XmasDungeon.xmaslifetime;
	}

	// Token: 0x06001722 RID: 5922 RVA: 0x000ADA9A File Offset: 0x000ABC9A
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.PlayerChecks), 1f, 1f);
	}

	// Token: 0x06001723 RID: 5923 RVA: 0x000ADAC0 File Offset: 0x000ABCC0
	public void PlayerChecks()
	{
		ProceduralDynamicDungeon proceduralDynamicDungeon = this.dungeonInstance.Get(true);
		if (proceduralDynamicDungeon == null)
		{
			return;
		}
		bool b = false;
		bool b2 = false;
		foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
		{
			float num = Vector3.Distance(basePlayer.transform.position, base.transform.position);
			float num2 = Vector3.Distance(basePlayer.transform.position, proceduralDynamicDungeon.GetExitPortal(true).transform.position);
			if (num < XmasDungeon.playerdetectrange)
			{
				b = true;
			}
			if (num2 < XmasDungeon.playerdetectrange * 2f)
			{
				b2 = true;
			}
		}
		base.SetFlag(BaseEntity.Flags.Reserved8, b2, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved7, b, false, true);
		proceduralDynamicDungeon.SetFlag(BaseEntity.Flags.Reserved7, b, false, true);
		proceduralDynamicDungeon.SetFlag(BaseEntity.Flags.Reserved8, b2, false, true);
	}

	// Token: 0x04001035 RID: 4149
	public const BaseEntity.Flags HasPlayerOutside = BaseEntity.Flags.Reserved7;

	// Token: 0x04001036 RID: 4150
	public const BaseEntity.Flags HasPlayerInside = BaseEntity.Flags.Reserved8;

	// Token: 0x04001037 RID: 4151
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float xmaspopulation = 0f;

	// Token: 0x04001038 RID: 4152
	[ServerVar(Help = "How long each active dungeon should last before dying", ShowInAdminUI = true)]
	public static float xmaslifetime = 1200f;

	// Token: 0x04001039 RID: 4153
	[ServerVar(Help = "How far we detect players from our inside/outside", ShowInAdminUI = true)]
	public static float playerdetectrange = 30f;
}
