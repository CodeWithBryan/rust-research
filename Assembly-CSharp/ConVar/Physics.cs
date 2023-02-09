using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000A90 RID: 2704
	[ConsoleSystem.Factory("physics")]
	public class Physics : ConsoleSystem
	{
		// Token: 0x17000554 RID: 1364
		// (get) Token: 0x0600406D RID: 16493 RVA: 0x0017B960 File Offset: 0x00179B60
		// (set) Token: 0x0600406E RID: 16494 RVA: 0x0017B967 File Offset: 0x00179B67
		[ServerVar]
		public static float bouncethreshold
		{
			get
			{
				return Physics.bounceThreshold;
			}
			set
			{
				Physics.bounceThreshold = value;
			}
		}

		// Token: 0x17000555 RID: 1365
		// (get) Token: 0x0600406F RID: 16495 RVA: 0x0017B96F File Offset: 0x00179B6F
		// (set) Token: 0x06004070 RID: 16496 RVA: 0x0017B976 File Offset: 0x00179B76
		[ServerVar]
		public static float sleepthreshold
		{
			get
			{
				return Physics.sleepThreshold;
			}
			set
			{
				Physics.sleepThreshold = value;
			}
		}

		// Token: 0x17000556 RID: 1366
		// (get) Token: 0x06004071 RID: 16497 RVA: 0x0017B97E File Offset: 0x00179B7E
		// (set) Token: 0x06004072 RID: 16498 RVA: 0x0017B985 File Offset: 0x00179B85
		[ServerVar(Help = "The default solver iteration count permitted for any rigid bodies (default 7). Must be positive")]
		public static int solveriterationcount
		{
			get
			{
				return Physics.defaultSolverIterations;
			}
			set
			{
				Physics.defaultSolverIterations = value;
			}
		}

		// Token: 0x17000557 RID: 1367
		// (get) Token: 0x06004073 RID: 16499 RVA: 0x0017B98D File Offset: 0x00179B8D
		// (set) Token: 0x06004074 RID: 16500 RVA: 0x0017B99F File Offset: 0x00179B9F
		[ServerVar(Help = "Gravity multiplier")]
		public static float gravity
		{
			get
			{
				return Physics.gravity.y / -9.81f;
			}
			set
			{
				Physics.gravity = new Vector3(0f, value * -9.81f, 0f);
			}
		}

		// Token: 0x06004075 RID: 16501 RVA: 0x0017B9BC File Offset: 0x00179BBC
		internal static void ApplyDropped(Rigidbody rigidBody)
		{
			if (Physics.droppedmode <= 0)
			{
				rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			}
			if (Physics.droppedmode == 1)
			{
				rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
			}
			if (Physics.droppedmode == 2)
			{
				rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			}
			if (Physics.droppedmode >= 3)
			{
				rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			}
		}

		// Token: 0x17000558 RID: 1368
		// (get) Token: 0x06004076 RID: 16502 RVA: 0x0017B9FA File Offset: 0x00179BFA
		// (set) Token: 0x06004077 RID: 16503 RVA: 0x0017BA07 File Offset: 0x00179C07
		[ClientVar(ClientAdmin = true)]
		[ServerVar(Help = "The amount of physics steps per second")]
		public static float steps
		{
			get
			{
				return 1f / Time.fixedDeltaTime;
			}
			set
			{
				if (value < 10f)
				{
					value = 10f;
				}
				if (value > 60f)
				{
					value = 60f;
				}
				Time.fixedDeltaTime = 1f / value;
			}
		}

		// Token: 0x17000559 RID: 1369
		// (get) Token: 0x06004078 RID: 16504 RVA: 0x0017BA33 File Offset: 0x00179C33
		// (set) Token: 0x06004079 RID: 16505 RVA: 0x0017BA40 File Offset: 0x00179C40
		[ClientVar(ClientAdmin = true)]
		[ServerVar(Help = "The slowest physics steps will operate")]
		public static float minsteps
		{
			get
			{
				return 1f / Time.maximumDeltaTime;
			}
			set
			{
				if (value < 1f)
				{
					value = 1f;
				}
				if (value > 60f)
				{
					value = 60f;
				}
				Time.maximumDeltaTime = 1f / value;
			}
		}

		// Token: 0x1700055A RID: 1370
		// (get) Token: 0x0600407A RID: 16506 RVA: 0x0017BA6C File Offset: 0x00179C6C
		// (set) Token: 0x0600407B RID: 16507 RVA: 0x0017BA73 File Offset: 0x00179C73
		[ClientVar]
		[ServerVar]
		public static bool autosynctransforms
		{
			get
			{
				return Physics.autoSyncTransforms;
			}
			set
			{
				Physics.autoSyncTransforms = value;
			}
		}

		// Token: 0x04003979 RID: 14713
		private const float baseGravity = -9.81f;

		// Token: 0x0400397A RID: 14714
		[ServerVar(Help = "The collision detection mode that dropped items and corpses should use")]
		public static int droppedmode = 2;

		// Token: 0x0400397B RID: 14715
		[ServerVar(Help = "Send effects to clients when physics objects collide")]
		public static bool sendeffects = true;

		// Token: 0x0400397C RID: 14716
		[ServerVar]
		public static bool groundwatchdebug = false;

		// Token: 0x0400397D RID: 14717
		[ServerVar]
		public static int groundwatchfails = 1;

		// Token: 0x0400397E RID: 14718
		[ServerVar]
		public static float groundwatchdelay = 0.1f;

		// Token: 0x0400397F RID: 14719
		[ClientVar]
		[ServerVar]
		public static bool batchsynctransforms = true;
	}
}
