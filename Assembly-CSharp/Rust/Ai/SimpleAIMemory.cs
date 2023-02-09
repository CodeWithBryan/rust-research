using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;

namespace Rust.AI
{
	// Token: 0x02000B06 RID: 2822
	public class SimpleAIMemory
	{
		// Token: 0x060043C9 RID: 17353 RVA: 0x001891C4 File Offset: 0x001873C4
		public void SetKnown(BaseEntity ent, BaseEntity owner, AIBrainSenses brainSenses)
		{
			IAISenses iaisenses = owner as IAISenses;
			BasePlayer basePlayer = ent as BasePlayer;
			if (basePlayer != null && SimpleAIMemory.PlayerIgnoreList.Contains(basePlayer))
			{
				return;
			}
			bool flag = false;
			if (iaisenses != null && iaisenses.IsThreat(ent))
			{
				flag = true;
				if (brainSenses != null)
				{
					brainSenses.LastThreatTimestamp = UnityEngine.Time.realtimeSinceStartup;
				}
			}
			for (int i = 0; i < this.All.Count; i++)
			{
				if (this.All[i].Entity == ent)
				{
					SimpleAIMemory.SeenInfo seenInfo = this.All[i];
					seenInfo.Position = ent.transform.position;
					seenInfo.Timestamp = Mathf.Max(UnityEngine.Time.realtimeSinceStartup, seenInfo.Timestamp);
					this.All[i] = seenInfo;
					return;
				}
			}
			if (basePlayer != null)
			{
				if (AI.ignoreplayers && !basePlayer.IsNpc)
				{
					return;
				}
				this.Players.Add(ent);
			}
			if (iaisenses != null)
			{
				if (iaisenses.IsTarget(ent))
				{
					this.Targets.Add(ent);
				}
				if (iaisenses.IsFriendly(ent))
				{
					this.Friendlies.Add(ent);
				}
				if (flag)
				{
					this.Threats.Add(ent);
				}
			}
			this.All.Add(new SimpleAIMemory.SeenInfo
			{
				Entity = ent,
				Position = ent.transform.position,
				Timestamp = UnityEngine.Time.realtimeSinceStartup
			});
		}

		// Token: 0x060043CA RID: 17354 RVA: 0x00189326 File Offset: 0x00187526
		public void SetLOS(BaseEntity ent, bool flag)
		{
			if (ent == null)
			{
				return;
			}
			if (flag)
			{
				this.LOS.Add(ent);
				return;
			}
			this.LOS.Remove(ent);
		}

		// Token: 0x060043CB RID: 17355 RVA: 0x00189350 File Offset: 0x00187550
		public bool IsLOS(BaseEntity ent)
		{
			return this.LOS.Contains(ent);
		}

		// Token: 0x060043CC RID: 17356 RVA: 0x0018935E File Offset: 0x0018755E
		public bool IsPlayerKnown(BasePlayer player)
		{
			return this.Players.Contains(player);
		}

		// Token: 0x060043CD RID: 17357 RVA: 0x0018936C File Offset: 0x0018756C
		internal void Forget(float secondsOld)
		{
			for (int i = 0; i < this.All.Count; i++)
			{
				if (UnityEngine.Time.realtimeSinceStartup - this.All[i].Timestamp > secondsOld)
				{
					BaseEntity entity = this.All[i].Entity;
					if (entity != null)
					{
						if (entity is BasePlayer)
						{
							this.Players.Remove(entity);
						}
						this.Targets.Remove(entity);
						this.Threats.Remove(entity);
						this.Friendlies.Remove(entity);
						this.LOS.Remove(entity);
					}
					this.All.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x060043CE RID: 17358 RVA: 0x00189423 File Offset: 0x00187623
		public static void AddIgnorePlayer(BasePlayer player)
		{
			if (SimpleAIMemory.PlayerIgnoreList.Contains(player))
			{
				return;
			}
			SimpleAIMemory.PlayerIgnoreList.Add(player);
		}

		// Token: 0x060043CF RID: 17359 RVA: 0x0018943F File Offset: 0x0018763F
		public static void RemoveIgnorePlayer(BasePlayer player)
		{
			SimpleAIMemory.PlayerIgnoreList.Remove(player);
		}

		// Token: 0x060043D0 RID: 17360 RVA: 0x0018944D File Offset: 0x0018764D
		public static void ClearIgnoredPlayers()
		{
			SimpleAIMemory.PlayerIgnoreList.Clear();
		}

		// Token: 0x060043D1 RID: 17361 RVA: 0x0018945C File Offset: 0x0018765C
		public static string GetIgnoredPlayers()
		{
			TextTable textTable = new TextTable();
			textTable.AddColumns(new string[]
			{
				"Name",
				"Steam ID"
			});
			foreach (BasePlayer basePlayer in SimpleAIMemory.PlayerIgnoreList)
			{
				textTable.AddRow(new string[]
				{
					basePlayer.displayName,
					basePlayer.userID.ToString()
				});
			}
			return textTable.ToString();
		}

		// Token: 0x04003C52 RID: 15442
		public static HashSet<BasePlayer> PlayerIgnoreList = new HashSet<BasePlayer>();

		// Token: 0x04003C53 RID: 15443
		public List<SimpleAIMemory.SeenInfo> All = new List<SimpleAIMemory.SeenInfo>();

		// Token: 0x04003C54 RID: 15444
		public List<BaseEntity> Players = new List<BaseEntity>();

		// Token: 0x04003C55 RID: 15445
		public HashSet<BaseEntity> LOS = new HashSet<BaseEntity>();

		// Token: 0x04003C56 RID: 15446
		public List<BaseEntity> Targets = new List<BaseEntity>();

		// Token: 0x04003C57 RID: 15447
		public List<BaseEntity> Threats = new List<BaseEntity>();

		// Token: 0x04003C58 RID: 15448
		public List<BaseEntity> Friendlies = new List<BaseEntity>();

		// Token: 0x02000F3C RID: 3900
		public struct SeenInfo
		{
			// Token: 0x04004DCD RID: 19917
			public BaseEntity Entity;

			// Token: 0x04004DCE RID: 19918
			public Vector3 Position;

			// Token: 0x04004DCF RID: 19919
			public float Timestamp;

			// Token: 0x04004DD0 RID: 19920
			public float Danger;
		}
	}
}
