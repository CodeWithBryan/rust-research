using System;

namespace TinyJSON
{
	// Token: 0x02000994 RID: 2452
	[Flags]
	public enum EncodeOptions
	{
		// Token: 0x040034B4 RID: 13492
		None = 0,
		// Token: 0x040034B5 RID: 13493
		PrettyPrint = 1,
		// Token: 0x040034B6 RID: 13494
		NoTypeHints = 2,
		// Token: 0x040034B7 RID: 13495
		IncludePublicProperties = 4,
		// Token: 0x040034B8 RID: 13496
		EnforceHierarchyOrder = 8,
		// Token: 0x040034B9 RID: 13497
		[Obsolete("Use EncodeOptions.EnforceHierarchyOrder instead.")]
		EnforceHeirarchyOrder = 8
	}
}
