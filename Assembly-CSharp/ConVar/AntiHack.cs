using System;

namespace ConVar
{
	// Token: 0x02000A62 RID: 2658
	[ConsoleSystem.Factory("antihack")]
	public class AntiHack : ConsoleSystem
	{
		// Token: 0x04003887 RID: 14471
		[ServerVar]
		[Help("report violations to the anti cheat backend")]
		public static bool reporting = false;

		// Token: 0x04003888 RID: 14472
		[ServerVar]
		[Help("are admins allowed to use their admin cheat")]
		public static bool admincheat = true;

		// Token: 0x04003889 RID: 14473
		[ServerVar]
		[Help("use antihack to verify object placement by players")]
		public static bool objectplacement = true;

		// Token: 0x0400388A RID: 14474
		[ServerVar]
		[Help("use antihack to verify model state sent by players")]
		public static bool modelstate = true;

		// Token: 0x0400388B RID: 14475
		[ServerVar]
		[Help("whether or not to force the position on the client")]
		public static bool forceposition = true;

		// Token: 0x0400388C RID: 14476
		[ServerVar]
		[Help("0 == users, 1 == admins, 2 == developers")]
		public static int userlevel = 2;

		// Token: 0x0400388D RID: 14477
		[ServerVar]
		[Help("0 == no enforcement, 1 == kick, 2 == ban (DISABLED)")]
		public static int enforcementlevel = 1;

		// Token: 0x0400388E RID: 14478
		[ServerVar]
		[Help("max allowed client desync, lower value = more false positives")]
		public static float maxdesync = 1f;

		// Token: 0x0400388F RID: 14479
		[ServerVar]
		[Help("max allowed client tick interval delta time, lower value = more false positives")]
		public static float maxdeltatime = 1f;

		// Token: 0x04003890 RID: 14480
		[ServerVar]
		[Help("for how many seconds to keep a tick history to use for distance checks")]
		public static float tickhistorytime = 0.5f;

		// Token: 0x04003891 RID: 14481
		[ServerVar]
		[Help("how much forgiveness to add when checking the distance from the player tick history")]
		public static float tickhistoryforgiveness = 0.1f;

		// Token: 0x04003892 RID: 14482
		[ServerVar]
		[Help("the rate at which violation values go back down")]
		public static float relaxationrate = 0.1f;

		// Token: 0x04003893 RID: 14483
		[ServerVar]
		[Help("the time before violation values go back down")]
		public static float relaxationpause = 10f;

		// Token: 0x04003894 RID: 14484
		[ServerVar]
		[Help("violation value above this results in enforcement")]
		public static float maxviolation = 100f;

		// Token: 0x04003895 RID: 14485
		[ServerVar]
		[Help("0 == disabled, 1 == enabled")]
		public static int terrain_protection = 1;

		// Token: 0x04003896 RID: 14486
		[ServerVar]
		[Help("how many slices to subdivide players into for the terrain check")]
		public static int terrain_timeslice = 64;

		// Token: 0x04003897 RID: 14487
		[ServerVar]
		[Help("how far to penetrate the terrain before violating")]
		public static float terrain_padding = 0.3f;

		// Token: 0x04003898 RID: 14488
		[ServerVar]
		[Help("violation penalty to hand out when terrain is detected")]
		public static float terrain_penalty = 100f;

		// Token: 0x04003899 RID: 14489
		[ServerVar]
		[Help("whether or not to kill the player when terrain is detected")]
		public static bool terrain_kill = true;

		// Token: 0x0400389A RID: 14490
		[ServerVar]
		[Help("0 == disabled, 1 == ray, 2 == sphere, 3 == curve")]
		public static int noclip_protection = 3;

		// Token: 0x0400389B RID: 14491
		[ServerVar]
		[Help("whether or not to reject movement when noclip is detected")]
		public static bool noclip_reject = true;

		// Token: 0x0400389C RID: 14492
		[ServerVar]
		[Help("violation penalty to hand out when noclip is detected")]
		public static float noclip_penalty = 0f;

