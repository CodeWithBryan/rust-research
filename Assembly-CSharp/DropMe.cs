using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020008AF RID: 2223
public class DropMe : MonoBehaviour, IDropHandler, IEventSystemHandler
{
	// Token: 0x060035E7 RID: 13799 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnDrop(PointerEventData eventData)
	{
	}

	// Token: 0x040030EC RID: 12524
	public string[] droppableTypes;
}
