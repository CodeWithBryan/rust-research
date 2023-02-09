using System;
using Facepunch;

namespace ConVar
{
	// Token: 0x02000A88 RID: 2696
	public class Manifest
	{
		// Token: 0x0600405C RID: 16476 RVA: 0x0017B751 File Offset: 0x00179951
		[ClientVar]
		[ServerVar]
		public static object PrintManifest()
		{
			return Application.Manifest;
		}

		// Token: 0x0600405D RID: 16477 RVA: 0x0017B758 File Offset: 0x00179958
		[ClientVar]
		[ServerVar]
		public static object PrintManifestRaw()
		{
			return Manifest.Contents;
		}
	}
}
