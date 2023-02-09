using System;
using UnityEngine;

// Token: 0x02000101 RID: 257
public class DropBox : Mailbox
{
	// Token: 0x060014E7 RID: 5351 RVA: 0x000A4A47 File Offset: 0x000A2C47
	public override bool PlayerIsOwner(BasePlayer player)
	{
		return this.PlayerBehind(player);
	}

	// Token: 0x060014E8 RID: 5352 RVA: 0x000A4A50 File Offset: 0x000A2C50
	public bool PlayerBehind(BasePlayer player)
	{
		return Vector3.Dot(base.transform.forward, (player.transform.position - base.transform.position).normalized) <= -0.3f && GamePhysics.LineOfSight(player.eyes.position, this.EyePoint.position, 2162688, null);
	}

	// Token: 0x060014E9 RID: 5353 RVA: 0x000A4ABC File Offset: 0x000A2CBC
	public bool PlayerInfront(BasePlayer player)
	{
		return Vector3.Dot(base.transform.forward, (player.transform.position - base.transform.position).normalized) >= 0.7f;
	}

	// Token: 0x060014EA RID: 5354 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x04000D97 RID: 3479
	public Transform EyePoint;
}
