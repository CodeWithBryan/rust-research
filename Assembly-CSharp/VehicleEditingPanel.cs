using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200082C RID: 2092
public class VehicleEditingPanel : LootPanel
{
	// Token: 0x04002E57 RID: 11863
	[SerializeField]
	[Range(0f, 1f)]
	private float disabledAlpha = 0.25f;

	// Token: 0x04002E58 RID: 11864
	[Header("Edit Vehicle")]
	[SerializeField]
	private CanvasGroup editGroup;

	// Token: 0x04002E59 RID: 11865
	[SerializeField]
	private GameObject moduleInternalItemsGroup;

	// Token: 0x04002E5A RID: 11866
	[SerializeField]
	private GameObject moduleInternalLiquidsGroup;

	// Token: 0x04002E5B RID: 11867
	[SerializeField]
	private GameObject destroyChassisGroup;

	// Token: 0x04002E5C RID: 11868
	[SerializeField]
	private Button itemTakeButton;

	// Token: 0x04002E5D RID: 11869
	[SerializeField]
	private Button liquidTakeButton;

	// Token: 0x04002E5E RID: 11870
	[SerializeField]
	private GameObject liquidHelp;

	// Token: 0x04002E5F RID: 11871
	[SerializeField]
	private GameObject liquidButton;

	// Token: 0x04002E60 RID: 11872
	[SerializeField]
	private Color gotColor;

	// Token: 0x04002E61 RID: 11873
	[SerializeField]
	private Color notGotColor;

	// Token: 0x04002E62 RID: 11874
	[SerializeField]
	private Text generalInfoText;

	// Token: 0x04002E63 RID: 11875
	[SerializeField]
	private Text generalWarningText;

	// Token: 0x04002E64 RID: 11876
	[SerializeField]
	private Image generalWarningImage;

	// Token: 0x04002E65 RID: 11877
	[SerializeField]
	private Text repairInfoText;

	// Token: 0x04002E66 RID: 11878
	[SerializeField]
	private Button repairButton;

	// Token: 0x04002E67 RID: 11879
	[SerializeField]
	private Text destroyChassisButtonText;

	// Token: 0x04002E68 RID: 11880
	[SerializeField]
	private Text destroyChassisCountdown;

	// Token: 0x04002E69 RID: 11881
	[SerializeField]
	private Translate.Phrase phraseEditingInfo;

	// Token: 0x04002E6A RID: 11882
	[SerializeField]
	private Translate.Phrase phraseNoOccupant;

	// Token: 0x04002E6B RID: 11883
	[SerializeField]
	private Translate.Phrase phraseBadOccupant;

	// Token: 0x04002E6C RID: 11884
	[SerializeField]
	private Translate.Phrase phrasePlayerObstructing;

	// Token: 0x04002E6D RID: 11885
	[SerializeField]
	private Translate.Phrase phraseNotDriveable;

	// Token: 0x04002E6E RID: 11886
	[SerializeField]
	private Translate.Phrase phraseNotRepairable;

	// Token: 0x04002E6F RID: 11887
	[SerializeField]
	private Translate.Phrase phraseRepairNotNeeded;

	// Token: 0x04002E70 RID: 11888
	[SerializeField]
	private Translate.Phrase phraseRepairSelectInfo;

	// Token: 0x04002E71 RID: 11889
	[SerializeField]
	private Translate.Phrase phraseRepairEnactInfo;

	// Token: 0x04002E72 RID: 11890
	[SerializeField]
	private Translate.Phrase phraseHasLock;

	// Token: 0x04002E73 RID: 11891
	[SerializeField]
	private Translate.Phrase phraseHasNoLock;

	// Token: 0x04002E74 RID: 11892
	[SerializeField]
	private Translate.Phrase phraseAddLock;

	// Token: 0x04002E75 RID: 11893
	[SerializeField]
	private Translate.Phrase phraseAddLockButton;

	// Token: 0x04002E76 RID: 11894
	[SerializeField]
	private Translate.Phrase phraseChangeLockCodeButton;

	// Token: 0x04002E77 RID: 11895
	[SerializeField]
	private Text carLockInfoText;

	// Token: 0x04002E78 RID: 11896
	[SerializeField]
	private RustText carLockButtonText;

	// Token: 0x04002E79 RID: 11897
	[SerializeField]
	private Button actionLockButton;

	// Token: 0x04002E7A RID: 11898
	[SerializeField]
	private Button removeLockButton;

	// Token: 0x04002E7B RID: 11899
	[SerializeField]
	private GameObjectRef keyEnterDialog;

	// Token: 0x04002E7C RID: 11900
	[SerializeField]
	private Translate.Phrase phraseEmptyStorage;

	// Token: 0x04002E7D RID: 11901
	[Header("Create Chassis")]
	[SerializeField]
	private VehicleEditingPanel.CreateChassisEntry[] chassisOptions;

	// Token: 0x02000E2B RID: 3627
	[Serializable]
	private class CreateChassisEntry
	{
		// Token: 0x06005000 RID: 20480 RVA: 0x001A0BB1 File Offset: 0x0019EDB1
		public ItemDefinition GetChassisItemDef(ModularCarGarage garage)
		{
			return garage.chassisBuildOptions[(int)this.garageChassisIndex].itemDef;
		}

		// Token: 0x04004965 RID: 18789
		public byte garageChassisIndex;

		// Token: 0x04004966 RID: 18790
		public Button craftButton;

		// Token: 0x04004967 RID: 18791
		public Text craftButtonText;

		// Token: 0x04004968 RID: 18792
		public Text requirementsText;
	}
}
