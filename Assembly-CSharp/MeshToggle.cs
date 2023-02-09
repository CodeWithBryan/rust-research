using System;
using UnityEngine;

// Token: 0x020008D4 RID: 2260
public class MeshToggle : MonoBehaviour
{
	// Token: 0x0600364B RID: 13899 RVA: 0x00143E7C File Offset: 0x0014207C
	public void SwitchRenderer(int index)
	{
		if (this.RendererMeshes.Length == 0)
		{
			return;
		}
		MeshFilter component = base.GetComponent<MeshFilter>();
		if (!component)
		{
			return;
		}
		component.sharedMesh = this.RendererMeshes[Mathf.Clamp(index, 0, this.RendererMeshes.Length - 1)];
	}

	// Token: 0x0600364C RID: 13900 RVA: 0x00143EC4 File Offset: 0x001420C4
	public void SwitchRenderer(float factor)
	{
		int index = Mathf.RoundToInt(factor * (float)this.RendererMeshes.Length);
		this.SwitchRenderer(index);
	}

	// Token: 0x0600364D RID: 13901 RVA: 0x00143EEC File Offset: 0x001420EC
	public void SwitchCollider(int index)
	{
		if (this.ColliderMeshes.Length == 0)
		{
			return;
		}
		MeshCollider component = base.GetComponent<MeshCollider>();
		if (!component)
		{
			return;
		}
		component.sharedMesh = this.ColliderMeshes[Mathf.Clamp(index, 0, this.ColliderMeshes.Length - 1)];
	}

	// Token: 0x0600364E RID: 13902 RVA: 0x00143F34 File Offset: 0x00142134
	public void SwitchCollider(float factor)
	{
		int index = Mathf.RoundToInt(factor * (float)this.ColliderMeshes.Length);
		this.SwitchCollider(index);
	}

	// Token: 0x0600364F RID: 13903 RVA: 0x00143F59 File Offset: 0x00142159
	public void SwitchAll(int index)
	{
		this.SwitchRenderer(index);
		this.SwitchCollider(index);
	}

	// Token: 0x06003650 RID: 13904 RVA: 0x00143F69 File Offset: 0x00142169
	public void SwitchAll(float factor)
	{
		this.SwitchRenderer(factor);
		this.SwitchCollider(factor);
	}

	// Token: 0x0400314A RID: 12618
	public Mesh[] RendererMeshes;

	// Token: 0x0400314B RID: 12619
	public Mesh[] ColliderMeshes;
}
