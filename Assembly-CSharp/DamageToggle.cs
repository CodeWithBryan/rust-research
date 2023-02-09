using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000778 RID: 1912
[RequireComponent(typeof(Toggle))]
public class DamageToggle : MonoBehaviour
{
	// Token: 0x0600336E RID: 13166 RVA: 0x0013B824 File Offset: 0x00139A24
	private void Reset()
	{
		this.toggle = base.GetComponent<Toggle>();
	}

	// Token: 0x040029FA RID: 10746
	public Toggle toggle;
}
