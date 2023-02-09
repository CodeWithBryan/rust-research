using System;
using UnityEngine;

// Token: 0x020006FB RID: 1787
public class ImpostorAsset : ScriptableObject
{
	// Token: 0x06003197 RID: 12695 RVA: 0x001309D4 File Offset: 0x0012EBD4
	public Texture2D FindTexture(string name)
	{
		foreach (ImpostorAsset.TextureEntry textureEntry in this.textures)
		{
			if (textureEntry.name == name)
			{
				return textureEntry.texture;
			}
		}
		return null;
	}

	// Token: 0x04002841 RID: 10305
	public ImpostorAsset.TextureEntry[] textures;

	// Token: 0x04002842 RID: 10306
	public Vector2 size;

	// Token: 0x04002843 RID: 10307
	public Vector2 pivot;

	// Token: 0x04002844 RID: 10308
	public Mesh mesh;

	// Token: 0x02000DE3 RID: 3555
	[Serializable]
	public class TextureEntry
	{
		// Token: 0x06004F84 RID: 20356 RVA: 0x0019F893 File Offset: 0x0019DA93
		public TextureEntry(string name, Texture2D texture)
		{
			this.name = name;
			this.texture = texture;
		}

		// Token: 0x04004846 RID: 18502
		public string name;

		// Token: 0x04004847 RID: 18503
		public Texture2D texture;
	}
}
