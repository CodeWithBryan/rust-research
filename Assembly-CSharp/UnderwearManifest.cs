using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200073A RID: 1850
[CreateAssetMenu(menuName = "Rust/Underwear Manifest")]
public class UnderwearManifest : ScriptableObject
{
	// Token: 0x06003312 RID: 13074 RVA: 0x0013B22F File Offset: 0x0013942F
	public static UnderwearManifest Get()
	{
		if (UnderwearManifest.instance == null)
		{
			UnderwearManifest.instance = Resources.Load<UnderwearManifest>("UnderwearManifest");
		}
		return UnderwearManifest.instance;
	}

	// Token: 0x06003313 RID: 13075 RVA: 0x0013B254 File Offset: 0x00139454
	public void PrintManifest()
	{
		Debug.Log("MANIFEST CONTENTS");
		foreach (Underwear underwear in this.underwears)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Underwear name : ",
				underwear.shortname,
				" underwear ID : ",
				underwear.GetID()
			}));
		}
	}

	// Token: 0x06003314 RID: 13076 RVA: 0x0013B2E4 File Offset: 0x001394E4
	public Underwear GetUnderwear(uint id)
	{
		foreach (Underwear underwear in this.underwears)
		{
			if (underwear.GetID() == id)
			{
				return underwear;
			}
		}
		return null;
	}

	// Token: 0x04002992 RID: 10642
	public static UnderwearManifest instance;

	// Token: 0x04002993 RID: 10643
	public List<Underwear> underwears;
}
