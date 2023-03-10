using System;
using System.Collections.Generic;
using Network;
using UnityEngine;

// Token: 0x02000715 RID: 1813
public class ConnectionQueue
{
	// Token: 0x170003EA RID: 1002
	// (get) Token: 0x06003213 RID: 12819
	public int Queued
	{
		get
		{
			return this.queue.Count;
		}
	}

	// Token: 0x170003EB RID: 1003
	// (get) Token: 0x06003214 RID: 12820
	public int Joining
	{
		get
		{
			return this.joining.Count;
		}
	}

	// Token: 0x06003215 RID: 12821
	public void SkipQueue(ulong userid)
	{
		for (int i = 0; i < this.queue.Count; i++)
		{
			Connection connection = this.queue[i];
			if (connection.userid == userid)
			{
				this.JoinGame(connection);
				return;
			}
		}
	}

	// Token: 0x06003216 RID: 12822
	internal void Join(Connection connection)
	{
		Debug.Log("ConnectionQueue:Join");
		connection.state = Connection.State.InQueue;
		this.queue.Add(connection);
		this.nextMessageTime = 0f;
		if (this.CanJumpQueue(connection))
		{
			this.JoinGame(connection);
		}
	}

	// Token: 0x06003217 RID: 12823
	public void Cycle(int availableSlots)
	{
		if (this.queue.Count == 0)
		{
			return;
		}
		if (availableSlots - this.Joining > 0)
		{
			this.JoinGame(this.queue[0]);
		}
		this.SendMessages();
	}

	// Token: 0x06003218 RID: 12824
	private void SendMessages()
	{
		if (this.nextMessageTime > Time.realtimeSinceStartup)
		{
			return;
		}
		this.nextMessageTime = Time.realtimeSinceStartup + 10f;
		for (int i = 0; i < this.queue.Count; i++)
		{
			this.SendMessage(this.queue[i], i);
		}
	}

	// Token: 0x06003219 RID: 12825
	private void SendMessage(Connection c, int position)
	{
		string val = string.Empty;
		if (position > 0)
		{
			val = string.Format("{0:N0} PLAYERS AHEAD OF YOU, {1:N0} PLAYERS BEHIND", position, this.queue.Count - position - 1);
		}
		else
		{
			val = string.Format("YOU'RE NEXT - {1:N0} PLAYERS BEHIND YOU", position, this.queue.Count - position - 1);
		}
		NetWrite netWrite = Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.Message);
		netWrite.String("QUEUE");
		netWrite.String(val);
		netWrite.Send(new SendInfo(c));
	}

	// Token: 0x0600321A RID: 12826
	public void RemoveConnection(Connection connection)
	{
		if (this.queue.Remove(connection))
		{
			this.nextMessageTime = 0f;
		}
		this.joining.Remove(connection);
	}

	// Token: 0x0600321B RID: 12827
	private void JoinGame(Connection connection)
	{
		Debug.Log("ConnectionQueue:JoinGame");
		this.queue.Remove(connection);
		connection.state = Connection.State.Welcoming;
		this.nextMessageTime = 0f;
		this.joining.Add(connection);
		SingletonComponent<ServerMgr>.Instance.JoinGame(connection);
	}

	// Token: 0x0600321C RID: 12828
	public void JoinedGame(Connection connection)
	{
		this.RemoveConnection(connection);
	}

	// Token: 0x0600321D RID: 12829
	private bool CanJumpQueue(Connection connection)
	{
		if (DeveloperList.Contains(connection.userid))
		{
			return true;
		}
		ServerUsers.User user = ServerUsers.Get(connection.userid);
		return (user != null && user.group == ServerUsers.UserGroup.Moderator) || (user != null && user.group == ServerUsers.UserGroup.Owner) || (user != null && user.group == ServerUsers.UserGroup.SkipQueue);
	}

	// Token: 0x0600321E RID: 12830
	public bool IsQueued(ulong userid)
	{
		for (int i = 0; i < this.queue.Count; i++)
		{
			if (this.queue[i].userid == userid)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600321F RID: 12831
	public bool IsJoining(ulong userid)
	{
		for (int i = 0; i < this.joining.Count; i++)
		{
			if (this.joining[i].userid == userid)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040028B4 RID: 10420
	private List<Connection> queue = new List<Connection>();

	// Token: 0x040028B5 RID: 10421
	private List<Connection> joining = new List<Connection>();

	// Token: 0x040028B6 RID: 10422
	private float nextMessageTime;
}
