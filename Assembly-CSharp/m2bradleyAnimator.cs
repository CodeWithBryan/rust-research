using System;
using UnityEngine;

// Token: 0x020003F4 RID: 1012
public class m2bradleyAnimator : MonoBehaviour
{
	// Token: 0x060021FF RID: 8703 RVA: 0x000D96D8 File Offset: 0x000D78D8
	private void Start()
	{
		this.mainRigidbody = base.GetComponent<Rigidbody>();
		for (int i = 0; i < this.ShocksBones.Length; i++)
		{
			this.vecShocksOffsetPosition[i] = this.ShocksBones[i].localPosition;
		}
	}

	// Token: 0x06002200 RID: 8704 RVA: 0x000D971D File Offset: 0x000D791D
	private void Update()
	{
		this.TrackTurret();
		this.TrackSpotLight();
		this.TrackSideGuns();
		this.AnimateWheelsTreads();
		this.AdjustShocksHeight();
		this.m2Animator.SetBool("rocketpods", this.rocketsOpen);
	}

	// Token: 0x06002201 RID: 8705 RVA: 0x000D9754 File Offset: 0x000D7954
	private void AnimateWheelsTreads()
	{
		float num = 0f;
		if (this.mainRigidbody != null)
		{
			num = Vector3.Dot(this.mainRigidbody.velocity, base.transform.forward);
		}
		float x = Time.time * -1f * num * this.treadConstant % 1f;
		this.treadLeftMaterial.SetTextureOffset("_MainTex", new Vector2(x, 0f));
		this.treadLeftMaterial.SetTextureOffset("_BumpMap", new Vector2(x, 0f));
		this.treadLeftMaterial.SetTextureOffset("_SpecGlossMap", new Vector2(x, 0f));
		this.treadRightMaterial.SetTextureOffset("_MainTex", new Vector2(x, 0f));
		this.treadRightMaterial.SetTextureOffset("_BumpMap", new Vector2(x, 0f));
		this.treadRightMaterial.SetTextureOffset("_SpecGlossMap", new Vector2(x, 0f));
		if (num >= 0f)
		{
			this.wheelAngle = (this.wheelAngle + Time.deltaTime * num * this.wheelSpinConstant) % 360f;
		}
		else
		{
			this.wheelAngle += Time.deltaTime * num * this.wheelSpinConstant;
			if (this.wheelAngle <= 0f)
			{
				this.wheelAngle = 360f;
			}
		}
		this.m2Animator.SetFloat("wheel_spin", this.wheelAngle);
		this.m2Animator.SetFloat("speed", num);
	}

	// Token: 0x06002202 RID: 8706 RVA: 0x000D98D4 File Offset: 0x000D7AD4
	private void AdjustShocksHeight()
	{
		Ray ray = default(Ray);
		int mask = LayerMask.GetMask(new string[]
		{
			"Terrain",
			"World",
			"Construction"
		});
		int num = this.ShocksBones.Length;
		float num2 = 0.55f;
		float maxDistance = 0.79f;
		for (int i = 0; i < num; i++)
		{
			ray.origin = this.ShockTraceLineBegin[i].position;
			ray.direction = base.transform.up * -1f;
			RaycastHit raycastHit;
			float num3;
			if (Physics.SphereCast(ray, 0.15f, out raycastHit, maxDistance, mask))
			{
				num3 = raycastHit.distance - num2;
			}
			else
			{
				num3 = 0.26f;
			}
			this.vecShocksOffsetPosition[i].y = Mathf.Lerp(this.vecShocksOffsetPosition[i].y, Mathf.Clamp(num3 * -1f, -0.26f, 0f), Time.deltaTime * 5f);
			this.ShocksBones[i].localPosition = this.vecShocksOffsetPosition[i];
		}
	}

