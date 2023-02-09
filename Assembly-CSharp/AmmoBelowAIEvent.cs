using System;
using ProtoBuf;

// Token: 0x02000349 RID: 841
public class AmmoBelowAIEvent : BaseAIEvent
{
	// Token: 0x1700023E RID: 574
	// (get) Token: 0x06001E55 RID: 7765 RVA: 0x000CD698 File Offset: 0x000CB898
	// (set) Token: 0x06001E56 RID: 7766 RVA: 0x000CD6A0 File Offset: 0x000CB8A0
	public float Value { get; private set; }

	// Token: 0x06001E57 RID: 7767 RVA: 0x000CD6A9 File Offset: 0x000CB8A9
	public AmmoBelowAIEvent() : base(AIEventType.AmmoBelow)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Normal;
	}

	// Token: 0x06001E58 RID: 7768 RVA: 0x000CD6BC File Offset: 0x000CB8BC
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		AmmoBelowAIEventData ammoBelowData = data.ammoBelowData;
		this.Value = ammoBelowData.value;
	}

	// Token: 0x06001E59 RID: 7769 RVA: 0x000CD6E4 File Offset: 0x000CB8E4
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.ammoBelowData = new AmmoBelowAIEventData();
		aieventData.ammoBelowData.value = this.Value;
		return aieventData;
	}

	// Token: 0x06001E5A RID: 7770 RVA: 0x000CD708 File Offset: 0x000CB908
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		IAIAttack iaiattack = base.Owner as IAIAttack;
		if (iaiattack == null)
		{
			return;
		}
		bool flag = iaiattack.GetAmmoFraction() < this.Value;
		if (base.Inverted)
		{
			base.Result = !flag;
			return;
		}
		base.Result = flag;
	}
}
