using System;
using UnityEngine;

// Token: 0x02000420 RID: 1056
public class PlayerEyes : EntityComponent<BasePlayer>
{
	// Token: 0x170002A9 RID: 681
	// (get) Token: 0x06002319 RID: 8985 RVA: 0x000DF828 File Offset: 0x000DDA28
	public Vector3 worldMountedPosition
	{
		get
		{
			if (base.baseEntity && base.baseEntity.isMounted)
			{
				Vector3 vector = base.baseEntity.GetMounted().EyePositionForPlayer(base.baseEntity, this.GetLookRotation());
				if (vector != Vector3.zero)
				{
					return vector;
				}
			}
			return this.worldStandingPosition;
		}
	}

	// Token: 0x170002AA RID: 682
	// (get) Token: 0x0600231A RID: 8986 RVA: 0x000DF881 File Offset: 0x000DDA81
	public Vector3 worldStandingPosition
	{
		get
		{
			return base.transform.position + PlayerEyes.EyeOffset;
		}
	}

	// Token: 0x170002AB RID: 683
	// (get) Token: 0x0600231B RID: 8987 RVA: 0x000DF898 File Offset: 0x000DDA98
	public Vector3 worldCrouchedPosition
	{
		get
		{
			return this.worldStandingPosition + PlayerEyes.DuckOffset;
		}
	}

	// Token: 0x170002AC RID: 684
	// (get) Token: 0x0600231C RID: 8988 RVA: 0x000DF8AA File Offset: 0x000DDAAA
	public Vector3 worldCrawlingPosition
	{
		get
		{
			return this.worldStandingPosition + PlayerEyes.CrawlOffset;
		}
	}

	// Token: 0x170002AD RID: 685
	// (get) Token: 0x0600231D RID: 8989 RVA: 0x000DF8BC File Offset: 0x000DDABC
	public Vector3 position
	{
		get
		{
			if (!base.baseEntity || !base.baseEntity.isMounted)
			{
				return base.transform.position + base.transform.rotation * (PlayerEyes.EyeOffset + this.viewOffset) + this.BodyLeanOffset;
			}
			Vector3 vector = base.baseEntity.GetMounted().EyePositionForPlayer(base.baseEntity, this.GetLookRotation());
			if (vector != Vector3.zero)
			{
				return vector;
			}
			return base.transform.position + base.transform.up * (PlayerEyes.EyeOffset.y + this.viewOffset.y) + this.BodyLeanOffset;
		}
	}

	// Token: 0x170002AE RID: 686
	// (get) Token: 0x0600231E RID: 8990 RVA: 0x00029180 File Offset: 0x00027380
	private Vector3 BodyLeanOffset
	{
		get
		{
			return Vector3.zero;
		}
	}

	// Token: 0x170002AF RID: 687
	// (get) Token: 0x0600231F RID: 8991 RVA: 0x000DF98C File Offset: 0x000DDB8C
	public Vector3 center
	{
		get
		{
			if (base.baseEntity && base.baseEntity.isMounted)
			{
				Vector3 vector = base.baseEntity.GetMounted().EyeCenterForPlayer(base.baseEntity, this.GetLookRotation());
				if (vector != Vector3.zero)
				{
					return vector;
				}
			}
			return base.transform.position + base.transform.up * (PlayerEyes.EyeOffset.y + PlayerEyes.DuckOffset.y);
		}
	}

	// Token: 0x170002B0 RID: 688
	// (get) Token: 0x06002320 RID: 8992 RVA: 0x000DFA14 File Offset: 0x000DDC14
	public Vector3 offset
	{
		get
		{
			return base.transform.up * (PlayerEyes.EyeOffset.y + this.viewOffset.y);
		}
	}

	// Token: 0x170002B1 RID: 689
	// (get) Token: 0x06002321 RID: 8993 RVA: 0x000DFA3C File Offset: 0x000DDC3C
	// (set) Token: 0x06002322 RID: 8994 RVA: 0x000DFA4F File Offset: 0x000DDC4F
	public Quaternion rotation
	{
		get
		{
			return this.parentRotation * this.bodyRotation;
		}
		set
		{
			this.bodyRotation = Quaternion.Inverse(this.parentRotation) * value;
		}
	}

	// Token: 0x170002B2 RID: 690
	// (get) Token: 0x06002323 RID: 8995 RVA: 0x000DFA68 File Offset: 0x000DDC68
	// (set) Token: 0x06002324 RID: 8996 RVA: 0x000DFA70 File Offset: 0x000DDC70
	public Quaternion bodyRotation { get; set; }

