using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch.Sqlite;
using Ionic.Crc;
using UnityEngine.Assertions;

// Token: 0x0200071F RID: 1823
public class FileStorage : IDisposable
{
	// Token: 0x060032A1 RID: 12961 RVA: 0x00138E90 File Offset: 0x00137090
	protected FileStorage(string name, bool server)
	{
		if (server)
		{
			string path = Server.rootFolder + "/" + name + ".db";
			this.db = new Database();
			this.db.Open(path, true);
			if (!this.db.TableExists("data"))
			{
				this.db.Execute("CREATE TABLE data ( crc INTEGER PRIMARY KEY, data BLOB, updated INTEGER, entid INTEGER, filetype INTEGER, part INTEGER )");
				this.db.Execute("CREATE INDEX IF NOT EXISTS entindex ON data ( entid )");
			}
		}
	}

	// Token: 0x060032A2 RID: 12962 RVA: 0x00138F24 File Offset: 0x00137124
	~FileStorage()
	{
		this.Dispose();
	}

	// Token: 0x060032A3 RID: 12963 RVA: 0x00138F50 File Offset: 0x00137150
	public void Dispose()
	{
		if (this.db != null)
		{
			this.db.Close();
			this.db = null;
		}
	}

	// Token: 0x060032A4 RID: 12964 RVA: 0x00138F6C File Offset: 0x0013716C
	private uint GetCRC(byte[] data, FileStorage.Type type)
	{
		uint crc32Result;
		using (TimeWarning.New("FileStorage.GetCRC", 0))
		{
			this.crc.Reset();
			this.crc.SlurpBlock(data, 0, data.Length);
			this.crc.UpdateCRC((byte)type);
			crc32Result = (uint)this.crc.Crc32Result;
		}
		return crc32Result;
	}

	// Token: 0x060032A5 RID: 12965 RVA: 0x00138FD8 File Offset: 0x001371D8
	public uint Store(byte[] data, FileStorage.Type type, uint entityID, uint numID = 0U)
	{
		uint result;
		using (TimeWarning.New("FileStorage.Store", 0))
		{
			uint num = this.GetCRC(data, type);
			if (this.db != null)
			{
				this.db.Execute<int, byte[], int, int, int>("INSERT OR REPLACE INTO data ( crc, data, entid, filetype, part ) VALUES ( ?, ?, ?, ?, ? )", (int)num, data, (int)entityID, (int)type, (int)numID);
			}
			this._cache.Remove(num);
			this._cache.Add(num, new FileStorage.CacheData
			{
				data = data,
				entityID = entityID,
				numID = numID
			});
			result = num;
		}
		return result;
	}

	// Token: 0x060032A6 RID: 12966 RVA: 0x0013906C File Offset: 0x0013726C
	public byte[] Get(uint crc, FileStorage.Type type, uint entityID, uint numID = 0U)
	{
		byte[] result;
		using (TimeWarning.New("FileStorage.Get", 0))
		{
			FileStorage.CacheData cacheData;
			if (this._cache.TryGetValue(crc, out cacheData))
			{
				Assert.IsTrue(cacheData.data != null, "FileStorage cache contains a null texture");
				result = cacheData.data;
			}
			else if (this.db == null)
			{
				result = null;
			}
			else
			{
				byte[] array = this.db.QueryBlob<int, int, int, int>("SELECT data FROM data WHERE crc = ? AND filetype = ? AND entid = ? AND part = ? LIMIT 1", (int)crc, (int)type, (int)entityID, (int)numID);
				if (array == null)
				{
					result = null;
				}
				else
				{
					this._cache.Remove(crc);
					this._cache.Add(crc, new FileStorage.CacheData
					{
						data = array,
						entityID = entityID,
						numID = 0U
					});
					result = array;
				}
			}
		}
		return result;
	}

