using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020008AC RID: 2220
public class DragMe : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
{
	// Token: 0x060035E0 RID: 13792 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnBeginDrag(PointerEventData eventData)
	{
	}

	// Token: 0x1700040F RID: 1039
	// (get) Token: 0x060035E1 RID: 13793 RVA: 0x00142CFE File Offset: 0x00140EFE
	protected virtual Canvas TopCanvas
	{
		get
		{
			return UIRootScaled.DragOverlayCanvas;
		}
	}

	// Token: 0x060035E2 RID: 13794 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnDrag(PointerEventData eventData)
	{
	}

	// Token: 0x060035E3 RID: 13795 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnEndDrag(PointerEventData eventData)
	{
	}

	// Token: 0x060035E4 RID: 13796 RVA: 0x00142D05 File Offset: 0x00140F05
	public void CancelDrag()
	{
		this.OnEndDrag(new PointerEventData(EventSystem.current));
	}

	// Token: 0x040030E6 RID: 12518
	public static DragMe dragging;

	// Token: 0x040030E7 RID: 12519
	public static GameObject dragIcon;

	// Token: 0x040030E8 RID: 12520
	public static object data;

	// Token: 0x040030E9 RID: 12521
	[NonSerialized]
	public string dragType = "generic";
}
