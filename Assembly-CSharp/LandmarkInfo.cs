using System;
using UnityEngine;

// Token: 0x02000520 RID: 1312
public class LandmarkInfo : MonoBehaviour
{
	// Token: 0x17000332 RID: 818
	// (get) Token: 0x06002858 RID: 10328 RVA: 0x00040DA9 File Offset: 0x0003EFA9
	public virtual MapLayer MapLayer
	{
		get
		{
			return MapLayer.Overworld;
		}
	}

	// Token: 0x06002859 RID: 10329 RVA: 0x000F6225 File Offset: 0x000F4425
	protected virtual void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.Landmarks.Add(this);
		}
	}

	// Token: 0x040020CB RID: 8395
	[Header("LandmarkInfo")]
	public bool shouldDisplayOnMap;

	// Token: 0x040020CC RID: 8396
	public bool isLayerSpecific;

	// Token: 0x040020CD RID: 8397
	public Translate.Phrase displayPhrase;

	// Token: 0x040020CE RID: 8398
	public Sprite mapIcon;
}
