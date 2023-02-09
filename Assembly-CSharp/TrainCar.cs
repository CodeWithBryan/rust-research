using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

// Token: 0x020000DC RID: 220
public class TrainCar : global::BaseVehicle, TriggerHurtNotChild.IHurtTriggerUser, TrainTrackSpline.ITrainTrackUser, ITrainCollidable, IPrefabPreProcess
{
	// Token: 0x060012E0 RID: 4832 RVA: 0x00097298 File Offset: 0x00095498
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TrainCar.OnRpcMessage", 0))
		{
			if (rpc == 3930273067U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_WantsUncouple ");
				}
				using (TimeWarning.New("RPC_WantsUncouple", 0))
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
							this.RPC_WantsUncouple(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_WantsUncouple");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700018E RID: 398
	// (get) Token: 0x060012E1 RID: 4833 RVA: 0x000299AB File Offset: 0x00027BAB
	public Vector3 Position
	{
		get
		{
			return base.transform.position;
		}
	}

	// Token: 0x1700018F RID: 399
	// (get) Token: 0x060012E2 RID: 4834 RVA: 0x000973BC File Offset: 0x000955BC
	// (set) Token: 0x060012E3 RID: 4835 RVA: 0x000973C4 File Offset: 0x000955C4
	public float FrontWheelSplineDist { get; private set; }

	// Token: 0x17000190 RID: 400
	// (get) Token: 0x060012E4 RID: 4836 RVA: 0x000973CD File Offset: 0x000955CD
	public bool FrontAtEndOfLine
	{
		get
		{
			return this.frontAtEndOfLine;
		}
	}

	// Token: 0x17000191 RID: 401
	// (get) Token: 0x060012E5 RID: 4837 RVA: 0x000973D5 File Offset: 0x000955D5
	public bool RearAtEndOfLine
	{
		get
		{
			return this.rearAtEndOfLine;
		}
	}

	// Token: 0x17000192 RID: 402
	// (get) Token: 0x060012E6 RID: 4838 RVA: 0x00007074 File Offset: 0x00005274
	protected virtual bool networkUpdateOnCompleteTrainChange
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000193 RID: 403
	// (get) Token: 0x060012E7 RID: 4839 RVA: 0x000973DD File Offset: 0x000955DD
	// (set) Token: 0x060012E8 RID: 4840 RVA: 0x000973E8 File Offset: 0x000955E8
	public TrainTrackSpline FrontTrackSection
	{
		get
		{
			return this._frontTrackSection;
		}
		private set
		{
			if (this._frontTrackSection != value)
			{
				if (this._frontTrackSection != null)
				{
					this._frontTrackSection.DeregisterTrackUser(this);
				}
				this._frontTrackSection = value;
				if (this._frontTrackSection != null)
				{
					this._frontTrackSection.RegisterTrackUser(this);
				}
			}
		}
	}

	// Token: 0x17000194 RID: 404
	// (get) Token: 0x060012E9 RID: 4841 RVA: 0x0009743E File Offset: 0x0009563E
	// (set) Token: 0x060012EA RID: 4842 RVA: 0x00097446 File Offset: 0x00095646
	public TrainTrackSpline RearTrackSection { get; private set; }

	// Token: 0x17000195 RID: 405
	// (get) Token: 0x060012EB RID: 4843 RVA: 0x0009744F File Offset: 0x0009564F
	protected bool IsAtAStation
	{
		get
		{
			return this.FrontTrackSection != null && this.FrontTrackSection.isStation;
		}
	}

	// Token: 0x17000196 RID: 406
	// (get) Token: 0x060012EC RID: 4844 RVA: 0x0009746C File Offset: 0x0009566C
	protected bool IsOnAboveGroundSpawnRail
	{
		get
		{
			return this.FrontTrackSection != null && this.FrontTrackSection.aboveGroundSpawn;
		}
	}

	// Token: 0x17000197 RID: 407
	// (get) Token: 0x060012ED RID: 4845 RVA: 0x00097489 File Offset: 0x00095689
	private bool RecentlySpawned
	{
		get
		{
			return UnityEngine.Time.time < this.initialSpawnTime + 2f;
		}
	}

	// Token: 0x060012EE RID: 4846 RVA: 0x000974A0 File Offset: 0x000956A0
	public override void ServerInit()
	{
		base.ServerInit();
		this.spawnOrigin = base.transform.position;
		this.distFrontToBackWheel = Vector3.Distance(this.GetFrontWheelPos(), this.GetRearWheelPos());
		this.rigidBody.centerOfMass = this.centreOfMassTransform.localPosition;
		this.UpdateCompleteTrain();
		this.lastDecayTick = UnityEngine.Time.time;
		base.InvokeRandomized(new Action(this.UpdateClients), 0f, 0.15f, 0.02f);
		base.InvokeRandomized(new Action(this.DecayTick), UnityEngine.Random.Range(20f, 40f), this.decayTickSpacing, this.decayTickSpacing * 0.1f);
	}

	// Token: 0x060012EF RID: 4847 RVA: 0x00097556 File Offset: 0x00095756
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (base.health <= 0f)
		{
			this.ActualDeath();
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved3, false, false, true);
	}

	// Token: 0x060012F0 RID: 4848 RVA: 0x00097590 File Offset: 0x00095790
	public override void Spawn()
	{
		base.Spawn();
		this.initialSpawnTime = UnityEngine.Time.time;
		TrainTrackSpline trainTrackSpline;
		float frontWheelSplineDist;
		if (TrainTrackSpline.TryFindTrackNear(this.GetFrontWheelPos(), 15f, out trainTrackSpline, out frontWheelSplineDist))
		{
			this.FrontWheelSplineDist = frontWheelSplineDist;
			Vector3 targetFrontWheelTangent;
			Vector3 positionAndTangent = trainTrackSpline.GetPositionAndTangent(this.FrontWheelSplineDist, base.transform.forward, out targetFrontWheelTangent);
			this.SetTheRestFromFrontWheelData(ref trainTrackSpline, positionAndTangent, targetFrontWheelTangent, this.localTrackSelection, null, true);
			this.FrontTrackSection = trainTrackSpline;
			if (!Rust.Application.isLoadingSave && !this.SpaceIsClear())
			{
				base.Invoke(new Action(base.KillMessage), 0f);
				return;
			}
		}
		else
		{
			base.Invoke(new Action(base.KillMessage), 0f);
		}
	}

	// Token: 0x060012F1 RID: 4849 RVA: 0x0009763C File Offset: 0x0009583C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseTrain = Facepunch.Pool.Get<BaseTrain>();
		info.msg.baseTrain.time = this.GetNetworkTime();
		info.msg.baseTrain.frontBogieYRot = this.frontBogieYRot;
		info.msg.baseTrain.rearBogieYRot = this.rearBogieYRot;
		uint num;
		if (this.coupling.frontCoupling.TryGetCoupledToID(out num))
		{
			info.msg.baseTrain.frontCouplingID = num;
			info.msg.baseTrain.frontCouplingToFront = this.coupling.frontCoupling.CoupledTo.isFrontCoupling;
		}
		if (this.coupling.rearCoupling.TryGetCoupledToID(out num))
		{
			info.msg.baseTrain.rearCouplingID = num;
			info.msg.baseTrain.rearCouplingToFront = this.coupling.rearCoupling.CoupledTo.isFrontCoupling;
		}
	}

