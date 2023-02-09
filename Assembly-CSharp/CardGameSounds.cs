using System;
using UnityEngine;

// Token: 0x020003E9 RID: 1001
public class CardGameSounds : PrefabAttribute
{
	// Token: 0x060021D8 RID: 8664 RVA: 0x000D8DCF File Offset: 0x000D6FCF
	protected override Type GetIndexedType()
	{
		return typeof(CardGameSounds);
	}

	// Token: 0x060021D9 RID: 8665 RVA: 0x000D8DDC File Offset: 0x000D6FDC
	public void PlaySound(CardGameSounds.SoundType sound, GameObject forGameObject)
	{
		switch (sound)
		{
		case CardGameSounds.SoundType.Chips:
			this.ChipsSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Draw:
			this.DrawSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Play:
			this.PlaySfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Shuffle:
			this.ShuffleSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Win:
			this.WinSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.YourTurn:
			this.YourTurnSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Check:
			this.CheckSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Hit:
			this.HitSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Stand:
			this.StandSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Bet:
			this.BetSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.IncreaseBet:
			this.IncreaseBetSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.DecreaseBet:
			this.DecreaseBetSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.AllIn:
			this.AllInSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.UIInteract:
			this.UIInteractSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.DealerCool:
			this.DealerCoolSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.DealerHappy:
			this.DealerHappySfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.DealerLove:
			this.DealerLoveSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.DealerSad:
			this.DealerSadSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.DealerShocked:
			this.DealerShockedSfx.Play(forGameObject);
			return;
		default:
			throw new ArgumentOutOfRangeException("sound", sound, null);
		}
	}

	// Token: 0x04001A2B RID: 6699
	public SoundDefinition ChipsSfx;

	// Token: 0x04001A2C RID: 6700
	public SoundDefinition DrawSfx;

	// Token: 0x04001A2D RID: 6701
	public SoundDefinition PlaySfx;

	// Token: 0x04001A2E RID: 6702
	public SoundDefinition ShuffleSfx;

	// Token: 0x04001A2F RID: 6703
	public SoundDefinition WinSfx;

	// Token: 0x04001A30 RID: 6704
	public SoundDefinition LoseSfx;

	// Token: 0x04001A31 RID: 6705
	public SoundDefinition YourTurnSfx;

	// Token: 0x04001A32 RID: 6706
	public SoundDefinition CheckSfx;

	// Token: 0x04001A33 RID: 6707
	public SoundDefinition HitSfx;

	// Token: 0x04001A34 RID: 6708
	public SoundDefinition StandSfx;

	// Token: 0x04001A35 RID: 6709
	public SoundDefinition BetSfx;

	// Token: 0x04001A36 RID: 6710
	public SoundDefinition IncreaseBetSfx;

	// Token: 0x04001A37 RID: 6711
	public SoundDefinition DecreaseBetSfx;

	// Token: 0x04001A38 RID: 6712
	public SoundDefinition AllInSfx;

	// Token: 0x04001A39 RID: 6713
	public SoundDefinition UIInteractSfx;

	// Token: 0x04001A3A RID: 6714
	[Header("Dealer Reactions")]
	public SoundDefinition DealerCoolSfx;

	// Token: 0x04001A3B RID: 6715
	public SoundDefinition DealerHappySfx;

	// Token: 0x04001A3C RID: 6716
	public SoundDefinition DealerLoveSfx;

	// Token: 0x04001A3D RID: 6717
	public SoundDefinition DealerSadSfx;

	// Token: 0x04001A3E RID: 6718
	public SoundDefinition DealerShockedSfx;

	// Token: 0x02000C84 RID: 3204
	public enum SoundType
	{
		// Token: 0x040042A4 RID: 17060
		Chips,
		// Token: 0x040042A5 RID: 17061
		Draw,
		// Token: 0x040042A6 RID: 17062
		Play,
		// Token: 0x040042A7 RID: 17063
		Shuffle,
		// Token: 0x040042A8 RID: 17064
		Win,
		// Token: 0x040042A9 RID: 17065
		YourTurn,
		// Token: 0x040042AA RID: 17066
		Check,
		// Token: 0x040042AB RID: 17067
		Hit,
		// Token: 0x040042AC RID: 17068
		Stand,
		// Token: 0x040042AD RID: 17069
		Bet,
		// Token: 0x040042AE RID: 17070
		IncreaseBet,
		// Token: 0x040042AF RID: 17071
		DecreaseBet,
		// Token: 0x040042B0 RID: 17072
		AllIn,
		// Token: 0x040042B1 RID: 17073
		UIInteract,
		// Token: 0x040042B2 RID: 17074
		DealerCool,
		// Token: 0x040042B3 RID: 17075
		DealerHappy,
		// Token: 0x040042B4 RID: 17076
		DealerLove,
		// Token: 0x040042B5 RID: 17077
		DealerSad,
		// Token: 0x040042B6 RID: 17078
		DealerShocked
	}
}