	// Token: 0x06002203 RID: 8707 RVA: 0x000D9A00 File Offset: 0x000D7C00
	private void TrackTurret()
	{
		if (this.targetTurret != null)
		{
			Vector3 normalized = (this.targetTurret.position - this.turret.position).normalized;
			float num;
			float num2;
			this.CalculateYawPitchOffset(this.turret, this.turret.position, this.targetTurret.position, out num, out num2);
			num = this.NormalizeYaw(num);
			float num3 = Time.deltaTime * this.turretTurnSpeed;
			if (num < -0.5f)
			{
				this.vecTurret.y = (this.vecTurret.y - num3) % 360f;
			}
			else if (num > 0.5f)
			{
				this.vecTurret.y = (this.vecTurret.y + num3) % 360f;
			}
			this.turret.localEulerAngles = this.vecTurret;
			float num4 = Time.deltaTime * this.cannonPitchSpeed;
			this.CalculateYawPitchOffset(this.mainCannon, this.mainCannon.position, this.targetTurret.position, out num, out num2);
			if (num2 < -0.5f)
			{
				this.vecMainCannon.x = this.vecMainCannon.x - num4;
			}
			else if (num2 > 0.5f)
			{
				this.vecMainCannon.x = this.vecMainCannon.x + num4;
			}
			this.vecMainCannon.x = Mathf.Clamp(this.vecMainCannon.x, -55f, 5f);
			this.mainCannon.localEulerAngles = this.vecMainCannon;
			if (num2 < -0.5f)
			{
				this.vecCoaxGun.x = this.vecCoaxGun.x - num4;
			}
			else if (num2 > 0.5f)
			{
				this.vecCoaxGun.x = this.vecCoaxGun.x + num4;
			}
			this.vecCoaxGun.x = Mathf.Clamp(this.vecCoaxGun.x, -65f, 15f);
			this.coaxGun.localEulerAngles = this.vecCoaxGun;
			if (this.rocketsOpen)
			{
				num4 = Time.deltaTime * this.rocketPitchSpeed;
				this.CalculateYawPitchOffset(this.rocketsPitch, this.rocketsPitch.position, this.targetTurret.position, out num, out num2);
				if (num2 < -0.5f)
				{
					this.vecRocketsPitch.x = this.vecRocketsPitch.x - num4;
				}
				else if (num2 > 0.5f)
				{
					this.vecRocketsPitch.x = this.vecRocketsPitch.x + num4;
				}
				this.vecRocketsPitch.x = Mathf.Clamp(this.vecRocketsPitch.x, -45f, 45f);
			}
			else
			{
				this.vecRocketsPitch.x = Mathf.Lerp(this.vecRocketsPitch.x, 0f, Time.deltaTime * 1.7f);
			}
			this.rocketsPitch.localEulerAngles = this.vecRocketsPitch;
		}
	}

	// Token: 0x06002204 RID: 8708 RVA: 0x000D9CD4 File Offset: 0x000D7ED4
	private void TrackSpotLight()
	{
		if (this.targetSpotLight != null)
		{
			Vector3 normalized = (this.targetSpotLight.position - this.spotLightYaw.position).normalized;
			float num;
			float num2;
			this.CalculateYawPitchOffset(this.spotLightYaw, this.spotLightYaw.position, this.targetSpotLight.position, out num, out num2);
			num = this.NormalizeYaw(num);
			float num3 = Time.deltaTime * this.spotLightTurnSpeed;
			if (num < -0.5f)
			{
				this.vecSpotLightBase.y = (this.vecSpotLightBase.y - num3) % 360f;
			}
			else if (num > 0.5f)
			{
				this.vecSpotLightBase.y = (this.vecSpotLightBase.y + num3) % 360f;
			}
			this.spotLightYaw.localEulerAngles = this.vecSpotLightBase;
			this.CalculateYawPitchOffset(this.spotLightPitch, this.spotLightPitch.position, this.targetSpotLight.position, out num, out num2);
			if (num2 < -0.5f)
			{
				this.vecSpotLight.x = this.vecSpotLight.x - num3;
			}
			else if (num2 > 0.5f)
			{
				this.vecSpotLight.x = this.vecSpotLight.x + num3;
			}
			this.vecSpotLight.x = Mathf.Clamp(this.vecSpotLight.x, -50f, 50f);
			this.spotLightPitch.localEulerAngles = this.vecSpotLight;
			this.m2Animator.SetFloat("sideMG_pitch", this.vecSpotLight.x, 0.5f, Time.deltaTime);
		}
	}

