using System;
using System.Collections.Generic;
using System.Linq;
using Rust.UI;
using UnityEngine;

// Token: 0x02000814 RID: 2068
public class OvenItemIcon : MonoBehaviour
{
	// Token: 0x06003464 RID: 13412 RVA: 0x0013D9B0 File Offset: 0x0013BBB0
	private void Start()
	{
		OvenItemIcon.OvenSlotConfig ovenSlotConfig = this.SlotConfigs.FirstOrDefault((OvenItemIcon.OvenSlotConfig x) => x.Type == this.SlotType);
		if (ovenSlotConfig == null)
		{
			Debug.LogError(string.Format("Can't find slot config for '{0}'", this.SlotType));
			return;
		}
		this.ItemIcon.emptySlotBackgroundSprite = ovenSlotConfig.BackgroundImage;
		this.MaterialLabel.SetPhrase(ovenSlotConfig.SlotPhrase);
		this.UpdateLabels();
	}

	// Token: 0x06003465 RID: 13413 RVA: 0x0013DA1B File Offset: 0x0013BC1B
	private void Update()
	{
		if (this.ItemIcon.item == this._item)
		{
			return;
		}
		this._item = this.ItemIcon.item;
		this.UpdateLabels();
	}

	// Token: 0x06003466 RID: 13414 RVA: 0x0013DA48 File Offset: 0x0013BC48
	private void UpdateLabels()
	{
		this.CanvasGroup.alpha = ((this._item != null) ? 1f : this.DisabledAlphaScale);
		RustText itemLabel = this.ItemLabel;
		if (itemLabel == null)
		{
			return;
		}
		itemLabel.SetPhrase((this._item == null) ? this.EmptyPhrase : this._item.info.displayName);
	}

	// Token: 0x04002DB9 RID: 11705
	public ItemIcon ItemIcon;

	// Token: 0x04002DBA RID: 11706
	public RustText ItemLabel;

	// Token: 0x04002DBB RID: 11707
	public RustText MaterialLabel;

	// Token: 0x04002DBC RID: 11708
	public OvenSlotType SlotType;

	// Token: 0x04002DBD RID: 11709
	public Translate.Phrase EmptyPhrase = new Translate.Phrase("empty", "empty");

	// Token: 0x04002DBE RID: 11710
	public List<OvenItemIcon.OvenSlotConfig> SlotConfigs = new List<OvenItemIcon.OvenSlotConfig>();

	// Token: 0x04002DBF RID: 11711
	public float DisabledAlphaScale;

	// Token: 0x04002DC0 RID: 11712
	public CanvasGroup CanvasGroup;

	// Token: 0x04002DC1 RID: 11713
	private Item _item;

	// Token: 0x02000E29 RID: 3625
	[Serializable]
	public class OvenSlotConfig
	{
		// Token: 0x04004960 RID: 18784
		public OvenSlotType Type;

		// Token: 0x04004961 RID: 18785
		public Sprite BackgroundImage;

		// Token: 0x04004962 RID: 18786
		public Translate.Phrase SlotPhrase;
	}
}
