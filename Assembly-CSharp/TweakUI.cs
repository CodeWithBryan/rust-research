using System;
using UnityEngine;

// Token: 0x02000857 RID: 2135
public class TweakUI : SingletonComponent<TweakUI>
{
	// Token: 0x060034F4 RID: 13556 RVA: 0x0013F6C3 File Offset: 0x0013D8C3
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F2) && this.CanToggle())
		{
			this.SetVisible(!TweakUI.isOpen);
		}
	}

	// Token: 0x060034F5 RID: 13557 RVA: 0x0013F6E7 File Offset: 0x0013D8E7
	protected bool CanToggle()
	{
		return LevelManager.isLoaded;
	}

	// Token: 0x060034F6 RID: 13558 RVA: 0x0013F6F3 File Offset: 0x0013D8F3
	public void SetVisible(bool b)
	{
		if (b)
		{
			TweakUI.isOpen = true;
			return;
		}
		TweakUI.isOpen = false;
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "writecfg", Array.Empty<object>());
	}

	// Token: 0x04002F6F RID: 12143
	public static bool isOpen;
}
