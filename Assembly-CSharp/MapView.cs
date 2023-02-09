using System;
using System.Collections.Generic;
using Rust.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x020007C8 RID: 1992
public class MapView : FacepunchBehaviour
{
	// Token: 0x04002C24 RID: 11300
	public RawImage mapImage;

	// Token: 0x04002C25 RID: 11301
	public Image cameraPositon;

	// Token: 0x04002C26 RID: 11302
	public ScrollRectEx scrollRect;

	// Token: 0x04002C27 RID: 11303
	public GameObject monumentMarkerContainer;

	// Token: 0x04002C28 RID: 11304
	public Transform clusterMarkerContainer;

	// Token: 0x04002C29 RID: 11305
	public GameObjectRef monumentMarkerPrefab;

	// Token: 0x04002C2A RID: 11306
	public GameObject missionMarkerContainer;

	// Token: 0x04002C2B RID: 11307
	public GameObjectRef missionMarkerPrefab;

	// Token: 0x04002C2C RID: 11308
	public TeamMemberMapMarker[] teamPositions;

	// Token: 0x04002C2D RID: 11309
	public PointOfInterestMapMarker PointOfInterestMarker;

	// Token: 0x04002C2E RID: 11310
	public PointOfInterestMapMarker LeaderPointOfInterestMarker;

	// Token: 0x04002C2F RID: 11311
	public GameObject PlayerDeathMarker;

	// Token: 0x04002C30 RID: 11312
	public List<SleepingBagMapMarker> SleepingBagMarkers = new List<SleepingBagMapMarker>();

	// Token: 0x04002C31 RID: 11313
	public List<SleepingBagClusterMapMarker> SleepingBagClusters = new List<SleepingBagClusterMapMarker>();

	// Token: 0x04002C32 RID: 11314
	[FormerlySerializedAs("TrainLayer")]
	public RawImage UndergroundLayer;

	// Token: 0x04002C33 RID: 11315
	public bool ShowGrid;

	// Token: 0x04002C34 RID: 11316
	public bool ShowPointOfInterestMarkers;

	// Token: 0x04002C35 RID: 11317
	public bool ShowDeathMarker = true;

	// Token: 0x04002C36 RID: 11318
	public bool ShowSleepingBags = true;

	// Token: 0x04002C37 RID: 11319
	public bool ShowLocalPlayer = true;

	// Token: 0x04002C38 RID: 11320
	public bool ShowTeamMembers = true;

	// Token: 0x04002C39 RID: 11321
	public bool ShowTrainLayer;

	// Token: 0x04002C3A RID: 11322
	public bool ShowMissions;

	// Token: 0x04002C3B RID: 11323
	[FormerlySerializedAs("ShowTrainLayer")]
	public bool ShowUndergroundLayers;

	// Token: 0x04002C3C RID: 11324
	public bool MLRSMarkerMode;

	// Token: 0x04002C3D RID: 11325
	public RustImageButton LockButton;

	// Token: 0x04002C3E RID: 11326
	public RustImageButton OverworldButton;

	// Token: 0x04002C3F RID: 11327
	public RustImageButton TrainButton;

	// Token: 0x04002C40 RID: 11328
	public RustImageButton[] UnderwaterButtons;

	// Token: 0x04002C41 RID: 11329
	public RustImageButton DungeonButton;
}
