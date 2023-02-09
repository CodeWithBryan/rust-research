using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020002C3 RID: 707
public class Ragdoll : BaseMonoBehaviour, IPrefabPreProcess
{
	// Token: 0x06001CAC RID: 7340 RVA: 0x000C4C28 File Offset: 0x000C2E28
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (!clientside)
		{
			return;
		}
		this.joints.Clear();
		this.characterJoints.Clear();
		this.configurableJoints.Clear();
		this.rigidbodies.Clear();
		base.GetComponentsInChildren<Joint>(true, this.joints);
		base.GetComponentsInChildren<CharacterJoint>(true, this.characterJoints);
		base.GetComponentsInChildren<ConfigurableJoint>(true, this.configurableJoints);
		base.GetComponentsInChildren<Rigidbody>(true, this.rigidbodies);
	}

	// Token: 0x0400163C RID: 5692
	public Transform eyeTransform;

	// Token: 0x0400163D RID: 5693
	public Transform centerBone;

	// Token: 0x0400163E RID: 5694
	public Rigidbody primaryBody;

	// Token: 0x0400163F RID: 5695
	public PhysicMaterial physicMaterial;

	// Token: 0x04001640 RID: 5696
	public SpringJoint corpseJoint;

	// Token: 0x04001641 RID: 5697
	public Skeleton skeleton;

	// Token: 0x04001642 RID: 5698
	public Model model;

	// Token: 0x04001643 RID: 5699
	public List<Joint> joints = new List<Joint>();

	// Token: 0x04001644 RID: 5700
	public List<CharacterJoint> characterJoints = new List<CharacterJoint>();

	// Token: 0x04001645 RID: 5701
	public List<ConfigurableJoint> configurableJoints = new List<ConfigurableJoint>();

	// Token: 0x04001646 RID: 5702
	public List<Rigidbody> rigidbodies = new List<Rigidbody>();

	// Token: 0x04001647 RID: 5703
	public GameObject GibEffect;
}
