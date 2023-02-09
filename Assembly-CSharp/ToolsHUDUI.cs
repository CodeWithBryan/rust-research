using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200088F RID: 2191
public class ToolsHUDUI : MonoBehaviour
{
	// Token: 0x0600359D RID: 13725 RVA: 0x001421D0 File Offset: 0x001403D0
	protected void OnEnable()
	{
		this.Init();
	}

	// Token: 0x0600359E RID: 13726 RVA: 0x001421D8 File Offset: 0x001403D8
	private void Init()
	{
		if (this.initialised)
		{
			return;
		}
		UIHUD instance = SingletonComponent<UIHUD>.Instance;
		if (instance == null)
		{
			return;
		}
		this.initialised = true;
		foreach (Transform transform in instance.GetComponentsInChildren<Transform>())
		{
			string name = transform.name;
			if (name.ToLower().StartsWith("gameui.hud."))
			{
				if (name.ToLower() == "gameui.hud.crosshair")
				{
					foreach (object obj in transform)
					{
						Transform transform2 = (Transform)obj;
						this.AddToggleObj(transform2.name, "<color=yellow>Crosshair sub:</color> " + transform2.name);
					}
				}
				this.AddToggleObj(name, name.Substring(11));
			}
		}
	}

	// Token: 0x0600359F RID: 13727 RVA: 0x001422CC File Offset: 0x001404CC
	private void AddToggleObj(string trName, string labelText)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab, Vector3.zero, Quaternion.identity, this.parent);
		gameObject.name = trName;
		ToggleHUDLayer component = gameObject.GetComponent<ToggleHUDLayer>();
		component.hudComponentName = trName;
		component.textControl.text = labelText;
	}

	// Token: 0x060035A0 RID: 13728 RVA: 0x00142308 File Offset: 0x00140508
	public void SelectAll()
	{
		Toggle[] componentsInChildren = this.parent.GetComponentsInChildren<Toggle>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].isOn = true;
		}
	}

	// Token: 0x060035A1 RID: 13729 RVA: 0x00142338 File Offset: 0x00140538
	public void SelectNone()
	{
		Toggle[] componentsInChildren = this.parent.GetComponentsInChildren<Toggle>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].isOn = false;
		}
	}

	// Token: 0x0400309C RID: 12444
	[SerializeField]
	private GameObject prefab;

	// Token: 0x0400309D RID: 12445
	[SerializeField]
	private Transform parent;

	// Token: 0x0400309E RID: 12446
	private bool initialised;
}
