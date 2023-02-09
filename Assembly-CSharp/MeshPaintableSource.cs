using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020002B4 RID: 692
public class MeshPaintableSource : MonoBehaviour, IClientComponent
{
	// Token: 0x06001C5B RID: 7259 RVA: 0x000C3720 File Offset: 0x000C1920
	public void Init()
	{
		if (this.texture == null)
		{
			this.texture = new Texture2D(this.texWidth, this.texHeight, TextureFormat.ARGB32, false);
			this.texture.name = "MeshPaintableSource_" + base.gameObject.name;
			this.texture.wrapMode = TextureWrapMode.Clamp;
			this.texture.Clear(Color.clear);
		}
		if (MeshPaintableSource.block == null)
		{
			MeshPaintableSource.block = new MaterialPropertyBlock();
		}
		else
		{
			MeshPaintableSource.block.Clear();
		}
		this.UpdateMaterials(MeshPaintableSource.block, null, false, this.isSelected);
		List<Renderer> list = Pool.GetList<Renderer>();
		(this.applyToAllRenderers ? base.transform.root : base.transform).GetComponentsInChildren<Renderer>(true, list);
		foreach (Renderer renderer in list)
		{
			renderer.SetPropertyBlock(MeshPaintableSource.block);
		}
		if (this.extraRenderers != null)
		{
			foreach (Renderer renderer2 in this.extraRenderers)
			{
				if (renderer2 != null)
				{
					renderer2.SetPropertyBlock(MeshPaintableSource.block);
				}
			}
		}
		if (this.legRenderer != null)
		{
			this.legRenderer.SetPropertyBlock(MeshPaintableSource.block);
		}
		Pool.FreeList<Renderer>(ref list);
	}

	// Token: 0x06001C5C RID: 7260 RVA: 0x000C3890 File Offset: 0x000C1A90
	public void Free()
	{
		if (this.texture)
		{
			UnityEngine.Object.Destroy(this.texture);
			this.texture = null;
		}
	}

	// Token: 0x06001C5D RID: 7261 RVA: 0x000C38B1 File Offset: 0x000C1AB1
	public virtual void UpdateMaterials(MaterialPropertyBlock block, Texture2D textureOverride = null, bool forEditing = false, bool isSelected = false)
	{
		block.SetTexture(this.replacementTextureName, textureOverride ?? this.texture);
	}

	// Token: 0x06001C5E RID: 7262 RVA: 0x000C38CC File Offset: 0x000C1ACC
	public virtual Color32[] UpdateFrom(Texture2D input)
	{
		this.Init();
		Color32[] pixels = input.GetPixels32();
		this.texture.SetPixels32(pixels);
		this.texture.Apply(true, false);
		return pixels;
	}

	// Token: 0x06001C5F RID: 7263 RVA: 0x000C3900 File Offset: 0x000C1B00
	public void Load(byte[] data)
	{
		this.Init();
		if (data != null)
		{
			this.texture.LoadImage(data);
			this.texture.Apply(true, false);
		}
	}

	// Token: 0x06001C60 RID: 7264 RVA: 0x000C3928 File Offset: 0x000C1B28
	public void Clear()
	{
		if (this.texture == null)
		{
			return;
		}
		this.texture.Clear(new Color(0f, 0f, 0f, 0f));
		this.texture.Apply(true, false);
	}

	// Token: 0x040015C9 RID: 5577
	public Vector4 uvRange = new Vector4(0f, 0f, 1f, 1f);

	// Token: 0x040015CA RID: 5578
	public int texWidth = 256;

	// Token: 0x040015CB RID: 5579
	public int texHeight = 128;

	// Token: 0x040015CC RID: 5580
	public string replacementTextureName = "_DecalTexture";

	// Token: 0x040015CD RID: 5581
	public float cameraFOV = 60f;

	// Token: 0x040015CE RID: 5582
	public float cameraDistance = 2f;

	// Token: 0x040015CF RID: 5583
	[NonSerialized]
	public Texture2D texture;

	// Token: 0x040015D0 RID: 5584
	public GameObject sourceObject;

	// Token: 0x040015D1 RID: 5585
	public Mesh collisionMesh;

	// Token: 0x040015D2 RID: 5586
	public Vector3 localPosition;

	// Token: 0x040015D3 RID: 5587
	public Vector3 localRotation;

	// Token: 0x040015D4 RID: 5588
	public bool applyToAllRenderers = true;

	// Token: 0x040015D5 RID: 5589
	public Renderer[] extraRenderers;

	// Token: 0x040015D6 RID: 5590
	public bool paint3D;

	// Token: 0x040015D7 RID: 5591
	[NonSerialized]
	public bool isSelected;

	// Token: 0x040015D8 RID: 5592
	[NonSerialized]
	public Renderer legRenderer;

	// Token: 0x040015D9 RID: 5593
	private static MaterialPropertyBlock block;
}
