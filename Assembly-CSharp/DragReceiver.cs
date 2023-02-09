using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x020007EE RID: 2030
public class DragReceiver : MonoBehaviour
{
	// Token: 0x04002D09 RID: 11529
	public DragReceiver.TriggerEvent onEndDrag;

	// Token: 0x02000E27 RID: 3623
	[Serializable]
	public class TriggerEvent : UnityEvent<BaseEventData>
	{
	}
}
