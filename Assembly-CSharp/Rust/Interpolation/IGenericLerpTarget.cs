using System;
using System.Collections.Generic;

namespace Rust.Interpolation
{
	// Token: 0x02000AED RID: 2797
	public interface IGenericLerpTarget<T> : ILerpInfo where T : ISnapshot<T>, new()
	{
		// Token: 0x06004331 RID: 17201
		void SetFrom(T snapshot);

		// Token: 0x06004332 RID: 17202
		T GetCurrentState();

		// Token: 0x06004333 RID: 17203
		void DebugInterpolationState(Interpolator<T>.Segment segment, List<T> entries);
	}
}
