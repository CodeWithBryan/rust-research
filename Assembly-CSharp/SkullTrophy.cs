using System;
using Facepunch;
using ntw.CurvedTextMeshPro;
using ProtoBuf;
using Rust.UI;
using UnityEngine;

// Token: 0x0200015C RID: 348
public class SkullTrophy : StorageContainer
{
	// Token: 0x0600165F RID: 5727 RVA: 0x000AA210 File Offset: 0x000A8410
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001660 RID: 5728 RVA: 0x000AA224 File Offset: 0x000A8424
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (base.inventory != null && base.inventory.itemList.Count == 1)
		{
			info.msg.skullTrophy = Pool.Get<ProtoBuf.SkullTrophy>();
			info.msg.skullTrophy.playerName = base.inventory.itemList[0].name;
			return;
		}
		if (info.msg.skullTrophy != null)
		{
			info.msg.skullTrophy.playerName = string.Empty;
		}
	}

	// Token: 0x04000F41 RID: 3905
	public RustText NameText;

	// Token: 0x04000F42 RID: 3906
	public TextProOnACircle CircleModifier;

	// Token: 0x04000F43 RID: 3907
	public int AngleModifierMinCharCount = 3;

	// Token: 0x04000F44 RID: 3908
	public int AngleModifierMaxCharCount = 20;

	// Token: 0x04000F45 RID: 3909
	public int AngleModifierMinArcAngle = 20;

	// Token: 0x04000F46 RID: 3910
	public int AngleModifierMaxArcAngle = 45;

	// Token: 0x04000F47 RID: 3911
	public float SunsetTime = 18f;

	// Token: 0x04000F48 RID: 3912
	public float SunriseTime = 5f;

	// Token: 0x04000F49 RID: 3913
	public MeshRenderer[] SkullRenderers;

	// Token: 0x04000F4A RID: 3914
	public Material[] DaySkull;

	// Token: 0x04000F4B RID: 3915
	public Material[] NightSkull;

	// Token: 0x04000F4C RID: 3916
	public Material[] NoSkull;
}
