using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000031 RID: 49
public class BaseAIBrain : EntityComponent<global::BaseEntity>, IPet, IAISleepable, IAIDesign, IAIGroupable, IAIEventListener
{
	// Token: 0x060001A8 RID: 424 RVA: 0x00022D54 File Offset: 0x00020F54
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseAIBrain.OnRpcMessage", 0))
		{
			if (rpc == 66191493U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RequestAIDesign ");
				}
				using (TimeWarning.New("RequestAIDesign", 0))
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
							this.RequestAIDesign(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RequestAIDesign");
					}
				}
				return true;
			}
			if (rpc == 2122228512U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StopAIDesign ");
				}
				using (TimeWarning.New("StopAIDesign", 0))
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
							this.StopAIDesign(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in StopAIDesign");
					}
				}
				return true;
			}
			if (rpc == 657290375U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SubmitAIDesign ");
				}
				using (TimeWarning.New("SubmitAIDesign", 0))
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
							this.SubmitAIDesign(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in SubmitAIDesign");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x000230C0 File Offset: 0x000212C0
	public bool IsPet()
	{
		return this.Pet;
	}

	// Token: 0x060001AA RID: 426 RVA: 0x000230C8 File Offset: 0x000212C8
	public void SetPetOwner(global::BasePlayer player)
	{
		global::BaseEntity baseEntity = this.GetBaseEntity();
		player.PetEntity = baseEntity;
		baseEntity.OwnerID = player.userID;
		BasePet.ActivePetByOwnerID[player.userID] = (baseEntity as BasePet);
	}

	// Token: 0x060001AB RID: 427 RVA: 0x00023105 File Offset: 0x00021305
	public bool IsOwnedBy(global::BasePlayer player)
	{
		return !(this.OwningPlayer == null) && !(player == null) && this != null && this.OwningPlayer == player;
	}

	// Token: 0x060001AC RID: 428 RVA: 0x00023134 File Offset: 0x00021334
	public bool IssuePetCommand(PetCommandType cmd, int param, Ray? ray)
	{
		if (ray != null)
		{
			int layerMask = 10551296;
			RaycastHit raycastHit;
			if (UnityEngine.Physics.Raycast(ray.Value, out raycastHit, 75f, layerMask))
			{
				this.Events.Memory.Position.Set(raycastHit.point, 6);
			}
			else
			{
				this.Events.Memory.Position.Set(base.transform.position, 6);
			}
		}
		switch (cmd)
		{
		case PetCommandType.LoadDesign:
			if (param < 0 || param >= this.Designs.Count)
			{
				return false;
			}
			this.LoadAIDesign(AIDesigns.GetByNameOrInstance(this.Designs[param].Filename, this.InstanceSpecificDesign), null, param);
			return true;
		case PetCommandType.SetState:
		{
			global::AIStateContainer stateContainerByID = this.AIDesign.GetStateContainerByID(param);
			return stateContainerByID != null && this.SwitchToState(stateContainerByID.State, param);
		}
		case PetCommandType.Destroy:
			this.GetBaseEntity().Kill(global::BaseNetworkable.DestroyMode.None);
			return true;
		default:
			return false;
		}
	}

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x060001AD RID: 429 RVA: 0x00023224 File Offset: 0x00021424
	// (set) Token: 0x060001AE RID: 430 RVA: 0x0002322C File Offset: 0x0002142C
	public BaseAIBrain.BasicAIState CurrentState { get; private set; }

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x060001AF RID: 431 RVA: 0x00023235 File Offset: 0x00021435
	// (set) Token: 0x060001B0 RID: 432 RVA: 0x0002323D File Offset: 0x0002143D
	public AIThinkMode ThinkMode { get; protected set; } = AIThinkMode.Interval;

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x060001B1 RID: 433 RVA: 0x00023246 File Offset: 0x00021446
	// (set) Token: 0x060001B2 RID: 434 RVA: 0x0002324E File Offset: 0x0002144E
	public float Age { get; private set; }

	// Token: 0x060001B3 RID: 435 RVA: 0x00023257 File Offset: 0x00021457
	public void ForceSetAge(float age)
	{
		this.Age = age;
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x060001B4 RID: 436 RVA: 0x00023260 File Offset: 0x00021460
	// (set) Token: 0x060001B5 RID: 437 RVA: 0x00023268 File Offset: 0x00021468
	public AIBrainSenses Senses { get; private set; } = new AIBrainSenses();

	// Token: 0x17000026 RID: 38
	// (get) Token: 0x060001B6 RID: 438 RVA: 0x00023271 File Offset: 0x00021471
	// (set) Token: 0x060001B7 RID: 439 RVA: 0x00023279 File Offset: 0x00021479
	public BasePathFinder PathFinder { get; protected set; }

	// Token: 0x17000027 RID: 39
	// (get) Token: 0x060001B8 RID: 440 RVA: 0x00023282 File Offset: 0x00021482
	// (set) Token: 0x060001B9 RID: 441 RVA: 0x0002328A File Offset: 0x0002148A
	public AIEvents Events { get; private set; }

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x060001BA RID: 442 RVA: 0x00023293 File Offset: 0x00021493
	// (set) Token: 0x060001BB RID: 443 RVA: 0x0002329B File Offset: 0x0002149B
	public global::AIDesign AIDesign { get; private set; }

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x060001BC RID: 444 RVA: 0x000232A4 File Offset: 0x000214A4
	// (set) Token: 0x060001BD RID: 445 RVA: 0x000232AC File Offset: 0x000214AC
	public global::BasePlayer DesigningPlayer { get; private set; }

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x060001BE RID: 446 RVA: 0x000232B5 File Offset: 0x000214B5
	// (set) Token: 0x060001BF RID: 447 RVA: 0x000232BD File Offset: 0x000214BD
	public global::BasePlayer OwningPlayer { get; private set; }

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x060001C0 RID: 448 RVA: 0x000232C6 File Offset: 0x000214C6
	// (set) Token: 0x060001C1 RID: 449 RVA: 0x000232CE File Offset: 0x000214CE
	public bool IsGroupLeader { get; private set; }

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x060001C2 RID: 450 RVA: 0x000232D7 File Offset: 0x000214D7
	// (set) Token: 0x060001C3 RID: 451 RVA: 0x000232DF File Offset: 0x000214DF
	public bool IsGrouped { get; private set; }

	// Token: 0x1700002D RID: 45
	// (get) Token: 0x060001C4 RID: 452 RVA: 0x000232E8 File Offset: 0x000214E8
	// (set) Token: 0x060001C5 RID: 453 RVA: 0x000232F0 File Offset: 0x000214F0
	public IAIGroupable GroupLeader { get; private set; }

	// Token: 0x060001C6 RID: 454 RVA: 0x000232F9 File Offset: 0x000214F9
	public int LoadedDesignIndex()
	{
		return this.loadedDesignIndex;
	}

	// Token: 0x1700002E RID: 46
	// (get) Token: 0x060001C7 RID: 455 RVA: 0x00023301 File Offset: 0x00021501
	// (set) Token: 0x060001C8 RID: 456 RVA: 0x00023309 File Offset: 0x00021509
	public BaseNavigator Navigator { get; private set; }

	// Token: 0x060001C9 RID: 457 RVA: 0x00023312 File Offset: 0x00021512
	public void SetEnabled(bool flag)
	{
		this.disabled = !flag;
	}

	// Token: 0x060001CA RID: 458 RVA: 0x0002331E File Offset: 0x0002151E
	bool IAIDesign.CanPlayerDesignAI(global::BasePlayer player)
	{
		return this.PlayerCanDesignAI(player);
	}

	// Token: 0x060001CB RID: 459 RVA: 0x00023327 File Offset: 0x00021527
	private bool PlayerCanDesignAI(global::BasePlayer player)
	{
		return AI.allowdesigning && !(player == null) && this.UseAIDesign && !(this.DesigningPlayer != null) && player.IsDeveloper;
	}

	// Token: 0x060001CC RID: 460 RVA: 0x00023364 File Offset: 0x00021564
	[global::BaseEntity.RPC_Server]
	private void RequestAIDesign(global::BaseEntity.RPCMessage msg)
	{
		if (!this.UseAIDesign)
		{
			return;
		}
		if (msg.player == null)
		{
			return;
		}
		if (this.AIDesign == null)
		{
			return;
		}
		if (!this.PlayerCanDesignAI(msg.player))
		{
			return;
		}
		msg.player.designingAIEntity = this.GetBaseEntity();
		msg.player.ClientRPCPlayer<ProtoBuf.AIDesign>(null, msg.player, "StartDesigningAI", this.AIDesign.ToProto(this.currentStateContainerID));
		this.DesigningPlayer = msg.player;
		this.SetOwningPlayer(msg.player);
	}

	// Token: 0x060001CD RID: 461 RVA: 0x000233F4 File Offset: 0x000215F4
	[global::BaseEntity.RPC_Server]
	private void SubmitAIDesign(global::BaseEntity.RPCMessage msg)
	{
		ProtoBuf.AIDesign aidesign = ProtoBuf.AIDesign.Deserialize(msg.read);
		if (!this.LoadAIDesign(aidesign, msg.player, this.loadedDesignIndex))
		{
			return;
		}
		this.SaveDesign();
		if (aidesign.scope == 2)
		{
			return;
		}
		global::BaseEntity baseEntity = this.GetBaseEntity();
		global::BaseEntity[] array = global::BaseEntity.Util.FindTargets(baseEntity.ShortPrefabName, false);
		if (array == null || array.Length == 0)
		{
			return;
		}
		foreach (global::BaseEntity baseEntity2 in array)
		{
			if (!(baseEntity2 == null) && !(baseEntity2 == baseEntity))
			{
				EntityComponentBase[] components = baseEntity2.Components;
				if (components != null)
				{
					EntityComponentBase[] array3 = components;
					for (int j = 0; j < array3.Length; j++)
					{
						IAIDesign iaidesign;
						if ((iaidesign = (array3[j] as IAIDesign)) != null)
						{
							iaidesign.LoadAIDesign(aidesign, null);
							break;
						}
					}
				}
			}
		}
	}

	// Token: 0x060001CE RID: 462 RVA: 0x000234BD File Offset: 0x000216BD
	void IAIDesign.StopDesigning()
	{
		this.ClearDesigningPlayer();
	}

	// Token: 0x060001CF RID: 463 RVA: 0x000234C5 File Offset: 0x000216C5
	void IAIDesign.LoadAIDesign(ProtoBuf.AIDesign design, global::BasePlayer player)
	{
		this.LoadAIDesign(design, player, this.loadedDesignIndex);
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x000234D6 File Offset: 0x000216D6
	public bool LoadDefaultAIDesign()
	{
		return this.loadedDesignIndex == 0 || this.LoadAIDesignAtIndex(0);
	}

	// Token: 0x060001D1 RID: 465 RVA: 0x000234EC File Offset: 0x000216EC
	public bool LoadAIDesignAtIndex(int index)
	{
		return this.Designs != null && index >= 0 && index < this.Designs.Count && this.LoadAIDesign(AIDesigns.GetByNameOrInstance(this.Designs[index].Filename, this.InstanceSpecificDesign), null, index);
	}

	// Token: 0x060001D2 RID: 466 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnAIDesignLoadedAtIndex(int index)
	{
	}

	// Token: 0x060001D3 RID: 467 RVA: 0x0002353C File Offset: 0x0002173C
	protected bool LoadAIDesign(ProtoBuf.AIDesign design, global::BasePlayer player, int index)
	{
		if (design == null)
		{
			Debug.LogError(this.GetBaseEntity().gameObject.name + " failed to load AI design!");
			return false;
		}
		if (player != null)
		{
			AIDesignScope scope = (AIDesignScope)design.scope;
			if (scope == AIDesignScope.Default && !player.IsDeveloper)
			{
				return false;
			}
			if (scope == AIDesignScope.EntityServerWide && !player.IsDeveloper && !player.IsAdmin)
			{
				return false;
			}
		}
		if (this.AIDesign == null)
		{
			return false;
		}
		this.AIDesign.Load(design, base.baseEntity);
		global::AIStateContainer defaultStateContainer = this.AIDesign.GetDefaultStateContainer();
		if (defaultStateContainer != null)
		{
			this.SwitchToState(defaultStateContainer.State, defaultStateContainer.ID);
		}
		this.loadedDesignIndex = index;
		this.OnAIDesignLoadedAtIndex(this.loadedDesignIndex);
		return true;
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x000235F4 File Offset: 0x000217F4
	public void SaveDesign()
	{
		if (this.AIDesign == null)
		{
			return;
		}
		ProtoBuf.AIDesign aidesign = this.AIDesign.ToProto(this.currentStateContainerID);
		string text = "cfg/ai/";
		string text2 = this.Designs[this.loadedDesignIndex].Filename;
		switch (this.AIDesign.Scope)
		{
		case AIDesignScope.Default:
			text += text2;
			try
			{
				using (FileStream fileStream = File.Create(text))
				{
					ProtoBuf.AIDesign.Serialize(fileStream, aidesign);
				}
				AIDesigns.RefreshCache(text2, aidesign);
				return;
			}
			catch (Exception)
			{
				Debug.LogWarning("Error trying to save default AI Design: " + text);
				return;
			}
			break;
		case AIDesignScope.EntityServerWide:
			break;
		case AIDesignScope.EntityInstance:
			return;
		default:
			return;
		}
		text2 += "_custom";
		text += text2;
		try
		{
			using (FileStream fileStream2 = File.Create(text))
			{
				ProtoBuf.AIDesign.Serialize(fileStream2, aidesign);
			}
			AIDesigns.RefreshCache(text2, aidesign);
		}
		catch (Exception)
		{
			Debug.LogWarning("Error trying to save server-wide AI Design: " + text);
		}
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x0002371C File Offset: 0x0002191C
	[global::BaseEntity.RPC_Server]
	private void StopAIDesign(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == this.DesigningPlayer)
		{
			this.ClearDesigningPlayer();
		}
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x00023737 File Offset: 0x00021937
	private void ClearDesigningPlayer()
	{
		this.DesigningPlayer = null;
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x00023740 File Offset: 0x00021940
	public void SetOwningPlayer(global::BasePlayer owner)
	{
		this.OwningPlayer = owner;
		this.Events.Memory.Entity.Set(this.OwningPlayer, 5);
		if (this != null && ((IPet)this).IsPet())
		{
			((IPet)this).SetPetOwner(owner);
			owner.Pet = this;
		}
	}

	// Token: 0x060001D8 RID: 472 RVA: 0x0002378B File Offset: 0x0002198B
	public virtual bool ShouldServerThink()
	{
		return this.ThinkMode == AIThinkMode.Interval && UnityEngine.Time.time > this.lastThinkTime + this.thinkRate;
	}

	// Token: 0x060001D9 RID: 473 RVA: 0x000237B0 File Offset: 0x000219B0
	public virtual void DoThink()
	{
		float delta = UnityEngine.Time.time - this.lastThinkTime;
		this.Think(delta);
	}

	// Token: 0x060001DA RID: 474 RVA: 0x000237D1 File Offset: 0x000219D1
	public List<AIState> GetStateList()
	{
		return this.states.Keys.ToList<AIState>();
	}

	// Token: 0x060001DB RID: 475 RVA: 0x000237E3 File Offset: 0x000219E3
	public void Start()
	{
		this.AddStates();
		this.InitializeAI();
	}

	// Token: 0x060001DC RID: 476 RVA: 0x000237F1 File Offset: 0x000219F1
	public virtual void AddStates()
	{
		this.states = new Dictionary<AIState, BaseAIBrain.BasicAIState>();
	}

	// Token: 0x060001DD RID: 477 RVA: 0x00023800 File Offset: 0x00021A00
	public virtual void InitializeAI()
	{
		global::BaseEntity baseEntity = this.GetBaseEntity();
		baseEntity.HasBrain = true;
		this.Navigator = base.GetComponent<BaseNavigator>();
		if (this.UseAIDesign)
		{
			this.AIDesign = new global::AIDesign();
			this.AIDesign.SetAvailableStates(this.GetStateList());
			if (this.Events == null)
			{
				this.Events = new AIEvents();
			}
			bool senseFriendlies = this.MaxGroupSize > 0;
			this.Senses.Init(baseEntity, this, this.MemoryDuration, this.SenseRange, this.TargetLostRange, this.VisionCone, this.CheckVisionCone, this.CheckLOS, this.IgnoreNonVisionSneakers, this.ListenRange, this.HostileTargetsOnly, senseFriendlies, this.IgnoreSafeZonePlayers, this.SenseTypes, this.RefreshKnownLOS);
			if (this.DefaultDesignSO == null && this.Designs.Count == 0)
			{
				Debug.LogWarning("Brain on " + base.gameObject.name + " is trying to load a null AI design!");
				return;
			}
			this.Events.Memory.Position.Set(base.transform.position, 4);
			if (this.Designs.Count == 0)
			{
				this.Designs.Add(this.DefaultDesignSO);
			}
			this.loadedDesignIndex = 0;
			this.LoadAIDesign(AIDesigns.GetByNameOrInstance(this.Designs[this.loadedDesignIndex].Filename, this.InstanceSpecificDesign), null, this.loadedDesignIndex);
			AIInformationZone forPoint = AIInformationZone.GetForPoint(base.transform.position, false);
			if (forPoint != null)
			{
				forPoint.RegisterSleepableEntity(this);
			}
		}
		global::BaseEntity.Query.Server.AddBrain(baseEntity);
		this.StartMovementTick();
	}

	// Token: 0x060001DE RID: 478 RVA: 0x000239A4 File Offset: 0x00021BA4
	public global::BaseEntity GetBrainBaseEntity()
	{
		return this.GetBaseEntity();
	}

	// Token: 0x060001DF RID: 479 RVA: 0x000239AC File Offset: 0x00021BAC
	public virtual void OnDestroy()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		global::BaseEntity.Query.Server.RemoveBrain(this.GetBaseEntity());
		AIInformationZone aiinformationZone = null;
		HumanNPC humanNPC = this.GetBaseEntity() as HumanNPC;
		if (humanNPC != null)
		{
			aiinformationZone = humanNPC.VirtualInfoZone;
		}
		if (aiinformationZone == null)
		{
			aiinformationZone = AIInformationZone.GetForPoint(base.transform.position, true);
		}
		if (aiinformationZone != null)
		{
			aiinformationZone.UnregisterSleepableEntity(this);
		}
		this.LeaveGroup();
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x00023A20 File Offset: 0x00021C20
	private void StartMovementTick()
	{
		base.CancelInvoke(new Action(this.TickMovement));
		base.InvokeRandomized(new Action(this.TickMovement), 1f, 0.1f, 0.010000001f);
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x00023A55 File Offset: 0x00021C55
	private void StopMovementTick()
	{
		base.CancelInvoke(new Action(this.TickMovement));
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x00023A6C File Offset: 0x00021C6C
	public void TickMovement()
	{
		if (BasePet.queuedMovementsAllowed && this.UseQueuedMovementUpdates && this.Navigator != null)
		{
			if (BasePet.onlyQueueBaseNavMovements && this.Navigator.CurrentNavigationType != BaseNavigator.NavigationType.Base)
			{
				this.DoMovementTick();
				return;
			}
			BasePet basePet = this.GetBaseEntity() as BasePet;
			if (basePet != null && !basePet.inQueue)
			{
				BasePet._movementProcessQueue.Enqueue(basePet);
				basePet.inQueue = true;
				return;
			}
		}
		else
		{
			this.DoMovementTick();
		}
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x00023AE8 File Offset: 0x00021CE8
	public void DoMovementTick()
	{
		float delta = UnityEngine.Time.realtimeSinceStartup - this.lastMovementTickTime;
		this.lastMovementTickTime = UnityEngine.Time.realtimeSinceStartup;
		if (this.Navigator != null)
		{
			this.Navigator.Think(delta);
		}
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x00023B28 File Offset: 0x00021D28
	public void AddState(BaseAIBrain.BasicAIState newState)
	{
		if (this.states.ContainsKey(newState.StateType))
		{
			Debug.LogWarning("Trying to add duplicate state: " + newState.StateType.ToString() + " to " + this.GetBaseEntity().PrefabName);
			return;
		}
		newState.brain = this;
		newState.Reset();
		this.states.Add(newState.StateType, newState);
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x00023B9B File Offset: 0x00021D9B
	protected bool SwitchToState(AIState newState, int stateContainerID = -1)
	{
		if (this.states.ContainsKey(newState))
		{
			bool flag = this.SwitchToState(this.states[newState], stateContainerID);
			if (flag)
			{
				this.OnStateChanged();
			}
			return flag;
		}
		return false;
	}

	// Token: 0x060001E6 RID: 486 RVA: 0x00023BCC File Offset: 0x00021DCC
	private bool SwitchToState(BaseAIBrain.BasicAIState newState, int stateContainerID = -1)
	{
		if (newState == null || !newState.CanEnter())
		{
			return false;
		}
		if (this.CurrentState != null)
		{
			if (!this.CurrentState.CanLeave())
			{
				return false;
			}
			if (this.CurrentState == newState && !this.UseAIDesign)
			{
				return false;
			}
			this.CurrentState.StateLeave(this, this.GetBaseEntity());
		}
		this.AddEvents(stateContainerID);
		this.CurrentState = newState;
		this.CurrentState.StateEnter(this, this.GetBaseEntity());
		this.currentStateContainerID = stateContainerID;
		return true;
	}

	// Token: 0x060001E7 RID: 487 RVA: 0x00023C4C File Offset: 0x00021E4C
	protected virtual void OnStateChanged()
	{
		if (this.SendClientCurrentState)
		{
			global::BaseEntity baseEntity = this.GetBaseEntity();
			if (baseEntity != null)
			{
				baseEntity.ClientRPC<int>(null, "ClientChangeState", (int)((this.CurrentState != null) ? this.CurrentState.StateType : AIState.None));
			}
		}
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x00023C93 File Offset: 0x00021E93
	private void AddEvents(int stateContainerID)
	{
		if (!this.UseAIDesign)
		{
			return;
		}
		if (this.AIDesign == null)
		{
			return;
		}
		this.Events.Init(this, this.AIDesign.GetStateContainerByID(stateContainerID), base.baseEntity, this.Senses);
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x00023CCC File Offset: 0x00021ECC
	public virtual void Think(float delta)
	{
		if (!AI.think)
		{
			return;
		}
		this.lastThinkTime = UnityEngine.Time.time;
		if (this.sleeping || this.disabled)
		{
			return;
		}
		this.Age += delta;
		if (this.UseAIDesign)
		{
			this.Senses.Update();
			this.UpdateGroup();
		}
		if (this.CurrentState != null)
		{
			this.UpdateAgressionTimer(delta);
			StateStatus stateStatus = this.CurrentState.StateThink(delta, this, this.GetBaseEntity());
			if (this.Events != null)
			{
				this.Events.Tick(delta, stateStatus);
			}
		}
		if (!this.UseAIDesign && (this.CurrentState == null || this.CurrentState.CanLeave()))
		{
			float num = 0f;
			BaseAIBrain.BasicAIState basicAIState = null;
			foreach (BaseAIBrain.BasicAIState basicAIState2 in this.states.Values)
			{
				if (basicAIState2 != null && basicAIState2.CanEnter())
				{
					float weight = basicAIState2.GetWeight();
					if (weight > num)
					{
						num = weight;
						basicAIState = basicAIState2;
					}
				}
			}
			if (basicAIState != this.CurrentState)
			{
				this.SwitchToState(basicAIState, -1);
			}
		}
	}

	// Token: 0x060001EA RID: 490 RVA: 0x00023DFC File Offset: 0x00021FFC
	private void UpdateAgressionTimer(float delta)
	{
		if (this.CurrentState == null)
		{
			this.Senses.TimeInAgressiveState = 0f;
			return;
		}
		if (this.CurrentState.AgrresiveState)
		{
			this.Senses.TimeInAgressiveState += delta;
			return;
		}
		this.Senses.TimeInAgressiveState = 0f;
	}

	// Token: 0x060001EB RID: 491 RVA: 0x00023E53 File Offset: 0x00022053
	bool IAISleepable.AllowedToSleep()
	{
		return this.AllowedToSleep;
	}

	// Token: 0x060001EC RID: 492 RVA: 0x00023E5B File Offset: 0x0002205B
	void IAISleepable.SleepAI()
	{
		if (this.sleeping)
		{
			return;
		}
		this.sleeping = true;
		if (this.Navigator != null)
		{
			this.Navigator.Pause();
		}
		this.StopMovementTick();
	}

	// Token: 0x060001ED RID: 493 RVA: 0x00023E8C File Offset: 0x0002208C
	void IAISleepable.WakeAI()
	{
		if (!this.sleeping)
		{
			return;
		}
		this.sleeping = false;
		if (this.Navigator != null)
		{
			this.Navigator.Resume();
		}
		this.StartMovementTick();
	}

	// Token: 0x060001EE RID: 494 RVA: 0x00023EC0 File Offset: 0x000220C0
	private void UpdateGroup()
	{
		if (!AI.groups)
		{
			return;
		}
		if (this.MaxGroupSize <= 0)
		{
			return;
		}
		if (!this.InGroup() && this.Senses.Memory.Friendlies.Count > 0)
		{
			IAIGroupable iaigroupable = null;
			foreach (global::BaseEntity baseEntity in this.Senses.Memory.Friendlies)
			{
				if (!(baseEntity == null))
				{
					IAIGroupable component = baseEntity.GetComponent<IAIGroupable>();
					if (component != null)
					{
						if (component.InGroup() && component.AddMember(this))
						{
							break;
						}
						if (iaigroupable == null && !component.InGroup())
						{
							iaigroupable = component;
						}
					}
				}
			}
			if (!this.InGroup() && iaigroupable != null)
			{
				this.AddMember(iaigroupable);
			}
		}
	}

	// Token: 0x060001EF RID: 495 RVA: 0x00023F98 File Offset: 0x00022198
	public bool AddMember(IAIGroupable member)
	{
		if (this.InGroup() && !this.IsGroupLeader)
		{
			return this.GroupLeader.AddMember(member);
		}
		if (this.MaxGroupSize <= 0)
		{
			return false;
		}
		if (this.groupMembers.Contains(member))
		{
			return true;
		}
		if (this.groupMembers.Count + 1 >= this.MaxGroupSize)
		{
			return false;
		}
		this.groupMembers.Add(member);
		this.IsGrouped = true;
		this.IsGroupLeader = true;
		this.GroupLeader = this;
		global::BaseEntity baseEntity = this.GetBaseEntity();
		this.Events.Memory.Entity.Set(baseEntity, 6);
		member.JoinGroup(this, baseEntity);
		return true;
	}

	// Token: 0x060001F0 RID: 496 RVA: 0x0002403C File Offset: 0x0002223C
	public void JoinGroup(IAIGroupable leader, global::BaseEntity leaderEntity)
	{
		this.Events.Memory.Entity.Set(leaderEntity, 6);
		this.GroupLeader = leader;
		this.IsGroupLeader = false;
		this.IsGrouped = true;
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x0002406C File Offset: 0x0002226C
	public void SetGroupRoamRootPosition(Vector3 rootPos)
	{
		if (this.IsGroupLeader)
		{
			foreach (IAIGroupable iaigroupable in this.groupMembers)
			{
				iaigroupable.SetGroupRoamRootPosition(rootPos);
			}
		}
		this.Events.Memory.Position.Set(rootPos, 5);
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x000240DC File Offset: 0x000222DC
	public bool InGroup()
	{
		return this.IsGrouped;
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x000240E4 File Offset: 0x000222E4
	public void LeaveGroup()
	{
		if (!this.InGroup())
		{
			return;
		}
		if (!this.IsGroupLeader)
		{
			if (this.GroupLeader != null)
			{
				this.GroupLeader.RemoveMember(base.GetComponent<IAIGroupable>());
			}
			return;
		}
		if (this.groupMembers.Count == 0)
		{
			return;
		}
		IAIGroupable iaigroupable = this.groupMembers[0];
		if (iaigroupable == null)
		{
			return;
		}
		this.RemoveMember(iaigroupable);
		for (int i = this.groupMembers.Count - 1; i >= 0; i--)
		{
			IAIGroupable iaigroupable2 = this.groupMembers[i];
			if (iaigroupable2 != null && iaigroupable2 != iaigroupable)
			{
				this.RemoveMember(iaigroupable2);
				iaigroupable.AddMember(iaigroupable2);
			}
		}
		this.groupMembers.Clear();
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x00024188 File Offset: 0x00022388
	public void RemoveMember(IAIGroupable member)
	{
		if (member == null)
		{
			return;
		}
		if (!this.IsGroupLeader)
		{
			return;
		}
		if (!this.groupMembers.Contains(member))
		{
			return;
		}
		this.groupMembers.Remove(member);
		member.SetUngrouped();
		if (this.groupMembers.Count == 0)
		{
			this.SetUngrouped();
		}
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x000241D7 File Offset: 0x000223D7
	public void SetUngrouped()
	{
		this.IsGrouped = false;
		this.IsGroupLeader = false;
		this.GroupLeader = null;
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x000241EE File Offset: 0x000223EE
	public override void LoadComponent(global::BaseNetworkable.LoadInfo info)
	{
		base.LoadComponent(info);
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x000241F8 File Offset: 0x000223F8
	public override void SaveComponent(global::BaseNetworkable.SaveInfo info)
	{
		base.SaveComponent(info);
		if (this.SendClientCurrentState && this.CurrentState != null)
		{
			info.msg.brainComponent = Facepunch.Pool.Get<BrainComponent>();
			info.msg.brainComponent.currentState = (int)this.CurrentState.StateType;
		}
	}

	// Token: 0x060001F8 RID: 504 RVA: 0x00024247 File Offset: 0x00022447
	private void SendStateChangeEvent(int previousStateID, int newStateID, int sourceEventID)
	{
		if (this.DesigningPlayer != null)
		{
			this.DesigningPlayer.ClientRPCPlayer<int, int, int>(null, this.DesigningPlayer, "OnDebugAIEventTriggeredStateChange", previousStateID, newStateID, sourceEventID);
		}
	}

	// Token: 0x060001F9 RID: 505 RVA: 0x00024274 File Offset: 0x00022474
	public void EventTriggeredStateChange(int newStateContainerID, int sourceEventID)
	{
		if (this.AIDesign == null)
		{
			return;
		}
		if (newStateContainerID == -1)
		{
			return;
		}
		global::AIStateContainer stateContainerByID = this.AIDesign.GetStateContainerByID(newStateContainerID);
		int previousStateID = this.currentStateContainerID;
		this.SwitchToState(stateContainerByID.State, newStateContainerID);
		this.SendStateChangeEvent(previousStateID, this.currentStateContainerID, sourceEventID);
	}

	// Token: 0x040001CE RID: 462
	public bool SendClientCurrentState;

	// Token: 0x040001CF RID: 463
	public bool UseQueuedMovementUpdates;

	// Token: 0x040001D0 RID: 464
	public bool AllowedToSleep = true;

	// Token: 0x040001D1 RID: 465
	public AIDesignSO DefaultDesignSO;

	// Token: 0x040001D2 RID: 466
	public List<AIDesignSO> Designs = new List<AIDesignSO>();

	// Token: 0x040001D3 RID: 467
	public ProtoBuf.AIDesign InstanceSpecificDesign;

	// Token: 0x040001D4 RID: 468
	public float SenseRange = 10f;

	// Token: 0x040001D5 RID: 469
	public float AttackRangeMultiplier = 1f;

	// Token: 0x040001D6 RID: 470
	public float TargetLostRange = 40f;

	// Token: 0x040001D7 RID: 471
	public float VisionCone = -0.8f;

	// Token: 0x040001D8 RID: 472
	public bool CheckVisionCone;

	// Token: 0x040001D9 RID: 473
	public bool CheckLOS;

	// Token: 0x040001DA RID: 474
	public bool IgnoreNonVisionSneakers = true;

	// Token: 0x040001DB RID: 475
	public float IgnoreSneakersMaxDistance = 4f;

	// Token: 0x040001DC RID: 476
	public float IgnoreNonVisionMaxDistance = 15f;

	// Token: 0x040001DD RID: 477
	public float ListenRange;

	// Token: 0x040001DE RID: 478
	public EntityType SenseTypes;

	// Token: 0x040001DF RID: 479
	public bool HostileTargetsOnly;

	// Token: 0x040001E0 RID: 480
	public bool IgnoreSafeZonePlayers;

	// Token: 0x040001E1 RID: 481
	public int MaxGroupSize;

	// Token: 0x040001E2 RID: 482
	public float MemoryDuration = 10f;

	// Token: 0x040001E3 RID: 483
	public bool RefreshKnownLOS;

	// Token: 0x040001E5 RID: 485
	public AIState ClientCurrentState;

	// Token: 0x040001E6 RID: 486
	public Vector3 mainInterestPoint;

	// Token: 0x040001EB RID: 491
	public bool UseAIDesign;

	// Token: 0x040001F3 RID: 499
	public bool Pet;

	// Token: 0x040001F4 RID: 500
	private List<IAIGroupable> groupMembers = new List<IAIGroupable>();

	// Token: 0x040001F5 RID: 501
	[Header("Healing")]
	public bool CanUseHealingItems;

	// Token: 0x040001F6 RID: 502
	public float HealChance = 0.5f;

	// Token: 0x040001F7 RID: 503
	public float HealBelowHealthFraction = 0.5f;

	// Token: 0x040001F8 RID: 504
	protected int loadedDesignIndex;

	// Token: 0x040001FA RID: 506
	private int currentStateContainerID = -1;

	// Token: 0x040001FB RID: 507
	private float lastMovementTickTime;

	// Token: 0x040001FC RID: 508
	private bool sleeping;

	// Token: 0x040001FD RID: 509
	private bool disabled;

	// Token: 0x040001FE RID: 510
	protected Dictionary<AIState, BaseAIBrain.BasicAIState> states;

	// Token: 0x040001FF RID: 511
	protected float thinkRate = 0.25f;

	// Token: 0x04000200 RID: 512
	protected float lastThinkTime;

	// Token: 0x02000B19 RID: 2841
	public class BaseAttackState : BaseAIBrain.BasicAIState
	{
		// Token: 0x060049ED RID: 18925 RVA: 0x0018EAAB File Offset: 0x0018CCAB
		public BaseAttackState() : base(AIState.Attack)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x060049EE RID: 18926 RVA: 0x0018EABC File Offset: 0x0018CCBC
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.attack = (entity as IAIAttack);
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				Vector3 aimDirection = BaseAIBrain.BaseAttackState.GetAimDirection(brain.Navigator.transform.position, baseEntity.transform.position);
				brain.Navigator.SetFacingDirectionOverride(aimDirection);
				if (this.attack.CanAttack(baseEntity))
				{
					this.StartAttacking(baseEntity);
				}
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x060049EF RID: 18927 RVA: 0x0018EB6B File Offset: 0x0018CD6B
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
			brain.Navigator.Stop();
			this.StopAttacking();
		}

		// Token: 0x060049F0 RID: 18928 RVA: 0x0018EB91 File Offset: 0x0018CD91
		private void StopAttacking()
		{
			this.attack.StopAttacking();
		}

		// Token: 0x060049F1 RID: 18929 RVA: 0x0018EBA0 File Offset: 0x0018CDA0
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (this.attack == null)
			{
				return StateStatus.Error;
			}
			if (baseEntity == null)
			{
				brain.Navigator.ClearFacingDirectionOverride();
				this.StopAttacking();
				return StateStatus.Finished;
			}
			if (brain.Senses.ignoreSafeZonePlayers)
			{
				global::BasePlayer basePlayer = baseEntity as global::BasePlayer;
				if (basePlayer != null && basePlayer.InSafeZone())
				{
					return StateStatus.Error;
				}
			}
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0.25f, 0f))
			{
				return StateStatus.Error;
			}
			Vector3 aimDirection = BaseAIBrain.BaseAttackState.GetAimDirection(brain.Navigator.transform.position, baseEntity.transform.position);
			brain.Navigator.SetFacingDirectionOverride(aimDirection);
			if (this.attack.CanAttack(baseEntity))
			{
				this.StartAttacking(baseEntity);
			}
			else
			{
				this.StopAttacking();
			}
			return StateStatus.Running;
		}

		// Token: 0x060049F2 RID: 18930 RVA: 0x0018EC95 File Offset: 0x0018CE95
		private static Vector3 GetAimDirection(Vector3 from, Vector3 target)
		{
			return Vector3Ex.Direction2D(target, from);
		}

		// Token: 0x060049F3 RID: 18931 RVA: 0x0018EC9E File Offset: 0x0018CE9E
		private void StartAttacking(global::BaseEntity entity)
		{
			this.attack.StartAttacking(entity);
		}

		// Token: 0x04003C9E RID: 15518
		private IAIAttack attack;
	}

	// Token: 0x02000B1A RID: 2842
	public class BaseChaseState : BaseAIBrain.BasicAIState
	{
		// Token: 0x060049F4 RID: 18932 RVA: 0x0018ECAD File Offset: 0x0018CEAD
		public BaseChaseState() : base(AIState.Chase)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x060049F5 RID: 18933 RVA: 0x0018ECC0 File Offset: 0x0018CEC0
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x060049F6 RID: 18934 RVA: 0x0018ED21 File Offset: 0x0018CF21
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x060049F7 RID: 18935 RVA: 0x0018ED31 File Offset: 0x0018CF31
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x060049F8 RID: 18936 RVA: 0x0018ED44 File Offset: 0x0018CF44
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.Stop();
				return StateStatus.Error;
			}
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0.25f, 0f))
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}
	}

	// Token: 0x02000B1B RID: 2843
	public class BaseCooldownState : BaseAIBrain.BasicAIState
	{
		// Token: 0x060049F9 RID: 18937 RVA: 0x0018EDC2 File Offset: 0x0018CFC2
		public BaseCooldownState() : base(AIState.Cooldown)
		{
		}
	}

	// Token: 0x02000B1C RID: 2844
	public class BaseDismountedState : BaseAIBrain.BasicAIState
	{
		// Token: 0x060049FA RID: 18938 RVA: 0x0018EDCC File Offset: 0x0018CFCC
		public BaseDismountedState() : base(AIState.Dismounted)
		{
		}
	}

	// Token: 0x02000B1D RID: 2845
	public class BaseFleeState : BaseAIBrain.BasicAIState
	{
		// Token: 0x060049FB RID: 18939 RVA: 0x0018EDD6 File Offset: 0x0018CFD6
		public BaseFleeState() : base(AIState.Flee)
		{
		}

		// Token: 0x060049FC RID: 18940 RVA: 0x0018EDEC File Offset: 0x0018CFEC
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				this.stopFleeDistance = UnityEngine.Random.Range(80f, 100f) + Mathf.Clamp(Vector3Ex.Distance2D(brain.Navigator.transform.position, baseEntity.transform.position), 0f, 50f);
			}
			this.FleeFrom(brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot), entity);
		}

		// Token: 0x060049FD RID: 18941 RVA: 0x0018EE98 File Offset: 0x0018D098
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x060049FE RID: 18942 RVA: 0x0018ED31 File Offset: 0x0018CF31
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x060049FF RID: 18943 RVA: 0x0018EEA8 File Offset: 0x0018D0A8
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				return StateStatus.Finished;
			}
			if (Vector3Ex.Distance2D(brain.Navigator.transform.position, baseEntity.transform.position) >= this.stopFleeDistance)
			{
				return StateStatus.Finished;
			}
			if ((brain.Navigator.UpdateIntervalElapsed(this.nextInterval) || !brain.Navigator.Moving) && !this.FleeFrom(baseEntity, entity))
			{
				return StateStatus.Error;
			}
			return StateStatus.Running;
		}

		// Token: 0x06004A00 RID: 18944 RVA: 0x0018EF48 File Offset: 0x0018D148
		private bool FleeFrom(global::BaseEntity fleeFromEntity, global::BaseEntity thisEntity)
		{
			if (thisEntity == null || fleeFromEntity == null)
			{
				return false;
			}
			this.nextInterval = UnityEngine.Random.Range(3f, 6f);
			Vector3 pos;
			if (!this.brain.PathFinder.GetBestFleePosition(this.brain.Navigator, this.brain.Senses, fleeFromEntity, this.brain.Events.Memory.Position.Get(4), 50f, 100f, out pos))
			{
				return false;
			}
			bool flag = this.brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			if (!flag)
			{
				this.Stop();
			}
			return flag;
		}

		// Token: 0x04003C9F RID: 15519
		private float nextInterval = 2f;

		// Token: 0x04003CA0 RID: 15520
		private float stopFleeDistance;
	}

	// Token: 0x02000B1E RID: 2846
	public class BaseFollowPathState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004A01 RID: 18945 RVA: 0x0018EFF5 File Offset: 0x0018D1F5
		public BaseFollowPathState() : base(AIState.FollowPath)
		{
		}

		// Token: 0x06004A02 RID: 18946 RVA: 0x0018F000 File Offset: 0x0018D200
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			brain.Navigator.SetBrakingEnabled(false);
			this.path = brain.Navigator.Path;
			if (this.path == null)
			{
				AIInformationZone forPoint = AIInformationZone.GetForPoint(entity.ServerPosition, true);
				if (forPoint == null)
				{
					return;
				}
				this.path = forPoint.GetNearestPath(entity.ServerPosition);
				if (this.path == null)
				{
					return;
				}
			}
			this.currentNodeIndex = this.path.FindNearestPointIndex(entity.ServerPosition);
			this.currentTargetPoint = this.path.FindNearestPoint(entity.ServerPosition);
			if (this.currentTargetPoint == null)
			{
				return;
			}
			this.status = StateStatus.Running;
			this.currentWaitTime = 0f;
			brain.Navigator.SetDestination(this.currentTargetPoint.transform.position, BaseNavigator.NavigationSpeed.Slow, 0f, 0f);
		}

		// Token: 0x06004A03 RID: 18947 RVA: 0x0018F0F5 File Offset: 0x0018D2F5
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
			brain.Navigator.SetBrakingEnabled(true);
		}

		// Token: 0x06004A04 RID: 18948 RVA: 0x0018F118 File Offset: 0x0018D318
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (this.status == StateStatus.Error)
			{
				return this.status;
			}
			if (!brain.Navigator.Moving)
			{
				if (this.currentWaitTime <= 0f && this.currentTargetPoint.HasLookAtPoints())
				{
					Transform randomLookAtPoint = this.currentTargetPoint.GetRandomLookAtPoint();
					if (randomLookAtPoint != null)
					{
						brain.Navigator.SetFacingDirectionOverride(Vector3Ex.Direction2D(randomLookAtPoint.transform.position, entity.ServerPosition));
					}
				}
				if (this.currentTargetPoint.WaitTime > 0f)
				{
					this.currentWaitTime += delta;
				}
				if (this.currentTargetPoint.WaitTime <= 0f || this.currentWaitTime >= this.currentTargetPoint.WaitTime)
				{
					brain.Navigator.ClearFacingDirectionOverride();
					this.currentWaitTime = 0f;
					int num = this.currentNodeIndex;
					this.currentNodeIndex = this.path.GetNextPointIndex(this.currentNodeIndex, ref this.pathDirection);
					this.currentTargetPoint = this.path.GetPointAtIndex(this.currentNodeIndex);
					if ((!(this.currentTargetPoint != null) || this.currentNodeIndex != num) && (this.currentTargetPoint == null || !brain.Navigator.SetDestination(this.currentTargetPoint.transform.position, BaseNavigator.NavigationSpeed.Slow, 0f, 0f)))
					{
						return StateStatus.Error;
					}
				}
			}
			else if (this.currentTargetPoint != null)
			{
				brain.Navigator.SetDestination(this.currentTargetPoint.transform.position, BaseNavigator.NavigationSpeed.Slow, 1f, 0f);
			}
			return StateStatus.Running;
		}

		// Token: 0x04003CA1 RID: 15521
		private AIMovePointPath path;

		// Token: 0x04003CA2 RID: 15522
		private StateStatus status;

		// Token: 0x04003CA3 RID: 15523
		private AIMovePoint currentTargetPoint;

		// Token: 0x04003CA4 RID: 15524
		private float currentWaitTime;

		// Token: 0x04003CA5 RID: 15525
		private AIMovePointPath.PathDirection pathDirection;

		// Token: 0x04003CA6 RID: 15526
		private int currentNodeIndex;
	}

	// Token: 0x02000B1F RID: 2847
	public class BaseIdleState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004A05 RID: 18949 RVA: 0x0018F2BD File Offset: 0x0018D4BD
		public BaseIdleState() : base(AIState.Idle)
		{
		}
	}

	// Token: 0x02000B20 RID: 2848
	public class BaseMountedState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004A06 RID: 18950 RVA: 0x0018F2C6 File Offset: 0x0018D4C6
		public BaseMountedState() : base(AIState.Mounted)
		{
		}

		// Token: 0x06004A07 RID: 18951 RVA: 0x0018F2CF File Offset: 0x0018D4CF
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			brain.Navigator.Stop();
		}
	}

	// Token: 0x02000B21 RID: 2849
	public class BaseMoveTorwardsState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004A08 RID: 18952 RVA: 0x0018F2E4 File Offset: 0x0018D4E4
		public BaseMoveTorwardsState() : base(AIState.MoveTowards)
		{
		}

		// Token: 0x06004A09 RID: 18953 RVA: 0x0018F2EE File Offset: 0x0018D4EE
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004A0A RID: 18954 RVA: 0x0018ED31 File Offset: 0x0018CF31
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004A0B RID: 18955 RVA: 0x0018F300 File Offset: 0x0018D500
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.Stop();
				return StateStatus.Error;
			}
			this.FaceTarget();
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, brain.Navigator.MoveTowardsSpeed, 0.25f, 0f))
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}

		// Token: 0x06004A0C RID: 18956 RVA: 0x0018F390 File Offset: 0x0018D590
		private void FaceTarget()
		{
			if (!this.brain.Navigator.FaceMoveTowardsTarget)
			{
				return;
			}
			global::BaseEntity baseEntity = this.brain.Events.Memory.Entity.Get(this.brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.brain.Navigator.ClearFacingDirectionOverride();
				return;
			}
			if (Vector3.Distance(baseEntity.transform.position, this.brain.transform.position) <= 1.5f)
			{
				this.brain.Navigator.SetFacingDirectionEntity(baseEntity);
			}
		}
	}

	// Token: 0x02000B22 RID: 2850
	public class BaseNavigateHomeState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004A0D RID: 18957 RVA: 0x0018F42D File Offset: 0x0018D62D
		public BaseNavigateHomeState() : base(AIState.NavigateHome)
		{
		}

		// Token: 0x06004A0E RID: 18958 RVA: 0x0018F438 File Offset: 0x0018D638
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			Vector3 pos = brain.Events.Memory.Position.Get(4);
			this.status = StateStatus.Running;
			if (!brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Normal, 0f, 0f))
			{
				this.status = StateStatus.Error;
			}
		}

		// Token: 0x06004A0F RID: 18959 RVA: 0x0018F48B File Offset: 0x0018D68B
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004A10 RID: 18960 RVA: 0x0018ED31 File Offset: 0x0018CF31
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004A11 RID: 18961 RVA: 0x0018F49B File Offset: 0x0018D69B
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (this.status == StateStatus.Error)
			{
				return this.status;
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}

		// Token: 0x04003CA7 RID: 15527
		private StateStatus status;
	}

	// Token: 0x02000B23 RID: 2851
	public class BasePatrolState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004A12 RID: 18962 RVA: 0x0018F4C7 File Offset: 0x0018D6C7
		public BasePatrolState() : base(AIState.Patrol)
		{
		}
	}

	// Token: 0x02000B24 RID: 2852
	public class BaseRoamState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004A13 RID: 18963 RVA: 0x0018F4D0 File Offset: 0x0018D6D0
		public BaseRoamState() : base(AIState.Roam)
		{
		}

		// Token: 0x06004A14 RID: 18964 RVA: 0x00026FFC File Offset: 0x000251FC
		public override float GetWeight()
		{
			return 0f;
		}

		// Token: 0x06004A15 RID: 18965 RVA: 0x0018F4E4 File Offset: 0x0018D6E4
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.nextRoamPositionTime = -1f;
			this.lastDestinationTime = UnityEngine.Time.time;
		}

		// Token: 0x06004A16 RID: 18966 RVA: 0x00029180 File Offset: 0x00027380
		public virtual Vector3 GetDestination()
		{
			return Vector3.zero;
		}

		// Token: 0x06004A17 RID: 18967 RVA: 0x0018F504 File Offset: 0x0018D704
		public virtual Vector3 GetForwardDirection()
		{
			return Vector3.forward;
		}

		// Token: 0x06004A18 RID: 18968 RVA: 0x000059DD File Offset: 0x00003BDD
		public virtual void SetDestination(Vector3 destination)
		{
		}

		// Token: 0x06004A19 RID: 18969 RVA: 0x0018F50B File Offset: 0x0018D70B
		public override void DrawGizmos()
		{
			base.DrawGizmos();
			this.brain.PathFinder.DebugDraw();
		}

		// Token: 0x06004A1A RID: 18970 RVA: 0x0018F524 File Offset: 0x0018D724
		public virtual Vector3 GetRoamAnchorPosition()
		{
			if (this.brain.Navigator.MaxRoamDistanceFromHome > -1f)
			{
				return this.brain.Events.Memory.Position.Get(4);
			}
			return this.brain.GetBaseEntity().transform.position;
		}

		// Token: 0x06004A1B RID: 18971 RVA: 0x0018F57C File Offset: 0x0018D77C
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			bool flag = UnityEngine.Time.time - this.lastDestinationTime > 25f;
			if ((Vector3.Distance(this.GetDestination(), entity.transform.position) < 2f || flag) && this.nextRoamPositionTime == -1f)
			{
				this.nextRoamPositionTime = UnityEngine.Time.time + UnityEngine.Random.Range(5f, 10f);
			}
			if (this.nextRoamPositionTime != -1f && UnityEngine.Time.time > this.nextRoamPositionTime)
			{
				AIMovePoint bestRoamPoint = brain.PathFinder.GetBestRoamPoint(this.GetRoamAnchorPosition(), entity.ServerPosition, this.GetForwardDirection(), brain.Navigator.MaxRoamDistanceFromHome, brain.Navigator.BestRoamPointMaxDistance);
				if (bestRoamPoint)
				{
					float num = Vector3.Distance(bestRoamPoint.transform.position, entity.transform.position) / 1.5f;
					bestRoamPoint.SetUsedBy(entity, num + 11f);
				}
				this.lastDestinationTime = UnityEngine.Time.time;
				Vector3 insideUnitSphere = UnityEngine.Random.insideUnitSphere;
				insideUnitSphere.y = 0f;
				insideUnitSphere.Normalize();
				Vector3 destination = (bestRoamPoint == null) ? entity.transform.position : (bestRoamPoint.transform.position + insideUnitSphere * bestRoamPoint.radius);
				this.SetDestination(destination);
				this.nextRoamPositionTime = -1f;
			}
			return StateStatus.Running;
		}

		// Token: 0x04003CA8 RID: 15528
		private float nextRoamPositionTime = -1f;

		// Token: 0x04003CA9 RID: 15529
		private float lastDestinationTime;
	}

	// Token: 0x02000B25 RID: 2853
	public class BaseSleepState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004A1C RID: 18972 RVA: 0x0018F6E8 File Offset: 0x0018D8E8
		public BaseSleepState() : base(AIState.Sleep)
		{
		}

		// Token: 0x06004A1D RID: 18973 RVA: 0x0018F6FC File Offset: 0x0018D8FC
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			IAISleep iaisleep;
			if ((iaisleep = (entity as IAISleep)) == null)
			{
				return;
			}
			iaisleep.StartSleeping();
			this.status = StateStatus.Running;
		}

		// Token: 0x06004A1E RID: 18974 RVA: 0x0018F730 File Offset: 0x0018D930
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			IAISleep iaisleep;
			if ((iaisleep = (entity as IAISleep)) == null)
			{
				return;
			}
			iaisleep.StopSleeping();
		}

		// Token: 0x06004A1F RID: 18975 RVA: 0x0018F756 File Offset: 0x0018D956
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			return this.status;
		}

		// Token: 0x04003CAA RID: 15530
		private StateStatus status = StateStatus.Error;
	}

	// Token: 0x02000B26 RID: 2854
	public class BasicAIState
	{
		// Token: 0x170005ED RID: 1517
		// (get) Token: 0x06004A20 RID: 18976 RVA: 0x0018F768 File Offset: 0x0018D968
		// (set) Token: 0x06004A21 RID: 18977 RVA: 0x0018F770 File Offset: 0x0018D970
		public AIState StateType { get; private set; }

		// Token: 0x06004A22 RID: 18978 RVA: 0x0018F779 File Offset: 0x0018D979
		public virtual void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			this.TimeInState = 0f;
		}

		// Token: 0x06004A23 RID: 18979 RVA: 0x0018F786 File Offset: 0x0018D986
		public virtual StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			this.TimeInState += delta;
			return StateStatus.Running;
		}

		// Token: 0x06004A24 RID: 18980 RVA: 0x0018F797 File Offset: 0x0018D997
		public virtual void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			this.TimeInState = 0f;
			this._lastStateExitTime = UnityEngine.Time.time;
		}

		// Token: 0x06004A25 RID: 18981 RVA: 0x00003A54 File Offset: 0x00001C54
		public virtual bool CanInterrupt()
		{
			return true;
		}

		// Token: 0x06004A26 RID: 18982 RVA: 0x00003A54 File Offset: 0x00001C54
		public virtual bool CanEnter()
		{
			return true;
		}

		// Token: 0x06004A27 RID: 18983 RVA: 0x0018F7AF File Offset: 0x0018D9AF
		public virtual bool CanLeave()
		{
			return this.CanInterrupt();
		}

		// Token: 0x06004A28 RID: 18984 RVA: 0x00026FFC File Offset: 0x000251FC
		public virtual float GetWeight()
		{
			return 0f;
		}

		// Token: 0x170005EE RID: 1518
		// (get) Token: 0x06004A29 RID: 18985 RVA: 0x0018F7B7 File Offset: 0x0018D9B7
		// (set) Token: 0x06004A2A RID: 18986 RVA: 0x0018F7BF File Offset: 0x0018D9BF
		public float TimeInState { get; private set; }

		// Token: 0x06004A2B RID: 18987 RVA: 0x0018F7C8 File Offset: 0x0018D9C8
		public float TimeSinceState()
		{
			return UnityEngine.Time.time - this._lastStateExitTime;
		}

		// Token: 0x170005EF RID: 1519
		// (get) Token: 0x06004A2C RID: 18988 RVA: 0x0018F7D6 File Offset: 0x0018D9D6
		// (set) Token: 0x06004A2D RID: 18989 RVA: 0x0018F7DE File Offset: 0x0018D9DE
		public bool AgrresiveState { get; protected set; }

		// Token: 0x06004A2E RID: 18990 RVA: 0x0018F7E7 File Offset: 0x0018D9E7
		public BasicAIState(AIState state)
		{
			this.StateType = state;
		}

		// Token: 0x06004A2F RID: 18991 RVA: 0x0018F779 File Offset: 0x0018D979
		public void Reset()
		{
			this.TimeInState = 0f;
		}

		// Token: 0x06004A30 RID: 18992 RVA: 0x0018F7F6 File Offset: 0x0018D9F6
		public bool IsInState()
		{
			return this.brain != null && this.brain.CurrentState != null && this.brain.CurrentState == this;
		}

		// Token: 0x06004A31 RID: 18993 RVA: 0x000059DD File Offset: 0x00003BDD
		public virtual void DrawGizmos()
		{
		}

		// Token: 0x04003CAC RID: 15532
		public BaseAIBrain brain;

		// Token: 0x04003CAE RID: 15534
		protected float _lastStateExitTime;
	}
}
