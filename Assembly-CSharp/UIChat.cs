using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200076B RID: 1899
public class UIChat : PriorityListComponent<UIChat>
{
	// Token: 0x040029CD RID: 10701
	public GameObject inputArea;

	// Token: 0x040029CE RID: 10702
	public GameObject chatArea;

	// Token: 0x040029CF RID: 10703
	public TMP_InputField inputField;

	// Token: 0x040029D0 RID: 10704
	public TextMeshProUGUI channelLabel;

	// Token: 0x040029D1 RID: 10705
	public ScrollRect scrollRect;

	// Token: 0x040029D2 RID: 10706
	public CanvasGroup canvasGroup;

	// Token: 0x040029D3 RID: 10707
	public GameObjectRef chatItemPlayer;

	// Token: 0x040029D4 RID: 10708
	public GameObject userPopup;

	// Token: 0x040029D5 RID: 10709
	public static bool isOpen;
}