		// Token: 0x0400389D RID: 14493
		[ServerVar]
		[Help("collider margin when checking for noclipping")]
		public static float noclip_margin = 0.09f;

		// Token: 0x0400389E RID: 14494
		[ServerVar]
		[Help("collider margin when checking for noclipping on dismount")]
		public static float noclip_margin_dismount = 0.22f;

		// Token: 0x0400389F RID: 14495
		[ServerVar]
		[Help("collider backtracking when checking for noclipping")]
		public static float noclip_backtracking = 0.01f;

		// Token: 0x040038A0 RID: 14496
		[ServerVar]
		[Help("movement curve step size, lower value = less false positives")]
		public static float noclip_stepsize = 0.1f;

		// Token: 0x040038A1 RID: 14497
		[ServerVar]
		[Help("movement curve max steps, lower value = more false positives")]
		public static int noclip_maxsteps = 15;

		// Token: 0x040038A2 RID: 14498
		[ServerVar]
		[Help("0 == disabled, 1 == simple, 2 == advanced")]
		public static int speedhack_protection = 2;

		// Token: 0x040038A3 RID: 14499
		[ServerVar]
		[Help("whether or not to reject movement when speedhack is detected")]
		public static bool speedhack_reject = true;

		// Token: 0x040038A4 RID: 14500
		[ServerVar]
		[Help("violation penalty to hand out when speedhack is detected")]
		public static float speedhack_penalty = 0f;

		// Token: 0x040038A5 RID: 14501
		[ServerVar]
		[Help("speed threshold to assume speedhacking, lower value = more false positives")]
		public static float speedhack_forgiveness = 2f;

		// Token: 0x040038A6 RID: 14502
		[ServerVar]
		[Help("speed threshold to assume speedhacking, lower value = more false positives")]
		public static float speedhack_forgiveness_inertia = 10f;

		// Token: 0x040038A7 RID: 14503
		[ServerVar]
		[Help("speed forgiveness when moving down slopes, lower value = more false positives")]
		public static float speedhack_slopespeed = 10f;

		// Token: 0x040038A8 RID: 14504
		[ServerVar]
		[Help("0 == disabled, 1 == client, 2 == capsule, 3 == curve")]
		public static int flyhack_protection = 3;

		// Token: 0x040038A9 RID: 14505
		[ServerVar]
		[Help("whether or not to reject movement when flyhack is detected")]
		public static bool flyhack_reject = false;

		// Token: 0x040038AA RID: 14506
		[ServerVar]
		[Help("violation penalty to hand out when flyhack is detected")]
		public static float flyhack_penalty = 100f;

		// Token: 0x040038AB RID: 14507
		[ServerVar]
		[Help("distance threshold to assume flyhacking, lower value = more false positives")]
		public static float flyhack_forgiveness_vertical = 1.5f;

		// Token: 0x040038AC RID: 14508
		[ServerVar]
		[Help("distance threshold to assume flyhacking, lower value = more false positives")]
		public static float flyhack_forgiveness_vertical_inertia = 10f;

		// Token: 0x040038AD RID: 14509
		[ServerVar]
		[Help("distance threshold to assume flyhacking, lower value = more false positives")]
		public static float flyhack_forgiveness_horizontal = 1.5f;

		// Token: 0x040038AE RID: 14510
		[ServerVar]
		[Help("distance threshold to assume flyhacking, lower value = more false positives")]
		public static float flyhack_forgiveness_horizontal_inertia = 10f;

		// Token: 0x040038AF RID: 14511
		[ServerVar]
		[Help("collider downwards extrusion when checking for flyhacking")]
		public static float flyhack_extrusion = 2f;

		// Token: 0x040038B0 RID: 14512
		[ServerVar]
		[Help("collider margin when checking for flyhacking")]
		public static float flyhack_margin = 0.05f;

		// Token: 0x040038B1 RID: 14513
		[ServerVar]
		[Help("movement curve step size, lower value = less false positives")]
		public static float flyhack_stepsize = 0.1f;

