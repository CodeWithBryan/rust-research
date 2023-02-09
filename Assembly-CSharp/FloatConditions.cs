using System;

// Token: 0x02000901 RID: 2305
[Serializable]
public class FloatConditions
{
	// Token: 0x060036DC RID: 14044 RVA: 0x001465B8 File Offset: 0x001447B8
	public bool AllTrue(float val)
	{
		foreach (FloatConditions.Condition condition in this.conditions)
		{
			if (!condition.Test(val))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0400319B RID: 12699
	public FloatConditions.Condition[] conditions;

	// Token: 0x02000E5E RID: 3678
	[Serializable]
	public struct Condition
	{
		// Token: 0x06005067 RID: 20583 RVA: 0x001A18D0 File Offset: 0x0019FAD0
		public bool Test(float val)
		{
			switch (this.type)
			{
			case FloatConditions.Condition.Types.Equal:
				return val == this.value;
			case FloatConditions.Condition.Types.NotEqual:
				return val != this.value;
			case FloatConditions.Condition.Types.Higher:
				return val > this.value;
			case FloatConditions.Condition.Types.Lower:
				return val < this.value;
			default:
				return false;
			}
		}

		// Token: 0x04004A2E RID: 18990
		public FloatConditions.Condition.Types type;

		// Token: 0x04004A2F RID: 18991
		public float value;

		// Token: 0x02000F6E RID: 3950
		public enum Types
		{
			// Token: 0x04004E43 RID: 20035
			Equal,
			// Token: 0x04004E44 RID: 20036
			NotEqual,
			// Token: 0x04004E45 RID: 20037
			Higher,
			// Token: 0x04004E46 RID: 20038
			Lower
		}
	}
}