	// Token: 0x06002205 RID: 8709 RVA: 0x000D9E64 File Offset: 0x000D8064
	private void TrackSideGuns()
	{
		for (int i = 0; i < this.sideguns.Length; i++)
		{
			if (!(this.targetSideguns[i] == null))
			{
				Vector3 normalized = (this.targetSideguns[i].position - this.sideguns[i].position).normalized;
				float num;
				float num2;
				this.CalculateYawPitchOffset(this.sideguns[i], this.sideguns[i].position, this.targetSideguns[i].position, out num, out num2);
				num = this.NormalizeYaw(num);
				float num3 = Time.deltaTime * this.sidegunsTurnSpeed;
				if (num < -0.5f)
				{
					Vector3[] array = this.vecSideGunRotation;
					int num4 = i;
					array[num4].y = array[num4].y - num3;
				}
				else if (num > 0.5f)
				{
					Vector3[] array2 = this.vecSideGunRotation;
					int num5 = i;
					array2[num5].y = array2[num5].y + num3;
				}
				if (num2 < -0.5f)
				{
					Vector3[] array3 = this.vecSideGunRotation;
					int num6 = i;
					array3[num6].x = array3[num6].x - num3;
				}
				else if (num2 > 0.5f)
				{
					Vector3[] array4 = this.vecSideGunRotation;
					int num7 = i;
					array4[num7].x = array4[num7].x + num3;
				}
				this.vecSideGunRotation[i].x = Mathf.Clamp(this.vecSideGunRotation[i].x, -45f, 45f);
				this.vecSideGunRotation[i].y = Mathf.Clamp(this.vecSideGunRotation[i].y, -45f, 45f);
				this.sideguns[i].localEulerAngles = this.vecSideGunRotation[i];
			}
		}
	}

	// Token: 0x06002206 RID: 8710 RVA: 0x000DA000 File Offset: 0x000D8200
	public void CalculateYawPitchOffset(Transform objectTransform, Vector3 vecStart, Vector3 vecEnd, out float yaw, out float pitch)
	{
		Vector3 vector = objectTransform.InverseTransformDirection(vecEnd - vecStart);
		float x = Mathf.Sqrt(vector.x * vector.x + vector.z * vector.z);
		pitch = -Mathf.Atan2(vector.y, x) * 57.295776f;
		vector = (vecEnd - vecStart).normalized;
		Vector3 forward = objectTransform.forward;
		forward.y = 0f;
		forward.Normalize();
		float num = Vector3.Dot(vector, forward);
		float num2 = Vector3.Dot(vector, objectTransform.right);
		float y = 360f * num2;
		float x2 = 360f * -num;
		yaw = (Mathf.Atan2(y, x2) + 3.1415927f) * 57.295776f;
	}

	// Token: 0x06002207 RID: 8711 RVA: 0x000DA0C0 File Offset: 0x000D82C0
	public float NormalizeYaw(float flYaw)
	{
		float result;
		if (flYaw > 180f)
		{
			result = 360f - flYaw;
		}
		else
		{
			result = flYaw * -1f;
		}
		return result;
	}

	// Token: 0x04001A64 RID: 6756
	public Animator m2Animator;

	// Token: 0x04001A65 RID: 6757
	public Material treadLeftMaterial;

	// Token: 0x04001A66 RID: 6758
	public Material treadRightMaterial;

	// Token: 0x04001A67 RID: 6759
	private Rigidbody mainRigidbody;

	// Token: 0x04001A68 RID: 6760
	[Header("GunBones")]
	public Transform turret;

	// Token: 0x04001A69 RID: 6761
	public Transform mainCannon;

	// Token: 0x04001A6A RID: 6762
	public Transform coaxGun;

