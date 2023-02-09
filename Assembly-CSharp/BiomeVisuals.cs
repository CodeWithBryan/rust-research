using System;
using UnityEngine;

// Token: 0x0200064A RID: 1610
public class BiomeVisuals : MonoBehaviour
{
	// Token: 0x06002E08 RID: 11784 RVA: 0x001144CC File Offset: 0x001126CC
	protected void Start()
	{
		int num = (TerrainMeta.BiomeMap != null) ? TerrainMeta.BiomeMap.GetBiomeMaxType(base.transform.position, -1) : 2;
		switch (num)
		{
		case 1:
			this.SetChoice(this.Arid);
			return;
		case 2:
			this.SetChoice(this.Temperate);
			return;
		case 3:
			break;
		case 4:
			this.SetChoice(this.Tundra);
			return;
		default:
			if (num != 8)
			{
				return;
			}
			this.SetChoice(this.Arctic);
			break;
		}
	}

	// Token: 0x06002E09 RID: 11785 RVA: 0x00114550 File Offset: 0x00112750
	private void SetChoice(GameObject selection)
	{
		bool shouldDestroy = !base.gameObject.SupportsPoolingInParent();
		this.ApplyChoice(selection, this.Arid, shouldDestroy);
		this.ApplyChoice(selection, this.Temperate, shouldDestroy);
		this.ApplyChoice(selection, this.Tundra, shouldDestroy);
		this.ApplyChoice(selection, this.Arctic, shouldDestroy);
		if (selection != null)
		{
			selection.SetActive(true);
		}
		GameManager.Destroy(this, 0f);
	}

	// Token: 0x06002E0A RID: 11786 RVA: 0x001145BF File Offset: 0x001127BF
	private void ApplyChoice(GameObject selection, GameObject target, bool shouldDestroy)
	{
		if (target != null && target != selection)
		{
			if (shouldDestroy)
			{
				GameManager.Destroy(target, 0f);
				return;
			}
			target.SetActive(false);
		}
	}

	// Token: 0x040025AF RID: 9647
	public GameObject Arid;

	// Token: 0x040025B0 RID: 9648
	public GameObject Temperate;

	// Token: 0x040025B1 RID: 9649
	public GameObject Tundra;

	// Token: 0x040025B2 RID: 9650
	public GameObject Arctic;
}
