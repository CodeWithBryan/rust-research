using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200005A RID: 90
public class ComputerStation : BaseMountable
{
	// Token: 0x06000978 RID: 2424 RVA: 0x000584D4 File Offset: 0x000566D4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ComputerStation.OnRpcMessage", 0))
		{
			if (rpc == 481778085U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AddBookmark ");
				}
				using (TimeWarning.New("AddBookmark", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.AddBookmark(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in AddBookmark");
					}
				}
				return true;
			}
			if (rpc == 552248427U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - BeginControllingBookmark ");
				}
				using (TimeWarning.New("BeginControllingBookmark", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.BeginControllingBookmark(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in BeginControllingBookmark");
					}
				}
				return true;
			}
			if (rpc == 2498687923U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DeleteBookmark ");
				}
				using (TimeWarning.New("DeleteBookmark", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.DeleteBookmark(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in DeleteBookmark");
					}
				}
				return true;
			}
			if (rpc == 2139261430U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_DisconnectControl ");
				}
				using (TimeWarning.New("Server_DisconnectControl", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_DisconnectControl(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in Server_DisconnectControl");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000979 RID: 2425 RVA: 0x0005894C File Offset: 0x00056B4C
	public static bool IsValidIdentifier(string str)
	{
		return !string.IsNullOrEmpty(str) && str.Length <= 32 && str.IsAlphaNumeric();
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x0005896A File Offset: 0x00056B6A
	public override void DestroyShared()
	{
		if (base.isServer && base.GetMounted())
		{
			this.StopControl(base.GetMounted());
		}
		base.DestroyShared();
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x00058993 File Offset: 0x00056B93
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.GatherStaticCameras), 5f);
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x000589B4 File Offset: 0x00056BB4
	public void GatherStaticCameras()
	{
		if (Rust.Application.isLoadingSave)
		{
			base.Invoke(new Action(this.GatherStaticCameras), 1f);
			return;
		}
		if (this.isStatic && this.autoGatherRadius > 0f)
		{
			List<global::BaseEntity> list = Facepunch.Pool.GetList<global::BaseEntity>();
			global::Vis.Entities<global::BaseEntity>(base.transform.position, this.autoGatherRadius, list, 256, QueryTriggerInteraction.Ignore);
			foreach (global::BaseEntity baseEntity in list)
			{
				IRemoteControllable component = baseEntity.GetComponent<IRemoteControllable>();
				if (component != null)
				{
					CCTV_RC component2 = baseEntity.GetComponent<CCTV_RC>();
					if ((!component2 || component2.IsStatic()) && !this.controlBookmarks.ContainsKey(component.GetIdentifier()))
					{
						this.ForceAddBookmark(component.GetIdentifier());
					}
				}
			}
			Facepunch.Pool.FreeList<global::BaseEntity>(ref list);
		}
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x00058AA4 File Offset: 0x00056CA4
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.GatherStaticCameras();
	}

	// Token: 0x0600097E RID: 2430 RVA: 0x00058AB4 File Offset: 0x00056CB4
	public void SetPlayerSecondaryGroupFor(global::BaseEntity ent)
	{
		global::BasePlayer mounted = this._mounted;
		if (mounted)
		{
			mounted.net.SwitchSecondaryGroup(ent ? ent.net.group : null);
		}
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x00058AF4 File Offset: 0x00056CF4
	public void StopControl(global::BasePlayer ply)
	{
		global::BaseEntity baseEntity = this.currentlyControllingEnt.Get(true);
		if (baseEntity)
		{
			baseEntity.GetComponent<IRemoteControllable>().StopControl();
			if (ply)
			{
				ply.net.SwitchSecondaryGroup(null);
			}
		}
		this.currentlyControllingEnt.uid = 0U;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		this.SendControlBookmarks(ply);
		base.CancelInvoke(new Action(this.ControlCheck));
		base.CancelInvoke(new Action(this.CheckCCTVAchievement));
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x00058B74 File Offset: 0x00056D74
	public bool IsPlayerAdmin(global::BasePlayer player)
	{
		return player == this._mounted;
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x00058B84 File Offset: 0x00056D84
	[global::BaseEntity.RPC_Server]
	public void DeleteBookmark(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.IsPlayerAdmin(player))
		{
			return;
		}
		if (this.isStatic)
		{
			return;
		}
		string text = msg.read.String(256);
		if (!global::ComputerStation.IsValidIdentifier(text))
		{
			return;
		}
		if (this.controlBookmarks.ContainsKey(text))
		{
			uint num = this.controlBookmarks[text];
			this.controlBookmarks.Remove(text);
			this.SendControlBookmarks(player);
			if (num == this.currentlyControllingEnt.uid)
			{
				this.currentlyControllingEnt.Set(null);
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
	}

	// Token: 0x06000982 RID: 2434 RVA: 0x00058C14 File Offset: 0x00056E14
	[global::BaseEntity.RPC_Server]
	public void Server_DisconnectControl(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.IsPlayerAdmin(player))
		{
			return;
		}
		this.StopControl(player);
	}

	// Token: 0x06000983 RID: 2435 RVA: 0x00058C3C File Offset: 0x00056E3C
	[global::BaseEntity.RPC_Server]
	public void BeginControllingBookmark(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.IsPlayerAdmin(player))
		{
			return;
		}
		string text = msg.read.String(256);
		if (!global::ComputerStation.IsValidIdentifier(text))
		{
			return;
		}
		if (!this.controlBookmarks.ContainsKey(text))
		{
			return;
		}
		uint uid = this.controlBookmarks[text];
		global::BaseNetworkable baseNetworkable = global::BaseNetworkable.serverEntities.Find(uid);
		if (baseNetworkable == null)
		{
			return;
		}
		IRemoteControllable component = baseNetworkable.GetComponent<IRemoteControllable>();
		if (!component.CanControl())
		{
			return;
		}
		if (component.GetIdentifier() != text)
		{
			return;
		}
		global::BaseEntity baseEntity = this.currentlyControllingEnt.Get(true);
		if (baseEntity)
		{
			IRemoteControllable component2 = baseEntity.GetComponent<IRemoteControllable>();
			if (component2 != null)
			{
				component2.StopControl();
			}
		}
		player.net.SwitchSecondaryGroup(baseNetworkable.net.group);
		this.currentlyControllingEnt.uid = baseNetworkable.net.ID;
		base.SendNetworkUpdateImmediate(false);
		this.SendControlBookmarks(player);
		component.InitializeControl(player);
		if (GameInfo.HasAchievements && component.GetEnt() is CCTV_RC)
		{
			base.InvokeRepeating(new Action(this.CheckCCTVAchievement), 1f, 3f);
		}
		base.InvokeRepeating(new Action(this.ControlCheck), 0f, 0f);
	}

	// Token: 0x06000984 RID: 2436 RVA: 0x00058D84 File Offset: 0x00056F84
	private void CheckCCTVAchievement()
	{
		if (this._mounted != null)
		{
			global::BaseEntity baseEntity = this.currentlyControllingEnt.Get(true);
			CCTV_RC cctv_RC;
			if (baseEntity != null && (cctv_RC = (baseEntity as CCTV_RC)) != null)
			{
				foreach (Connection connection in this._mounted.net.secondaryGroup.subscribers)
				{
					if (connection.active)
					{
						global::BasePlayer basePlayer = connection.player as global::BasePlayer;
						if (!(basePlayer == null))
						{
							Vector3 vector = basePlayer.CenterPoint();
							float num = Vector3.Dot((vector - cctv_RC.pitch.position).normalized, cctv_RC.pitch.forward);
							Vector3 vector2 = cctv_RC.pitch.InverseTransformPoint(vector);
							if (num > 0.6f && vector2.magnitude < 10f)
							{
								this._mounted.GiveAchievement("BIG_BROTHER");
								base.CancelInvoke(new Action(this.CheckCCTVAchievement));
								break;
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x00058EBC File Offset: 0x000570BC
	public bool CanAddBookmark(global::BasePlayer player)
	{
		if (!this.IsPlayerAdmin(player))
		{
			return false;
		}
		if (this.isStatic)
		{
			return false;
		}
		if (UnityEngine.Time.realtimeSinceStartup < this.nextAddTime)
		{
			return false;
		}
		if (this.controlBookmarks.Count > 3)
		{
			player.ChatMessage("Too many bookmarks, delete some");
			return false;
		}
		return true;
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x00058F0C File Offset: 0x0005710C
	public void ForceAddBookmark(string identifier)
	{
		if (this.controlBookmarks.Count >= 128)
		{
			return;
		}
		if (!global::ComputerStation.IsValidIdentifier(identifier))
		{
			return;
		}
		foreach (KeyValuePair<string, uint> keyValuePair in this.controlBookmarks)
		{
			if (keyValuePair.Key == identifier)
			{
				return;
			}
		}
		uint num = 0U;
		bool flag = false;
		foreach (IRemoteControllable remoteControllable in RemoteControlEntity.allControllables)
		{
			if (remoteControllable != null && remoteControllable.GetIdentifier() == identifier)
			{
				if (!(remoteControllable.GetEnt() == null))
				{
					num = remoteControllable.GetEnt().net.ID;
					flag = true;
					break;
				}
				Debug.LogWarning("Computer station added bookmark with missing ent, likely a static CCTV (wipe the server)");
			}
		}
		if (!flag)
		{
			return;
		}
		global::BaseNetworkable baseNetworkable = global::BaseNetworkable.serverEntities.Find(num);
		if (baseNetworkable == null)
		{
			return;
		}
		IRemoteControllable component = baseNetworkable.GetComponent<IRemoteControllable>();
		if (component == null)
		{
			return;
		}
		string identifier2 = component.GetIdentifier();
		if (identifier == identifier2)
		{
			this.controlBookmarks.Add(identifier, num);
		}
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x00059054 File Offset: 0x00057254
	[global::BaseEntity.RPC_Server]
	public void AddBookmark(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.IsPlayerAdmin(player))
		{
			return;
		}
		if (this.isStatic)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup < this.nextAddTime)
		{
			player.ChatMessage("Slow down...");
			return;
		}
		if (this.controlBookmarks.Count >= 128)
		{
			player.ChatMessage("Too many bookmarks, delete some");
			return;
		}
		this.nextAddTime = UnityEngine.Time.realtimeSinceStartup + 1f;
		string text = msg.read.String(256);
		if (!global::ComputerStation.IsValidIdentifier(text))
		{
			return;
		}
		foreach (KeyValuePair<string, uint> keyValuePair in this.controlBookmarks)
		{
			if (keyValuePair.Key == text)
			{
				return;
			}
		}
		uint num = 0U;
		bool flag = false;
		foreach (IRemoteControllable remoteControllable in RemoteControlEntity.allControllables)
		{
			if (remoteControllable != null && remoteControllable.GetIdentifier() == text)
			{
				if (!(remoteControllable.GetEnt() == null))
				{
					num = remoteControllable.GetEnt().net.ID;
					flag = true;
					break;
				}
				Debug.LogWarning("Computer station added bookmark with missing ent, likely a static CCTV (wipe the server)");
			}
		}
		if (!flag)
		{
			return;
		}
		global::BaseNetworkable baseNetworkable = global::BaseNetworkable.serverEntities.Find(num);
		if (baseNetworkable == null)
		{
			return;
		}
		IRemoteControllable component = baseNetworkable.GetComponent<IRemoteControllable>();
		if (component == null)
		{
			return;
		}
		string identifier = component.GetIdentifier();
		if (text == identifier)
		{
			this.controlBookmarks.Add(text, num);
		}
		this.SendControlBookmarks(player);
	}

	// Token: 0x06000988 RID: 2440 RVA: 0x00059208 File Offset: 0x00057408
	public void ControlCheck()
	{
		bool flag = false;
		global::BaseEntity baseEntity = this.currentlyControllingEnt.Get(base.isServer);
		if (baseEntity)
		{
			IRemoteControllable component = baseEntity.GetComponent<IRemoteControllable>();
			if (component != null && component.CanControl())
			{
				flag = true;
				if (this._mounted != null)
				{
					this._mounted.net.SwitchSecondaryGroup(baseEntity.net.group);
				}
			}
		}
		if (!flag)
		{
			this.StopControl(this._mounted);
		}
	}

	// Token: 0x06000989 RID: 2441 RVA: 0x00059280 File Offset: 0x00057480
	public string GenerateControlBookmarkString()
	{
		string text = "";
		foreach (KeyValuePair<string, uint> keyValuePair in this.controlBookmarks)
		{
			text += keyValuePair.Key;
			text += ":";
			text += keyValuePair.Value;
			text += ";";
		}
		return text;
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x0005930C File Offset: 0x0005750C
	public void SendControlBookmarks(global::BasePlayer player)
	{
		if (player == null)
		{
			return;
		}
		string arg = this.GenerateControlBookmarkString();
		base.ClientRPCPlayer<string>(null, player, "ReceiveBookmarks", arg);
	}

	// Token: 0x0600098B RID: 2443 RVA: 0x00059338 File Offset: 0x00057538
	public override void OnPlayerMounted()
	{
		base.OnPlayerMounted();
		global::BasePlayer mounted = this._mounted;
		if (mounted)
		{
			this.SendControlBookmarks(mounted);
		}
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
	}

	// Token: 0x0600098C RID: 2444 RVA: 0x0005936B File Offset: 0x0005756B
	public override void OnPlayerDismounted(global::BasePlayer player)
	{
		base.OnPlayerDismounted(player);
		this.StopControl(player);
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x0600098D RID: 2445 RVA: 0x00059385 File Offset: 0x00057585
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
		if (this.currentlyControllingEnt.IsValid(true))
		{
			this.currentlyControllingEnt.Get(true).GetComponent<IRemoteControllable>().UserInput(inputState, player);
		}
	}

	// Token: 0x0600098E RID: 2446 RVA: 0x000593B8 File Offset: 0x000575B8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
			info.msg.ioEntity.genericEntRef1 = this.currentlyControllingEnt.uid;
			return;
		}
		info.msg.computerStation = Facepunch.Pool.Get<ProtoBuf.ComputerStation>();
		info.msg.computerStation.bookmarks = this.GenerateControlBookmarkString();
	}

	// Token: 0x0600098F RID: 2447 RVA: 0x00059428 File Offset: 0x00057628
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (!info.fromDisk)
		{
			if (info.msg.ioEntity != null)
			{
				this.currentlyControllingEnt.uid = info.msg.ioEntity.genericEntRef1;
				return;
			}
		}
		else if (info.msg.computerStation != null)
		{
			string[] array = info.msg.computerStation.bookmarks.Split(new char[]
			{
				';'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					':'
				});
				if (array2.Length < 2)
				{
					break;
				}
				string text = array2[0];
				uint value;
				uint.TryParse(array2[1], out value);
				if (global::ComputerStation.IsValidIdentifier(text))
				{
					this.controlBookmarks.Add(text, value);
				}
			}
		}
	}

	// Token: 0x0400064A RID: 1610
	[Header("Computer")]
	public GameObjectRef menuPrefab;

	// Token: 0x0400064B RID: 1611
	public ComputerMenu computerMenu;

	// Token: 0x0400064C RID: 1612
	public EntityRef currentlyControllingEnt;

	// Token: 0x0400064D RID: 1613
	public Dictionary<string, uint> controlBookmarks = new Dictionary<string, uint>();

	// Token: 0x0400064E RID: 1614
	public Transform leftHandIKPosition;

	// Token: 0x0400064F RID: 1615
	public Transform rightHandIKPosition;

	// Token: 0x04000650 RID: 1616
	public SoundDefinition turnOnSoundDef;

	// Token: 0x04000651 RID: 1617
	public SoundDefinition turnOffSoundDef;

	// Token: 0x04000652 RID: 1618
	public SoundDefinition onLoopSoundDef;

	// Token: 0x04000653 RID: 1619
	public bool isStatic;

	// Token: 0x04000654 RID: 1620
	public float autoGatherRadius;

	// Token: 0x04000655 RID: 1621
	private float nextAddTime;
}
