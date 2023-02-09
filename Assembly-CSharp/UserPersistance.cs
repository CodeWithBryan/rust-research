using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Facepunch;
using Facepunch.Math;
using Facepunch.Sqlite;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200071C RID: 1820
public class UserPersistance : IDisposable
{
	// Token: 0x06003288 RID: 12936 RVA: 0x00138674 File Offset: 0x00136874
	public UserPersistance(string strFolder)
	{
		UserPersistance.blueprints = new Facepunch.Sqlite.Database();
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		string arg = strFolder + "/player.blueprints.";
		if (activeGameMode != null && activeGameMode.wipeBpsOnProtocol)
		{
			arg = arg + 233 + ".";
		}
		UserPersistance.blueprints.Open(arg + 5 + ".db", false);
		if (!UserPersistance.blueprints.TableExists("data"))
		{
			UserPersistance.blueprints.Execute("CREATE TABLE data ( userid TEXT PRIMARY KEY, info BLOB, updated INTEGER )");
		}
		UserPersistance.deaths = new Facepunch.Sqlite.Database();
		UserPersistance.deaths.Open(string.Concat(new object[]
		{
			strFolder,
			"/player.deaths.",
			5,
			".db"
		}), false);
		if (!UserPersistance.deaths.TableExists("data"))
		{
			UserPersistance.deaths.Execute("CREATE TABLE data ( userid TEXT, born INTEGER, died INTEGER, info BLOB )");
			UserPersistance.deaths.Execute("CREATE INDEX IF NOT EXISTS userindex ON data ( userid )");
			UserPersistance.deaths.Execute("CREATE INDEX IF NOT EXISTS diedindex ON data ( died )");
		}
		UserPersistance.identities = new Facepunch.Sqlite.Database();
		UserPersistance.identities.Open(string.Concat(new object[]
		{
			strFolder,
			"/player.identities.",
			5,
			".db"
		}), false);
		if (!UserPersistance.identities.TableExists("data"))
		{
			UserPersistance.identities.Execute("CREATE TABLE data ( userid INT PRIMARY KEY, username TEXT )");
		}
		UserPersistance.tokens = new Facepunch.Sqlite.Database();
		UserPersistance.tokens.Open(strFolder + "/player.tokens.db", false);
		if (!UserPersistance.tokens.TableExists("data"))
		{
			UserPersistance.tokens.Execute("CREATE TABLE data ( userid INT PRIMARY KEY, token INT, locked BOOLEAN DEFAULT 0 )");
		}
		if (!UserPersistance.tokens.ColumnExists("data", "locked"))
		{
			UserPersistance.tokens.Execute("ALTER TABLE data ADD COLUMN locked BOOLEAN DEFAULT 0");
		}
		UserPersistance.playerState = new Facepunch.Sqlite.Database();
		UserPersistance.playerState.Open(string.Concat(new object[]
		{
			strFolder,
			"/player.states.",
			233,
			".db"
		}), false);
		if (!UserPersistance.playerState.TableExists("data"))
		{
			UserPersistance.playerState.Execute("CREATE TABLE data ( userid INT PRIMARY KEY, state BLOB )");
		}
		UserPersistance.nameCache = new Dictionary<ulong, string>();
		UserPersistance.tokenCache = new MruDictionary<ulong, ValueTuple<int, bool>>(500, null);
	}

	// Token: 0x06003289 RID: 12937 RVA: 0x001388C0 File Offset: 0x00136AC0
	public virtual void Dispose()
	{
		if (UserPersistance.blueprints != null)
		{
			UserPersistance.blueprints.Close();
			UserPersistance.blueprints = null;
		}
		if (UserPersistance.deaths != null)
		{
			UserPersistance.deaths.Close();
			UserPersistance.deaths = null;
		}
		if (UserPersistance.identities != null)
		{
			UserPersistance.identities.Close();
			UserPersistance.identities = null;
		}
		if (UserPersistance.tokens != null)
		{
			UserPersistance.tokens.Close();
			UserPersistance.tokens = null;
		}
		if (UserPersistance.playerState != null)
		{
			UserPersistance.playerState.Close();
			UserPersistance.playerState = null;
		}
	}