		// Token: 0x040038B2 RID: 14514
		[ServerVar]
		[Help("movement curve max steps, lower value = more false positives")]
		public static int flyhack_maxsteps = 15;

		// Token: 0x040038B3 RID: 14515
		[ServerVar]
		[Help("0 == disabled, 1 == speed, 2 == speed + entity, 3 == speed + entity + LOS, 4 == speed + entity + LOS + trajectory, 5 == speed + entity + LOS + trajectory + update, 6 == speed + entity + LOS + trajectory + tickhistory")]
		public static int projectile_protection = 6;

		// Token: 0x040038B4 RID: 14516
		[ServerVar]
		[Help("violation penalty to hand out when projectile hack is detected")]
		public static float projectile_penalty = 0f;

		// Token: 0x040038B5 RID: 14517
		[ServerVar]
		[Help("projectile speed forgiveness in percent, lower value = more false positives")]
		public static float projectile_forgiveness = 0.5f;

		// Token: 0x040038B6 RID: 14518
		[ServerVar]
		[Help("projectile server frames to include in delay, lower value = more false positives")]
		public static float projectile_serverframes = 2f;

		// Token: 0x040038B7 RID: 14519
		[ServerVar]
		[Help("projectile client frames to include in delay, lower value = more false positives")]
		public static float projectile_clientframes = 2f;

		// Token: 0x040038B8 RID: 14520
		[ServerVar]
		[Help("projectile trajectory forgiveness, lower value = more false positives")]
		public static float projectile_trajectory = 1f;

		// Token: 0x040038B9 RID: 14521
		[ServerVar]
		[Help("projectile penetration angle change, lower value = more false positives")]
		public static float projectile_anglechange = 60f;

		// Token: 0x040038BA RID: 14522
		[ServerVar]
		[Help("projectile penetration velocity change, lower value = more false positives")]
		public static float projectile_velocitychange = 1.1f;

		// Token: 0x040038BB RID: 14523
		[ServerVar]
		[Help("projectile desync forgiveness, lower value = more false positives")]
		public static float projectile_desync = 1f;

		// Token: 0x040038BC RID: 14524
		[ServerVar]
		[Help("projectile backtracking when checking for LOS")]
		public static float projectile_backtracking = 0.01f;

		// Token: 0x040038BD RID: 14525
		[ServerVar]
		[Help("line of sight directional forgiveness when checking eye or center position")]
		public static float projectile_losforgiveness = 0.2f;

		// Token: 0x040038BE RID: 14526
		[ServerVar]
		[Help("how often a projectile is allowed to penetrate something before its damage is ignored")]
		public static int projectile_damagedepth = 2;

		// Token: 0x040038BF RID: 14527
		[ServerVar]
		[Help("how often a projectile is allowed to penetrate something before its impact spawn is ignored")]
		public static int projectile_impactspawndepth = 1;

		// Token: 0x040038C0 RID: 14528
		[ServerVar]
		[Help("whether or not to include terrain in the projectile LOS checks")]
		public static bool projectile_terraincheck = true;

		// Token: 0x040038C1 RID: 14529
		[ServerVar]
		[Help("0 == disabled, 1 == initiator, 2 == initiator + target, 3 == initiator + target + LOS, 4 == initiator + target + LOS + tickhistory")]
		public static int melee_protection = 4;

		// Token: 0x040038C2 RID: 14530
		[ServerVar]
		[Help("violation penalty to hand out when melee hack is detected")]
		public static float melee_penalty = 0f;

		// Token: 0x040038C3 RID: 14531
		[ServerVar]
		[Help("melee distance forgiveness in percent, lower value = more false positives")]
		public static float melee_forgiveness = 0.5f;

		// Token: 0x040038C4 RID: 14532
		[ServerVar]
		[Help("melee server frames to include in delay, lower value = more false positives")]
		public static float melee_serverframes = 2f;

