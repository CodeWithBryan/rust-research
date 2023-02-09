using System;
using System.Collections.Generic;

// Token: 0x020003BB RID: 955
public interface IUGCBrowserEntity
{
	// Token: 0x17000289 RID: 649
	// (get) Token: 0x060020B0 RID: 8368
	uint[] GetContentCRCs { get; }

	// Token: 0x060020B1 RID: 8369
	void ClearContent();

	// Token: 0x1700028A RID: 650
	// (get) Token: 0x060020B2 RID: 8370
	UGCType ContentType { get; }

	// Token: 0x1700028B RID: 651
	// (get) Token: 0x060020B3 RID: 8371
	List<ulong> EditingHistory { get; }

	// Token: 0x1700028C RID: 652
	// (get) Token: 0x060020B4 RID: 8372
	BaseNetworkable UgcEntity { get; }
}
