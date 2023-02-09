using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

// Token: 0x02000578 RID: 1400
public class Wearable : MonoBehaviour, IItemSetup, IPrefabPreProcess
{
	// Token: 0x06002A53 RID: 10835 RVA: 0x000059DD File Offset: 0x00003BDD
	public void OnItemSetup(Item item)
	{
	}

	// Token: 0x06002A54 RID: 10836 RVA: 0x0010001C File Offset: 0x000FE21C
	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		foreach (LODGroup lodgroup in base.GetComponentsInChildren<LODGroup>(true))
		{
			lodgroup.SetLODs(Wearable.emptyLOD);
			preProcess.RemoveComponent(lodgroup);
		}
	}

	// Token: 0x06002A55 RID: 10837 RVA: 0x00100058 File Offset: 0x000FE258
	public void CacheComponents()
	{
		this.playerModelHairCap = base.GetComponent<PlayerModelHairCap>();
		this.playerModelHair = base.GetComponent<PlayerModelHair>();
		this.wearableReplacementByRace = base.GetComponent<WearableReplacementByRace>();
		this.wearableShadowLod = base.GetComponent<WearableShadowLod>();
		base.GetComponentsInChildren<Renderer>(true, this.renderers);
		base.GetComponentsInChildren<PlayerModelSkin>(true, this.playerModelSkins);
		base.GetComponentsInChildren<BoneRetarget>(true, this.boneRetargets);
		base.GetComponentsInChildren<SkinnedMeshRenderer>(true, this.skinnedRenderers);
		base.GetComponentsInChildren<SkeletonSkin>(true, this.skeletonSkins);
		base.GetComponentsInChildren<ComponentInfo>(true, this.componentInfos);
		this.RenderersLod0 = (from x in this.renderers
		where x.gameObject.name.EndsWith("0")
		select x).ToArray<Renderer>();
		this.RenderersLod1 = (from x in this.renderers
		where x.gameObject.name.EndsWith("1")
		select x).ToArray<Renderer>();
		this.RenderersLod2 = (from x in this.renderers
		where x.gameObject.name.EndsWith("2")
		select x).ToArray<Renderer>();
		this.RenderersLod3 = (from x in this.renderers
		where x.gameObject.name.EndsWith("3")
		select x).ToArray<Renderer>();
		foreach (Renderer renderer in this.renderers)
		{
			renderer.gameObject.AddComponent<ObjectMotionVectorFix>();
			renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
		}
	}

	// Token: 0x06002A56 RID: 10838 RVA: 0x00100208 File Offset: 0x000FE408
	public void StripRig(IPrefabProcessor preProcess, SkinnedMeshRenderer skinnedMeshRenderer)
	{
		if (this.disableRigStripping)
		{
			return;
		}
		Transform transform = skinnedMeshRenderer.FindRig();
		if (transform != null)
		{
			List<Transform> list = Pool.GetList<Transform>();
			transform.GetComponentsInChildren<Transform>(list);
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (preProcess != null)
				{
					preProcess.NominateForDeletion(list[i].gameObject);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(list[i].gameObject);
				}
			}
			Pool.FreeList<Transform>(ref list);
		}
	}

	// Token: 0x06002A57 RID: 10839 RVA: 0x000059DD File Offset: 0x00003BDD
	public void SetupRendererCache(IPrefabProcessor preProcess)
	{
	}

	// Token: 0x04002212 RID: 8722
	[global::InspectorFlags]
	public Wearable.RemoveSkin removeSkin;

	// Token: 0x04002213 RID: 8723
	[global::InspectorFlags]
	public Wearable.RemoveSkin removeSkinFirstPerson;

	// Token: 0x04002214 RID: 8724
	[global::InspectorFlags]
	public Wearable.RemoveHair removeHair;

	// Token: 0x04002215 RID: 8725
	[global::InspectorFlags]
	public Wearable.DeformHair deformHair;

	// Token: 0x04002216 RID: 8726
	[global::InspectorFlags]
	public Wearable.OccupationSlots occupationUnder;

	// Token: 0x04002217 RID: 8727
	[global::InspectorFlags]
	public Wearable.OccupationSlots occupationOver;

	// Token: 0x04002218 RID: 8728
	public bool showCensorshipCube;

	// Token: 0x04002219 RID: 8729
	public bool showCensorshipCubeBreasts;

	// Token: 0x0400221A RID: 8730
	public bool forceHideCensorshipBreasts;

	// Token: 0x0400221B RID: 8731
	public string followBone;

	// Token: 0x0400221C RID: 8732
	public bool disableRigStripping;

	// Token: 0x0400221D RID: 8733
	public bool overrideDownLimit;

	// Token: 0x0400221E RID: 8734
	public float downLimit = 70f;

	// Token: 0x0400221F RID: 8735
	[HideInInspector]
	public PlayerModelHair playerModelHair;

	// Token: 0x04002220 RID: 8736
	[HideInInspector]
	public PlayerModelHairCap playerModelHairCap;

	// Token: 0x04002221 RID: 8737
	[HideInInspector]
	public WearableReplacementByRace wearableReplacementByRace;

	// Token: 0x04002222 RID: 8738
	[HideInInspector]
	public WearableShadowLod wearableShadowLod;

	// Token: 0x04002223 RID: 8739
	[HideInInspector]
	public List<Renderer> renderers = new List<Renderer>();

	// Token: 0x04002224 RID: 8740
	[HideInInspector]
	public List<PlayerModelSkin> playerModelSkins = new List<PlayerModelSkin>();

	// Token: 0x04002225 RID: 8741
	[HideInInspector]
	public List<BoneRetarget> boneRetargets = new List<BoneRetarget>();

	// Token: 0x04002226 RID: 8742
	[HideInInspector]
	public List<SkinnedMeshRenderer> skinnedRenderers = new List<SkinnedMeshRenderer>();

	// Token: 0x04002227 RID: 8743
	[HideInInspector]
	public List<SkeletonSkin> skeletonSkins = new List<SkeletonSkin>();

	// Token: 0x04002228 RID: 8744
	[HideInInspector]
	public List<ComponentInfo> componentInfos = new List<ComponentInfo>();

	// Token: 0x04002229 RID: 8745
	public bool HideInEyesView;

	// Token: 0x0400222A RID: 8746
	[Header("First Person Legs")]
	[Tooltip("If this is true, we'll hide this item in the first person view. Usually done for items that you definitely won't see in first person view, like facemasks and hats.")]
	public bool HideInFirstPerson;

	// Token: 0x0400222B RID: 8747
	[Tooltip("Use this if the clothing item clips into the player view. It'll push the chest legs model backwards.")]
	[Range(0f, 5f)]
	public float ExtraLeanBack;

	// Token: 0x0400222C RID: 8748
	[Tooltip("Enable this to check for BoneRetargets which need to be preserved in first person view")]
	public bool PreserveBones;

	// Token: 0x0400222D RID: 8749
	public Renderer[] RenderersLod0;

	// Token: 0x0400222E RID: 8750
	public Renderer[] RenderersLod1;

	// Token: 0x0400222F RID: 8751
	public Renderer[] RenderersLod2;

	// Token: 0x04002230 RID: 8752
	public Renderer[] RenderersLod3;

	// Token: 0x04002231 RID: 8753
	public Renderer[] SkipInFirstPersonLegs;

	// Token: 0x04002232 RID: 8754
	private static LOD[] emptyLOD = new LOD[1];

	// Token: 0x02000D08 RID: 3336
	[Flags]
	public enum RemoveSkin
	{
		// Token: 0x040044B2 RID: 17586
		Torso = 1,
		// Token: 0x040044B3 RID: 17587
		Feet = 2,
		// Token: 0x040044B4 RID: 17588
		Hands = 4,
		// Token: 0x040044B5 RID: 17589
		Legs = 8,
		// Token: 0x040044B6 RID: 17590
		Head = 16
	}

	// Token: 0x02000D09 RID: 3337
	[Flags]
	public enum RemoveHair
	{
		// Token: 0x040044B8 RID: 17592
		Head = 1,
		// Token: 0x040044B9 RID: 17593
		Eyebrow = 2,
		// Token: 0x040044BA RID: 17594
		Facial = 4,
		// Token: 0x040044BB RID: 17595
		Armpit = 8,
		// Token: 0x040044BC RID: 17596
		Pubic = 16
	}

	// Token: 0x02000D0A RID: 3338
	[Flags]
	public enum DeformHair
	{
		// Token: 0x040044BE RID: 17598
		None = 0,
		// Token: 0x040044BF RID: 17599
		BaseballCap = 1,
		// Token: 0x040044C0 RID: 17600
		BoonieHat = 2,
		// Token: 0x040044C1 RID: 17601
		CandleHat = 3,
		// Token: 0x040044C2 RID: 17602
		MinersHat = 4,
		// Token: 0x040044C3 RID: 17603
		WoodHelmet = 5
	}

	// Token: 0x02000D0B RID: 3339
	[Flags]
	public enum OccupationSlots
	{
		// Token: 0x040044C5 RID: 17605
		HeadTop = 1,
		// Token: 0x040044C6 RID: 17606
		Face = 2,
		// Token: 0x040044C7 RID: 17607
		HeadBack = 4,
		// Token: 0x040044C8 RID: 17608
		TorsoFront = 8,
		// Token: 0x040044C9 RID: 17609
		TorsoBack = 16,
		// Token: 0x040044CA RID: 17610
		LeftShoulder = 32,
		// Token: 0x040044CB RID: 17611
		RightShoulder = 64,
		// Token: 0x040044CC RID: 17612
		LeftArm = 128,
		// Token: 0x040044CD RID: 17613
		RightArm = 256,
		// Token: 0x040044CE RID: 17614
		LeftHand = 512,
		// Token: 0x040044CF RID: 17615
		RightHand = 1024,
		// Token: 0x040044D0 RID: 17616
		Groin = 2048,
		// Token: 0x040044D1 RID: 17617
		Bum = 4096,
		// Token: 0x040044D2 RID: 17618
		LeftKnee = 8192,
		// Token: 0x040044D3 RID: 17619
		RightKnee = 16384,
		// Token: 0x040044D4 RID: 17620
		LeftLeg = 32768,
		// Token: 0x040044D5 RID: 17621
		RightLeg = 65536,
		// Token: 0x040044D6 RID: 17622
		LeftFoot = 131072,
		// Token: 0x040044D7 RID: 17623
		RightFoot = 262144,
		// Token: 0x040044D8 RID: 17624
		Mouth = 524288,
		// Token: 0x040044D9 RID: 17625
		Eyes = 1048576
	}
}
