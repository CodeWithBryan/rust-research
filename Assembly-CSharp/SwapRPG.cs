using System;
using UnityEngine;

// Token: 0x02000941 RID: 2369
public class SwapRPG : MonoBehaviour
{
	// Token: 0x06003838 RID: 14392 RVA: 0x0014C654 File Offset: 0x0014A854
	public void SelectRPGType(int iType)
	{
		GameObject[] array = this.rpgModels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		this.rpgModels[iType].SetActive(true);
	}

	// Token: 0x06003839 RID: 14393 RVA: 0x0014C690 File Offset: 0x0014A890
	public void UpdateAmmoType(ItemDefinition ammoType)
	{
		if (this.curAmmoType == ammoType.shortname)
		{
			return;
		}
		this.curAmmoType = ammoType.shortname;
		string a = this.curAmmoType;
		if (!(a == "ammo.rocket.basic"))
		{
			if (a == "ammo.rocket.fire")
			{
				this.SelectRPGType(1);
				return;
			}
			if (a == "ammo.rocket.hv")
			{
				this.SelectRPGType(2);
				return;
			}
			if (a == "ammo.rocket.smoke")
			{
				this.SelectRPGType(3);
				return;
			}
		}
		this.SelectRPGType(0);
	}

	// Token: 0x0600383A RID: 14394 RVA: 0x000059DD File Offset: 0x00003BDD
	private void Start()
	{
	}

	// Token: 0x04003258 RID: 12888
	public GameObject[] rpgModels;

	// Token: 0x04003259 RID: 12889
	[NonSerialized]
	private string curAmmoType = "";

	// Token: 0x02000E6B RID: 3691
	public enum RPGType
	{
		// Token: 0x04004A5E RID: 19038
		One,
		// Token: 0x04004A5F RID: 19039
		Two,
		// Token: 0x04004A60 RID: 19040
		Three,
		// Token: 0x04004A61 RID: 19041
		Four
	}
}
