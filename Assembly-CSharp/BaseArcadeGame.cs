using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200013E RID: 318
public class BaseArcadeGame : BaseMonoBehaviour
{
	// Token: 0x06001618 RID: 5656 RVA: 0x000A8CB3 File Offset: 0x000A6EB3
	public BasePlayer GetHostPlayer()
	{
		if (this.ownerMachine)
		{
			return this.ownerMachine.GetDriver();
		}
		return null;
	}

	// Token: 0x04000EBD RID: 3773
	public static List<BaseArcadeGame> globalActiveGames = new List<BaseArcadeGame>();

	// Token: 0x04000EBE RID: 3774
	public Camera cameraToRender;

	// Token: 0x04000EBF RID: 3775
	public RenderTexture renderTexture;

	// Token: 0x04000EC0 RID: 3776
	public Texture2D distantTexture;

	// Token: 0x04000EC1 RID: 3777
	public Transform center;

	// Token: 0x04000EC2 RID: 3778
	public int frameRate = 30;

	// Token: 0x04000EC3 RID: 3779
	public Dictionary<uint, ArcadeEntity> activeArcadeEntities = new Dictionary<uint, ArcadeEntity>();

	// Token: 0x04000EC4 RID: 3780
	public Sprite[] spriteManifest;

	// Token: 0x04000EC5 RID: 3781
	public ArcadeEntity[] entityManifest;

	// Token: 0x04000EC6 RID: 3782
	public bool clientside;

	// Token: 0x04000EC7 RID: 3783
	public bool clientsideInput = true;

	// Token: 0x04000EC8 RID: 3784
	public const int spriteIndexInvisible = 1555;

	// Token: 0x04000EC9 RID: 3785
	public GameObject arcadeEntityPrefab;

	// Token: 0x04000ECA RID: 3786
	public BaseArcadeMachine ownerMachine;

	// Token: 0x04000ECB RID: 3787
	public static int gameOffsetIndex = 0;

	// Token: 0x04000ECC RID: 3788
	private bool isAuthorative;

	// Token: 0x04000ECD RID: 3789
	public Canvas canvas;
}
