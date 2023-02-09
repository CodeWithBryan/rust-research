using System;
using System.Collections.Generic;
using Facepunch;
using Rust.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000859 RID: 2137
public class TweakUIDropdown : TweakUIBase
{
	// Token: 0x06003502 RID: 13570 RVA: 0x0013F7EC File Offset: 0x0013D9EC
	protected override void Init()
	{
		base.Init();
		this.DropdownItemPrefab.SetActive(false);
		this.UpdateDropdownOptions();
		this.Opener.SetToggleFalse();
		this.ResetToConvar();
	}

	// Token: 0x06003503 RID: 13571 RVA: 0x0013F7A8 File Offset: 0x0013D9A8
	protected void OnEnable()
	{
		this.ResetToConvar();
	}

	// Token: 0x06003504 RID: 13572 RVA: 0x0013F818 File Offset: 0x0013DA18
	public void UpdateDropdownOptions()
	{
		List<RustButton> list = Pool.GetList<RustButton>();
		this.DropdownContainer.GetComponentsInChildren<RustButton>(false, list);
		foreach (RustButton rustButton in list)
		{
			UnityEngine.Object.Destroy(rustButton.gameObject);
		}
		Pool.FreeList<RustButton>(ref list);
		for (int i = 0; i < this.nameValues.Length; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.DropdownItemPrefab, this.DropdownContainer);
			int itemIndex = i;
			RustButton component = gameObject.GetComponent<RustButton>();
			component.Text.SetPhrase(this.nameValues[i].label);
			component.OnPressed.AddListener(delegate()
			{
				this.ChangeValue(itemIndex);
			});
			gameObject.SetActive(true);
		}
	}

	// Token: 0x06003505 RID: 13573 RVA: 0x0013F8F4 File Offset: 0x0013DAF4
	public void OnValueChanged()
	{
		if (this.ApplyImmediatelyOnChange)
		{
			this.SetConvarValue();
		}
	}

	// Token: 0x06003506 RID: 13574 RVA: 0x0013F904 File Offset: 0x0013DB04
	public void OnDropdownOpen()
	{
		RectTransform rectTransform = (RectTransform)base.transform;
		if (rectTransform.position.y <= (float)Screen.height / 2f)
		{
			this.Dropdown.pivot = new Vector2(0.5f, 0f);
			this.Dropdown.anchoredPosition = this.Dropdown.anchoredPosition.WithY(0f);
			return;
		}
		this.Dropdown.pivot = new Vector2(0.5f, 1f);
		this.Dropdown.anchoredPosition = this.Dropdown.anchoredPosition.WithY(-rectTransform.rect.height);
	}

	// Token: 0x06003507 RID: 13575 RVA: 0x0013F9BC File Offset: 0x0013DBBC
	public void ChangeValue(int index)
	{
		this.Opener.SetToggleFalse();
		int num = Mathf.Clamp(index, 0, this.nameValues.Length - 1);
		bool flag = num != this.currentValue;
		this.currentValue = num;
		if (this.ApplyImmediatelyOnChange)
		{
			this.SetConvarValue();
		}
		else
		{
			this.ShowValue(this.nameValues[this.currentValue].value);
		}
		if (flag)
		{
			UnityEvent unityEvent = this.onValueChanged;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x06003508 RID: 13576 RVA: 0x0013FA34 File Offset: 0x0013DC34
	protected override void SetConvarValue()
	{
		base.SetConvarValue();
		TweakUIDropdown.NameValue nameValue = this.nameValues[this.currentValue];
		if (this.conVar == null)
		{
			return;
		}
		if (this.conVar.String == nameValue.value)
		{
			return;
		}
		this.conVar.Set(nameValue.value);
	}

	// Token: 0x06003509 RID: 13577 RVA: 0x0013FA88 File Offset: 0x0013DC88
	public override void ResetToConvar()
	{
		base.ResetToConvar();
		if (this.conVar == null)
		{
			return;
		}
		string @string = this.conVar.String;
		this.ShowValue(@string);
	}

	// Token: 0x0600350A RID: 13578 RVA: 0x0013FAB8 File Offset: 0x0013DCB8
	private void ShowValue(string value)
	{
		int i = 0;
		while (i < this.nameValues.Length)
		{
			if (!(this.nameValues[i].value != value))
			{
				this.Current.SetPhrase(this.nameValues[i].label);
				this.currentValue = i;
				if (this.assignImageColor)
				{
					this.BackgroundImage.color = this.nameValues[i].imageColor;
					return;
				}
				break;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x04002F73 RID: 12147
	public RustText Current;

	// Token: 0x04002F74 RID: 12148
	public Image BackgroundImage;

	// Token: 0x04002F75 RID: 12149
	public RustButton Opener;

	// Token: 0x04002F76 RID: 12150
	public RectTransform Dropdown;

	// Token: 0x04002F77 RID: 12151
	public RectTransform DropdownContainer;

	// Token: 0x04002F78 RID: 12152
	public GameObject DropdownItemPrefab;

	// Token: 0x04002F79 RID: 12153
	public TweakUIDropdown.NameValue[] nameValues;

	// Token: 0x04002F7A RID: 12154
	public bool assignImageColor;

	// Token: 0x04002F7B RID: 12155
	public UnityEvent onValueChanged = new UnityEvent();

	// Token: 0x04002F7C RID: 12156
	public int currentValue;

	// Token: 0x02000E3B RID: 3643
	[Serializable]
	public class NameValue
	{
		// Token: 0x040049AF RID: 18863
		public string value;

		// Token: 0x040049B0 RID: 18864
		public Color imageColor;

		// Token: 0x040049B1 RID: 18865
		public Translate.Phrase label;
	}
}
