using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000816 RID: 2070
public class OvenLootPanel : MonoBehaviour
{
	// Token: 0x04002DCE RID: 11726
	public GameObject controlsOn;

	// Token: 0x04002DCF RID: 11727
	public GameObject controlsOff;

	// Token: 0x04002DD0 RID: 11728
	public Image TitleBackground;

	// Token: 0x04002DD1 RID: 11729
	public RustText TitleText;

	// Token: 0x04002DD2 RID: 11730
	public Color AlertBackgroundColor;

	// Token: 0x04002DD3 RID: 11731
	public Color AlertTextColor;

	// Token: 0x04002DD4 RID: 11732
	public Color OffBackgroundColor;

	// Token: 0x04002DD5 RID: 11733
	public Color OffTextColor;

	// Token: 0x04002DD6 RID: 11734
	public Color OnBackgroundColor;

	// Token: 0x04002DD7 RID: 11735
	public Color OnTextColor;

	// Token: 0x04002DD8 RID: 11736
	private Translate.Phrase OffPhrase = new Translate.Phrase("off", "off");

	// Token: 0x04002DD9 RID: 11737
	private Translate.Phrase OnPhrase = new Translate.Phrase("on", "on");

	// Token: 0x04002DDA RID: 11738
	private Translate.Phrase NoFuelPhrase = new Translate.Phrase("no_fuel", "No Fuel");

	// Token: 0x04002DDB RID: 11739
	public GameObject FuelRowPrefab;

	// Token: 0x04002DDC RID: 11740
	public GameObject MaterialRowPrefab;

	// Token: 0x04002DDD RID: 11741
	public GameObject ItemRowPrefab;

	// Token: 0x04002DDE RID: 11742
	public Sprite IconBackground_Wood;

	// Token: 0x04002DDF RID: 11743
	public Sprite IconBackGround_Input;

	// Token: 0x04002DE0 RID: 11744
	public LootGrid LootGrid_Wood;

	// Token: 0x04002DE1 RID: 11745
	public LootGrid LootGrid_Input;

	// Token: 0x04002DE2 RID: 11746
	public LootGrid LootGrid_Output;

	// Token: 0x04002DE3 RID: 11747
	public GameObject Contents;

	// Token: 0x04002DE4 RID: 11748
	public GameObject[] ElectricDisableRoots = new GameObject[0];
}
