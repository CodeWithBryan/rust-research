using System;
using UnityEngine;

// Token: 0x02000596 RID: 1430
public class InputState
{
	// Token: 0x06002A92 RID: 10898 RVA: 0x001018F6 File Offset: 0x000FFAF6
	public bool IsDown(BUTTON btn)
	{
		return this.current != null && (this.SwallowedButtons & (int)btn) != (int)btn && (this.current.buttons & (int)btn) == (int)btn;
	}

	// Token: 0x06002A93 RID: 10899 RVA: 0x0010191F File Offset: 0x000FFB1F
	public bool WasDown(BUTTON btn)
	{
		return this.previous != null && (this.previous.buttons & (int)btn) == (int)btn;
	}

	// Token: 0x06002A94 RID: 10900 RVA: 0x0010193B File Offset: 0x000FFB3B
	public bool IsAnyDown()
	{
		return this.current != null && (float)(this.current.buttons & ~(float)this.SwallowedButtons) > 0f;
	}

	// Token: 0x06002A95 RID: 10901 RVA: 0x00101962 File Offset: 0x000FFB62
	public bool WasJustPressed(BUTTON btn)
	{
		return this.IsDown(btn) && !this.WasDown(btn);
	}

	// Token: 0x06002A96 RID: 10902 RVA: 0x00101979 File Offset: 0x000FFB79
	public bool WasJustReleased(BUTTON btn)
	{
		return !this.IsDown(btn) && this.WasDown(btn);
	}

	// Token: 0x06002A97 RID: 10903 RVA: 0x0010198D File Offset: 0x000FFB8D
	public void SwallowButton(BUTTON btn)
	{
		if (this.current == null)
		{
			return;
		}
		this.SwallowedButtons |= (int)btn;
	}

	// Token: 0x06002A98 RID: 10904 RVA: 0x001019A6 File Offset: 0x000FFBA6
	public Quaternion AimAngle()
	{
		if (this.current == null)
		{
			return Quaternion.identity;
		}
		return Quaternion.Euler(this.current.aimAngles);
	}

	// Token: 0x06002A99 RID: 10905 RVA: 0x001019C6 File Offset: 0x000FFBC6
	public Vector3 MouseDelta()
	{
		if (this.current == null)
		{
			return Vector3.zero;
		}
		return this.current.mouseDelta;
	}

	// Token: 0x06002A9A RID: 10906 RVA: 0x001019E4 File Offset: 0x000FFBE4
	public void Flip(InputMessage newcurrent)
	{
		this.SwallowedButtons = 0;
		this.previous.aimAngles = this.current.aimAngles;
		this.previous.buttons = this.current.buttons;
		this.previous.mouseDelta = this.current.mouseDelta;
		this.current.aimAngles = newcurrent.aimAngles;
		this.current.buttons = newcurrent.buttons;
		this.current.mouseDelta = newcurrent.mouseDelta;
	}

	// Token: 0x06002A9B RID: 10907 RVA: 0x00101A6D File Offset: 0x000FFC6D
	public void Clear()
	{
		this.current.buttons = 0;
		this.previous.buttons = 0;
		this.current.mouseDelta = Vector3.zero;
		this.SwallowedButtons = 0;
	}

	// Token: 0x0400229F RID: 8863
	public InputMessage current = new InputMessage
	{
		ShouldPool = false
	};

	// Token: 0x040022A0 RID: 8864
	public InputMessage previous = new InputMessage
	{
		ShouldPool = false
	};

	// Token: 0x040022A1 RID: 8865
	private int SwallowedButtons;
}
