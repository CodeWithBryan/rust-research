using System;
using Facepunch.Extend;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000852 RID: 2130
public class ToggleHUDLayer : MonoBehaviour, IClientComponent
{
	// Token: 0x060034E4 RID: 13540 RVA: 0x0013F47C File Offset: 0x0013D67C
	protected void OnEnable()
	{
		UIHUD instance = SingletonComponent<UIHUD>.Instance;
		if (instance != null)
		{
			Transform transform = instance.transform.FindChildRecursive(this.hudComponentName);
			if (transform != null)
			{
				Canvas component = transform.GetComponent<Canvas>();
				if (component != null)
				{
					this.toggleControl.isOn = component.enabled;
					return;
				}
				this.toggleControl.isOn = transform.gameObject.activeSelf;
				return;
			}
			else
			{
				Debug.LogWarning(base.GetType().Name + ": Couldn't find child: " + this.hudComponentName);
			}
		}
	}

	// Token: 0x060034E5 RID: 13541 RVA: 0x0013F50C File Offset: 0x0013D70C
	public void OnToggleChanged()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "global.hudcomponent", new object[]
		{
			this.hudComponentName,
			this.toggleControl.isOn
		});
	}

	// Token: 0x04002F65 RID: 12133
	public Toggle toggleControl;

	// Token: 0x04002F66 RID: 12134
	public TextMeshProUGUI textControl;

	// Token: 0x04002F67 RID: 12135
	public string hudComponentName;
}
