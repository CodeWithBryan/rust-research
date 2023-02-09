using System;
using UnityEngine;

// Token: 0x02000416 RID: 1046
public class LightListener : BaseEntity
{
	// Token: 0x060022FB RID: 8955 RVA: 0x000DEE81 File Offset: 0x000DD081
	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		base.OnEntityMessage(from, msg);
		if (msg == this.onMessage)
		{
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			return;
		}
		if (msg == this.offMessage)
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
		}
	}

	// Token: 0x04001B62 RID: 7010
	public string onMessage = "";

	// Token: 0x04001B63 RID: 7011
	public string offMessage = "";

	// Token: 0x04001B64 RID: 7012
	[Tooltip("Must be part of this prefab")]
	public LightGroupAtTime onLights;

	// Token: 0x04001B65 RID: 7013
	[Tooltip("Must be part of this prefab")]
	public LightGroupAtTime offLights;
}
