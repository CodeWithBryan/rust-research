using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000167 RID: 359
public class UIMarketTerminal : UIDialog, IVendingMachineInterface
{
	// Token: 0x04000F7C RID: 3964
	public static readonly Translate.Phrase PendingDeliveryPluralPhrase = new Translate.Phrase("market.pending_delivery.plural", "Waiting for {n} deliveries...");

	// Token: 0x04000F7D RID: 3965
	public static readonly Translate.Phrase PendingDeliverySingularPhrase = new Translate.Phrase("market.pending_delivery.singular", "Waiting for delivery...");

	// Token: 0x04000F7E RID: 3966
	public Canvas canvas;

	// Token: 0x04000F7F RID: 3967
	public MapView mapView;

	// Token: 0x04000F80 RID: 3968
	public RectTransform shopDetailsPanel;

	// Token: 0x04000F81 RID: 3969
	public float shopDetailsMargin = 16f;

	// Token: 0x04000F82 RID: 3970
	public float easeDuration = 0.2f;

	// Token: 0x04000F83 RID: 3971
	public LeanTweenType easeType = LeanTweenType.linear;

	// Token: 0x04000F84 RID: 3972
	public RustText shopName;

	// Token: 0x04000F85 RID: 3973
	public GameObject shopOrderingPanel;

	// Token: 0x04000F86 RID: 3974
	public RectTransform sellOrderContainer;

	// Token: 0x04000F87 RID: 3975
	public GameObjectRef sellOrderPrefab;

	// Token: 0x04000F88 RID: 3976
	public VirtualItemIcon deliveryFeeIcon;

	// Token: 0x04000F89 RID: 3977
	public GameObject deliveryFeeCantAffordIndicator;

	// Token: 0x04000F8A RID: 3978
	public GameObject inventoryFullIndicator;

	// Token: 0x04000F8B RID: 3979
	public GameObject notEligiblePanel;

	// Token: 0x04000F8C RID: 3980
	public GameObject pendingDeliveryPanel;

	// Token: 0x04000F8D RID: 3981
	public RustText pendingDeliveryLabel;

	// Token: 0x04000F8E RID: 3982
	public RectTransform itemNoticesContainer;

	// Token: 0x04000F8F RID: 3983
	public GameObjectRef itemRemovedPrefab;

	// Token: 0x04000F90 RID: 3984
	public GameObjectRef itemPendingPrefab;

	// Token: 0x04000F91 RID: 3985
	public GameObjectRef itemAddedPrefab;

	// Token: 0x04000F92 RID: 3986
	public CanvasGroup gettingStartedTip;

	// Token: 0x04000F93 RID: 3987
	public SoundDefinition buyItemSoundDef;

	// Token: 0x04000F94 RID: 3988
	public SoundDefinition buttonPressSoundDef;
}
