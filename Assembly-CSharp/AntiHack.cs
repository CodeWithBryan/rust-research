using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ConVar;
using Epic.OnlineServices;
using Epic.OnlineServices.Reports;
using UnityEngine;

// Token: 0x02000710 RID: 1808
public static class AntiHack
{
	// Token: 0x060031E8 RID: 12776 RVA: 0x0013237D File Offset: 0x0013057D
	public static void ResetTimer(BasePlayer ply)
	{
		ply.lastViolationTime = UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x060031E9 RID: 12777 RVA: 0x0013238C File Offset: 0x0013058C
	public static bool ShouldIgnore(BasePlayer ply)
	{
		bool result;
		using (TimeWarning.New("AntiHack.ShouldIgnore", 0))
		{
			if (ply.IsFlying)
			{
				ply.lastAdminCheatTime = UnityEngine.Time.realtimeSinceStartup;
			}
			else if ((ply.IsAdmin || ply.IsDeveloper) && ply.lastAdminCheatTime == 0f)
			{
				ply.lastAdminCheatTime = UnityEngine.Time.realtimeSinceStartup;
			}
			if (ply.IsAdmin)
			{
				if (ConVar.AntiHack.userlevel < 1)
				{
					return true;
				}
				if (ConVar.AntiHack.admincheat && ply.UsedAdminCheat(2f))
				{
					return true;
				}
			}
			if (ply.IsDeveloper)
			{
				if (ConVar.AntiHack.userlevel < 2)
				{
					return true;
				}
				if (ConVar.AntiHack.admincheat && ply.UsedAdminCheat(2f))
				{
					return true;
				}
			}
			if (ply.IsSpectating())
			{
				result = true;
			}
			else
			{
				result = false;
			}
		}
		return result;
	}

	// Token: 0x060031EA RID: 12778 RVA: 0x00132468 File Offset: 0x00130668
	public static bool ValidateMove(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		bool result;
		using (TimeWarning.New("AntiHack.ValidateMove", 0))
		{
			if (global::AntiHack.ShouldIgnore(ply))
			{
				result = true;
			}
			else
			{
				bool flag = deltaTime > ConVar.AntiHack.maxdeltatime;
				if (global::AntiHack.IsNoClipping(ply, ticks, deltaTime))
				{
					if (flag)
					{
						return false;
					}
					global::AntiHack.AddViolation(ply, AntiHackType.NoClip, ConVar.AntiHack.noclip_penalty * ticks.Length);
					if (ConVar.AntiHack.noclip_reject)
					{
						return false;
					}
				}
				if (global::AntiHack.IsSpeeding(ply, ticks, deltaTime))
				{
					if (flag)
					{
						return false;
					}
					global::AntiHack.AddViolation(ply, AntiHackType.SpeedHack, ConVar.AntiHack.speedhack_penalty * ticks.Length);
					if (ConVar.AntiHack.speedhack_reject)
					{
						return false;
					}
				}
				if (global::AntiHack.IsFlying(ply, ticks, deltaTime))
				{
					if (flag)
					{
						return false;
					}
					global::AntiHack.AddViolation(ply, AntiHackType.FlyHack, ConVar.AntiHack.flyhack_penalty * ticks.Length);
					if (ConVar.AntiHack.flyhack_reject)
					{
						return false;
					}
				}
				result = true;
			}
		}
		return result;
	}

	// Token: 0x060031EB RID: 12779 RVA: 0x00132548 File Offset: 0x00130748
	public static void ValidateEyeHistory(BasePlayer ply)
	{
		using (TimeWarning.New("AntiHack.ValidateEyeHistory", 0))
		{
			for (int i = 0; i < ply.eyeHistory.Count; i++)
			{
				Vector3 point = ply.eyeHistory[i];
				if (ply.tickHistory.Distance(ply, point) > ConVar.AntiHack.eye_history_forgiveness)
				{
					global::AntiHack.AddViolation(ply, AntiHackType.EyeHack, ConVar.AntiHack.eye_history_penalty);
				}
			}
			ply.eyeHistory.Clear();
		}
	}

	// Token: 0x060031EC RID: 12780 RVA: 0x001325CC File Offset: 0x001307CC
	public static bool IsInsideTerrain(BasePlayer ply)
	{
		bool result;
		using (TimeWarning.New("AntiHack.IsInsideTerrain", 0))
		{
			result = global::AntiHack.TestInsideTerrain(ply.transform.position);
		}
		return result;
	}

	// Token: 0x060031ED RID: 12781 RVA: 0x00132614 File Offset: 0x00130814
	public static bool TestInsideTerrain(Vector3 pos)
	{
		if (!TerrainMeta.Terrain)
		{
			return false;
		}
		if (!TerrainMeta.HeightMap)
		{
			return false;
		}
		if (!TerrainMeta.Collision)
		{
			return false;
		}
		float terrain_padding = ConVar.AntiHack.terrain_padding;
		float height = TerrainMeta.HeightMap.GetHeight(pos);
		if (pos.y > height - terrain_padding)
		{
			return false;
		}
		float num = TerrainMeta.Position.y + TerrainMeta.Terrain.SampleHeight(pos);
		return pos.y <= num - terrain_padding && !TerrainMeta.Collision.GetIgnore(pos, 0.01f);
	}

	// Token: 0x060031EE RID: 12782 RVA: 0x001326A4 File Offset: 0x001308A4
	public static bool IsNoClipping(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		bool result;
		using (TimeWarning.New("AntiHack.IsNoClipping", 0))
		{
			ply.vehiclePauseTime = Mathf.Max(0f, ply.vehiclePauseTime - deltaTime);
			if (ConVar.AntiHack.noclip_protection <= 0)
			{
				result = false;
			}
			else
			{
				ticks.Reset();
				if (!ticks.HasNext())
				{
					result = false;
				}
				else
				{
					bool flag = ply.transform.parent == null;
					Matrix4x4 matrix4x = flag ? Matrix4x4.identity : ply.transform.parent.localToWorldMatrix;
					Vector3 a = flag ? ticks.StartPoint : matrix4x.MultiplyPoint3x4(ticks.StartPoint);
					Vector3 vector = flag ? ticks.EndPoint : matrix4x.MultiplyPoint3x4(ticks.EndPoint);
					Vector3 b = ply.NoClipOffset();
					float radius = ply.NoClipRadius(ConVar.AntiHack.noclip_margin);
					float noclip_backtracking = ConVar.AntiHack.noclip_backtracking;
					bool vehicleLayer = ply.vehiclePauseTime <= 0f;
					if (ConVar.AntiHack.noclip_protection >= 3)
					{
						float num = Mathf.Max(ConVar.AntiHack.noclip_stepsize, 0.1f);
						int num2 = Mathf.Max(ConVar.AntiHack.noclip_maxsteps, 1);
						num = Mathf.Max(ticks.Length / (float)num2, num);
						while (ticks.MoveNext(num))
						{
							vector = (flag ? ticks.CurrentPoint : matrix4x.MultiplyPoint3x4(ticks.CurrentPoint));
							if (global::AntiHack.TestNoClipping(ply, a + b, vector + b, radius, noclip_backtracking, true, vehicleLayer, null))
							{
								return true;
							}
							a = vector;
						}
					}
					else if (ConVar.AntiHack.noclip_protection >= 2)
					{
						if (global::AntiHack.TestNoClipping(ply, a + b, vector + b, radius, noclip_backtracking, true, vehicleLayer, null))
						{
							return true;
						}
					}
					else if (global::AntiHack.TestNoClipping(ply, a + b, vector + b, radius, noclip_backtracking, false, vehicleLayer, null))
					{
						return true;
					}
					result = false;
				}
			}
		}
		return result;
	}

	// Token: 0x060031EF RID: 12783 RVA: 0x00132898 File Offset: 0x00130A98
	public static bool TestNoClipping(BasePlayer ply, Vector3 oldPos, Vector3 newPos, float radius, float backtracking, bool sphereCast, bool vehicleLayer = false, BaseEntity ignoreEntity = null)
	{
		int num = 429990145;
		if (!vehicleLayer)
		{
			num &= -8193;
		}
		Vector3 normalized = (newPos - oldPos).normalized;
		Vector3 vector = oldPos - normalized * backtracking;
		float magnitude = (newPos - vector).magnitude;
		Ray ray = new Ray(vector, normalized);
		RaycastHit hitInfo;
		bool flag = (ignoreEntity == null) ? UnityEngine.Physics.Raycast(ray, out hitInfo, magnitude + radius, num, QueryTriggerInteraction.Ignore) : GamePhysics.Trace(ray, 0f, out hitInfo, magnitude + radius, num, QueryTriggerInteraction.Ignore, ignoreEntity);
		if (!flag && sphereCast)
		{
			flag = ((ignoreEntity == null) ? UnityEngine.Physics.SphereCast(ray, radius, out hitInfo, magnitude, num, QueryTriggerInteraction.Ignore) : GamePhysics.Trace(ray, radius, out hitInfo, magnitude, num, QueryTriggerInteraction.Ignore, ignoreEntity));
		}
		return flag && GamePhysics.Verify(hitInfo, null);
	}

	// Token: 0x060031F0 RID: 12784 RVA: 0x00132964 File Offset: 0x00130B64
	public static bool IsSpeeding(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		bool result;
		using (TimeWarning.New("AntiHack.IsSpeeding", 0))
		{
			ply.speedhackPauseTime = Mathf.Max(0f, ply.speedhackPauseTime - deltaTime);
			if (ConVar.AntiHack.speedhack_protection <= 0)
			{
				result = false;
			}
			else
			{
				bool flag = ply.transform.parent == null;
				Matrix4x4 matrix4x = flag ? Matrix4x4.identity : ply.transform.parent.localToWorldMatrix;
				Vector3 vector = flag ? ticks.StartPoint : matrix4x.MultiplyPoint3x4(ticks.StartPoint);
				Vector3 a = flag ? ticks.EndPoint : matrix4x.MultiplyPoint3x4(ticks.EndPoint);
				float running = 1f;
				float ducking = 0f;
				float crawling = 0f;
				if (ConVar.AntiHack.speedhack_protection >= 2)
				{
					bool flag2 = ply.IsRunning();
					bool flag3 = ply.IsDucked();
					bool flag4 = ply.IsSwimming();
					bool flag5 = ply.IsCrawling();
					running = (flag2 ? 1f : 0f);
					ducking = ((flag3 || flag4) ? 1f : 0f);
					crawling = (flag5 ? 1f : 0f);
				}
				float speed = ply.GetSpeed(running, ducking, crawling);
				Vector3 v = a - vector;
				float num = v.Magnitude2D();
				float num2 = deltaTime * speed;
				if (num > num2)
				{
					Vector3 v2 = TerrainMeta.HeightMap ? TerrainMeta.HeightMap.GetNormal(vector) : Vector3.up;
					float num3 = Mathf.Max(0f, Vector3.Dot(v2.XZ3D(), v.XZ3D())) * ConVar.AntiHack.speedhack_slopespeed * deltaTime;
					num = Mathf.Max(0f, num - num3);
				}
				float num4 = Mathf.Max((ply.speedhackPauseTime > 0f) ? ConVar.AntiHack.speedhack_forgiveness_inertia : ConVar.AntiHack.speedhack_forgiveness, 0.1f);
				float num5 = num4 + Mathf.Max(ConVar.AntiHack.speedhack_forgiveness, 0.1f);
				ply.speedhackDistance = Mathf.Clamp(ply.speedhackDistance, -num5, num5);
				ply.speedhackDistance = Mathf.Clamp(ply.speedhackDistance - num2, -num5, num5);
				if (ply.speedhackDistance > num4)
				{
					result = true;
				}
				else
				{
					ply.speedhackDistance = Mathf.Clamp(ply.speedhackDistance + num, -num5, num5);
					if (ply.speedhackDistance > num4)
					{
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x060031F1 RID: 12785 RVA: 0x00132BC0 File Offset: 0x00130DC0
	public static bool IsFlying(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		bool result;
		using (TimeWarning.New("AntiHack.IsFlying", 0))
		{
			ply.flyhackPauseTime = Mathf.Max(0f, ply.flyhackPauseTime - deltaTime);
			if (ConVar.AntiHack.flyhack_protection <= 0)
			{
				result = false;
			}
			else
			{
				ticks.Reset();
				if (!ticks.HasNext())
				{
					result = false;
				}
				else
				{
					bool flag = ply.transform.parent == null;
					Matrix4x4 matrix4x = flag ? Matrix4x4.identity : ply.transform.parent.localToWorldMatrix;
					Vector3 oldPos = flag ? ticks.StartPoint : matrix4x.MultiplyPoint3x4(ticks.StartPoint);
					Vector3 vector = flag ? ticks.EndPoint : matrix4x.MultiplyPoint3x4(ticks.EndPoint);
					if (ConVar.AntiHack.flyhack_protection >= 3)
					{
						float num = Mathf.Max(ConVar.AntiHack.flyhack_stepsize, 0.1f);
						int num2 = Mathf.Max(ConVar.AntiHack.flyhack_maxsteps, 1);
						num = Mathf.Max(ticks.Length / (float)num2, num);
						while (ticks.MoveNext(num))
						{
							vector = (flag ? ticks.CurrentPoint : matrix4x.MultiplyPoint3x4(ticks.CurrentPoint));
							if (global::AntiHack.TestFlying(ply, oldPos, vector, true))
							{
								return true;
							}
							oldPos = vector;
						}
					}
					else if (ConVar.AntiHack.flyhack_protection >= 2)
					{
						if (global::AntiHack.TestFlying(ply, oldPos, vector, true))
						{
							return true;
						}
					}
					else if (global::AntiHack.TestFlying(ply, oldPos, vector, false))
					{
						return true;
					}
					result = false;
				}
			}
		}
		return result;
	}

	// Token: 0x060031F2 RID: 12786 RVA: 0x00132D44 File Offset: 0x00130F44
	public static bool TestFlying(BasePlayer ply, Vector3 oldPos, Vector3 newPos, bool verifyGrounded)
	{
		ply.isInAir = false;
		ply.isOnPlayer = false;
		if (verifyGrounded)
		{
			float flyhack_extrusion = ConVar.AntiHack.flyhack_extrusion;
			Vector3 vector = (oldPos + newPos) * 0.5f;
			if (!ply.OnLadder() && !WaterLevel.Test(vector - new Vector3(0f, flyhack_extrusion, 0f), true, ply) && (EnvironmentManager.Get(vector) & EnvironmentType.Elevator) == (EnvironmentType)0)
			{
				float flyhack_margin = ConVar.AntiHack.flyhack_margin;
				float radius = ply.GetRadius();
				float height = ply.GetHeight(false);
				Vector3 vector2 = vector + new Vector3(0f, radius - flyhack_extrusion, 0f);
				Vector3 vector3 = vector + new Vector3(0f, height - radius, 0f);
				float radius2 = radius - flyhack_margin;
				ply.isInAir = !UnityEngine.Physics.CheckCapsule(vector2, vector3, radius2, 1503731969, QueryTriggerInteraction.Ignore);
				if (ply.isInAir)
				{
					int num = UnityEngine.Physics.OverlapCapsuleNonAlloc(vector2, vector3, radius2, global::AntiHack.buffer, 131072, QueryTriggerInteraction.Ignore);
					for (int i = 0; i < num; i++)
					{
						BasePlayer basePlayer = global::AntiHack.buffer[i].gameObject.ToBaseEntity() as BasePlayer;
						if (!(basePlayer == null) && !(basePlayer == ply) && !basePlayer.isInAir && !basePlayer.isOnPlayer && !basePlayer.TriggeredAntiHack(1f, float.PositiveInfinity) && !basePlayer.IsSleeping())
						{
							ply.isOnPlayer = true;
							ply.isInAir = false;
							break;
						}
					}
					for (int j = 0; j < global::AntiHack.buffer.Length; j++)
					{
						global::AntiHack.buffer[j] = null;
					}
				}
			}
		}
		else
		{
			ply.isInAir = (!ply.OnLadder() && !ply.IsSwimming() && !ply.IsOnGround());
		}
		if (ply.isInAir)
		{
			bool flag = false;
			Vector3 vector4 = newPos - oldPos;
			float num2 = Mathf.Abs(vector4.y);
			float num3 = vector4.Magnitude2D();
			if (vector4.y >= 0f)
			{
				ply.flyhackDistanceVertical += vector4.y;
				flag = true;
			}
			if (num2 < num3)
			{
				ply.flyhackDistanceHorizontal += num3;
				flag = true;
			}
			if (flag)
			{
				float num4 = Mathf.Max((ply.flyhackPauseTime > 0f) ? ConVar.AntiHack.flyhack_forgiveness_vertical_inertia : ConVar.AntiHack.flyhack_forgiveness_vertical, 0f);
				float num5 = ply.GetJumpHeight() + num4;
				if (ply.flyhackDistanceVertical > num5)
				{
					return true;
				}
				float num6 = Mathf.Max((ply.flyhackPauseTime > 0f) ? ConVar.AntiHack.flyhack_forgiveness_horizontal_inertia : ConVar.AntiHack.flyhack_forgiveness_horizontal, 0f);
				float num7 = 5f + num6;
				if (ply.flyhackDistanceHorizontal > num7)
				{
					return true;
				}
			}
		}
		else
		{
			ply.flyhackDistanceVertical = 0f;
			ply.flyhackDistanceHorizontal = 0f;
		}
		return false;
	}

	// Token: 0x060031F3 RID: 12787 RVA: 0x00133000 File Offset: 0x00131200
	public static bool TestIsBuildingInsideSomething(Construction.Target target, Vector3 deployPos)
	{
		if (ConVar.AntiHack.build_inside_check <= 0)
		{
			return false;
		}
		bool queriesHitBackfaces = UnityEngine.Physics.queriesHitBackfaces;
		UnityEngine.Physics.queriesHitBackfaces = true;
		if (global::AntiHack.<TestIsBuildingInsideSomething>g__IsInside|19_0(deployPos) && global::AntiHack.<TestIsBuildingInsideSomething>g__IsInside|19_0(target.ray.origin))
		{
			global::AntiHack.LogToConsole(target.player, AntiHackType.InsideTerrain, "Tried to build while clipped inside " + global::AntiHack.buildRayHit.collider.name);
			if (ConVar.AntiHack.build_inside_check > 1)
			{
				return true;
			}
		}
		UnityEngine.Physics.queriesHitBackfaces = queriesHitBackfaces;
		return false;
	}

	// Token: 0x060031F4 RID: 12788 RVA: 0x00133075 File Offset: 0x00131275
	public static void NoteAdminHack(BasePlayer ply)
	{
		global::AntiHack.Ban(ply, "Cheat Detected!");
	}

	// Token: 0x060031F5 RID: 12789 RVA: 0x00133082 File Offset: 0x00131282
	public static void FadeViolations(BasePlayer ply, float deltaTime)
	{
		if (UnityEngine.Time.realtimeSinceStartup - ply.lastViolationTime > ConVar.AntiHack.relaxationpause)
		{
			ply.violationLevel = Mathf.Max(0f, ply.violationLevel - ConVar.AntiHack.relaxationrate * deltaTime);
		}
	}

	// Token: 0x060031F6 RID: 12790 RVA: 0x001330B8 File Offset: 0x001312B8
	public static void EnforceViolations(BasePlayer ply)
	{
		if (ConVar.AntiHack.enforcementlevel <= 0)
		{
			return;
		}
		if (ply.violationLevel > ConVar.AntiHack.maxviolation)
		{
			if (ConVar.AntiHack.debuglevel >= 1)
			{
				global::AntiHack.LogToConsole(ply, ply.lastViolationType, "Enforcing (violation of " + ply.violationLevel + ")");
			}
			string reason = ply.lastViolationType + " Violation Level " + ply.violationLevel;
			if (ConVar.AntiHack.enforcementlevel > 1)
			{
				global::AntiHack.Kick(ply, reason);
				return;
			}
			global::AntiHack.Kick(ply, reason);
		}
	}

	// Token: 0x060031F7 RID: 12791 RVA: 0x00133141 File Offset: 0x00131341
	public static void Log(BasePlayer ply, AntiHackType type, string message)
	{
		if (ConVar.AntiHack.debuglevel > 1)
		{
			global::AntiHack.LogToConsole(ply, type, message);
		}
		global::AntiHack.LogToEAC(ply, type, message);
	}

	// Token: 0x060031F8 RID: 12792 RVA: 0x0013315C File Offset: 0x0013135C
	private static void LogToConsole(BasePlayer ply, AntiHackType type, string message)
	{
		Debug.LogWarning(string.Concat(new object[]
		{
			ply,
			" ",
			type,
			": ",
			message,
			" at ",
			ply.transform.position
		}));
	}

	// Token: 0x060031F9 RID: 12793 RVA: 0x001331B8 File Offset: 0x001313B8
	private static void LogToEAC(BasePlayer ply, AntiHackType type, string message)
	{
		if (ConVar.AntiHack.reporting && EACServer.Reports != null)
		{
			SendPlayerBehaviorReportOptions sendPlayerBehaviorReportOptions = new SendPlayerBehaviorReportOptions
			{
				ReportedUserId = ProductUserId.FromString(ply.UserIDString),
				Category = PlayerReportsCategory.Exploiting,
				Message = type + ": " + message
			};
			EACServer.Reports.SendPlayerBehaviorReport(ref sendPlayerBehaviorReportOptions, null, null);
		}
	}

	// Token: 0x060031FA RID: 12794 RVA: 0x00133230 File Offset: 0x00131430
	public static void AddViolation(BasePlayer ply, AntiHackType type, float amount)
	{
		using (TimeWarning.New("AntiHack.AddViolation", 0))
		{
			ply.lastViolationType = type;
			ply.lastViolationTime = UnityEngine.Time.realtimeSinceStartup;
			ply.violationLevel += amount;
			if ((ConVar.AntiHack.debuglevel >= 2 && amount > 0f) || (ConVar.AntiHack.debuglevel >= 3 && type != AntiHackType.NoClip) || ConVar.AntiHack.debuglevel >= 4)
			{
				global::AntiHack.LogToConsole(ply, type, string.Concat(new object[]
				{
					"Added violation of ",
					amount,
					" in frame ",
					UnityEngine.Time.frameCount,
					" (now has ",
					ply.violationLevel,
					")"
				}));
			}
			global::AntiHack.EnforceViolations(ply);
		}
	}

	// Token: 0x060031FB RID: 12795 RVA: 0x00133308 File Offset: 0x00131508
	public static void Kick(BasePlayer ply, string reason)
	{
		global::AntiHack.AddRecord(ply, global::AntiHack.kicks);
		ConsoleSystem.Run(ConsoleSystem.Option.Server, "kick", new object[]
		{
			ply.userID,
			reason
		});
	}

	// Token: 0x060031FC RID: 12796 RVA: 0x0013333D File Offset: 0x0013153D
	public static void Ban(BasePlayer ply, string reason)
	{
		global::AntiHack.AddRecord(ply, global::AntiHack.bans);
		ConsoleSystem.Run(ConsoleSystem.Option.Server, "ban", new object[]
		{
			ply.userID,
			reason
		});
	}

	// Token: 0x060031FD RID: 12797 RVA: 0x00133374 File Offset: 0x00131574
	private static void AddRecord(BasePlayer ply, Dictionary<ulong, int> records)
	{
		if (records.ContainsKey(ply.userID))
		{
			ulong userID = ply.userID;
			records[userID]++;
			return;
		}
		records.Add(ply.userID, 1);
	}

	// Token: 0x060031FE RID: 12798 RVA: 0x001333B6 File Offset: 0x001315B6
	public static int GetKickRecord(BasePlayer ply)
	{
		return global::AntiHack.GetRecord(ply, global::AntiHack.kicks);
	}

	// Token: 0x060031FF RID: 12799 RVA: 0x001333C3 File Offset: 0x001315C3
	public static int GetBanRecord(BasePlayer ply)
	{
		return global::AntiHack.GetRecord(ply, global::AntiHack.bans);
	}

	// Token: 0x06003200 RID: 12800 RVA: 0x001333D0 File Offset: 0x001315D0
	private static int GetRecord(BasePlayer ply, Dictionary<ulong, int> records)
	{
		if (!records.ContainsKey(ply.userID))
		{
			return 0;
		}
		return records[ply.userID];
	}

	// Token: 0x06003202 RID: 12802 RVA: 0x0013340F File Offset: 0x0013160F
	[CompilerGenerated]
	internal static bool <TestIsBuildingInsideSomething>g__IsInside|19_0(Vector3 pos)
	{
		return UnityEngine.Physics.Raycast(pos, Vector3.up, out global::AntiHack.buildRayHit, 50f, 65537) && Vector3.Dot(Vector3.up, global::AntiHack.buildRayHit.normal) > 0f;
	}

	// Token: 0x040028A9 RID: 10409
	private const int movement_mask = 429990145;

	// Token: 0x040028AA RID: 10410
	private const int grounded_mask = 1503731969;

	// Token: 0x040028AB RID: 10411
	private const int vehicle_mask = 8192;

	// Token: 0x040028AC RID: 10412
	private const int player_mask = 131072;

	// Token: 0x040028AD RID: 10413
	private static Collider[] buffer = new Collider[4];

	// Token: 0x040028AE RID: 10414
	private static Dictionary<ulong, int> kicks = new Dictionary<ulong, int>();

	// Token: 0x040028AF RID: 10415
	private static Dictionary<ulong, int> bans = new Dictionary<ulong, int>();

	// Token: 0x040028B0 RID: 10416
	private static RaycastHit buildRayHit;
}
