using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x020008B3 RID: 2227
public class RightClickReceiver : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x060035F6 RID: 13814 RVA: 0x00142E36 File Offset: 0x00141036
	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			UnityEvent clickReceiver = this.ClickReceiver;
			if (clickReceiver == null)
			{
				return;
			}
			clickReceiver.Invoke();
		}
	}

	// Token: 0x040030F1 RID: 12529
	public UnityEvent ClickReceiver;
}
