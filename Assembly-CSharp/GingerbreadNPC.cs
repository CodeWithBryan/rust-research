using System;
using System.Runtime.CompilerServices;
using ConVar;
using ProtoBuf;
using UnityEngine;

// Token: 0x020001D9 RID: 473
public class GingerbreadNPC : HumanNPC, IClientBrainStateListener
{
	// Token: 0x060018CB RID: 6347 RVA: 0x000B540F File Offset: 0x000B360F
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		info.HitMaterial = Global.GingerbreadMaterialID();
	}

	// Token: 0x060018CC RID: 6348 RVA: 0x000B5423 File Offset: 0x000B3623
	public override string Categorize()
	{
		return "Gingerbread";
	}

	// Token: 0x060018CD RID: 6349 RVA: 0x00007074 File Offset: 0x00005274
	public override bool ShouldDropActiveItem()
	{
		return false;
	}

	// Token: 0x060018CE RID: 6350 RVA: 0x000B542C File Offset: 0x000B362C
	public override BaseCorpse CreateCorpse()
	{
		BaseCorpse result;
		using (TimeWarning.New("Create corpse", 0))
		{
			string corpseResourcePath = this.CorpseResourcePath;
			NPCPlayerCorpse npcplayerCorpse = base.DropCorpse(corpseResourcePath) as NPCPlayerCorpse;
			if (npcplayerCorpse)
			{
				npcplayerCorpse.transform.position = npcplayerCorpse.transform.position + Vector3.down * this.NavAgent.baseOffset;
				npcplayerCorpse.SetLootableIn(2f);
				npcplayerCorpse.SetFlag(global::BaseEntity.Flags.Reserved5, base.HasPlayerFlag(global::BasePlayer.PlayerFlags.DisplaySash), false, true);
				npcplayerCorpse.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
				npcplayerCorpse.TakeFrom(new global::ItemContainer[]
				{
					this.inventory.containerMain
				});
				npcplayerCorpse.playerName = "Gingerbread";
				npcplayerCorpse.playerSteamID = this.userID;
				npcplayerCorpse.Spawn();
				global::ItemContainer[] containers = npcplayerCorpse.containers;
				for (int i = 0; i < containers.Length; i++)
				{
					containers[i].Clear();
				}
				if (this.LootSpawnSlots.Length != 0)
				{
					foreach (LootContainer.LootSpawnSlot lootSpawnSlot in this.LootSpawnSlots)
					{
						for (int j = 0; j < lootSpawnSlot.numberToSpawn; j++)
						{
							if (UnityEngine.Random.Range(0f, 1f) <= lootSpawnSlot.probability)
							{
								lootSpawnSlot.definition.SpawnIntoContainer(npcplayerCorpse.containers[0]);
							}
						}
					}
				}
			}
			result = npcplayerCorpse;
		}
		return result;
	}

	// Token: 0x060018CF RID: 6351 RVA: 0x000B55BC File Offset: 0x000B37BC
	public override void AttackerInfo(PlayerLifeStory.DeathInfo info)
	{
		base.AttackerInfo(info);
		info.inflictorName = this.inventory.containerBelt.GetSlot(0).info.shortname;
		info.attackerName = base.ShortPrefabName;
	}

	// Token: 0x170001E0 RID: 480
	// (get) Token: 0x060018D0 RID: 6352 RVA: 0x000B55F4 File Offset: 0x000B37F4
	protected string CorpseResourcePath
	{
		get
		{
			bool flag = GingerbreadNPC.<get_CorpseResourcePath>g__GetFloatBasedOnUserID|10_0(this.userID, 4332UL) > 0.5f;
			if (this.OverrideCorpseMale.isValid && !flag)
			{
				return this.OverrideCorpseMale.resourcePath;
			}
			if (this.OverrideCorpseFemale.isValid && flag)
			{
				return this.OverrideCorpseFemale.resourcePath;
			}
			return "assets/prefabs/npc/murderer/murderer_corpse.prefab";
		}
	}

	// Token: 0x060018D1 RID: 6353 RVA: 0x000059DD File Offset: 0x00003BDD
	public void OnClientStateChanged(AIState state)
	{
	}

	// Token: 0x060018D3 RID: 6355 RVA: 0x000B5664 File Offset: 0x000B3864
	[CompilerGenerated]
	internal static float <get_CorpseResourcePath>g__GetFloatBasedOnUserID|10_0(ulong steamid, ulong seed)
	{
		UnityEngine.Random.State state = UnityEngine.Random.state;
		UnityEngine.Random.InitState((int)(seed + steamid));
		float result = UnityEngine.Random.Range(0f, 1f);
		UnityEngine.Random.state = state;
		return result;
	}

	// Token: 0x040011C5 RID: 4549
	public GameObjectRef OverrideCorpseMale;

	// Token: 0x040011C6 RID: 4550
	public GameObjectRef OverrideCorpseFemale;

	// Token: 0x040011C7 RID: 4551
	public PhysicMaterial HitMaterial;

	// Token: 0x040011C8 RID: 4552
	public bool RoamAroundHomePoint;
}
