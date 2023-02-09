using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000836 RID: 2102
public class LookAtIOEnt : MonoBehaviour
{
	// Token: 0x04002EAF RID: 11951
	public Text objectTitle;

	// Token: 0x04002EB0 RID: 11952
	public RectTransform slotToolTip;

	// Token: 0x04002EB1 RID: 11953
	public Text slotTitle;

	// Token: 0x04002EB2 RID: 11954
	public Text slotConnection;

	// Token: 0x04002EB3 RID: 11955
	public Text slotPower;

	// Token: 0x04002EB4 RID: 11956
	public Text powerText;

	// Token: 0x04002EB5 RID: 11957
	public Text passthroughText;

	// Token: 0x04002EB6 RID: 11958
	public Text chargeLeftText;

	// Token: 0x04002EB7 RID: 11959
	public Text capacityText;

	// Token: 0x04002EB8 RID: 11960
	public Text maxOutputText;

	// Token: 0x04002EB9 RID: 11961
	public Text activeOutputText;

	// Token: 0x04002EBA RID: 11962
	public IOEntityUISlotEntry[] inputEntries;

	// Token: 0x04002EBB RID: 11963
	public IOEntityUISlotEntry[] outputEntries;

	// Token: 0x04002EBC RID: 11964
	public Color NoPowerColor;

	// Token: 0x04002EBD RID: 11965
	public GameObject GravityWarning;

	// Token: 0x04002EBE RID: 11966
	public GameObject DistanceWarning;

	// Token: 0x04002EBF RID: 11967
	public GameObject LineOfSightWarning;

	// Token: 0x04002EC0 RID: 11968
	public GameObject TooManyInputsWarning;

	// Token: 0x04002EC1 RID: 11969
	public GameObject TooManyOutputsWarning;

	// Token: 0x04002EC2 RID: 11970
	public CanvasGroup group;

	// Token: 0x04002EC3 RID: 11971
	public LookAtIOEnt.HandleSet[] handleSets;

	// Token: 0x04002EC4 RID: 11972
	public RectTransform clearNotification;

	// Token: 0x04002EC5 RID: 11973
	public CanvasGroup wireInfoGroup;

	// Token: 0x04002EC6 RID: 11974
	public Text wireLengthText;

	// Token: 0x04002EC7 RID: 11975
	public Text wireClipsText;

	// Token: 0x04002EC8 RID: 11976
	public Text errorReasonTextTooFar;

	// Token: 0x04002EC9 RID: 11977
	public Text errorReasonTextNoSurface;

	// Token: 0x04002ECA RID: 11978
	public Text errorShortCircuit;

	// Token: 0x04002ECB RID: 11979
	public RawImage ConnectionTypeIcon;

	// Token: 0x04002ECC RID: 11980
	public Texture ElectricSprite;

	// Token: 0x04002ECD RID: 11981
	public Texture FluidSprite;

	// Token: 0x04002ECE RID: 11982
	public Texture IndustrialSprite;

	// Token: 0x02000E31 RID: 3633
	[Serializable]
	public struct HandleSet
	{
		// Token: 0x0400498B RID: 18827
		public IOEntity.IOType ForIO;

		// Token: 0x0400498C RID: 18828
		public GameObjectRef handlePrefab;

		// Token: 0x0400498D RID: 18829
		public GameObjectRef handleOccupiedPrefab;

		// Token: 0x0400498E RID: 18830
		public GameObjectRef selectedHandlePrefab;

		// Token: 0x0400498F RID: 18831
		public GameObjectRef pluggedHandlePrefab;
	}
}
