using System;
using System.Collections.Generic;
using System.IO;
using Facepunch.Utility;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering;

namespace Facepunch.Unity
{
	// Token: 0x02000ABB RID: 2747
	public static class RenderInfo
	{
		// Token: 0x0600427F RID: 17023 RVA: 0x001844B4 File Offset: 0x001826B4
		public static void GenerateReport()
		{
			Renderer[] array = UnityEngine.Object.FindObjectsOfType<Renderer>();
			List<RenderInfo.RendererInstance> list = new List<RenderInfo.RendererInstance>();
			Renderer[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RenderInfo.RendererInstance item = RenderInfo.RendererInstance.From(array2[i]);
				list.Add(item);
			}
			string text = string.Format(Application.dataPath + "/../RenderInfo-{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now);
			string contents = JsonConvert.SerializeObject(list, Formatting.Indented);
			File.WriteAllText(text, contents);
			string text2 = Application.streamingAssetsPath + "/RenderInfo.exe";
			string text3 = "\"" + text + "\"";
			Debug.Log("Launching " + text2 + " " + text3);
			Os.StartProcess(text2, text3);
		}

		// Token: 0x02000F1E RID: 3870
		public struct RendererInstance
		{
			// Token: 0x060051F3 RID: 20979 RVA: 0x001A5CD8 File Offset: 0x001A3ED8
			public static RenderInfo.RendererInstance From(Renderer renderer)
			{
				RenderInfo.RendererInstance result = default(RenderInfo.RendererInstance);
				result.IsVisible = renderer.isVisible;
				result.CastShadows = (renderer.shadowCastingMode > ShadowCastingMode.Off);
				result.RecieveShadows = renderer.receiveShadows;
				result.Enabled = (renderer.enabled && renderer.gameObject.activeInHierarchy);
				result.Size = renderer.bounds.size.magnitude;
				result.Distance = Vector3.Distance(renderer.bounds.center, Camera.main.transform.position);
				result.MaterialCount = renderer.sharedMaterials.Length;
				result.RenderType = renderer.GetType().Name;
				BaseEntity baseEntity = renderer.gameObject.ToBaseEntity();
				if (baseEntity)
				{
					result.EntityName = baseEntity.PrefabName;
					if (baseEntity.net != null)
					{
						result.EntityId = baseEntity.net.ID;
					}
				}
				else
				{
					result.ObjectName = renderer.transform.GetRecursiveName("");
				}
				if (renderer is MeshRenderer)
				{
					result.BoneCount = 0;
					MeshFilter component = renderer.GetComponent<MeshFilter>();
					if (component)
					{
						result.ReadMesh(component.sharedMesh);
					}
				}
				if (renderer is SkinnedMeshRenderer)
				{
					SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
					result.ReadMesh(skinnedMeshRenderer.sharedMesh);
					result.UpdateWhenOffscreen = skinnedMeshRenderer.updateWhenOffscreen;
				}
				if (renderer is ParticleSystemRenderer)
				{
					ParticleSystem component2 = renderer.GetComponent<ParticleSystem>();
					if (component2)
					{
						result.MeshName = component2.name;
						result.ParticleCount = component2.particleCount;
					}
				}
				return result;
			}

			// Token: 0x060051F4 RID: 20980 RVA: 0x001A5E80 File Offset: 0x001A4080
			public void ReadMesh(UnityEngine.Mesh mesh)
			{
				if (mesh == null)
				{
					this.MeshName = "<NULL>";
					return;
				}
				this.VertexCount = mesh.vertexCount;
				this.SubMeshCount = mesh.subMeshCount;
				this.BlendShapeCount = mesh.blendShapeCount;
				this.MeshName = mesh.name;
			}

			// Token: 0x04004D42 RID: 19778
			public bool IsVisible;

			// Token: 0x04004D43 RID: 19779
			public bool CastShadows;

			// Token: 0x04004D44 RID: 19780
			public bool Enabled;

			// Token: 0x04004D45 RID: 19781
			public bool RecieveShadows;

			// Token: 0x04004D46 RID: 19782
			public float Size;

			// Token: 0x04004D47 RID: 19783
			public float Distance;

			// Token: 0x04004D48 RID: 19784
			public int BoneCount;

			// Token: 0x04004D49 RID: 19785
			public int MaterialCount;

			// Token: 0x04004D4A RID: 19786
			public int VertexCount;

			// Token: 0x04004D4B RID: 19787
			public int TriangleCount;

			// Token: 0x04004D4C RID: 19788
			public int SubMeshCount;

			// Token: 0x04004D4D RID: 19789
			public int BlendShapeCount;

			// Token: 0x04004D4E RID: 19790
			public string RenderType;

			// Token: 0x04004D4F RID: 19791
			public string MeshName;

			// Token: 0x04004D50 RID: 19792
			public string ObjectName;

			// Token: 0x04004D51 RID: 19793
			public string EntityName;

			// Token: 0x04004D52 RID: 19794
			public uint EntityId;

			// Token: 0x04004D53 RID: 19795
			public bool UpdateWhenOffscreen;

			// Token: 0x04004D54 RID: 19796
			public int ParticleCount;
		}
	}
}
