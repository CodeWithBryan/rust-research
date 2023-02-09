using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x020002CE RID: 718
public class TextureColorPicker : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IDragHandler
{
	// Token: 0x06001CC6 RID: 7366 RVA: 0x000C5190 File Offset: 0x000C3390
	public virtual void OnPointerDown(PointerEventData eventData)
	{
		this.OnDrag(eventData);
	}

	// Token: 0x06001CC7 RID: 7367 RVA: 0x000C519C File Offset: 0x000C339C
	public virtual void OnDrag(PointerEventData eventData)
	{
		RectTransform rectTransform = base.transform as RectTransform;
		Vector2 vector = default(Vector2);
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out vector))
		{
			vector.x += rectTransform.rect.width * 0.5f;
			vector.y += rectTransform.rect.height * 0.5f;
			vector.x /= rectTransform.rect.width;
			vector.y /= rectTransform.rect.height;
			Color pixel = this.texture.GetPixel((int)(vector.x * (float)this.texture.width), (int)(vector.y * (float)this.texture.height));
			this.onColorSelected.Invoke(pixel);
		}
	}

	// Token: 0x04001678 RID: 5752
	public Texture2D texture;

	// Token: 0x04001679 RID: 5753
	public TextureColorPicker.onColorSelectedEvent onColorSelected = new TextureColorPicker.onColorSelectedEvent();

	// Token: 0x02000C49 RID: 3145
	[Serializable]
	public class onColorSelectedEvent : UnityEvent<Color>
	{
	}
}