	// Token: 0x060012F2 RID: 4850 RVA: 0x00097738 File Offset: 0x00095938
	protected virtual void ServerFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		if (this.isSpawned && (next.HasFlag(global::BaseEntity.Flags.Reserved2) != old.HasFlag(global::BaseEntity.Flags.Reserved2) || next.HasFlag(global::BaseEntity.Flags.Reserved3) != old.HasFlag(global::BaseEntity.Flags.Reserved3)))
		{
			this.UpdateCompleteTrain();
		}
	}

	// Token: 0x060012F3 RID: 4851 RVA: 0x000977AC File Offset: 0x000959AC
	private void UpdateCompleteTrain()
	{
		List<TrainCar> list = Facepunch.Pool.GetList<TrainCar>();
		this.coupling.GetAll(ref list);
		if (this.completeTrain == null || !this.completeTrain.Matches(list))
		{
			this.SetNewCompleteTrain(new CompleteTrain(list));
			return;
		}
		Facepunch.Pool.FreeList<TrainCar>(ref list);
	}

	// Token: 0x060012F4 RID: 4852 RVA: 0x000977F6 File Offset: 0x000959F6
	public void SetNewCompleteTrain(CompleteTrain ct)
	{
		if (this.completeTrain == ct)
		{
			return;
		}
		this.RemoveFromCompleteTrain();
		this.completeTrain = ct;
		if (this.networkUpdateOnCompleteTrainChange)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060012F5 RID: 4853 RVA: 0x0009781E File Offset: 0x00095A1E
	public override void Hurt(HitInfo info)
	{
		if (this.RecentlySpawned)
		{
			return;
		}
		base.Hurt(info);
	}

	// Token: 0x060012F6 RID: 4854 RVA: 0x00097830 File Offset: 0x00095A30
	public override void OnKilled(HitInfo info)
	{
		float num = (info == null) ? 0f : info.damageTypes.Get(DamageType.AntiVehicle);
		float num2 = (info == null) ? 0f : info.damageTypes.Get(DamageType.Explosion);
		float num3 = (info == null) ? 0f : info.damageTypes.Total();
		if ((num + num2) / num3 > 0.5f || vehicle.cinematictrains || this.corpseSeconds == 0f)
		{
			if (base.HasDriver())
			{
				base.GetDriver().Hurt(float.MaxValue);
			}
			base.OnKilled(info);
		}
		else
		{
			base.Invoke(new Action(this.ActualDeath), this.corpseSeconds);
		}
		if (base.IsDestroyed && this.fxDestroyed.isValid)
		{
			Effect.server.Run(this.fxDestroyed.resourcePath, this.GetExplosionPos(), Vector3.up, null, true);
		}
	}

	// Token: 0x060012F7 RID: 4855 RVA: 0x0009790C File Offset: 0x00095B0C
	protected virtual Vector3 GetExplosionPos()
	{
		return this.GetCentreOfTrainPos();
	}

	// Token: 0x060012F8 RID: 4856 RVA: 0x00026D90 File Offset: 0x00024F90
	public void ActualDeath()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x060012F9 RID: 4857 RVA: 0x00097914 File Offset: 0x00095B14
	public override void DoRepair(global::BasePlayer player)
	{
		base.DoRepair(player);
		if (this.IsDead() && this.Health() > 0f)
		{
			base.CancelInvoke(new Action(this.ActualDeath));
			this.lifestate = BaseCombatEntity.LifeState.Alive;
		}
	}

	// Token: 0x060012FA RID: 4858 RVA: 0x0009794B File Offset: 0x00095B4B
	public float GetPlayerDamageMultiplier()
	{
		return Mathf.Abs(this.GetTrackSpeed()) * 1f;
	}

	// Token: 0x060012FB RID: 4859 RVA: 0x000059DD File Offset: 0x00003BDD
	public void OnHurtTriggerOccupant(global::BaseEntity hurtEntity, DamageType damageType, float damageTotal)
	{
	}

	// Token: 0x060012FC RID: 4860 RVA: 0x0009795E File Offset: 0x00095B5E
	internal override void DoServerDestroy()
	{
		if (this.FrontTrackSection != null)
		{
			this.FrontTrackSection.DeregisterTrackUser(this);
		}
		this.coupling.Uncouple(true);
		this.coupling.Uncouple(false);
		this.RemoveFromCompleteTrain();
		base.DoServerDestroy();
	}

	// Token: 0x060012FD RID: 4861 RVA: 0x0009799E File Offset: 0x00095B9E
	private void RemoveFromCompleteTrain()
	{
		if (this.completeTrain == null)
		{
			return;
		}
		if (this.completeTrain.ContainsOnly(this))
		{
			this.completeTrain.Dispose();
			this.completeTrain = null;
			return;
		}
		this.completeTrain.RemoveTrainCar(this);
	}

	// Token: 0x060012FE RID: 4862 RVA: 0x000979D6 File Offset: 0x00095BD6
	public override bool MountEligable(global::BasePlayer player)
	{
		return !this.IsDead() && base.MountEligable(player);
	}

	// Token: 0x060012FF RID: 4863 RVA: 0x000979E9 File Offset: 0x00095BE9
	public override float MaxVelocity()
	{
		return TrainCar.TRAINCAR_MAX_SPEED;
	}

	// Token: 0x06001300 RID: 4864 RVA: 0x000979F0 File Offset: 0x00095BF0
	public float GetTrackSpeed()
	{
		if (this.completeTrain == null)
		{
			return 0f;
		}
		return this.completeTrain.GetTrackSpeedFor(this);
	}

	// Token: 0x06001301 RID: 4865 RVA: 0x00097A0C File Offset: 0x00095C0C
	public bool IsCoupledBackwards()
	{
		return this.completeTrain != null && this.completeTrain.IsCoupledBackwards(this);
	}

	// Token: 0x06001302 RID: 4866 RVA: 0x00097A24 File Offset: 0x00095C24
	public float GetPrevTrackSpeed()
	{
		if (this.completeTrain == null)
		{
			return 0f;
		}
		return this.completeTrain.GetPrevTrackSpeedFor(this);
	}

	// Token: 0x06001303 RID: 4867 RVA: 0x00097A40 File Offset: 0x00095C40
	public override Vector3 GetLocalVelocityServer()
	{
		return base.transform.forward * this.GetTrackSpeed();
	}

	// Token: 0x06001304 RID: 4868 RVA: 0x00097A58 File Offset: 0x00095C58
	public bool AnyPlayersOnTrainCar()
	{
		if (this.AnyMounted())
		{
			return true;
		}
		if (this.platformParentTrigger != null && this.platformParentTrigger.HasAnyEntityContents)
		{
			using (HashSet<global::BaseEntity>.Enumerator enumerator = this.platformParentTrigger.entityContents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ToPlayer() != null)
					{
						return true;
					}
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x06001305 RID: 4869 RVA: 0x00097AE4 File Offset: 0x00095CE4
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (this.completeTrain == null)
		{
			return;
		}
		this.completeTrain.UpdateTick(UnityEngine.Time.fixedDeltaTime);
		float trackSpeed = this.GetTrackSpeed();
		this.hurtTriggerFront.gameObject.SetActive(!this.coupling.IsFrontCoupled && trackSpeed > this.hurtTriggerMinSpeed);
		this.hurtTriggerRear.gameObject.SetActive(!this.coupling.IsRearCoupled && trackSpeed < -this.hurtTriggerMinSpeed);
		GameObject[] array = this.hurtOrRepelTriggersInternal;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(Mathf.Abs(trackSpeed) > this.hurtTriggerMinSpeed);
		}
	}

	// Token: 0x06001306 RID: 4870 RVA: 0x00097B95 File Offset: 0x00095D95
	public override void PostVehicleFixedUpdate()
	{
		base.PostVehicleFixedUpdate();
		if (this.completeTrain == null)
		{
			return;
		}
		this.completeTrain.ResetUpdateTick();
	}

	// Token: 0x06001307 RID: 4871 RVA: 0x00097BB1 File Offset: 0x00095DB1
	public Vector3 GetCentreOfTrainPos()
	{
		return base.transform.position + base.transform.rotation * this.bounds.center;
	}

	// Token: 0x06001308 RID: 4872 RVA: 0x00097BE0 File Offset: 0x00095DE0
	public Vector3 GetFrontOfTrainPos()
	{
		return base.transform.position + base.transform.rotation * (this.bounds.center + Vector3.forward * this.bounds.extents.z);
	}

	// Token: 0x06001309 RID: 4873 RVA: 0x00097C38 File Offset: 0x00095E38
	public Vector3 GetRearOfTrainPos()
	{
		return base.transform.position + base.transform.rotation * (this.bounds.center - Vector3.forward * this.bounds.extents.z);
	}

	// Token: 0x0600130A RID: 4874 RVA: 0x00097C90 File Offset: 0x00095E90
	public void FrontTrainCarTick(TrainTrackSpline.TrackSelection trackSelection, float dt)
	{
		float distToMove = this.GetTrackSpeed() * dt;
		TrainTrackSpline preferredAltTrack = (this.RearTrackSection != this.FrontTrackSection) ? this.RearTrackSection : null;
		this.MoveFrontWheelsAlongTrackSpline(this.FrontTrackSection, this.FrontWheelSplineDist, distToMove, preferredAltTrack, trackSelection);
	}

	// Token: 0x0600130B RID: 4875 RVA: 0x00097CD8 File Offset: 0x00095ED8
	public void OtherTrainCarTick(TrainTrackSpline theirTrackSpline, float prevSplineDist, float distanceOffset)
	{
		this.MoveFrontWheelsAlongTrackSpline(theirTrackSpline, prevSplineDist, distanceOffset, this.FrontTrackSection, TrainTrackSpline.TrackSelection.Default);
	}

	// Token: 0x0600130C RID: 4876 RVA: 0x00097CEA File Offset: 0x00095EEA
	public bool TryGetNextTrainCar(Vector3 forwardDir, out TrainCar result)
	{
		return this.TryGetTrainCar(true, forwardDir, out result);
	}

	// Token: 0x0600130D RID: 4877 RVA: 0x00097CF5 File Offset: 0x00095EF5
	public bool TryGetPrevTrainCar(Vector3 forwardDir, out TrainCar result)
	{
		return this.TryGetTrainCar(false, forwardDir, out result);
	}

	// Token: 0x0600130E RID: 4878 RVA: 0x00097D00 File Offset: 0x00095F00
	public bool TryGetTrainCar(bool next, Vector3 forwardDir, out TrainCar result)
	{
		result = null;
		return this.completeTrain != null && this.completeTrain.TryGetAdjacentTrainCar(this, next, forwardDir, out result);
	}

	// Token: 0x0600130F RID: 4879 RVA: 0x00097D20 File Offset: 0x00095F20
	private void MoveFrontWheelsAlongTrackSpline(TrainTrackSpline trackSpline, float prevSplineDist, float distToMove, TrainTrackSpline preferredAltTrack, TrainTrackSpline.TrackSelection trackSelection)
	{
		TrainTrackSpline trainTrackSpline;
		this.FrontWheelSplineDist = trackSpline.GetSplineDistAfterMove(prevSplineDist, base.transform.forward, distToMove, trackSelection, out trainTrackSpline, out this.frontAtEndOfLine, preferredAltTrack, null);
		Vector3 targetFrontWheelTangent;
		Vector3 positionAndTangent = trainTrackSpline.GetPositionAndTangent(this.FrontWheelSplineDist, base.transform.forward, out targetFrontWheelTangent);
		this.SetTheRestFromFrontWheelData(ref trainTrackSpline, positionAndTangent, targetFrontWheelTangent, trackSelection, trackSpline, false);
		this.FrontTrackSection = trainTrackSpline;
	}

	// Token: 0x06001310 RID: 4880 RVA: 0x00097D82 File Offset: 0x00095F82
	private Vector3 GetFrontWheelPos()
	{
		return base.transform.position + base.transform.rotation * this.frontBogieLocalOffset;
	}

	// Token: 0x06001311 RID: 4881 RVA: 0x00097DAA File Offset: 0x00095FAA
	private Vector3 GetRearWheelPos()
	{
		return base.transform.position + base.transform.rotation * this.rearBogieLocalOffset;
	}

	// Token: 0x06001312 RID: 4882 RVA: 0x00097DD4 File Offset: 0x00095FD4
	private void SetTheRestFromFrontWheelData(ref TrainTrackSpline frontTS, Vector3 targetFrontWheelPos, Vector3 targetFrontWheelTangent, TrainTrackSpline.TrackSelection trackSelection, TrainTrackSpline additionalAlt, bool instantMove)
	{
		TrainTrackSpline trainTrackSpline;
		float splineDistAfterMove = frontTS.GetSplineDistAfterMove(this.FrontWheelSplineDist, base.transform.forward, -this.distFrontToBackWheel, trackSelection, out trainTrackSpline, out this.rearAtEndOfLine, this.RearTrackSection, additionalAlt);
		Vector3 to;
		Vector3 positionAndTangent = trainTrackSpline.GetPositionAndTangent(splineDistAfterMove, base.transform.forward, out to);
		if (this.rearAtEndOfLine)
		{
			this.FrontWheelSplineDist = trainTrackSpline.GetSplineDistAfterMove(splineDistAfterMove, base.transform.forward, this.distFrontToBackWheel, trackSelection, out frontTS, out this.frontAtEndOfLine, trainTrackSpline, additionalAlt);
			targetFrontWheelPos = frontTS.GetPositionAndTangent(this.FrontWheelSplineDist, base.transform.forward, out targetFrontWheelTangent);
		}
		this.RearTrackSection = trainTrackSpline;
		Vector3 normalized = (targetFrontWheelPos - positionAndTangent).normalized;
		Vector3 vector = targetFrontWheelPos - Quaternion.LookRotation(normalized) * this.frontBogieLocalOffset;
		if (instantMove)
		{
			base.transform.position = vector;
			if (normalized.magnitude == 0f)
			{
				base.transform.rotation = Quaternion.identity;
			}
			else
			{
				base.transform.rotation = Quaternion.LookRotation(normalized);
			}
		}
		else
		{
			this.rigidBody.MovePosition(vector);
			if (normalized.magnitude == 0f)
			{
				this.rigidBody.MoveRotation(Quaternion.identity);
			}
			else
			{
				this.rigidBody.MoveRotation(Quaternion.LookRotation(normalized));
			}
		}
		this.frontBogieYRot = Vector3.SignedAngle(base.transform.forward, targetFrontWheelTangent, base.transform.up);
		this.rearBogieYRot = Vector3.SignedAngle(base.transform.forward, to, base.transform.up);
		if (UnityEngine.Application.isEditor)
		{
			Debug.DrawLine(targetFrontWheelPos, positionAndTangent, Color.magenta, 0.2f);
			Debug.DrawLine(this.rigidBody.position, vector, Color.yellow, 0.2f);
			Debug.DrawRay(vector, Vector3.up, Color.yellow, 0.2f);
		}
	}

	// Token: 0x06001313 RID: 4883 RVA: 0x00097FBC File Offset: 0x000961BC
	public float GetForces()
	{
		float num = 0f;
		float num2 = base.transform.localEulerAngles.x;
		if (num2 > 180f)
		{
			num2 -= 360f;
		}
		return num + num2 / 90f * -UnityEngine.Physics.gravity.y * this.RealisticMass + this.GetThrottleForce();
	}

	// Token: 0x06001314 RID: 4884 RVA: 0x00026FFC File Offset: 0x000251FC
	protected virtual float GetThrottleForce()
	{
		return 0f;
	}

	// Token: 0x06001315 RID: 4885 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool HasThrottleInput()
	{
		return false;
	}

	// Token: 0x06001316 RID: 4886 RVA: 0x00098014 File Offset: 0x00096214
	public float ApplyCollisionDamage(float forceMagnitude)
	{
		float num;
		if (forceMagnitude > this.derailCollisionForce)
		{
			num = float.MaxValue;
		}
		else
		{
			num = Mathf.Pow(forceMagnitude, 1.3f) / this.collisionDamageDivide;
		}
		base.Hurt(num, DamageType.Collision, this, false);
		return num;
	}

	// Token: 0x06001317 RID: 4887 RVA: 0x00098054 File Offset: 0x00096254
	public bool SpaceIsClear()
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapOBB(this.WorldSpaceBounds(), list, 32768, QueryTriggerInteraction.Ignore);
		foreach (Collider collider in list)
		{
			if (!this.ColliderIsPartOfTrain(collider))
			{
				return false;
			}
		}
		Facepunch.Pool.FreeList<Collider>(ref list);
		return true;
	}

	// Token: 0x06001318 RID: 4888 RVA: 0x000980CC File Offset: 0x000962CC
	public bool ColliderIsPartOfTrain(Collider collider)
	{
		global::BaseEntity baseEntity = collider.ToBaseEntity();
		if (baseEntity == null)
		{
			return false;
		}
		if (baseEntity == this)
		{
			return true;
		}
		global::BaseEntity baseEntity2 = baseEntity.parentEntity.Get(base.isServer);
		return baseEntity2.IsValid() && baseEntity2 == this;
	}

	// Token: 0x06001319 RID: 4889 RVA: 0x00098119 File Offset: 0x00096319
	private void UpdateClients()
	{
		if (base.IsMoving())
		{
			base.ClientRPC<float, float, float>(null, "BaseTrainUpdate", this.GetNetworkTime(), this.frontBogieYRot, this.rearBogieYRot);
		}
	}

	// Token: 0x0600131A RID: 4890 RVA: 0x00098144 File Offset: 0x00096344
	private void DecayTick()
	{
		if (this.completeTrain == null)
		{
			return;
		}
		bool flag = base.HasDriver() || this.completeTrain.AnyPlayersOnTrain();
		if (flag)
		{
			this.decayingFor = 0f;
		}
		float num = this.GetDecayMinutes(flag) * 60f;
		float time = UnityEngine.Time.time;
		float num2 = time - this.lastDecayTick;
		this.lastDecayTick = time;
		if (num != float.PositiveInfinity)
		{
			this.decayingFor += num2;
			if (this.decayingFor >= num && this.CanDieFromDecayNow())
			{
				this.ActualDeath();
			}
		}
	}

	// Token: 0x0600131B RID: 4891 RVA: 0x000981D8 File Offset: 0x000963D8
	protected virtual float GetDecayMinutes(bool hasPassengers)
	{
		bool flag = this.IsAtAStation && Vector3.Distance(this.spawnOrigin, base.transform.position) < 50f;
		if (hasPassengers || this.AnyPlayersNearby(30f) || flag || this.IsOnAboveGroundSpawnRail)
		{
			return float.PositiveInfinity;
		}
		return TrainCar.decayminutes * this.decayTimeMultiplier;
	}

	// Token: 0x0600131C RID: 4892 RVA: 0x0009823D File Offset: 0x0009643D
	protected virtual bool CanDieFromDecayNow()
	{
		return this.CarType == TrainCar.TrainCarType.Engine || !this.completeTrain.IncludesAnEngine();
	}

	// Token: 0x0600131D RID: 4893 RVA: 0x00098258 File Offset: 0x00096458
	private bool AnyPlayersNearby(float maxDist)
	{
		List<global::BasePlayer> list = Facepunch.Pool.GetList<global::BasePlayer>();
		global::Vis.Entities<global::BasePlayer>(base.transform.position, maxDist, list, 131072, QueryTriggerInteraction.Collide);
		bool result = false;
		foreach (global::BasePlayer basePlayer in list)
		{
			if (!basePlayer.IsSleeping() && basePlayer.IsAlive())
			{
				result = true;
				break;
			}
		}
		Facepunch.Pool.FreeList<global::BasePlayer>(ref list);
		return result;
	}

	// Token: 0x0600131E RID: 4894 RVA: 0x000982DC File Offset: 0x000964DC
	[global::BaseEntity.RPC_Server]
	public void RPC_WantsUncouple(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (Vector3.SqrMagnitude(base.transform.position - player.transform.position) > 200f)
		{
			return;
		}
		bool front = msg.read.Bit();
		this.coupling.Uncouple(front);
	}

	// Token: 0x17000198 RID: 408
	// (get) Token: 0x0600131F RID: 4895 RVA: 0x0009833A File Offset: 0x0009653A
	public TriggerTrainCollisions FrontCollisionTrigger
	{
		get
		{
			return this.frontCollisionTrigger;
		}
	}

	// Token: 0x17000199 RID: 409
	// (get) Token: 0x06001320 RID: 4896 RVA: 0x00098342 File Offset: 0x00096542
	public TriggerTrainCollisions RearCollisionTrigger
	{
		get
		{
			return this.rearCollisionTrigger;
		}
	}

	// Token: 0x1700019A RID: 410
	// (get) Token: 0x06001321 RID: 4897 RVA: 0x00007074 File Offset: 0x00005274
	public virtual TrainCar.TrainCarType CarType
	{
		get
		{
			return TrainCar.TrainCarType.Wagon;
		}
	}

	// Token: 0x1700019B RID: 411
	// (get) Token: 0x06001322 RID: 4898 RVA: 0x00020F08 File Offset: 0x0001F108
	public bool LinedUpToUnload
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved4);
		}
	}

	// Token: 0x06001323 RID: 4899 RVA: 0x0009834C File Offset: 0x0009654C
	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(process, rootObj, name, serverside, clientside, bundling);
		this.frontBogieLocalOffset = base.transform.InverseTransformPoint(this.frontBogiePivot.position);
		float num;
		if (this.frontCoupling != null)
		{
			num = base.transform.InverseTransformPoint(this.frontCoupling.position).z;
		}
		else
		{
			num = this.bounds.extents.z + this.bounds.center.z;
		}
		float num2;
		if (this.rearCoupling != null)
		{
			num2 = base.transform.InverseTransformPoint(this.rearCoupling.position).z;
		}
		else
		{
			num2 = -this.bounds.extents.z + this.bounds.center.z;
		}
		this.DistFrontWheelToFrontCoupling = num - this.frontBogieLocalOffset.z;
		this.DistFrontWheelToBackCoupling = -num2 + this.frontBogieLocalOffset.z;
		this.rearBogieLocalOffset = base.transform.InverseTransformPoint(this.rearBogiePivot.position);
	}

	// Token: 0x06001324 RID: 4900 RVA: 0x00098464 File Offset: 0x00096664
	public override void InitShared()
	{
		base.InitShared();
		this.coupling = new TrainCouplingController(this);
	}

	// Token: 0x06001325 RID: 4901 RVA: 0x00098478 File Offset: 0x00096678
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseTrain != null && base.isServer)
		{
			this.frontBogieYRot = info.msg.baseTrain.frontBogieYRot;
			this.rearBogieYRot = info.msg.baseTrain.rearBogieYRot;
		}
	}

	// Token: 0x06001326 RID: 4902 RVA: 0x000984CD File Offset: 0x000966CD
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (old == next)
		{
			return;
		}
		if (base.isServer)
		{
			this.ServerFlagsChanged(old, next);
		}
	}

	// Token: 0x06001327 RID: 4903 RVA: 0x00007074 File Offset: 0x00005274
	public bool CustomCollision(TrainCar train, TriggerTrainCollisions trainTrigger)
	{
		return false;
	}

	// Token: 0x06001328 RID: 4904 RVA: 0x00026238 File Offset: 0x00024438
	public override float InheritedVelocityScale()
	{
		return 0.5f;
	}

	// Token: 0x06001329 RID: 4905 RVA: 0x000984EC File Offset: 0x000966EC
	protected virtual void SetTrackSelection(TrainTrackSpline.TrackSelection trackSelection)
	{
		if (this.localTrackSelection == trackSelection)
		{
			return;
		}
		this.localTrackSelection = trackSelection;
		if (base.isServer)
		{
			base.ClientRPC<sbyte>(null, "SetTrackSelection", (sbyte)this.localTrackSelection);
		}
	}

	// Token: 0x0600132A RID: 4906 RVA: 0x0009851A File Offset: 0x0009671A
	protected bool PlayerIsOnPlatform(global::BasePlayer player)
	{
		return player.GetParentEntity() == this;
	}

	// Token: 0x04000BC0 RID: 3008
	protected bool trainDebug;

	// Token: 0x04000BC1 RID: 3009
	public CompleteTrain completeTrain;

	// Token: 0x04000BC2 RID: 3010
	private bool frontAtEndOfLine;

	// Token: 0x04000BC3 RID: 3011
	private bool rearAtEndOfLine;

	// Token: 0x04000BC4 RID: 3012
	private float frontBogieYRot;

	// Token: 0x04000BC5 RID: 3013
	private float rearBogieYRot;

	// Token: 0x04000BC6 RID: 3014
	private Vector3 spawnOrigin;

	// Token: 0x04000BC7 RID: 3015
	public static float TRAINCAR_MAX_SPEED = 25f;

	// Token: 0x04000BC8 RID: 3016
	private TrainTrackSpline _frontTrackSection;

	// Token: 0x04000BCA RID: 3018
	private float distFrontToBackWheel;

	// Token: 0x04000BCB RID: 3019
	private float initialSpawnTime;

	// Token: 0x04000BCC RID: 3020
	protected float decayingFor;

	// Token: 0x04000BCD RID: 3021
	private float decayTickSpacing = 60f;

	// Token: 0x04000BCE RID: 3022
	private float lastDecayTick;

	// Token: 0x04000BCF RID: 3023
	[Header("Train Car")]
	[SerializeField]
	private float corpseSeconds = 60f;

	// Token: 0x04000BD0 RID: 3024
	[SerializeField]
	private TriggerTrainCollisions frontCollisionTrigger;

	// Token: 0x04000BD1 RID: 3025
	[SerializeField]
	private TriggerTrainCollisions rearCollisionTrigger;

	// Token: 0x04000BD2 RID: 3026
	[SerializeField]
	private float collisionDamageDivide = 100000f;

	// Token: 0x04000BD3 RID: 3027
	[SerializeField]
	private float derailCollisionForce = 130000f;

	// Token: 0x04000BD4 RID: 3028
	[SerializeField]
	private TriggerHurtNotChild hurtTriggerFront;

	// Token: 0x04000BD5 RID: 3029
	[SerializeField]
	private TriggerHurtNotChild hurtTriggerRear;

	// Token: 0x04000BD6 RID: 3030
	[SerializeField]
	private GameObject[] hurtOrRepelTriggersInternal;

	// Token: 0x04000BD7 RID: 3031
	[SerializeField]
	private float hurtTriggerMinSpeed = 1f;

	// Token: 0x04000BD8 RID: 3032
	[SerializeField]
	private Transform centreOfMassTransform;

	// Token: 0x04000BD9 RID: 3033
	[SerializeField]
	private Transform frontBogiePivot;

	// Token: 0x04000BDA RID: 3034
	[SerializeField]
	private bool frontBogieCanRotate = true;

	// Token: 0x04000BDB RID: 3035
	[SerializeField]
	private Transform rearBogiePivot;

	// Token: 0x04000BDC RID: 3036
	[SerializeField]
	private bool rearBogieCanRotate = true;

	// Token: 0x04000BDD RID: 3037
	[SerializeField]
	private Transform[] wheelVisuals;

	// Token: 0x04000BDE RID: 3038
	[SerializeField]
	private float wheelRadius = 0.615f;

	// Token: 0x04000BDF RID: 3039
	[FormerlySerializedAs("fxFinalExplosion")]
	[SerializeField]
	private GameObjectRef fxDestroyed;

	// Token: 0x04000BE0 RID: 3040
	[SerializeField]
	protected TriggerParent platformParentTrigger;

	// Token: 0x04000BE1 RID: 3041
	public GameObjectRef collisionEffect;

	// Token: 0x04000BE2 RID: 3042
	public Transform frontCoupling;

	// Token: 0x04000BE3 RID: 3043
	public Transform frontCouplingPivot;

	// Token: 0x04000BE4 RID: 3044
	public Transform rearCoupling;

	// Token: 0x04000BE5 RID: 3045
	public Transform rearCouplingPivot;

	// Token: 0x04000BE6 RID: 3046
	[SerializeField]
	private SoundDefinition coupleSound;

	// Token: 0x04000BE7 RID: 3047
	[SerializeField]
	private SoundDefinition uncoupleSound;

	// Token: 0x04000BE8 RID: 3048
	[SerializeField]
	private TrainCarAudio trainCarAudio;

	// Token: 0x04000BE9 RID: 3049
	[FormerlySerializedAs("frontCoupleFx")]
	[SerializeField]
	private ParticleSystem frontCouplingChangedFx;

	// Token: 0x04000BEA RID: 3050
	[FormerlySerializedAs("rearCoupleFx")]
	[SerializeField]
	private ParticleSystem rearCouplingChangedFx;

	// Token: 0x04000BEB RID: 3051
	[FormerlySerializedAs("fxCoupling")]
	[SerializeField]
	private ParticleSystem newCouplingFX;

	// Token: 0x04000BEC RID: 3052
	[SerializeField]
	private float decayTimeMultiplier = 1f;

	// Token: 0x04000BED RID: 3053
	[SerializeField]
	[ReadOnly]
	private Vector3 frontBogieLocalOffset;

	// Token: 0x04000BEE RID: 3054
	[SerializeField]
	[ReadOnly]
	private Vector3 rearBogieLocalOffset;

	// Token: 0x04000BEF RID: 3055
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 2.3f;

	// Token: 0x04000BF0 RID: 3056
	[ServerVar(Help = "Ratio of wagons to train engines that spawn")]
	public static int wagons_per_engine = 2;

	// Token: 0x04000BF1 RID: 3057
	[ServerVar(Help = "How long before a train car despawns")]
	public static float decayminutes = 30f;

	// Token: 0x04000BF2 RID: 3058
	[ReadOnly]
	public float DistFrontWheelToFrontCoupling;

	// Token: 0x04000BF3 RID: 3059
	[ReadOnly]
	public float DistFrontWheelToBackCoupling;

	// Token: 0x04000BF4 RID: 3060
	public TrainCouplingController coupling;

	// Token: 0x04000BF5 RID: 3061
	[NonSerialized]
	public TrainTrackSpline.TrackSelection localTrackSelection;

	// Token: 0x04000BF6 RID: 3062
	public const global::BaseEntity.Flags Flag_LinedUpToUnload = global::BaseEntity.Flags.Reserved4;

	// Token: 0x02000BC0 RID: 3008
	public enum TrainCarType
	{
		// Token: 0x04003F6D RID: 16237
		Wagon,
		// Token: 0x04003F6E RID: 16238
		Engine,
		// Token: 0x04003F6F RID: 16239
		Other
	}
}
