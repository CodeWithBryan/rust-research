using System;
using Facepunch.CardGames;
using UnityEngine;

// Token: 0x020003E7 RID: 999
public class BlackjackMachine : BaseCardGameEntity
{
	// Token: 0x1700029B RID: 667
	// (get) Token: 0x060021CA RID: 8650 RVA: 0x000D8C8C File Offset: 0x000D6E8C
	// (set) Token: 0x060021CB RID: 8651 RVA: 0x000D8C93 File Offset: 0x000D6E93
	[ServerVar(Help = "Maximum initial bet per round")]
	public static int maxbet
	{
		get
		{
			return BlackjackMachine._maxbet;
		}
		set
		{
			BlackjackMachine._maxbet = Mathf.Clamp(value, 25, 1000000);
		}
	}

	// Token: 0x1700029C RID: 668
	// (get) Token: 0x060021CC RID: 8652 RVA: 0x000062DD File Offset: 0x000044DD
	protected override float MaxStorageInteractionDist
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x060021CD RID: 8653 RVA: 0x000D8CA7 File Offset: 0x000D6EA7
	public override void InitShared()
	{
		base.InitShared();
		this.controller = (BlackjackController)base.GameController;
	}

	// Token: 0x060021CE RID: 8654 RVA: 0x000D8CC0 File Offset: 0x000D6EC0
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	// Token: 0x060021CF RID: 8655 RVA: 0x000D8CC9 File Offset: 0x000D6EC9
	public override void PlayerStorageChanged()
	{
		base.PlayerStorageChanged();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x04001A22 RID: 6690
	[Header("Blackjack Machine")]
	[SerializeField]
	private GameObjectRef mainScreenPrefab;

	// Token: 0x04001A23 RID: 6691
	[SerializeField]
	private GameObjectRef smallScreenPrefab;

	// Token: 0x04001A24 RID: 6692
	[SerializeField]
	private Transform mainScreenParent;

	// Token: 0x04001A25 RID: 6693
	[SerializeField]
	private Transform[] smallScreenParents;

	// Token: 0x04001A26 RID: 6694
	private static int _maxbet = 500;

	// Token: 0x04001A27 RID: 6695
	private BlackjackController controller;

	// Token: 0x04001A28 RID: 6696
	private BlackjackMainScreenUI mainScreenUI;

	// Token: 0x04001A29 RID: 6697
	private BlackjackSmallScreenUI[] smallScreenUIs = new BlackjackSmallScreenUI[3];
}
