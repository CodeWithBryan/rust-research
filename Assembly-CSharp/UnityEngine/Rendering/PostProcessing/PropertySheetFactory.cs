using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A59 RID: 2649
	public sealed class PropertySheetFactory
	{
		// Token: 0x06003EAC RID: 16044 RVA: 0x0016FACB File Offset: 0x0016DCCB
		public PropertySheetFactory()
		{
			this.m_Sheets = new Dictionary<Shader, PropertySheet>();
		}

		// Token: 0x06003EAD RID: 16045 RVA: 0x0016FAE0 File Offset: 0x0016DCE0
		[Obsolete("Use PropertySheet.Get(Shader) with a direct reference to the Shader instead.")]
		public PropertySheet Get(string shaderName)
		{
			Shader shader = Shader.Find(shaderName);
			if (shader == null)
			{
				throw new ArgumentException(string.Format("Invalid shader ({0})", shaderName));
			}
			return this.Get(shader);
		}

		// Token: 0x06003EAE RID: 16046 RVA: 0x0016FB18 File Offset: 0x0016DD18
		public PropertySheet Get(Shader shader)
		{
			if (shader == null)
			{
				throw new ArgumentException(string.Format("Invalid shader ({0})", shader));
			}
			PropertySheet propertySheet;
			if (this.m_Sheets.TryGetValue(shader, out propertySheet))
			{
				return propertySheet;
			}
			string name = shader.name;
			propertySheet = new PropertySheet(new Material(shader)
			{
				name = string.Format("PostProcess - {0}", name.Substring(name.LastIndexOf('/') + 1)),
				hideFlags = HideFlags.DontSave
			});
			this.m_Sheets.Add(shader, propertySheet);
			return propertySheet;
		}

		// Token: 0x06003EAF RID: 16047 RVA: 0x0016FB9C File Offset: 0x0016DD9C
		public void Release()
		{
			foreach (PropertySheet propertySheet in this.m_Sheets.Values)
			{
				propertySheet.Release();
			}
			this.m_Sheets.Clear();
		}

		// Token: 0x0400379D RID: 14237
		private readonly Dictionary<Shader, PropertySheet> m_Sheets;
	}
}