	// Token: 0x04001A6B RID: 6763
	public Transform rocketsPitch;

	// Token: 0x04001A6C RID: 6764
	public Transform spotLightYaw;

	// Token: 0x04001A6D RID: 6765
	public Transform spotLightPitch;

	// Token: 0x04001A6E RID: 6766
	public Transform sideMG;

	// Token: 0x04001A6F RID: 6767
	public Transform[] sideguns;

	// Token: 0x04001A70 RID: 6768
	[Header("WheelBones")]
	public Transform[] ShocksBones;

	// Token: 0x04001A71 RID: 6769
	public Transform[] ShockTraceLineBegin;

	// Token: 0x04001A72 RID: 6770
	public Vector3[] vecShocksOffsetPosition;

	// Token: 0x04001A73 RID: 6771
	[Header("Targeting")]
	public Transform targetTurret;

	// Token: 0x04001A74 RID: 6772
	public Transform targetSpotLight;

	// Token: 0x04001A75 RID: 6773
	public Transform[] targetSideguns;

	// Token: 0x04001A76 RID: 6774
	private Vector3 vecTurret = new Vector3(0f, 0f, 0f);

	// Token: 0x04001A77 RID: 6775
	private Vector3 vecMainCannon = new Vector3(0f, 0f, 0f);

	// Token: 0x04001A78 RID: 6776
	private Vector3 vecCoaxGun = new Vector3(0f, 0f, 0f);

	// Token: 0x04001A79 RID: 6777
	private Vector3 vecRocketsPitch = new Vector3(0f, 0f, 0f);

	// Token: 0x04001A7A RID: 6778
	private Vector3 vecSpotLightBase = new Vector3(0f, 0f, 0f);

	// Token: 0x04001A7B RID: 6779
	private Vector3 vecSpotLight = new Vector3(0f, 0f, 0f);

	// Token: 0x04001A7C RID: 6780
	private float sideMGPitchValue;

	// Token: 0x04001A7D RID: 6781
	[Header("MuzzleFlash locations")]
	public GameObject muzzleflashCannon;

	// Token: 0x04001A7E RID: 6782
	public GameObject muzzleflashCoaxGun;

	// Token: 0x04001A7F RID: 6783
	public GameObject muzzleflashSideMG;

	// Token: 0x04001A80 RID: 6784
	public GameObject[] muzzleflashRockets;

	// Token: 0x04001A81 RID: 6785
	public GameObject spotLightHaloSawnpoint;

	// Token: 0x04001A82 RID: 6786
	public GameObject[] muzzleflashSideguns;

	// Token: 0x04001A83 RID: 6787
	[Header("MuzzleFlash Particle Systems")]
	public GameObjectRef machineGunMuzzleFlashFX;

	// Token: 0x04001A84 RID: 6788
	public GameObjectRef mainCannonFireFX;

	// Token: 0x04001A85 RID: 6789
	public GameObjectRef rocketLaunchFX;

	// Token: 0x04001A86 RID: 6790
	[Header("Misc")]
	public bool rocketsOpen;

	// Token: 0x04001A87 RID: 6791
	public Vector3[] vecSideGunRotation;

	// Token: 0x04001A88 RID: 6792
	public float treadConstant = 0.14f;

	// Token: 0x04001A89 RID: 6793
	public float wheelSpinConstant = 80f;

	// Token: 0x04001A8A RID: 6794
	[Header("Gun Movement speeds")]
	public float sidegunsTurnSpeed = 30f;

	// Token: 0x04001A8B RID: 6795
	public float turretTurnSpeed = 6f;

	// Token: 0x04001A8C RID: 6796
	public float cannonPitchSpeed = 10f;

	// Token: 0x04001A8D RID: 6797
	public float rocketPitchSpeed = 20f;

	// Token: 0x04001A8E RID: 6798
	public float spotLightTurnSpeed = 60f;

	// Token: 0x04001A8F RID: 6799
	public float machineGunSpeed = 20f;

	// Token: 0x04001A90 RID: 6800
	private float wheelAngle;
}
