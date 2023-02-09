using System;
using UnityEngine;

namespace Smaa
{
	// Token: 0x02000991 RID: 2449
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Image Effects/Subpixel Morphological Antialiasing")]
	public class SMAA : MonoBehaviour
	{
		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x060039FC RID: 14844 RVA: 0x00155FA9 File Offset: 0x001541A9
		public Material Material
		{
			get
			{
				if (this.m_Material == null)
				{
					this.m_Material = new Material(this.Shader);
					this.m_Material.hideFlags = HideFlags.HideAndDontSave;
				}
				return this.m_Material;
			}
		}

		// Token: 0x0400349A RID: 13466
		public DebugPass DebugPass;

		// Token: 0x0400349B RID: 13467
		public QualityPreset Quality = QualityPreset.High;

		// Token: 0x0400349C RID: 13468
		public EdgeDetectionMethod DetectionMethod = EdgeDetectionMethod.Luma;

		// Token: 0x0400349D RID: 13469
		public bool UsePredication;

		// Token: 0x0400349E RID: 13470
		public Preset CustomPreset;

		// Token: 0x0400349F RID: 13471
		public PredicationPreset CustomPredicationPreset;

		// Token: 0x040034A0 RID: 13472
		public Shader Shader;

		// Token: 0x040034A1 RID: 13473
		public Texture2D AreaTex;

		// Token: 0x040034A2 RID: 13474
		public Texture2D SearchTex;

		// Token: 0x040034A3 RID: 13475
		protected Camera m_Camera;

		// Token: 0x040034A4 RID: 13476
		protected Preset m_LowPreset;

		// Token: 0x040034A5 RID: 13477
		protected Preset m_MediumPreset;

		// Token: 0x040034A6 RID: 13478
		protected Preset m_HighPreset;

		// Token: 0x040034A7 RID: 13479
		protected Preset m_UltraPreset;

		// Token: 0x040034A8 RID: 13480
		protected Material m_Material;
	}
}
