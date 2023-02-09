using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A4C RID: 2636
	[Serializable]
	public class PostProcessEffectSettings : ScriptableObject
	{
		// Token: 0x06003E4E RID: 15950 RVA: 0x0016DDD8 File Offset: 0x0016BFD8
		private void OnEnable()
		{
			this.parameters = (from t in base.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
			where t.FieldType.IsSubclassOf(typeof(ParameterOverride))
			orderby t.MetadataToken
			select (ParameterOverride)t.GetValue(this)).ToList<ParameterOverride>().AsReadOnly();
			foreach (ParameterOverride parameterOverride in this.parameters)
			{
				parameterOverride.OnEnable();
			}
		}

		// Token: 0x06003E4F RID: 15951 RVA: 0x0016DE98 File Offset: 0x0016C098
		private void OnDisable()
		{
			if (this.parameters == null)
			{
				return;
			}
			foreach (ParameterOverride parameterOverride in this.parameters)
			{
				parameterOverride.OnDisable();
			}
		}

		// Token: 0x06003E50 RID: 15952 RVA: 0x0016DEEC File Offset: 0x0016C0EC
		public void SetAllOverridesTo(bool state, bool excludeEnabled = true)
		{
			foreach (ParameterOverride parameterOverride in this.parameters)
			{
				if (!excludeEnabled || parameterOverride != this.enabled)
				{
					parameterOverride.overrideState = state;
				}
			}
		}

		// Token: 0x06003E51 RID: 15953 RVA: 0x0016DF48 File Offset: 0x0016C148
		public virtual bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value;
		}

		// Token: 0x06003E52 RID: 15954 RVA: 0x0016DF58 File Offset: 0x0016C158
		public int GetHash()
		{
			int num = 17;
			foreach (ParameterOverride parameterOverride in this.parameters)
			{
				num = num * 23 + parameterOverride.GetHash();
			}
			return num;
		}

		// Token: 0x0400376A RID: 14186
		public bool active = true;

		// Token: 0x0400376B RID: 14187
		public BoolParameter enabled = new BoolParameter
		{
			overrideState = true,
			value = false
		};

		// Token: 0x0400376C RID: 14188
		internal ReadOnlyCollection<ParameterOverride> parameters;
	}
}
