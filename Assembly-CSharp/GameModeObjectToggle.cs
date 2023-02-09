using System;
using System.Linq;
using UnityEngine;

// Token: 0x020004E7 RID: 1255
public class GameModeObjectToggle : BaseMonoBehaviour
{
	// Token: 0x060027DD RID: 10205 RVA: 0x000F448A File Offset: 0x000F268A
	public void Awake()
	{
		this.SetToggle(this.defaultState);
		BaseGameMode.GameModeChanged += this.OnGameModeChanged;
	}

	// Token: 0x060027DE RID: 10206 RVA: 0x000F44A9 File Offset: 0x000F26A9
	public void OnDestroy()
	{
		BaseGameMode.GameModeChanged -= this.OnGameModeChanged;
	}

	// Token: 0x060027DF RID: 10207 RVA: 0x000F44BC File Offset: 0x000F26BC
	public void OnGameModeChanged(BaseGameMode newGameMode)
	{
		bool toggle = this.ShouldBeVisible(newGameMode);
		this.SetToggle(toggle);
	}

	// Token: 0x060027E0 RID: 10208 RVA: 0x000F44D8 File Offset: 0x000F26D8
	public void SetToggle(bool wantsOn)
	{
		foreach (GameObject gameObject in this.toToggle)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(wantsOn);
			}
		}
	}

	// Token: 0x060027E1 RID: 10209 RVA: 0x000F4510 File Offset: 0x000F2710
	public bool ShouldBeVisible(BaseGameMode newGameMode)
	{
		if (newGameMode == null)
		{
			return this.defaultState;
		}
		return (this.tagsToDisable.Length == 0 || (!newGameMode.HasAnyGameModeTag(this.tagsToDisable) && !this.tagsToDisable.Contains("*"))) && ((this.gameModeTags.Length != 0 && (newGameMode.HasAnyGameModeTag(this.gameModeTags) || this.gameModeTags.Contains("*"))) || this.defaultState);
	}

	// Token: 0x0400201B RID: 8219
	public string[] gameModeTags;

	// Token: 0x0400201C RID: 8220
	public string[] tagsToDisable;

	// Token: 0x0400201D RID: 8221
	public GameObject[] toToggle;

	// Token: 0x0400201E RID: 8222
	public bool defaultState;
}
