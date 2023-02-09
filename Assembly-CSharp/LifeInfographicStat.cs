using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000833 RID: 2099
public class LifeInfographicStat : MonoBehaviour
{
	// Token: 0x04002EA3 RID: 11939
	public LifeInfographicStat.DataType dataSource;

	// Token: 0x04002EA4 RID: 11940
	[Header("Generic Stats")]
	public string genericStatKey;

	// Token: 0x04002EA5 RID: 11941
	[Header("Weapon Info")]
	public string targetWeaponName;

	// Token: 0x04002EA6 RID: 11942
	public LifeInfographicStat.WeaponInfoType weaponInfoType;

	// Token: 0x04002EA7 RID: 11943
	public TextMeshProUGUI targetText;

	// Token: 0x04002EA8 RID: 11944
	public Image StatImage;

	// Token: 0x02000E2F RID: 3631
	public enum DataType
	{
		// Token: 0x0400496F RID: 18799
		None,
		// Token: 0x04004970 RID: 18800
		AliveTime_Short,
		// Token: 0x04004971 RID: 18801
		SleepingTime_Short,
		// Token: 0x04004972 RID: 18802
		KillerName,
		// Token: 0x04004973 RID: 18803
		KillerWeapon,
		// Token: 0x04004974 RID: 18804
		AliveTime_Long,
		// Token: 0x04004975 RID: 18805
		KillerDistance,
		// Token: 0x04004976 RID: 18806
		GenericStat,
		// Token: 0x04004977 RID: 18807
		DistanceTravelledWalk,
		// Token: 0x04004978 RID: 18808
		DistanceTravelledRun,
		// Token: 0x04004979 RID: 18809
		DamageTaken,
		// Token: 0x0400497A RID: 18810
		DamageHealed,
		// Token: 0x0400497B RID: 18811
		WeaponInfo,
		// Token: 0x0400497C RID: 18812
		SecondsWilderness,
		// Token: 0x0400497D RID: 18813
		SecondsSwimming,
		// Token: 0x0400497E RID: 18814
		SecondsInBase,
		// Token: 0x0400497F RID: 18815
		SecondsInMonument,
		// Token: 0x04004980 RID: 18816
		SecondsFlying,
		// Token: 0x04004981 RID: 18817
		SecondsBoating,
		// Token: 0x04004982 RID: 18818
		PlayersKilled,
		// Token: 0x04004983 RID: 18819
		ScientistsKilled,
		// Token: 0x04004984 RID: 18820
		AnimalsKilled,
		// Token: 0x04004985 RID: 18821
		SecondsDriving
	}

	// Token: 0x02000E30 RID: 3632
	public enum WeaponInfoType
	{
		// Token: 0x04004987 RID: 18823
		TotalShots,
		// Token: 0x04004988 RID: 18824
		ShotsHit,
		// Token: 0x04004989 RID: 18825
		ShotsMissed,
		// Token: 0x0400498A RID: 18826
		AccuracyPercentage
	}
}
