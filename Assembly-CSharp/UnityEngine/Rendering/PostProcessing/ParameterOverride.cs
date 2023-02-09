using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A39 RID: 2617
	public abstract class ParameterOverride
	{
		// Token: 0x06003DF4 RID: 15860
		internal abstract void Interp(ParameterOverride from, ParameterOverride to, float t);

		// Token: 0x06003DF5 RID: 15861
		public abstract int GetHash();

		// Token: 0x06003DF6 RID: 15862 RVA: 0x0016CDC3 File Offset: 0x0016AFC3
		public T GetValue<T>()
		{
			return ((ParameterOverride<T>)this).value;
		}

		// Token: 0x06003DF7 RID: 15863 RVA: 0x000059DD File Offset: 0x00003BDD
		protected internal virtual void OnEnable()
		{
		}

		// Token: 0x06003DF8 RID: 15864 RVA: 0x000059DD File Offset: 0x00003BDD
		protected internal virtual void OnDisable()
		{
		}

		// Token: 0x06003DF9 RID: 15865
		internal abstract void SetValue(ParameterOverride parameter);

		// Token: 0x04003738 RID: 14136
		public bool overrideState;
	}
}
