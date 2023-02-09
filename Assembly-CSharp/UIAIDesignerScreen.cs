using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000763 RID: 1891
public class UIAIDesignerScreen : SingletonComponent<UIAIDesignerScreen>, IUIScreen
{
	// Token: 0x040029A3 RID: 10659
	public GameObject SaveEntityButton;

	// Token: 0x040029A4 RID: 10660
	public GameObject SaveServerButton;

	// Token: 0x040029A5 RID: 10661
	public GameObject SaveDefaultButton;

	// Token: 0x040029A6 RID: 10662
	public RustInput InputAIDescription;

	// Token: 0x040029A7 RID: 10663
	public RustText TextDefaultStateContainer;

	// Token: 0x040029A8 RID: 10664
	public Transform PrefabAddNewStateButton;

	// Token: 0x040029A9 RID: 10665
	public Transform StateContainer;

	// Token: 0x040029AA RID: 10666
	public Transform PrefabState;

	// Token: 0x040029AB RID: 10667
	public EnumListUI PopupList;

	// Token: 0x040029AC RID: 10668
	public static EnumListUI EnumList;

	// Token: 0x040029AD RID: 10669
	public NeedsCursor needsCursor;

	// Token: 0x040029AE RID: 10670
	protected CanvasGroup canvasGroup;

	// Token: 0x040029AF RID: 10671
	public GameObject RootPanel;
}
