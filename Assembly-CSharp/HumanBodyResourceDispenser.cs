using System;

// Token: 0x020003AC RID: 940
public class HumanBodyResourceDispenser : ResourceDispenser
{
	// Token: 0x0600205C RID: 8284 RVA: 0x000D3338 File Offset: 0x000D1538
	public override bool OverrideOwnership(Item item, AttackEntity weapon)
	{
		if (item.info.shortname == "skull.human")
		{
			PlayerCorpse component = base.GetComponent<PlayerCorpse>();
			if (component)
			{
				item.name = HumanBodyResourceDispenser.CreateSkullName(component.playerName);
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600205D RID: 8285 RVA: 0x000D337F File Offset: 0x000D157F
	public static string CreateSkullName(string playerName)
	{
		return "Skull of \"" + playerName + "\"";
	}
}
