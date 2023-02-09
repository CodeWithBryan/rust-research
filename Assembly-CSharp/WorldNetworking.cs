using System;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x020006BE RID: 1726
public class WorldNetworking
{
	// Token: 0x06003073 RID: 12403 RVA: 0x0012A814 File Offset: 0x00128A14
	public static void OnMessageReceived(Message message)
	{
		WorldSerialization serialization = World.Serialization;
		using (WorldMessage worldMessage = WorldMessage.Deserialize(message.read))
		{
			switch (worldMessage.status)
			{
			case WorldMessage.MessageType.Request:
				WorldNetworking.SendWorldData(message.connection);
				return;
			}
			if (worldMessage.prefabs != null)
			{
				serialization.world.prefabs.AddRange(worldMessage.prefabs);
				worldMessage.prefabs.Clear();
			}
			if (worldMessage.paths != null)
			{
				serialization.world.paths.AddRange(worldMessage.paths);
				worldMessage.paths.Clear();
			}
		}
	}

	// Token: 0x06003074 RID: 12404 RVA: 0x0012A8CC File Offset: 0x00128ACC
	private static void SendWorldData(Connection connection)
	{
		if (connection.hasRequestedWorld)
		{
			DebugEx.LogWarning(string.Format("{0} requested world data more than once", connection), StackTraceLogType.None);
			return;
		}
		connection.hasRequestedWorld = true;
		WorldSerialization serialization = World.Serialization;
		WorldMessage worldMessage = Pool.Get<WorldMessage>();
		for (int i = 0; i < serialization.world.prefabs.Count; i++)
		{
			if (worldMessage.prefabs != null && worldMessage.prefabs.Count >= 100)
			{
				worldMessage.status = WorldMessage.MessageType.Receive;
				WorldNetworking.SendWorldData(connection, ref worldMessage);
				worldMessage = Pool.Get<WorldMessage>();
			}
			if (worldMessage.prefabs == null)
			{
				worldMessage.prefabs = Pool.GetList<PrefabData>();
			}
			worldMessage.prefabs.Add(serialization.world.prefabs[i]);
		}
		for (int j = 0; j < serialization.world.paths.Count; j++)
		{
			if (worldMessage.paths != null && worldMessage.paths.Count >= 10)
			{
				worldMessage.status = WorldMessage.MessageType.Receive;
				WorldNetworking.SendWorldData(connection, ref worldMessage);
				worldMessage = Pool.Get<WorldMessage>();
			}
			if (worldMessage.paths == null)
			{
				worldMessage.paths = Pool.GetList<PathData>();
			}
			worldMessage.paths.Add(serialization.world.paths[j]);
		}
		if (worldMessage != null)
		{
			worldMessage.status = WorldMessage.MessageType.Done;
			WorldNetworking.SendWorldData(connection, ref worldMessage);
		}
	}

	// Token: 0x06003075 RID: 12405 RVA: 0x0012AA04 File Offset: 0x00128C04
	private static void SendWorldData(Connection connection, ref WorldMessage data)
	{
		NetWrite netWrite = Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.World);
		data.ToProto(netWrite);
		netWrite.Send(new SendInfo(connection));
		if (data.prefabs != null)
		{
			data.prefabs.Clear();
		}
		if (data.paths != null)
		{
			data.paths.Clear();
		}
		data.Dispose();
		data = null;
	}

	// Token: 0x0400276B RID: 10091
	private const int prefabsPerPacket = 100;

	// Token: 0x0400276C RID: 10092
	private const int pathsPerPacket = 10;
}
