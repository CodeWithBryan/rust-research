using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x02000038 RID: 56
public class BaseHelicopter : BaseCombatEntity
{
	// Token: 0x060003A5 RID: 933 RVA: 0x0002E488 File Offset: 0x0002C688
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseHelicopter.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x0002E4C8 File Offset: 0x0002C6C8
	public void InitalizeWeakspots()
	{
		BaseHelicopter.weakspot[] array = this.weakspots;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].body = this;
		}
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x0002E4F3 File Offset: 0x0002C6F3
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if (base.isServer)
		{
			this.myAI.WasAttacked(info);
		}
	}

	// Token: 0x060003A8 RID: 936 RVA: 0x0002E510 File Offset: 0x0002C710
	public override void Hurt(HitInfo info)
	{
		bool flag = false;
		if (info.damageTypes.Total() >= base.health)
		{
			base.health = 1000000f;
			this.myAI.CriticalDamage();
			flag = true;
		}
		base.Hurt(info);
		if (!flag)
		{
			foreach (BaseHelicopter.weakspot weakspot in this.weakspots)
			{
				foreach (string str in weakspot.bonenames)
				{
					if (info.HitBone == StringPool.Get(str))
					{
						weakspot.Hurt(info.damageTypes.Total(), info);
						this.myAI.WeakspotDamaged(weakspot, info);
					}
				}
			}
		}
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x00027640 File Offset: 0x00025840
	public override float MaxVelocity()
	{
		return 100f;
	}

	// Token: 0x060003AA RID: 938 RVA: 0x0002E5BD File Offset: 0x0002C7BD
	public override void InitShared()
	{
		base.InitShared();
		this.InitalizeWeakspots();
	}

	// Token: 0x060003AB RID: 939 RVA: 0x0002E5CB File Offset: 0x0002C7CB
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.helicopter != null)
		{
			this.spotlightTarget = info.msg.helicopter.spotlightVec;
		}
	}

	// Token: 0x060003AC RID: 940 RVA: 0x0002E5F8 File Offset: 0x0002C7F8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.helicopter = Pool.Get<Helicopter>();
		info.msg.helicopter.tiltRot = this.rotorPivot.transform.localRotation.eulerAngles;
		info.msg.helicopter.spotlightVec = this.spotlightTarget;
		info.msg.helicopter.weakspothealths = Pool.Get<List<float>>();
		for (int i = 0; i < this.weakspots.Length; i++)
		{
			info.msg.helicopter.weakspothealths.Add(this.weakspots[i].health);
		}
	}

	// Token: 0x060003AD RID: 941 RVA: 0x0002E6A4 File Offset: 0x0002C8A4
	public override void ServerInit()
	{
		base.ServerInit();
		this.myAI = base.GetComponent<PatrolHelicopterAI>();
		if (!this.myAI.hasInterestZone)
		{
			this.myAI.SetInitialDestination(Vector3.zero, 1.25f);
			this.myAI.targetThrottleSpeed = 1f;
			this.myAI.ExitCurrentState();
			this.myAI.State_Patrol_Enter();
		}
		this.CreateMapMarker();
	}

	// Token: 0x060003AE RID: 942 RVA: 0x0002E714 File Offset: 0x0002C914
	public void CreateMapMarker()
	{
		if (this.mapMarkerInstance)
		{
			this.mapMarkerInstance.Kill(global::BaseNetworkable.DestroyMode.None);
		}
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.mapMarkerEntityPrefab.resourcePath, Vector3.zero, Quaternion.identity, true);
		baseEntity.SetParent(this, false, false);
		baseEntity.Spawn();
		this.mapMarkerInstance = baseEntity;
	}

	// Token: 0x060003AF RID: 943 RVA: 0x0002E771 File Offset: 0x0002C971
	public override void OnPositionalNetworkUpdate()
	{
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		base.OnPositionalNetworkUpdate();
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x0002E780 File Offset: 0x0002C980
	public void CreateExplosionMarker(float durationMinutes)
	{
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.debrisFieldMarker.resourcePath, base.transform.position, Quaternion.identity, true);
		baseEntity.Spawn();
		baseEntity.SendMessage("SetDuration", durationMinutes, SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x060003B1 RID: 945 RVA: 0x0002E7C0 File Offset: 0x0002C9C0
	public override void OnKilled(HitInfo info)
	{
		if (base.isClient)
		{
			return;
		}
		this.CreateExplosionMarker(10f);
		Effect.server.Run(this.explosionEffect.resourcePath, base.transform.position, Vector3.up, null, true);
		Vector3 vector = this.myAI.GetLastMoveDir() * this.myAI.GetMoveSpeed() * 0.75f;
		GameObject gibSource = this.servergibs.Get().GetComponent<global::ServerGib>()._gibSource;
		List<global::ServerGib> list = global::ServerGib.CreateGibs(this.servergibs.resourcePath, base.gameObject, gibSource, vector, 3f);
		if (info.damageTypes.GetMajorityDamageType() != DamageType.Decay)
		{
			for (int i = 0; i < 12 - this.maxCratesToSpawn; i++)
			{
				global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.fireBall.resourcePath, base.transform.position, base.transform.rotation, true);
				if (baseEntity)
				{
					float min = 3f;
					float max = 10f;
					Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
					baseEntity.transform.position = base.transform.position + new Vector3(0f, 1.5f, 0f) + onUnitSphere * UnityEngine.Random.Range(-4f, 4f);
					Collider component = baseEntity.GetComponent<Collider>();
					baseEntity.Spawn();
					baseEntity.SetVelocity(vector + onUnitSphere * UnityEngine.Random.Range(min, max));
					foreach (global::ServerGib serverGib in list)
					{
						Physics.IgnoreCollision(component, serverGib.GetCollider(), true);
					}
				}
			}
		}
		for (int j = 0; j < this.maxCratesToSpawn; j++)
		{
			Vector3 onUnitSphere2 = UnityEngine.Random.onUnitSphere;
			Vector3 pos = base.transform.position + new Vector3(0f, 1.5f, 0f) + onUnitSphere2 * UnityEngine.Random.Range(2f, 3f);
			global::BaseEntity baseEntity2 = GameManager.server.CreateEntity(this.crateToDrop.resourcePath, pos, Quaternion.LookRotation(onUnitSphere2), true);
			baseEntity2.Spawn();
			LootContainer lootContainer = baseEntity2 as LootContainer;
			if (lootContainer)
			{
				lootContainer.Invoke(new Action(lootContainer.RemoveMe), 1800f);
			}
			Collider component2 = baseEntity2.GetComponent<Collider>();
			Rigidbody rigidbody = baseEntity2.gameObject.AddComponent<Rigidbody>();
			rigidbody.useGravity = true;
			rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			rigidbody.mass = 2f;
			rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			rigidbody.velocity = vector + onUnitSphere2 * UnityEngine.Random.Range(1f, 3f);
			rigidbody.angularVelocity = Vector3Ex.Range(-1.75f, 1.75f);
			rigidbody.drag = 0.5f * (rigidbody.mass / 5f);
			rigidbody.angularDrag = 0.2f * (rigidbody.mass / 5f);
			FireBall fireBall = GameManager.server.CreateEntity(this.fireBall.resourcePath, default(Vector3), default(Quaternion), true) as FireBall;
			if (fireBall)
			{
				fireBall.SetParent(baseEntity2, false, false);
				fireBall.Spawn();
				fireBall.GetComponent<Rigidbody>().isKinematic = true;
				fireBall.GetComponent<Collider>().enabled = false;
			}
			baseEntity2.SendMessage("SetLockingEnt", fireBall.gameObject, SendMessageOptions.DontRequireReceiver);
			foreach (global::ServerGib serverGib2 in list)
			{
				Physics.IgnoreCollision(component2, serverGib2.GetCollider(), true);
			}
		}
		base.OnKilled(info);
	}

	// Token: 0x060003B2 RID: 946 RVA: 0x0002EBC4 File Offset: 0x0002CDC4
	public void Update()
	{
		if (base.isServer && Time.realtimeSinceStartup - this.lastNetworkUpdate >= 0.25f)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.lastNetworkUpdate = Time.realtimeSinceStartup;
		}
	}

	// Token: 0x040002BB RID: 699
	public BaseHelicopter.weakspot[] weakspots;

	// Token: 0x040002BC RID: 700
	public GameObject rotorPivot;

	// Token: 0x040002BD RID: 701
	public GameObject mainRotor;

	// Token: 0x040002BE RID: 702
	public GameObject mainRotor_blades;

	// Token: 0x040002BF RID: 703
	public GameObject mainRotor_blur;

	// Token: 0x040002C0 RID: 704
	public GameObject tailRotor;

	// Token: 0x040002C1 RID: 705
	public GameObject tailRotor_blades;

	// Token: 0x040002C2 RID: 706
	public GameObject tailRotor_blur;

	// Token: 0x040002C3 RID: 707
	public GameObject rocket_tube_left;

	// Token: 0x040002C4 RID: 708
	public GameObject rocket_tube_right;

	// Token: 0x040002C5 RID: 709
	public GameObject left_gun_yaw;

	// Token: 0x040002C6 RID: 710
	public GameObject left_gun_pitch;

	// Token: 0x040002C7 RID: 711
	public GameObject left_gun_muzzle;

	// Token: 0x040002C8 RID: 712
	public GameObject right_gun_yaw;

	// Token: 0x040002C9 RID: 713
	public GameObject right_gun_pitch;

	// Token: 0x040002CA RID: 714
	public GameObject right_gun_muzzle;

	// Token: 0x040002CB RID: 715
	public GameObject spotlight_rotation;

	// Token: 0x040002CC RID: 716
	public GameObjectRef rocket_fire_effect;

	// Token: 0x040002CD RID: 717
	public GameObjectRef gun_fire_effect;

	// Token: 0x040002CE RID: 718
	public GameObjectRef bulletEffect;

	// Token: 0x040002CF RID: 719
	public GameObjectRef explosionEffect;

	// Token: 0x040002D0 RID: 720
	public GameObjectRef fireBall;

	// Token: 0x040002D1 RID: 721
	public GameObjectRef crateToDrop;

	// Token: 0x040002D2 RID: 722
	public int maxCratesToSpawn = 4;

	// Token: 0x040002D3 RID: 723
	public float bulletSpeed = 250f;

	// Token: 0x040002D4 RID: 724
	public float bulletDamage = 20f;

	// Token: 0x040002D5 RID: 725
	public GameObjectRef servergibs;

	// Token: 0x040002D6 RID: 726
	public GameObjectRef debrisFieldMarker;

	// Token: 0x040002D7 RID: 727
	public SoundDefinition rotorWashSoundDef;

	// Token: 0x040002D8 RID: 728
	private Sound _rotorWashSound;

	// Token: 0x040002D9 RID: 729
	public SoundDefinition flightEngineSoundDef;

	// Token: 0x040002DA RID: 730
	public SoundDefinition flightThwopsSoundDef;

	// Token: 0x040002DB RID: 731
	private Sound flightEngineSound;

	// Token: 0x040002DC RID: 732
	private Sound flightThwopsSound;

	// Token: 0x040002DD RID: 733
	private SoundModulation.Modulator flightEngineGainMod;

	// Token: 0x040002DE RID: 734
	private SoundModulation.Modulator flightThwopsGainMod;

	// Token: 0x040002DF RID: 735
	public float rotorGainModSmoothing = 0.25f;

	// Token: 0x040002E0 RID: 736
	public float engineGainMin = 0.5f;

	// Token: 0x040002E1 RID: 737
	public float engineGainMax = 1f;

	// Token: 0x040002E2 RID: 738
	public float thwopGainMin = 0.5f;

	// Token: 0x040002E3 RID: 739
	public float thwopGainMax = 1f;

	// Token: 0x040002E4 RID: 740
	public float spotlightJitterAmount = 5f;

	// Token: 0x040002E5 RID: 741
	public float spotlightJitterSpeed = 5f;

	// Token: 0x040002E6 RID: 742
	public GameObject[] nightLights;

	// Token: 0x040002E7 RID: 743
	public Vector3 spotlightTarget;

	// Token: 0x040002E8 RID: 744
	public float engineSpeed = 1f;

	// Token: 0x040002E9 RID: 745
	public float targetEngineSpeed = 1f;

	// Token: 0x040002EA RID: 746
	public float blur_rotationScale = 0.05f;

	// Token: 0x040002EB RID: 747
	public ParticleSystem[] _rotorWashParticles;

	// Token: 0x040002EC RID: 748
	private PatrolHelicopterAI myAI;

	// Token: 0x040002ED RID: 749
	public GameObjectRef mapMarkerEntityPrefab;

	// Token: 0x040002EE RID: 750
	private float lastNetworkUpdate = float.NegativeInfinity;

	// Token: 0x040002EF RID: 751
	private const float networkUpdateRate = 0.25f;

	// Token: 0x040002F0 RID: 752
	private global::BaseEntity mapMarkerInstance;

	// Token: 0x02000B49 RID: 2889
	[Serializable]
	public class weakspot
	{
		// Token: 0x06004A54 RID: 19028 RVA: 0x0018FCE8 File Offset: 0x0018DEE8
		public float HealthFraction()
		{
			return this.health / this.maxHealth;
		}

		// Token: 0x06004A55 RID: 19029 RVA: 0x0018FCF8 File Offset: 0x0018DEF8
		public void Hurt(float amount, HitInfo info)
		{
			if (this.isDestroyed)
			{
				return;
			}
			this.health -= amount;
			Effect.server.Run(this.damagedParticles.resourcePath, this.body, StringPool.Get(this.bonenames[UnityEngine.Random.Range(0, this.bonenames.Length)]), Vector3.zero, Vector3.up, null, true);
			if (this.health <= 0f)
			{
				this.health = 0f;
				this.WeakspotDestroyed();
			}
		}

		// Token: 0x06004A56 RID: 19030 RVA: 0x0018FD76 File Offset: 0x0018DF76
		public void Heal(float amount)
		{
			this.health += amount;
		}

		// Token: 0x06004A57 RID: 19031 RVA: 0x0018FD88 File Offset: 0x0018DF88
		public void WeakspotDestroyed()
		{
			this.isDestroyed = true;
			Effect.server.Run(this.destroyedParticles.resourcePath, this.body, StringPool.Get(this.bonenames[UnityEngine.Random.Range(0, this.bonenames.Length)]), Vector3.zero, Vector3.up, null, true);
			this.body.Hurt(this.body.MaxHealth() * this.healthFractionOnDestroyed, DamageType.Generic, null, false);
		}

		// Token: 0x04003D56 RID: 15702
		[NonSerialized]
		public BaseHelicopter body;

		// Token: 0x04003D57 RID: 15703
		public string[] bonenames;

		// Token: 0x04003D58 RID: 15704
		public float maxHealth;

		// Token: 0x04003D59 RID: 15705
		public float health;

		// Token: 0x04003D5A RID: 15706
		public float healthFractionOnDestroyed = 0.5f;

		// Token: 0x04003D5B RID: 15707
		public GameObjectRef destroyedParticles;

		// Token: 0x04003D5C RID: 15708
		public GameObjectRef damagedParticles;

		// Token: 0x04003D5D RID: 15709
		public GameObject damagedEffect;

		// Token: 0x04003D5E RID: 15710
		public GameObject destroyedEffect;

		// Token: 0x04003D5F RID: 15711
		public List<global::BasePlayer> attackers;

		// Token: 0x04003D60 RID: 15712
		private bool isDestroyed;
	}
}
