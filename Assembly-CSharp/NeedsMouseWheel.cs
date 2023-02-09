using System;

// Token: 0x02000777 RID: 1911
public class NeedsMouseWheel : ListComponent<NeedsMouseWheel>
{
	// Token: 0x0600336C RID: 13164 RVA: 0x0013B80D File Offset: 0x00139A0D
	public static bool AnyActive()
	{
		return ListComponent<NeedsMouseWheel>.InstanceList.Count > 0;
	}
}
