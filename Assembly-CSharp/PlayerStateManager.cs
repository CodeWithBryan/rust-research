using System;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000717 RID: 1815
public class PlayerStateManager
{
	// Token: 0x06003236 RID: 12854 RVA: 0x001347E1 File Offset: 0x001329E1
	public PlayerStateManager(UserPersistance persistence)
	{
		this._cache = new MruDictionary<ulong, PlayerState>(1000, new Action<ulong, PlayerState>(this.FreeOldState));
		this._persistence = persistence;
	}

	// Token: 0x06003237 RID: 12855 RVA: 0x0013480C File Offset: 0x00132A0C
	public PlayerState Get(ulong playerId)
	{
		PlayerState result;
		using (TimeWarning.New("PlayerStateManager.Get", 0))
		{
			PlayerState playerState;
			if (this._cache.TryGetValue(playerId, out playerState))
			{
				result = playerState;
			}
			else
			{
				byte[] playerState2 = this._persistence.GetPlayerState(playerId);
				PlayerState playerState3;
				if (playerState2 != null && playerState2.Length != 0)
				{
					try
					{
						playerState3 = PlayerState.Deserialize(playerState2);
						this.OnPlayerStateLoaded(playerState3);
						this._cache.Add(playerId, playerState3);
						return playerState3;
					}
					catch (Exception arg)
					{
						Debug.LogError(string.Format("Failed to load player state for {0}: {1}", playerId, arg));
					}
				}
				playerState3 = Pool.Get<PlayerState>();
				this._cache.Add(playerId, playerState3);
				result = playerState3;
			}
		}
		return result;
	}

	// Token: 0x06003238 RID: 12856 RVA: 0x001348CC File Offset: 0x00132ACC
	public void Save(ulong playerId)
	{
		PlayerState state;
		if (!this._cache.TryGetValue(playerId, out state))
		{
			return;
		}
		this.SaveState(playerId, state);
	}

	// Token: 0x06003239 RID: 12857 RVA: 0x001348F4 File Offset: 0x00132AF4
	private void SaveState(ulong playerId, PlayerState state)
	{
		using (TimeWarning.New("PlayerStateManager.SaveState", 0))
		{
			try
			{
				byte[] state2 = PlayerState.SerializeToBytes(state);
				this._persistence.SetPlayerState(playerId, state2);
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Failed to save player state for {0}: {1}", playerId, arg));
			}
		}
	}

	// Token: 0x0600323A RID: 12858 RVA: 0x00134964 File Offset: 0x00132B64
	private void FreeOldState(ulong playerId, PlayerState state)
	{
		this.SaveState(playerId, state);
		state.Dispose();
	}

	// Token: 0x0600323B RID: 12859 RVA: 0x00134974 File Offset: 0x00132B74
	public void Reset(ulong playerId)
	{
		this._cache.Remove(playerId);
		this._persistence.ResetPlayerState(playerId);
	}

	// Token: 0x0600323C RID: 12860 RVA: 0x0013498E File Offset: 0x00132B8E
	private void OnPlayerStateLoaded(PlayerState state)
	{
		state.unHostileTimestamp = Math.Min(state.unHostileTimestamp, TimeEx.currentTimestamp + 1800.0);
	}

	// Token: 0x040028BD RID: 10429
	private readonly MruDictionary<ulong, PlayerState> _cache;

	// Token: 0x040028BE RID: 10430
	private readonly UserPersistance _persistence;
}
