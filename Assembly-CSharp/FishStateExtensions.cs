using System;

// Token: 0x0200038B RID: 907
public static class FishStateExtensions
{
	// Token: 0x06001FC4 RID: 8132 RVA: 0x000D177E File Offset: 0x000CF97E
	public static bool Contains(this BaseFishingRod.FishState state, BaseFishingRod.FishState check)
	{
		return (state & check) == check;
	}

	// Token: 0x06001FC5 RID: 8133 RVA: 0x000D1786 File Offset: 0x000CF986
	public static BaseFishingRod.FishState FlipHorizontal(this BaseFishingRod.FishState state)
	{
		if (state.Contains(BaseFishingRod.FishState.PullingLeft))
		{
			state |= BaseFishingRod.FishState.PullingRight;
			state &= ~BaseFishingRod.FishState.PullingLeft;
		}
		else if (state.Contains(BaseFishingRod.FishState.PullingRight))
		{
			state |= BaseFishingRod.FishState.PullingLeft;
			state &= ~BaseFishingRod.FishState.PullingRight;
		}
		return state;
	}
}
