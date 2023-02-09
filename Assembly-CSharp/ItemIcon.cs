using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020007F6 RID: 2038
public class ItemIcon : BaseMonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IDraggable, IInventoryChanged, IItemAmountChanged, IItemIconChanged
{
	// Token: 0x0600343C RID: 13372 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnPointerClick(PointerEventData eventData)
	{
	}

	// Token: 0x0600343D RID: 13373 RVA: 0x000059DD File Offset: 0x00003BDD
	public void OnPointerEnter(PointerEventData eventData)
	{
	}

	// Token: 0x0600343E RID: 13374 RVA: 0x000059DD File Offset: 0x00003BDD
	public void OnPointerExit(PointerEventData eventData)
	{
	}

	// Token: 0x04002D2D RID: 11565
	private Color backgroundColor;

	// Token: 0x04002D2E RID: 11566
	public Color selectedBackgroundColor = new Color(0.12156863f, 0.41960785f, 0.627451f, 0.78431374f);

	// Token: 0x04002D2F RID: 11567
	public float unoccupiedAlpha = 1f;

	// Token: 0x04002D30 RID: 11568
	public Color unoccupiedColor;

	// Token: 0x04002D31 RID: 11569
	public ItemContainerSource containerSource;

	// Token: 0x04002D32 RID: 11570
	public int slotOffset;

	// Token: 0x04002D33 RID: 11571
	[Range(0f, 64f)]
	public int slot;

	// Token: 0x04002D34 RID: 11572
	public bool setSlotFromSiblingIndex = true;

	// Token: 0x04002D35 RID: 11573
	public GameObject slots;

	// Token: 0x04002D36 RID: 11574
	public CanvasGroup iconContents;

	// Token: 0x04002D37 RID: 11575
	public CanvasGroup canvasGroup;

	// Token: 0x04002D38 RID: 11576
	public Image iconImage;

	// Token: 0x04002D39 RID: 11577
	public Image underlayImage;

	// Token: 0x04002D3A RID: 11578
	public Text amountText;

	// Token: 0x04002D3B RID: 11579
	public Text hoverText;

	// Token: 0x04002D3C RID: 11580
	public Image hoverOutline;

	// Token: 0x04002D3D RID: 11581
	public Image cornerIcon;

	// Token: 0x04002D3E RID: 11582
	public Image lockedImage;

	// Token: 0x04002D3F RID: 11583
	public Image progressImage;

	// Token: 0x04002D40 RID: 11584
	public Image backgroundImage;

	// Token: 0x04002D41 RID: 11585
	public Image backgroundUnderlayImage;

	// Token: 0x04002D42 RID: 11586
	public Image progressPanel;

	// Token: 0x04002D43 RID: 11587
	public Sprite emptySlotBackgroundSprite;

	// Token: 0x04002D44 RID: 11588
	public CanvasGroup conditionObject;

	// Token: 0x04002D45 RID: 11589
	public Image conditionFill;

	// Token: 0x04002D46 RID: 11590
	public Image maxConditionFill;

	// Token: 0x04002D47 RID: 11591
	public GameObject lightEnabled;

	// Token: 0x04002D48 RID: 11592
	public bool allowSelection = true;

	// Token: 0x04002D49 RID: 11593
	public bool allowDropping = true;

	// Token: 0x04002D4A RID: 11594
	public bool allowMove = true;

	// Token: 0x04002D4B RID: 11595
	public bool showCountDropShadow;

	// Token: 0x04002D4C RID: 11596
	[NonSerialized]
	public Item item;

	// Token: 0x04002D4D RID: 11597
	[NonSerialized]
	public bool invalidSlot;

	// Token: 0x04002D4E RID: 11598
	public SoundDefinition hoverSound;
}
