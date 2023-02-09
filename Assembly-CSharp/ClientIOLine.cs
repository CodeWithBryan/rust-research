using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001B0 RID: 432
public class ClientIOLine : BaseMonoBehaviour
{
	// Token: 0x040010F6 RID: 4342
	public RendererLOD _lod;

	// Token: 0x040010F7 RID: 4343
	public LineRenderer _line;

	// Token: 0x040010F8 RID: 4344
	public Material directionalMaterial;

	// Token: 0x040010F9 RID: 4345
	public Material defaultMaterial;

	// Token: 0x040010FA RID: 4346
	public IOEntity.IOType lineType;

	// Token: 0x040010FB RID: 4347
	public static List<ClientIOLine> _allLines = new List<ClientIOLine>();

	// Token: 0x040010FC RID: 4348
	public WireTool.WireColour colour;

	// Token: 0x040010FD RID: 4349
	public IOEntity ownerIOEnt;
}
