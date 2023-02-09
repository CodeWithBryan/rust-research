using System;
using UnityEngine;

// Token: 0x020003BA RID: 954
public interface ISignage
{
	// Token: 0x060020A8 RID: 8360
	bool CanUpdateSign(BasePlayer player);

	// Token: 0x060020A9 RID: 8361
	float Distance(Vector3 position);

	// Token: 0x17000285 RID: 645
	// (get) Token: 0x060020AA RID: 8362
	Vector2i TextureSize { get; }

	// Token: 0x17000286 RID: 646
	// (get) Token: 0x060020AB RID: 8363
	int TextureCount { get; }

	// Token: 0x060020AC RID: 8364
	uint[] GetTextureCRCs();

	// Token: 0x17000287 RID: 647
	// (get) Token: 0x060020AD RID: 8365
	uint NetworkID { get; }

	// Token: 0x17000288 RID: 648
	// (get) Token: 0x060020AE RID: 8366
	FileStorage.Type FileType { get; }

	// Token: 0x060020AF RID: 8367
	void SetTextureCRCs(uint[] crcs);
}
