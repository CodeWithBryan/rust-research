using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

// Token: 0x02000551 RID: 1361
[CreateAssetMenu(fileName = "NewTechTree", menuName = "Rust/Tech Tree", order = 2)]
public class TechTreeData : ScriptableObject
{
	// Token: 0x06002978 RID: 10616 RVA: 0x000FBC90 File Offset: 0x000F9E90
	public TechTreeData.NodeInstance GetByID(int id)
	{
		if (UnityEngine.Application.isPlaying)
		{
			if (this._idToNode == null)
			{
				this._idToNode = this.nodes.ToDictionary((TechTreeData.NodeInstance n) => n.id, (TechTreeData.NodeInstance n) => n);
			}
			TechTreeData.NodeInstance result;
			this._idToNode.TryGetValue(id, out result);
			return result;
		}
		this._idToNode = null;
		foreach (TechTreeData.NodeInstance nodeInstance in this.nodes)
		{
			if (nodeInstance.id == id)
			{
				return nodeInstance;
			}
		}
		return null;
	}

	// Token: 0x06002979 RID: 10617 RVA: 0x000FBD64 File Offset: 0x000F9F64
	public TechTreeData.NodeInstance GetEntryNode()
	{
		if (UnityEngine.Application.isPlaying && this._entryNode != null && this._entryNode.groupName == "Entry")
		{
			return this._entryNode;
		}
		this._entryNode = null;
		foreach (TechTreeData.NodeInstance nodeInstance in this.nodes)
		{
			if (nodeInstance.groupName == "Entry")
			{
				this._entryNode = nodeInstance;
				return nodeInstance;
			}
		}
		Debug.LogError("NO ENTRY NODE FOR TECH TREE, This will Fail hard");
		return null;
	}

	// Token: 0x0600297A RID: 10618 RVA: 0x000FBE10 File Offset: 0x000FA010
	public void ClearInputs(TechTreeData.NodeInstance node)
	{
		foreach (int id in node.outputs)
		{
			TechTreeData.NodeInstance byID = this.GetByID(id);
			byID.inputs.Clear();
			this.ClearInputs(byID);
		}
	}

	// Token: 0x0600297B RID: 10619 RVA: 0x000FBE78 File Offset: 0x000FA078
	public void SetupInputs(TechTreeData.NodeInstance node)
	{
		foreach (int id in node.outputs)
		{
			TechTreeData.NodeInstance byID = this.GetByID(id);
			if (!byID.inputs.Contains(node.id))
			{
				byID.inputs.Add(node.id);
			}
			this.SetupInputs(byID);
		}
	}

	// Token: 0x0600297C RID: 10620 RVA: 0x000FBEF8 File Offset: 0x000FA0F8
	public bool PlayerHasPathForUnlock(BasePlayer player, TechTreeData.NodeInstance node)
	{
		TechTreeData.NodeInstance entryNode = this.GetEntryNode();
		return entryNode != null && this.CheckChainRecursive(player, entryNode, node);
	}

