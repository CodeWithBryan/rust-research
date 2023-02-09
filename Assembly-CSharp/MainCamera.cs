using System;
using UnityEngine;

// Token: 0x020002AE RID: 686
[ExecuteInEditMode]
public class MainCamera : RustCamera<MainCamera>
{
	// Token: 0x1700021B RID: 539
	// (get) Token: 0x06001C46 RID: 7238 RVA: 0x000C353C File Offset: 0x000C173C
	public static bool isValid
	{
		get
		{
			return MainCamera.mainCamera != null && MainCamera.mainCamera.enabled;
		}
	}

	// Token: 0x1700021C RID: 540
	// (get) Token: 0x06001C47 RID: 7239 RVA: 0x000C3557 File Offset: 0x000C1757
	// (set) Token: 0x06001C48 RID: 7240 RVA: 0x000C355E File Offset: 0x000C175E
	public static Vector3 velocity { get; private set; }

	// Token: 0x1700021D RID: 541
	// (get) Token: 0x06001C49 RID: 7241 RVA: 0x000C3566 File Offset: 0x000C1766
	// (set) Token: 0x06001C4A RID: 7242 RVA: 0x000C3572 File Offset: 0x000C1772
	public static Vector3 position
	{
		get
		{
			return MainCamera.mainCameraTransform.position;
		}
		set
		{
			MainCamera.mainCameraTransform.position = value;
		}
	}

	// Token: 0x1700021E RID: 542
	// (get) Token: 0x06001C4B RID: 7243 RVA: 0x000C357F File Offset: 0x000C177F
	// (set) Token: 0x06001C4C RID: 7244 RVA: 0x000C358B File Offset: 0x000C178B
	public static Vector3 forward
	{
		get
		{
			return MainCamera.mainCameraTransform.forward;
		}
		set
		{
			if (value.sqrMagnitude > 0f)
			{
				MainCamera.mainCameraTransform.forward = value;
			}
		}
	}

	// Token: 0x1700021F RID: 543
	// (get) Token: 0x06001C4D RID: 7245 RVA: 0x000C35A6 File Offset: 0x000C17A6
	// (set) Token: 0x06001C4E RID: 7246 RVA: 0x000C35B2 File Offset: 0x000C17B2
	public static Vector3 right
	{
		get
		{
			return MainCamera.mainCameraTransform.right;
		}
		set
		{
			if (value.sqrMagnitude > 0f)
			{
				MainCamera.mainCameraTransform.right = value;
			}
		}
	}

	// Token: 0x17000220 RID: 544
	// (get) Token: 0x06001C4F RID: 7247 RVA: 0x000C35CD File Offset: 0x000C17CD
	// (set) Token: 0x06001C50 RID: 7248 RVA: 0x000C35D9 File Offset: 0x000C17D9
	public static Vector3 up
	{
		get
		{
			return MainCamera.mainCameraTransform.up;
		}
		set
		{
			if (value.sqrMagnitude > 0f)
			{
				MainCamera.mainCamera.transform.up = value;
			}
		}
	}

	// Token: 0x17000221 RID: 545
	// (get) Token: 0x06001C51 RID: 7249 RVA: 0x000C35F9 File Offset: 0x000C17F9
	// (set) Token: 0x06001C52 RID: 7250 RVA: 0x000C3605 File Offset: 0x000C1805
	public static Quaternion rotation
	{
		get
		{
			return MainCamera.mainCameraTransform.rotation;
		}
		set
		{
			MainCamera.mainCameraTransform.rotation = value;
		}
	}

	// Token: 0x17000222 RID: 546
	// (get) Token: 0x06001C53 RID: 7251 RVA: 0x000C3612 File Offset: 0x000C1812
	public static Ray Ray
	{
		get
		{
			return new Ray(MainCamera.position, MainCamera.forward);
		}
	}

	// Token: 0x040015A6 RID: 5542
	public static Camera mainCamera;

	// Token: 0x040015A7 RID: 5543
	public static Transform mainCameraTransform;
}
