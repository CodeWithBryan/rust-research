using System;
using UnityEngine;

// Token: 0x02000775 RID: 1909
public class CursorManager : SingletonComponent<CursorManager>
{
	// Token: 0x06003362 RID: 13154 RVA: 0x0013B740 File Offset: 0x00139940
	private void Update()
	{
		if (SingletonComponent<CursorManager>.Instance != this)
		{
			return;
		}
		if (CursorManager.iHoldOpen == 0 && CursorManager.iPreviousOpen == 0)
		{
			this.SwitchToGame();
		}
		else
		{
			this.SwitchToUI();
		}
		CursorManager.iPreviousOpen = CursorManager.iHoldOpen;
		CursorManager.iHoldOpen = 0;
	}

	// Token: 0x06003363 RID: 13155 RVA: 0x0013B77C File Offset: 0x0013997C
	public void SwitchToGame()
	{
		if (Cursor.lockState != CursorLockMode.Locked)
		{
			Cursor.lockState = CursorLockMode.Locked;
		}
		if (Cursor.visible)
		{
			Cursor.visible = false;
		}
		CursorManager.lastTimeInvisible = Time.time;
	}

	// Token: 0x06003364 RID: 13156 RVA: 0x0013B7A3 File Offset: 0x001399A3
	private void SwitchToUI()
	{
		if (Cursor.lockState != CursorLockMode.None)
		{
			Cursor.lockState = CursorLockMode.None;
		}
		if (!Cursor.visible)
		{
			Cursor.visible = true;
		}
		CursorManager.lastTimeVisible = Time.time;
	}

	// Token: 0x06003365 RID: 13157 RVA: 0x0013B7C9 File Offset: 0x001399C9
	public static void HoldOpen(bool cursorVisible = false)
	{
		CursorManager.iHoldOpen++;
	}

	// Token: 0x06003366 RID: 13158 RVA: 0x0013B7D7 File Offset: 0x001399D7
	public static bool WasVisible(float deltaTime)
	{
		return Time.time - CursorManager.lastTimeVisible <= deltaTime;
	}

	// Token: 0x06003367 RID: 13159 RVA: 0x0013B7EA File Offset: 0x001399EA
	public static bool WasInvisible(float deltaTime)
	{
		return Time.time - CursorManager.lastTimeInvisible <= deltaTime;
	}

	// Token: 0x040029F6 RID: 10742
	private static int iHoldOpen;

	// Token: 0x040029F7 RID: 10743
	private static int iPreviousOpen;

	// Token: 0x040029F8 RID: 10744
	private static float lastTimeVisible;

	// Token: 0x040029F9 RID: 10745
	private static float lastTimeInvisible;
}
