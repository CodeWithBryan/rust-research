using System;
using Network;
using Network.Visibility;
using UnityEngine;

// Token: 0x0200031B RID: 795
public static class EffectNetwork
{
	// Token: 0x06001DCB RID: 7627 RVA: 0x000CB1E4 File Offset: 0x000C93E4
	public static void Send(Effect effect)
	{
		if (Net.sv == null)
		{
			return;
		}
		if (!Net.sv.IsConnected())
		{
			return;
		}
		using (TimeWarning.New("EffectNetwork.Send", 0))
		{
			if (!string.IsNullOrEmpty(effect.pooledString))
			{
				effect.pooledstringid = StringPool.Get(effect.pooledString);
			}
			if (effect.pooledstringid == 0U)
			{
				Debug.Log("String ID is 0 - unknown effect " + effect.pooledString);
			}
			else if (effect.broadcast)
			{
				NetWrite netWrite = Net.sv.StartWrite();
				netWrite.PacketID(Message.Type.Effect);
				effect.WriteToStream(netWrite);
				netWrite.Send(new SendInfo(BaseNetworkable.GlobalNetworkGroup.subscribers));
			}
			else
			{
				Group group;
				if (effect.entity > 0U)
				{
					BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(effect.entity) as BaseEntity;
					if (!baseEntity.IsValid())
					{
						return;
					}
					group = baseEntity.net.group;
				}
				else
				{
					group = Net.sv.visibility.GetGroup(effect.worldPos);
				}
				if (group != null)
				{
					NetWrite netWrite2 = Net.sv.StartWrite();
					netWrite2.PacketID(Message.Type.Effect);
					effect.WriteToStream(netWrite2);
					netWrite2.Send(new SendInfo(group.subscribers));
				}
			}
		}
	}

	// Token: 0x06001DCC RID: 7628 RVA: 0x000CB338 File Offset: 0x000C9538
	public static void Send(Effect effect, Connection target)
	{
		effect.pooledstringid = StringPool.Get(effect.pooledString);
		if (effect.pooledstringid == 0U)
		{
			Debug.LogWarning("EffectNetwork.Send - unpooled effect name: " + effect.pooledString);
			return;
		}
		NetWrite netWrite = Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.Effect);
		effect.WriteToStream(netWrite);
		netWrite.Send(new SendInfo(target));
	}
}
