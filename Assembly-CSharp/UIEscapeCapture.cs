using System;
using UnityEngine.Events;

// Token: 0x02000899 RID: 2201
public class UIEscapeCapture : ListComponent<UIEscapeCapture>
{
	// Token: 0x060035B4 RID: 13748 RVA: 0x00142550 File Offset: 0x00140750
	public static bool EscapePressed()
	{
		using (ListHashSet<UIEscapeCapture>.Enumerator enumerator = ListComponent<UIEscapeCapture>.InstanceList.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				enumerator.Current.onEscape.Invoke();
				return true;
			}
		}
		return false;
	}

	// Token: 0x040030C2 RID: 12482
	public UnityEvent onEscape = new UnityEvent();
}