		// Token: 0x040038C5 RID: 14533
		[ServerVar]
		[Help("melee client frames to include in delay, lower value = more false positives")]
		public static float melee_clientframes = 2f;

		// Token: 0x040038C6 RID: 14534
		[ServerVar]
		[Help("line of sight directional forgiveness when checking eye or center position")]
		public static float melee_losforgiveness = 0.2f;

		// Token: 0x040038C7 RID: 14535
		[ServerVar]
		[Help("whether or not to include terrain in the melee LOS checks")]
		public static bool melee_terraincheck = true;

		// Token: 0x040038C8 RID: 14536
		[ServerVar]
		[Help("0 == disabled, 1 == distance, 2 == distance + LOS, 3 = distance + LOS + altitude, 4 = distance + LOS + altitude + noclip, 5 = distance + LOS + altitude + noclip + history")]
		public static int eye_protection = 4;

		// Token: 0x040038C9 RID: 14537
		[ServerVar]
		[Help("violation penalty to hand out when eye hack is detected")]
		public static float eye_penalty = 0f;

		// Token: 0x040038CA RID: 14538
		[ServerVar]
		[Help("eye speed forgiveness in percent, lower value = more false positives")]
		public static float eye_forgiveness = 0.5f;

		// Token: 0x040038CB RID: 14539
		[ServerVar]
		[Help("eye server frames to include in delay, lower value = more false positives")]
		public static float eye_serverframes = 2f;

		// Token: 0x040038CC RID: 14540
		[ServerVar]
		[Help("eye client frames to include in delay, lower value = more false positives")]
		public static float eye_clientframes = 2f;

		// Token: 0x040038CD RID: 14541
		[ServerVar]
		[Help("whether or not to include terrain in the eye LOS checks")]
		public static bool eye_terraincheck = true;

		// Token: 0x040038CE RID: 14542
		[ServerVar]
		[Help("distance at which to start testing eye noclipping")]
		public static float eye_noclip_cutoff = 0.06f;

		// Token: 0x040038CF RID: 14543
		[ServerVar]
		[Help("collider margin when checking for noclipping")]
		public static float eye_noclip_margin = 0.21f;

		// Token: 0x040038D0 RID: 14544
		[ServerVar]
		[Help("collider backtracking when checking for noclipping")]
		public static float eye_noclip_backtracking = 0.01f;

		// Token: 0x040038D1 RID: 14545
		[ServerVar]
		[Help("line of sight sphere cast radius, 0 == raycast")]
		public static float eye_losradius = 0.18f;

		// Token: 0x040038D2 RID: 14546
		[ServerVar]
		[Help("violation penalty to hand out when eye history mismatch is detected")]
		public static float eye_history_penalty = 100f;

		// Token: 0x040038D3 RID: 14547
		[ServerVar]
		[Help("how much forgiveness to add when checking the distance between player tick history and player eye history")]
		public static float eye_history_forgiveness = 0.1f;

		// Token: 0x040038D4 RID: 14548
		[ServerVar]
		[Help("line of sight sphere cast radius, 0 == raycast")]
		public static float build_losradius = 0.01f;

		// Token: 0x040038D5 RID: 14549
		[ServerVar]
		[Help("line of sight sphere cast radius, 0 == raycast")]
		public static float build_losradius_sleepingbag = 0.3f;

		// Token: 0x040038D6 RID: 14550
		[ServerVar]
		[Help("whether or not to include terrain in the build LOS checks")]
		public static bool build_terraincheck = true;

		// Token: 0x040038D7 RID: 14551
		[ServerVar]
		[Help("whether or not to check for building being done on the wrong side of something (e.g. inside rocks). 0 = Disabled, 1 = Info only, 2 = Enabled")]
		public static int build_inside_check = 2;

		// Token: 0x040038D8 RID: 14552
		[ServerVar]
		[Help("0 == silent, 1 == print max violation, 2 == print nonzero violation, 3 == print any violation except noclip, 4 == print any violation")]
		public static int debuglevel = 1;
	}
}
