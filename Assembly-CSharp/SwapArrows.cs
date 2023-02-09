using System;
using Rust;
using UnityEngine;

// Token: 0x0200093F RID: 2367
public class SwapArrows : MonoBehaviour, IClientComponent
{
	// Token: 0x0600382E RID: 14382 RVA: 0x0014C485 File Offset: 0x0014A685
	public void SelectArrowType(int iType)
	{
		this.HideAllArrowHeads();
		if (iType < this.arrowModels.Length)
		{
			this.arrowModels[iType].SetActive(true);
		}
	}

	// Token: 0x0600382F RID: 14383 RVA: 0x0014C4A8 File Offset: 0x0014A6A8
	public void HideAllArrowHeads()
	{
		GameObject[] array = this.arrowModels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
	}

	// Token: 0x06003830 RID: 14384 RVA: 0x0014C4D4 File Offset: 0x0014A6D4
	public void UpdateAmmoType(ItemDefinition ammoType, bool hidden = false)
	{
		if (hidden)
		{
			this.wasHidden = hidden;
			this.HideAllArrowHeads();
			return;
		}
		if (this.curAmmoType == ammoType.shortname && hidden == this.wasHidden)
		{
			return;
		}
		this.curAmmoType = ammoType.shortname;
		this.wasHidden = hidden;
		string a = this.curAmmoType;
		if (!(a == "ammo_arrow"))
		{
			if (a == "arrow.bone")
			{
				this.SelectArrowType(0);
				return;
			}
			if (a == "arrow.fire")
			{
				this.SelectArrowType(1);
				return;
			}
			if (a == "arrow.hv")
			{
				this.SelectArrowType(2);
				return;
			}
			if (a == "ammo_arrow_poison")
			{
				this.SelectArrowType(3);
				return;
			}
			if (a == "ammo_arrow_stone")
			{
				this.SelectArrowType(4);
				return;
			}
		}
		this.HideAllArrowHeads();
	}

	// Token: 0x06003831 RID: 14385 RVA: 0x0014C5A5 File Offset: 0x0014A7A5
	private void Cleanup()
	{
		this.HideAllArrowHeads();
		this.curAmmoType = "";
	}

	// Token: 0x06003832 RID: 14386 RVA: 0x0014C5B8 File Offset: 0x0014A7B8
	public void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.Cleanup();
	}

	// Token: 0x06003833 RID: 14387 RVA: 0x0014C5C8 File Offset: 0x0014A7C8
	public void OnEnable()
	{
		this.Cleanup();
	}

	// Token: 0x04003254 RID: 12884
	public GameObject[] arrowModels;

	// Token: 0x04003255 RID: 12885
	[NonSerialized]
	private string curAmmoType = "";

	// Token: 0x04003256 RID: 12886
	private bool wasHidden;

	// Token: 0x02000E6A RID: 3690
	public enum ArrowType
	{
		// Token: 0x04004A59 RID: 19033
		One,
		// Token: 0x04004A5A RID: 19034
		Two,
		// Token: 0x04004A5B RID: 19035
		Three,
		// Token: 0x04004A5C RID: 19036
		Four
	}
}
