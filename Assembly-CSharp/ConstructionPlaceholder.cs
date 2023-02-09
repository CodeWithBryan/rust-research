using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200023C RID: 572
[ExecuteInEditMode]
public class ConstructionPlaceholder : PrefabAttribute, IPrefabPreProcess
{
	// Token: 0x06001B11 RID: 6929 RVA: 0x000BD9B0 File Offset: 0x000BBBB0
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		if (clientside)
		{
			if (this.renderer)
			{
				UnityEngine.Object component = rootObj.GetComponent<MeshFilter>();
				MeshRenderer meshRenderer = rootObj.GetComponent<MeshRenderer>();
				if (!component)
				{
					rootObj.AddComponent<MeshFilter>().sharedMesh = this.mesh;
				}
				if (!meshRenderer)
				{
					meshRenderer = rootObj.AddComponent<MeshRenderer>();
					meshRenderer.sharedMaterial = this.material;
					meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
				}
			}
			if (this.collider && !rootObj.GetComponent<MeshCollider>())
			{
				rootObj.AddComponent<MeshCollider>().sharedMesh = this.mesh;
			}
		}
	}

	// Token: 0x06001B12 RID: 6930 RVA: 0x000BDA43 File Offset: 0x000BBC43
	protected override Type GetIndexedType()
	{
		return typeof(ConstructionPlaceholder);
	}

	// Token: 0x04001428 RID: 5160
	public Mesh mesh;

	// Token: 0x04001429 RID: 5161
	public Material material;

	// Token: 0x0400142A RID: 5162
	public bool renderer;

	// Token: 0x0400142B RID: 5163
	public bool collider;
}
