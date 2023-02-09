using System;
using UnityEngine;

// Token: 0x02000423 RID: 1059
public class PlayerModelCinematicList : PrefabAttribute, IClientComponent
{
	// Token: 0x06002343 RID: 9027 RVA: 0x000DFF66 File Offset: 0x000DE166
	protected override Type GetIndexedType()
	{
		return typeof(PlayerModelCinematicList);
	}

	// Token: 0x06002344 RID: 9028 RVA: 0x000DFF72 File Offset: 0x000DE172
	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
	}

	// Token: 0x04001BF0 RID: 7152
	public PlayerModelCinematicList.PlayerModelCinematicAnimation[] Animations;

	// Token: 0x02000C93 RID: 3219
	[Serializable]
	public struct PlayerModelCinematicAnimation
	{
		// Token: 0x04004316 RID: 17174
		public string StateName;

		// Token: 0x04004317 RID: 17175
		public string ClipName;

		// Token: 0x04004318 RID: 17176
		public float Length;
	}
}
