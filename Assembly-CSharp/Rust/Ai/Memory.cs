using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000AF9 RID: 2809
	public class Memory
	{
		// Token: 0x0600438D RID: 17293 RVA: 0x00187B80 File Offset: 0x00185D80
		public Memory.SeenInfo Update(BaseEntity entity, float score, Vector3 direction, float dot, float distanceSqr, byte lineOfSight, bool updateLastHurtUsTime, float lastHurtUsTime, out Memory.ExtendedInfo extendedInfo)
		{
			return this.Update(entity, entity.ServerPosition, score, direction, dot, distanceSqr, lineOfSight, updateLastHurtUsTime, lastHurtUsTime, out extendedInfo);
		}

		// Token: 0x0600438E RID: 17294 RVA: 0x00187BA8 File Offset: 0x00185DA8
		public Memory.SeenInfo Update(BaseEntity entity, Vector3 position, float score, Vector3 direction, float dot, float distanceSqr, byte lineOfSight, bool updateLastHurtUsTime, float lastHurtUsTime, out Memory.ExtendedInfo extendedInfo)
		{
			extendedInfo = default(Memory.ExtendedInfo);
			bool flag = false;
			for (int i = 0; i < this.AllExtended.Count; i++)
			{
				if (this.AllExtended[i].Entity == entity)
				{
					Memory.ExtendedInfo extendedInfo2 = this.AllExtended[i];
					extendedInfo2.Direction = direction;
					extendedInfo2.Dot = dot;
					extendedInfo2.DistanceSqr = distanceSqr;
					extendedInfo2.LineOfSight = lineOfSight;
					if (updateLastHurtUsTime)
					{
						extendedInfo2.LastHurtUsTime = lastHurtUsTime;
					}
					this.AllExtended[i] = extendedInfo2;
					extendedInfo = extendedInfo2;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (updateLastHurtUsTime)
				{
					Memory.ExtendedInfo extendedInfo3 = new Memory.ExtendedInfo
					{
						Entity = entity,
						Direction = direction,
						Dot = dot,
						DistanceSqr = distanceSqr,
						LineOfSight = lineOfSight,
						LastHurtUsTime = lastHurtUsTime
					};
					this.AllExtended.Add(extendedInfo3);
					extendedInfo = extendedInfo3;
				}
				else
				{
					Memory.ExtendedInfo extendedInfo4 = new Memory.ExtendedInfo
					{
						Entity = entity,
						Direction = direction,
						Dot = dot,
						DistanceSqr = distanceSqr,
						LineOfSight = lineOfSight
					};
					this.AllExtended.Add(extendedInfo4);
					extendedInfo = extendedInfo4;
				}
			}
			return this.Update(entity, position, score);
		}

		// Token: 0x0600438F RID: 17295 RVA: 0x00187CFF File Offset: 0x00185EFF
		public Memory.SeenInfo Update(BaseEntity ent, float danger = 0f)
		{
			return this.Update(ent, ent.ServerPosition, danger);
		}

		// Token: 0x06004390 RID: 17296 RVA: 0x00187D10 File Offset: 0x00185F10
		public Memory.SeenInfo Update(BaseEntity ent, Vector3 position, float danger = 0f)
		{
			for (int i = 0; i < this.All.Count; i++)
			{
				if (this.All[i].Entity == ent)
				{
					Memory.SeenInfo seenInfo = this.All[i];
					seenInfo.Position = position;
					seenInfo.Timestamp = Time.realtimeSinceStartup;
					seenInfo.Danger += danger;
					this.All[i] = seenInfo;
					return seenInfo;
				}
			}
			Memory.SeenInfo seenInfo2 = new Memory.SeenInfo
			{
				Entity = ent,
				Position = position,
				Timestamp = Time.realtimeSinceStartup,
				Danger = danger
			};
			this.All.Add(seenInfo2);
			this.Visible.Add(ent);
			return seenInfo2;
		}

		// Token: 0x06004391 RID: 17297 RVA: 0x00187DD0 File Offset: 0x00185FD0
		public void AddDanger(Vector3 position, float amount)
		{
			for (int i = 0; i < this.All.Count; i++)
			{
				if (Mathf.Approximately(this.All[i].Position.x, position.x) && Mathf.Approximately(this.All[i].Position.y, position.y) && Mathf.Approximately(this.All[i].Position.z, position.z))
				{
					Memory.SeenInfo value = this.All[i];
					value.Danger = amount;
					this.All[i] = value;
					return;
				}
			}
			this.All.Add(new Memory.SeenInfo
			{
				Position = position,
				Timestamp = Time.realtimeSinceStartup,
				Danger = amount
			});
		}

		// Token: 0x06004392 RID: 17298 RVA: 0x00187EB8 File Offset: 0x001860B8
		public Memory.SeenInfo GetInfo(BaseEntity entity)
		{
			foreach (Memory.SeenInfo seenInfo in this.All)
			{
				if (seenInfo.Entity == entity)
				{
					return seenInfo;
				}
			}
			return default(Memory.SeenInfo);
		}

		// Token: 0x06004393 RID: 17299 RVA: 0x00187F24 File Offset: 0x00186124
		public Memory.SeenInfo GetInfo(Vector3 position)
		{
			foreach (Memory.SeenInfo seenInfo in this.All)
			{
				if ((seenInfo.Position - position).sqrMagnitude < 1f)
				{
					return seenInfo;
				}
			}
			return default(Memory.SeenInfo);
		}

		// Token: 0x06004394 RID: 17300 RVA: 0x00187F9C File Offset: 0x0018619C
		public Memory.ExtendedInfo GetExtendedInfo(BaseEntity entity)
		{
			foreach (Memory.ExtendedInfo extendedInfo in this.AllExtended)
			{
				if (extendedInfo.Entity == entity)
				{
					return extendedInfo;
				}
			}
			return default(Memory.ExtendedInfo);
		}

		// Token: 0x06004395 RID: 17301 RVA: 0x00188008 File Offset: 0x00186208
		internal void Forget(float maxSecondsOld)
		{
			for (int i = 0; i < this.All.Count; i++)
			{
				float num = Time.realtimeSinceStartup - this.All[i].Timestamp;
				if (num > maxSecondsOld)
				{
					if (this.All[i].Entity != null)
					{
						this.Visible.Remove(this.All[i].Entity);
						for (int j = 0; j < this.AllExtended.Count; j++)
						{
							if (this.AllExtended[j].Entity == this.All[i].Entity)
							{
								this.AllExtended.RemoveAt(j);
								break;
							}
						}
					}
					this.All.RemoveAt(i);
					i--;
				}
				else if (num > 0f)
				{
					float num2 = num / maxSecondsOld;
					if (this.All[i].Danger > 0f)
					{
						Memory.SeenInfo value = this.All[i];
						value.Danger -= num2;
						this.All[i] = value;
					}
					if (num >= 1f)
					{
						for (int k = 0; k < this.AllExtended.Count; k++)
						{
							if (this.AllExtended[k].Entity == this.All[i].Entity)
							{
								Memory.ExtendedInfo value2 = this.AllExtended[k];
								value2.LineOfSight = 0;
								this.AllExtended[k] = value2;
								break;
							}
						}
					}
				}
			}
			for (int l = 0; l < this.Visible.Count; l++)
			{
				if (this.Visible[l] == null)
				{
					this.Visible.RemoveAt(l);
					l--;
				}
			}
			for (int m = 0; m < this.AllExtended.Count; m++)
			{
				if (this.AllExtended[m].Entity == null)
				{
					this.AllExtended.RemoveAt(m);
					m--;
				}
			}
		}

		// Token: 0x04003C0F RID: 15375
		public List<BaseEntity> Visible = new List<BaseEntity>();

		// Token: 0x04003C10 RID: 15376
		public List<Memory.SeenInfo> All = new List<Memory.SeenInfo>();

		// Token: 0x04003C11 RID: 15377
		public List<Memory.ExtendedInfo> AllExtended = new List<Memory.ExtendedInfo>();

		// Token: 0x02000F35 RID: 3893
		public struct SeenInfo
		{
			// Token: 0x04004DAB RID: 19883
			public BaseEntity Entity;

			// Token: 0x04004DAC RID: 19884
			public Vector3 Position;

			// Token: 0x04004DAD RID: 19885
			public float Timestamp;

			// Token: 0x04004DAE RID: 19886
			public float Danger;
		}

		// Token: 0x02000F36 RID: 3894
		public struct ExtendedInfo
		{
			// Token: 0x04004DAF RID: 19887
			public BaseEntity Entity;

			// Token: 0x04004DB0 RID: 19888
			public Vector3 Direction;

			// Token: 0x04004DB1 RID: 19889
			public float Dot;

			// Token: 0x04004DB2 RID: 19890
			public float DistanceSqr;

			// Token: 0x04004DB3 RID: 19891
			public byte LineOfSight;

			// Token: 0x04004DB4 RID: 19892
			public float LastHurtUsTime;
		}
	}
}
