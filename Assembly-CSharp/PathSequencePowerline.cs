using System;
using System.Collections.Generic;

// Token: 0x02000657 RID: 1623
public class PathSequencePowerline : PathSequence
{
	// Token: 0x06002E24 RID: 11812 RVA: 0x00114BCC File Offset: 0x00112DCC
	public override void ApplySequenceReplacement(List<Prefab> sequence, ref Prefab replacement, Prefab[] possibleReplacements, int pathLength, int pathIndex)
	{
		bool flag = false;
		if (this.Rule == PathSequencePowerline.SequenceRule.Powerline)
		{
			if (pathLength >= 3)
			{
				flag = (sequence.Count == 0 || pathIndex == pathLength - 1);
				if (!flag)
				{
					flag = (this.GetIndexCountToRule(sequence, PathSequencePowerline.SequenceRule.PowerlinePlatform) >= 2);
				}
			}
		}
		else if (this.Rule == PathSequencePowerline.SequenceRule.PowerlinePlatform)
		{
			flag = (pathLength < 3);
			if (!flag)
			{
				int indexCountToRule = this.GetIndexCountToRule(sequence, PathSequencePowerline.SequenceRule.PowerlinePlatform);
				flag = (indexCountToRule < 2 && indexCountToRule != sequence.Count && pathIndex < pathLength - 1);
			}
		}
		if (flag)
		{
			Prefab prefabOfType = this.GetPrefabOfType(possibleReplacements, (this.Rule == PathSequencePowerline.SequenceRule.PowerlinePlatform) ? PathSequencePowerline.SequenceRule.Powerline : PathSequencePowerline.SequenceRule.PowerlinePlatform);
			if (prefabOfType != null)
			{
				replacement = prefabOfType;
			}
		}
	}

	// Token: 0x06002E25 RID: 11813 RVA: 0x00114C64 File Offset: 0x00112E64
	private Prefab GetPrefabOfType(Prefab[] options, PathSequencePowerline.SequenceRule ruleToFind)
	{
		for (int i = 0; i < options.Length; i++)
		{
			PathSequencePowerline pathSequencePowerline = options[i].Attribute.Find<PathSequence>(options[i].ID) as PathSequencePowerline;
			if (pathSequencePowerline == null || pathSequencePowerline.Rule == ruleToFind)
			{
				return options[i];
			}
		}
		return null;
	}

	// Token: 0x06002E26 RID: 11814 RVA: 0x00114CB4 File Offset: 0x00112EB4
	private int GetIndexCountToRule(List<Prefab> sequence, PathSequencePowerline.SequenceRule rule)
	{
		int num = 0;
		for (int i = sequence.Count - 1; i >= 0; i--)
		{
			PathSequencePowerline pathSequencePowerline = sequence[i].Attribute.Find<PathSequence>(sequence[i].ID) as PathSequencePowerline;
			if (pathSequencePowerline != null)
			{
				if (pathSequencePowerline.Rule == rule)
				{
					break;
				}
				num++;
			}
		}
		return num;
	}

	// Token: 0x040025D4 RID: 9684
	public PathSequencePowerline.SequenceRule Rule;

	// Token: 0x040025D5 RID: 9685
	private const int RegularPowerlineSpacing = 2;

	// Token: 0x02000D5D RID: 3421
	public enum SequenceRule
	{
		// Token: 0x04004643 RID: 17987
		PowerlinePlatform,
		// Token: 0x04004644 RID: 17988
		Powerline
	}
}
