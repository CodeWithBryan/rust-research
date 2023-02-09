using System;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200053A RID: 1338
[CreateAssetMenu(menuName = "Rust/Convar Controlled Spawn Population")]
public class ConvarControlledSpawnPopulation : SpawnPopulation
{
	// Token: 0x17000336 RID: 822
	// (get) Token: 0x060028D3 RID: 10451 RVA: 0x000F8C09 File Offset: 0x000F6E09
	protected ConsoleSystem.Command Command
	{
		get
		{
			if (this._command == null)
			{
				this._command = ConsoleSystem.Index.Server.Find(this.PopulationConvar);
				Assert.IsNotNull<ConsoleSystem.Command>(this._command, string.Format("{0} has missing convar {1}", this, this.PopulationConvar));
			}
			return this._command;
		}
	}

	// Token: 0x17000337 RID: 823
	// (get) Token: 0x060028D4 RID: 10452 RVA: 0x000F8C46 File Offset: 0x000F6E46
	public override float TargetDensity
	{
		get
		{
			return this.Command.AsFloat;
		}
	}

	// Token: 0x04002127 RID: 8487
	[Header("Convars")]
	public string PopulationConvar;

	// Token: 0x04002128 RID: 8488
	private ConsoleSystem.Command _command;
}
