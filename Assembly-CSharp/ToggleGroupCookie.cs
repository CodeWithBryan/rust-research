using System;
using System.Linq;
using Rust;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200088E RID: 2190
public class ToggleGroupCookie : MonoBehaviour
{
	// Token: 0x1700040B RID: 1035
	// (get) Token: 0x06003597 RID: 13719 RVA: 0x0014200E File Offset: 0x0014020E
	public ToggleGroup group
	{
		get
		{
			return base.GetComponent<ToggleGroup>();
		}
	}

	// Token: 0x06003598 RID: 13720 RVA: 0x00142018 File Offset: 0x00140218
	private void OnEnable()
	{
		string @string = PlayerPrefs.GetString("ToggleGroupCookie_" + base.name);
		if (!string.IsNullOrEmpty(@string))
		{
			Transform transform = base.transform.Find(@string);
			if (transform)
			{
				Toggle component = transform.GetComponent<Toggle>();
				if (component)
				{
					Toggle[] componentsInChildren = base.GetComponentsInChildren<Toggle>(true);
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].isOn = false;
					}
					component.isOn = false;
					component.isOn = true;
					this.SetupListeners();
					return;
				}
			}
		}
		Toggle toggle = this.group.ActiveToggles().FirstOrDefault((Toggle x) => x.isOn);
		if (toggle)
		{
			toggle.isOn = false;
			toggle.isOn = true;
		}
		this.SetupListeners();
	}

	// Token: 0x06003599 RID: 13721 RVA: 0x001420F0 File Offset: 0x001402F0
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		Toggle[] componentsInChildren = base.GetComponentsInChildren<Toggle>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].onValueChanged.RemoveListener(new UnityAction<bool>(this.OnToggleChanged));
		}
	}

	// Token: 0x0600359A RID: 13722 RVA: 0x00142134 File Offset: 0x00140334
	private void SetupListeners()
	{
		Toggle[] componentsInChildren = base.GetComponentsInChildren<Toggle>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleChanged));
		}
	}

	// Token: 0x0600359B RID: 13723 RVA: 0x00142170 File Offset: 0x00140370
	private void OnToggleChanged(bool b)
	{
		Toggle toggle = base.GetComponentsInChildren<Toggle>().FirstOrDefault((Toggle x) => x.isOn);
		if (toggle)
		{
			PlayerPrefs.SetString("ToggleGroupCookie_" + base.name, toggle.gameObject.name);
		}
	}
}
