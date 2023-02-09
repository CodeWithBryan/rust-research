using System;
using UnityEngine;

// Token: 0x020008B9 RID: 2233
public class VitalRadial : MonoBehaviour
{
	// Token: 0x060035FC RID: 13820 RVA: 0x00142E7E File Offset: 0x0014107E
	private void Awake()
	{
		Debug.LogWarning("VitalRadial is obsolete " + base.transform.GetRecursiveName(""), base.gameObject);
	}
}
