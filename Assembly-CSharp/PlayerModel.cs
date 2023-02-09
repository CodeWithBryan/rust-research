using System;
using UnityEngine;

// Token: 0x02000422 RID: 1058
public class PlayerModel : ListComponent<PlayerModel>
{
	// Token: 0x06002337 RID: 9015 RVA: 0x000DFCFF File Offset: 0x000DDEFF
	private static Vector3 GetFlat(Vector3 dir)
	{
		dir.y = 0f;
		return dir.normalized;
	}

	// Token: 0x06002338 RID: 9016 RVA: 0x000059DD File Offset: 0x00003BDD
	public static void RebuildAll()
	{
	}

	// Token: 0x170002B4 RID: 692
	// (get) Token: 0x06002339 RID: 9017 RVA: 0x000DFD14 File Offset: 0x000DDF14
	// (set) Token: 0x0600233A RID: 9018 RVA: 0x000DFD1C File Offset: 0x000DDF1C
	public ulong overrideSkinSeed { get; private set; }

	// Token: 0x170002B5 RID: 693
	// (get) Token: 0x0600233B RID: 9019 RVA: 0x000DFD25 File Offset: 0x000DDF25
	public bool IsFemale
	{
		get
		{
			return this.skinType == 1;
		}
	}

	// Token: 0x170002B6 RID: 694
	// (get) Token: 0x0600233C RID: 9020 RVA: 0x000DFD30 File Offset: 0x000DDF30
	public SkinSetCollection SkinSet
	{
		get
		{
			if (!this.IsFemale)
			{
				return this.MaleSkin;
			}
			return this.FemaleSkin;
		}
	}

	// Token: 0x170002B7 RID: 695
	// (get) Token: 0x0600233D RID: 9021 RVA: 0x000DFD47 File Offset: 0x000DDF47
	// (set) Token: 0x0600233E RID: 9022 RVA: 0x000DFD4F File Offset: 0x000DDF4F
	public Quaternion AimAngles { get; set; }

	// Token: 0x170002B8 RID: 696
	// (get) Token: 0x0600233F RID: 9023 RVA: 0x000DFD58 File Offset: 0x000DDF58
	// (set) Token: 0x06002340 RID: 9024 RVA: 0x000DFD60 File Offset: 0x000DDF60
	public Quaternion LookAngles { get; set; }

	// Token: 0x04001BA0 RID: 7072
	public Transform[] Shoulders;

	// Token: 0x04001BA1 RID: 7073
	public Transform[] AdditionalSpineBones;

	// Token: 0x04001BA2 RID: 7074
	protected static int speed = Animator.StringToHash("speed");

	// Token: 0x04001BA3 RID: 7075
	protected static int acceleration = Animator.StringToHash("acceleration");

	// Token: 0x04001BA4 RID: 7076
	protected static int rotationYaw = Animator.StringToHash("rotationYaw");

	// Token: 0x04001BA5 RID: 7077
	protected static int forward = Animator.StringToHash("forward");

	// Token: 0x04001BA6 RID: 7078
	protected static int right = Animator.StringToHash("right");

	// Token: 0x04001BA7 RID: 7079
	protected static int up = Animator.StringToHash("up");

	// Token: 0x04001BA8 RID: 7080
	protected static int ducked = Animator.StringToHash("ducked");

	// Token: 0x04001BA9 RID: 7081
	protected static int grounded = Animator.StringToHash("grounded");

	// Token: 0x04001BAA RID: 7082
	protected static int crawling = Animator.StringToHash("crawling");

	// Token: 0x04001BAB RID: 7083
	protected static int waterlevel = Animator.StringToHash("waterlevel");

	// Token: 0x04001BAC RID: 7084
	protected static int attack = Animator.StringToHash("attack");

	// Token: 0x04001BAD RID: 7085
	protected static int attack_alt = Animator.StringToHash("attack_alt");

	// Token: 0x04001BAE RID: 7086
	protected static int deploy = Animator.StringToHash("deploy");

	// Token: 0x04001BAF RID: 7087
	protected static int reload = Animator.StringToHash("reload");

	// Token: 0x04001BB0 RID: 7088
	protected static int throwWeapon = Animator.StringToHash("throw");

	// Token: 0x04001BB1 RID: 7089
	protected static int holster = Animator.StringToHash("holster");

