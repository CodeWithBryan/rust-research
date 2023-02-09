using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000770 RID: 1904
public class ConsoleUI : SingletonComponent<ConsoleUI>
{
	// Token: 0x040029E0 RID: 10720
	public RustText text;

	// Token: 0x040029E1 RID: 10721
	public InputField outputField;

	// Token: 0x040029E2 RID: 10722
	public InputField inputField;

	// Token: 0x040029E3 RID: 10723
	public GameObject AutocompleteDropDown;

	// Token: 0x040029E4 RID: 10724
	public GameObject ItemTemplate;

	// Token: 0x040029E5 RID: 10725
	public Color errorColor;

	// Token: 0x040029E6 RID: 10726
	public Color warningColor;

	// Token: 0x040029E7 RID: 10727
	public Color inputColor;
}
