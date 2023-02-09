using System;
using UnityEngine;

// Token: 0x020005E6 RID: 1510
[CreateAssetMenu(menuName = "Rust/MissionManifest")]
public class MissionManifest : ScriptableObject
{
	// Token: 0x06002C55 RID: 11349 RVA: 0x00109CA4 File Offset: 0x00107EA4
	public static MissionManifest Get()
	{
		if (MissionManifest.instance == null)
		{
			MissionManifest.instance = Resources.Load<MissionManifest>("MissionManifest");
			foreach (WorldPositionGenerator worldPositionGenerator in MissionManifest.instance.positionGenerators)
			{
				if (worldPositionGenerator != null)
				{
					worldPositionGenerator.PrecalculatePositions();
				}
			}
		}
		return MissionManifest.instance;
	}

	// Token: 0x06002C56 RID: 11350 RVA: 0x00109D00 File Offset: 0x00107F00
	public static BaseMission GetFromShortName(string shortname)
	{
		ScriptableObjectRef[] array = MissionManifest.Get().missionList;
		for (int i = 0; i < array.Length; i++)
		{
			BaseMission baseMission = array[i].Get() as BaseMission;
			if (baseMission.shortname == shortname)
			{
				return baseMission;
			}
		}
		return null;
	}

	// Token: 0x06002C57 RID: 11351 RVA: 0x00109D48 File Offset: 0x00107F48
	public static BaseMission GetFromID(uint id)
	{
		MissionManifest missionManifest = MissionManifest.Get();
		if (missionManifest.missionList == null)
		{
			return null;
		}
		ScriptableObjectRef[] array = missionManifest.missionList;
		for (int i = 0; i < array.Length; i++)
		{
			BaseMission baseMission = array[i].Get() as BaseMission;
			if (baseMission.id == id)
			{
				return baseMission;
			}
		}
		return null;
	}

	// Token: 0x04002423 RID: 9251
	public ScriptableObjectRef[] missionList;

	// Token: 0x04002424 RID: 9252
	public WorldPositionGenerator[] positionGenerators;

	// Token: 0x04002425 RID: 9253
	public static MissionManifest instance;
}
