using System;
using Network;

// Token: 0x02000018 RID: 24
public class RepeatingFirework : BaseFirework
{
	// Token: 0x0600004F RID: 79 RVA: 0x00003501 File Offset: 0x00001701
	public override void Begin()
	{
		base.Begin();
		base.InvokeRepeating(new Action(this.SendFire), 0f, this.timeBetweenRepeats);
		base.CancelInvoke(new Action(this.OnExhausted));
	}

	// Token: 0x06000050 RID: 80 RVA: 0x0000353C File Offset: 0x0000173C
	public void SendFire()
	{
		base.ClientRPC(null, "RPCFire");
		this.numFired++;
		if (this.numFired >= this.maxRepeats)
		{
			base.CancelInvoke(new Action(this.SendFire));
			this.numFired = 0;
			this.OnExhausted();
		}
	}

	// Token: 0x06000051 RID: 81 RVA: 0x00003590 File Offset: 0x00001790
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RepeatingFirework.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x04000058 RID: 88
	public float timeBetweenRepeats = 1f;

	// Token: 0x04000059 RID: 89
	public int maxRepeats = 12;

	// Token: 0x0400005A RID: 90
	public SoundPlayer launchSound;

	// Token: 0x0400005B RID: 91
	private int numFired;
}
