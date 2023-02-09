using System;
using UnityEngine;

// Token: 0x0200058D RID: 1421
[CreateAssetMenu(menuName = "Rust/Gestures/Gesture Config")]
public class GestureConfig : ScriptableObject
{
	// Token: 0x06002A86 RID: 10886 RVA: 0x0010162C File Offset: 0x000FF82C
	public bool IsOwnedBy(BasePlayer player)
	{
		return this.forceUnlock || (this.dlcItem != null && this.dlcItem.CanUse(player)) || (this.inventoryItem != null && player.blueprints.steamInventory.HasItem(this.inventoryItem.id));
	}

	// Token: 0x06002A87 RID: 10887 RVA: 0x00101690 File Offset: 0x000FF890
	public bool CanBeUsedBy(BasePlayer player)
	{
		if (player.isMounted)
		{
			if (this.playerModelLayer == GestureConfig.PlayerModelLayer.FullBody)
			{
				return false;
			}
			if (player.GetMounted().allowedGestures == BaseMountable.MountGestureType.None)
			{
				return false;
			}
		}
		return (!player.IsSwimming() || this.playerModelLayer != GestureConfig.PlayerModelLayer.FullBody) && (this.playerModelLayer != GestureConfig.PlayerModelLayer.FullBody || !player.modelState.ducked);
	}

	// Token: 0x04002263 RID: 8803
	[ReadOnly]
	public uint gestureId;

	// Token: 0x04002264 RID: 8804
	public string gestureCommand;

	// Token: 0x04002265 RID: 8805
	public string convarName;

	// Token: 0x04002266 RID: 8806
	public Translate.Phrase gestureName;

	// Token: 0x04002267 RID: 8807
	public Sprite icon;

	// Token: 0x04002268 RID: 8808
	public int order = 1;

	// Token: 0x04002269 RID: 8809
	public float duration = 1.5f;

	// Token: 0x0400226A RID: 8810
	public bool canCancel = true;

	// Token: 0x0400226B RID: 8811
	[Header("Player model setup")]
	public GestureConfig.PlayerModelLayer playerModelLayer = GestureConfig.PlayerModelLayer.UpperBody;

	// Token: 0x0400226C RID: 8812
	public GestureConfig.MovementCapabilities movementMode;

	// Token: 0x0400226D RID: 8813
	public GestureConfig.AnimationType animationType;

	// Token: 0x0400226E RID: 8814
	public BasePlayer.CameraMode viewMode;

	// Token: 0x0400226F RID: 8815
	public bool useRootMotion;

	// Token: 0x04002270 RID: 8816
	public GestureConfig.GestureActionType actionType;

	// Token: 0x04002271 RID: 8817
	public bool forceUnlock;

	// Token: 0x04002272 RID: 8818
	public SteamDLCItem dlcItem;

	// Token: 0x04002273 RID: 8819
	public SteamInventoryItem inventoryItem;

	// Token: 0x02000D0E RID: 3342
	public enum PlayerModelLayer
	{
		// Token: 0x040044E4 RID: 17636
		UpperBody = 3,
		// Token: 0x040044E5 RID: 17637
		FullBody
	}

	// Token: 0x02000D0F RID: 3343
	public enum MovementCapabilities
	{
		// Token: 0x040044E7 RID: 17639
		FullMovement,
		// Token: 0x040044E8 RID: 17640
		NoMovement
	}

	// Token: 0x02000D10 RID: 3344
	public enum AnimationType
	{
		// Token: 0x040044EA RID: 17642
		OneShot,
		// Token: 0x040044EB RID: 17643
		Loop
	}

	// Token: 0x02000D11 RID: 3345
	public enum ViewMode
	{
		// Token: 0x040044ED RID: 17645
		FirstPerson,
		// Token: 0x040044EE RID: 17646
		ThirdPerson
	}

	// Token: 0x02000D12 RID: 3346
	public enum GestureActionType
	{
		// Token: 0x040044F0 RID: 17648
		None,
		// Token: 0x040044F1 RID: 17649
		ShowNameTag,
		// Token: 0x040044F2 RID: 17650
		DanceAchievement
	}
}
