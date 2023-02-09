using System;
using Rust;
using TMPro;
using UnityEngine;

// Token: 0x0200081B RID: 2075
public class ProtectionValue : MonoBehaviour, IClothingChanged
{
	// Token: 0x04002DF8 RID: 11768
	public CanvasGroup group;

	// Token: 0x04002DF9 RID: 11769
	public TextMeshProUGUI text;

	// Token: 0x04002DFA RID: 11770
	public DamageType damageType;

	// Token: 0x04002DFB RID: 11771
	public bool selectedItem;

	// Token: 0x04002DFC RID: 11772
	public bool displayBaseProtection;
}
