using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003EA RID: 1002
public class CardTable : BaseCardGameEntity
{
	// Token: 0x060021DB RID: 8667 RVA: 0x000D8CC0 File Offset: 0x000D6EC0
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	// Token: 0x1700029D RID: 669
	// (get) Token: 0x060021DC RID: 8668 RVA: 0x000062DD File Offset: 0x000044DD
	protected override float MaxStorageInteractionDist
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x04001A3F RID: 6719
	[Header("Card Table")]
	[SerializeField]
	private ViewModel viewModel;

	// Token: 0x04001A40 RID: 6720
	[SerializeField]
	private CardGameUI.PlayingCardImage[] tableCards;

	// Token: 0x04001A41 RID: 6721
	[SerializeField]
	private Renderer[] tableCardBackings;

	// Token: 0x04001A42 RID: 6722
	[SerializeField]
	private Canvas cardUICanvas;

	// Token: 0x04001A43 RID: 6723
	[SerializeField]
	private Image[] tableCardImages;

	// Token: 0x04001A44 RID: 6724
	[SerializeField]
	private Sprite blankCard;

	// Token: 0x04001A45 RID: 6725
	[SerializeField]
	private CardTable.ChipStack[] chipStacks;

	// Token: 0x04001A46 RID: 6726
	[SerializeField]
	private CardTable.ChipStack[] fillerStacks;

	// Token: 0x02000C85 RID: 3205
	[Serializable]
	public class ChipStack : IComparable<CardTable.ChipStack>
	{
		// Token: 0x06004D03 RID: 19715 RVA: 0x00196DC5 File Offset: 0x00194FC5
		public int CompareTo(CardTable.ChipStack other)
		{
			if (other == null)
			{
				return 1;
			}
			return this.chipValue.CompareTo(other.chipValue);
		}

		// Token: 0x040042B7 RID: 17079
		public int chipValue;

		// Token: 0x040042B8 RID: 17080
		public GameObject[] chips;
	}
}
