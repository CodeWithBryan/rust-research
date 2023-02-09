using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002A9 RID: 681
public class IconSkinPicker : MonoBehaviour
{
	// Token: 0x04001580 RID: 5504
	public GameObjectRef pickerIcon;

	// Token: 0x04001581 RID: 5505
	public GameObject container;

	// Token: 0x04001582 RID: 5506
	public Action skinChangedEvent;

	// Token: 0x04001583 RID: 5507
	public ScrollRect scroller;

	// Token: 0x04001584 RID: 5508
	public SearchFilterInput searchFilter;
}
