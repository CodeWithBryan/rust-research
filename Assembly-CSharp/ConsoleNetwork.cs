using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;

// Token: 0x0200033A RID: 826
public static class ConsoleNetwork
{
	// Token: 0x06001E0C RID: 7692 RVA: 0x000059DD File Offset: 0x00003BDD
	internal static void Init()
	{
	}

	// Token: 0x06001E0D RID: 7693 RVA: 0x000CC234 File Offset: 0x000CA434
	internal static void OnClientCommand(Message packet)
	{
		if (packet.read.Unread > ConVar.Server.maxpacketsize_command)
		{
			Debug.LogWarning("Dropping client command due to size");
			return;
		}
		string text = packet.read.StringRaw(8388608U);
		if (packet.connection == null || !packet.connection.connected)
		{
			Debug.LogWarning("Client without connection tried to run command: " + text);
			return;
		}
		string text2 = ConsoleSystem.Run(ConsoleSystem.Option.Server.FromConnection(packet.connection).Quiet(), text, Array.Empty<object>());
		if (!string.IsNullOrEmpty(text2))
		{
			ConsoleNetwork.SendClientReply(packet.connection, text2);
		}
	}

	// Token: 0x06001E0E RID: 7694 RVA: 0x000CC2D0 File Offset: 0x000CA4D0
	internal static void SendClientReply(Connection cn, string strCommand)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.ConsoleMessage);
		netWrite.String(strCommand);
		netWrite.Send(new SendInfo(cn));
	}

	// Token: 0x06001E0F RID: 7695 RVA: 0x000CC304 File Offset: 0x000CA504
	public static void SendClientCommand(Connection cn, string strCommand, params object[] args)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.ConsoleCommand);
		string val = ConsoleSystem.BuildCommand(strCommand, args);
		netWrite.String(val);
		netWrite.Send(new SendInfo(cn));
	}

	// Token: 0x06001E10 RID: 7696 RVA: 0x000CC34A File Offset: 0x000CA54A
	public static void SendClientCommand(List<Connection> cn, string strCommand, params object[] args)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.ConsoleCommand);
		netWrite.String(ConsoleSystem.BuildCommand(strCommand, args));
		netWrite.Send(new SendInfo(cn));
	}

	// Token: 0x06001E11 RID: 7697 RVA: 0x000CC384 File Offset: 0x000CA584
	public static void BroadcastToAllClients(string strCommand, params object[] args)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.ConsoleCommand);
		netWrite.String(ConsoleSystem.BuildCommand(strCommand, args));
		netWrite.Send(new SendInfo(Network.Net.sv.connections));
	}
}