	// Token: 0x0600297D RID: 10621 RVA: 0x000FBF1C File Offset: 0x000FA11C
	public bool CheckChainRecursive(BasePlayer player, TechTreeData.NodeInstance start, TechTreeData.NodeInstance target)
	{
		if (start.groupName != "Entry")
		{
			if (start.IsGroup())
			{
				using (List<int>.Enumerator enumerator = start.inputs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						int id = enumerator.Current;
						if (!this.PlayerHasPathForUnlock(player, this.GetByID(id)))
						{
							return false;
						}
					}
					goto IL_69;
				}
			}
			if (!this.HasPlayerUnlocked(player, start))
			{
				return false;
			}
		}
		IL_69:
		bool result = false;
		foreach (int num in start.outputs)
		{
			if (num == target.id)
			{
				return true;
			}
			if (this.CheckChainRecursive(player, this.GetByID(num), target))
			{
				result = true;
			}
		}
		return result;
	}

	// Token: 0x0600297E RID: 10622 RVA: 0x000FC008 File Offset: 0x000FA208
	public bool PlayerCanUnlock(BasePlayer player, TechTreeData.NodeInstance node)
	{
		return this.PlayerHasPathForUnlock(player, node) && !this.HasPlayerUnlocked(player, node);
	}

	// Token: 0x0600297F RID: 10623 RVA: 0x000FC024 File Offset: 0x000FA224
	public bool HasPlayerUnlocked(BasePlayer player, TechTreeData.NodeInstance node)
	{
		if (node.IsGroup())
		{
			bool result = true;
			foreach (int id in node.outputs)
			{
				TechTreeData.NodeInstance byID = this.GetByID(id);
				if (!this.HasPlayerUnlocked(player, byID))
				{
					result = false;
				}
			}
			return result;
		}
		return player.blueprints.HasUnlocked(node.itemDef);
	}

	// Token: 0x06002980 RID: 10624 RVA: 0x000FC0A4 File Offset: 0x000FA2A4
	public void GetNodesRequiredToUnlock(BasePlayer player, TechTreeData.NodeInstance node, List<TechTreeData.NodeInstance> foundNodes)
	{
		foundNodes.Add(node);
		if (node == this.GetEntryNode())
		{
			return;
		}
		if (node.inputs.Count == 1)
		{
			this.GetNodesRequiredToUnlock(player, this.GetByID(node.inputs[0]), foundNodes);
			return;
		}
		List<TechTreeData.NodeInstance> list = Pool.GetList<TechTreeData.NodeInstance>();
		int num = int.MaxValue;
		foreach (int id in node.inputs)
		{
			List<TechTreeData.NodeInstance> list2 = Pool.GetList<TechTreeData.NodeInstance>();
			this.GetNodesRequiredToUnlock(player, this.GetByID(id), list2);
			int num2 = 0;
			foreach (TechTreeData.NodeInstance nodeInstance in list2)
			{
				if (!(nodeInstance.itemDef == null) && !this.HasPlayerUnlocked(player, nodeInstance))
				{
					num2 += ResearchTable.ScrapForResearch(nodeInstance.itemDef, ResearchTable.ResearchType.TechTree);
				}
			}
			if (num2 < num)
			{
				list.Clear();
				list.AddRange(list2);
				num = num2;
			}
			Pool.FreeList<TechTreeData.NodeInstance>(ref list2);
		}
		foundNodes.AddRange(list);
		Pool.FreeList<TechTreeData.NodeInstance>(ref list);
	}

	// Token: 0x0400219E RID: 8606
	public string shortname;

	// Token: 0x0400219F RID: 8607
	public int nextID;

	// Token: 0x040021A0 RID: 8608
	private Dictionary<int, TechTreeData.NodeInstance> _idToNode;

	// Token: 0x040021A1 RID: 8609
	private TechTreeData.NodeInstance _entryNode;

	// Token: 0x040021A2 RID: 8610
	public List<TechTreeData.NodeInstance> nodes = new List<TechTreeData.NodeInstance>();

	// Token: 0x02000D00 RID: 3328
	[Serializable]
	public class NodeInstance
	{
		// Token: 0x06004E01 RID: 19969 RVA: 0x0019A1EA File Offset: 0x001983EA
		public bool IsGroup()
		{
			return this.itemDef == null && this.groupName != "Entry" && !string.IsNullOrEmpty(this.groupName);
		}

		// Token: 0x04004497 RID: 17559
		public int id;

		// Token: 0x04004498 RID: 17560
		public ItemDefinition itemDef;

		// Token: 0x04004499 RID: 17561
		public Vector2 graphPosition;

		// Token: 0x0400449A RID: 17562
		public List<int> outputs = new List<int>();

		// Token: 0x0400449B RID: 17563
		public List<int> inputs = new List<int>();

		// Token: 0x0400449C RID: 17564
		public string groupName;

		// Token: 0x0400449D RID: 17565
		public int costOverride = -1;
	}
}
