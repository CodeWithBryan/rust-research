using System;
using UnityEngine;

// Token: 0x0200026C RID: 620
public class AnimalSkin : MonoBehaviour, IClientComponent
{
	// Token: 0x06001BC4 RID: 7108 RVA: 0x000C0F88 File Offset: 0x000BF188
	private void Start()
	{
		this.model = base.gameObject.GetComponent<Model>();
		if (!this.dontRandomizeOnStart)
		{
			int iSkin = Mathf.FloorToInt((float)UnityEngine.Random.Range(0, this.animalSkins.Length));
			this.ChangeSkin(iSkin);
		}
	}

	// Token: 0x06001BC5 RID: 7109 RVA: 0x000C0FCC File Offset: 0x000BF1CC
	public void ChangeSkin(int iSkin)
	{
		if (this.animalSkins.Length == 0)
		{
			return;
		}
		iSkin = Mathf.Clamp(iSkin, 0, this.animalSkins.Length - 1);
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.animalMesh)
		{
			Material[] sharedMaterials = skinnedMeshRenderer.sharedMaterials;
			if (sharedMaterials != null)
			{
				for (int j = 0; j < sharedMaterials.Length; j++)
				{
					sharedMaterials[j] = this.animalSkins[iSkin].multiSkin[j];
				}
				skinnedMeshRenderer.sharedMaterials = sharedMaterials;
			}
		}
		if (this.model != null)
		{
			this.model.skin = iSkin;
		}
	}

	// Token: 0x040014C7 RID: 5319
	public SkinnedMeshRenderer[] animalMesh;

	// Token: 0x040014C8 RID: 5320
	public AnimalMultiSkin[] animalSkins;

	// Token: 0x040014C9 RID: 5321
	private Model model;

	// Token: 0x040014CA RID: 5322
	public bool dontRandomizeOnStart;
}