	// Token: 0x04001BB2 RID: 7090
	protected static int aiming = Animator.StringToHash("aiming");

	// Token: 0x04001BB3 RID: 7091
	protected static int onLadder = Animator.StringToHash("onLadder");

	// Token: 0x04001BB4 RID: 7092
	protected static int posing = Animator.StringToHash("posing");

	// Token: 0x04001BB5 RID: 7093
	protected static int poseType = Animator.StringToHash("poseType");

	// Token: 0x04001BB6 RID: 7094
	protected static int relaxGunPose = Animator.StringToHash("relaxGunPose");

	// Token: 0x04001BB7 RID: 7095
	protected static int vehicle_aim_yaw = Animator.StringToHash("vehicleAimYaw");

	// Token: 0x04001BB8 RID: 7096
	protected static int vehicle_aim_speed = Animator.StringToHash("vehicleAimYawSpeed");

	// Token: 0x04001BB9 RID: 7097
	protected static int onPhone = Animator.StringToHash("onPhone");

	// Token: 0x04001BBA RID: 7098
	protected static int usePoseTransition = Animator.StringToHash("usePoseTransition");

	// Token: 0x04001BBB RID: 7099
	protected static int leftFootIK = Animator.StringToHash("leftFootIK");

	// Token: 0x04001BBC RID: 7100
	protected static int rightFootIK = Animator.StringToHash("rightFootIK");

	// Token: 0x04001BBD RID: 7101
	protected static int vehicleSteering = Animator.StringToHash("vehicleSteering");

	// Token: 0x04001BBE RID: 7102
	protected static int sitReaction = Animator.StringToHash("sitReaction");

	// Token: 0x04001BBF RID: 7103
	protected static int forwardReaction = Animator.StringToHash("forwardReaction");

	// Token: 0x04001BC0 RID: 7104
	protected static int rightReaction = Animator.StringToHash("rightReaction");

	// Token: 0x04001BC1 RID: 7105
	public BoxCollider collision;

	// Token: 0x04001BC2 RID: 7106
	public GameObject censorshipCube;

	// Token: 0x04001BC3 RID: 7107
	public GameObject censorshipCubeBreasts;

	// Token: 0x04001BC4 RID: 7108
	public GameObject jawBone;

	// Token: 0x04001BC5 RID: 7109
	public GameObject neckBone;

	// Token: 0x04001BC6 RID: 7110
	public GameObject headBone;

	// Token: 0x04001BC7 RID: 7111
	public EyeController eyeController;

	// Token: 0x04001BC8 RID: 7112
	public EyeBlink blinkController;

	// Token: 0x04001BC9 RID: 7113
	public Transform[] SpineBones;

	// Token: 0x04001BCA RID: 7114
	public Transform leftFootBone;

	// Token: 0x04001BCB RID: 7115
	public Transform rightFootBone;

	// Token: 0x04001BCC RID: 7116
	public Transform leftHandPropBone;

	// Token: 0x04001BCD RID: 7117
	public Transform rightHandPropBone;

	// Token: 0x04001BCE RID: 7118
	public Vector3 rightHandTarget;

	// Token: 0x04001BCF RID: 7119
	[Header("IK")]
	public Vector3 leftHandTargetPosition;

	// Token: 0x04001BD0 RID: 7120
	public Quaternion leftHandTargetRotation;

	// Token: 0x04001BD1 RID: 7121
	public Vector3 rightHandTargetPosition;

	// Token: 0x04001BD2 RID: 7122
	public Quaternion rightHandTargetRotation;

	// Token: 0x04001BD3 RID: 7123
	public float steeringTargetDegrees;

	// Token: 0x04001BD4 RID: 7124
	public Vector3 rightFootTargetPosition;

	// Token: 0x04001BD5 RID: 7125
	public Quaternion rightFootTargetRotation;

	// Token: 0x04001BD6 RID: 7126
	public Vector3 leftFootTargetPosition;

	// Token: 0x04001BD7 RID: 7127
	public Quaternion leftFootTargetRotation;

	// Token: 0x04001BD8 RID: 7128
	public RuntimeAnimatorController CinematicAnimationController;

	// Token: 0x04001BD9 RID: 7129
	public Avatar DefaultAvatar;

	// Token: 0x04001BDA RID: 7130
	public Avatar CinematicAvatar;

	// Token: 0x04001BDB RID: 7131
	public RuntimeAnimatorController DefaultHoldType;