	// Token: 0x060032A7 RID: 12967 RVA: 0x0013912C File Offset: 0x0013732C
	public void Remove(uint crc, FileStorage.Type type, uint entityID)
	{
		using (TimeWarning.New("FileStorage.Remove", 0))
		{
			if (this.db != null)
			{
				this.db.Execute<int, int, int>("DELETE FROM data WHERE crc = ? AND filetype = ? AND entid = ?", (int)crc, (int)type, (int)entityID);
			}
			this._cache.Remove(crc);
		}
	}

	// Token: 0x060032A8 RID: 12968 RVA: 0x00139188 File Offset: 0x00137388
	public void RemoveExact(uint crc, FileStorage.Type type, uint entityID, uint numid)
	{
		using (TimeWarning.New("FileStorage.RemoveExact", 0))
		{
			if (this.db != null)
			{
				this.db.Execute<int, int, int, int>("DELETE FROM data WHERE crc = ? AND filetype = ? AND entid = ? AND part = ?", (int)crc, (int)type, (int)entityID, (int)numid);
			}
			this._cache.Remove(crc);
		}
	}

	// Token: 0x060032A9 RID: 12969 RVA: 0x001391E8 File Offset: 0x001373E8
	public void RemoveEntityNum(uint entityid, uint numid)
	{
		using (TimeWarning.New("FileStorage.RemoveEntityNum", 0))
		{
			if (this.db != null)
			{
				this.db.Execute<int, int>("DELETE FROM data WHERE entid = ? AND part = ?", (int)entityid, (int)numid);
			}
			IEnumerable<KeyValuePair<uint, FileStorage.CacheData>> cache = this._cache;
			Func<KeyValuePair<uint, FileStorage.CacheData>, bool> <>9__0;
			Func<KeyValuePair<uint, FileStorage.CacheData>, bool> predicate;
			if ((predicate = <>9__0) == null)
			{
				predicate = (<>9__0 = ((KeyValuePair<uint, FileStorage.CacheData> x) => x.Value.entityID == entityid && x.Value.numID == numid));
			}
			foreach (uint key in (from x in cache.Where(predicate)
			select x.Key).ToArray<uint>())
			{
				this._cache.Remove(key);
			}
		}
	}

	// Token: 0x060032AA RID: 12970 RVA: 0x001392CC File Offset: 0x001374CC
	internal void RemoveAllByEntity(uint entityid)
	{
		using (TimeWarning.New("FileStorage.RemoveAllByEntity", 0))
		{
			if (this.db != null)
			{
				this.db.Execute<int>("DELETE FROM data WHERE entid = ?", (int)entityid);
			}
		}
	}

	// Token: 0x060032AB RID: 12971 RVA: 0x0013931C File Offset: 0x0013751C
	public void ReassignEntityId(uint oldId, uint newId)
	{
		using (TimeWarning.New("FileStorage.ReassignEntityId", 0))
		{
			if (this.db != null)
			{
				this.db.Execute<int, int>("UPDATE data SET entid = ? WHERE entid = ?", (int)newId, (int)oldId);
			}
		}
	}

	// Token: 0x040028F2 RID: 10482
	private Database db;

	// Token: 0x040028F3 RID: 10483
	private CRC32 crc = new CRC32();

	// Token: 0x040028F4 RID: 10484
	private MruDictionary<uint, FileStorage.CacheData> _cache = new MruDictionary<uint, FileStorage.CacheData>(1000, null);

	// Token: 0x040028F5 RID: 10485
	public static FileStorage server = new FileStorage("sv.files." + 233, true);

	// Token: 0x02000DFB RID: 3579
	private class CacheData
	{
		// Token: 0x0400489A RID: 18586
		public byte[] data;

		// Token: 0x0400489B RID: 18587
		public uint entityID;

		// Token: 0x0400489C RID: 18588
		public uint numID;
	}

	// Token: 0x02000DFC RID: 3580
	public enum Type
	{
		// Token: 0x0400489E RID: 18590
		png,
		// Token: 0x0400489F RID: 18591
		jpg,
		// Token: 0x040048A0 RID: 18592
		ogg
	}
}
