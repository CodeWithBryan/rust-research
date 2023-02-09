using System;
using UnityEngine;

// Token: 0x02000944 RID: 2372
[Serializable]
public struct ViewModelDrawEvent : IEquatable<ViewModelDrawEvent>
{
	// Token: 0x0600383D RID: 14397 RVA: 0x0014C72C File Offset: 0x0014A92C
	public bool Equals(ViewModelDrawEvent other)
	{
		return object.Equals(this.viewModelRenderer, other.viewModelRenderer) && object.Equals(this.renderer, other.renderer) && this.skipDepthPrePass == other.skipDepthPrePass && object.Equals(this.material, other.material) && this.subMesh == other.subMesh && this.pass == other.pass;
	}

	// Token: 0x0600383E RID: 14398 RVA: 0x0014C7A0 File Offset: 0x0014A9A0
	public override bool Equals(object obj)
	{
		if (obj is ViewModelDrawEvent)
		{
			ViewModelDrawEvent other = (ViewModelDrawEvent)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x0600383F RID: 14399 RVA: 0x0014C7C8 File Offset: 0x0014A9C8
	public override int GetHashCode()
	{
		return ((((((this.viewModelRenderer != null) ? this.viewModelRenderer.GetHashCode() : 0) * 397 ^ ((this.renderer != null) ? this.renderer.GetHashCode() : 0)) * 397 ^ this.skipDepthPrePass.GetHashCode()) * 397 ^ ((this.material != null) ? this.material.GetHashCode() : 0)) * 397 ^ this.subMesh) * 397 ^ this.pass;
	}

	// Token: 0x04003260 RID: 12896
	public ViewModelRenderer viewModelRenderer;

	// Token: 0x04003261 RID: 12897
	public Renderer renderer;

	// Token: 0x04003262 RID: 12898
	public bool skipDepthPrePass;

	// Token: 0x04003263 RID: 12899
	public Material material;

	// Token: 0x04003264 RID: 12900
	public int subMesh;

	// Token: 0x04003265 RID: 12901
	public int pass;
}
