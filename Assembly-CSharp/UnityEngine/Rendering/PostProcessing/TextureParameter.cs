using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A44 RID: 2628
	[Serializable]
	public sealed class TextureParameter : ParameterOverride<Texture>
	{
		// Token: 0x06003E1C RID: 15900 RVA: 0x0016D1C8 File Offset: 0x0016B3C8
		public override void Interp(Texture from, Texture to, float t)
		{
			if (from == null && to == null)
			{
				this.value = null;
				return;
			}
			if (from != null && to != null)
			{
				this.value = TextureLerper.instance.Lerp(from, to, t);
				return;
			}
			if (this.defaultState == TextureParameterDefault.Lut2D)
			{
				Texture lutStrip = RuntimeUtilities.GetLutStrip((from != null) ? from.height : to.height);
				if (from == null)
				{
					from = lutStrip;
				}
				if (to == null)
				{
					to = lutStrip;
				}
			}
			Color to2;
			switch (this.defaultState)
			{
			case TextureParameterDefault.Black:
				to2 = Color.black;
				break;
			case TextureParameterDefault.White:
				to2 = Color.white;
				break;
			case TextureParameterDefault.Transparent:
				to2 = Color.clear;
				break;
			case TextureParameterDefault.Lut2D:
			{
				Texture lutStrip2 = RuntimeUtilities.GetLutStrip((from != null) ? from.height : to.height);
				if (from == null)
				{
					from = lutStrip2;
				}
				if (to == null)
				{
					to = lutStrip2;
				}
				if (from.width != to.width || from.height != to.height)
				{
					this.value = null;
					return;
				}
				this.value = TextureLerper.instance.Lerp(from, to, t);
				return;
			}
			default:
				base.Interp(from, to, t);
				return;
			}
			if (from == null)
			{
				this.value = TextureLerper.instance.Lerp(to, to2, 1f - t);
				return;
			}
			this.value = TextureLerper.instance.Lerp(from, to2, t);
		}

		// Token: 0x04003740 RID: 14144
		public TextureParameterDefault defaultState = TextureParameterDefault.Black;
	}
}
