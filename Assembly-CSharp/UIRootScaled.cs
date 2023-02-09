using System;
using ConVar;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008A2 RID: 2210
public class UIRootScaled : UIRoot
{
	// Token: 0x1700040E RID: 1038
	// (get) Token: 0x060035D1 RID: 13777 RVA: 0x00142BA4 File Offset: 0x00140DA4
	public static Canvas DragOverlayCanvas
	{
		get
		{
			return UIRootScaled.Instance.overlayCanvas;
		}
	}

	// Token: 0x060035D2 RID: 13778 RVA: 0x00142BB0 File Offset: 0x00140DB0
	protected override void Awake()
	{
		UIRootScaled.Instance = this;
		base.Awake();
	}

	// Token: 0x060035D3 RID: 13779 RVA: 0x00142BC0 File Offset: 0x00140DC0
	protected override void Refresh()
	{
		Vector2 vector = new Vector2(1280f / ConVar.Graphics.uiscale, 720f / ConVar.Graphics.uiscale);
		if (this.OverrideReference)
		{
			vector = new Vector2(this.TargetReference.x / ConVar.Graphics.uiscale, this.TargetReference.y / ConVar.Graphics.uiscale);
		}
		if (this.scaler.referenceResolution != vector)
		{
			this.scaler.referenceResolution = vector;
		}
	}

	// Token: 0x040030D6 RID: 12502
	private static UIRootScaled Instance;

	// Token: 0x040030D7 RID: 12503
	public bool OverrideReference;

	// Token: 0x040030D8 RID: 12504
	public Vector2 TargetReference = new Vector2(1280f, 720f);

	// Token: 0x040030D9 RID: 12505
	public CanvasScaler scaler;
}