	// Token: 0x04001BDC RID: 7132
	public RuntimeAnimatorController SleepGesture;

	// Token: 0x04001BDD RID: 7133
	public RuntimeAnimatorController CrawlToIncapacitatedGesture;

	// Token: 0x04001BDE RID: 7134
	public RuntimeAnimatorController StandToIncapacitatedGesture;

	// Token: 0x04001BDF RID: 7135
	[NonSerialized]
	public RuntimeAnimatorController CurrentGesture;

	// Token: 0x04001BE0 RID: 7136
	[Header("Skin")]
	public SkinSetCollection MaleSkin;

	// Token: 0x04001BE1 RID: 7137
	public SkinSetCollection FemaleSkin;

	// Token: 0x04001BE2 RID: 7138
	public SubsurfaceProfile subsurfaceProfile;

	// Token: 0x04001BE3 RID: 7139
	[Header("Parameters")]
	[Range(0f, 1f)]
	public float voiceVolume;

	// Token: 0x04001BE4 RID: 7140
	[Range(0f, 1f)]
	public float skinColor = 1f;

	// Token: 0x04001BE5 RID: 7141
	[Range(0f, 1f)]
	public float skinNumber = 1f;

	// Token: 0x04001BE6 RID: 7142
	[Range(0f, 1f)]
	public float meshNumber;

	// Token: 0x04001BE7 RID: 7143
	[Range(0f, 1f)]
	public float hairNumber;

	// Token: 0x04001BE8 RID: 7144
	[Range(0f, 1f)]
	public int skinType;

	// Token: 0x04001BE9 RID: 7145
	public MovementSounds movementSounds;

	// Token: 0x04001BEA RID: 7146
	public bool showSash;

	// Token: 0x04001BEB RID: 7147
	public int tempPoseType;

	// Token: 0x04001BEC RID: 7148
	public uint underwearSkin;

	// Token: 0x02000C92 RID: 3218
	public enum MountPoses
	{
		// Token: 0x040042F7 RID: 17143
		Chair,
		// Token: 0x040042F8 RID: 17144
		Driving,
		// Token: 0x040042F9 RID: 17145
		Horseback,
		// Token: 0x040042FA RID: 17146
		HeliUnarmed,
		// Token: 0x040042FB RID: 17147
		HeliArmed,
		// Token: 0x040042FC RID: 17148
		HandMotorBoat,
		// Token: 0x040042FD RID: 17149
		MotorBoatPassenger,
		// Token: 0x040042FE RID: 17150
		SitGeneric,
		// Token: 0x040042FF RID: 17151
		SitRaft,
		// Token: 0x04004300 RID: 17152
		StandDrive,
		// Token: 0x04004301 RID: 17153
		SitShootingGeneric,
		// Token: 0x04004302 RID: 17154
		SitMinicopter_Pilot,
		// Token: 0x04004303 RID: 17155
		SitMinicopter_Passenger,
		// Token: 0x04004304 RID: 17156
		ArcadeLeft,
		// Token: 0x04004305 RID: 17157
		ArcadeRight,
		// Token: 0x04004306 RID: 17158
		SitSummer_Ring,
		// Token: 0x04004307 RID: 17159
		SitSummer_BoogieBoard,
		// Token: 0x04004308 RID: 17160
		SitCarPassenger,
		// Token: 0x04004309 RID: 17161
		SitSummer_Chair,
		// Token: 0x0400430A RID: 17162
		SitRaft_NoPaddle,
		// Token: 0x0400430B RID: 17163
		Sit_SecretLab,
		// Token: 0x0400430C RID: 17164
		Sit_Workcart,
		// Token: 0x0400430D RID: 17165
		Sit_Cardgame,
		// Token: 0x0400430E RID: 17166
		Sit_Crane,
		// Token: 0x0400430F RID: 17167
		Sit_Snowmobile_Shooting,
		// Token: 0x04004310 RID: 17168
		Sit_RetroSnowmobile_Shooting,
		// Token: 0x04004311 RID: 17169
		Driving_Snowmobile,
		// Token: 0x04004312 RID: 17170
		ZiplineHold,
		// Token: 0x04004313 RID: 17171
		Sit_Locomotive,
		// Token: 0x04004314 RID: 17172
		Sit_Throne,
		// Token: 0x04004315 RID: 17173
		Standing = 128
	}
}
