using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200074E RID: 1870
public class EnumListUI : MonoBehaviour
{
	// Token: 0x0600332E RID: 13102 RVA: 0x0013B3F0 File Offset: 0x001395F0
	private void Awake()
	{
		this.Hide();
	}

	// Token: 0x0600332F RID: 13103 RVA: 0x0013B3F8 File Offset: 0x001395F8
	public void Show(List<object> values, Action<object> clicked)
	{
		base.gameObject.SetActive(true);
		this.clickedAction = clicked;
		foreach (object obj in this.Container)
		{
			UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
		foreach (object obj2 in values)
		{
			Transform transform = UnityEngine.Object.Instantiate<Transform>(this.PrefabItem);
			transform.SetParent(this.Container, false);
			transform.GetComponent<EnumListItemUI>().Init(obj2, obj2.ToString(), this);
		}
	}

	// Token: 0x06003330 RID: 13104 RVA: 0x0013B4C8 File Offset: 0x001396C8
	public void ItemClicked(object value)
	{
		Action<object> action = this.clickedAction;
		if (action != null)
		{
			action(value);
		}
		this.Hide();
	}

	// Token: 0x06003331 RID: 13105 RVA: 0x0013B4E2 File Offset: 0x001396E2
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x0400299E RID: 10654
	public Transform PrefabItem;

	// Token: 0x0400299F RID: 10655
	public Transform Container;

	// Token: 0x040029A0 RID: 10656
	private Action<object> clickedAction;

	// Token: 0x040029A1 RID: 10657
	private CanvasScaler canvasScaler;
}
