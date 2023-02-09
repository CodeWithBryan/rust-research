using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000793 RID: 1939
public class CardGamePlayerWidget : MonoBehaviour
{
	// Token: 0x04002AA4 RID: 10916
	[SerializeField]
	private GameObjectRef cardImageSmallPrefab;

	// Token: 0x04002AA5 RID: 10917
	[SerializeField]
	private RawImage avatar;

	// Token: 0x04002AA6 RID: 10918
	[SerializeField]
	private RustText playerName;

	// Token: 0x04002AA7 RID: 10919
	[SerializeField]
	private RustText scrapTotal;

	// Token: 0x04002AA8 RID: 10920
	[SerializeField]
	private RustText betText;

	// Token: 0x04002AA9 RID: 10921
	[SerializeField]
	private Image background;

	// Token: 0x04002AAA RID: 10922
	[SerializeField]
	private Color inactiveBackground;

	// Token: 0x04002AAB RID: 10923
	[SerializeField]
	private Color activeBackground;

	// Token: 0x04002AAC RID: 10924
	[SerializeField]
	private Color foldedBackground;

	// Token: 0x04002AAD RID: 10925
	[SerializeField]
	private Color winnerBackground;

	// Token: 0x04002AAE RID: 10926
	[SerializeField]
	private Animation actionShowAnimation;

	// Token: 0x04002AAF RID: 10927
	[SerializeField]
	private RustText actionText;

	// Token: 0x04002AB0 RID: 10928
	[SerializeField]
	private Sprite canSeeIcon;

	// Token: 0x04002AB1 RID: 10929
	[SerializeField]
	private Sprite cannotSeeIcon;

	// Token: 0x04002AB2 RID: 10930
	[SerializeField]
	private Sprite blankSprite;

	// Token: 0x04002AB3 RID: 10931
	[SerializeField]
	private Image cornerIcon;

	// Token: 0x04002AB4 RID: 10932
	[SerializeField]
	private Transform cardDisplayParent;

	// Token: 0x04002AB5 RID: 10933
	[SerializeField]
	private GridLayoutGroup cardDisplayGridLayout;

	// Token: 0x04002AB6 RID: 10934
	[SerializeField]
	private GameObject circle;

	// Token: 0x04002AB7 RID: 10935
	[SerializeField]
	private RustText circleText;
}
