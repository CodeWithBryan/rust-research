using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A50 RID: 2640
	public sealed class PostProcessProfile : ScriptableObject
	{
		// Token: 0x06003E6A RID: 15978 RVA: 0x0016E8E9 File Offset: 0x0016CAE9
		private void OnEnable()
		{
			this.settings.RemoveAll((PostProcessEffectSettings x) => x == null);
		}

		// Token: 0x06003E6B RID: 15979 RVA: 0x0016E916 File Offset: 0x0016CB16
		public T AddSettings<T>() where T : PostProcessEffectSettings
		{
			return (T)((object)this.AddSettings(typeof(T)));
		}

		// Token: 0x06003E6C RID: 15980 RVA: 0x0016E930 File Offset: 0x0016CB30
		public PostProcessEffectSettings AddSettings(Type type)
		{
			if (this.HasSettings(type))
			{
				throw new InvalidOperationException("Effect already exists in the stack");
			}
			PostProcessEffectSettings postProcessEffectSettings = (PostProcessEffectSettings)ScriptableObject.CreateInstance(type);
			postProcessEffectSettings.hideFlags = (HideFlags.HideInHierarchy | HideFlags.HideInInspector);
			postProcessEffectSettings.name = type.Name;
			postProcessEffectSettings.enabled.value = true;
			this.settings.Add(postProcessEffectSettings);
			this.isDirty = true;
			return postProcessEffectSettings;
		}

		// Token: 0x06003E6D RID: 15981 RVA: 0x0016E990 File Offset: 0x0016CB90
		public PostProcessEffectSettings AddSettings(PostProcessEffectSettings effect)
		{
			if (this.HasSettings(this.settings.GetType()))
			{
				throw new InvalidOperationException("Effect already exists in the stack");
			}
			this.settings.Add(effect);
			this.isDirty = true;
			return effect;
		}

		// Token: 0x06003E6E RID: 15982 RVA: 0x0016E9C4 File Offset: 0x0016CBC4
		public void RemoveSettings<T>() where T : PostProcessEffectSettings
		{
			this.RemoveSettings(typeof(T));
		}

		// Token: 0x06003E6F RID: 15983 RVA: 0x0016E9D8 File Offset: 0x0016CBD8
		public void RemoveSettings(Type type)
		{
			int num = -1;
			for (int i = 0; i < this.settings.Count; i++)
			{
				if (this.settings[i].GetType() == type)
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				throw new InvalidOperationException("Effect doesn't exist in the profile");
			}
			this.settings.RemoveAt(num);
			this.isDirty = true;
		}

		// Token: 0x06003E70 RID: 15984 RVA: 0x0016EA3C File Offset: 0x0016CC3C
		public bool HasSettings<T>() where T : PostProcessEffectSettings
		{
			return this.HasSettings(typeof(T));
		}

		// Token: 0x06003E71 RID: 15985 RVA: 0x0016EA50 File Offset: 0x0016CC50
		public bool HasSettings(Type type)
		{
			using (List<PostProcessEffectSettings>.Enumerator enumerator = this.settings.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.GetType() == type)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06003E72 RID: 15986 RVA: 0x0016EAB0 File Offset: 0x0016CCB0
		public T GetSetting<T>() where T : PostProcessEffectSettings
		{
			foreach (PostProcessEffectSettings postProcessEffectSettings in this.settings)
			{
				if (postProcessEffectSettings is T)
				{
					return postProcessEffectSettings as T;
				}
			}
			return default(T);
		}

		// Token: 0x06003E73 RID: 15987 RVA: 0x0016EB20 File Offset: 0x0016CD20
		public bool TryGetSettings<T>(out T outSetting) where T : PostProcessEffectSettings
		{
			Type typeFromHandle = typeof(T);
			outSetting = default(T);
			foreach (PostProcessEffectSettings postProcessEffectSettings in this.settings)
			{
				if (postProcessEffectSettings.GetType() == typeFromHandle)
				{
					outSetting = (T)((object)postProcessEffectSettings);
					return true;
				}
			}
			return false;
		}

		// Token: 0x04003778 RID: 14200
		[Tooltip("A list of all settings currently stored in this profile.")]
		public List<PostProcessEffectSettings> settings = new List<PostProcessEffectSettings>();

		// Token: 0x04003779 RID: 14201
		[NonSerialized]
		public bool isDirty = true;
	}
}
