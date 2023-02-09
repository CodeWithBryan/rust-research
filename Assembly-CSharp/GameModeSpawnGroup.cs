using System;

// Token: 0x02000542 RID: 1346
public class GameModeSpawnGroup : SpawnGroup
{
	// Token: 0x06002906 RID: 10502 RVA: 0x000F9D19 File Offset: 0x000F7F19
	public void ResetSpawnGroup()
	{
		base.Clear();
		this.SpawnInitial();
	}

	// Token: 0x06002907 RID: 10503 RVA: 0x000F9D28 File Offset: 0x000F7F28
	public bool ShouldSpawn()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		return !(activeGameMode == null) && (this.gameModeTags.Length == 0 || activeGameMode.HasAnyGameModeTag(this.gameModeTags));
	}

	// Token: 0x06002908 RID: 10504 RVA: 0x000F9D63 File Offset: 0x000F7F63
	protected override void Spawn(int numToSpawn)
	{
		if (this.ShouldSpawn())
		{
			base.Spawn(numToSpawn);
		}
	}

	// Token: 0x04002151 RID: 8529
	public string[] gameModeTags;
}
