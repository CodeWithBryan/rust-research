using System;
using Facepunch;
using UnityEngine;

// Token: 0x0200089F RID: 2207
public class UIPrefab : MonoBehaviour
{
	// Token: 0x060035C6 RID: 13766 RVA: 0x00142AA8 File Offset: 0x00140CA8
	private void Awake()
	{
		if (this.prefabSource == null)
		{
			return;
		}
		if (this.createdGameObject != null)
		{
			return;
		}
		this.createdGameObject = Facepunch.Instantiate.GameObject(this.prefabSource, null);
		this.createdGameObject.name = this.prefabSource.name;
		this.createdGameObject.transform.SetParent(base.transform, false);
		this.createdGameObject.Identity();
	}

	// Token: 0x060035C7 RID: 13767 RVA: 0x00142B1D File Offset: 0x00140D1D
	public void SetVisible(bool visible)
	{
		if (this.createdGameObject == null)
		{
			return;
		}
		if (this.createdGameObject.activeSelf == visible)
		{
			return;
		}
		this.createdGameObject.SetActive(visible);
	}

	// Token: 0x040030D2 RID: 12498
	public GameObject prefabSource;

	// Token: 0x040030D3 RID: 12499
	internal GameObject createdGameObject;
}
