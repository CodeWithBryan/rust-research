using System;
using Network;
using UnityEngine;

// Token: 0x02000048 RID: 72
public class BigWheelBettingTerminal : StorageContainer
{
	// Token: 0x0600081D RID: 2077 RVA: 0x0004E1D4 File Offset: 0x0004C3D4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BigWheelBettingTerminal.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x0004E214 File Offset: 0x0004C414
	public new void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(base.transform.TransformPoint(this.seatedPlayerOffset), this.offsetCheckRadius);
		base.OnDrawGizmos();
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x0004E244 File Offset: 0x0004C444
	public bool IsPlayerValid(BasePlayer player)
	{
		if (!player.isMounted || !(player.GetMounted() is BaseChair))
		{
			return false;
		}
		Vector3 b = base.transform.TransformPoint(this.seatedPlayerOffset);
		return Vector3Ex.Distance2D(player.transform.position, b) <= this.offsetCheckRadius;
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x0004E296 File Offset: 0x0004C496
	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		return this.IsPlayerValid(player) && base.PlayerOpenLoot(player, panelToOpen, true);
	}

	// Token: 0x06000821 RID: 2081 RVA: 0x0004E2AC File Offset: 0x0004C4AC
	public bool TrySetBigWheel(BigWheelGame newWheel)
	{
		if (base.isClient)
		{
			return false;
		}
		if (this.bigWheel != null && this.bigWheel != newWheel)
		{
			float num = Vector3.SqrMagnitude(this.bigWheel.transform.position - base.transform.position);
			if (Vector3.SqrMagnitude(newWheel.transform.position - base.transform.position) >= num)
			{
				return false;
			}
			this.bigWheel.RemoveTerminal(this);
		}
		this.bigWheel = newWheel;
		return true;
	}

	// Token: 0x0400055A RID: 1370
	public BigWheelGame bigWheel;

	// Token: 0x0400055B RID: 1371
	public Vector3 seatedPlayerOffset = Vector3.forward;

	// Token: 0x0400055C RID: 1372
	public float offsetCheckRadius = 0.4f;

	// Token: 0x0400055D RID: 1373
	public SoundDefinition winSound;

	// Token: 0x0400055E RID: 1374
	public SoundDefinition loseSound;
}
