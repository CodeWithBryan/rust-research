using System;

namespace Rust.Interpolation
{
	// Token: 0x02000AEF RID: 2799
	public interface ISnapshot<T>
	{
		// Token: 0x170005D9 RID: 1497
		// (get) Token: 0x0600433F RID: 17215
		// (set) Token: 0x06004340 RID: 17216
		float Time { get; set; }

		// Token: 0x06004341 RID: 17217
		void MatchValuesTo(T entry);

		// Token: 0x06004342 RID: 17218
		void Lerp(T prev, T next, float delta);

		// Token: 0x06004343 RID: 17219
		T GetNew();
	}
}
