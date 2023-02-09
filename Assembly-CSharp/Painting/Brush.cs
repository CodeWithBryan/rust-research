using System;
using UnityEngine;

namespace Painting
{
	// Token: 0x020009A9 RID: 2473
	[Serializable]
	public class Brush
	{
		// Token: 0x040034D8 RID: 13528
		public float spacing;

		// Token: 0x040034D9 RID: 13529
		public Vector2 brushSize;

		// Token: 0x040034DA RID: 13530
		public Texture2D texture;

		// Token: 0x040034DB RID: 13531
		public Color color;

		// Token: 0x040034DC RID: 13532
		public bool erase;
	}
}
