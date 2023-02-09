using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000839 RID: 2105
public class LookatHealth : MonoBehaviour
{
	// Token: 0x04002EE1 RID: 12001
	public static bool Enabled = true;

	// Token: 0x04002EE2 RID: 12002
	public GameObject container;

	// Token: 0x04002EE3 RID: 12003
	public Text textHealth;

	// Token: 0x04002EE4 RID: 12004
	public Text textStability;

	// Token: 0x04002EE5 RID: 12005
	public Image healthBar;

	// Token: 0x04002EE6 RID: 12006
	public Image healthBarBG;

	// Token: 0x04002EE7 RID: 12007
	public Color barBGColorNormal;

	// Token: 0x04002EE8 RID: 12008
	public Color barBGColorUnstable;
}
