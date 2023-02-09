using System;
using System.ComponentModel;
using UnityEngine;

// Token: 0x0200070C RID: 1804
public class RgbEffects : SingletonComponent<RgbEffects>
{
	// Token: 0x060031DB RID: 12763 RVA: 0x000059DD File Offset: 0x00003BDD
	[ClientVar(Name = "static")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void ConVar_Static(ConsoleSystem.Arg args)
	{
	}

	// Token: 0x060031DC RID: 12764 RVA: 0x000059DD File Offset: 0x00003BDD
	[ClientVar(Name = "pulse")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void ConVar_Pulse(ConsoleSystem.Arg args)
	{
	}

	// Token: 0x0400288C RID: 10380
	[ClientVar(Help = "Enables RGB lighting effects (supports SteelSeries and Razer)", Saved = true)]
	public static bool Enabled = true;

	// Token: 0x0400288D RID: 10381
	[ClientVar(Help = "Controls how RGB values are mapped to LED lights on SteelSeries devices", Saved = true)]
	public static Vector3 ColorCorrection_SteelSeries = new Vector3(1.5f, 1.5f, 1.5f);

	// Token: 0x0400288E RID: 10382
	[ClientVar(Help = "Controls how RGB values are mapped to LED lights on Razer devices", Saved = true)]
	public static Vector3 ColorCorrection_Razer = new Vector3(3f, 3f, 3f);

	// Token: 0x0400288F RID: 10383
	[ClientVar(Help = "Brightness of colors, from 0 to 1 (note: may affect color accuracy)", Saved = true)]
	public static float Brightness = 1f;

	// Token: 0x04002890 RID: 10384
	public Color defaultColor;

	// Token: 0x04002891 RID: 10385
	public Color buildingPrivilegeColor;

	// Token: 0x04002892 RID: 10386
	public Color coldColor;

	// Token: 0x04002893 RID: 10387
	public Color hotColor;

	// Token: 0x04002894 RID: 10388
	public Color hurtColor;

	// Token: 0x04002895 RID: 10389
	public Color healedColor;

	// Token: 0x04002896 RID: 10390
	public Color irradiatedColor;

	// Token: 0x04002897 RID: 10391
	public Color comfortedColor;
}