	// Token: 0x170002B3 RID: 691
	// (get) Token: 0x06002325 RID: 8997 RVA: 0x000DFA7C File Offset: 0x000DDC7C
	public Quaternion parentRotation
	{
		get
		{
			if (base.baseEntity.isMounted || !(base.transform.parent != null))
			{
				return Quaternion.identity;
			}
			return Quaternion.Euler(0f, base.transform.parent.rotation.eulerAngles.y, 0f);
		}
	}

	// Token: 0x06002326 RID: 8998 RVA: 0x000DFADC File Offset: 0x000DDCDC
	public void NetworkUpdate(Quaternion rot)
	{
		if (base.baseEntity.IsCrawling())
		{
			this.viewOffset = PlayerEyes.CrawlOffset;
		}
		else if (base.baseEntity.IsDucked())
		{
			this.viewOffset = PlayerEyes.DuckOffset;
		}
		else
		{
			this.viewOffset = Vector3.zero;
		}
		this.bodyRotation = rot;
	}

	// Token: 0x06002327 RID: 8999 RVA: 0x000DFB30 File Offset: 0x000DDD30
	public Vector3 MovementForward()
	{
		return Quaternion.Euler(new Vector3(0f, this.rotation.eulerAngles.y, 0f)) * Vector3.forward;
	}

	// Token: 0x06002328 RID: 9000 RVA: 0x000DFB70 File Offset: 0x000DDD70
	public Vector3 MovementRight()
	{
		return Quaternion.Euler(new Vector3(0f, this.rotation.eulerAngles.y, 0f)) * Vector3.right;
	}

	// Token: 0x06002329 RID: 9001 RVA: 0x000DFBAE File Offset: 0x000DDDAE
	public Ray BodyRay()
	{
		return new Ray(this.position, this.BodyForward());
	}

	// Token: 0x0600232A RID: 9002 RVA: 0x000DFBC1 File Offset: 0x000DDDC1
	public Vector3 BodyForward()
	{
		return this.rotation * Vector3.forward;
	}

	// Token: 0x0600232B RID: 9003 RVA: 0x000DFBD3 File Offset: 0x000DDDD3
	public Vector3 BodyRight()
	{
		return this.rotation * Vector3.right;
	}

	// Token: 0x0600232C RID: 9004 RVA: 0x000DFBE5 File Offset: 0x000DDDE5
	public Vector3 BodyUp()
	{
		return this.rotation * Vector3.up;
	}

	// Token: 0x0600232D RID: 9005 RVA: 0x000DFBF7 File Offset: 0x000DDDF7
	public Ray HeadRay()
	{
		return new Ray(this.position, this.HeadForward());
	}

	// Token: 0x0600232E RID: 9006 RVA: 0x000DFC0A File Offset: 0x000DDE0A
	public Vector3 HeadForward()
	{
		return this.GetLookRotation() * Vector3.forward;
	}

	// Token: 0x0600232F RID: 9007 RVA: 0x000DFC1C File Offset: 0x000DDE1C
	public Vector3 HeadRight()
	{
		return this.GetLookRotation() * Vector3.right;
	}

	// Token: 0x06002330 RID: 9008 RVA: 0x000DFC2E File Offset: 0x000DDE2E
	public Vector3 HeadUp()
	{
		return this.GetLookRotation() * Vector3.up;
	}

	// Token: 0x06002331 RID: 9009 RVA: 0x000DFC40 File Offset: 0x000DDE40
	public Quaternion GetLookRotation()
	{
		return this.rotation;
	}

	// Token: 0x06002332 RID: 9010 RVA: 0x000DFC40 File Offset: 0x000DDE40
	public Quaternion GetAimRotation()
	{
		return this.rotation;
	}

	// Token: 0x04001B97 RID: 7063
	public static readonly Vector3 EyeOffset = new Vector3(0f, 1.5f, 0f);

	// Token: 0x04001B98 RID: 7064
	public static readonly Vector3 DuckOffset = new Vector3(0f, -0.6f, 0f);

	// Token: 0x04001B99 RID: 7065
	public static readonly Vector3 CrawlOffset = new Vector3(0f, -1.15f, 0.175f);

	// Token: 0x04001B9A RID: 7066
	public Vector3 thirdPersonSleepingOffset = new Vector3(0.43f, 1.25f, 0.7f);

	// Token: 0x04001B9B RID: 7067
	public LazyAimProperties defaultLazyAim;

	// Token: 0x04001B9C RID: 7068
	private Vector3 viewOffset = Vector3.zero;
}
