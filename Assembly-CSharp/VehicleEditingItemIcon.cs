using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200082B RID: 2091
[RequireComponent(typeof(ItemIcon))]
public class VehicleEditingItemIcon : MonoBehaviour, IInventoryChanged
{
	// Token: 0x04002E55 RID: 11861
	[SerializeField]
	private Image foregroundImage;

	// Token: 0x04002E56 RID: 11862
	[SerializeField]
	private Image linkImage;
}
