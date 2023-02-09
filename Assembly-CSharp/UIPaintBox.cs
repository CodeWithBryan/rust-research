using System;
using Painting;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020007D3 RID: 2003
public class UIPaintBox : MonoBehaviour
{
	// Token: 0x06003405 RID: 13317 RVA: 0x0013CEE0 File Offset: 0x0013B0E0
	public void UpdateBrushSize(int size)
	{
		this.brush.brushSize = Vector2.one * (float)size;
		this.brush.spacing = Mathf.Clamp((float)size * 0.1f, 1f, 3f);
		this.OnChanged();
	}

	// Token: 0x06003406 RID: 13318 RVA: 0x0013CF2C File Offset: 0x0013B12C
	public void UpdateBrushTexture(Texture2D tex)
	{
		this.brush.texture = tex;
		this.OnChanged();
	}

	// Token: 0x06003407 RID: 13319 RVA: 0x0013CF40 File Offset: 0x0013B140
	public void UpdateBrushColor(Color col)
	{
		this.brush.color.r = col.r;
		this.brush.color.g = col.g;
		this.brush.color.b = col.b;
		this.OnChanged();
	}

	// Token: 0x06003408 RID: 13320 RVA: 0x0013CF95 File Offset: 0x0013B195
	public void UpdateBrushAlpha(float a)
	{
		this.brush.color.a = a;
		this.OnChanged();
	}

	// Token: 0x06003409 RID: 13321 RVA: 0x0013CFAE File Offset: 0x0013B1AE
	public void UpdateBrushEraser(bool b)
	{
		this.brush.erase = b;
	}

	// Token: 0x0600340A RID: 13322 RVA: 0x0013CFBC File Offset: 0x0013B1BC
	private void OnChanged()
	{
		this.onBrushChanged.Invoke(this.brush);
	}

	// Token: 0x04002C6D RID: 11373
	public UIPaintBox.OnBrushChanged onBrushChanged = new UIPaintBox.OnBrushChanged();

	// Token: 0x04002C6E RID: 11374
	public Brush brush;

	// Token: 0x02000E26 RID: 3622
	[Serializable]
	public class OnBrushChanged : UnityEvent<Brush>
	{
	}
}
