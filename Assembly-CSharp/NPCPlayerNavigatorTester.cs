using System;

// Token: 0x020001F1 RID: 497
public class NPCPlayerNavigatorTester : BaseMonoBehaviour
{
	// Token: 0x06001A09 RID: 6665 RVA: 0x000B973C File Offset: 0x000B793C
	private void Update()
	{
		if (this.TargetNode != this.currentNode)
		{
			base.GetComponent<BaseNavigator>().SetDestination(this.TargetNode.Path, this.TargetNode, 0.5f);
			this.currentNode = this.TargetNode;
		}
	}

	// Token: 0x04001254 RID: 4692
	public BasePathNode TargetNode;

	// Token: 0x04001255 RID: 4693
	private BasePathNode currentNode;
}
