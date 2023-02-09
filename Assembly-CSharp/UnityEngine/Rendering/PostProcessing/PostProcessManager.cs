using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A4F RID: 2639
	public sealed class PostProcessManager
	{
		// Token: 0x17000515 RID: 1301
		// (get) Token: 0x06003E57 RID: 15959 RVA: 0x0016DFEC File Offset: 0x0016C1EC
		public static PostProcessManager instance
		{
			get
			{
				if (PostProcessManager.s_Instance == null)
				{
					PostProcessManager.s_Instance = new PostProcessManager();
				}
				return PostProcessManager.s_Instance;
			}
		}

		// Token: 0x06003E58 RID: 15960 RVA: 0x0016E004 File Offset: 0x0016C204
		private PostProcessManager()
		{
			this.m_SortedVolumes = new Dictionary<int, List<PostProcessVolume>>();
			this.m_Volumes = new List<PostProcessVolume>();
			this.m_SortNeeded = new Dictionary<int, bool>();
			this.m_BaseSettings = new List<PostProcessEffectSettings>();
			this.settingsTypes = new Dictionary<Type, PostProcessAttribute>();
			this.ReloadBaseTypes();
		}

		// Token: 0x06003E59 RID: 15961 RVA: 0x0016E054 File Offset: 0x0016C254
		private void CleanBaseTypes()
		{
			this.settingsTypes.Clear();
			foreach (PostProcessEffectSettings obj in this.m_BaseSettings)
			{
				RuntimeUtilities.Destroy(obj);
			}
			this.m_BaseSettings.Clear();
		}

		// Token: 0x06003E5A RID: 15962 RVA: 0x0016E0BC File Offset: 0x0016C2BC
		private void ReloadBaseTypes()
		{
			this.CleanBaseTypes();
			foreach (Type type in from t in RuntimeUtilities.GetAllAssemblyTypes()
			where t.IsSubclassOf(typeof(PostProcessEffectSettings)) && t.IsDefined(typeof(PostProcessAttribute), false) && !t.IsAbstract
			select t)
			{
				this.settingsTypes.Add(type, type.GetAttribute<PostProcessAttribute>());
				PostProcessEffectSettings postProcessEffectSettings = (PostProcessEffectSettings)ScriptableObject.CreateInstance(type);
				postProcessEffectSettings.SetAllOverridesTo(true, false);
				this.m_BaseSettings.Add(postProcessEffectSettings);
			}
		}

		// Token: 0x06003E5B RID: 15963 RVA: 0x0016E160 File Offset: 0x0016C360
		public void GetActiveVolumes(PostProcessLayer layer, List<PostProcessVolume> results, bool skipDisabled = true, bool skipZeroWeight = true)
		{
			int value = layer.volumeLayer.value;
			Transform volumeTrigger = layer.volumeTrigger;
			bool flag = volumeTrigger == null;
			Vector3 vector = flag ? Vector3.zero : volumeTrigger.position;
			foreach (PostProcessVolume postProcessVolume in this.GrabVolumes(value))
			{
				if ((!skipDisabled || postProcessVolume.enabled) && !(postProcessVolume.profileRef == null) && (!skipZeroWeight || postProcessVolume.weight > 0f))
				{
					if (postProcessVolume.isGlobal)
					{
						results.Add(postProcessVolume);
					}
					else if (!flag)
					{
						OBB obb = new OBB(postProcessVolume.transform, postProcessVolume.bounds);
						float sqrMagnitude = ((obb.ClosestPoint(vector) - vector) / 2f).sqrMagnitude;
						float num = postProcessVolume.blendDistance * postProcessVolume.blendDistance;
						if (sqrMagnitude <= num)
						{
							results.Add(postProcessVolume);
						}
					}
				}
			}
		}

		// Token: 0x06003E5C RID: 15964 RVA: 0x0016E280 File Offset: 0x0016C480
		public PostProcessVolume GetHighestPriorityVolume(PostProcessLayer layer)
		{
			if (layer == null)
			{
				throw new ArgumentNullException("layer");
			}
			return this.GetHighestPriorityVolume(layer.volumeLayer);
		}

		// Token: 0x06003E5D RID: 15965 RVA: 0x0016E2A4 File Offset: 0x0016C4A4
		public PostProcessVolume GetHighestPriorityVolume(LayerMask mask)
		{
			float num = float.NegativeInfinity;
			PostProcessVolume result = null;
			List<PostProcessVolume> list;
			if (this.m_SortedVolumes.TryGetValue(mask, out list))
			{
				foreach (PostProcessVolume postProcessVolume in list)
				{
					if (postProcessVolume.priority > num)
					{
						num = postProcessVolume.priority;
						result = postProcessVolume;
					}
				}
			}
			return result;
		}

		// Token: 0x06003E5E RID: 15966 RVA: 0x0016E320 File Offset: 0x0016C520
		public PostProcessVolume QuickVolume(int layer, float priority, params PostProcessEffectSettings[] settings)
		{
			PostProcessVolume postProcessVolume = new GameObject
			{
				name = "Quick Volume",
				layer = layer,
				hideFlags = HideFlags.HideAndDontSave
			}.AddComponent<PostProcessVolume>();
			postProcessVolume.priority = priority;
			postProcessVolume.isGlobal = true;
			PostProcessProfile profile = postProcessVolume.profile;
			foreach (PostProcessEffectSettings postProcessEffectSettings in settings)
			{
				Assert.IsNotNull<PostProcessEffectSettings>(postProcessEffectSettings, "Trying to create a volume with null effects");
				profile.AddSettings(postProcessEffectSettings);
			}
			return postProcessVolume;
		}

		// Token: 0x06003E5F RID: 15967 RVA: 0x0016E394 File Offset: 0x0016C594
		internal void SetLayerDirty(int layer)
		{
			Assert.IsTrue(layer >= 0 && layer <= 32, "Invalid layer bit");
			foreach (KeyValuePair<int, List<PostProcessVolume>> keyValuePair in this.m_SortedVolumes)
			{
				int key = keyValuePair.Key;
				if ((key & 1 << layer) != 0)
				{
					this.m_SortNeeded[key] = true;
				}
			}
		}

		// Token: 0x06003E60 RID: 15968 RVA: 0x0016E418 File Offset: 0x0016C618
		internal void UpdateVolumeLayer(PostProcessVolume volume, int prevLayer, int newLayer)
		{
			Assert.IsTrue(prevLayer >= 0 && prevLayer <= 32, "Invalid layer bit");
			this.Unregister(volume, prevLayer);
			this.Register(volume, newLayer);
		}

		// Token: 0x06003E61 RID: 15969 RVA: 0x0016E444 File Offset: 0x0016C644
		private void Register(PostProcessVolume volume, int layer)
		{
			this.m_Volumes.Add(volume);
			foreach (KeyValuePair<int, List<PostProcessVolume>> keyValuePair in this.m_SortedVolumes)
			{
				if ((keyValuePair.Key & 1 << layer) != 0)
				{
					keyValuePair.Value.Add(volume);
				}
			}
			this.SetLayerDirty(layer);
		}

		// Token: 0x06003E62 RID: 15970 RVA: 0x0016E4C0 File Offset: 0x0016C6C0
		internal void Register(PostProcessVolume volume)
		{
			int layer = volume.gameObject.layer;
			this.Register(volume, layer);
		}

		// Token: 0x06003E63 RID: 15971 RVA: 0x0016E4E4 File Offset: 0x0016C6E4
		private void Unregister(PostProcessVolume volume, int layer)
		{
			this.m_Volumes.Remove(volume);
			foreach (KeyValuePair<int, List<PostProcessVolume>> keyValuePair in this.m_SortedVolumes)
			{
				if ((keyValuePair.Key & 1 << layer) != 0)
				{
					keyValuePair.Value.Remove(volume);
				}
			}
		}

		// Token: 0x06003E64 RID: 15972 RVA: 0x0016E55C File Offset: 0x0016C75C
		internal void Unregister(PostProcessVolume volume)
		{
			int layer = volume.gameObject.layer;
			this.Unregister(volume, layer);
		}

		// Token: 0x06003E65 RID: 15973 RVA: 0x0016E580 File Offset: 0x0016C780
		private void ReplaceData(PostProcessLayer postProcessLayer)
		{
			foreach (PostProcessEffectSettings postProcessEffectSettings in this.m_BaseSettings)
			{
				PostProcessEffectSettings settings = postProcessLayer.GetBundle(postProcessEffectSettings.GetType()).settings;
				int count = postProcessEffectSettings.parameters.Count;
				for (int i = 0; i < count; i++)
				{
					settings.parameters[i].SetValue(postProcessEffectSettings.parameters[i]);
				}
			}
		}

		// Token: 0x06003E66 RID: 15974 RVA: 0x0016E61C File Offset: 0x0016C81C
		internal void UpdateSettings(PostProcessLayer postProcessLayer, Camera camera)
		{
			this.ReplaceData(postProcessLayer);
			int value = postProcessLayer.volumeLayer.value;
			Transform volumeTrigger = postProcessLayer.volumeTrigger;
			bool flag = volumeTrigger == null;
			Vector3 vector = flag ? Vector3.zero : volumeTrigger.position;
			foreach (PostProcessVolume postProcessVolume in this.GrabVolumes(value))
			{
				if (postProcessVolume.enabled && !(postProcessVolume.profileRef == null) && postProcessVolume.weight > 0f)
				{
					List<PostProcessEffectSettings> settings = postProcessVolume.profileRef.settings;
					if (postProcessVolume.isGlobal)
					{
						postProcessLayer.OverrideSettings(settings, Mathf.Clamp01(postProcessVolume.weight));
					}
					else if (!flag)
					{
						OBB obb = new OBB(postProcessVolume.transform, postProcessVolume.bounds);
						float sqrMagnitude = ((obb.ClosestPoint(vector) - vector) / 2f).sqrMagnitude;
						float num = postProcessVolume.blendDistance * postProcessVolume.blendDistance;
						if (sqrMagnitude <= num)
						{
							float num2 = 1f;
							if (num > 0f)
							{
								num2 = 1f - sqrMagnitude / num;
							}
							postProcessLayer.OverrideSettings(settings, num2 * Mathf.Clamp01(postProcessVolume.weight));
						}
					}
				}
			}
		}

		// Token: 0x06003E67 RID: 15975 RVA: 0x0016E7A0 File Offset: 0x0016C9A0
		private List<PostProcessVolume> GrabVolumes(LayerMask mask)
		{
			List<PostProcessVolume> list;
			if (!this.m_SortedVolumes.TryGetValue(mask, out list))
			{
				list = new List<PostProcessVolume>();
				foreach (PostProcessVolume postProcessVolume in this.m_Volumes)
				{
					if ((mask & 1 << postProcessVolume.gameObject.layer) != 0)
					{
						list.Add(postProcessVolume);
						this.m_SortNeeded[mask] = true;
					}
				}
				this.m_SortedVolumes.Add(mask, list);
			}
			bool flag;
			if (this.m_SortNeeded.TryGetValue(mask, out flag) && flag)
			{
				this.m_SortNeeded[mask] = false;
				PostProcessManager.SortByPriority(list);
			}
			return list;
		}

		// Token: 0x06003E68 RID: 15976 RVA: 0x0016E87C File Offset: 0x0016CA7C
		private static void SortByPriority(List<PostProcessVolume> volumes)
		{
			Assert.IsNotNull<List<PostProcessVolume>>(volumes, "Trying to sort volumes of non-initialized layer");
			for (int i = 1; i < volumes.Count; i++)
			{
				PostProcessVolume postProcessVolume = volumes[i];
				int num = i - 1;
				while (num >= 0 && volumes[num].priority > postProcessVolume.priority)
				{
					volumes[num + 1] = volumes[num];
					num--;
				}
				volumes[num + 1] = postProcessVolume;
			}
		}

		// Token: 0x06003E69 RID: 15977 RVA: 0x00003A54 File Offset: 0x00001C54
		private static bool IsVolumeRenderedByCamera(PostProcessVolume volume, Camera camera)
		{
			return true;
		}

		// Token: 0x04003771 RID: 14193
		private static PostProcessManager s_Instance;

		// Token: 0x04003772 RID: 14194
		private const int k_MaxLayerCount = 32;

		// Token: 0x04003773 RID: 14195
		private readonly Dictionary<int, List<PostProcessVolume>> m_SortedVolumes;

		// Token: 0x04003774 RID: 14196
		private readonly List<PostProcessVolume> m_Volumes;

		// Token: 0x04003775 RID: 14197
		private readonly Dictionary<int, bool> m_SortNeeded;

		// Token: 0x04003776 RID: 14198
		private readonly List<PostProcessEffectSettings> m_BaseSettings;

		// Token: 0x04003777 RID: 14199
		public readonly Dictionary<Type, PostProcessAttribute> settingsTypes;
	}
}
