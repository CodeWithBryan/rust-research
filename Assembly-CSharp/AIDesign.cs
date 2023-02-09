using System;
using System.Collections.Generic;
using ProtoBuf;

// Token: 0x0200033F RID: 831
public class AIDesign
{
	// Token: 0x1700023B RID: 571
	// (get) Token: 0x06001E36 RID: 7734 RVA: 0x000CCF1D File Offset: 0x000CB11D
	// (set) Token: 0x06001E37 RID: 7735 RVA: 0x000CCF25 File Offset: 0x000CB125
	public AIDesignScope Scope { get; private set; }

	// Token: 0x1700023C RID: 572
	// (get) Token: 0x06001E38 RID: 7736 RVA: 0x000CCF2E File Offset: 0x000CB12E
	// (set) Token: 0x06001E39 RID: 7737 RVA: 0x000CCF36 File Offset: 0x000CB136
	public string Description { get; private set; }

	// Token: 0x06001E3A RID: 7738 RVA: 0x000CCF3F File Offset: 0x000CB13F
	public void SetAvailableStates(List<AIState> states)
	{
		this.AvailableStates = new List<AIState>();
		this.AvailableStates.AddRange(states);
	}

	// Token: 0x06001E3B RID: 7739 RVA: 0x000CCF58 File Offset: 0x000CB158
	public void Load(ProtoBuf.AIDesign design, global::BaseEntity owner)
	{
		this.Scope = (AIDesignScope)design.scope;
		this.DefaultStateContainerID = design.defaultStateContainer;
		this.Description = design.description;
		this.InitStateContainers(design, owner);
	}

	// Token: 0x06001E3C RID: 7740 RVA: 0x000CCF88 File Offset: 0x000CB188
	private void InitStateContainers(ProtoBuf.AIDesign design, global::BaseEntity owner)
	{
		this.stateContainers = new Dictionary<int, global::AIStateContainer>();
		if (design.stateContainers == null)
		{
			return;
		}
		foreach (ProtoBuf.AIStateContainer container in design.stateContainers)
		{
			global::AIStateContainer aistateContainer = new global::AIStateContainer();
			aistateContainer.Init(container, owner);
			this.stateContainers.Add(aistateContainer.ID, aistateContainer);
		}
	}

	// Token: 0x06001E3D RID: 7741 RVA: 0x000CD008 File Offset: 0x000CB208
	public global::AIStateContainer GetDefaultStateContainer()
	{
		return this.GetStateContainerByID(this.DefaultStateContainerID);
	}

	// Token: 0x06001E3E RID: 7742 RVA: 0x000CD016 File Offset: 0x000CB216
	public global::AIStateContainer GetStateContainerByID(int id)
	{
		if (!this.stateContainers.ContainsKey(id))
		{
			return null;
		}
		return this.stateContainers[id];
	}

	// Token: 0x06001E3F RID: 7743 RVA: 0x000CD034 File Offset: 0x000CB234
	public ProtoBuf.AIDesign ToProto(int currentStateID)
	{
		ProtoBuf.AIDesign aidesign = new ProtoBuf.AIDesign();
		aidesign.description = this.Description;
		aidesign.scope = (int)this.Scope;
		aidesign.defaultStateContainer = this.DefaultStateContainerID;
		aidesign.availableStates = new List<int>();
		foreach (AIState item in this.AvailableStates)
		{
			aidesign.availableStates.Add((int)item);
		}
		aidesign.stateContainers = new List<ProtoBuf.AIStateContainer>();
		foreach (global::AIStateContainer aistateContainer in this.stateContainers.Values)
		{
			aidesign.stateContainers.Add(aistateContainer.ToProto());
		}
		aidesign.intialViewStateID = currentStateID;
		return aidesign;
	}

	// Token: 0x040017FC RID: 6140
	public List<AIState> AvailableStates = new List<AIState>();

	// Token: 0x040017FD RID: 6141
	public int DefaultStateContainerID;

	// Token: 0x040017FE RID: 6142
	private Dictionary<int, global::AIStateContainer> stateContainers = new Dictionary<int, global::AIStateContainer>();
}
