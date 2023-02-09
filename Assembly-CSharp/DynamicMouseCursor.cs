using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020007B1 RID: 1969
public class DynamicMouseCursor : MonoBehaviour
{
	// Token: 0x060033B6 RID: 13238 RVA: 0x0013BB74 File Offset: 0x00139D74
	private void LateUpdate()
	{
		if (!Cursor.visible)
		{
			return;
		}
		GameObject gameObject = this.CurrentlyHoveredItem();
		if (gameObject != null)
		{
			using (TimeWarning.New("RustControl", 0))
			{
				RustControl componentInParent = gameObject.GetComponentInParent<RustControl>();
				if (componentInParent != null && componentInParent.IsDisabled)
				{
					this.UpdateCursor(this.RegularCursor, this.RegularCursorPos);
					return;
				}
			}
			using (TimeWarning.New("ISubmitHandler", 0))
			{
				if (gameObject.GetComponentInParent<ISubmitHandler>() != null)
				{
					this.UpdateCursor(this.HoverCursor, this.HoverCursorPos);
					return;
				}
			}
			using (TimeWarning.New("IPointerDownHandler", 0))
			{
				if (gameObject.GetComponentInParent<IPointerDownHandler>() != null)
				{
					this.UpdateCursor(this.HoverCursor, this.HoverCursorPos);
					return;
				}
			}
		}
		using (TimeWarning.New("UpdateCursor", 0))
		{
			this.UpdateCursor(this.RegularCursor, this.RegularCursorPos);
		}
	}

	// Token: 0x060033B7 RID: 13239 RVA: 0x0013BCAC File Offset: 0x00139EAC
	private void UpdateCursor(Texture2D cursor, Vector2 offs)
	{
		if (this.current == cursor)
		{
			return;
		}
		this.current = cursor;
		Cursor.SetCursor(cursor, offs, CursorMode.Auto);
	}

	// Token: 0x060033B8 RID: 13240 RVA: 0x0013BCCC File Offset: 0x00139ECC
	private GameObject CurrentlyHoveredItem()
	{
		FpStandaloneInputModule fpStandaloneInputModule = EventSystem.current.currentInputModule as FpStandaloneInputModule;
		if (fpStandaloneInputModule == null)
		{
			return null;
		}
		return fpStandaloneInputModule.CurrentData.pointerCurrentRaycast.gameObject;
	}

	// Token: 0x04002BA7 RID: 11175
	public Texture2D RegularCursor;

	// Token: 0x04002BA8 RID: 11176
	public Vector2 RegularCursorPos;

	// Token: 0x04002BA9 RID: 11177
	public Texture2D HoverCursor;

	// Token: 0x04002BAA RID: 11178
	public Vector2 HoverCursorPos;

	// Token: 0x04002BAB RID: 11179
	private Texture2D current;
}
