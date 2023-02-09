using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C5 RID: 197
public class SleepingBag : global::DecayEntity
{
	// Token: 0x0600116A RID: 4458 RVA: 0x0008CD0C File Offset: 0x0008AF0C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SleepingBag.OnRpcMessage", 0))
		{
			if (rpc == 3057055788U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AssignToFriend ");
				}
				using (TimeWarning.New("AssignToFriend", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3057055788U, "AssignToFriend", this, player, 3f))
						{
							return true;
						}
					}
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
							this.AssignToFriend(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in AssignToFriend");
					}
				}
				return true;
			}
			if (rpc == 1335950295U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Rename ");
				}
				using (TimeWarning.New("Rename", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1335950295U, "Rename", this, player, 3f))
						{
							return true;
						}
					}
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
							this.Rename(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in Rename");
					}
				}
				return true;
			}
			if (rpc == 42669546U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_MakeBed ");
				}
				using (TimeWarning.New("RPC_MakeBed", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(42669546U, "RPC_MakeBed", this, player, 3f))
						{
							return true;
						}
					}
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
							this.RPC_MakeBed(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RPC_MakeBed");
					}
				}
				return true;
			}
			if (rpc == 393812086U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_MakePublic ");
				}
				using (TimeWarning.New("RPC_MakePublic", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(393812086U, "RPC_MakePublic", this, player, 3f))
						{
							return true;
						}
					}
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
							this.RPC_MakePublic(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in RPC_MakePublic");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600116B RID: 4459 RVA: 0x0002D546 File Offset: 0x0002B746
	public bool IsPublic()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved3);
	}

	// Token: 0x17000172 RID: 370
	// (get) Token: 0x0600116C RID: 4460 RVA: 0x0008D2C4 File Offset: 0x0008B4C4
	public virtual float unlockSeconds
	{
		get
		{
			if (this.unlockTime < UnityEngine.Time.realtimeSinceStartup)
			{
				return 0f;
			}
			return this.unlockTime - UnityEngine.Time.realtimeSinceStartup;
		}
	}

	// Token: 0x0600116D RID: 4461 RVA: 0x0008D2E5 File Offset: 0x0008B4E5
	public virtual float GetUnlockSeconds(ulong playerID)
	{
		return this.unlockSeconds;
	}

	// Token: 0x0600116E RID: 4462 RVA: 0x0008D2ED File Offset: 0x0008B4ED
	public virtual bool ValidForPlayer(ulong playerID, bool ignoreTimers)
	{
		return this.deployerUserID == playerID && (ignoreTimers || this.unlockTime < UnityEngine.Time.realtimeSinceStartup);
	}

	// Token: 0x0600116F RID: 4463 RVA: 0x0008D30C File Offset: 0x0008B50C
	public static global::SleepingBag[] FindForPlayer(ulong playerID, bool ignoreTimers)
	{
		return (from x in global::SleepingBag.sleepingBags
		where x.ValidForPlayer(playerID, ignoreTimers)
		select x).ToArray<global::SleepingBag>();
	}

	// Token: 0x06001170 RID: 4464 RVA: 0x0008D348 File Offset: 0x0008B548
	public static global::SleepingBag FindForPlayer(ulong playerID, uint sleepingBagID, bool ignoreTimers)
	{
		return global::SleepingBag.sleepingBags.FirstOrDefault((global::SleepingBag x) => x.deployerUserID == playerID && x.net.ID == sleepingBagID && (ignoreTimers || x.unlockTime < UnityEngine.Time.realtimeSinceStartup));
	}

	// Token: 0x06001171 RID: 4465 RVA: 0x0008D388 File Offset: 0x0008B588
	public static bool SpawnPlayer(global::BasePlayer player, uint sleepingBag)
	{
		global::SleepingBag[] array = global::SleepingBag.FindForPlayer(player.userID, true);
		global::SleepingBag sleepingBag2 = array.FirstOrDefault((global::SleepingBag x) => x.ValidForPlayer(player.userID, false) && x.net.ID == sleepingBag && x.unlockTime < UnityEngine.Time.realtimeSinceStartup);
		if (sleepingBag2 == null)
		{
			return false;
		}
		if (sleepingBag2.IsOccupied())
		{
			return false;
		}
		Vector3 position;
		Quaternion rotation;
		sleepingBag2.GetSpawnPos(out position, out rotation);
		player.RespawnAt(position, rotation);
		sleepingBag2.PostPlayerSpawn(player);
		global::SleepingBag[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			global::SleepingBag.SetBagTimer(array2[i], position, global::SleepingBag.SleepingBagResetReason.Respawned);
		}
		return true;
	}

	// Token: 0x06001172 RID: 4466 RVA: 0x0008D42A File Offset: 0x0008B62A
	public virtual void SetUnlockTime(float newTime)
	{
		this.unlockTime = newTime;
	}

	// Token: 0x06001173 RID: 4467 RVA: 0x0008D434 File Offset: 0x0008B634
	public static bool DestroyBag(global::BasePlayer player, uint sleepingBag)
	{
		global::SleepingBag sleepingBag2 = global::SleepingBag.FindForPlayer(player.userID, sleepingBag, false);
		if (sleepingBag2 == null)
		{
			return false;
		}
		if (sleepingBag2.canBePublic)
		{
			sleepingBag2.SetPublic(true);
			sleepingBag2.deployerUserID = 0UL;
		}
		else
		{
			sleepingBag2.Kill(global::BaseNetworkable.DestroyMode.None);
		}
		player.SendRespawnOptions();
		return true;
	}

	// Token: 0x06001174 RID: 4468 RVA: 0x0008D484 File Offset: 0x0008B684
	public static void ResetTimersForPlayer(global::BasePlayer player)
	{
		global::SleepingBag[] array = global::SleepingBag.FindForPlayer(player.userID, true);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].unlockTime = 0f;
		}
	}

	// Token: 0x06001175 RID: 4469 RVA: 0x0008D4BC File Offset: 0x0008B6BC
	public virtual void GetSpawnPos(out Vector3 pos, out Quaternion rot)
	{
		pos = base.transform.position + this.spawnOffset;
		rot = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
	}

	// Token: 0x06001176 RID: 4470 RVA: 0x0008D512 File Offset: 0x0008B712
	public void SetPublic(bool isPublic)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved3, isPublic, false, true);
	}

	// Token: 0x06001177 RID: 4471 RVA: 0x0008D522 File Offset: 0x0008B722
	private void SetDeployedBy(global::BasePlayer player)
	{
		if (player == null)
		{
			return;
		}
		this.deployerUserID = player.userID;
		global::SleepingBag.SetBagTimer(this, base.transform.position, global::SleepingBag.SleepingBagResetReason.Placed);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001178 RID: 4472 RVA: 0x0008D554 File Offset: 0x0008B754
	public static void OnPlayerDeath(global::BasePlayer player)
	{
		global::SleepingBag[] array = global::SleepingBag.FindForPlayer(player.userID, true);
		for (int i = 0; i < array.Length; i++)
		{
			global::SleepingBag.SetBagTimer(array[i], player.transform.position, global::SleepingBag.SleepingBagResetReason.Death);
		}
	}

	// Token: 0x06001179 RID: 4473 RVA: 0x0008D590 File Offset: 0x0008B790
	public static void SetBagTimer(global::SleepingBag bag, Vector3 position, global::SleepingBag.SleepingBagResetReason reason)
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		float? num = null;
		if (activeGameMode != null)
		{
			num = activeGameMode.EvaluateSleepingBagReset(bag, position, reason);
		}
		if (num != null)
		{
			bag.SetUnlockTime(UnityEngine.Time.realtimeSinceStartup + num.Value);
			return;
		}
		if (reason == global::SleepingBag.SleepingBagResetReason.Respawned && Vector3.Distance(position, bag.transform.position) <= ConVar.Server.respawnresetrange)
		{
			bag.SetUnlockTime(UnityEngine.Time.realtimeSinceStartup + bag.secondsBetweenReuses);
			bag.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		if (reason == global::SleepingBag.SleepingBagResetReason.Placed)
		{
			float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
			foreach (global::SleepingBag sleepingBag in (from x in global::SleepingBag.sleepingBags
			where x.deployerUserID != 0UL && x.deployerUserID == bag.deployerUserID && x.unlockTime > UnityEngine.Time.realtimeSinceStartup
			select x).ToArray<global::SleepingBag>())
			{
				if (bag.unlockTime > realtimeSinceStartup && Vector3.Distance(sleepingBag.transform.position, position) <= ConVar.Server.respawnresetrange)
				{
					realtimeSinceStartup = bag.unlockTime;
				}
			}
			bag.SetUnlockTime(Mathf.Max(realtimeSinceStartup, UnityEngine.Time.realtimeSinceStartup + bag.secondsBetweenReuses));
			bag.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x0600117A RID: 4474 RVA: 0x0008D6E0 File Offset: 0x0008B8E0
	public override void ServerInit()
	{
		base.ServerInit();
		if (!global::SleepingBag.sleepingBags.Contains(this))
		{
			global::SleepingBag.sleepingBags.Add(this);
		}
	}

	// Token: 0x0600117B RID: 4475 RVA: 0x0008D700 File Offset: 0x0008B900
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		global::SleepingBag.sleepingBags.RemoveAll((global::SleepingBag x) => x == this);
	}

	// Token: 0x0600117C RID: 4476 RVA: 0x0008D720 File Offset: 0x0008B920
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.sleepingBag = Facepunch.Pool.Get<ProtoBuf.SleepingBag>();
		info.msg.sleepingBag.name = this.niceName;
		info.msg.sleepingBag.deployerID = this.deployerUserID;
	}

	// Token: 0x0600117D RID: 4477 RVA: 0x0008D770 File Offset: 0x0008B970
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void Rename(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		string text = msg.read.String(256);
		text = WordFilter.Filter(text);
		if (string.IsNullOrEmpty(text))
		{
			text = "Unnamed Sleeping Bag";
		}
		if (text.Length > 24)
		{
			text = text.Substring(0, 22) + "..";
		}
		this.niceName = text;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600117E RID: 4478 RVA: 0x0008D7E0 File Offset: 0x0008B9E0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void AssignToFriend(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (this.deployerUserID != msg.player.userID)
		{
			return;
		}
		ulong num = msg.read.UInt64();
		if (num == 0UL)
		{
			return;
		}
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode != null)
		{
			BaseGameMode.CanAssignBedResult? canAssignBedResult = activeGameMode.CanAssignBed(msg.player, this, num, 1, -1, null);
			if (canAssignBedResult != null)
			{
				global::BasePlayer basePlayer = global::RelationshipManager.FindByID(num);
				if (!canAssignBedResult.Value.Result)
				{
					msg.player.ShowToast(GameTip.Styles.Red_Normal, this.cannotAssignBedPhrase, new string[]
					{
						((basePlayer != null) ? basePlayer.displayName : null) ?? "other player"
					});
				}
				else if (basePlayer != null)
				{
					global::BasePlayer basePlayer2 = basePlayer;
					GameTip.Styles style = GameTip.Styles.Blue_Long;
					Translate.Phrase phrase = this.assignedBagPhrase;
					string[] array = new string[2];
					int num2 = 0;
					BaseGameMode.CanAssignBedResult value = canAssignBedResult.Value;
					array[num2] = value.Count.ToString();
					int num3 = 1;
					value = canAssignBedResult.Value;
					array[num3] = value.Max.ToString();
					basePlayer2.ShowToast(style, phrase, array);
				}
				if (!canAssignBedResult.Value.Result)
				{
					return;
				}
			}
		}
		this.deployerUserID = num;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		this.CheckForOnlineAndDead();
	}

	// Token: 0x0600117F RID: 4479 RVA: 0x0008D900 File Offset: 0x0008BB00
	private void CheckForOnlineAndDead()
	{
		if (this.deployerUserID == 0UL)
		{
			return;
		}
		global::BasePlayer basePlayer = global::BasePlayer.FindByID(this.deployerUserID);
		if (basePlayer != null && basePlayer.IsConnected && basePlayer.IsDead())
		{
			basePlayer.SendRespawnOptions();
		}
	}

	// Token: 0x06001180 RID: 4480 RVA: 0x0008D944 File Offset: 0x0008BB44
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public virtual void RPC_MakePublic(global::BaseEntity.RPCMessage msg)
	{
		if (!this.canBePublic)
		{
			return;
		}
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (this.deployerUserID != msg.player.userID && !msg.player.CanBuild())
		{
			return;
		}
		bool flag = msg.read.Bit();
		if (flag == this.IsPublic())
		{
			return;
		}
		this.SetPublic(flag);
		if (!this.IsPublic())
		{
			BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
			BaseGameMode.CanAssignBedResult? canAssignBedResult = (activeGameMode != null) ? activeGameMode.CanAssignBed(msg.player, this, msg.player.userID, 1, 0, this) : null;
			if (canAssignBedResult != null)
			{
				if (canAssignBedResult.Value.Result)
				{
					global::BasePlayer player = msg.player;
					GameTip.Styles style = GameTip.Styles.Blue_Long;
					Translate.Phrase phrase = global::SleepingBag.bagLimitPhrase;
					string[] array = new string[2];
					int num = 0;
					BaseGameMode.CanAssignBedResult value = canAssignBedResult.Value;
					array[num] = value.Count.ToString();
					int num2 = 1;
					value = canAssignBedResult.Value;
					array[num2] = value.Max.ToString();
					player.ShowToast(style, phrase, array);
				}
				else
				{
					global::BasePlayer player2 = msg.player;
					GameTip.Styles style2 = GameTip.Styles.Blue_Long;
					Translate.Phrase phrase2 = this.cannotMakeBedPhrase;
					string[] array2 = new string[2];
					int num3 = 0;
					BaseGameMode.CanAssignBedResult value = canAssignBedResult.Value;
					array2[num3] = value.Count.ToString();
					int num4 = 1;
					value = canAssignBedResult.Value;
					array2[num4] = value.Max.ToString();
					player2.ShowToast(style2, phrase2, array2);
				}
				if (!canAssignBedResult.Value.Result)
				{
					return;
				}
			}
			this.deployerUserID = msg.player.userID;
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001181 RID: 4481 RVA: 0x0008DAB0 File Offset: 0x0008BCB0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_MakeBed(global::BaseEntity.RPCMessage msg)
	{
		if (!this.canBePublic || !this.IsPublic())
		{
			return;
		}
		if (!msg.player.CanInteract())
		{
			return;
		}
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		BaseGameMode.CanAssignBedResult? canAssignBedResult = (activeGameMode != null) ? activeGameMode.CanAssignBed(msg.player, this, msg.player.userID, 1, 0, this) : null;
		if (canAssignBedResult != null)
		{
			if (!canAssignBedResult.Value.Result)
			{
				msg.player.ShowToast(GameTip.Styles.Red_Normal, this.cannotMakeBedPhrase, Array.Empty<string>());
			}
			else
			{
				global::BasePlayer player = msg.player;
				GameTip.Styles style = GameTip.Styles.Blue_Long;
				Translate.Phrase phrase = global::SleepingBag.bagLimitPhrase;
				string[] array = new string[2];
				int num = 0;
				BaseGameMode.CanAssignBedResult value = canAssignBedResult.Value;
				array[num] = value.Count.ToString();
				int num2 = 1;
				value = canAssignBedResult.Value;
				array[num2] = value.Max.ToString();
				player.ShowToast(style, phrase, array);
			}
			if (!canAssignBedResult.Value.Result)
			{
				return;
			}
		}
		this.deployerUserID = msg.player.userID;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001182 RID: 4482 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void PostPlayerSpawn(global::BasePlayer p)
	{
	}

	// Token: 0x06001183 RID: 4483 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool IsOccupied()
	{
		return false;
	}

	// Token: 0x06001184 RID: 4484 RVA: 0x0008DBA8 File Offset: 0x0008BDA8
	public override string Admin_Who()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(base.Admin_Who());
		stringBuilder.AppendLine(string.Format("Assigned bag ID: {0}", this.deployerUserID));
		stringBuilder.AppendLine("Assigned player name: " + Admin.GetPlayerName(this.deployerUserID));
		stringBuilder.AppendLine("Bag Name:" + this.niceName);
		return stringBuilder.ToString();
	}

	// Token: 0x06001185 RID: 4485 RVA: 0x0008DC1C File Offset: 0x0008BE1C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.sleepingBag != null)
		{
			this.niceName = info.msg.sleepingBag.name;
			this.deployerUserID = info.msg.sleepingBag.deployerID;
		}
	}

	// Token: 0x06001186 RID: 4486 RVA: 0x0008DC69 File Offset: 0x0008BE69
	public override bool CanPickup(global::BasePlayer player)
	{
		return base.CanPickup(player) && player.userID == this.deployerUserID;
	}

	// Token: 0x04000AD8 RID: 2776
	[NonSerialized]
	public ulong deployerUserID;

	// Token: 0x04000AD9 RID: 2777
	public GameObject renameDialog;

	// Token: 0x04000ADA RID: 2778
	public GameObject assignDialog;

	// Token: 0x04000ADB RID: 2779
	public float secondsBetweenReuses = 300f;

	// Token: 0x04000ADC RID: 2780
	public string niceName = "Unnamed Bag";

	// Token: 0x04000ADD RID: 2781
	public Vector3 spawnOffset = Vector3.zero;

	// Token: 0x04000ADE RID: 2782
	public RespawnInformation.SpawnOptions.RespawnType RespawnType = RespawnInformation.SpawnOptions.RespawnType.SleepingBag;

	// Token: 0x04000ADF RID: 2783
	public bool isStatic;

	// Token: 0x04000AE0 RID: 2784
	public bool canBePublic;

	// Token: 0x04000AE1 RID: 2785
	public const global::BaseEntity.Flags IsPublicFlag = global::BaseEntity.Flags.Reserved3;

	// Token: 0x04000AE2 RID: 2786
	public static Translate.Phrase bagLimitPhrase = new Translate.Phrase("bag_limit_update", "You are now at {0}/{1} bags");

	// Token: 0x04000AE3 RID: 2787
	public static Translate.Phrase bagLimitReachedPhrase = new Translate.Phrase("bag_limit_reached", "You have reached your bag limit!");

	// Token: 0x04000AE4 RID: 2788
	public Translate.Phrase assignOtherBagPhrase = new Translate.Phrase("assigned_other_bag_limit", "You have assigned {0} a bag, they are now at {0}/{1} bags");

	// Token: 0x04000AE5 RID: 2789
	public Translate.Phrase assignedBagPhrase = new Translate.Phrase("assigned_bag_limit", "You have been assigned a bag, you are now at {0}/{1} bags");

	// Token: 0x04000AE6 RID: 2790
	public Translate.Phrase cannotAssignBedPhrase = new Translate.Phrase("cannot_assign_bag_limit", "You cannot assign {0} a bag, they have reached their bag limit!");

	// Token: 0x04000AE7 RID: 2791
	public Translate.Phrase cannotMakeBedPhrase = new Translate.Phrase("cannot_make_bed_limit", "You cannot take ownership of the bed, you are at your bag limit");

	// Token: 0x04000AE8 RID: 2792
	internal float unlockTime;

	// Token: 0x04000AE9 RID: 2793
	public static List<global::SleepingBag> sleepingBags = new List<global::SleepingBag>();

	// Token: 0x02000BB1 RID: 2993
	public enum SleepingBagResetReason
	{
		// Token: 0x04003F2B RID: 16171
		Respawned,
		// Token: 0x04003F2C RID: 16172
		Placed,
		// Token: 0x04003F2D RID: 16173
		Death
	}
}
