using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000083 RID: 131
public class IOEntity : global::DecayEntity
{
	// Token: 0x06000C46 RID: 3142 RVA: 0x000697A4 File Offset: 0x000679A4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("IOEntity.OnRpcMessage", 0))
		{
			if (rpc == 4161541566U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RequestData ");
				}
				using (TimeWarning.New("Server_RequestData", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(4161541566U, "Server_RequestData", this, player, 10UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(4161541566U, "Server_RequestData", this, player, 6f))
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
							this.Server_RequestData(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_RequestData");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000C47 RID: 3143 RVA: 0x00069968 File Offset: 0x00067B68
	public override void ResetState()
	{
		base.ResetState();
		if (base.isServer)
		{
			this.lastResetIndex = 0;
			this.cachedOutputsUsed = 0;
			this.lastPassthroughEnergy = 0;
			this.lastEnergy = 0;
			this.currentEnergy = 0;
			this.lastUpdateTime = 0f;
			this.ensureOutputsUpdated = false;
		}
		this.ClearIndustrialPreventBuilding();
	}

	// Token: 0x06000C48 RID: 3144 RVA: 0x000699BE File Offset: 0x00067BBE
	public string GetDisplayName()
	{
		if (this.sourceItem != null)
		{
			return this.sourceItem.displayName.translated;
		}
		return base.ShortPrefabName;
	}

	// Token: 0x06000C49 RID: 3145 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool IsRootEntity()
	{
		return false;
	}

	// Token: 0x17000111 RID: 273
	// (get) Token: 0x06000C4A RID: 3146 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool IsGravitySource
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000C4B RID: 3147 RVA: 0x000699E8 File Offset: 0x00067BE8
	public global::IOEntity FindGravitySource(ref Vector3 worldHandlePosition, int depth, bool ignoreSelf)
	{
		if (depth <= 0)
		{
			return null;
		}
		if (!ignoreSelf && this.IsGravitySource)
		{
			worldHandlePosition = base.transform.TransformPoint(this.outputs[0].handlePosition);
			return this;
		}
		global::IOEntity.IOSlot[] array = this.inputs;
		for (int i = 0; i < array.Length; i++)
		{
			global::IOEntity ioentity = array[i].connectedTo.Get(base.isServer);
			if (ioentity != null)
			{
				if (ioentity.IsGravitySource)
				{
					worldHandlePosition = ioentity.transform.TransformPoint(ioentity.outputs[0].handlePosition);
					return ioentity;
				}
				ioentity = ioentity.FindGravitySource(ref worldHandlePosition, depth - 1, false);
				if (ioentity != null)
				{
					worldHandlePosition = ioentity.transform.TransformPoint(ioentity.outputs[0].handlePosition);
					return ioentity;
				}
			}
		}
		return null;
	}

	// Token: 0x06000C4C RID: 3148 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void SetFuelType(ItemDefinition def, global::IOEntity source)
	{
	}

	// Token: 0x06000C4D RID: 3149 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool WantsPower()
	{
		return true;
	}

	// Token: 0x06000C4E RID: 3150 RVA: 0x00069AB9 File Offset: 0x00067CB9
	public virtual bool WantsPassthroughPower()
	{
		return this.WantsPower();
	}

	// Token: 0x06000C4F RID: 3151 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual int ConsumptionAmount()
	{
		return 1;
	}

	// Token: 0x06000C50 RID: 3152 RVA: 0x00069AC1 File Offset: 0x00067CC1
	public virtual bool ShouldDrainBattery(global::IOEntity battery)
	{
		return this.ioType == battery.ioType;
	}

	// Token: 0x06000C51 RID: 3153 RVA: 0x00007074 File Offset: 0x00005274
	public virtual int MaximalPowerOutput()
	{
		return 0;
	}

	// Token: 0x06000C52 RID: 3154 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool AllowDrainFrom(int outputSlot)
	{
		return true;
	}

	// Token: 0x06000C53 RID: 3155 RVA: 0x000028C8 File Offset: 0x00000AC8
	public virtual bool IsPowered()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved8);
	}

	// Token: 0x06000C54 RID: 3156 RVA: 0x00069AD4 File Offset: 0x00067CD4
	public bool IsConnectedToAnySlot(global::IOEntity entity, int slot, int depth, bool defaultReturn = false)
	{
		if (depth > 0 && slot < this.inputs.Length)
		{
			global::IOEntity ioentity = this.inputs[slot].connectedTo.Get(true);
			if (ioentity != null)
			{
				if (ioentity == entity)
				{
					return true;
				}
				if (this.ConsiderConnectedTo(entity))
				{
					return true;
				}
				if (ioentity.IsConnectedTo(entity, depth - 1, defaultReturn))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000C55 RID: 3157 RVA: 0x00069B34 File Offset: 0x00067D34
	public bool IsConnectedTo(global::IOEntity entity, int slot, int depth, bool defaultReturn = false)
	{
		if (depth > 0 && slot < this.inputs.Length)
		{
			global::IOEntity.IOSlot ioslot = this.inputs[slot];
			if (ioslot.mainPowerSlot)
			{
				global::IOEntity ioentity = ioslot.connectedTo.Get(true);
				if (ioentity != null)
				{
					if (ioentity == entity)
					{
						return true;
					}
					if (this.ConsiderConnectedTo(entity))
					{
						return true;
					}
					if (ioentity.IsConnectedTo(entity, depth - 1, defaultReturn))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06000C56 RID: 3158 RVA: 0x00069BA0 File Offset: 0x00067DA0
	public bool IsConnectedTo(global::IOEntity entity, int depth, bool defaultReturn = false)
	{
		if (depth > 0)
		{
			for (int i = 0; i < this.inputs.Length; i++)
			{
				global::IOEntity.IOSlot ioslot = this.inputs[i];
				if (ioslot.mainPowerSlot)
				{
					global::IOEntity ioentity = ioslot.connectedTo.Get(true);
					if (ioentity != null)
					{
						if (ioentity == entity)
						{
							return true;
						}
						if (this.ConsiderConnectedTo(entity))
						{
							return true;
						}
						if (ioentity.IsConnectedTo(entity, depth - 1, defaultReturn))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		return defaultReturn;
	}

	// Token: 0x06000C57 RID: 3159 RVA: 0x00007074 File Offset: 0x00005274
	protected virtual bool ConsiderConnectedTo(global::IOEntity entity)
	{
		return false;
	}

	// Token: 0x06000C58 RID: 3160 RVA: 0x00069C18 File Offset: 0x00067E18
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(6f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(10UL)]
	private void Server_RequestData(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		int slot = msg.read.Int32();
		bool input = msg.read.Int32() == 1;
		this.SendAdditionalData(player, slot, input);
	}

	// Token: 0x06000C59 RID: 3161 RVA: 0x00069C50 File Offset: 0x00067E50
	public virtual void SendAdditionalData(global::BasePlayer player, int slot, bool input)
	{
		int passthroughAmountForAnySlot = this.GetPassthroughAmountForAnySlot(slot, input);
		base.ClientRPCPlayer<int, int, float, float>(null, player, "Client_ReceiveAdditionalData", this.currentEnergy, passthroughAmountForAnySlot, 0f, 0f);
	}

	// Token: 0x06000C5A RID: 3162 RVA: 0x00069C84 File Offset: 0x00067E84
	protected int GetPassthroughAmountForAnySlot(int slot, bool isInputSlot)
	{
		int result = 0;
		if (isInputSlot)
		{
			if (slot >= 0 && slot < this.inputs.Length)
			{
				global::IOEntity.IOSlot ioslot = this.inputs[slot];
				global::IOEntity ioentity = ioslot.connectedTo.Get(true);
				if (ioentity != null && ioslot.connectedToSlot >= 0 && ioslot.connectedToSlot < ioentity.outputs.Length)
				{
					result = ioentity.GetPassthroughAmount(this.inputs[slot].connectedToSlot);
				}
			}
		}
		else if (slot >= 0 && slot < this.outputs.Length)
		{
			result = this.GetPassthroughAmount(slot);
		}
		return result;
	}

	// Token: 0x06000C5B RID: 3163 RVA: 0x00069D0C File Offset: 0x00067F0C
	public static void ProcessQueue()
	{
		float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
		float num = global::IOEntity.framebudgetms / 1000f;
		if (global::IOEntity.debugBudget)
		{
			global::IOEntity.timings.Clear();
		}
		while (global::IOEntity._processQueue.Count > 0 && UnityEngine.Time.realtimeSinceStartup < realtimeSinceStartup + num && !global::IOEntity._processQueue.Peek().HasBlockedUpdatedOutputsThisFrame)
		{
			float realtimeSinceStartup2 = UnityEngine.Time.realtimeSinceStartup;
			global::IOEntity ioentity = global::IOEntity._processQueue.Dequeue();
			if (ioentity.IsValid())
			{
				ioentity.UpdateOutputs();
			}
			if (global::IOEntity.debugBudget)
			{
				global::IOEntity.timings.Add(new global::IOEntity.FrameTiming
				{
					PrefabName = ioentity.ShortPrefabName,
					Time = (UnityEngine.Time.realtimeSinceStartup - realtimeSinceStartup2) * 1000f
				});
			}
		}
		if (global::IOEntity.debugBudget)
		{
			float num2 = UnityEngine.Time.realtimeSinceStartup - realtimeSinceStartup;
			float num3 = global::IOEntity.debugBudgetThreshold / 1000f;
			if (num2 > num3)
			{
				TextTable textTable = new TextTable();
				textTable.AddColumns(new string[]
				{
					"Prefab Name",
					"Time (in ms)"
				});
				foreach (global::IOEntity.FrameTiming frameTiming in global::IOEntity.timings)
				{
					TextTable textTable2 = textTable;
					string[] array = new string[2];
					array[0] = frameTiming.PrefabName;
					int num4 = 1;
					float time = frameTiming.Time;
					array[num4] = time.ToString();
					textTable2.AddRow(array);
				}
				textTable.AddRow(new string[]
				{
					"Total time",
					(num2 * 1000f).ToString()
				});
				Debug.Log(textTable.ToString());
			}
		}
	}

	// Token: 0x06000C5C RID: 3164 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void ResetIOState()
	{
	}

	// Token: 0x06000C5D RID: 3165 RVA: 0x00069EB4 File Offset: 0x000680B4
	public virtual void Init()
	{
		for (int i = 0; i < this.outputs.Length; i++)
		{
			global::IOEntity.IOSlot ioslot = this.outputs[i];
			ioslot.connectedTo.Init();
			if (ioslot.connectedTo.Get(true) != null)
			{
				int connectedToSlot = ioslot.connectedToSlot;
				if (connectedToSlot < 0 || connectedToSlot >= ioslot.connectedTo.Get(true).inputs.Length)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"Slot IOR Error: ",
						base.name,
						" setting up inputs for ",
						ioslot.connectedTo.Get(true).name,
						" slot : ",
						ioslot.connectedToSlot
					}));
				}
				else
				{
					ioslot.connectedTo.Get(true).inputs[ioslot.connectedToSlot].connectedTo.Set(this);
					ioslot.connectedTo.Get(true).inputs[ioslot.connectedToSlot].connectedToSlot = i;
					ioslot.connectedTo.Get(true).inputs[ioslot.connectedToSlot].connectedTo.Init();
				}
			}
		}
		this.UpdateUsedOutputs();
		if (this.IsRootEntity())
		{
			base.Invoke(new Action(this.MarkDirtyForceUpdateOutputs), UnityEngine.Random.Range(1f, 1f));
		}
	}

	// Token: 0x06000C5E RID: 3166 RVA: 0x0006A00D File Offset: 0x0006820D
	internal override void DoServerDestroy()
	{
		if (base.isServer)
		{
			this.Shutdown();
		}
		base.DoServerDestroy();
	}

	// Token: 0x06000C5F RID: 3167 RVA: 0x0006A024 File Offset: 0x00068224
	public void ClearConnections()
	{
		List<global::IOEntity> list = new List<global::IOEntity>();
		foreach (global::IOEntity.IOSlot ioslot in this.inputs)
		{
			global::IOEntity ioentity = null;
			if (ioslot.connectedTo.Get(true) != null)
			{
				ioentity = ioslot.connectedTo.Get(true);
				foreach (global::IOEntity.IOSlot ioslot2 in ioslot.connectedTo.Get(true).outputs)
				{
					if (ioslot2.connectedTo.Get(true) != null && ioslot2.connectedTo.Get(true).EqualNetID(this))
					{
						ioslot2.Clear();
					}
				}
			}
			ioslot.Clear();
			if (ioentity)
			{
				ioentity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
		foreach (global::IOEntity.IOSlot ioslot3 in this.outputs)
		{
			if (ioslot3.connectedTo.Get(true) != null)
			{
				list.Add(ioslot3.connectedTo.Get(true));
				foreach (global::IOEntity.IOSlot ioslot4 in ioslot3.connectedTo.Get(true).inputs)
				{
					if (ioslot4.connectedTo.Get(true) != null && ioslot4.connectedTo.Get(true).EqualNetID(this))
					{
						ioslot4.Clear();
					}
				}
			}
			if (ioslot3.connectedTo.Get(true))
			{
				ioslot3.connectedTo.Get(true).UpdateFromInput(0, ioslot3.connectedToSlot);
			}
			ioslot3.Clear();
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		foreach (global::IOEntity ioentity2 in list)
		{
			if (ioentity2 != null)
			{
				ioentity2.MarkDirty();
				ioentity2.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
		for (int k = 0; k < this.inputs.Length; k++)
		{
			this.UpdateFromInput(0, k);
		}
	}

	// Token: 0x06000C60 RID: 3168 RVA: 0x0006A248 File Offset: 0x00068448
	public void Shutdown()
	{
		this.SendChangedToRoot(true);
		this.ClearConnections();
	}

	// Token: 0x06000C61 RID: 3169 RVA: 0x0006A257 File Offset: 0x00068457
	public void MarkDirtyForceUpdateOutputs()
	{
		this.ensureOutputsUpdated = true;
		this.MarkDirty();
	}

	// Token: 0x06000C62 RID: 3170 RVA: 0x0006A268 File Offset: 0x00068468
	public void UpdateUsedOutputs()
	{
		this.cachedOutputsUsed = 0;
		global::IOEntity.IOSlot[] array = this.outputs;
		for (int i = 0; i < array.Length; i++)
		{
			global::IOEntity ioentity = array[i].connectedTo.Get(true);
			if (ioentity != null && !ioentity.IsDestroyed)
			{
				this.cachedOutputsUsed++;
			}
		}
	}

	// Token: 0x06000C63 RID: 3171 RVA: 0x0006A2BF File Offset: 0x000684BF
	public virtual void MarkDirty()
	{
		if (base.isClient)
		{
			return;
		}
		this.UpdateUsedOutputs();
		this.TouchIOState();
	}

	// Token: 0x06000C64 RID: 3172 RVA: 0x0006A2D6 File Offset: 0x000684D6
	public virtual int DesiredPower()
	{
		return this.ConsumptionAmount();
	}

	// Token: 0x06000C65 RID: 3173 RVA: 0x0003421C File Offset: 0x0003241C
	public virtual int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		return inputAmount;
	}

	// Token: 0x06000C66 RID: 3174 RVA: 0x0006A2DE File Offset: 0x000684DE
	public virtual int GetCurrentEnergy()
	{
		return Mathf.Clamp(this.currentEnergy - this.ConsumptionAmount(), 0, this.currentEnergy);
	}

	// Token: 0x06000C67 RID: 3175 RVA: 0x0006A2FC File Offset: 0x000684FC
	public virtual int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot < 0 || outputSlot >= this.outputs.Length)
		{
			return 0;
		}
		global::IOEntity ioentity = this.outputs[outputSlot].connectedTo.Get(true);
		if (ioentity == null || ioentity.IsDestroyed)
		{
			return 0;
		}
		int num = (this.cachedOutputsUsed == 0) ? 1 : this.cachedOutputsUsed;
		return this.GetCurrentEnergy() / num;
	}

	// Token: 0x06000C68 RID: 3176 RVA: 0x0006A35B File Offset: 0x0006855B
	public virtual void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved8, inputAmount >= this.ConsumptionAmount() && inputAmount > 0, false, false);
	}

	// Token: 0x06000C69 RID: 3177 RVA: 0x0006A37C File Offset: 0x0006857C
	public void TouchInternal()
	{
		int passthroughAmount = this.GetPassthroughAmount(0);
		bool flag = this.lastPassthroughEnergy != passthroughAmount;
		this.lastPassthroughEnergy = passthroughAmount;
		if (flag)
		{
			this.IOStateChanged(this.currentEnergy, 0);
			this.ensureOutputsUpdated = true;
		}
		global::IOEntity._processQueue.Enqueue(this);
	}

	// Token: 0x06000C6A RID: 3178 RVA: 0x0006A3C8 File Offset: 0x000685C8
	public virtual void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (this.inputs[inputSlot].type != this.ioType || this.inputs[inputSlot].type == global::IOEntity.IOType.Industrial)
		{
			this.IOStateChanged(inputAmount, inputSlot);
			return;
		}
		this.UpdateHasPower(inputAmount, inputSlot);
		this.lastEnergy = this.currentEnergy;
		this.currentEnergy = this.CalculateCurrentEnergy(inputAmount, inputSlot);
		int passthroughAmount = this.GetPassthroughAmount(0);
		bool flag = this.lastPassthroughEnergy != passthroughAmount;
		this.lastPassthroughEnergy = passthroughAmount;
		if (this.currentEnergy != this.lastEnergy || flag)
		{
			this.IOStateChanged(inputAmount, inputSlot);
			this.ensureOutputsUpdated = true;
		}
		global::IOEntity._processQueue.Enqueue(this);
	}

	// Token: 0x06000C6B RID: 3179 RVA: 0x0006A470 File Offset: 0x00068670
	public virtual void TouchIOState()
	{
		if (base.isClient)
		{
			return;
		}
		this.TouchInternal();
	}

	// Token: 0x06000C6C RID: 3180 RVA: 0x0006A481 File Offset: 0x00068681
	public virtual void SendIONetworkUpdate()
	{
		base.SendNetworkUpdate_Flags();
	}

	// Token: 0x06000C6D RID: 3181 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void IOStateChanged(int inputAmount, int inputSlot)
	{
	}

	// Token: 0x06000C6E RID: 3182 RVA: 0x0006A489 File Offset: 0x00068689
	public virtual void OnCircuitChanged(bool forceUpdate)
	{
		if (forceUpdate)
		{
			this.MarkDirtyForceUpdateOutputs();
		}
	}

	// Token: 0x06000C6F RID: 3183 RVA: 0x0006A494 File Offset: 0x00068694
	public virtual void SendChangedToRoot(bool forceUpdate)
	{
		List<global::IOEntity> list = Facepunch.Pool.GetList<global::IOEntity>();
		this.SendChangedToRootRecursive(forceUpdate, ref list);
		Facepunch.Pool.FreeList<global::IOEntity>(ref list);
	}

	// Token: 0x06000C70 RID: 3184 RVA: 0x0006A4B8 File Offset: 0x000686B8
	public virtual void SendChangedToRootRecursive(bool forceUpdate, ref List<global::IOEntity> existing)
	{
		bool flag = this.IsRootEntity();
		if (!existing.Contains(this))
		{
			existing.Add(this);
			bool flag2 = false;
			for (int i = 0; i < this.inputs.Length; i++)
			{
				global::IOEntity.IOSlot ioslot = this.inputs[i];
				if (ioslot.mainPowerSlot)
				{
					global::IOEntity ioentity = ioslot.connectedTo.Get(true);
					if (!(ioentity == null) && !existing.Contains(ioentity))
					{
						flag2 = true;
						if (forceUpdate)
						{
							ioentity.ensureOutputsUpdated = true;
						}
						ioentity.SendChangedToRootRecursive(forceUpdate, ref existing);
					}
				}
			}
			if (flag)
			{
				forceUpdate = (forceUpdate && !flag2);
				this.OnCircuitChanged(forceUpdate);
			}
		}
	}

	// Token: 0x06000C71 RID: 3185 RVA: 0x0006A554 File Offset: 0x00068754
	protected bool ShouldUpdateOutputs()
	{
		if (UnityEngine.Time.realtimeSinceStartup - this.lastUpdateTime < global::IOEntity.responsetime)
		{
			this.lastUpdateBlockedFrame = UnityEngine.Time.frameCount;
			global::IOEntity._processQueue.Enqueue(this);
			return false;
		}
		this.lastUpdateTime = UnityEngine.Time.realtimeSinceStartup;
		this.SendIONetworkUpdate();
		if (this.outputs.Length == 0)
		{
			this.ensureOutputsUpdated = false;
			return false;
		}
		return true;
	}

	// Token: 0x17000112 RID: 274
	// (get) Token: 0x06000C72 RID: 3186 RVA: 0x0006A5B0 File Offset: 0x000687B0
	private bool HasBlockedUpdatedOutputsThisFrame
	{
		get
		{
			return UnityEngine.Time.frameCount == this.lastUpdateBlockedFrame;
		}
	}

	// Token: 0x06000C73 RID: 3187 RVA: 0x0006A5C0 File Offset: 0x000687C0
	public virtual void UpdateOutputs()
	{
		if (!this.ShouldUpdateOutputs())
		{
			return;
		}
		if (this.ensureOutputsUpdated)
		{
			this.ensureOutputsUpdated = false;
			using (TimeWarning.New("ProcessIOOutputs", 0))
			{
				for (int i = 0; i < this.outputs.Length; i++)
				{
					global::IOEntity.IOSlot ioslot = this.outputs[i];
					bool flag = true;
					global::IOEntity ioentity = ioslot.connectedTo.Get(true);
					if (ioentity != null)
					{
						if (this.ioType == global::IOEntity.IOType.Fluidic && !this.DisregardGravityRestrictionsOnLiquid && !ioentity.DisregardGravityRestrictionsOnLiquid)
						{
							using (TimeWarning.New("FluidOutputProcessing", 0))
							{
								if (!ioentity.AllowLiquidPassthrough(this, base.transform.TransformPoint(ioslot.handlePosition), false))
								{
									flag = false;
								}
							}
						}
						int passthroughAmount = this.GetPassthroughAmount(i);
						ioentity.UpdateFromInput(flag ? passthroughAmount : 0, ioslot.connectedToSlot);
					}
				}
			}
		}
	}

	// Token: 0x06000C74 RID: 3188 RVA: 0x0006A6CC File Offset: 0x000688CC
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			this.Init();
		}
	}

	// Token: 0x06000C75 RID: 3189 RVA: 0x0006A6E1 File Offset: 0x000688E1
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.Init();
	}

	// Token: 0x06000C76 RID: 3190 RVA: 0x0006A6EF File Offset: 0x000688EF
	public override void PostMapEntitySpawn()
	{
		base.PostMapEntitySpawn();
		this.Init();
	}

	// Token: 0x06000C77 RID: 3191 RVA: 0x0006A700 File Offset: 0x00068900
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.inputs = Facepunch.Pool.GetList<ProtoBuf.IOEntity.IOConnection>();
		info.msg.ioEntity.outputs = Facepunch.Pool.GetList<ProtoBuf.IOEntity.IOConnection>();
		foreach (global::IOEntity.IOSlot ioslot in this.inputs)
		{
			ProtoBuf.IOEntity.IOConnection ioconnection = Facepunch.Pool.Get<ProtoBuf.IOEntity.IOConnection>();
			ioconnection.connectedID = ioslot.connectedTo.entityRef.uid;
			ioconnection.connectedToSlot = ioslot.connectedToSlot;
			ioconnection.niceName = ioslot.niceName;
			ioconnection.type = (int)ioslot.type;
			ioconnection.inUse = (ioconnection.connectedID > 0U);
			ioconnection.colour = (int)ioslot.wireColour;
			info.msg.ioEntity.inputs.Add(ioconnection);
		}
		foreach (global::IOEntity.IOSlot ioslot2 in this.outputs)
		{
			ProtoBuf.IOEntity.IOConnection ioconnection2 = Facepunch.Pool.Get<ProtoBuf.IOEntity.IOConnection>();
			ioconnection2.connectedID = ioslot2.connectedTo.entityRef.uid;
			ioconnection2.connectedToSlot = ioslot2.connectedToSlot;
			ioconnection2.niceName = ioslot2.niceName;
			ioconnection2.type = (int)ioslot2.type;
			ioconnection2.inUse = (ioconnection2.connectedID > 0U);
			ioconnection2.colour = (int)ioslot2.wireColour;
			ioconnection2.worldSpaceRotation = ioslot2.worldSpaceLineEndRotation;
			if (ioslot2.linePoints != null)
			{
				ioconnection2.linePointList = Facepunch.Pool.GetList<ProtoBuf.IOEntity.IOConnection.LineVec>();
				ioconnection2.linePointList.Clear();
				for (int j = 0; j < ioslot2.linePoints.Length; j++)
				{
					Vector3 v = ioslot2.linePoints[j];
					ProtoBuf.IOEntity.IOConnection.LineVec lineVec = Facepunch.Pool.Get<ProtoBuf.IOEntity.IOConnection.LineVec>();
					lineVec.vec = v;
					if (ioslot2.slackLevels.Length > j)
					{
						lineVec.vec.w = ioslot2.slackLevels[j];
					}
					ioconnection2.linePointList.Add(lineVec);
				}
			}
			info.msg.ioEntity.outputs.Add(ioconnection2);
		}
	}

	// Token: 0x06000C78 RID: 3192 RVA: 0x0006A918 File Offset: 0x00068B18
	public virtual float IOInput(global::IOEntity from, global::IOEntity.IOType inputType, float inputAmount, int slot = 0)
	{
		foreach (global::IOEntity.IOSlot ioslot in this.outputs)
		{
			if (ioslot.connectedTo.Get(true) != null)
			{
				inputAmount = ioslot.connectedTo.Get(true).IOInput(this, ioslot.type, inputAmount, ioslot.connectedToSlot);
			}
		}
		return inputAmount;
	}

	// Token: 0x17000113 RID: 275
	// (get) Token: 0x06000C79 RID: 3193 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool BlockFluidDraining
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000C7A RID: 3194 RVA: 0x0006A974 File Offset: 0x00068B74
	public void FindContainerSource(List<global::IOEntity.ContainerInputOutput> found, int depth, bool input, Func<IIndustrialStorage, int, bool> validFilter)
	{
		global::IOEntity.<>c__DisplayClass83_0 CS$<>8__locals1;
		CS$<>8__locals1.found = found;
		if (depth <= 0 || CS$<>8__locals1.found.Count >= 16)
		{
			return;
		}
		int num = 0;
		foreach (global::IOEntity.IOSlot ioslot in input ? this.inputs : this.outputs)
		{
			num++;
			if (ioslot.type != global::IOEntity.IOType.Industrial)
			{
				return;
			}
			global::IOEntity ioentity = ioslot.connectedTo.Get(base.isServer);
			if (ioentity != null)
			{
				IIndustrialStorage industrialStorage;
				if ((industrialStorage = (ioentity as IIndustrialStorage)) != null && (validFilter == null || validFilter(industrialStorage, ioslot.connectedToSlot)))
				{
					num = ioslot.connectedToSlot;
					if (global::IOEntity.<FindContainerSource>g__GetExistingCount|83_0(industrialStorage, ref CS$<>8__locals1) < 2)
					{
						CS$<>8__locals1.found.Add(new global::IOEntity.ContainerInputOutput
						{
							SlotIndex = num,
							Storage = industrialStorage
						});
					}
				}
				if ((!(ioentity is IIndustrialStorage) || ioentity is IndustrialStorageAdaptor) && !(ioentity is global::IndustrialConveyor) && ioentity != null)
				{
					ioentity.FindContainerSource(CS$<>8__locals1.found, depth - 1, input, validFilter);
				}
			}
		}
	}

	// Token: 0x17000114 RID: 276
	// (get) Token: 0x06000C7B RID: 3195 RVA: 0x000062DD File Offset: 0x000044DD
	protected virtual float LiquidPassthroughGravityThreshold
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x17000115 RID: 277
	// (get) Token: 0x06000C7C RID: 3196 RVA: 0x00007074 File Offset: 0x00005274
	protected virtual bool DisregardGravityRestrictionsOnLiquid
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000C7D RID: 3197 RVA: 0x0006AA90 File Offset: 0x00068C90
	public virtual bool AllowLiquidPassthrough(global::IOEntity fromSource, Vector3 sourceWorldPosition, bool forPlacement = false)
	{
		if (fromSource.DisregardGravityRestrictionsOnLiquid || this.DisregardGravityRestrictionsOnLiquid)
		{
			return true;
		}
		if (this.inputs.Length == 0)
		{
			return false;
		}
		Vector3 vector = base.transform.TransformPoint(this.inputs[0].handlePosition);
		float num = sourceWorldPosition.y - vector.y;
		return num > 0f || Mathf.Abs(num) < this.LiquidPassthroughGravityThreshold;
	}

	// Token: 0x06000C7E RID: 3198 RVA: 0x0006AAFC File Offset: 0x00068CFC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity == null)
		{
			return;
		}
		if (!info.fromDisk && info.msg.ioEntity.inputs != null)
		{
			int count = info.msg.ioEntity.inputs.Count;
			if (this.inputs.Length != count)
			{
				this.inputs = new global::IOEntity.IOSlot[count];
			}
			for (int i = 0; i < count; i++)
			{
				if (this.inputs[i] == null)
				{
					this.inputs[i] = new global::IOEntity.IOSlot();
				}
				ProtoBuf.IOEntity.IOConnection ioconnection = info.msg.ioEntity.inputs[i];
				this.inputs[i].connectedTo = new global::IOEntity.IORef();
				this.inputs[i].connectedTo.entityRef.uid = ioconnection.connectedID;
				if (base.isClient)
				{
					this.inputs[i].connectedTo.InitClient();
				}
				this.inputs[i].connectedToSlot = ioconnection.connectedToSlot;
				this.inputs[i].niceName = ioconnection.niceName;
				this.inputs[i].type = (global::IOEntity.IOType)ioconnection.type;
				this.inputs[i].wireColour = (WireTool.WireColour)ioconnection.colour;
			}
		}
		if (info.msg.ioEntity.outputs != null)
		{
			if (!info.fromDisk && base.isClient)
			{
				global::IOEntity.IOSlot[] array = this.outputs;
				for (int j = 0; j < array.Length; j++)
				{
					array[j].Clear();
				}
			}
			int count2 = info.msg.ioEntity.outputs.Count;
			if (this.outputs.Length != count2 && count2 > 0)
			{
				global::IOEntity.IOSlot[] array2 = this.outputs;
				this.outputs = new global::IOEntity.IOSlot[count2];
				for (int k = 0; k < array2.Length; k++)
				{
					if (k < count2)
					{
						this.outputs[k] = array2[k];
					}
				}
			}
			for (int l = 0; l < count2; l++)
			{
				if (this.outputs[l] == null)
				{
					this.outputs[l] = new global::IOEntity.IOSlot();
				}
				ProtoBuf.IOEntity.IOConnection ioconnection2 = info.msg.ioEntity.outputs[l];
				this.outputs[l].connectedTo = new global::IOEntity.IORef();
				this.outputs[l].connectedTo.entityRef.uid = ioconnection2.connectedID;
				if (base.isClient)
				{
					this.outputs[l].connectedTo.InitClient();
				}
				this.outputs[l].connectedToSlot = ioconnection2.connectedToSlot;
				this.outputs[l].niceName = ioconnection2.niceName;
				this.outputs[l].type = (global::IOEntity.IOType)ioconnection2.type;
				this.outputs[l].wireColour = (WireTool.WireColour)ioconnection2.colour;
				this.outputs[l].worldSpaceLineEndRotation = ioconnection2.worldSpaceRotation;
				if (info.fromDisk || base.isClient)
				{
					List<ProtoBuf.IOEntity.IOConnection.LineVec> linePointList = ioconnection2.linePointList;
					if (this.outputs[l].linePoints == null || this.outputs[l].linePoints.Length != linePointList.Count)
					{
						this.outputs[l].linePoints = new Vector3[linePointList.Count];
					}
					if (this.outputs[l].slackLevels == null || this.outputs[l].slackLevels.Length != linePointList.Count)
					{
						this.outputs[l].slackLevels = new float[linePointList.Count];
					}
					for (int m = 0; m < linePointList.Count; m++)
					{
						this.outputs[l].linePoints[m] = linePointList[m].vec;
						this.outputs[l].slackLevels[m] = linePointList[m].vec.w;
					}
				}
			}
		}
		this.RefreshIndustrialPreventBuilding();
	}

	// Token: 0x06000C7F RID: 3199 RVA: 0x0006AEF4 File Offset: 0x000690F4
	public int GetConnectedInputCount()
	{
		int num = 0;
		global::IOEntity.IOSlot[] array = this.inputs;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].connectedTo.Get(base.isServer) != null)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06000C80 RID: 3200 RVA: 0x0006AF38 File Offset: 0x00069138
	public int GetConnectedOutputCount()
	{
		int num = 0;
		global::IOEntity.IOSlot[] array = this.outputs;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].connectedTo.Get(base.isServer) != null)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06000C81 RID: 3201 RVA: 0x0006AF7C File Offset: 0x0006917C
	public bool HasConnections()
	{
		return this.GetConnectedInputCount() > 0 || this.GetConnectedOutputCount() > 0;
	}

	// Token: 0x06000C82 RID: 3202 RVA: 0x0006AF92 File Offset: 0x00069192
	public override void DestroyShared()
	{
		base.DestroyShared();
		this.ClearIndustrialPreventBuilding();
	}

	// Token: 0x06000C83 RID: 3203 RVA: 0x0006AFA0 File Offset: 0x000691A0
	public void RefreshIndustrialPreventBuilding()
	{
		this.ClearIndustrialPreventBuilding();
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		for (int i = 0; i < this.outputs.Length; i++)
		{
			global::IOEntity.IOSlot ioslot = this.outputs[i];
			if (ioslot.type == global::IOEntity.IOType.Industrial && ioslot.linePoints != null && ioslot.linePoints.Length > 1)
			{
				Vector3 b = localToWorldMatrix.MultiplyPoint3x4(ioslot.linePoints[0]);
				for (int j = 1; j < ioslot.linePoints.Length; j++)
				{
					Vector3 vector = localToWorldMatrix.MultiplyPoint3x4(ioslot.linePoints[j]);
					Vector3 pos = Vector3.Lerp(vector, b, 0.5f);
					float z = Vector3.Distance(vector, b);
					Quaternion rot = Quaternion.LookRotation((vector - b).normalized);
					GameObject gameObject = base.gameManager.CreatePrefab("assets/prefabs/misc/ioentitypreventbuilding.prefab", pos, rot, true);
					gameObject.transform.SetParent(base.transform);
					BoxCollider boxCollider;
					if (gameObject.TryGetComponent<BoxCollider>(out boxCollider))
					{
						boxCollider.size = new Vector3(0.1f, 0.1f, z);
						this.spawnedColliders.Add(boxCollider);
					}
					ColliderInfo_Pipe colliderInfo_Pipe;
					if (gameObject.TryGetComponent<ColliderInfo_Pipe>(out colliderInfo_Pipe))
					{
						colliderInfo_Pipe.OutputSlotIndex = i;
						colliderInfo_Pipe.ParentEntity = this;
					}
					b = vector;
				}
			}
		}
	}

	// Token: 0x06000C84 RID: 3204 RVA: 0x0006B0E8 File Offset: 0x000692E8
	private void ClearIndustrialPreventBuilding()
	{
		foreach (BoxCollider boxCollider in this.spawnedColliders)
		{
			base.gameManager.Retire(boxCollider.gameObject);
		}
		this.spawnedColliders.Clear();
	}

	// Token: 0x06000C87 RID: 3207 RVA: 0x0006B1A4 File Offset: 0x000693A4
	[CompilerGenerated]
	internal static int <FindContainerSource>g__GetExistingCount|83_0(IIndustrialStorage storage, ref global::IOEntity.<>c__DisplayClass83_0 A_1)
	{
		int num = 0;
		using (List<global::IOEntity.ContainerInputOutput>.Enumerator enumerator = A_1.found.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Storage == storage)
				{
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x040007DF RID: 2015
	[Header("IOEntity")]
	public Transform debugOrigin;

	// Token: 0x040007E0 RID: 2016
	public ItemDefinition sourceItem;

	// Token: 0x040007E1 RID: 2017
	[NonSerialized]
	public int lastResetIndex;

	// Token: 0x040007E2 RID: 2018
	[ServerVar]
	[Help("How many miliseconds to budget for processing io entities per server frame")]
	public static float framebudgetms = 1f;

	// Token: 0x040007E3 RID: 2019
	[ServerVar]
	public static float responsetime = 0.1f;

	// Token: 0x040007E4 RID: 2020
	[ServerVar]
	public static int backtracking = 8;

	// Token: 0x040007E5 RID: 2021
	[ServerVar(Help = "Print out what is taking so long in the IO frame budget")]
	public static bool debugBudget = false;

	// Token: 0x040007E6 RID: 2022
	[ServerVar(Help = "Ignore frames with a lower ms than this while debugBudget is active")]
	public static float debugBudgetThreshold = 2f;

	// Token: 0x040007E7 RID: 2023
	public const global::BaseEntity.Flags Flag_ShortCircuit = global::BaseEntity.Flags.Reserved7;

	// Token: 0x040007E8 RID: 2024
	public const global::BaseEntity.Flags Flag_HasPower = global::BaseEntity.Flags.Reserved8;

	// Token: 0x040007E9 RID: 2025
	public global::IOEntity.IOSlot[] inputs;

	// Token: 0x040007EA RID: 2026
	public global::IOEntity.IOSlot[] outputs;

	// Token: 0x040007EB RID: 2027
	public global::IOEntity.IOType ioType;

	// Token: 0x040007EC RID: 2028
	public static Queue<global::IOEntity> _processQueue = new Queue<global::IOEntity>();

	// Token: 0x040007ED RID: 2029
	private static List<global::IOEntity.FrameTiming> timings = new List<global::IOEntity.FrameTiming>();

	// Token: 0x040007EE RID: 2030
	private int cachedOutputsUsed;

	// Token: 0x040007EF RID: 2031
	protected int lastPassthroughEnergy;

	// Token: 0x040007F0 RID: 2032
	private int lastEnergy;

	// Token: 0x040007F1 RID: 2033
	protected int currentEnergy;

	// Token: 0x040007F2 RID: 2034
	protected float lastUpdateTime;

	// Token: 0x040007F3 RID: 2035
	protected int lastUpdateBlockedFrame;

	// Token: 0x040007F4 RID: 2036
	protected bool ensureOutputsUpdated;

	// Token: 0x040007F5 RID: 2037
	public const int MaxContainerSourceCount = 16;

	// Token: 0x040007F6 RID: 2038
	private List<BoxCollider> spawnedColliders = new List<BoxCollider>();

	// Token: 0x02000B8E RID: 2958
	public enum IOType
	{
		// Token: 0x04003EB8 RID: 16056
		Electric,
		// Token: 0x04003EB9 RID: 16057
		Fluidic,
		// Token: 0x04003EBA RID: 16058
		Kinetic,
		// Token: 0x04003EBB RID: 16059
		Generic,
		// Token: 0x04003EBC RID: 16060
		Industrial
	}

	// Token: 0x02000B8F RID: 2959
	[Serializable]
	public class IORef
	{
		// Token: 0x06004AF0 RID: 19184 RVA: 0x0019122C File Offset: 0x0018F42C
		public void Init()
		{
			if (this.ioEnt != null && !this.entityRef.IsValid(true))
			{
				this.entityRef.Set(this.ioEnt);
			}
			if (this.entityRef.IsValid(true))
			{
				this.ioEnt = this.entityRef.Get(true).GetComponent<global::IOEntity>();
			}
		}

		// Token: 0x06004AF1 RID: 19185 RVA: 0x0019128B File Offset: 0x0018F48B
		public void InitClient()
		{
			if (this.entityRef.IsValid(false) && this.ioEnt == null)
			{
				this.ioEnt = this.entityRef.Get(false).GetComponent<global::IOEntity>();
			}
		}

		// Token: 0x06004AF2 RID: 19186 RVA: 0x001912C0 File Offset: 0x0018F4C0
		public global::IOEntity Get(bool isServer = true)
		{
			if (this.ioEnt == null && this.entityRef.IsValid(isServer))
			{
				this.ioEnt = (this.entityRef.Get(isServer) as global::IOEntity);
			}
			return this.ioEnt;
		}

		// Token: 0x06004AF3 RID: 19187 RVA: 0x001912FB File Offset: 0x0018F4FB
		public void Clear()
		{
			this.ioEnt = null;
			this.entityRef.Set(null);
		}

		// Token: 0x06004AF4 RID: 19188 RVA: 0x00191310 File Offset: 0x0018F510
		public void Set(global::IOEntity newIOEnt)
		{
			this.entityRef.Set(newIOEnt);
		}

		// Token: 0x04003EBD RID: 16061
		public EntityRef entityRef;

		// Token: 0x04003EBE RID: 16062
		public global::IOEntity ioEnt;
	}

	// Token: 0x02000B90 RID: 2960
	[Serializable]
	public class IOSlot
	{
		// Token: 0x06004AF6 RID: 19190 RVA: 0x0019131E File Offset: 0x0018F51E
		public void Clear()
		{
			this.connectedTo.Clear();
			this.connectedToSlot = 0;
			this.linePoints = null;
		}

		// Token: 0x04003EBF RID: 16063
		public string niceName;

		// Token: 0x04003EC0 RID: 16064
		public global::IOEntity.IOType type;

		// Token: 0x04003EC1 RID: 16065
		public global::IOEntity.IORef connectedTo;

		// Token: 0x04003EC2 RID: 16066
		public int connectedToSlot;

		// Token: 0x04003EC3 RID: 16067
		public Vector3[] linePoints;

		// Token: 0x04003EC4 RID: 16068
		public float[] slackLevels;

		// Token: 0x04003EC5 RID: 16069
		public Vector3 worldSpaceLineEndRotation;

		// Token: 0x04003EC6 RID: 16070
		public ClientIOLine line;

		// Token: 0x04003EC7 RID: 16071
		public Vector3 handlePosition;

		// Token: 0x04003EC8 RID: 16072
		public Vector3 handleDirection;

		// Token: 0x04003EC9 RID: 16073
		public bool rootConnectionsOnly;

		// Token: 0x04003ECA RID: 16074
		public bool mainPowerSlot;

		// Token: 0x04003ECB RID: 16075
		public WireTool.WireColour wireColour;
	}

	// Token: 0x02000B91 RID: 2961
	private struct FrameTiming
	{
		// Token: 0x04003ECC RID: 16076
		public string PrefabName;

		// Token: 0x04003ECD RID: 16077
		public float Time;
	}

	// Token: 0x02000B92 RID: 2962
	public struct ContainerInputOutput
	{
		// Token: 0x04003ECE RID: 16078
		public IIndustrialStorage Storage;

		// Token: 0x04003ECF RID: 16079
		public int SlotIndex;
	}
}