	// Token: 0x0600328A RID: 12938 RVA: 0x00138940 File Offset: 0x00136B40
	public PersistantPlayer GetPlayerInfo(ulong playerID)
	{
		PersistantPlayer persistantPlayer = this.FetchFromDatabase(playerID);
		if (persistantPlayer == null)
		{
			persistantPlayer = Pool.Get<PersistantPlayer>();
		}
		if (persistantPlayer.unlockedItems == null)
		{
			persistantPlayer.unlockedItems = Pool.GetList<int>();
		}
		return persistantPlayer;
	}

	// Token: 0x0600328B RID: 12939 RVA: 0x00138974 File Offset: 0x00136B74
	private PersistantPlayer FetchFromDatabase(ulong playerID)
	{
		try
		{
			byte[] array = UserPersistance.blueprints.QueryBlob<string>("SELECT info FROM data WHERE userid = ?", playerID.ToString());
			if (array != null)
			{
				return PersistantPlayer.Deserialize(array);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Error loading player blueprints: (" + ex.Message + ")");
		}
		return null;
	}

	// Token: 0x0600328C RID: 12940 RVA: 0x001389D8 File Offset: 0x00136BD8
	public void SetPlayerInfo(ulong playerID, PersistantPlayer info)
	{
		using (TimeWarning.New("SetPlayerInfo", 0))
		{
			byte[] arg;
			using (TimeWarning.New("ToProtoBytes", 0))
			{
				arg = info.ToProtoBytes();
			}
			UserPersistance.blueprints.Execute<string, byte[], int>("INSERT OR REPLACE INTO data ( userid, info, updated ) VALUES ( ?, ?, ? )", playerID.ToString(), arg, Epoch.Current);
		}
	}

	// Token: 0x0600328D RID: 12941 RVA: 0x00138A54 File Offset: 0x00136C54
	public void AddLifeStory(ulong playerID, PlayerLifeStory lifeStory)
	{
		if (UserPersistance.deaths == null)
		{
			return;
		}
		if (lifeStory == null)
		{
			return;
		}
		using (TimeWarning.New("AddLifeStory", 0))
		{
			byte[] arg;
			using (TimeWarning.New("ToProtoBytes", 0))
			{
				arg = lifeStory.ToProtoBytes();
			}
			UserPersistance.deaths.Execute<string, int, int, byte[]>("INSERT INTO data ( userid, born, died, info ) VALUES ( ?, ?, ?, ? )", playerID.ToString(), (int)lifeStory.timeBorn, (int)lifeStory.timeDied, arg);
		}
	}

	// Token: 0x0600328E RID: 12942 RVA: 0x00138AE4 File Offset: 0x00136CE4
	public PlayerLifeStory GetLastLifeStory(ulong playerID)
	{
		if (UserPersistance.deaths == null)
		{
			return null;
		}
		PlayerLifeStory result;
		using (TimeWarning.New("GetLastLifeStory", 0))
		{
			try
			{
				byte[] array = UserPersistance.deaths.QueryBlob<string>("SELECT info FROM data WHERE userid = ? ORDER BY died DESC LIMIT 1", playerID.ToString());
				if (array == null)
				{
					return null;
				}
				PlayerLifeStory playerLifeStory = PlayerLifeStory.Deserialize(array);
				playerLifeStory.ShouldPool = false;
				return playerLifeStory;
			}
			catch (Exception ex)
			{
				Debug.LogError("Error loading lifestory from database: (" + ex.Message + ")");
			}
			result = null;
		}
		return result;
	}

	// Token: 0x0600328F RID: 12943 RVA: 0x00138B7C File Offset: 0x00136D7C
	public string GetPlayerName(ulong playerID)
	{
		if (playerID == 0UL)
		{
			return null;
		}
		string result;
		if (UserPersistance.nameCache.TryGetValue(playerID, out result))
		{
			return result;
		}
		string text = UserPersistance.identities.QueryString<ulong>("SELECT username FROM data WHERE userid = ?", playerID);
		UserPersistance.nameCache[playerID] = text;
		return text;
	}

	// Token: 0x06003290 RID: 12944 RVA: 0x00138BC0 File Offset: 0x00136DC0
	public void SetPlayerName(ulong playerID, string name)
	{
		if (playerID == 0UL || string.IsNullOrEmpty(name))
		{
			return;
		}
		if (string.IsNullOrEmpty(this.GetPlayerName(playerID)))
		{
			UserPersistance.identities.Execute<ulong, string>("INSERT INTO data ( userid, username ) VALUES ( ?, ? )", playerID, name);
		}
		else
		{
			UserPersistance.identities.Execute<string, ulong>("UPDATE data SET username = ? WHERE userid = ?", name, playerID);
		}
		UserPersistance.nameCache[playerID] = name;
	}

	// Token: 0x06003291 RID: 12945 RVA: 0x00138C18 File Offset: 0x00136E18
	public int GetOrGenerateAppToken(ulong playerID, out bool locked)
	{
		if (UserPersistance.tokens == null)
		{
			locked = false;
			return 0;
		}
		int result;
		using (TimeWarning.New("GetOrGenerateAppToken", 0))
		{
			ValueTuple<int, bool> valueTuple;
			if (UserPersistance.tokenCache.TryGetValue(playerID, out valueTuple))
			{
				locked = valueTuple.Item2;
				result = valueTuple.Item1;
			}
			else
			{
				int num = UserPersistance.tokens.QueryInt<ulong>("SELECT token FROM data WHERE userid = ?", playerID);
				if (num != 0)
				{
					bool flag = UserPersistance.tokens.QueryInt<ulong>("SELECT locked FROM data WHERE userid = ?", playerID) != 0;
					UserPersistance.tokenCache.Add(playerID, new ValueTuple<int, bool>(num, flag));
					locked = flag;
					result = num;
				}
				else
				{
					int num2 = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
					UserPersistance.tokens.Execute<ulong, int>("INSERT INTO data ( userid, token ) VALUES ( ?, ? )", playerID, num2);
					UserPersistance.tokenCache.Add(playerID, new ValueTuple<int, bool>(num2, false));
					locked = false;
					result = num2;
				}
			}
		}
		return result;
	}

	// Token: 0x06003292 RID: 12946 RVA: 0x00138D00 File Offset: 0x00136F00
	public bool SetAppTokenLocked(ulong playerID, bool locked)
	{
		if (UserPersistance.tokens == null)
		{
			return false;
		}
		bool flag;
		this.GetOrGenerateAppToken(playerID, out flag);
		if (flag == locked)
		{
			return false;
		}
		UserPersistance.tokens.Execute<int, ulong>("UPDATE data SET locked = ? WHERE userid = ?", locked ? 1 : 0, playerID);
		UserPersistance.tokenCache.Remove(playerID);
		return true;
	}

	// Token: 0x06003293 RID: 12947 RVA: 0x00138D49 File Offset: 0x00136F49
	public byte[] GetPlayerState(ulong playerID)
	{
		if (playerID == 0UL)
		{
			return null;
		}
		return UserPersistance.playerState.QueryBlob<ulong>("SELECT state FROM data WHERE userid = ?", playerID);
	}

	// Token: 0x06003294 RID: 12948 RVA: 0x00138D60 File Offset: 0x00136F60
	public void SetPlayerState(ulong playerID, byte[] state)
	{
		if (playerID == 0UL || state == null)
		{
			return;
		}
		UserPersistance.playerState.Execute<ulong, byte[]>("INSERT OR REPLACE INTO data ( userid, state ) VALUES ( ?, ? )", playerID, state);
	}

	// Token: 0x06003295 RID: 12949 RVA: 0x00138D7A File Offset: 0x00136F7A
	public void ResetPlayerState(ulong playerID)
	{
		if (playerID == 0UL)
		{
			return;
		}
		UserPersistance.playerState.Execute<ulong>("DELETE FROM data WHERE userid = ?", playerID);
	}

	// Token: 0x040028E2 RID: 10466
	private static Facepunch.Sqlite.Database blueprints;

	// Token: 0x040028E3 RID: 10467
	private static Facepunch.Sqlite.Database deaths;

	// Token: 0x040028E4 RID: 10468
	private static Facepunch.Sqlite.Database identities;

	// Token: 0x040028E5 RID: 10469
	private static Facepunch.Sqlite.Database tokens;

	// Token: 0x040028E6 RID: 10470
	private static Facepunch.Sqlite.Database playerState;

	// Token: 0x040028E7 RID: 10471
	private static Dictionary<ulong, string> nameCache;

	// Token: 0x040028E8 RID: 10472
	[TupleElementNames(new string[]
	{
		"Token",
		"Locked"
	})]
	private static MruDictionary<ulong, ValueTuple<int, bool>> tokenCache;
}
