using System;
using System.Net;
using ConVar;
using Facepunch;
using Network;
using Rust;
using Rust.Platform.Common;

// Token: 0x0200070D RID: 1805
public class RustPlatformHooks : IPlatformHooks
{
	// Token: 0x170003E8 RID: 1000
	// (get) Token: 0x060031DF RID: 12767 RVA: 0x001321F7 File Offset: 0x001303F7
	public uint SteamAppId
	{
		get
		{
			return Rust.Defines.appID;
		}
	}

	// Token: 0x060031E0 RID: 12768 RVA: 0x001321FE File Offset: 0x001303FE
	public void Abort()
	{
		Rust.Application.Quit();
	}

	// Token: 0x060031E1 RID: 12769 RVA: 0x00132205 File Offset: 0x00130405
	public void OnItemDefinitionsChanged()
	{
		ItemManager.InvalidateWorkshopSkinCache();
	}

	// Token: 0x170003E9 RID: 1001
	// (get) Token: 0x060031E2 RID: 12770 RVA: 0x0013220C File Offset: 0x0013040C
	public ServerParameters? ServerParameters
	{
		get
		{
			if (Network.Net.sv == null)
			{
				return null;
			}
			IPAddress address = null;
			if (!string.IsNullOrEmpty(ConVar.Server.ip))
			{
				address = IPAddress.Parse(ConVar.Server.ip);
			}
			if (ConVar.Server.queryport <= 0 || ConVar.Server.queryport == ConVar.Server.port)
			{
				ConVar.Server.queryport = Math.Max(ConVar.Server.port, RCon.Port) + 1;
			}
			return new ServerParameters?(new ServerParameters("rust", "Rust", 2370.ToString(), ConVar.Server.secure, CommandLine.HasSwitch("-sdrnet"), address, (ushort)Network.Net.sv.port, (ushort)ConVar.Server.queryport));
		}
	}

	// Token: 0x060031E3 RID: 12771 RVA: 0x001322AF File Offset: 0x001304AF
	public void AuthSessionValidated(ulong userId, ulong ownerUserId, AuthResponse response)
	{
		SingletonComponent<ServerMgr>.Instance.OnValidateAuthTicketResponse(userId, ownerUserId, response);
	}

	// Token: 0x04002898 RID: 10392
	public static readonly RustPlatformHooks Instance = new RustPlatformHooks();
}
