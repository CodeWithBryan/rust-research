using System;
using ProtoBuf;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000886 RID: 2182
public class SleepingBagButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x17000406 RID: 1030
	// (get) Token: 0x06003576 RID: 13686 RVA: 0x00141A1D File Offset: 0x0013FC1D
	public float timerSeconds
	{
		get
		{
			return Mathf.Clamp(this.releaseTime - Time.realtimeSinceStartup, 0f, 216000f);
		}
	}

	// Token: 0x17000407 RID: 1031
	// (get) Token: 0x06003577 RID: 13687 RVA: 0x00141A3A File Offset: 0x0013FC3A
	public string friendlyName
	{
		get
		{
			if (this.spawnOption == null || string.IsNullOrEmpty(this.spawnOption.name))
			{
				return "Null Sleeping Bag";
			}
			return this.spawnOption.name;
		}
	}

	// Token: 0x06003578 RID: 13688 RVA: 0x00141A67 File Offset: 0x0013FC67
	private void OnEnable()
	{
		if (this.DeleteButton != null)
		{
			this.DeleteButton.SetActive(false);
		}
	}

	// Token: 0x06003579 RID: 13689 RVA: 0x00141A84 File Offset: 0x0013FC84
	public void Setup(RespawnInformation.SpawnOptions option, UIDeathScreen.RespawnColourScheme colourScheme)
	{
		this.spawnOption = option;
		switch (option.type)
		{
		case RespawnInformation.SpawnOptions.RespawnType.SleepingBag:
			this.Icon.sprite = this.SleepingBagSprite;
			break;
		case RespawnInformation.SpawnOptions.RespawnType.Bed:
			this.Icon.sprite = this.BedSprite;
			break;
		case RespawnInformation.SpawnOptions.RespawnType.BeachTowel:
			this.Icon.sprite = this.BeachTowelSprite;
			break;
		case RespawnInformation.SpawnOptions.RespawnType.Camper:
			this.Icon.sprite = this.CamperSprite;
			break;
		}
		this.Background.color = colourScheme.BackgroundColour;
		this.CircleFill.color = colourScheme.CircleFillColour;
		this.CircleRim.color = colourScheme.CircleRimColour;
		this.releaseTime = ((option.unlockSeconds > 0f) ? (Time.realtimeSinceStartup + option.unlockSeconds) : 0f);
		this.UpdateButtonState(option);
		this.BagName.text = this.friendlyName;
	}

	// Token: 0x0600357A RID: 13690 RVA: 0x00141B74 File Offset: 0x0013FD74
	private void UpdateButtonState(RespawnInformation.SpawnOptions option)
	{
		bool flag = this.releaseTime > 0f && this.releaseTime > Time.realtimeSinceStartup;
		bool occupied = option.occupied;
		this.LockRoot.SetActive(flag);
		this.OccupiedRoot.SetActive(occupied);
		this.TimeLockRoot.SetActive(flag);
		this.ClickButton.interactable = (!occupied && !flag);
	}

	// Token: 0x0600357B RID: 13691 RVA: 0x00141BE0 File Offset: 0x0013FDE0
	public void Update()
	{
		if (this.releaseTime == 0f)
		{
			return;
		}
		if (this.releaseTime < Time.realtimeSinceStartup)
		{
			this.UpdateButtonState(this.spawnOption);
			return;
		}
		this.LockTime.text = this.timerSeconds.ToString("N0");
	}

	// Token: 0x0600357C RID: 13692 RVA: 0x00141C33 File Offset: 0x0013FE33
	public void DoSpawn()
	{
		if (this.spawnOption == null)
		{
			return;
		}
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "respawn_sleepingbag", new object[]
		{
			this.spawnOption.id
		});
	}

	// Token: 0x0600357D RID: 13693 RVA: 0x00141C67 File Offset: 0x0013FE67
	public void DeleteBag()
	{
		if (this.spawnOption == null)
		{
			return;
		}
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "respawn_sleepingbag_remove", new object[]
		{
			this.spawnOption.id
		});
	}

	// Token: 0x0600357E RID: 13694 RVA: 0x00141C9B File Offset: 0x0013FE9B
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (this.DeleteButton != null)
		{
			this.DeleteButton.SetActive(true);
		}
	}

	// Token: 0x0600357F RID: 13695 RVA: 0x00141A67 File Offset: 0x0013FC67
	public void OnPointerExit(PointerEventData eventData)
	{
		if (this.DeleteButton != null)
		{
			this.DeleteButton.SetActive(false);
		}
	}

	// Token: 0x0400305B RID: 12379
	public GameObject TimeLockRoot;

	// Token: 0x0400305C RID: 12380
	public GameObject LockRoot;

	// Token: 0x0400305D RID: 12381
	public GameObject OccupiedRoot;

	// Token: 0x0400305E RID: 12382
	public Button ClickButton;

	// Token: 0x0400305F RID: 12383
	public TextMeshProUGUI BagName;

	// Token: 0x04003060 RID: 12384
	public TextMeshProUGUI LockTime;

	// Token: 0x04003061 RID: 12385
	public Image Icon;

	// Token: 0x04003062 RID: 12386
	public Sprite SleepingBagSprite;

	// Token: 0x04003063 RID: 12387
	public Sprite BedSprite;

	// Token: 0x04003064 RID: 12388
	public Sprite BeachTowelSprite;

	// Token: 0x04003065 RID: 12389
	public Sprite CamperSprite;

	// Token: 0x04003066 RID: 12390
	public Image CircleRim;

	// Token: 0x04003067 RID: 12391
	public Image CircleFill;

	// Token: 0x04003068 RID: 12392
	public Image Background;

	// Token: 0x04003069 RID: 12393
	public GameObject DeleteButton;

	// Token: 0x0400306A RID: 12394
	internal RespawnInformation.SpawnOptions spawnOption;

	// Token: 0x0400306B RID: 12395
	internal float releaseTime;
}
