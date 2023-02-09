using System;

namespace ConVar
{
	// Token: 0x02000A7E RID: 2686
	[ConsoleSystem.Factory("halloween")]
	public class Halloween : ConsoleSystem
	{
		// Token: 0x04003961 RID: 14689
		[ServerVar]
		public static bool enabled = false;

		// Token: 0x04003962 RID: 14690
		[ServerVar(Help = "Population active on the server, per square km")]
		public static float murdererpopulation = 0f;

		// Token: 0x04003963 RID: 14691
		[ServerVar(Help = "Population active on the server, per square km")]
		public static float scarecrowpopulation = 0f;

		// Token: 0x04003964 RID: 14692
		[ServerVar(Help = "Scarecrows can throw beancans (Default: true).")]
		public static bool scarecrows_throw_beancans = true;

		// Token: 0x04003965 RID: 14693
		[ServerVar(Help = "The delay globally on a server between each time a scarecrow throws a beancan (Default: 8 seconds).")]
		public static float scarecrow_throw_beancan_global_delay = 8f;

		// Token: 0x04003966 RID: 14694
		[ServerVar(Help = "Modified damage from beancan explosion vs players (Default: 0.1).")]
		public static float scarecrow_beancan_vs_player_dmg_modifier = 0.1f;

		// Token: 0x04003967 RID: 14695
		[ServerVar(Help = "Modifier to how much damage scarecrows take to the body. (Default: 0.25)")]
		public static float scarecrow_body_dmg_modifier = 0.25f;

		// Token: 0x04003968 RID: 14696
		[ServerVar(Help = "Stopping distance for destinations set while chasing a target (Default: 0.5)")]
		public static float scarecrow_chase_stopping_distance = 0.5f;
	}
}
