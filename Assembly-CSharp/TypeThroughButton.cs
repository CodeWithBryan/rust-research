using System;
using System.Collections;
using Rust;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000892 RID: 2194
public class TypeThroughButton : Button, IUpdateSelectedHandler, IEventSystemHandler
{
	// Token: 0x060035A6 RID: 13734 RVA: 0x00142384 File Offset: 0x00140584
	public void OnUpdateSelected(BaseEventData eventData)
	{
		if (this.typingTarget == null)
		{
			return;
		}
		while (Event.PopEvent(this._processingEvent))
		{
			if (this._processingEvent.rawType == EventType.KeyDown && this._processingEvent.character != '\0')
			{
				Event e = new Event(this._processingEvent);
				Global.Runner.StartCoroutine(this.DelayedActivateTextField(e));
				break;
			}
		}
		eventData.Use();
	}

	// Token: 0x060035A7 RID: 13735 RVA: 0x001423EE File Offset: 0x001405EE
	private IEnumerator DelayedActivateTextField(Event e)
	{
		this.typingTarget.ActivateInputField();
		this.typingTarget.Select();
		if (e.character != ' ')
		{
			InputField inputField = this.typingTarget;
			inputField.text += " ";
		}
		this.typingTarget.MoveTextEnd(false);
		this.typingTarget.ProcessEvent(e);
		yield return null;
		this.typingTarget.caretPosition = this.typingTarget.text.Length;
		this.typingTarget.ForceLabelUpdate();
		yield break;
	}

	// Token: 0x040030A3 RID: 12451
	public InputField typingTarget;

	// Token: 0x040030A4 RID: 12452
	private Event _processingEvent = new Event();
}
