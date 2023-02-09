using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A3A RID: 2618
	[Serializable]
	public class ParameterOverride<T> : ParameterOverride
	{
		// Token: 0x06003DFB RID: 15867 RVA: 0x0016CDD0 File Offset: 0x0016AFD0
		public ParameterOverride() : this(default(T), false)
		{
		}

		// Token: 0x06003DFC RID: 15868 RVA: 0x0016CDED File Offset: 0x0016AFED
		public ParameterOverride(T value) : this(value, false)
		{
		}

		// Token: 0x06003DFD RID: 15869 RVA: 0x0016CDF7 File Offset: 0x0016AFF7
		public ParameterOverride(T value, bool overrideState)
		{
			this.value = value;
			this.overrideState = overrideState;
		}

		// Token: 0x06003DFE RID: 15870 RVA: 0x0016CE0D File Offset: 0x0016B00D
		internal override void Interp(ParameterOverride from, ParameterOverride to, float t)
		{
			this.Interp(from.GetValue<T>(), to.GetValue<T>(), t);
		}

		// Token: 0x06003DFF RID: 15871 RVA: 0x0016CE22 File Offset: 0x0016B022
		public virtual void Interp(T from, T to, float t)
		{
			this.value = ((t > 0f) ? to : from);
		}

		// Token: 0x06003E00 RID: 15872 RVA: 0x0016CE36 File Offset: 0x0016B036
		public void Override(T x)
		{
			this.overrideState = true;
			this.value = x;
		}

		// Token: 0x06003E01 RID: 15873 RVA: 0x0016CE46 File Offset: 0x0016B046
		internal override void SetValue(ParameterOverride parameter)
		{
			this.value = parameter.GetValue<T>();
		}

		// Token: 0x06003E02 RID: 15874 RVA: 0x0016CE54 File Offset: 0x0016B054
		public override int GetHash()
		{
			return (17 * 23 + this.overrideState.GetHashCode()) * 23 + this.value.GetHashCode();
		}

		// Token: 0x06003E03 RID: 15875 RVA: 0x0016CE7C File Offset: 0x0016B07C
		public static implicit operator T(ParameterOverride<T> prop)
		{
			return prop.value;
		}

		// Token: 0x04003739 RID: 14137
		public T value;
	}
}
