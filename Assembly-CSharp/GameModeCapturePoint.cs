using System;
using Facepunch;
using ProtoBuf;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020004E5 RID: 1253
public class GameModeCapturePoint : global::BaseEntity
{
	// Token: 0x060027D2 RID: 10194 RVA: 0x00027875 File Offset: 0x00025A75
	public bool IsContested()
	{
		return base.HasFlag(global::BaseEntity.Flags.Busy);
	}

	// Token: 0x060027D3 RID: 10195 RVA: 0x000F3DFF File Offset: 0x000F1FFF
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.AssignPoints), 0f, 1f);
	}

	// Token: 0x060027D4 RID: 10196 RVA: 0x000F3E23 File Offset: 0x000F2023
	public void Update()
	{
		if (base.isClient)
		{
			return;
		}
		this.UpdateCaptureAmount();
	}

	// Token: 0x060027D5 RID: 10197 RVA: 0x000F3E34 File Offset: 0x000F2034
	public void AssignPoints()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode == null)
		{
			return;
		}
		if (!activeGameMode.IsMatchActive())
		{
			return;
		}
		if (activeGameMode.IsTeamGame())
		{
			if (this.captureTeam != -1 && this.captureFraction == 1f)
			{
				activeGameMode.ModifyTeamScore(this.captureTeam, this.scorePerSecond);
				return;
			}
		}
		else if (this.capturedPlayer.IsValid(true))
		{
			activeGameMode.ModifyPlayerGameScore(this.capturedPlayer.Get(true).GetComponent<global::BasePlayer>(), "score", this.scorePerSecond);
		}
	}

	// Token: 0x060027D6 RID: 10198 RVA: 0x000F3EBC File Offset: 0x000F20BC
	public void DoCaptureEffect()
	{
		Effect.server.Run(this.progressCompleteEffect.resourcePath, this.computerPoint.position, default(Vector3), null, false);
	}

	// Token: 0x060027D7 RID: 10199 RVA: 0x000F3EF0 File Offset: 0x000F20F0
	public void DoProgressEffect()
	{
		if (Time.time < this.nextBeepTime)
		{
			return;
		}
		Effect.server.Run(this.progressBeepEffect.resourcePath, this.computerPoint.position, default(Vector3), null, false);
		this.nextBeepTime = Time.time + 0.5f;
	}

	// Token: 0x060027D8 RID: 10200 RVA: 0x000F3F44 File Offset: 0x000F2144
	public void UpdateCaptureAmount()
	{
		if (base.isClient)
		{
			return;
		}
		float num = this.captureFraction;
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode == null)
		{
			return;
		}
		if (this.captureTrigger.entityContents == null)
		{
			base.SetFlag(global::BaseEntity.Flags.Busy, false, false, false);
			return;
		}
		if (!activeGameMode.IsMatchActive())
		{
			return;
		}
		if (activeGameMode.IsTeamGame())
		{
			int[] array = new int[activeGameMode.GetNumTeams()];
			foreach (global::BaseEntity baseEntity in this.captureTrigger.entityContents)
			{
				if (!(baseEntity == null) && !baseEntity.isClient)
				{
					global::BasePlayer component = baseEntity.GetComponent<global::BasePlayer>();
					if (!(component == null) && component.IsAlive() && !component.IsNpc && component.gamemodeteam != -1)
					{
						array[component.gamemodeteam]++;
					}
				}
			}
			int num2 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] > 0)
				{
					num2++;
				}
			}
			if (num2 < 2)
			{
				int num3 = -1;
				int num4 = 0;
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j] > num4)
					{
						num4 = array[j];
						num3 = j;
					}
				}
				if (this.captureTeam == -1 && this.captureFraction == 0f)
				{
					this.capturingTeam = num3;
				}
				if (this.captureFraction > 0f && num3 != this.captureTeam && num3 != this.capturingTeam)
				{
					this.captureFraction = Mathf.Clamp01(this.captureFraction - Time.deltaTime / this.timeToCapture);
					if (this.captureFraction == 0f)
					{
						this.captureTeam = -1;
					}
				}
				else if (this.captureTeam == -1 && this.captureFraction < 1f && this.capturingTeam == num3)
				{
					this.DoProgressEffect();
					this.captureFraction = Mathf.Clamp01(this.captureFraction + Time.deltaTime / this.timeToCapture);
					if (this.captureFraction == 1f)
					{
						this.DoCaptureEffect();
						this.captureTeam = num3;
					}
				}
			}
			base.SetFlag(global::BaseEntity.Flags.Busy, num2 > 1, false, true);
		}
		else
		{
			if (!this.capturingPlayer.IsValid(true) && !this.capturedPlayer.IsValid(true))
			{
				this.captureFraction = 0f;
			}
			if (this.captureTrigger.entityContents.Count == 0)
			{
				this.capturingPlayer.Set(null);
			}
			if (this.captureTrigger.entityContents.Count == 1)
			{
				foreach (global::BaseEntity baseEntity2 in this.captureTrigger.entityContents)
				{
					global::BasePlayer component2 = baseEntity2.GetComponent<global::BasePlayer>();
					if (!(component2 == null))
					{
						if (!this.capturedPlayer.IsValid(true) && this.captureFraction == 0f)
						{
							this.capturingPlayer.Set(component2);
						}
						if (this.captureFraction > 0f && component2 != this.capturedPlayer.Get(true) && component2 != this.capturingPlayer.Get(true))
						{
							this.captureFraction = Mathf.Clamp01(this.captureFraction - Time.deltaTime / this.timeToCapture);
							if (this.captureFraction == 0f)
							{
								this.capturedPlayer.Set(null);
								break;
							}
							break;
						}
						else
						{
							if (this.capturedPlayer.Get(true) || this.captureFraction >= 1f || !(this.capturingPlayer.Get(true) == component2))
							{
								break;
							}
							this.DoProgressEffect();
							this.captureFraction = Mathf.Clamp01(this.captureFraction + Time.deltaTime / this.timeToCapture);
							if (this.captureFraction == 1f)
							{
								this.DoCaptureEffect();
								this.capturedPlayer.Set(component2);
								break;
							}
							break;
						}
					}
				}
			}
			base.SetFlag(global::BaseEntity.Flags.Busy, this.captureTrigger.entityContents.Count > 1, false, true);
		}
		if (num != this.captureFraction)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060027D9 RID: 10201 RVA: 0x000F4398 File Offset: 0x000F2598
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericFloat1 = this.captureFraction;
		info.msg.ioEntity.genericInt1 = this.captureTeam;
		info.msg.ioEntity.genericInt2 = this.capturingTeam;
		info.msg.ioEntity.genericEntRef1 = this.capturedPlayer.uid;
		info.msg.ioEntity.genericEntRef2 = this.capturingPlayer.uid;
	}

	// Token: 0x04002008 RID: 8200
	public CapturePointTrigger captureTrigger;

	// Token: 0x04002009 RID: 8201
	public float timeToCapture = 3f;

	// Token: 0x0400200A RID: 8202
	public int scorePerSecond = 1;

	// Token: 0x0400200B RID: 8203
	public string scoreName = "score";

	// Token: 0x0400200C RID: 8204
	private float captureFraction;

	// Token: 0x0400200D RID: 8205
	private int captureTeam = -1;

	// Token: 0x0400200E RID: 8206
	private int capturingTeam = -1;

	// Token: 0x0400200F RID: 8207
	public EntityRef capturingPlayer;

	// Token: 0x04002010 RID: 8208
	public EntityRef capturedPlayer;

	// Token: 0x04002011 RID: 8209
	public const global::BaseEntity.Flags Flag_Contested = global::BaseEntity.Flags.Busy;

	// Token: 0x04002012 RID: 8210
	public RustText capturePointText;

	// Token: 0x04002013 RID: 8211
	public RustText captureOwnerName;

	// Token: 0x04002014 RID: 8212
	public Image captureProgressImage;

	// Token: 0x04002015 RID: 8213
	public GameObjectRef progressBeepEffect;

	// Token: 0x04002016 RID: 8214
	public GameObjectRef progressCompleteEffect;

	// Token: 0x04002017 RID: 8215
	public Transform computerPoint;

	// Token: 0x04002018 RID: 8216
	private float nextBeepTime;
}
