using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A58 RID: 2648
	public sealed class PropertySheet
	{
		// Token: 0x1700051D RID: 1309
		// (get) Token: 0x06003EA3 RID: 16035 RVA: 0x0016FA51 File Offset: 0x0016DC51
		// (set) Token: 0x06003EA4 RID: 16036 RVA: 0x0016FA59 File Offset: 0x0016DC59
		public MaterialPropertyBlock properties { get; private set; }

		// Token: 0x1700051E RID: 1310
		// (get) Token: 0x06003EA5 RID: 16037 RVA: 0x0016FA62 File Offset: 0x0016DC62
		// (set) Token: 0x06003EA6 RID: 16038 RVA: 0x0016FA6A File Offset: 0x0016DC6A
		internal Material material { get; private set; }

		// Token: 0x06003EA7 RID: 16039 RVA: 0x0016FA73 File Offset: 0x0016DC73
		internal PropertySheet(Material material)
		{
			this.material = material;
			this.properties = new MaterialPropertyBlock();
		}

		// Token: 0x06003EA8 RID: 16040 RVA: 0x0016FA8D File Offset: 0x0016DC8D
		public void ClearKeywords()
		{
			this.material.shaderKeywords = null;
		}

		// Token: 0x06003EA9 RID: 16041 RVA: 0x0016FA9B File Offset: 0x0016DC9B
		public void EnableKeyword(string keyword)
		{
			this.material.EnableKeyword(keyword);
		}

		// Token: 0x06003EAA RID: 16042 RVA: 0x0016FAA9 File Offset: 0x0016DCA9
		public void DisableKeyword(string keyword)
		{
			this.material.DisableKeyword(keyword);
		}

		// Token: 0x06003EAB RID: 16043 RVA: 0x0016FAB7 File Offset: 0x0016DCB7
		internal void Release()
		{
			RuntimeUtilities.Destroy(this.material);
			this.material = null;
		}
	}
}
